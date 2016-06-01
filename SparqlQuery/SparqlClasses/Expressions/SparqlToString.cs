using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlToString : SparqlExpression
    {
       // private SparqlExpression sparqlExpression;

        public SparqlToString(SparqlExpression child):base(child.AggregateLevel, child.IsStoreUsed)
           
        {
            var childConst = child.Const;
            if (childConst != null) Const =new OV_string(childConst.Content.ToString());
            else
            {
                Operator = result => child.Operator(result).ToString();
                                TypedOperator = result => new OV_string(child.Operator(result).ToString());                
               
            }
        }
    }
}
