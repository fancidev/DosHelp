using System;
using System.IO;

namespace QuickHelp.Serialization
{
    /// <summary>
    /// Represents a view on an underlying base stream with different position
    /// and length.
    /// </summary>
    class StreamView : Stream
    {
        private readonly Stream m_baseStream;
        private readonly long m_length;
        private long m_position;

        public StreamView(Stream baseStream, long length)
            : this(baseStream, length, 0)
        {
        }

        /// <summary>
        /// Creates a view on a stream.
        /// </summary>
        public StreamView(Stream baseStream, long length, long position)
        {
            if (baseStream == null)
                throw new ArgumentNullException(nameof(baseStream));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (!(position >= 0 && position <= length))
                throw new ArgumentOutOfRangeException(nameof(position));

            m_baseStream = baseStream;
            m_length = length;
            m_position = position;
        }

        public override long Length
        {
            get { return m_length; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (!(offset >= 0 && offset <= buffer.Length))
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (!(count >= 0 && offset + count <= buffer.Length))
                throw new ArgumentOutOfRangeException(nameof(count));

            if (count > m_length - m_position)
                count = (int)(m_length - m_position);
            if (count == 0)
                return 0;

            // TODO: read full or throw exception
            int actual = m_baseStream.Read(buffer, offset, count);
            m_position += actual;
            return actual;
        }
        
        public override bool CanRead
        {
            get { return m_baseStream.CanRead; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }
 
        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Position
        {
            get { return m_position; }
            set { throw new NotSupportedException(); }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
    }
}
