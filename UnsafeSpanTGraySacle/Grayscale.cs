namespace UnsafeSpanTGraySacle
{
    public static class Grayscale
    {
        public static void GrayscaleStandard(byte[] pixels)
        {
            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte r = pixels[i];
                byte g = pixels[i + 1];
                byte b = pixels[i + 2];
                byte gray = (byte)((r * 0.3) + (g * 0.59) + (b * 0.11));
                pixels[i] = pixels[i + 1] = pixels[i + 2] = gray;
            }
        }

        public unsafe static void GrayscaleAdvanced(byte[] pixels)
        {
            fixed (byte* ptr = pixels)
            {
                byte* p = ptr;
                byte* end = ptr + pixels.Length;

                while (p < end)
                {
                    byte r = p[0];
                    byte g = p[1];
                    byte b = p[2];
                    byte gray = (byte)((r * 0.3f) + (g * 0.59f) + (b * 0.11f));
                    p[0] = p[1] = p[2] = gray;
                    p += 4; // Move to the next pixel
                }
            }
        }

    }
}
