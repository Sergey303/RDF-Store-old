using RDFCommon;
using SparqlQuery.SparqlClasses.GraphPattern.Triples.Node;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlVarExpression : SparqlExpression
    {
       public VariableNode Variable;

        public SparqlVarExpression(VariableNode variableNode) :base(VariableDependenceGroupLevel.SimpleVariable, false)
        {
            // TODO: Complete member initialization
            Variable = variableNode;
            Operator = result =>
            {
                //ObjectVariants sparqlVariableBinding;
                ////if (result.TryGetValue(Variable, out sparqlVariableBinding))
                ////    return sparqlVariableBinding;
                ////else return new SparqlUnDefinedNode();
                var value = result[variableNode];
                if(value is IIriNode)
                    return value;
                else return value.Content;
            };
            TypedOperator = result =>
            {
                //ObjectVariants sparqlVariableBinding;
                ////if (result.TryGetValue(Variable, out sparqlVariableBinding))
                ////    return sparqlVariableBinding;
                ////else return new SparqlUnDefinedNode();
                return result[variableNode];
            };
        }

       
       

      
    }
}
