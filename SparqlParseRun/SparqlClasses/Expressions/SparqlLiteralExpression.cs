using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlLiteralExpression : SparqlExpression
    {

        public SparqlLiteralExpression(ObjectVariants sparqlLiteralNode)
        {
            Const = sparqlLiteralNode;
            //SetExprType(sparqlLiteralNode.Variant);
        }


    }
}
