// ReSharper disable MemberCanBePrivate.Global

using System.Collections.Generic;

namespace Kapture
{
    public class DisplayMode
    {
        private static readonly List<DisplayMode> DisplayModes = new List<DisplayMode>();
        public static readonly List<string> DisplayModeNames = new List<string>();

        public static readonly DisplayMode AlwaysOn = new DisplayMode(0, "Always On");
        public static readonly DisplayMode ContentOnly = new DisplayMode(1, "During Content Only");
        public static readonly DisplayMode DuringRollsOnly = new DisplayMode(2, "During Rolls Only");

        public DisplayMode()
        {
        }

        private DisplayMode(int code, string name)
        {
            Code = code;
            Name = name;
            DisplayModes.Add(this);
            DisplayModeNames.Add(name);
        }

        public int Code { get; }
        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}