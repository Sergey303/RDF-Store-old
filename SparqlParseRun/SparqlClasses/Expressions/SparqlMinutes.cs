using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlMinutes : SparqlExpression
    {
        public SparqlMinutes(SparqlExpression value):base(value.AggregateLevel, value.IsStoreUsed)
        {
            if (value.Const != null)
                Const = new OV_int(GetMinute(value.Const.Content));
            else
            {
                Operator = result => GetMinute(value.Operator(result));
                TypedOperator = result => new OV_int(Operator(result));
            }
        }

        private static dynamic GetMinute(dynamic o)
        {
            if (o is DateTime)
                return ((DateTime) o).Minute;
            if (o is DateTimeOffset)
                return ((DateTimeOffset) o).Minute;
            throw new ArgumentException();
        }
    }
}
