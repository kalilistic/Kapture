using System.Collections.Generic;

namespace Kapture
{
    /// <summary>
    /// Overlay display mode.
    /// </summary>
    public class DisplayMode
    {
        /// <summary>
        /// Display modes.
        /// </summary>
        public static readonly List<DisplayMode> DisplayModes = new();

        /// <summary>
        /// Display mode names.
        /// </summary>
        public static readonly List<string> DisplayModeNames = new();

        /// <summary>
        /// Display Mode: always.
        /// </summary>
        public static readonly DisplayMode AlwaysOn = new(0, "Always On");

        /// <summary>
        /// Display Mode: content only.
        /// </summary>
        public static readonly DisplayMode ContentOnly = new(1, "During Content Only");

        /// <summary>
        /// Display Mode: rolls only.
        /// </summary>
        public static readonly DisplayMode DuringRollsOnly = new(2, "During Rolls Only");

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayMode"/> class.
        /// </summary>
        public DisplayMode()
        {
        }

        private DisplayMode(int code, string name)
        {
            this.Code = code;
            this.Name = name;
            DisplayModes.Add(this);
            DisplayModeNames.Add(name);
        }

        /// <summary>
        /// Gets display mode code.
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// Gets display mode name.
        /// </summary>
        public string Name { get; } = null!;

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Name;
        }
    }
}
