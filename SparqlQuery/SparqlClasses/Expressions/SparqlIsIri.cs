using RDFCommon;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlIsIri : SparqlExpression
    {
        

        public SparqlIsIri(SparqlExpression value)
            : base(value.AggregateLevel, value.IsStoreUsed)
        {
            if (value.Const != null)
                Const = new OV_bool(value.Const is IIriNode);
            else
            {
                Operator = result => value.TypedOperator(result) is IIriNode;
                TypedOperator = result => new OV_bool(value.TypedOperator(result) is IIriNode); //todo 
            }
            //SetExprType(ObjectVariantEnum.Bool);
            

        }
    }
}
