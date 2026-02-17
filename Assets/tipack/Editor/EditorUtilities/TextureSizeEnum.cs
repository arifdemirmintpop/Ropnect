using System;

namespace tiplay.EditorUtilities
{
    public enum TextureSizeEnum
    {
        _32,
        _64,
        _128,
        _256,
        _512,
        _1024,
        _2048,
        _4096,
        _8192
    }

    public static class TextureSizeEnumUtility
    {
        public static int ToInt(this TextureSizeEnum textureSize)
        {
            return int.Parse(textureSize.ToString().Substring(1));
        }

        public static TextureSizeEnum IntToTextureSize(int textureSize)
        {
            if (Enum.TryParse<TextureSizeEnum>("_" + textureSize, false, out TextureSizeEnum size))
            {
                return size;
            }

            return TextureSizeEnum._2048;
        }
    }
}

