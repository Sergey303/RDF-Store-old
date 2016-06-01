using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
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
