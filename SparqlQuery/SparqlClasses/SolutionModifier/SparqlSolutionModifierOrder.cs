using System.Collections.Generic;
using System.Linq;
using SparqlQuery.SparqlClasses.Query.Result;

namespace SparqlQuery.SparqlClasses.SolutionModifier
{
    public class SparqlSolutionModifierOrder  :List<SparqlOrderCondition>
    {
        public IEnumerable<SparqlResult> Order(IEnumerable<SparqlResult> enumerable)
        {
           return this.Aggregate(enumerable, (current, order) => order.Order(current));
        }
        public IEnumerable<SparqlResult> Order4Grouped(IEnumerable<SparqlResult> enumerable)
        {
           return this.Aggregate(enumerable, (current, order) => order.Order4Grouped(current));
        }
    }
}
