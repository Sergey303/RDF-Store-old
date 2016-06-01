using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.GraphPattern
{
    public class SparqlMinusGraphPattern : ISparqlGraphPattern
    {
        private readonly ISparqlGraphPattern sparqlGraphPattern;
        private readonly RdfQuery11Translator q;

        public SparqlMinusGraphPattern(ISparqlGraphPattern sparqlGraphPattern, RdfQuery11Translator q)
        {
            // TODO: Complete member initialization
            this.sparqlGraphPattern = sparqlGraphPattern;
            this.q = q;
        }

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            var minusResults =
                sparqlGraphPattern.Run(Enumerable.Repeat(new SparqlResult(q), 1))
             .Select(result => result.Clone()).ToArray()       ;
            var before = variableBindings.Select(result => result.Clone()).ToArray();
            var after = before.Where(result =>
                minusResults.All(minusResult =>
                    minusResult.TestAll((minusVar, minusValue) =>
                    {
                        var value = result[minusVar];
                        return value == null || !Equals(minusValue, value);
                    })));
            return after;
        }

        public SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.Minus;} }
    }
}
