using System;
using System.Text;

namespace QuickHelp.Serialization
{
    public class Graphic437Encoding : Encoding
    {
        private static readonly Encoding CP437 = Encoding.GetEncoding(437);
        private const string GraphicCharacters = "\0☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼¶§▬↨↑↓→←∟↔▲▼";

        public static bool IsControlCharacter(char c)
        {
            return (c < 32) || (c == 127);
        }

        public static bool ContainsControlCharacter(string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            for (int i = 0; i < s.Length; i++)
            {
                if (IsControlCharacter(s[i]))
                    return true;
            }
            return false;
        }

        public static void SubstituteControlCharacters(char[] chars)
        {
            if (chars == null)
                throw new ArgumentNullException("chars");

            SubstituteControlCharacters(chars, 0, chars.Length);
        }

        public static void SubstituteControlCharacters(char[] chars, int index, int count)
        {
            if (chars == null)
                throw new ArgumentNullException("chars");
            if (index < 0 || index > chars.Length)
                throw new ArgumentOutOfRangeException("index");
            if (count < 0 || count > chars.Length - index)
                throw new ArgumentOutOfRangeException("count");

            for (int i = index; i < index + count; i++)
            {
                if (chars[i] < 32)
                    chars[i] = GraphicCharacters[chars[i]];
                else if (chars[i] == 127)
                    chars[i] = '⌂';
            }
        }

        public static string SubstituteControlCharacters(string s)
        {
            if (s == null)
                return null;

            if (!ContainsControlCharacter(s))
                return s;

            char[] chars = s.ToCharArray();
            SubstituteControlCharacters(chars);
            return new string(chars);
        }

        public override int GetByteCount(char[] chars, int index, int count)
        {
            return CP437.GetByteCount(chars, index, count);
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return CP437.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return CP437.GetCharCount(bytes, index, count);
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            int charCount = CP437.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
            SubstituteControlCharacters(chars, charIndex, charCount);
            return charCount;
        }

        public override int GetMaxByteCount(int charCount)
        {
            return CP437.GetMaxByteCount(charCount);
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return CP437.GetMaxCharCount(byteCount);
        }
    }
}
