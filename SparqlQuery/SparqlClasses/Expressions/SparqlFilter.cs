using System.Collections.Generic;
using System.Linq;
using SparqlQuery.SparqlClasses.GraphPattern;
using SparqlQuery.SparqlClasses.Query.Result;

namespace SparqlQuery.SparqlClasses.Expressions
{
    public class SparqlFilter : ISparqlGraphPattern
    {
   
      //  public Func<IEnumerable<Action>> SelectVariableValuesOrFilter { get; set; }
      //   public SparqlResultSet resultSet;
   //     private SparqlConstraint sparqlConstraint;
        public readonly SparqlExpression SparqlExpression;

        public SparqlFilter(SparqlExpression sparqlExpression)
        {
            // TODO: Complete member initialization
            this.SparqlExpression = sparqlExpression;
        }

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {                    
            return variableBindings.Where(SparqlExpression.Test);
        }

        public SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.Filter;} }
    }
}