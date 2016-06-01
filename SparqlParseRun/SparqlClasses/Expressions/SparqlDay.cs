using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlDay : SparqlExpression
    {
        public SparqlDay(SparqlExpression value)
            : base(value.AggregateLevel, value.IsStoreUsed)
        {
            if (value.Const != null)
                Const = new OV_int(GetDay(value.Const.Content));
            else
            {
                Operator = result => GetDay(value.Operator(result));
                TypedOperator = result => new OV_int(Operator(result));
            }
        }

        private int GetDay(dynamic o)
        {
            if (o is DateTime)
                return ((DateTime)o).Day;
            if (o is DateTimeOffset)
                return ((DateTimeOffset)o).Day;
            throw new ArgumentException();
        }
    }
}
