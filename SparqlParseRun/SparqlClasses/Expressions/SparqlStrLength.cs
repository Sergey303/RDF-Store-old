using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlStrLength  : SparqlExpression
    {
        public SparqlStrLength(SparqlExpression value) :base(value.AggregateLevel, value.IsStoreUsed)
        {
            if (value.Const != null)
                Const = new OV_int(((string) value.Const.Content).Length);
            else
            {
                Operator = result => ((string) value.TypedOperator(result).Content).Length;
                TypedOperator = result => new OV_int(((string) value.TypedOperator(result).Content).Length);
            }
        }
    }
}
