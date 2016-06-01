using System;
using RDFCommon.OVns;
using SparqlQuery.SparqlClasses.Expressions;
using SparqlQuery.SparqlClasses.GraphPattern;
using SparqlQuery.SparqlClasses.GraphPattern.Triples.Node;
using SparqlQuery.SparqlClasses.Query.Result;

namespace SparqlQuery.SparqlClasses.SparqlAggregateExpression
{
    public class SparqlGroupConstraint
    {
        public readonly Func<SparqlResult, ObjectVariants> Constrained;

        public SparqlGroupConstraint(SparqlExpression sparqlExpression)
        {
            // TODO: Const

            Constrained = sparqlExpression.TypedOperator;
           
        }

        public SparqlGroupConstraint(SparqlFunctionCall sparqlFunctionCall)
        {
            // TODO: Complete member initialization
            Constrained = sparqlFunctionCall.TypedOperator;
         
        }

        public SparqlGroupConstraint(VariableNode variableNode)
        {
            Constrained = result => result[variableNode];
        }

        public VariableNode Variable { get; set; }

        public SparqlGroupConstraint(SparqlExpressionAsVariable sparqlExpressionAsVariable)
        {
            // TODO: Complete member initialization
            Variable = sparqlExpressionAsVariable.variableNode;
            Constrained = sparqlExpressionAsVariable.RunExpressionCreateBind;
 
        }


    }
}
