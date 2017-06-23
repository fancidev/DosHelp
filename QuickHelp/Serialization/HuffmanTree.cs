using System;
using System.IO;

namespace QuickHelp.Serialization
{
    using HuffmanTreeNode = BinaryTreeNode<byte>;

    /// <summary>
    /// Represents a huffman tree that encodes a subset of the symbols 0-255.
    /// </summary>
    /// <remarks>
    /// A huffman tree is a proper binary tree that encodes symbols in the
    /// leaf nodes. A proper binary tree is a binary tree where every node
    /// has either two children or no child.
    /// 
    /// The oddity about the huffman tree used by QuickHelp is that the left
    /// child maps to a bit of 1 and the right child maps to a bit of 0.
    /// 
    /// Use the helper class <c>HuffmanDecoder</c> decode a symbol.
    /// </remarks>
    public class HuffmanTree
    {
        internal BinaryTreeNode<byte> Root { get; set; }

        public static HuffmanTree Deserialize(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            using (BinaryReader reader = new BinaryReader(new MemoryStream(buffer)))
            {
                short[] nodeValues = new Int16[512];
                for (int i = 0; i < 512; i++)
                {
                    nodeValues[i] = reader.ReadInt16();
                    if (nodeValues[i] == 0)
                        return Deserialize(nodeValues);
                }
            }
            throw new InvalidDataException("Huffman tree too long.");
        }

        /// <summary>
        /// Deserializes a huffman tree.
        /// </summary>
        /// <remarks>
        /// See Format.txt for the serialized format of a huffman tree.
        /// </remarks>
        private static HuffmanTree Deserialize(Int16[] nodeValues)
        {
#if DEBUG
            System.Diagnostics.Debug.Assert(nodeValues.Length <= 512);
            System.Diagnostics.Debug.Assert(nodeValues[nodeValues.Length - 1] == 0);
#endif
            if (nodeValues == null || nodeValues.Length == 0)
                return new HuffmanTree();

            int n = nodeValues.Length;
            HuffmanTreeNode[] nodes = new HuffmanTreeNode[n];
            nodes[0] = new HuffmanTreeNode();

            for (int i = 0; i < n; i++)
            {
                HuffmanTreeNode node = nodes[i];
                if (node == null) // not referenced
                    continue;

                short nodeValue = nodeValues[i];
                if (nodeValue < 0) // leaf; symbol stored in low byte
                {
                    node.Value = (byte)nodeValue;
                }
                else // right-child (1 bit) follows, left-child (0 bit) encoded
                {
                    int child0 = nodeValue / 2;
                    int child1 = i + 1;
                    if (!(child1 < child0 && child0 < n))
                        throw new ArgumentException("Tree is invalid.");
                    if (nodes[child0] != null || nodes[child1] != null)
                        throw new ArgumentException("Tree is invalid.");

                    nodes[child0] = new HuffmanTreeNode();
                    nodes[child1] = new HuffmanTreeNode();
                    node.LeftChild = nodes[child1];
                    node.RightChild = nodes[child0];
                }
            }
            return new HuffmanTree { Root = nodes[0] };
        }

#if false
        public void Dump()
        {
            Dump(0, 0);
        }

        internal void Dump(int nodeIndex, int depth)
        {
            System.Diagnostics.Debug.Write(nodeIndex.ToString("000"));


            if (IsLeaf)
            {
                //byte b = node.Symbol;
                //if (b > 32 && b < 127)
                //    System.Diagnostics.Debug.WriteLine("=" + (char)b);
                //else
                //    System.Diagnostics.Debug.WriteLine("=" + b.ToString("X2"));
                System.Diagnostics.Debug.WriteLine(string.Format("={0}", Value));
            }
            else
            {
                //System.Diagnostics.Debug.Write("-");
                //if (LeftChild!=null)
                //LeftChild Dump(node.OneBitChildIndex, depth + 1);

                //System.Diagnostics.Debug.Write(new string(' ', 4 * (depth + 1)));
                //Dump(node.ZeroBitChildIndex, depth + 1);
            }
        }
#endif
    }

#if false
    /// <summary>
    /// Represents a read-only sequence of bits.
    /// </summary>
    public class BitBuffer
    {
        private readonly byte[] m_buffer;
        private readonly int m_startBitIndex;
        private readonly int m_bitCount;

        /// <summary>
        /// Creates a bit sequence from the underlying byte buffer. The bits
        /// are stored in big-endian order, i.e. from MSB to LSB.
        /// </summary>
        public BitBuffer(byte[] buffer, int startBitIndex, int bitCount)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (!(startBitIndex >= 0 && startBitIndex <= buffer.Length * 8))
                throw new ArgumentOutOfRangeException(nameof(startBitIndex));
            if (!(bitCount >= 0 && bitCount <= buffer.Length * 8 - startBitIndex))
                throw new ArgumentOutOfRangeException(nameof(bitCount));

            m_buffer = buffer;
            m_startBitIndex = startBitIndex;
            m_bitCount = bitCount;
        }

        public bool this[int bitIndex]
        {
            get
            {
                if (!(bitIndex >= 0 && bitIndex < m_bitCount))
                    throw new ArgumentOutOfRangeException(nameof(bitIndex));
                int k = m_startBitIndex + bitIndex;
                return (m_buffer[k / 8] >> (7 - (k % 8))) != 0;
            }
        }
    }
#endif

    /// <summary>
    /// Helper class to decode a single symbol from a huffman tree.
    /// </summary>
    public struct HuffmanDecoder
    {
        private HuffmanTreeNode m_node;

        public HuffmanDecoder(HuffmanTree huffmanTree)
        {
            if (huffmanTree == null)
                throw new ArgumentNullException(nameof(huffmanTree));
            m_node = huffmanTree.Root;
        }

        public bool Next(bool bit)
        {
            if (m_node == null)
                throw new InvalidOperationException("Cannot walk an empty tree.");


            HuffmanTreeNode node = bit ? m_node.LeftChild : m_node.RightChild;
            if (node == null)
                throw new InvalidOperationException("Cannot call Next() on a leaf node.");
            m_node = node;
            return !m_node.IsLeaf;
        }

        public byte Symbol
        {
            get { return m_node.Value; }
        }
    }

    /// <summary>
    /// Represents a node in a binary tree.
    /// </summary>
    internal class BinaryTreeNode<T>
    {
        public T Value { get; set; }

        public BinaryTreeNode<T> LeftChild { get; set; }

        public BinaryTreeNode<T> RightChild { get; set; }

        /// <summary>
        /// Returns <c>true</c> if this node has no child.
        /// </summary>
        public bool IsLeaf
        {
            get { return LeftChild == null && RightChild == null; }
        }

        /// <summary>
        /// Returns <c>true</c> if every node has either two children or no
        /// child.
        /// </summary>
        public bool IsProper
        {
            get
            {
                if (LeftChild == null && RightChild == null)
                    return true;
                else if (LeftChild != null && RightChild != null)
                    return LeftChild.IsProper && RightChild.IsProper;
                else
                    return false;
            }
        }
    }
}