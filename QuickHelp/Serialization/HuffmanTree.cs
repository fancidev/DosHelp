using System;
using System.IO;

namespace QuickHelp.Serialization
{
    using HuffmanTreeNode = BinaryTreeNode<byte>;

    /// <summary>
    /// Represents a huffman tree that encodes a subset of the symbols 0-255.
    /// </summary>
    /// <remarks>
    /// A huffman tree is a proper binary tree that encodes symbols in its
    /// leaf nodes. A proper binary tree is a binary tree where every node
    /// has either two children or no child.
    /// 
    /// For completeness this class supports two special cases: (i) an empty
    /// tree, where no symbol is encoded, and (ii) a tree with a single node,
    /// where the unique symbol is emitted without consuming any input.
    /// 
    /// Use the helper class <c>HuffmanDecoder</c> to decode a symbol.
    /// </remarks>
    public class HuffmanTree
    {
        internal BinaryTreeNode<byte> Root { get; set; }

        public bool IsEmpty
        {
            get { return Root == null; }
        }

        public bool IsSingular
        {
            get { return Root != null && Root.IsLeaf; }
        }

        public static HuffmanTree Deserialize(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            short[] nodeValues = new Int16[512];
            for (int i = 0; i < 512; i++)
            {
                nodeValues[i] = reader.ReadInt16();
                if (nodeValues[i] == 0)
                    return Deserialize(nodeValues);
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
                    node.LeftChild = nodes[child0];
                    node.RightChild = nodes[child1];
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
    /// <remarks>
    /// This class should be used like the following:
    /// <code>
    ///   HuffmanDecoder decoder = new HuffmanDecoder(tree);
    ///   while (!decoder.HasValue) decoder.Push([next bit in the input]);
    ///   Console.WriteLine(decoder.Value);
    /// </code>
    /// Two special cases should be handled with care. If the huffman tree 
    /// is empty, <c>HasValue</c> always contains <c>false</c> and calling
    /// <c>Push()</c> results in an exception. If the huffman tree contains
    /// a single node, <c>HasValue</c> is <c>true</c> upon construction and
    /// the unique symbol is available without consuming any bit. In this 
    /// case caller must know the decoded data length a-priori in order not
    /// to run into an infinite loop.
    /// </remarks>
    public struct HuffmanDecoder
    {
        private HuffmanTreeNode m_node;

        public HuffmanDecoder(HuffmanTree huffmanTree)
        {
            if (huffmanTree == null)
                throw new ArgumentNullException(nameof(huffmanTree));
            m_node = huffmanTree.Root;
        }

        public bool HasValue
        {
            get { return m_node != null && m_node.IsLeaf; }
        }

        public byte Value
        {
            get
            {
                if (!this.HasValue)
                    throw new InvalidOperationException("Decoder does not have a value.");
                return m_node.Value;
            }
        }

        public void Push(bool bit)
        {
            if (m_node == null)
                throw new InvalidOperationException("Cannot walk an empty tree.");
            if (m_node.IsLeaf)
                throw new InvalidOperationException("Cannot walk further from a leaf.");

            m_node = bit ? m_node.RightChild : m_node.LeftChild;
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
    }
}