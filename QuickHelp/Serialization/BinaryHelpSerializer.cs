using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuickHelp.Serialization
{
    /// <summary>
    /// Provides methods to deserialize help databases from a .HLP file.
    /// </summary>
    public class BinaryHelpDeserializer
    {
        public delegate void InvalidTopicDataEventHandler(
            object sender, InvalidTopicDataEventArgs e);

        /// <summary>
        /// Raised when invalid data is encountered during deserialization
        /// of a topic.
        /// </summary>
        public event InvalidTopicDataEventHandler InvalidTopicData;

        public IEnumerable<HelpDatabase> LoadDatabases(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                while (stream.Position < stream.Length)
                {
                    HelpDatabase database = Deserialize(reader);
                    database.FileName = fileName;
                    yield return database;
                }
            }
        }

        private void RaiseInvalidTopicData(object sender, InvalidTopicDataEventArgs e)
        {
            if (InvalidTopicData != null)
            {
                InvalidTopicData(sender, e);
            }
        }

        /// <summary>
        /// Deserializes the next help database from the given binary reader.
        /// </summary>
        /// <remarks>
        /// This method throws an exception if it encounters an irrecoverable
        /// error in the input, which may be an IO error or format error in
        /// the meta data. It raises an <c>InvalidTopicData</c> event for
        /// each format error it encounters during topic deserialization.
        /// </remarks>
        public HelpDatabase Deserialize(BinaryReader reader)
        {
            HelpFile metaData = ReadMetaData(reader);
            HelpDatabase database = CreateDatabase(reader, metaData);
            return database;
        }

        private static HelpFile ReadMetaData(BinaryReader reader)
        {
            HelpFile file = new HelpFile();
            ReadHeader(reader, file);
            ReadTopicOffsets(reader, file);
            ReadContextStrings(reader, file);
            ReadContextMapping(reader, file);
            ReadDictionary(reader, file);
            ReadHuffmanTree(reader, file);
            //file.HuffmanTree.Dump();
            return file;
        }

        // Create a help database and initialize its structure.
        private HelpDatabase CreateDatabase(BinaryReader reader, HelpFile file)
        {
            bool isCaseSensitive = (file.Header.Attributes & HelpFileAttributes.CaseSensitive) != 0;
            HelpDatabase database = new HelpDatabase(file.Header.DatabaseName, isCaseSensitive);
            for (int i = 0; i < file.Header.TopicCount; i++)
            {
                database.NewTopic();
            }
            for (int i = 0; i < file.Header.ContextCount; i++)
            {
                database.AddContext(file.ContextStrings[i], file.ContextMapping[i]);
            }

            // Decode topic data.
            for (int i = 0; i < file.Header.TopicCount; i++)
            {
                HelpTopic topic = database.Topics[i];

                // Get the encoded binary data. Any error here is not recoverable.
                int inputLength = file.TopicOffsets[i + 1] - file.TopicOffsets[i];
                if (inputLength < 0)
                {
                    throw new InvalidDataException("Topic data length is negative.");
                }

                byte[] input = reader.ReadBytes(inputLength);
                if (input.Length != inputLength)
                {
                    var e = new InvalidTopicDataEventArgs(topic, input,
                        string.Format("Compressed topic size mismatch: " +
                        "expecting {0} bytes, got {1} bytes.",
                        inputLength, input.Length));
                    this.InvalidTopicData?.Invoke(this, e);
                }

                try
                {
                    byte[] decompressedData = DecompressTopicData(input, topic, file);
                    if (decompressedData == null)
                        continue;

                    topic.Source = decompressedData;

                    char controlCharacter = Graphic437.GetChars(
                        new byte[] { file.Header.ControlCharacter })[0];
                    DecodeTopic(decompressedData, topic, controlCharacter);
                }
                catch (Exception ex)
                {
                    var e = new InvalidTopicDataEventArgs(topic, input, 
                        "Exception: " + ex.Message);
                    this.InvalidTopicData?.Invoke(this, e);
                }
            }

            return database;
        }

        private static void ReadHeader(BinaryReader reader, HelpFile file)
        {
            HelpFileHeader header = new HelpFileHeader();
            header.Signature = reader.ReadUInt16();
            header.Unknown1 = reader.ReadUInt16();
            header.Attributes = (HelpFileAttributes)reader.ReadUInt16();
            header.ControlCharacter = reader.ReadByte();
            header.Unknown3 = reader.ReadByte();
            header.TopicCount = reader.ReadUInt16();
            header.ContextCount = reader.ReadUInt16();
            header.DisplayWidth = reader.ReadByte();
            header.Unknown4 = reader.ReadByte();
            header.Unknown5 = reader.ReadUInt16();

            byte[] stringData = reader.ReadBytes(14);
            header.DatabaseName = Encoding.ASCII.GetString(stringData);
            int k = header.DatabaseName.IndexOf('\0');
            if (k >= 0)
                header.DatabaseName = header.DatabaseName.Substring(0, k);

            header.reserved1 = reader.ReadInt32();
            header.TopicOffsetsOffset = reader.ReadInt32();
            header.ContextStringsOffset = reader.ReadInt32();
            header.ContextMappingOffset = reader.ReadInt32();
            header.DictionaryOffset = reader.ReadInt32();
            header.HuffmanTreeOffset = reader.ReadInt32();
            header.TopicDataOffset = reader.ReadInt32();
            header.reserved2 = reader.ReadInt32();
            header.reserved3 = reader.ReadInt32();
            header.DatabaseSize = reader.ReadInt32();

            // Verify signature.
            if (header.Signature != 0x4E4C)
                throw new InvalidDataException("File signature mismatch.");

            file.Header = header;
        }

        private static void ReadTopicOffsets(BinaryReader reader, HelpFile file)
        {
            if (file.Header.TopicOffsetsOffset != 0x46) // header size
                throw new InvalidDataException("Invalid TopicOffsetsOffset.");

            file.TopicOffsets = new int[file.Header.TopicCount + 1];
            for (int i = 0; i < file.TopicOffsets.Length; i++)
            {
                file.TopicOffsets[i] = reader.ReadInt32();
            }
        }

        private static void ReadContextStrings(BinaryReader reader, HelpFile file)
        {
            if (file.Header.ContextStringsOffset - file.Header.TopicOffsetsOffset
                != 4 * (file.Header.TopicCount + 1))
                throw new InvalidDataException("Invalid ContextStringsOffset.");

            // TODO: the NULL at the very end produces an extra, empty context string.
            int size = file.Header.ContextMappingOffset - file.Header.ContextStringsOffset;
            string all = Encoding.ASCII.GetString(reader.ReadBytes(size));
            file.ContextStrings = all.Split('\0');
        }

        private static void ReadContextMapping(BinaryReader reader, HelpFile file)
        {
            file.ContextMapping = new UInt16[file.Header.ContextCount];
            for (int i = 0; i < file.ContextMapping.Length; i++)
            {
                file.ContextMapping[i] = reader.ReadUInt16();
            }
        }

        private static void ReadDictionary(BinaryReader reader, HelpFile file)
        {
            if (file.Header.DictionaryOffset !=
                file.Header.ContextMappingOffset + 2 * file.ContextMapping.Length)
                throw new InvalidDataException("Invalid DictionaryOffset");

            List<byte[]> entries = new List<byte[]>();
            int dictionarySize = file.Header.HuffmanTreeOffset -
                                 file.Header.DictionaryOffset;
            int numBytesRead = 0;
            while (numBytesRead < dictionarySize)
            {
                byte length = reader.ReadByte();
                byte[] entry = reader.ReadBytes(length);
                if (entry.Length != length)
                    throw new InvalidDataException("Invalid length.");
                entries.Add(entry);
                numBytesRead += 1 + length;
            }
            if (numBytesRead != dictionarySize)
                throw new InvalidDataException("Dictionary size is wrong.");
            file.Dictionary = entries.ToArray();
        }

        private static void ReadHuffmanTree(BinaryReader reader, HelpFile file)
        {
            int huffmanTreeSize = file.Header.TopicDataOffset -
                                  file.Header.HuffmanTreeOffset;
            if (huffmanTreeSize % 2 != 0)
                throw new InvalidDataException("Huffman tree size must be even.");

            int nodeCount = huffmanTreeSize / 2;
            short[] nodeValues = new Int16[nodeCount];
            for (int i = 0; i < nodeCount; i++)
            {
                nodeValues[i] = reader.ReadInt16();
            }
            file.HuffmanTree = new CompactHuffmanTree(nodeValues);
        }

        private static readonly Graphic437Encoding Graphic437 =
            new Graphic437Encoding();

        private byte[] DecompressTopicData(byte[] input, HelpTopic topic, HelpFile file)
        {
            // The first two bytes indicates decompressed data size.
            if (input.Length < 2)
            {
                var e = new InvalidTopicDataEventArgs(topic, input,
                    "Not enough bytes for DecodedLength field.");
                this.InvalidTopicData?.Invoke(this, e);
                return null;
            }
            int decompressedLength = BitConverter.ToUInt16(input, 0);

            // The rest of the buffer is a huffman stream wrapping a
            // compression stream wrapping binary-encoded topic data.
            byte[] output;
            using (var memoryStream = new MemoryStream(input, 2, input.Length - 2))
            using (var huffmanStream = new HuffmanStream(memoryStream, file.HuffmanTree))
            using (var compressionStream = new CompressionStream(huffmanStream, file.Dictionary))
            using (var compressionReader = new BinaryReader(compressionStream))
            {
                output = compressionReader.ReadBytes(decompressedLength);
            }

            if (output.Length != decompressedLength)
            {
                var e = new InvalidTopicDataEventArgs(topic, input,
                    string.Format("Decompressed topic size mismatch: " +
                    "expecting {0} bytes, got {1} bytes.",
                    decompressedLength, output.Length));
                this.InvalidTopicData?.Invoke(this, e);
            }
            return output;
        }

        internal static void DecodeTopic(
            byte[] buffer, HelpTopic topic, char controlCharacter)
        {
            BufferReader reader = new BufferReader(buffer, Graphic437);

            while (!reader.IsEOF)
            {
                HelpLine line = null;
                try
                {
                    DecodeLine(reader, out line);
                }
                catch (Exception)
                {
                    if (line != null)
                        topic.Lines.Add(line);
                    throw;
                }

                // TODO: handle control character override.
                bool isCommand = HelpCommandConverter.ProcessColonCommand(
                    line.Text, controlCharacter, topic);
                if (!isCommand)
                {
                    topic.Lines.Add(line);
                }
            }
        }

        internal static void DecodeLine(BufferReader reader, out HelpLine line)
        {
            line = null;

            // Read text length in bytes.
            int textLength = reader.ReadByte();
            string text = reader.ReadFixedLengthString(textLength - 1);
            line = new HelpLine(text);

            // Read byte count of attributes.
            int attrLength = reader.ReadByte();
            BufferReader attrReader = reader.ReadBuffer(attrLength - 1);
            DecodeLineAttributes(line, attrReader);

            // Read hyperlinks.
            while (!attrReader.IsEOF)
            {
                DecodeLineHyperlink(line, attrReader);
            }
        }

        //private static void DecodeLine(Stream stream, out HelpLine line)
        //{
        //    line = null;

        //    // Read text length in bytes.
        //    int textLength = stream.ReadByte();
        //    if (textLength < 0)
        //        throw new EndOfStreamException("Cannot read text length byte.");
        //    byte[] textBytes = new byte[textLength];
        //    stream.ReadFull(textBytes, 0, textBytes.Length);
        //    string text = Graphic437.GetString(textBytes);
        //    line = new HelpLine(text);

        //    // Read byte count of attributes.
        //    int attrLength = reader.ReadByte();
        //    BufferReader attrReader = reader.ReadBuffer(attrLength - 1);
        //    DecodeLineAttributes(line, attrReader);

        //    // Read hyperlinks.
        //    while (!attrReader.IsEOF)
        //    {
        //        DecodeLineHyperlink(line, attrReader);
        //    }
        //}

        private static void DecodeLineAttributes(HelpLine line, BufferReader reader)
        {
            int charIndex = 0;
            for (int chunkIndex = 0; !reader.IsEOF; chunkIndex++)
            {
                TextStyle textStyle = TextStyle.None;

                // Read attribute byte except for the first chunk (for which
                // default attributes are applied).
                if (chunkIndex > 0)
                {
                    byte a = reader.ReadByte();
                    if (a == 0xFF) // marks the beginning of hyperlinks
                        break;

                    textStyle = TextStyle.None;
                    if ((a & 1) != 0)
                        textStyle |= TextStyle.Bold;
                    if ((a & 2) != 0)
                        textStyle |= TextStyle.Italic;
                    if ((a & 4) != 0)
                        textStyle |= TextStyle.Underline;
                    if ((a & 0xF8) != 0)
                    {
                        // should exit
                        //System.Diagnostics.Debug.WriteLine(string.Format(
                        //    "Text attribute bits {0:X2} is not recognized and is ignored.",
                        //    a & 0xF8));
                        throw new InvalidDataException("Invalid text attribute");
                    }
                }

                // Read chunk length to apply this attribute to.
                int charCount = reader.ReadByte();
                if (charCount > line.Length - charIndex)
                {
                    // TODO: issue warning
                    charCount = line.Length - charIndex;
                }
                if (textStyle != TextStyle.None)
                {
                    for (int j = 0; j < charCount; j++)
                        line.Attributes[charIndex + j] = new TextAttribute(textStyle, null);
                }
                charIndex += charCount;
            }
        }

        private static void DecodeLineHyperlink(HelpLine line, BufferReader reader)
        {
            // Read link location.
            int linkStartIndex = reader.ReadByte(); // one-base, inclusive
            int linkEndIndex = reader.ReadByte(); // one-base, inclusive

            if (linkStartIndex == 0 || linkStartIndex > linkEndIndex)
            {
                throw new InvalidDataException("Invalid link location.");
            }
            if (linkEndIndex > line.Length)
            {
                System.Diagnostics.Debug.WriteLine(string.Format(
                    "WARNING: Link end {0} is past line end {1}.",
                    linkEndIndex, line.Length));
                linkEndIndex = line.Length;
            }
            //if (linkStartIndex 

            // Read NULL-terminated context string.
            string context = reader.ReadNullTerminatedString();
            if (context == "") // link is WORD topic index
            {
                int numContext = reader.ReadUInt16(); // 0x8000 | topicIndex
                context = "@L" + numContext.ToString("X4");
            }

            // Add hyperlink to the line.
            HelpUri link = new HelpUri(context);
            for (int j = linkStartIndex; j <= linkEndIndex; j++)
            {
                line.Attributes[j - 1] = new TextAttribute(line.Attributes[j - 1].Style, link);
            }
        }

        private void RepairTopicData()
        {
#if false
            for (int byteIndex = 2; byteIndex < input.Length; byteIndex++)
            {
                byte original = input[byteIndex];
                byte b;
                for (b = (byte)(original + 1); b != original; b++)
                {
                    input[byteIndex] = b;
                    try
                    {
                        TopicDecoder.Decode(input, topic, file);
                        break;
                    }
                    catch (Exception)
                    {
                    }
                }
                if (b != original)
                {
                    break;
                }
                else
                    input[byteIndex] = original;
            }
#endif
        }
    }

    /// <summary>
    /// Contains information about invalid data encountered during topic
    /// deserialization.
    /// </summary>
    public class InvalidTopicDataEventArgs : EventArgs
    {
        private readonly HelpTopic topic;
        private readonly byte[] input;
        private string message;

        public InvalidTopicDataEventArgs(HelpTopic topic, byte[] input, string message)
        {
            this.topic = topic;
            this.input = input;
            this.message = message;
        }

        /// <summary>
        /// Gets the topic being deserialized.
        /// </summary>
        public HelpTopic Topic
        {
            get { return topic; }
        }

        public byte[] Input
        {
            get { return input; }
        }

        public string Message
        {
            get { return message; }
        }
    }

    //public class HelpDecoderException : Exception
    //{
    //    readonly List<Exception> errors = new List<Exception>();
    //
    //    public HelpDecoderException(Exception nestedException)
    //    {
    //        this.errors.Add(nestedException);
    //    }
    //
    //    public List<Exception> Errors
    //    {
    //        get { return errors; }
    //    }
    //}

    //static class BinaryReaderExtensions
    //{
    //    public static string ReadNullTerminatedString(BinaryReader reader)
    //    {
    //        StringBuilder sb = new StringBuilder(16);
    //        for (char c; (c = reader.ReadChar()) != '\0'; )
    //        {
    //            sb.Append(c);
    //        }
    //        return sb.ToString();
    //    }
    //}
}
