using System;

namespace DCSMissionTweaker
{
    public class ParseException : Exception
    {
        private ParseException(string message) : base(message)
        {
        }
        public static ParseException Create(string message, int line)
        {
            return new ParseException(message + " in line " + line);
        }
        public static ParseException Create(string message)
        {
            return new ParseException(message);
        }
    }
}
