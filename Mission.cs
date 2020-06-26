using System;
using System.IO;

namespace DCSMissionTweaker
{
    class Mission : Container
    {
        public Mission() : base(null, null) { }

        public Container SearchPlayerUnit()
        {
            Attribute attribute = SearchAttribute("skill", "Player");
            if (attribute == null)
                throw ParseException.Create("Could not find player unit");
            return attribute.parent;
        }

        internal void Write(Stream stream)
        {
            using (var streamWriter = new StreamWriter(stream))
            {
                Write(streamWriter);
            }
        }

        internal void Write(StreamWriter streamWriter)
        {
            WriteLine(streamWriter, "mission = ");
            WriteLine(streamWriter, "{");
            children.ForEach(e => e.Write(streamWriter, 1));
            WriteLine(streamWriter, "} -- end of mission");
        }
    }
}
