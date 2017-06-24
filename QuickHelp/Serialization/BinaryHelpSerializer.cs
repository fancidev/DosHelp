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
        /// Raised when malformed input is encountered during deserialization
        /// of a topic.
        /// </summary>
        public event InvalidTopicDataEventHandler InvalidTopicData;

        public IEnumerable<HelpDatabase> LoadDatabases(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
            {
                while (stream.Position < stream.Length)
                {
                    HelpDatabase database = DeserializeDatabase(stream);
                    yield return database;
                }
            }
        }

        public HelpDatabase DeserializeDatabase(Stream stream)
        {
            return DeserializeDatabase(stream, new SerializationOptions());
        }

        /// <summary>
        /// Deserializes the next help database from a binary reader.
        /// </summary>
        /// <remarks>
        /// This method throws an exception if it encounters an irrecoverable
        /// error in the input, such as an IO error or malformed input in the
        /// meta data. It raises an <c>InvalidTopicData</c> event for each
        /// format error it encounters during topic deserialization.
        /// </remarks>
        public HelpDatabase DeserializeDatabase(
            Stream stream, SerializationOptions options)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (options == null)
                options = new SerializationOptions();

            BinaryHelpFileHeader header = ReadFileHeader(stream);
            bool isCaseSensitive = (header.Attributes & HelpFileAttributes.CaseSensitive) != 0;
            HelpDatabase database = new HelpDatabase(header.DatabaseName, isCaseSensitive);
            options.ControlCharacter = Graphic437.GetChars(new byte[] { header.ControlCharacter })[0];

            using (Stream streamView = new StreamView(stream, header.DatabaseSize, 0x46))
            using (BinaryReader reader = new BinaryReader(streamView))
            {
                int[] topicOffsets = ReadTopicIndex(reader, header);

                // Read Context Strings and Context Map sections.
                if (true)
                {
                    string[] contextStrings = ReadContextStrings(reader, header);
                    UInt16[] contextMap = ReadContextMap(reader, header);
                    for (int i = 0; i < header.ContextCount; i++)
                    {
                        database.AddContext(contextStrings[i], contextMap[i]);
                    }
                }

                // Read Keywords section.
                if (header.KeywordsOffset > 0)
                {
                    options.Keywords = ReadKeywords(reader, header);
                    options.Compression |= CompressionFlags.Keyword;
                    options.Compression |= CompressionFlags.ExtendedKeyword;
                }
                else
                {
                    options.Keywords = null;
                }

                // Read Huffman Tree section.
                if (header.HuffmanTreeOffset > 0)
                {
                    options.HuffmanTree = ReadHuffmanTree(reader, header);
                    // file.HuffmanTree.Dump();
                    options.Compression |= CompressionFlags.Huffman;
                }
                else
                {
                    options.HuffmanTree = null;
                }

                // Read topic data.
                if (reader.BaseStream.Position != header.TopicTextOffset)
                {
                    throw new InvalidDataException("Incorrect topic position.");
                }
                for (int i = 0; i < header.TopicCount; i++)
                {
                    if (reader.BaseStream.Position != topicOffsets[i])
                        throw new InvalidDataException("Incorrect topic position.");
                    int inputLength = topicOffsets[i + 1] - topicOffsets[i];

                    byte[] inputData = reader.ReadBytes(inputLength);
                    database.NewTopic();
                    ReadTopic(inputData, database.Topics[i], options);
                }
            }
            return database;
        }

        private void ReadTopic(byte[] input, HelpTopic topic, SerializationOptions options)
        {
            try
            {
                byte[] decompressedData = DecompressTopicData(input, topic, options);
                if (decompressedData != null)
                {
                    topic.Source = decompressedData;
                    DecodeTopic(decompressedData, topic, options.ControlCharacter);
                }
            }
            catch (Exception ex)
            {
                var e = new InvalidTopicDataEventArgs(topic, input,
                    "Exception: " + ex.Message);
                this.InvalidTopicData?.Invoke(this, e);
            }
        }
        
        private static BinaryHelpFileHeader ReadFileHeader(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(new StreamView(stream, 0x46)))
            {
                BinaryHelpFileHeader header = new BinaryHelpFileHeader();
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
                header.TopicIndexOffset = reader.ReadInt32();
                header.ContextStringsOffset = reader.ReadInt32();
                header.ContextMapOffset = reader.ReadInt32();
                header.KeywordsOffset = reader.ReadInt32();
                header.HuffmanTreeOffset = reader.ReadInt32();
                header.TopicTextOffset = reader.ReadInt32();
                header.reserved2 = reader.ReadInt32();
                header.reserved3 = reader.ReadInt32();
                header.DatabaseSize = reader.ReadInt32();

                // Verify signature.
                if (header.Signature != 0x4E4C)
                    throw new InvalidDataException("File signature mismatch.");

                return header;
            }
        }

        private static void ReadFileHeader(BinaryReader reader, BinaryHelpMetaData file)
        {
            BinaryHelpFileHeader header = new BinaryHelpFileHeader();
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
            header.TopicIndexOffset = reader.ReadInt32();
            header.ContextStringsOffset = reader.ReadInt32();
            header.ContextMapOffset = reader.ReadInt32();
            header.KeywordsOffset = reader.ReadInt32();
            header.HuffmanTreeOffset = reader.ReadInt32();
            header.TopicTextOffset = reader.ReadInt32();
            header.reserved2 = reader.ReadInt32();
            header.reserved3 = reader.ReadInt32();
            header.DatabaseSize = reader.ReadInt32();

            // Verify signature.
            if (header.Signature != 0x4E4C)
                throw new InvalidDataException("File signature mismatch.");

            file.Header = header;
        }

        private static int[] ReadTopicIndex(BinaryReader reader, BinaryHelpFileHeader header)
        {
            if (reader.BaseStream.Position != header.TopicIndexOffset)
                throw new InvalidDataException("Incorrect Topic Index section position.");

            int[] topicOffsets = new int[header.TopicCount + 1];
            for (int i = 0; i <= header.TopicCount; i++)
            {
                topicOffsets[i] = reader.ReadInt32();
            }
            return topicOffsets;
        }

        private static string[] ReadContextStrings(BinaryReader reader, BinaryHelpFileHeader header)
        {
            if (reader.BaseStream.Position != header.ContextStringsOffset)
                throw new InvalidDataException("Incorrect Context Strings section position.");

            // TODO: the NULL at the very end produces an extra, empty context string.
            // TODO: check exact number of context strings.
            int size = header.ContextMapOffset - header.ContextStringsOffset;
            string all = Encoding.ASCII.GetString(reader.ReadBytes(size));
            return all.Split('\0');
        }

        private static UInt16[] ReadContextMap(BinaryReader reader, BinaryHelpFileHeader header)
        {
            if (reader.BaseStream.Position != header.ContextMapOffset)
                throw new InvalidDataException("Incorrect Context Map section position.");

            UInt16[] contextMap = new UInt16[header.ContextCount];
            for (int i = 0; i < header.ContextCount; i++)
            {
                contextMap[i] = reader.ReadUInt16();
            }
            return contextMap;
        }

        // TODO: this section doesn't terminate by itself?!
        private static byte[][] ReadKeywords(
            BinaryReader reader, BinaryHelpFileHeader header)
        {
            if (reader.BaseStream.Position != header.KeywordsOffset)
                throw new InvalidDataException("Incorrect Keywords section position.");

            int sectionSize = (header.HuffmanTreeOffset > 0 ? header.HuffmanTreeOffset : header.TopicTextOffset)
                - header.KeywordsOffset;
            byte[] section = reader.ReadBytes(sectionSize);
            if (section.Length != sectionSize)
                throw new InvalidDataException("Cannot fully read dictionary section.");

            return KeywordListSerializer.Deserialize(section).ToArray();
        }

        private static HuffmanTree ReadHuffmanTree(BinaryReader reader, BinaryHelpFileHeader header)
        {
            if (reader.BaseStream.Position != header.HuffmanTreeOffset)
            {
                throw new InvalidDataException("Incorrect Huffman Tree section position.");
            }

            //int sectionSize = file.Header.TopicTextOffset - file.Header.HuffmanTreeOffset;
            HuffmanTree tree = HuffmanTree.Deserialize(reader);
            if (tree.IsEmpty || tree.IsSingular)
                throw new InvalidDataException("Invalid huffman tree.");
            return tree;
        }

        private static readonly Graphic437Encoding Graphic437 =
            new Graphic437Encoding();

        private byte[] DecompressTopicData(byte[] input, HelpTopic topic,
            SerializationOptions options)
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
            using (var huffmanStream = new HuffmanStream(memoryStream, options.HuffmanTree))
            using (var compressionStream = new CompressionStream(huffmanStream, options.Keywords))
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

                bool isCommand = true;
                try
                {
                    isCommand = HelpCommandConverter.ProcessCommand(
                        line.Text, controlCharacter, topic);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format(
                        "Unable to process command '{0}': {1}",
                        line.Text, ex.Message));
                }

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
}
