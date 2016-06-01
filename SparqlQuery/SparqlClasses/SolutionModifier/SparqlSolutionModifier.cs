using System;
using System.Collections.Generic;
using System.Linq;
using SparqlQuery.SparqlClasses.Query.Result;

namespace SparqlQuery.SparqlClasses.SolutionModifier
{
    public class SparqlSolutionModifier
    {
        public SparqlSolutionModifier()
        {
            //if (isDistinct)
            //Modifier = enumerable => enumerable.Distinct();
        }

        private Func<IEnumerable<SparqlResult>, IEnumerable<SparqlResult>> LimitOffset;
        
        private Func<IEnumerable<SparqlResult>, IEnumerable<SparqlGroupOfResults>> Group;
        public SparqlSelect Select;
        private SparqlSolutionModifierHaving sparqlSolutionModifierHaving;
        private RdfQuery11Translator q;
        private SparqlSolutionModifierOrder sparqlSolutionModifierOrder;

        internal void Add(SparqlSolutionModifierLimit sparqlSolutionModifierLimit)
        {
        
                LimitOffset = sparqlSolutionModifierLimit.LimitOffset;
        }

        internal void Add(SparqlSolutionModifierOrder sparqlSolutionModifierOrder)
        {
            this.sparqlSolutionModifierOrder = sparqlSolutionModifierOrder;
        }

        internal void Add(SparqlSolutionModifierHaving sparqlSolutionModifierHaving, RdfQuery11Translator q)
        {
            this.sparqlSolutionModifierHaving = sparqlSolutionModifierHaving;
            this.q = q;
        }

        internal void Add(SparqlSolutionModifierGroup sparqlSolutionModifierGroup)
        {
            Group = sparqlSolutionModifierGroup.Group;
        }

        internal void Add(SparqlSelect projection)
        {
            Select = projection;
        }
        public IEnumerable<SparqlResult> Run( IEnumerable<SparqlResult> results, SparqlResultSet sparqlResultSet)
        {
            if (Group != null)
            {
                var groupedResults = Group(results.Select(r => r.Clone()));
                if (sparqlSolutionModifierHaving != null)
                    groupedResults= sparqlSolutionModifierHaving.Having4CollectionGroups(groupedResults, q);

                if (sparqlSolutionModifierOrder != null)
                    groupedResults= sparqlSolutionModifierOrder.Order4Grouped(groupedResults).Cast<SparqlGroupOfResults>();

                var res = groupedResults.Cast<SparqlResult>();
                if (Select != null)
                    res = Select.Run(res, sparqlResultSet, true);

                if (LimitOffset != null)
                    res = LimitOffset(res);
                return res;
            }
            else
            {
                if (sparqlSolutionModifierHaving != null)
                    results = sparqlSolutionModifierHaving.Having(results, q);

                if (sparqlSolutionModifierOrder != null)
                    results = sparqlSolutionModifierOrder.Order(results.Select(r => r.Clone()));

                if (Select != null)
                    results = Select.Run(results, sparqlResultSet, false);

                if (LimitOffset != null)
                    results = LimitOffset(results);
                return results;
            }
        }
    }
}
