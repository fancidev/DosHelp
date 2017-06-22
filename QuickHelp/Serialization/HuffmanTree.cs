using System;
using System.Collections.Generic;
using System.Text;

namespace QuickHelp.Serialization
{
    /// <summary>
    /// Represents a huffman tree that encodes a subset of the symbols 0-255.
    /// </summary>
    /// <remarks>
    /// See Format.txt for a description of the huffman tree storage format.
    /// </remarks>
    public class HuffmanTree
    {
        private readonly HuffmanTreeNode[] nodes;

        public HuffmanTree(Int16[] nodeValues)
        {
            this.nodes = new HuffmanTreeNode[nodeValues.Length];
            for (int i = 0; i < nodeValues.Length; i++)
                this.nodes[i] = new HuffmanTreeNode(i, nodeValues[i]);
        }

        public HuffmanTreeNode[] Nodes
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
            HuffmanTreeNode node = nodes[nodeIndex];

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
    public struct HuffmanTreeNode
    {
        readonly short nodeIndex;
        readonly short nodeValue;

        public HuffmanTreeNode(int nodeIndex, short nodeValue)
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
