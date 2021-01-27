using System;

namespace Showdoc
{
    public class Debug
    {

        public static void Log(object obj)
        {
            Console.WriteLine(string.Format("==== {0}", obj));
        }

        public static void LogTest(object obj)
        {
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
