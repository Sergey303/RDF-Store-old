using System;
using RDFCommon;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlLCase :SparqlExpression
    {
        public SparqlLCase(SparqlExpression value)        :base(value.AggregateLevel, value.IsStoreUsed)
        {
            if (value.Const != null)
                Const = value.Const.Change(o => o.ToLower());
            else
            {
                Operator = result => value.Operator(result).ToLower();
                TypedOperator = result => value.TypedOperator(result).Change(o => o.ToLower());
            }
        }
    }
}
