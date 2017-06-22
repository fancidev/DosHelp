using System;
using System.IO;

namespace QuickHelp.Serialization
{
    using HuffmanTreeNode = BinaryTreeNode<byte>;

    // The huffman tree must be a full binary tree.

    /// <summary>
    /// Represents a huffman-encoded stream. Only supports decoding.
    /// </summary>
    /// <remarks>
    /// This stream class is unbuffered; that is, it does not pre-read any
    /// byte that is not necessary from the base stream. Therefore it is 
    /// suitable for detecting errors in the base stream.
    /// </remarks>
    sealed class HuffmanStream : Stream
    {
        readonly HuffmanTree huffmanTree;
        readonly BitStream bitStream;

        public HuffmanStream(Stream baseStream, HuffmanTree huffmanTree)
        {
            if (baseStream == null)
                throw new ArgumentNullException(nameof(baseStream));
            if (huffmanTree == null)
                throw new ArgumentNullException(nameof(huffmanTree));

            this.bitStream = new BitStream(baseStream);
            this.huffmanTree = huffmanTree;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return StreamExtensions.ReadBytes(this, buffer, offset, count);
        }

        public override int ReadByte()
        {
            HuffmanTreeNode node = huffmanTree.Root;
            while (true)
            {
                if (node.IsLeaf)
                    return node.Value;

                int bit = bitStream.ReadByte();
                if (bit < 0) // EOF
                    return -1;

                // TBD here
                if (bit == 0)
                    node = node.LeftChild;
                else
                    node = node.RightChild;
            }
        }

        #region Stream Members

        public override bool CanRead
        {
            get { return true; }
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

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
