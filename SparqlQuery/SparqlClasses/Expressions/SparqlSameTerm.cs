using System;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlSameTerm : SparqlExpression
    {
        public SparqlSameTerm(SparqlExpression str, SparqlExpression pattern)
        {
            switch (NullablePairExt.Get(str.Const, pattern.Const))
            {
                case NP.bothNull:
                    Operator = result => str.TypedOperator(result).Equals(pattern.TypedOperator(result));
                    AggregateLevel = SetAggregateLevel(str.AggregateLevel, pattern.AggregateLevel);
                    break;
                case NP.leftNull:
                    Operator = result => str.TypedOperator(result).Equals(pattern.Const);
                    AggregateLevel = str.AggregateLevel;
                    break;
                case NP.rigthNull:
                    Operator = result => str.Const.Equals(pattern.TypedOperator(result));
                    AggregateLevel = pattern.AggregateLevel;
                    break;
                case NP.bothNotNull:
                    Const = new OV_bool(str.Const.Equals(pattern.Const));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            TypedOperator = result => new OV_bool(Operator(result));
        }
    }
}
