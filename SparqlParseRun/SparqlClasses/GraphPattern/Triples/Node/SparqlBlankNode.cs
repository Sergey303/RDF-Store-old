using RDFCommon;

namespace SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node
{
    public class SparqlBlankNode : VariableNode, IBlankNode
    {


        public SparqlBlankNode(string varName, int count):base(varName, count)
        {
            
        }

        public SparqlBlankNode(int count)    :base("blank var", count)
        {
         
        }


        public string Name
        {
            get { return VariableName; }
        }


        
    }
}