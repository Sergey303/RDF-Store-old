using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.Interfaces;
using RDFCommon.OVns;
using RDFTurtleParser;


namespace TestingNs
{
    /// <summary>
    /// Расширения для интерфейсов    <see cref="RDFCommon.IGraph<string,string,ObjectVariants>"/> и <see cref="RDFCommon.IGraph<Ts,Tp,To>"/>
    /// </summary>
    public static class Testing
    {
        /// <summary>
        /// запускает Build и  замеряет время.
        /// </summary>
        /// <param name="graph"> тестируемый граф должен реализовать интерфейс <see cref="RDFCommon.IGraph<string,string,ObjectVariants>"/></param>
        /// <param name="millions">в данных пока предполагаются варианты: 1, 10, 100, 1000</param>
        public static void TestReadTtl(this IGraph<Triple<string, string, ObjectVariants>> graph, int millions)
        {
            Performance.ComputeTime(() =>
                graph.Build(
                    ReadTripleStringsFromTurtle.LoadGraph(
                        Config.Source_data_folder_path + millions + ".ttl")), "build " + millions + ".ttl ", true);
        }          

        /// <summary>
        /// запускает Build и  замеряет время.
        /// </summary>
        /// <param name="graph"> тестируемый граф должен реализовать интерфейс <seealso cref="RDFCommon.IGraph<string,string,ObjectVariants>"/></param>
        /// <param name="turtleFileName"> путь к внешнему файлу ttl</param>
        public static void TestReadTtl(this IGraph<Triple<string, string, ObjectVariants>> graph, string turtleFileName)
        {
            Performance.ComputeTime(() =>
                graph.Build(
                    ReadTripleStringsFromTurtle.LoadGraph(turtleFileName)),
                "build " + turtleFileName + " ", true);
        }

        /// <summary>
        /// запускает Build и  замеряет время.
        ///    использует <see cref="TripleGeneratorBufferedParallel"/>
        /// 
        /// </summary>
        /// <param name="graph"> тестируемый граф должен реализовать интерфейс <see cref="RDFCommon.IGraph<string,string,ObjectVariants>"/></param>
        /// <param name="millions">в данных пока предполагаются варианты: 1, 10, 100, 1000</param>
        public static void TestReadTtl_Cocor(this IGraph<Triple<string, string, ObjectVariants>> graph, int millions)
        {

            Performance.ComputeTime(() =>
            {
                var generator = new TripleGeneratorBufferedParallel(Config.Source_data_folder_path + millions + ".ttl", "g");
                graph.Build(generator);
            },
                "build " + millions + ".ttl ", true);
        }

        /// <summary>
        /// Я пока не смог организовать выдачу потока триплетов из парсера.
        /// Парсер выполняет делегат для каждого триплета.
        /// Этот метод группирует триплеты в буфер и выполняет указанный делегат над буфером триплетов.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="foreachBuffer">делегат выполняемый над буффером, когда тот заполнен, после чего очищает его.</param>
        /// <param name="bufferlength">максимальная длина. Чем больше, тем реже будет выполняется делегат.</param>
        private static void ForeachBuffer(Parser parser, Action<List<Triple<string,string, ObjectVariants>>> foreachBuffer, int bufferlength=1000)
        {
                            var buffer=new List<Triple<string, string, ObjectVariants>>(bufferlength);
         parser.ft = (s, s1, arg3) =>
            {
                buffer.Add(new Triple<string, string, ObjectVariants>(s, s1, arg3));
                if (buffer.Count == bufferlength)
                {
                    foreachBuffer(buffer);
                    //buffer=new List<Triple<string, string, ObjectVariants>>();
                    buffer.Clear();
                }
            };
               parser.Parse();
        }

        /// <summary>
        /// запускает Build и  замеряет время.
        /// </summary>
        /// <param name="graph"> тестируемый граф должен реализовать интерфейс <seealso cref="RDFCommon.IGraph<string,string,ObjectVariants>"/></param>
        /// <param name="turtleFileName"> путь к внешнему файлу ttl</param>
        public static void TestReadTtl_Cocor(this IGraph<Triple<string, string, ObjectVariants>> graph, string turtleFileName)
        {
            Performance.ComputeTime(() =>
                graph.Build(new TripleGeneratorBufferedParallel(turtleFileName,"g")),
                "build " + turtleFileName + " ", true);
        }


        /// <summary>
        /// Замеряет время:
        ///  1) поток всех триплетов ограничен 100 триплетами;
        ///  2) заменяет субъекты объектами, если они uri и проводит поиск; 
        ///  3) поиск только по предикаьам взятым из первых 100 триплетов; 
        /// </summary>
        /// <typeparam name="Ts"></typeparam>
        /// <typeparam name="Tp"></typeparam>
        /// <typeparam name="To"></typeparam>
        /// <param name="graph"></param>
        public static void TestSearch(this IGraph<Triple<string, string, ObjectVariants>> graph)
        {
            var all = graph.Search();
            Triple<string, string, ObjectVariants>[] ts100 = null;
            Performance.ComputeTime(() =>
            {
                ts100 = all.Take(100).ToArray();
            }, "get first's 100 triples ", true);
            Performance.ComputeTime(() =>
            {
                foreach (var t in ts100)
                {
                    if (t.Object.Variant == ObjectVariantEnum.Iri)
                        graph.Search(((OV_iri) t.Object).UriString).ToArray();

                }
            }, "search by object as subject from first's 100 triples ", true);
            Performance.ComputeTime(() =>
            {
                foreach (var t in ts100)
                {
                    graph.Search(predicate: t.Predicate).ToArray();

                }
            }, "search by predicate from first's 100 triples ", true);
            Performance.ComputeTime(() =>
            {
                foreach (var t in ts100)
                {
                    var triples = graph.Search(t.Subject, t.Predicate, t.Object).ToArray();
                    if (!triples.All(
                        tt => tt.Subject == t.Subject && tt.Predicate == t.Predicate && tt.Object == t.Object))
                        throw new Exception();
                }
            }, "search by subject predicate and object from first's 100 triples, compare correctness ", true);
            
        }

      
    }
}
