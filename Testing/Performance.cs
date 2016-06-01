using System;
using System.Diagnostics;
using System.IO;

namespace TestingNs
{
    public static class Performance
    {
        private static Stopwatch timer = new Stopwatch();

        /// <summary>
        /// Выводит в консоль время исполнения
        /// </summary>
        /// <param name="action">тестируемый метод</param>
        /// <param name="mesage"></param>
        /// <param name="outputFile">if true, write result at file</param>
        public static void ComputeTime(this Action action, string mesage, bool outputFile = false, string pathOutputFile=@"..\..\Performance.txt")
        {
            timer.Restart();
            action.Invoke();
            timer.Stop();
            if (outputFile)
                using (StreamWriter file = new StreamWriter(pathOutputFile, true))
                    file.WriteLine("{0} {1}ms", mesage, timer.ElapsedMilliseconds); 
                Console.WriteLine("{0} {1}ms", mesage, timer.ElapsedMilliseconds);
        }
    }
}