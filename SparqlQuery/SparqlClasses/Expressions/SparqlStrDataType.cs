using System;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlStrDataType : SparqlExpression
    {
     

        public SparqlStrDataType(SparqlExpression sparqlExpression1, SparqlExpression sparqlExpression2, NodeGenerator q)
        {
            // TODO: Complete member initialization
            switch (NullablePairExt.Get(sparqlExpression1.Const, sparqlExpression2.Const))
            {
                case NP.bothNull:
                    TypedOperator = result => q.CreateLiteralNode((string)sparqlExpression1.Operator(result), (string)sparqlExpression2.Operator(result));
                    Operator = res => sparqlExpression1.Operator(res);
                    AggregateLevel = SetAggregateLevel(sparqlExpression1.AggregateLevel, sparqlExpression2.AggregateLevel);
                    break;
                case NP.leftNull:
                    TypedOperator = result => q.CreateLiteralNode((string)sparqlExpression1.Operator(result), (string)sparqlExpression2.Const.Content);
                    Operator = res => sparqlExpression1.Operator(res);
                    AggregateLevel = sparqlExpression1.AggregateLevel;
                    break;
                case NP.rigthNull:
                    TypedOperator = result => q.CreateLiteralNode((string)sparqlExpression1.Const.Content, (string)sparqlExpression2.Operator(result));
                    Operator = res => sparqlExpression1.Const.Content;
                    AggregateLevel = sparqlExpression2.AggregateLevel;

                    break;
                case NP.bothNotNull:
                    Const = q.CreateLiteralNode((string)sparqlExpression1.Const.Content, (string)sparqlExpression2.Const.Content);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
