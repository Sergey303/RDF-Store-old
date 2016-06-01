using System.Collections.Generic;
using SparqlQuery.SparqlClasses.Query.Result;

namespace SparqlQuery.SparqlClasses.GraphPattern
{
    public interface ISparqlGraphPattern   
    {
        IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings);
        SparqlGraphPatternType PatternType { get; }
    }
}