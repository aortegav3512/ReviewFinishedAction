using System;
using System.Collections;

namespace ReviewFinishedAction
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Environment variables:");

            var variables = Environment.GetEnvironmentVariables();
            if (variables != null)
            {
                foreach (DictionaryEntry de in variables)
                {
                    Console.WriteLine("  {0} = {1}", de.Key, de.Value);
                }
            }
        }
    }
}
