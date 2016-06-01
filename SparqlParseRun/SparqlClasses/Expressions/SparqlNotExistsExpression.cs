using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern;

namespace SparqlParseRun.SparqlClasses
{
    public class SparqlNotExistsExpression : SparqlExistsExpression
    {
             
        public SparqlNotExistsExpression(ISparqlGraphPattern sparqlGraphPattern)
            : base(sparqlGraphPattern)
        {
            // TODO: Complete member initialization
            //this.sparqlGraphPattern = sparqlGraphPattern;
            Operator = variableBinding => !sparqlGraphPattern.Run(Enumerable.Repeat(variableBinding, 1)).Any(); 
        }
    }
}
