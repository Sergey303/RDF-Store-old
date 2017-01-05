using System.Collections.Generic;
using RDFCommon.OVns;
using SparqlQuery.SparqlClasses.GraphPattern.Triples.Node;

namespace SparqlQuery.SparqlClasses.Query.Result
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

        public SparqlGroupOfResults(ObjectVariants[] rowArray, RdfQuery11Translator q)
            :base(rowArray, q)
        {
        }

        public override SparqlResult Clone()
        {
            return new SparqlGroupOfResults(base.rowArray, base.q) {Group = Group};
        }
    }
}