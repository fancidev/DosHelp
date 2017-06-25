using System;
using System.Collections.Generic;
using System.IO;

namespace QuickHelp.Serialization
{
    /// <summary>
    /// Maintains a list of keywords used for keyword compression (also known
    /// as dictionary substitution).
    /// </summary>
    /// <remarks>
    /// The number of keywords must not exceed 1024.
    /// </remarks>
    public class KeywordList : List<byte[]>
    {

    }

    internal static class KeywordListSerializer
    {
        public static KeywordList Deserialize(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            // Serialized sequentially as length-prefixed string.
            KeywordList keywords = new KeywordList();
            using (BinaryReader reader = new BinaryReader(new MemoryStream(buffer)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    byte length = reader.ReadByte();
                    byte[] keyword = reader.ReadBytes(length);
                    keywords.Add(keyword);
                }
            }
            return keywords;
        }
    }
}
