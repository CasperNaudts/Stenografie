using System;
using System.Drawing;

namespace Crypto.Stenografie
{
    public class StenografieHelper
    {
        public enum Opdracht
        {
            Verbergen,
            VullenMetNullen
        };

        /// <summary>
        /// TODO: een regel tekst verbergen in een afbeelding dmv een kleine aanpassing van pixels
        /// </summary>
        /// <param name="text"></param>
        /// <param name="afbeelding"></param>
        /// <returns></returns>
        public static Bitmap EmbedText(string text, Bitmap afbeelding)
        {
            Opdracht opdracht = Opdracht.Verbergen;
            int letterPositie = 0;
            int letterWaarde = 0;
            long pixelPositie = 0;
            int nullen = 0;
            int R, G, B;

            for (int i = 0; i < afbeelding.Height; i++)
            {
                for (int j = 0; j < afbeelding.Width; j++)
                {
                    Color pixel = afbeelding.GetPixel(j, i);

                    R = pixel.R - pixel.R % 2;
                    G = pixel.G - pixel.G % 2;
                    B = pixel.B - pixel.B % 2;

                    for (int n = 0; n < 3; n++)
                    {
                        if (pixelPositie % 8 == 0)
                        {
                            if (opdracht == Opdracht.VullenMetNullen && nullen == 8)
                            {
                                if ((pixelPositie - 1) % 3 < 2)
                                {
                                    afbeelding.SetPixel(j, i, Color.FromArgb(R, G, B));
                                }
                                return afbeelding;
                            }

                            if (letterPositie >= text.Length)
                            {
                                opdracht = Opdracht.VullenMetNullen;
                            }
                            else
                            {
                                letterWaarde = text[letterPositie++];
                            }
                        }

                        switch (pixelPositie % 3)
                        {
                            case 0:
                                {
                                    if (opdracht == Opdracht.Verbergen)
                                    {
                                        R += letterWaarde % 2;

                                        letterWaarde /= 2;
                                    }
                                }
                                break;
                            case 1:
                                {
                                    if (opdracht == Opdracht.Verbergen)
                                    {
                                        G += letterWaarde % 2;

                                        letterWaarde /= 2;
                                    }
                                }
                                break;
                            case 2:
                                {
                                    if (opdracht == Opdracht.Verbergen)
                                    {
                                        B += letterWaarde % 2;

                                        letterWaarde /= 2;
                                    }

                                    afbeelding.SetPixel(j, i, Color.FromArgb(R, G, B));
                                }
                                break;
                        }

                        pixelPositie++;

                        if (opdracht == Opdracht.VullenMetNullen)
                        {
                            nullen++;
                        }
                    }
                }
            }
            return afbeelding;
        }

        public static string ExtractText(Bitmap bmp)
        {
            int colorUnitIndex = 0;
            int charValue = 0;

            string extractedText = String.Empty;

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    Color pixel = bmp.GetPixel(j, i);

                    for (int n = 0; n < 3; n++)
                    {
                        switch (colorUnitIndex % 3)
                        {
                            case 0:
                                {
                                    charValue = charValue * 2 + pixel.R % 2;
                                }
                                break;
                            case 1:
                                {
                                    charValue = charValue * 2 + pixel.G % 2;
                                }
                                break;
                            case 2:
                                {
                                    charValue = charValue * 2 + pixel.B % 2;
                                }
                                break;
                        }

                        colorUnitIndex++;

                        if (colorUnitIndex % 8 == 0)
                        {
                            charValue = ReverseBits(charValue);

                            if (charValue == 0)
                            {
                                return extractedText;
                            }

                            char c = (char)charValue;

                            extractedText += c.ToString();
                        }
                    }
                }
            }

            return extractedText;
        }

        public static int ReverseBits(int n)
        {
            int result = 0;

            for (int i = 0; i < 8; i++)
            {
                result = result * 2 + n % 2;

                n /= 2;
            }

            return result;
        }
    }
}
