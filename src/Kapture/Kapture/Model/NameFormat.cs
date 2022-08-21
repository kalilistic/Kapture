using System.Collections.Generic;

namespace Kapture
{
    /// <summary>
    /// Name format.
    /// </summary>
    public class NameFormat
    {
        /// <summary>
        /// Name formats.
        /// </summary>
        public static readonly List<NameFormat> NameFormats = new();

        /// <summary>
        /// Name format names.
        /// </summary>
        public static readonly List<string> NameFormatNames = new();

        /// <summary>
        /// Name format: full name.
        /// </summary>
        public static readonly NameFormat FullName = new(0, "Full Name");

        /// <summary>
        /// Name format: surname abbreviated.
        /// </summary>
        public static readonly NameFormat SurnameAbbreviated = new(1, "Surname Abbreviated");

        /// <summary>
        /// Name format: forename abbreviated.
        /// </summary>
        public static readonly NameFormat ForenameAbbreviated = new(2, "Forename Abbreviated");

        /// <summary>
        /// Name format: initials.
        /// </summary>
        public static readonly NameFormat Initials = new(3, "Initials");

        /// <summary>
        /// Initializes a new instance of the <see cref="NameFormat"/> class.
        /// </summary>
        public NameFormat()
        {
        }

        private NameFormat(int code, string name)
        {
            this.Code = code;
            this.Name = name;
            NameFormats.Add(this);
            NameFormatNames.Add(name);
        }

        /// <summary>
        /// Gets name format code.
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// Gets name format name.
        /// </summary>
        public string Name { get; } = null!;

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Name;
        }
    }
}
