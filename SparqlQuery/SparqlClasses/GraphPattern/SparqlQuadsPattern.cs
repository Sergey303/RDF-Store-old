using System.Collections.Generic;
using System.Linq;
using SparqlQuery.SparqlClasses.Query.Result;

namespace SparqlQuery.SparqlClasses.GraphPattern
{
    public class SparqlQuadsPattern : List<ISparqlGraphPattern>, ISparqlGraphPattern
    {
        public virtual IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            return this.Aggregate(variableBindings, (current, pattern) => pattern.Run(current));
        }

        public virtual SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.ListOfPatterns;} }
    
    }
  
}
