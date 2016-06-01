using RDFCommon;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlIsBlank : SparqlExpression
    {
      

        public SparqlIsBlank(SparqlExpression value)  :base(value.AggregateLevel, value.IsStoreUsed)
        {
            //SetExprType(ObjectVariantEnum.Bool);
            if(value.Const!=null)
                Const=new OV_bool(value.Const is IBlankNode);
            else
            {
                Operator = result => value.TypedOperator(result) is IBlankNode;
                TypedOperator = result => new OV_bool(value.TypedOperator(result) is IBlankNode); //todo     
            }
            
        }
    }
}
