using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    abstract class SparqlHashExpression : SparqlExpression
    {

        public SparqlHashExpression(SparqlExpression value)
            : base(value.AggregateLevel, value.IsStoreUsed)
        {
        }

        protected void Create(SparqlExpression value)
        {
            if (value.Const != null)
                Const = new OV_string(CreateHash((string) value.Const.Content));
            else
            {
                Operator = result => CreateHash(value.Operator(result));
                TypedOperator = result => new OV_string(Operator(result));
            }
        }

        protected abstract string CreateHash(string f);
    }
}