using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlNumLiteralExpression : SparqlExpression
    {
        public SparqlNumLiteralExpression(ObjectVariants sparqlLiteralNode)  
        {
            //SetExprType(ExpressionTypeEnum.numeric);
            Const = sparqlLiteralNode;
            //TypedOperator = result => sparqlLiteralNode;
        }
    }
}
