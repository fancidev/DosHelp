using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace QuickHelp.Serialization
{
    /// <summary>
    /// Provides methods to load one or more help databases from a .HLP file.
    /// </summary>
    public class DatabaseDecoder
    {
        /// <summary>
        /// Loads the first help database in the given file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public HelpDatabase Deserialize(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                return Deserialize(reader);
            }
        }

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

        public delegate void TopicDecodingEventHandler(object sender, TopicDecodingError e);

        public event TopicDecodingEventHandler TopicDecodingError;

        private void RaiseTopicDecodingError(object sender, TopicDecodingError e)
        {
            if (TopicDecodingError != null)
            {
                TopicDecodingError(sender, e);
            }
        }

        /// <summary>
        /// Loads the next help database from the file. The stream need not
        /// be seekable.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public HelpDatabase Deserialize(BinaryReader reader)
        {
            HelpDatabase database = ReadDatabase(reader);
            return database;
        }

        private static bool flag = false;

        private HelpDatabase ReadDatabase(BinaryReader reader)
        {
            HelpFileHeader header = ReadHeader(reader);

            // Check signature.
            if (header.Signature != 0x4E4C)
                throw new InvalidDataException("File signature mismatch.");

            // TODO: validate header fields.

            // Read the sections.
            HelpFile file = new HelpFile();
            file.Header = header;

            ReadTopicOffsets(reader, file);
            ReadContextStrings(reader, file);
            ReadContextTopics(reader, file);
            ReadDictionary(reader, file);
            ReadHuffmanTree(reader, file);
            //file.HuffmanTree.Dump();

            // Create a help database and initialize its structure.
            bool isCaseSensitive = (header.Attributes & HelpFileAttributes.CaseSensitive) != 0;
            HelpDatabase database = new HelpDatabase(header.FileName, isCaseSensitive);
            for (int i = 0; i < file.Header.TopicCount; i++)
            {
                database.NewTopic();
            }
            for (int i = 0; i < file.Header.ContextCount; i++)
            {
                database.AddContext(file.ContextStrings[i], file.ContextTopics[i]);
            }

            // Decode the actual topics.
            for (int i = 0; i < file.Header.TopicCount; i++)
            {
                HelpTopic topic = database.Topics[i];

                // Get the encoded binary data. Any error here is not recoverable.
                byte[] input = null;
                try
                {
                    int inputLength = file.TopicOffsets[i + 1] - file.TopicOffsets[i];
                    input = reader.ReadBytes(inputLength);
                    if (input.Length != inputLength)
                        throw new EndOfStreamException("Cannot read topic input.");

                    TopicDecoder.Decode(input, topic, file);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format(
                        "FAILED TO READ TOPIC {0}: {1}",
                        i, ex.Message));

                    TopicDecodingError e = new Serialization.TopicDecodingError(topic, input, ex.Message);
                    RaiseTopicDecodingError(this, e);

                    flag = true;
                    if (!flag)
                    {
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
                        flag = true;
                    }
                }
            }

            return database;
        }

        private static HelpFileHeader ReadHeader(BinaryReader reader)
        {
            HelpFileHeader header = new HelpFileHeader();
            header.Signature = reader.ReadUInt16();
            header.Unknown1 = reader.ReadUInt16();
            header.Attributes = (HelpFileAttributes)reader.ReadUInt16();
            header.ControlCharacter = reader.ReadByte();
            header.Unknown3 = reader.ReadByte();
            header.TopicCount = reader.ReadUInt16();
            header.ContextCount = reader.ReadUInt16();
            header.TextWidth = reader.ReadByte();
            header.Unknown4 = reader.ReadByte();
            header.Unknown5 = reader.ReadUInt16();

            byte[] stringData = reader.ReadBytes(14);            
            header.FileName = Encoding.ASCII.GetString(stringData);
            int k = header.FileName.IndexOf('\0');
            if (k >= 0)
                header.FileName = header.FileName.Substring(0, k);

            header.reserved1 = reader.ReadInt32();
            header.TopicOffsetsOffset = reader.ReadInt32();
            header.ContextStringsOffset = reader.ReadInt32();
            header.ContextTopicsOffset = reader.ReadInt32();
            header.DictionaryOffset = reader.ReadInt32();
            header.HuffmanTreeOffset = reader.ReadInt32();
            header.TopicDataOffset = reader.ReadInt32();
            header.reserved2 = reader.ReadInt32();
            header.reserved3 = reader.ReadInt32();
            header.DatabaseSize = reader.ReadInt32();
            return header;
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
            int size = file.Header.ContextTopicsOffset - file.Header.ContextStringsOffset;
            string all = Encoding.ASCII.GetString(reader.ReadBytes(size));
            file.ContextStrings = all.Split('\0');
        }

        private static void ReadContextTopics(BinaryReader reader, HelpFile file)
        {
            file.ContextTopics = new UInt16[file.Header.ContextCount];
            for (int i = 0; i < file.ContextTopics.Length; i++)
            {
                file.ContextTopics[i] = reader.ReadUInt16();
            }
        }

        private static void ReadDictionary(BinaryReader reader, HelpFile file)
        {
            if (file.Header.DictionaryOffset !=
                file.Header.ContextTopicsOffset + 2 * file.ContextTopics.Length)
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
    }

    /// <summary>
    /// Contains information about an error encountered when decoding topic.
    /// </summary>
    public class TopicDecodingError
    {
        private readonly HelpTopic topic;
        private readonly byte[] input;
        private string message;

        public TopicDecodingError(HelpTopic topic, byte[] input, string message)
        {
            this.topic = topic;
            this.input = input;
            this.message = message;
        }

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
