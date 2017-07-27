using System;
using ChackCogLib;
using System.Threading.Tasks;

namespace ChackCogLibSample_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var help = true;

            Console.WriteLine();
            Console.WriteLine("ChackCogLib and Console Sample");

            for (var lastArg = 0; lastArg < args.Length; lastArg++)
            {
                if (IsArg(args[lastArg], "h", "help") ||
                    args[lastArg] == "-?" ||
                    args[lastArg] == "/?")
                {
                }
                else if (IsArg(args[lastArg], "info"))
                {
                }
                else if (args[lastArg].StartsWith("-") ||
                         args[lastArg].StartsWith("/"))
                {
                    Console.WriteLine("Unknown option: {0}", args[lastArg]);
                }
                else if (args[lastArg].StartsWith("http"))
                {
                    help = !Ocr(args[lastArg]);
                }
                else
                {
                }
            }

            if (help)
            {
                PrintHelp();
            }
        }

        private static bool Ocr(string imageUrl)
        {
            try
            {
                Uri image = new Uri(imageUrl);

                // Setup for OcrLib
                // 1) Go to https://www.microsoft.com/cognitive-services/en-us/computer-vision-api 
                //    Sign up for computer vision api
                // 2) Add environment variable "Vision_API_Subscription_Key" and set Computer vision key as value
                //    e.g. Vision_API_Subscription_Key=123456789abcdefghijklmnopqrstuvw

                Task<string> task = Task.Run(() => Vision.Ocr(image));
                task.Wait();

                Console.WriteLine();
                Console.WriteLine("OCR Result:");
                Console.WriteLine(task.Result);
                Console.WriteLine();

                return true;
            }
            catch
            {
                Console.WriteLine("Error:");
                Console.WriteLine("Please check internet connection, Vision API Key, and so on.");

                return false;
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine();
            Console.WriteLine("Usage: cclc [Image URL]");
            Console.WriteLine("Usage: cclc [options]");
            Console.WriteLine("Usage: dotnet cclc.dll [Image URL]");
            Console.WriteLine("Usage: dotnet cclc.dll [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  -h|--help  Display help.");
            Console.WriteLine();
        }

        private static bool IsArg(string candidate, string longName)
        {
            return IsArg(candidate, shortName: null, longName: longName);
        }

        private static bool IsArg(string candidate, string shortName, string longName)
        {
            return (shortName != null && candidate.Equals("-" + shortName)) || (longName != null && candidate.Equals("--" + longName));
        }
    }
}
