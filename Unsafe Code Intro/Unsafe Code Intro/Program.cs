using System;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;

namespace Unsafe_Code_Intro
{
    struct MyStruct
    {
        long a;
        long b;
        long c;
        long d;
        long e;
        long a1;
        long b1;
        long c1;
        long d1;
        long e1;
        long a2;
        long b2;
        long c2;
        long d2;
        long e2;
        long a21;
        long b21;
        long c21;
        long d21;
        long e21;
        long a3;
        long b3;
        long c3;
        long d3;
        long e3;
        long a31;
        long b31;
        long c31;
        long d31;
        long e31;
    }
    struct MyOtherStruct
    {
        MyStruct a;
        MyStruct b;
        MyStruct c;
        MyStruct d;
    }

    unsafe struct Blocks
    {
        fixed byte buffer[64];
    }


    class Program
    {
        static Stopwatch sw = new Stopwatch();
        static unsafe void Main(string[] args)
        {

            Console.WriteLine("Hello World!");
            bool temp = true;
            //  while (temp)
            // {
            var bytes = File.ReadAllBytes("Files/FerrariImage.jpg");
            long[] times = new long[100];
            long[] times2 = new long[100];
            for (int i = 0; i < times.Length; i++)
            {
                times[i] = CopyBytesUnsafe(bytes);
                //1308 with byte
                //176 with long
                //110 with struct with 2 longs
                //78 with 3 longs
                //70-80 with 4
                times2[i] = CopyBytesArray(bytes); //28-44
            }
            double average = times.Average();
            double average2 = times2.Average();
            temp = average > average2;
            //  }
            ;
        }

        static unsafe long CopyBytesArray(byte[] bytes)
        {
            byte[] copyArr = new byte[bytes.Length];
            sw.Reset();
            sw.Start();
            Array.Copy(bytes, copyArr, bytes.Length);
            sw.Stop();
            Debug.Assert(bytes.SequenceEqual(copyArr));

            return sw.ElapsedTicks;


        }

        static unsafe long CopyBytesUnsafe(byte[] bytes)
        {
            byte[] copyArr = new byte[bytes.Length];
            sw.Reset();
            sw.Start();
            long[] l = new long[bytes.Length / 8];

            fixed (byte* sBytes = &bytes[0])
            {
                fixed (byte* sCopy = &copyArr[0])
                {
                    //MyOtherStruct* currByte = (MyOtherStruct*)sBytes;
                    //MyOtherStruct* currByteCopy = (MyOtherStruct*)sCopy;
                    Blocks* currByte = (Blocks*)sBytes;
                    Blocks* currByteCopy = (Blocks*)sCopy;
                    byte* endByte = sBytes + bytes.Length;

                    while (currByte < endByte)
                    {
                        *currByteCopy = *currByte;

                        currByte++;
                        currByteCopy++;
                    }


                }
            }


            sw.Stop();
            Debug.Assert(bytes.SequenceEqual(copyArr));

            return sw.ElapsedTicks;


        }
    }
}
