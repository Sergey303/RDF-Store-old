using System;
using System.Collections.Generic;
using SparqlParseRun.SparqlClasses;

namespace SparqlParseRun
{
    public class OptionalWhere : SparqlWhere
    {
        internal int StartIndex;
        internal int EndIndex;
        private readonly SparqlResultSet sparqlResultSet;

        //public OptionalWhere(SparqlResultSet sparqlResultSet) : base()
        //{
        //    this.sparqlResultSet = sparqlResultSet;
        //}

        public OptionalWhere(SparqlGraphPattern sparqlGraphPattern) : base(sparqlGraphPattern)
        {
        }

        private IEnumerable<Action> Optional(Func<IEnumerable<Action>> graphSelector)
        {
            //var packBox = Enumerable.Repeat(pack, 1); // as RPackInt[] ?? packs.ToArray();

            bool any = false;
            foreach (var rPackInt in graphSelector())
            {
                yield return rPackInt;
                if (!any)
                    any = true;
            }
            if (!any)
                yield return () => sparqlResultSet.ResetDiapason(StartIndex, EndIndex);
        }
    }
}