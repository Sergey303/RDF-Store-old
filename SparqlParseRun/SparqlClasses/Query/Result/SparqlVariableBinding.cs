using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;

namespace SparqlParseRun.SparqlClasses.Query.Result
{
    public class SparqlVariableBinding
    {
        public readonly VariableNode Variable;
        private readonly ObjectVariants value;


        public SparqlVariableBinding(VariableNode variable, ObjectVariants node)
        {
            // TODO: Complete member initialization
            Variable = variable;
            value = node;
        }

        public ObjectVariants Value { get { return value; } }
    }
}
