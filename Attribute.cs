using System.IO;

namespace DCSMissionTweaker
{
    class Attribute : Element
    {
        public object value;

        public Attribute(Container parent, string name, object value) : base(parent, name)
        {
            parent.children.Add(this);
            this.value = value;
        }

        internal override void Write(StreamWriter streamWriter, int level)
        {
            WriteLine(streamWriter, Indent(level) + NameToString() + " = " + ToString(value) + ",");
        }
    }
}
