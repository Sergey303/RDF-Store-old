using RDFCommon;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlIsLiteral : SparqlExpression
    {
        public SparqlIsLiteral(SparqlExpression value)
            : base(value.AggregateLevel, value.IsStoreUsed)
        {
            //SetExprType(ObjectVariantEnum.Bool);

            if (value.Const != null)
                Const = new OV_bool(value.Const is ILiteralNode);
            else
            {
                Operator = result => value.TypedOperator(result) is ILiteralNode;
                TypedOperator = result => new OV_bool(value.TypedOperator(result) is ILiteralNode); //todo 
            }
        }
    }
}
