using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.GraphPattern.Triples
{
    public class VariableDataSet : DataSet
    {
        public readonly VariableNode Variable;

        public VariableDataSet(VariableNode variable, IEnumerable<ObjectVariants> @base):base(@base)
            
        {
            Variable = variable;
        }

        //public IEnumerable<IUriNode> GetGraphUri(SparqlResult variablesBindings)
        //{
        //    SparqlVariableBinding fixedGraph;
        //    if (!variablesBindings.row.TryGetValue(Variable, out fixedGraph)) return this;
        //    var uriNode = fixedGraph.Value as IUriNode;
        //    if (uriNode == null) throw new ArgumentOutOfRangeException("graphs variable's value");
        //    return Enumerable.Repeat(uriNode,1);
        //}
    }
}