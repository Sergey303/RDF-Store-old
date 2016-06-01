using System.Collections.Generic;
using System.Linq;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses
{
    public class SparqlOptionalGraphPattern : ISparqlGraphPattern
    {
        private readonly ISparqlGraphPattern sparqlGraphPattern;

        public SparqlOptionalGraphPattern(ISparqlGraphPattern sparqlGraphPattern)
        {
            // TODO: Complete member initialization
            this.sparqlGraphPattern = sparqlGraphPattern;
        }
        
      
        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            
            foreach (var variableBinding in variableBindings)
            {
                var any = false;
                foreach (var resultVariableBinding in sparqlGraphPattern.Run(Enumerable.Repeat(variableBinding, 1)))
                {
                    yield return resultVariableBinding;
                    any = true;
                }

                if (any) continue;

                yield return variableBinding;
            }
        }

        public SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.Optional;} }
    }
}
