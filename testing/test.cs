using System;
using System.IO;
using System.Text;

namespace MyNamespace
{
    class Program
    {
        static void SecretMaliciousFunction(int a, int b)
        {
            Console.WriteLine(a + b);// This is a harmless function
        }


        static void Main()
        {
            string text = "Hello, World!";
            int a = 9;
            int b = 11;
            Console.WriteLine(text);
            SecretMaliciousFunction(a, b);
        }
    }
}