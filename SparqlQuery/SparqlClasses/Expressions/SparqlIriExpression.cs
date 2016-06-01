using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlIriExpression : SparqlExpression
    {
        public SparqlIriExpression(string sparqlUriNode, NodeGenerator q) 
        {
            //TypedOperator = result => uri;
            Const = q.GetUri(sparqlUriNode);
        }

        public SparqlIriExpression(ObjectVariants sparqlUriNode)
        {
            Const = sparqlUriNode;
        }
    }
}
