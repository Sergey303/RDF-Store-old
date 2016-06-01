using System;
using System.Linq;
using RDFCommon;
using SparqlQuery.SparqlClasses.GraphPattern;
using SparqlQuery.SparqlClasses.GraphPattern.Triples;

namespace SparqlQuery.SparqlClasses.Update
{
    public class SparqlUpdateDeleteData : ISparqlUpdate
    {
        private readonly SparqlQuadsPattern sparqlQuadsPattern;

        public SparqlUpdateDeleteData(SparqlQuadsPattern sparqlQuadsPattern)
        {
            // TODO: Complete member initialization
            this.sparqlQuadsPattern = sparqlQuadsPattern;
        }

        public void Run(IStore store)
        {
            throw new NotImplementedException();
            foreach (var triple in sparqlQuadsPattern
                .Where(pattern => pattern.PatternType == SparqlGraphPatternType.SparqlTriple)
                .Cast<SparqlTriple>())
            {
                store.Delete(triple.Subject, triple.Predicate, triple.Object);
                
            }

            foreach (var sparqlGraphGraph in
                sparqlQuadsPattern.Where(pattern => Equals(pattern.PatternType, SparqlGraphPatternType.Graph))
                    .Cast<SparqlGraphGraph>())                                                                     
            {
               // if (sparqlGraphGraph.Name == null)
                    //store.NamedGraphs.DeleteFromAll(
                      //  sparqlGraphGraph.GetTriples().Select(t => new Triple<ObjectVariants, ObjectVariants>(t.Subject, t.Predicate, t.Object)));
                //store.NamedGraphs.Delete(sparqlGraphGraph.Name, sparqlGraphGraph.GetTriples().Select(t => new Triple<ObjectVariants, ObjectVariants>(t.Subject, t.Predicate, t.Object)));
            }
        }
    }
}
