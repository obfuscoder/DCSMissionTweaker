using System;
using System.IO;
using System.IO.Compression;

namespace DCSMissionTweaker
{
    class MissionSaver
    {
        public static void Save(string filePath, Mission mission)
        {
            string mpFilePath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "_mp.miz");
            Console.WriteLine("Writing updated mission to " + mpFilePath);
            File.Delete(mpFilePath);
            File.Copy(filePath, mpFilePath);
            using (var file = File.Open(mpFilePath, FileMode.Open))
            using (var zip = new ZipArchive(file, ZipArchiveMode.Update))
            {
                zip.GetEntry("mission").Delete();
                var entry = zip.CreateEntry("mission");
                using (var stream = entry.Open())
                {
                    mission.Write(stream);
                }
            }
        }
    }
}
