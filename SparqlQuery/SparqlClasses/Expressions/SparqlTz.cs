using System;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlTz : SparqlExpression
    {
        public SparqlTz(SparqlExpression value)   :base(value.AggregateLevel, value.IsStoreUsed)
        {
            if (value.Const != null)
            {
                var f = value.Const.Content;

                if (f is DateTimeOffset)
                    Const = new OV_string(((DateTimeOffset) f).Offset.ToString());
                else if (f is DateTime) Const = new OV_string("");
                throw new ArgumentException();
            }
            else
            {
                Operator = result =>
                {
                    var f = value.Operator(result);
                    if (f is DateTimeOffset)
                    {
                        return ((DateTimeOffset) f).Offset.ToString();
                    }
                    else if (f is DateTime) return "";
                    throw new ArgumentException();
                };
                TypedOperator = result => new OV_string(Operator(result));
            }
        }

       
    }
}
