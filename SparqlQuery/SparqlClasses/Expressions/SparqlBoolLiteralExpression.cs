using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
   public class SparqlBoolLiteralExpression : SparqlExpression
    {

        public SparqlBoolLiteralExpression(ObjectVariants sparqlLiteralNode)
        {
            Const = sparqlLiteralNode;
            
        }

       
    }

}
