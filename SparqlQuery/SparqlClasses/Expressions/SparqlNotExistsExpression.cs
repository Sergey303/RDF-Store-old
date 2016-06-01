using System.Linq;
using SparqlQuery.SparqlClasses.GraphPattern;

namespace SparqlQuery.SparqlClasses.Expressions
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
