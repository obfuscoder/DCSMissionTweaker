using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace DCSMissionTweaker
{
    class MissionParser
    {
        public static Mission ParseFile(string filePath)
        {
            using (var file = File.OpenRead(filePath))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    if (entry.FullName != "mission") continue;
                    using (var stream = entry.Open())
                    {
                        return MissionParser.Parse(stream);
                    }
                }
            }
            throw ParseException.Create("Could not find mission file in " + filePath);
        }
        public static Mission Parse(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                if (reader.ReadLine() != "mission = ")
                {
                    throw ParseException.Create("Unknown file format!");
                }
                Mission mission = new Mission();
                Container currentElement = mission;
                var endOfContainerRegex = new Regex("^\\},? -- end of \\[(\"?.+?\"?)\\]$");
                var endOfMissionRegex = new Regex("^\\},? -- end of mission$");
                var containerStartRegex = new Regex("^\\[(\"?.+?\"?)\\] =$");
                var stringElementRegex = new Regex("^\\[(\"?.+?\"?)\\] = \"(.*?)\",$");
                var integerElementRegex = new Regex("^\\[(\"?.+?\"?)\\] = (-?\\d+),$");
                var decimalElementRegex = new Regex("^\\[(\"?.+?\"?)\\] = (-?\\d+\\.\\d+),$");
                var booleanElementRegex = new Regex("^\\[(\"?.+?\"?)\\] = (false|true),$");
                for (int lineNumber = 1; !reader.EndOfStream; lineNumber++)
                {
                    var line = reader.ReadLine();
                    line = line.Trim();
                    if (line == "{")
                    {
                        if (!(currentElement is Container))
                        {
                            throw ParseException.Create("'{' unexpected", lineNumber);
                        }
                        continue;
                    }
                    if (line.StartsWith("}"))
                    {
                        if (!(currentElement is Container))
                        {
                            throw ParseException.Create("'}' unexpected", lineNumber);
                        }
                        else
                        {
                            var match = endOfContainerRegex.Match(line);
                            if (match.Success)
                            {
                                if (!currentElement.IsNameMatch(match.Groups[1].Value))
                                {
                                    throw ParseException.Create("End of struct reference name " + match.Groups[1].Value + " does not match current container name " + currentElement.name, lineNumber);
                                }
                                currentElement = currentElement.parent;
                                continue;
                            }
                            if (currentElement is Mission && endOfMissionRegex.IsMatch(line))
                            {
                                return mission;
                            }
                        }
                    }
                    if (containerStartRegex.IsMatch(line))
                    {
                        Match match = containerStartRegex.Match(line);
                        currentElement = new Container(currentElement, match.Groups[1].Value);
                        continue;
                    }
                    if (integerElementRegex.IsMatch(line))
                    {
                        Match match = integerElementRegex.Match(line);
                        var element = new Attribute(currentElement, match.Groups[1].Value, int.Parse(match.Groups[2].Value));
                        continue;
                    }
                    if (stringElementRegex.IsMatch(line))
                    {
                        Match match = stringElementRegex.Match(line);
                        var element = new Attribute(currentElement, match.Groups[1].Value, match.Groups[2].Value);
                        continue;
                    }
                    if (decimalElementRegex.IsMatch(line))
                    {
                        Match match = decimalElementRegex.Match(line);
                        var element = new Attribute(currentElement, match.Groups[1].Value, decimal.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture));
                        continue;
                    }
                    if (booleanElementRegex.IsMatch(line))
                    {
                        Match match = booleanElementRegex.Match(line);
                        var element = new Attribute(currentElement, match.Groups[1].Value, Boolean.Parse(match.Groups[2].Value));
                        continue;
                    }
                    throw ParseException.Create("Cannot parse content " + line, lineNumber);
                }
            }
            throw ParseException.Create("Reached and of mission file unexpectedly.");
        }
    }
}
