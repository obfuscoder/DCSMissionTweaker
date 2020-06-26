using System.Globalization;
using System.IO;

namespace DCSMissionTweaker
{
    abstract class Element
    {
        public object name;
        public Container parent;

        public Element(string name)
        {
            if (name == null)
            {
                return;
            }
            this.name = ToName(name);

        }

        public bool IsNameMatch(string name)
        {
            object obj = ToName(name);
            return obj.Equals(this.name);
        }

        private object ToName(string name)
        {
            if (name.StartsWith("\"") && name.EndsWith("\""))
            {
                return name.Trim('"');
            }
            else
            {
                return int.Parse(name);
            }
        }

        public Element(Container parent, string name) : this(name)
        {
            this.parent = parent;
        }

        internal abstract void Write(StreamWriter streamWriter, int level);

        protected string NameToString()
        {
            return "[" + ToString(name) + "]";
        }

        protected string ToString(object obj)
        {
            if (obj is string)
                return "\"" + obj + "\"";
            if (obj is decimal)
                return ((decimal) obj).ToString(CultureInfo.InvariantCulture);
            if (obj is bool)
                return obj.ToString().ToLower();
            return obj.ToString();
        }

        protected string Indent(int level)
        {
            StringWriter stringWriter = new StringWriter();
            for (int i = 0; i < level; i++)
                stringWriter.Write("    ");
            return stringWriter.ToString();
        }
        protected void WriteLine(StreamWriter streamWriter, string str)
        {
            streamWriter.Write(str);
            streamWriter.Write("\n");
        }

    }
}
