using System.Collections.Generic;
using System.Linq;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses
{
    public class SparqlUnionGraphPattern : List<ISparqlGraphPattern>, ISparqlGraphPattern
    {
       

        public SparqlUnionGraphPattern(ISparqlGraphPattern sparqlGraphPattern)
        {
            // TODO: Complete member initialization
            Add(  sparqlGraphPattern);
        }
       

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            return this.SelectMany(graphPattern => graphPattern.Run(variableBindings));
        }

        public SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.Union;} }
    }
}
