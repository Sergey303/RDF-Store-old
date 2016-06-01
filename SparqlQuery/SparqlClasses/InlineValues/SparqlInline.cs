using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlQuery.SparqlClasses.GraphPattern;
using SparqlQuery.SparqlClasses.GraphPattern.Triples.Node;
using SparqlQuery.SparqlClasses.Query.Result;

namespace SparqlQuery.SparqlClasses.InlineValues
{
    public class SparqlInline : ISparqlGraphPattern
    {
        private readonly List<VariableNode> variables = new List<VariableNode>();
        private int currentVariableIndex=0; 
        public List<SparqlVariableBinding[]> VariablesBindingsList=new List<SparqlVariableBinding[]>();
        internal void AddVar(VariableNode variableNode)
        {
            variables.Add(variableNode);
        }

        internal void AddValue(ObjectVariants sparqlNode)
        {
              if (currentVariableIndex == 0)
                   VariablesBindingsList.Add(new SparqlVariableBinding[variables.Count]);
            if (sparqlNode is SparqlUnDefinedNode) { currentVariableIndex++; return; }
            VariablesBindingsList.Last()[currentVariableIndex] = new SparqlVariableBinding(variables[currentVariableIndex], sparqlNode);
            currentVariableIndex++;     
        }

        internal void NextListOfVarBindings()
        {
            currentVariableIndex = 0;
          
        }

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> bindings)
        {
            ObjectVariants exists;
            foreach (SparqlResult result in bindings)
            {
                foreach (var arrayofBindings in VariablesBindingsList)
                {
                    bool iSContinue = false;
                    foreach (var sparqlVariableBinding in arrayofBindings.Where(binding => binding!=null))
                    {
                        exists = result[sparqlVariableBinding.Variable];
                        if (exists!=null)
                        {
                            if (exists.Equals(sparqlVariableBinding.Value)) continue;
                            iSContinue = true;
                            break;
                        }
                        else result.Add(sparqlVariableBinding.Variable, sparqlVariableBinding.Value);}
                    if (iSContinue) continue;
                    yield return result;
                }
            }
        }

        public SparqlGraphPatternType PatternType { get { return SparqlGraphPatternType.InlineDataValues; } }

    }
}
