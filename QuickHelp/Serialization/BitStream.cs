using System;
using System.IO;

namespace QuickHelp.Serialization
{
    /// <summary>
    /// Represents a stream that reads and writes bits (from MSB to LSB) to
    /// and from a base stream as bytes of value zero and one.
    /// </summary>
    /// <remarks>
    /// This class only accesses the base stream when necessary and does not
    /// prefetch reads or buffer writes.
    /// </remarks>
    class BitStream : Stream
    {
        readonly Stream m_baseStream;
        int m_currentByte;
        int m_numBitsAvailable;

        public BitStream(Stream baseStream)
        {
            if (baseStream == null)
                throw new ArgumentNullException(nameof(baseStream));

            m_baseStream = baseStream;
            m_currentByte = 0;
            m_numBitsAvailable = 0;
        }

        public override bool CanRead
        {
            get { return m_baseStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return m_baseStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return m_baseStream.CanWrite; }
        }

        public override long Length
        {
            get { return m_baseStream.Length * 8; }
        }

        public override long Position
        {
            get { return m_baseStream.Position * 8 - m_numBitsAvailable; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                long bitPosition = value;
                long bytePosition = bitPosition / 8;
                m_baseStream.Position = bytePosition;
                m_numBitsAvailable = (int)(bytePosition * 8 - bitPosition);
                // numBitsAvailable may be negative and this is intended.
            }
        }

        public override void Flush()
        {
            m_baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0 || count > buffer.Length - offset)
                throw new ArgumentOutOfRangeException(nameof(count));

            int n;
            for (n = 0; n < count; n++)
            {
                int value = ReadByte();
                if (value == 0 || value == 1)
                    buffer[offset + n] = (byte)value;
                else
                    break;
            }
            return n;
        }

        /// <summary>
        /// Reads the next bit from the base stream as a byte of value 0 or 1.
        /// </summary>
        /// <returns>
        /// 1 if the bit is set; 0 if the bit is reset; -1 if EOF.
        /// </returns>
        public override int ReadByte()
        {
            if (m_numBitsAvailable <= 0)
            {
                m_currentByte = m_baseStream.ReadByte();
                if (m_currentByte < 0)
                    return -1;
                m_numBitsAvailable += 8;
            }
            return (m_currentByte >> --m_numBitsAvailable) & 1;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.Position = offset;
                    break;
                case SeekOrigin.Current:
                    this.Position += offset;
                    break;
                case SeekOrigin.End:
                    this.Position = this.Length + offset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin));
            }
            return this.Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException(
                "BitStream does not support SetLength().");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || offset > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0 || count > buffer.Length - offset)
                throw new ArgumentOutOfRangeException(nameof(count));

            for (int n = 0; n < count; n++)
            {
                WriteByte(buffer[offset + n]);
            }
        }

        public override void WriteByte(byte value)
        {
            if (value != 0 && value != 1)
                throw new ArgumentOutOfRangeException(nameof(value));

            int newByte = m_currentByte;
            int numBitsRemaining = m_numBitsAvailable;
            int seekOffset = -1;

            if (numBitsRemaining <= 0)
            {
                newByte = m_baseStream.ReadByte();
                if (newByte < 0)
                {
                    // Writing past the end of the stream is allowed in case
                    // the base stream supports extending the stream on write.
                    seekOffset = 0;
                    newByte = 0;
                }
                numBitsRemaining += 8;
            }

            --numBitsRemaining;
            newByte = newByte
                & ~(1 << numBitsRemaining)
                | (value << numBitsRemaining);

            if (seekOffset != 0)
                m_baseStream.Seek(seekOffset, SeekOrigin.Current);

            try
            {
                m_baseStream.WriteByte((byte)newByte);
            }
            finally
            {
                if (seekOffset != 0)
                    m_baseStream.Seek(-seekOffset, SeekOrigin.Current);
            }

            m_currentByte = newByte;
            m_numBitsAvailable = numBitsRemaining;
        }
    }
}
