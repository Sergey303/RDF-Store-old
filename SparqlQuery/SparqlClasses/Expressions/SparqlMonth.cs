using System;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlMonth : SparqlExpression
    {
     
        public SparqlMonth(SparqlExpression value)  :base(value.AggregateLevel, value.IsStoreUsed)
        {
            if (value.Const != null)
                Const = new OV_int(GetMonth(value.Const.Content));
            else
            {
                Operator = result => GetMonth(value.Operator(result));
                TypedOperator = result => new OV_int(Operator(result));
            }
        }

        private int GetMonth(dynamic o)
        {
            if (o is DateTime)
                return ((DateTime)o).Month;
            if (o is DateTimeOffset)
                return ((DateTimeOffset)o).Month;
            throw new ArgumentException();
        }
    }
}
