using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlHours : SparqlExpression
    {
        public SparqlHours(SparqlExpression value)
            : base(value.AggregateLevel, value.IsStoreUsed)
        {
            if (value.Const != null)
                Const = new OV_int(GetHours(value.Const.Content));
            else
            {
                Operator = result => GetHours(value.Operator(result));
                TypedOperator = result => new OV_int(Operator(result));
            }
        }

        private int GetHours(dynamic o)
        {
            if (o is DateTime)
                return ((DateTime)o).Hour;
            if (o is DateTimeOffset)
                return ((DateTimeOffset)o).Hour;
            throw new ArgumentException();
        }
    }
}
