using System;
using System.IO;

namespace QuickHelp.Serialization
{
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
        readonly CompactHuffmanTree huffmanTree;
        readonly BitStream bitStream;

        public HuffmanStream(Stream baseStream, CompactHuffmanTree huffmanTree)
        {
            this.bitStream = new BitStream(baseStream);
            this.huffmanTree = huffmanTree;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int b = this.ReadByte();
                if (b < 0)
                    return i;
                buffer[offset + i] = (byte)b;
            }
            return count;
        }

        public override int ReadByte()
        {
            int nodeIndex = 0; // root
            while (true)
            {
                CompactHuffmanTreeNode node = huffmanTree.Nodes[nodeIndex];
                if (node.IsLeaf)
                    return node.Symbol;

                int bit = bitStream.ReadBit();
                if (bit < 0) // EOF
                    return -1;
                
                if (bit == 0)
                    nodeIndex = node.ZeroBitChildIndex;
                else
                    nodeIndex = node.OneBitChildIndex;
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

    /// <summary>
    /// Represents a compactly stored huffman tree that encodes values 0-255.
    /// </summary>
    /// <remarks>
    /// See Format.txt for a description of the huffman tree storage format.
    /// </remarks>
    class CompactHuffmanTree
    {
        readonly CompactHuffmanTreeNode[] nodes;

        public CompactHuffmanTree(Int16[] nodeValues)
        {
            this.nodes = new CompactHuffmanTreeNode[nodeValues.Length];
            for (int i = 0; i < nodeValues.Length; i++)
                this.nodes[i] = new CompactHuffmanTreeNode(i, nodeValues[i]);
        }

        public CompactHuffmanTreeNode[] Nodes
        {
            get { return nodes; }
        }

        public void Dump()
        {
            Dump(0, 0);
        }

        private void Dump(int nodeIndex, int depth)
        {
            System.Diagnostics.Debug.Write(nodeIndex.ToString("000"));
            CompactHuffmanTreeNode node = nodes[nodeIndex];

            if (node.IsLeaf)
            {
                byte b = node.Symbol;
                if (b > 32 && b < 127)
                    System.Diagnostics.Debug.WriteLine("=" + (char)b);
                else
                    System.Diagnostics.Debug.WriteLine("=" + b.ToString("X2"));
            }
            else
            {
                System.Diagnostics.Debug.Write("-");
                Dump(node.OneBitChildIndex, depth + 1);

                System.Diagnostics.Debug.Write(new string(' ', 4 * (depth + 1)));
                Dump(node.ZeroBitChildIndex, depth + 1);
            }
        }
    }

    /// <summary>
    /// Represents a node in a compactly stored huffman tree.
    /// </summary>
    struct CompactHuffmanTreeNode
    {
        readonly short nodeIndex;
        readonly short nodeValue;

        public CompactHuffmanTreeNode(int nodeIndex, short nodeValue)
        {
            this.nodeIndex = (short)nodeIndex;
            this.nodeValue = nodeValue;
        }

        /// <summary>
        /// Gets a flag that indicates whether this node is a leaf node.
        /// </summary>
        public bool IsLeaf
        {
            get { return nodeValue < 0; }
        }

        /// <summary>
        /// Gets the symbol encoded by this node.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If this node is not a leaf node.
        /// </exception>
        public byte Symbol
        {
            get
            {
                if (!IsLeaf)
                    throw new InvalidOperationException("This node is not a leaf node.");
                return (byte)nodeValue;
            }
        }

        /// <summary>
        /// Gets the index of the child node that encodes a 0 bit.
        /// </summary>
        public int ZeroBitChildIndex
        {
            get
            {
                if (IsLeaf)
                    throw new InvalidOperationException("This node is not an internal node.");
                return nodeValue / 2;
            }
        }

        /// <summary>
        /// Gets the index of the child node that encodes a 1 bit.
        /// </summary>
        public int OneBitChildIndex
        {
            get
            {
                if (IsLeaf)
                    throw new InvalidOperationException("This node is not an internal node.");
                return nodeIndex + 1;
            }
        }
    }
}
