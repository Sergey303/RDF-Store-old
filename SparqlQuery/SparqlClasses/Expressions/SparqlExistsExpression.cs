using System.Linq;
using RDFCommon.OVns;
using SparqlQuery.SparqlClasses.GraphPattern;

namespace SparqlQuery.SparqlClasses.Expressions
{
    public class SparqlExistsExpression :SparqlExpression
    {
        //private SparqlGraphPattern sparqlGraphPattern;

        public SparqlExistsExpression(ISparqlGraphPattern sparqlGraphPattern)   : base(VariableDependenceGroupLevel.SimpleVariable, true)
        {
            // TODO: Complete member initialization
            //this.sparqlGraphPattern = sparqlGraphPattern;
            //SetExprType(ObjectVariantEnum.Bool);
            Operator = variableBinding => sparqlGraphPattern.Run(Enumerable.Repeat(variableBinding, 1)).Any();
            TypedOperator = variableBinding => new OV_bool(Operator(variableBinding));
        }

    }
}
