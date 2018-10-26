using System;
using System.Collections;

public static class UIDHandler
{
    private static readonly int charLength = 8;
    private static BitArray CurrentBits;

    public static BitArray Next()
    {
        if (CurrentBits == null)
        {
            CurrentBits = new BitArray(charLength * 6, false);
        }
        else
        {
            CurrentBits = BinaryConverter.IncreaseByOne(CurrentBits);
        }

        return CurrentBits;
    }
}

internal class BinaryConverter
{
    public static BitArray ToBinary(int numeral, int Length)
    {
        BitArray binary = new BitArray(new int[] { numeral });
        bool[] bits = new bool[Length];
        binary.CopyTo(bits, 0);
        return binary;
    }

    public static int ToNumeral(BitArray binary, int length)
    {
        int numeral = 0;
        for (int i = 0; i < length; i++)
        {
            if (binary[i])
            {
                numeral = numeral | (((int)1) << (length - 1 - i));
            }
        }
        return numeral;
    }

    public static byte ConvertToByte(BitArray bits)
    {
        if (bits.Count != 8)
        {
            throw new ArgumentException("bits");
        }
        byte[] bytes = new byte[1];
        bits.CopyTo(bytes, 0);
        return bytes[0];
    }

    public static BitArray IncreaseByOne(BitArray ba)
    {
        bool increase = false;
        for (int i = 0; i < ba.Count; i++)
        {
            if (!ba[i])
            {
                ba[i] = true;
                increase = true;
                break;
            }
            else
            {
                ba[i] = false;
            }
        }
        if (increase)
        {
            return ba;
        }
        else
        {
            throw new ArgumentException("ba is already maxSize");
        }
    }
}

public class _UUID
{
    public string code;

    public _UUID()
    {
        BitArray ba = UIDHandler.Next();
        byte[] bA = new byte[ba.Count / 8];
        for (int b = 0; b < ba.Length; b += 8)
        {
            bA[b / 8] = BinaryConverter.ConvertToByte(new BitArray(new bool[]
            {
                ba[b],
                ba[b + 1],
                ba[b + 2],
                ba[b + 3],
                ba[b + 4],
                ba[b + 5],
                ba[b + 6],
                ba[b + 7],
            }));
        }
        code = Convert.ToBase64String(bA);
    }
}