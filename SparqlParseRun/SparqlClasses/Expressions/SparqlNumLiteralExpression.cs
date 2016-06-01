using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
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
