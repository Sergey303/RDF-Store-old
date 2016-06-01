using System;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlNow : SparqlExpression
    {
        public SparqlNow()    :base(VariableDependenceGroupLevel.UndependableFunc, false)
        {
            Operator = res => DateTime.UtcNow;
            TypedOperator=res=>new OV_dateTime(DateTime.UtcNow);
        }
    }
}
