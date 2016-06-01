using System;

namespace SparqlQuery.SparqlClasses.Expressions
{
    internal class SparqlStrStarts : SparqlExpression
    {


        public SparqlStrStarts(SparqlExpression str, SparqlExpression pattern)
        {
            switch (NullablePairExt.Get(str.Const, pattern.Const))
            {
                case NP.bothNull:
                    Operator = result => str.Operator(result).StartsWith(pattern.Operator(result));
                    TypedOperator =
                        result => str.TypedOperator(result).Change(o => o.StartsWith(pattern.Operator(result)));
                    AggregateLevel = SetAggregateLevel(str.AggregateLevel, pattern.AggregateLevel);
                    break;
                case NP.leftNull:
                    Operator = result => str.Operator(result).StartsWith(pattern.Const.Content);
                    TypedOperator = result => str.TypedOperator(result).Change(o => o.StartsWith(pattern.Const.Content));
                    AggregateLevel = str.AggregateLevel;
                    break;
                case NP.rigthNull:

                    Operator = result => ((string) str.Const.Content).StartsWith(pattern.Operator(result));
                    TypedOperator = result => str.Const.Change(o => o.StartsWith(pattern.Operator(result)));
                    AggregateLevel = pattern.AggregateLevel;
                    break;
                case NP.bothNotNull:
                    Const = str.Const.Change(o => o.StartsWith(pattern.Const.Content));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
