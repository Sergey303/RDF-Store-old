using System;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlStringLang  :SparqlExpression
    {
        public SparqlStringLang(SparqlExpression literalExpression, SparqlExpression langExpression)
        {
            switch (NullablePairExt.Get(literalExpression.Const, langExpression.Const))
            {
                case NP.bothNull:
                    Operator = literalExpression.Operator;
                    TypedOperator = result => new OV_langstring((string)literalExpression.Operator(result), (string)langExpression.Operator(result));
                    AggregateLevel = SetAggregateLevel(literalExpression.AggregateLevel, langExpression.AggregateLevel);
                    break;
                case NP.leftNull:
                    Operator = literalExpression.Operator;
                    TypedOperator = result => new OV_langstring((string)literalExpression.Operator(result), (string) langExpression.Const.Content);
                    AggregateLevel = literalExpression.AggregateLevel;
                    break;
                case NP.rigthNull:                                       
                    Operator = result => literalExpression.Const.Content;
                    TypedOperator = result => new OV_langstring((string) literalExpression.Const.Content, langExpression.Operator(result));
                    AggregateLevel = langExpression.AggregateLevel;
                    break;
                case NP.bothNotNull:
                    Const = new OV_langstring((string) literalExpression.Const.Content, (string) langExpression.Const.Content);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
    }
}
