using System.Collections.Generic;
using System.Linq;
using SparqlQuery.SparqlClasses.Query.Result;
using SparqlQuery.SparqlClasses.SparqlAggregateExpression;

namespace SparqlQuery.SparqlClasses.SolutionModifier
{
    public class SparqlSolutionModifierGroup : List<SparqlGroupConstraint>
    {
        private readonly RdfQuery11Translator q;

        public SparqlSolutionModifierGroup(RdfQuery11Translator q)
        {
            this.q = q;
        }

        public IEnumerable<SparqlGroupOfResults> Group(IEnumerable<SparqlResult> enumerable)
        {
               if(Count==1)
                   if (this[0].Variable != null)
                       return enumerable.GroupBy(result => this[0].Constrained(result))
                           .Select(grouping =>
                               new SparqlGroupOfResults(this[0].Variable, grouping.Key, q) {Group = grouping});
                   else
                       return
                           enumerable.GroupBy(result => this[0].Constrained(result))
                               .Select(grouping => new SparqlGroupOfResults(q) {Group = grouping});


            return enumerable
                .GroupBy(result =>this.Select(constraint => constraint.Constrained(result)).ToList(), new CollectionEqualityComparer())
                .Select(grouping => new SparqlGroupOfResults(this.Select(constraint => constraint.Variable), grouping.Key, q) { Group = grouping });
        }
    }
}
