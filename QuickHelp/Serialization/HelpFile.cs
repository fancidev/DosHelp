using System;

namespace QuickHelp.Serialization
{
    /// <summary>
    /// Represents the contents of a help database as it is stored in a
    /// physical file. A single physical file may contain multiple help
    /// databases concatenated one after another.
    /// </summary>
    /// TODO: rename to HelpBinaryMetaData
    class BinaryHelpMetaData
    {
        public BinaryHelpFileHeader Header;
        public int[] TopicOffsets;
        public string[] ContextStrings;
        public UInt16[] ContextMapping;
        public byte[][] Dictionary;
        //public BinaryTree HuffmanTree;
    }

    class BinaryHelpFileHeader
    {
        public UInt16 Signature;
        public UInt16 Unknown1;
        public HelpFileAttributes Attributes;
        public byte ControlCharacter;
        public byte Unknown3;
        public UInt16 TopicCount;
        public UInt16 ContextCount;
        public byte DisplayWidth;
        public byte Unknown4;
        public UInt16 Unknown5;
        public string DatabaseName;
        public int reserved1;
        public int TopicOffsetsOffset;
        public int ContextStringsOffset;
        public int ContextMappingOffset;
        public int DictionaryOffset;
        public int HuffmanTreeOffset;
        public int TopicDataOffset;
        public int reserved2;
        public int reserved3;
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
