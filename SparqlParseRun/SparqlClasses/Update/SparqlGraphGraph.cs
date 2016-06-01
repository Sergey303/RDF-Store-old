using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.Update  
{
    public class SparqlGraphGraph :  ISparqlGraphPattern 
    {
        private SparqlGraphPattern sparqlTriples;
        public ObjectVariants Name;

        public SparqlGraphGraph(ObjectVariants sparqlNode)
        {
            // TODO: Complete member initialization
            Name = sparqlNode;
        }
      
        internal void AddTriples(SparqlGraphPattern sparqlTriples)
        {
            this.sparqlTriples = sparqlTriples;
        }

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            return sparqlTriples.Run(variableBindings);
        }

        public SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.Graph;} }

        
        public IEnumerable<SparqlTriple> GetTriples()
        {
            return sparqlTriples.Where(pattern => Equals(pattern.PatternType, SparqlGraphPatternType.SparqlTriple))
                .Cast<SparqlTriple>();
        }

       
    }

}
