using System.Collections.Generic;
using SparqlParseRun.SparqlClasses.GraphPattern;

namespace SparqlParseRun.SparqlClasses
{
    public class InlineDataValues : ISparqlGraphPattern
    {
        private readonly ISparqlDataBlock sparqlDataBlock;

        public InlineDataValues(ISparqlDataBlock sparqlDataBlock)
        {
            this.sparqlDataBlock = sparqlDataBlock;
          
        }

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            return sparqlDataBlock.SetVariables(variableBindings);
        }

        public SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.InlineDataValues;} }
    }
}