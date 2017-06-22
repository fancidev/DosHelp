using System;
using System.Collections.Generic;
using System.Text;

namespace QuickHelp.Serialization
{
    /// <summary>
    /// Contains options to control the serialization process.
    /// </summary>
    public class SerializationOptions
    {
        private readonly List<byte[]> m_keywords = new List<byte[]>();

        /// <summary>
        /// Gets or sets the serialized format.
        /// </summary>
        public SerializationFormat Format { get; set; }

        /// <summary>
        /// Gets or sets the compression level.
        /// </summary>
        /// <remarks>
        /// On serialization, if Format is Automatic or Binary, this value
        /// controls the compression level in the serialized .HLP file. If
        /// Format is Markup, this value is ignored.
        /// 
        /// On deserialization, this value is ignored and updated to the
        /// actual compression level used in the input.
        /// </remarks>
        public CompressionFlags Compression { get; set; }

        /// <summary>
        /// Gets a list of keywords used for keyword compression.
        /// </summary>
        /// <remarks>
        /// On serialization, if keyword compression is enabled, the
        /// serializer uses the dictionary specified by this property if it
        /// is not <c>null</c>, or computes the dictionary on the fly and 
        /// updates this property if it is null.
        /// 
        /// On deserialization, the serializaer sets this property to the
        /// actual dictionary used in the input.
        /// </remarks>
        public KeywordCollection Keywords { get; set; }

        public HuffmanTree HuffmanTree { get; set; }
    }

    /// <summary>
    /// Specifies the serialized format of a help database.
    /// </summary>
    public enum SerializationFormat
    {
        /// <summary>
        /// On deserialization, automatically detect the input format. On
        /// serialization, use Binary format.
        /// </summary>
        Automatic = 0,

        /// <summary>
        /// Specifies the binary format (with .HLP extension).
        /// </summary>
        Binary = 1,

        /// <summary>
        /// Specifies the markup format (with .SRC extension).
        /// </summary>
        Markup = 2,
    }

    [Flags]
    public enum CompressionFlags
    {
        None = 0,
        RunLength = 1,
        Keyword = 2,
        ExtendedKeyword = 4,
        Huffman = 8,
        All = RunLength | Keyword | ExtendedKeyword | Huffman
    }

    // no more than 1024 (or 23?) entries
    public class KeywordCollection : List<byte[]>
    {

    }
}
