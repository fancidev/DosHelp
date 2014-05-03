using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QuickHelp.Serialization
{
    /// <summary>
    /// Provides methods to decode a help topic stored in binary QuickHelp
    /// format.
    /// </summary>
    static class TopicDecoder
    {
        static readonly Graphic437Encoding Graphic437 = new Graphic437Encoding();

        public static void Decode(byte[] input, HelpTopic topic, HelpFile file)
        {
            if (input == null)
                throw new ArgumentNullException("buffer");
            if (topic == null)
                throw new ArgumentNullException("topic");
            if (file == null)
                throw new ArgumentNullException("file");

            // The first two bytes indicates decoded length.
            if (input.Length < 2)
                throw new EndOfStreamException("Cannot read DecodedLength field.");
            int decodedLength = BitConverter.ToUInt16(input, 0);

            // The rest of the buffer is a huffman stream encapsulating a
            // compression stream encapsulating binary-encoded topic data.
            byte[] output;
            using (MemoryStream memoryStream = new MemoryStream(input, 2, input.Length - 2))
            using (HuffmanStream huffmanStream = new
                   HuffmanStream(memoryStream, file.HuffmanTree))
            using (CompressionStream compressionStream = new
                   CompressionStream(huffmanStream, file.Dictionary))
            // using limitedlengthstream = ...
            using (BinaryReader compressionReader = new
                   BinaryReader(compressionStream)) // BinaryReader has ReadFull
            {
                output = compressionReader.ReadBytes(decodedLength);
            }

            topic.Lines.Clear();
            topic.Source = output;
            DecodeTopic(output, topic);

            if (output.Length != decodedLength)
            {
                System.Diagnostics.Debug.WriteLine(string.Format(
                    "{0} [Topic #{1}]: Actual decoded length ({2}) " +
                    "is different from stated decoded length ({3}).",
                    file.Header.FileName,
                    -1,
                    output.Length,
                    decodedLength));
            }
        }

        internal static void DecodeTopic(byte[] buffer, HelpTopic topic)
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
                bool isCommand = HelpCommandConverter.ProcessColonCommand(line.Text, topic);
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
                        textStyle |= TextStyle.Italics;
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
    }
}
