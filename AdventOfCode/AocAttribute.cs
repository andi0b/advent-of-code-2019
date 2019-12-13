using System;
using System.ComponentModel.Composition;

namespace AdventOfCode
{
    [MetadataAttribute]
    public class AocAttribute : Attribute
    {
        public int Day { get; }

        public AocAttribute(int day)
        {
            Day = day;
        }
    }

    public class AocMetadata
    {
        public int Day { get; set; }
    }
}