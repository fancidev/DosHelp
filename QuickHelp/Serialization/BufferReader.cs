using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QuickHelp.Serialization
{
    /// <summary>
    /// Provides methods to read typed data from a byte buffer. This class is
    /// similar in functionality to a BinaryReader backed by a MemoryStream,
    /// but provides additional methods with a lightweight implementation.
    /// </summary>
    /// TODO: a BufferReader should keep track of context information (like
    /// line and column) so that it can be used to indicate location of any
    /// error.
    public class BufferReader
    {
        readonly Encoding encoding;
        readonly byte[] buffer;
        readonly int endIndex;
        int index;

        public BufferReader(byte[] buffer)
            : this(buffer, 0, buffer.Length, Encoding.UTF8)
        {
        }

        public BufferReader(byte[] buffer, Encoding encoding)
            : this(buffer, 0, buffer.Length, encoding)
        {
        }

        public BufferReader(byte[] buffer, int index, int count)
            : this(buffer, index, count, Encoding.UTF8)
        {
        }

        public BufferReader(byte[] buffer, int index, int count, Encoding encoding)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (index < 0 || index > buffer.Length)
                throw new ArgumentOutOfRangeException("index");
            if (count < 0 || count > buffer.Length - index)
                throw new ArgumentOutOfRangeException("count");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            this.buffer = buffer;
            this.index = index;
            this.endIndex = index + count;
            this.encoding = encoding;
        }

        public bool IsEOF
        {
            get { return index >= endIndex; }
        }

        public byte ReadByte()
        {
            if (index >= endIndex)
                throw new EndOfStreamException();
            return buffer[index++];
        }

        public UInt16 ReadUInt16()
        {
            if (index + 2 > endIndex)
                throw new EndOfStreamException();
            int value = buffer[index] | (buffer[index + 1] << 8);
            index += 2;
            return (UInt16)value;
        }

        public string ReadNullTerminatedString()
        {
            int k = Array.IndexOf(buffer, (byte)0, index, endIndex - index);
            if (k == -1)
                throw new EndOfStreamException();

            string s = encoding.GetString(buffer, index, k - index);
            index = k + 1;
            return s;
        }

        public string ReadFixedLengthString(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");
            if (length > endIndex - index)
                throw new EndOfStreamException();

            string s = encoding.GetString(buffer, index, length);
            index += length;
            return s;
        }

        public BufferReader ReadBuffer(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length");
            if (length > endIndex - index)
                throw new EndOfStreamException();

            BufferReader subReader = new BufferReader(buffer, index, length, encoding);
            index += length;
            return subReader;
        }
    }
}
