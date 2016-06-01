using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;

namespace SparqlParseRun.SparqlClasses.Query.Result
{
    public class SparqlGroupOfResults : SparqlResult
    {
        public IEnumerable<SparqlResult> Group;

        public SparqlGroupOfResults(VariableNode variable, ObjectVariants value, RdfQuery11Translator q) : base(q)
        {
            Add(variable, value);
        }

        public SparqlGroupOfResults(RdfQuery11Translator q) : base(q)
        {
          
        }

        public SparqlGroupOfResults(IEnumerable<VariableNode> variables, List<ObjectVariants> values, RdfQuery11Translator q)
            : base(q)
        {
            int i = 0;
            var valuesArray = values.ToArray();
            foreach (var variable in variables)
            {
                i++;
                if(variable==null) continue;
                Add(variable, valuesArray[i]);
            }   
        }
    }
}