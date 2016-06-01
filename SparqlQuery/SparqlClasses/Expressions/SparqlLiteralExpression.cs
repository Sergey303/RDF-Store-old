using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
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
