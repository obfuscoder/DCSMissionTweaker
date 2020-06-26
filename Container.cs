using System;
using System.Collections.Generic;
using System.IO;

namespace DCSMissionTweaker
{
    class Container : Element
    {
        public List<Element> children = new List<Element>();

        public Container(Container parent, string name) : base(parent, name)
        {
            if (parent != null)
                parent.children.Add(this);
        }
        public Attribute SearchAttribute(object name, object value)
        {
            foreach (Attribute attribute in children.FindAll(c => c is Attribute))
                if (attribute.name.Equals(name) && attribute.value.Equals(value))
                    return attribute;

            foreach (Container container in children.FindAll(c => c is Container))
            {
                Attribute attribute = container.SearchAttribute(name, value);
                if (attribute != null)
                    return attribute;
            }
            return null;
        }

        public Attribute SearchAttribute(object name)
        {
            foreach (Attribute attribute in children.FindAll(c => c is Attribute))
                if (attribute.name.Equals(name))
                    return attribute;

            foreach (Container container in children.FindAll(c => c is Container))
            {
                Attribute attribute = container.SearchAttribute(name);
                if (attribute != null)
                    return attribute;
            }
            return null;
        }

        internal override void Write(StreamWriter streamWriter, int level)
        {
            string indent = Indent(level);
            WriteLine(streamWriter, indent + NameToString() + " = ");
            WriteLine(streamWriter, indent + "{");
            children.ForEach(c => c.Write(streamWriter, level + 1));
            WriteLine(streamWriter, indent + "}, -- end of " + NameToString());
        }
    }
}
