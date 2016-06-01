using System.Collections.Generic;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.InlineValues
{
    public class SparqlInlineVariable : HashSet<ObjectVariants>,ISparqlGraphPattern
    {
        private readonly VariableNode variableNode;

        public SparqlInlineVariable(VariableNode variableNode)
        {
            // TODO: Complete member initialization
            this.variableNode = variableNode;
        }

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> bindings)
        {
            ObjectVariants exists;
            foreach (SparqlResult result in bindings)
            {
                exists = result[variableNode];
                if (exists != null)
                {
                    if (this.Contains(exists)) yield return result; //TODO test
                }
                else
                {
                   
                    foreach (var newvariableBinding in this)
                        yield return
                            result.Add(newvariableBinding, variableNode);
                    result[variableNode] = null;
                }
            }
        }

        public SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.InlineDataValues;} }
    }
}
