using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlIriExpression : SparqlExpression
    {
        public SparqlIriExpression(string sparqlUriNode, NodeGenerator q)
        {
            var uri = q.GetUri(sparqlUriNode);
            Func = result => uri;
        }

        public SparqlIriExpression(ObjectVariants sparqlUriNode)
        {
            Func = result => sparqlUriNode;
        }
    }
}
