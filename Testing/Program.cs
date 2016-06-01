using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDFCommon.OVns;
using RDFTripleStore;
using sema2012m;
using VirtuosoTest;

namespace TestingNs
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch timer;
            Store store=new Store("../../../Databases/");
             timer=new Stopwatch();
            timer.Start();
            store.ReloadFrom(10*1000*1000, @"C:\deployed\10M.ttl");
            timer.Stop();
            Console.WriteLine(timer.Elapsed.TotalSeconds);
            store.ActivateCache();

            foreach (var tripleOvStruct in store.GetTriplesWithTextObject(new OV_string("prayers")))
            {
                Console.WriteLine(tripleOvStruct.Subject.ToString());
                Console.WriteLine(tripleOvStruct.Predicate.ToString());
                Console.WriteLine(tripleOvStruct.Object.ToString());
                Console.WriteLine();
            }

            SparqlTesting.OneBerlinParametrized(store, 6, 100);
        }

        private static void Virtuoso()
        {
            //Engine engine=new EngineVirtuoso("HOST=localhost:1550;UID=dba;PWD=dba;Charset=UTF-8;Connection Timeout=5000;", "g");
            var engine = new AdapterVirtuoso("HOST=localhost:1550;UID=dba;PWD=dba;Charset=UTF-8;Connection Timeout=5000;", "g");
            Stopwatch timer = new Stopwatch();
            timer.Start();
            //   engine.Load(Enumerable.Range(0,1*10*1000).SelectMany(i=>Enumerable.Range(0, 10).Select(j=>Tuple.Create("s"+i, "p"+ j , (ObjectVariants)new OV_string("o" + i + "" + j)))));
            engine.Query(@"sparql SELECT * WHERE { <s1> ?p ?o  } ").Count();
            ;
            timer.Stop();
            Console.WriteLine(timer.Elapsed.TotalMilliseconds);
            return;
        }
    }
}
