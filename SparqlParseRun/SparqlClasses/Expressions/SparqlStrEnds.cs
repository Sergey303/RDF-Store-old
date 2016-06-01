using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlStrEnds : SparqlExpression
    {

        public SparqlStrEnds(SparqlExpression str, SparqlExpression pattern)
        {
            switch (NullablePairExt.Get(str.Const, pattern.Const))
            {
                case NP.bothNull:
                    Operator = result => ((string)str.Operator(result)).EndsWith(pattern.Operator(result));
                    AggregateLevel = SetAggregateLevel(str.AggregateLevel, pattern.AggregateLevel);
                    TypedOperator = result => str.TypedOperator(result).Change(o => ((string)o).EndsWith((string)pattern.TypedOperator(result).Content));
                    break;
                case NP.leftNull:
                    Operator = result => ((string)str.Operator(result)).EndsWith((string)pattern.Const.Content);
                    TypedOperator = result => pattern.Const.Change(o => ((string)str.Operator(result)).EndsWith(o));
                    AggregateLevel = str.AggregateLevel;
                    break;
                case NP.rigthNull:
                    Operator = result => ((string)str.Const.Content).EndsWith(pattern.Operator(result));
                    TypedOperator = result => str.Const.Change(o => ((string)o).EndsWith((string)pattern.Operator(result).Content));
                    AggregateLevel = pattern.AggregateLevel;
                    break;
                case NP.bothNotNull:
                    Const = new OV_bool(((string)str.Const.Content).EndsWith((string)pattern.Const.Content));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}
