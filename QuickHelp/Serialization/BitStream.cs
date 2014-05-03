using System;
using System.IO;

namespace QuickHelp.Serialization
{
    /// <summary>
    /// Represents a stream of bits. The bits are read from a byte stream
    /// sequentially; in each byte, the bits are read from MSB to LSB.
    /// </summary>
    /// <remarks>
    /// This stream class is not buffered; that is, it does not read more
    /// bytes from the base stream than requested. Therefore it is suitable
    /// for detecting errors in the base stream.
    /// </remarks>
    class BitStream
    {
        readonly Stream baseStream;
        int currentByte;
        int numBitsLeft;

        public BitStream(Stream baseStream)
        {
            if (baseStream == null)
                throw new ArgumentNullException("baseStream");

            this.baseStream = baseStream;
            this.currentByte = 0;
            this.numBitsLeft = 0;
        }

        /// <summary>
        /// Reads the next bit in the stream.
        /// </summary>
        /// <returns>
        /// 1 if the bit is set; 0 if the bit is reset; -1 if EOF encountered.
        /// </returns>
        public int ReadBit()
        {
            if (numBitsLeft == 0)
            {
                currentByte = baseStream.ReadByte();
                if (currentByte < 0)
                    return -1;
                numBitsLeft = 8;
            }
            return (currentByte >> --numBitsLeft) & 1;
        }
    }
}
