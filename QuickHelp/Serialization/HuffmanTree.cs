using System;
using System.Collections.Generic;
using System.Text;

namespace QuickHelp.Serialization
{
    using HuffmanTreeNode = BinaryTreeNode<byte>;

    /// <summary>
    /// Represents a huffman tree that encodes a subset of the symbols 0-255.
    /// </summary>
    /// <remarks>
    /// See Format.txt for a description of the huffman tree storage format.
    /// </remarks>
    public class HuffmanTree
    {
        public BinaryTreeNode<byte> Root { get; set; }

        /// <summary>
        /// Deserializes a proper binary tree.
        /// </summary>
        public static HuffmanTree Deserialize(Int16[] nodeValues)
        {
            int n = nodeValues.Length;
            if (n == 0)
                return null;

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
    }

    /// <summary>
    /// Represents a node in a binary tree.
    /// </summary>
    public class BinaryTreeNode<T>
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
    }
}