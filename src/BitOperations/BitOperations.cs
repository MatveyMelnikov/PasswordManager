using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operations
{
    internal static class BitOperations
    {
        // Methods
        public static bool GetBit(in byte data, in byte position)
        {
            return (data & (1 << position)) != 0;
        }

        public static bool GetBit(in byte[] data, in BitPosition position)
        {
            return GetBit(data[position.BytePosition], position.BitInBytePosition);
        }

        public static void SetBit(ref byte data, in byte position, in bool value)
        {
            data &= (byte)(~(1 << position));
            data |= (byte)(Convert.ToByte(value) << position);
        }

        public static void SetBit(ref byte[] data, in BitPosition position, in bool value)
        {
            SetBit(ref data[position.BytePosition], position.BitInBytePosition, value);
        }

        public static void SwapBits(ref byte[] data, in BitPosition positionA, in BitPosition positionB)
        {
            bool tmp = GetBit(data, positionA);
            SetBit(ref data, positionA, GetBit(data, positionB));
            SetBit(ref data, positionB, tmp);
        }

        /// <summary>
        /// Swap bits if on their position in the template is one
        /// </summary>
        public static void SwapBitsByTemplate(ref byte[] data, in byte[] template)
        {
            BitPosition? swapPosition = null;

            for (int bytesIndex = 0; bytesIndex < template.Length; bytesIndex++)
            {
                for (byte bitsIndex = 0; bitsIndex < 8; bitsIndex++)
                {
                    if (BitOperations.GetBit(template[bytesIndex], bitsIndex))
                    {
                        if (swapPosition == null)
                        {
                            swapPosition = new BitPosition(bytesIndex, bitsIndex);
                        }
                        else
                        {
                            BitPosition currentPosition = new BitPosition(bytesIndex, bitsIndex);

                            BitOperations.SwapBits(
                                ref data,
                                swapPosition ?? default(BitPosition),
                                currentPosition
                            );
                            swapPosition = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Performs an exclusive or on a subsequence of the elements of the first array. 
        /// The numbering of the second array remains the same
        /// </summary>
        public static void XORBytes(ref byte[] dataA, in byte[] dataB, in int positionA, in int positionB)
        {
            for (int i = positionA; i < positionB; i++)
            {
                dataA[i] ^= dataB[i - positionA];
            }
        }

        /// <summary>
        /// Performs exclusive or for all elements of the first array. 
        /// Arrays must be the same size
        /// </summary>
        public static void XORBytes(ref byte[] dataA, in byte[] dataB)
        {
            XORBytes(ref dataA, dataB, 0, dataA.Length);
        }

        public static void PrintBytes(in byte[] data)
        {
            for (int bytesIndex = 0; bytesIndex < data.Length; bytesIndex++)
            {
                for (byte bitsIndex = 0; bitsIndex < 8; bitsIndex++)
                {
                    Console.Write(GetBit(data[bytesIndex], bitsIndex) ? 1 : 0);
                }
                Console.Write($" ({data[bytesIndex]})\n");
            }
        }
    }
}
