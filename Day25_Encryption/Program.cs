using System;

namespace Day25_Encryption
{
    class Program
    {
        static void Main(string[] args)
        {
            //test data
            //var doorPublicKey = 17807724;
            //var cardPublicKey = 5764801;

            //real data
            var doorPublicKey = 8252394;
            var cardPublicKey = 6269621;

            int doorLoopSize;

            long value = 1;
            for (doorLoopSize = 0; value != doorPublicKey; doorLoopSize++)
            {
                value = EncryptionStep(value, 7);
            }

            Console.WriteLine($"Door loop size: {doorLoopSize}");

            long encryptionKey = 1;

            for (int i = 0; i < doorLoopSize; i++)
            {
                encryptionKey = EncryptionStep(encryptionKey, cardPublicKey);
            }

            Console.WriteLine($"Part 1: Encryption key: {encryptionKey}");
        }

        static long EncryptionStep(long value, long subjectMatter)
        {
            value = value * subjectMatter;
            value = value % 20201227;

            return value;
        }
    }
}
