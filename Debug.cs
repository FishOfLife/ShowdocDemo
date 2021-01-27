using System;
using System.Configuration;

namespace Showdoc
{
    public static class Debug
    {
        public static bool IsTest { get; } = ConfigurationManager.AppSettings["debug"].Equals("true");

        public static void Log(object obj)
        {
            Console.WriteLine(string.Format("==== {0}", obj));
        }

        public static void LogTest()
        {
            if (!IsTest)
            {
                return;
            }
            Console.WriteLine();
        }

        public static void LogTest(object obj)
        {
            if (!IsTest)
            {
                return;
            }
            Console.WriteLine(string.Format("[Test] {0}", obj));
        }

        public static void LogWarnning(object obj)
        {
            Console.WriteLine(string.Format("[Warnning] {0}", obj));
        }

        public static void LogError(object obj)
        {
            Console.WriteLine(string.Format("[Error] {0}", obj));
        }

        public static void ReadKey()
        {
            Console.ReadKey();
        }

    }

}
