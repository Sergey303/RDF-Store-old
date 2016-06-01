using System.Collections.Generic;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.GraphPattern
{
    public interface ISparqlGraphPattern   
    {
        IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings);
        SparqlGraphPatternType PatternType { get; }
    }
}