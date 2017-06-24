using System;

namespace QuickHelp.Serialization
{
    class BinaryHelpFileHeader
    {
        public UInt16 Version;
        public HelpFileAttributes Attributes;
        public byte ControlCharacter;
        public byte Padding1;
        public UInt16 TopicCount;
        public UInt16 ContextCount;
        public byte DisplayWidth;
        public byte Padding2;
        public UInt16 Padding3;
        public string DatabaseName;
        public int Reserved1;
        public int TopicIndexOffset;
        public int ContextStringsOffset;
        public int ContextMapOffset;
        public int KeywordsOffset;
        public int HuffmanTreeOffset;
        public int TopicTextOffset;
        public int Reserved2;
        public int Reserved3;
        public int DatabaseSize;
    }

    [Flags]
    enum HelpFileAttributes : ushort
    {
        None = 0,

        /// <summary>
        /// Indicates that the context strings in the archive are
        /// case-sensitive.
        /// </summary>
        CaseSensitive = 1,

        /// <summary>
        /// Indicates that the help archive may not be decoded by the
        /// HELPMAKE utility.
        /// </summary>
        Locked = 2,
    }
}
