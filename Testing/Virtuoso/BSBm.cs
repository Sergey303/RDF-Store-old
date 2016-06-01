using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using sema2012m;
using TestingNs;
using VirtuosoTest;

namespace VirtuosoBigData
{
   public class BSBm
    {
        public static void RunBerlinsWithConstants(EngineVirtuoso Store)
        {
            double[] memoryUsage = new double[50];
            long[] totalrun = new long[50];
            Console.WriteLine("bsbm with constants");
            var timer = new Stopwatch();

            //  for (
            int i = 49;
            //i < 12; i++)
            {
                string file = string.Format(@"..\..\..\Testing\examples\bsbm\queries\with constants\{0}.rq", i + 1);
                var qu = "sparql " + File.ReadAllText(file);
                GC.Collect();

                if (qu.Contains("SELECT "))
                {

                    timer.Restart();

                    var select_qu = Store.Query(qu);
                    foreach (var objectse in select_qu)
                    {

                    }

                    timer.Stop();
                }
                else
                {
                    timer.Restart();
                    Store.Execute(qu);
                    timer.Stop();
                }
                totalrun[i] += timer.ElapsedMilliseconds > 10
                    ? timer.ElapsedMilliseconds
                    : ((int)(timer.ElapsedTicks / 10000) * 100) / 100;
                memoryUsage[i] = GC.GetTotalMemory(false);
                //results[i]=

            }
            using (StreamWriter r = new StreamWriter(@"..\..\output.txt", true))
            {
                r.WriteLine("date time " + DateTime.Now);
                r.WriteLine("memory usage (bytes)" + string.Join(", ", memoryUsage));
                r.WriteLine("run " + string.Join(", ", totalrun));
                r.WriteLine("total run " + totalrun.Sum());
            }

        }

        private static void RunTests(EngineVirtuoso engine)
        {
            string[] query_files = 
            {
                @"D:\Home\FactographDatabases\queries\query1.rq",
                @"D:\Home\FactographDatabases\queries\query2.rq",
                @"D:\Home\FactographDatabases\queries\query3.rq",
                @"D:\Home\FactographDatabases\queries\query4.rq",
                @"D:\Home\FactographDatabases\queries\query5.rq",
                @"D:\Home\FactographDatabases\queries\query6.rq",
                @"D:\Home\FactographDatabases\queries\query7.rq",
                @"D:\Home\FactographDatabases\queries\query8.rq",
                @"D:\Home\FactographDatabases\queries\query9.rq",
                @"D:\Home\FactographDatabases\queries\query10.rq",
                @"D:\Home\FactographDatabases\queries\query11.rq",
                @"D:\Home\FactographDatabases\queries\query12.rq",
            };
            string[] queries = new string[query_files.Length];
            for (int i = 0; i < query_files.Length; i++)
            {
                System.IO.StreamReader sr = new StreamReader(query_files[i]);
                queries[i] = "sparql " + sr.ReadToEnd();
            }
            int f = 0, t = query_files.Length;
            DateTime tt0;
            for (int i = f; i < t; i++)
            {
                tt0 = DateTime.Now;
                string qu = queries[i];
                //if (i == 5) continue;
                if (qu.Contains("SELECT "))
                {

                    //var res = engine.Query(queries[i]).ToArray();
                    var select_qu = engine.Query(queries[i]);
                    int limit = 3;
                    foreach (var v in select_qu)
                    {
                        int len = v.Length;
                        foreach (var ob in v) Console.Write("{0} ", ob);
                        Console.WriteLine();
                        limit--;
                        if (limit <= 0) break;
                    }
                    //Console.Write(" {0} ", res.Count());
                }
                else
                {
                    var res = engine.Execute(queries[i]); // engine.Query(queries[i]).ToArray();
                }
                Console.WriteLine("query {0} time = {1}", i + 1, (DateTime.Now - tt0).Ticks / 10000L); tt0 = DateTime.Now;
            }
        }
        private static void RunMany(EngineVirtuoso engine)
        {
            //System.IO.StreamReader sr = new StreamReader(@"D:\Home\FactographDatabases\queries\query5param.rq");
            System.IO.StreamReader sr = new StreamReader(@"D:\Home\FactographDatabases\queries\query5param.rq");
            string query_param = "sparql " + sr.ReadToEnd();
            DateTime tt0 = DateTime.Now;
            //foreach (string prod in Sarr.sarr)
            //{
            //    string query_s = query_param.Replace("%ProductXYZ%", "<" + prod + ">");
            //    var res = engine.Query(query_s).ToArray();
            //    Console.WriteLine("query {0} time = {1}", res.Count(), (DateTime.Now - tt0).Ticks / 10000L); tt0 = DateTime.Now;
            //}
            int n1 = 500, s2 = 500, n2 = 500;
            string[] sa = Sarr.sarr;
            for (int i = 0; i < n1; i++)
            {
                string prod = sa[i];
                string query_s = query_param.Replace("%ProductXYZ%", "<" + prod + ">");
                var res = engine.Query(query_s).ToArray();
            }
            var dur1 = (DateTime.Now - tt0).Ticks / 10000L;
            Console.WriteLine("1-st test ok. time = {0} number={1} QpS={2}", dur1, n1, (double)(n1 * 1000) / (double)dur1); tt0 = DateTime.Now;
            tt0 = DateTime.Now;
            for (int i = 0; i < n2; i++)
            {
                string prod = sa[s2 + i];
                string query_s = query_param.Replace("%ProductXYZ%", "<" + prod + ">");
                var res = engine.Query(query_s).ToArray();
            }
            var dur = (DateTime.Now - tt0).Ticks / 10000L;
            Console.WriteLine("test ok. time = {0} number={1} QpS={2}", dur, n2, (double)(n2 * 1000) / (double)dur); tt0 = DateTime.Now;

        }

  
        private static string[] sarr = new string[] {
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer1/Product1", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer1/Product21", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer1/Product41", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer2/Product61", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer2/Product81", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer3/Product101", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer3/Product121", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer4/Product141", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer4/Product161", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer5/Product181", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer5/Product201", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer6/Product221", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer6/Product241", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer6/Product261", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer7/Product281", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer7/Product301", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer7/Product321", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer8/Product341", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer8/Product361", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer8/Product381", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer9/Product401", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer9/Product421", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer9/Product441", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer10/Product461", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer11/Product481", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer11/Product501", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer11/Product521", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer12/Product541", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer12/Product561", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer12/Product581", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer13/Product601", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer13/Product621", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer14/Product641", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer14/Product661", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer14/Product681", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer15/Product701", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer15/Product721", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer16/Product741", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer16/Product761", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer16/Product781", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer17/Product801", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer17/Product821", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer18/Product841", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer19/Product861", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer19/Product881", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer20/Product901", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer20/Product921", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer20/Product941", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer21/Product961", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer21/Product981", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer22/Product1001", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer22/Product1021", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer22/Product1041", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer23/Product1061", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer23/Product1081", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer23/Product1101", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer24/Product1121", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer25/Product1141", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer25/Product1161", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer25/Product1181", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer26/Product1201", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer26/Product1221", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer26/Product1241", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer27/Product1261", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer27/Product1281", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer28/Product1301", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer29/Product1321", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer29/Product1341", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer30/Product1361", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer31/Product1381", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer31/Product1401", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer32/Product1421", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer32/Product1441", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer32/Product1461", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer32/Product1481", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer33/Product1501", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer33/Product1521", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer33/Product1541", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer35/Product1561", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer35/Product1581", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer36/Product1601", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer36/Product1621", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer37/Product1641", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer37/Product1661", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer37/Product1681", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer37/Product1701", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer38/Product1721", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer38/Product1741", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer39/Product1761", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer39/Product1781", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer39/Product1801", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer39/Product1821", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer40/Product1841", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer40/Product1861", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer41/Product1881", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer41/Product1901", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer41/Product1921", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer41/Product1941", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer42/Product1961", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer42/Product1981", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer42/Product2001", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer42/Product2021", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer43/Product2041", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer43/Product2061", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer44/Product2081", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer45/Product2101", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer45/Product2121", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer45/Product2141", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer46/Product2161", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer47/Product2181", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer47/Product2201", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer47/Product2221", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer48/Product2241", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer48/Product2261", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer49/Product2281", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer49/Product2301", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer50/Product2321", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer50/Product2341", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer50/Product2361", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer50/Product2381", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer51/Product2401", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer51/Product2421", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer51/Product2441", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer52/Product2461", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer53/Product2481", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer53/Product2501", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer54/Product2521", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer54/Product2541", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer54/Product2561", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer55/Product2581", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer55/Product2601", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer56/Product2621", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer57/Product2641", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer58/Product2661", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer58/Product2681", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer58/Product2701", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer59/Product2721", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer59/Product2741", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer59/Product2761", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer60/Product2781", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer61/Product2801", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer61/Product2821", 
            "http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer61/Product2841", 
        };
        private static void ViruosoBSBmParameters(EngineVirtuoso engine)
        {
            long[] results = new long[12];
            double[] minimums = Enumerable.Repeat(double.MaxValue, 12).ToArray();
            double[] maximums = new double[12];
            double maxMemoryUsage = 0;
            Console.WriteLine("antrl parametered");
            int i = 0;

            int Millions = 1;

            using (StreamReader streamQueryParameters = new StreamReader(string.Format(@"..\..\..\Testing\examples\bsbm\queries\parameters\param values for{0} m.txt", Millions)))
            {
                int j;
                for (j = 0; j < 500; j++)
                    for (i = 0; i < 12; i++)
                    {
                        var file = new FileInfo(string.Format(@"..\..\..\Testing\examples\bsbm\queries\parameters\{0}.rq", i + 1));

                        using (StreamReader sr = new StreamReader(file.FullName))
                        {
                            var queryReadParameters = "sparql " +
                                                      BSBmParams.QueryReadParameters(sr.ReadToEnd(),
                                                          streamQueryParameters);

                            queryReadParameters = queryReadParameters.Replace("18.04.2008 0:00:00",
                                DateTime.Parse("18.04.2008 0:00:00").ToString("s"));

                            if (queryReadParameters.Contains("SELECT "))
                            {
                                var res = engine.Query(queryReadParameters).ToArray();
                            }
                            else
                            {
                                var res = engine.Execute(queryReadParameters); // engine.Query(queries[i]).ToArray();
                            }
                        }
                    }
                for (j = 0; j < 500; j++)
                {

                    for (i = 0; i < 12; i++)
                    {
                        var totalMilliseconds = 0;//OneParametred(engine, i, TODO);
                        if (minimums[i] > totalMilliseconds)
                            minimums[i] = totalMilliseconds;
                        if (maximums[i] < totalMilliseconds)
                            maximums[i] = totalMilliseconds;
                        results[i++] += totalMilliseconds;
                        var memory = GC.GetTotalMemory(false);
                        if (maxMemoryUsage < memory)
                            maxMemoryUsage = memory;
                        //File.WriteAllText(Path.ChangeExtension(file.FullName, ".txt"), resultString);
                        //.Save(Path.ChangeExtension(file.FullName,".xml"));
                    }
                }
            }
            using (StreamWriter r = new StreamWriter(@"..\..\output.txt", true))
            {
                r.WriteLine("mils " + Millions);
                r.WriteLine(string.Join(", ", results.Select(l => 500 * 1000 / l)));
                r.WriteLine("minimums " + string.Join(", ", minimums));
                r.WriteLine("maximums " + string.Join(", ", maximums));
                r.WriteLine("max memory usage " + maxMemoryUsage);
            }
            Console.WriteLine("average " + string.Join(", ", results.Select(l => 500 * 1000 / l)));
            Console.WriteLine("minimums " + string.Join(", ", minimums));
            Console.WriteLine("maximums " + string.Join(", ", maximums));
        }

        private static void OneParametred(EngineVirtuoso engine, int i, int count)
        {
            using (StreamReader streamQueryParameters = new StreamReader(string.Format(
                @"..\..\..\Testing\examples\bsbm\queries\parameters\param values for{0}m {1} query.txt", 1, i)))
            {
                var file =
                    new FileInfo(string.Format(@"..\..\..\Testing\examples\bsbm\queries\parameters\{0}.rq", i));
                var parametred = "sparql " + File.ReadAllText(file.FullName);
                double min = int.MaxValue, max = -1, average = 0;
                for (int j = 0; j < count; j++)
                {
                    var consted = BSBmParams.QueryReadNewParameters(parametred, streamQueryParameters);
                    consted = consted.Replace("16.05.2008 0:00:00", DateTime.Parse("16.04.2008 0:00:00").ToString("s"));

                    Stopwatch timer = new Stopwatch();
                    if (consted.Contains("SELECT "))
                    {
                        timer.Restart();
                        var res = engine.Query(consted).ToArray();
                        timer.Stop();
                    }
                    else
                    {
                        timer.Restart();
                        var res = engine.Execute(consted); // engine.Query(queries[i]).ToArray();
                        timer.Stop();
                    }
                    double time = SparqlTesting.GetTimeWthLast2Digits(timer);
                    average += (double)((int)(100 * time / count)) / 100;
                    if (time > max) max = time;
                    if (min > time) min = time;
                }
                using (StreamWriter r = new StreamWriter(@"..\..\output.txt", true))
                {
                    r.WriteLine(DateTimeOffset.Now);
                    r.WriteLine("q " + i);
                    r.WriteLine("average " + average);
                    r.WriteLine("qps " + ((double)((int)(100000 / average)) / 100));
                    r.WriteLine("min " + min);
                    r.WriteLine("max " + max);
                    r.WriteLine("memory " + GC.GetTotalMemory(false));
                }
            }
        }

        public static void RunTestParametred(AdapterVirtuoso engine, int iq = 5, int count = 100)
        {
            var paramvaluesFilePath =
                string.Format(@"..\..\..\Testing\examples\bsbm\queries\parameters\param values for{0}m {1} query.txt", 1, iq);
            var file = new FileInfo(string.Format(@"..\..\..\Testing\examples\bsbm\queries\parameters\{0}.rq", iq));

            using (StreamReader streamParameters = new StreamReader(paramvaluesFilePath))
            using (StreamReader streamQuery = new StreamReader(file.OpenRead()))
            {
                string qparams = "sparql " + streamQuery.ReadToEnd();
                Stopwatch timer = new Stopwatch();
                for (int j = 0; j < count; j++)
                {
                    string q = BSBmParams.QueryReadParameters(qparams, streamParameters);
                    timer.Start();
                  engine.Query(q).Count();
                    timer.Stop();
                }

                using (StreamWriter r = new StreamWriter(@"..\..\output.txt", true))
                {
                    r.WriteLine();
                    r.WriteLine("one query {0}, {1} times", iq, count);
                    r.WriteLine("milions " + 1);
                    r.WriteLine("date time " + DateTime.Now);
                    r.WriteLine("total ms " + timer.ElapsedMilliseconds);
                    double l = timer.ElapsedMilliseconds / count;
                    r.WriteLine("ms на запрос в среденем " + l);
                    r.WriteLine("qps " + (int)(1000.0 / l));
                    r.WriteLine("next results count: {0}",
                        engine.Query(BSBmParams.QueryReadParameters(qparams, streamParameters)).Count());
                }
            }
        } 
    }
}