using System;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples;

namespace SparqlParseRun.SparqlClasses.Update
{
    public class SparqlUpdateInsertData : ISparqlUpdate
    {
        private readonly SparqlQuadsPattern sparqlQuadsPattern;

        public SparqlUpdateInsertData(SparqlQuadsPattern sparqlQuadsPattern)
        {
            // TODO: Complete member initialization
            this.sparqlQuadsPattern = sparqlQuadsPattern;
        }
        public  void Run(IStore store)   
        {
            throw new NotImplementedException();
            //store.Insert( SparqlQuadsPattern.Where(pattern => pattern.PatternType == SparqlGraphPatternType.SparqlTriple)
            //    .Cast<SparqlTriple>().Select(t=>new TripleOV(t.Subject, t.Predicate,t.Object)));
            //foreach (var sparqlGraph in
            //        SparqlQuadsPattern.Where(pattern => pattern.PatternType == SparqlGraphPatternType.Graph)
            //            .Cast<SparqlGraphGraph>()
            //            //.Where(graph => store.NamedGraphs.ContainsGraph(graph.Name))
            //            )
            //    store.NamedGraphs.Insert(sparqlGraph.Name, sparqlGraph.GetTriples().Select(t => new TripleOV(t.Subject, t.Predicate, t.Object)));
        }
    }
}
