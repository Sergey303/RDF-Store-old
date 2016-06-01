
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
   public class SparqlBoolLiteralExpression : SparqlExpression
    {

        public SparqlBoolLiteralExpression(ObjectVariants sparqlLiteralNode)
        {
            Const = sparqlLiteralNode;
            
        }

       
    }

}
