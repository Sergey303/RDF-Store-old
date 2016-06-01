using System;
using System.Collections.Generic;
using System.Linq;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.SolutionModifier
{
    public class SparqlSolutionModifierLimit
    {
        private int limit;
        private int offset;
        //public bool IsOffsetFirst

        internal void CreateLimit(int p)
        {
            limit = p;
            if (LimitOffset == null)
                LimitOffset = enumerable => enumerable.Take(limit);
            else
            {
                var limitOffsetClone = LimitOffset.Clone() as Func<IEnumerable<SparqlResult>, IEnumerable<SparqlResult>>;
                LimitOffset = enumerable => limitOffsetClone(enumerable).Take(limit);
            }
        }

        internal void CreateOffset(int p)
        {
            offset = p;
            if (LimitOffset == null)
                LimitOffset = enumerable => enumerable.Skip(offset);
            else
            {
                var limitOffsetClone = LimitOffset.Clone() as Func<IEnumerable<SparqlResult>, IEnumerable<SparqlResult>>;
                LimitOffset = enumerable => limitOffsetClone(enumerable).Skip(offset);
            }  
        }

        //internal void CreateDistinct()
        //{
        //        HashSet<SparqlResult> results = new HashSet<SparqlResult>();
        //    if (offset == -1)
        //        //limit>0
        //        LimitOffset = enumerable =>
        //        {
        //            foreach (var res in enumerable)
        //            {
        //                if (results.Contains(res)) continue;
        //                results.Add(res);
        //                if (results.Count == limit) break;
        //            }
        //            return results;
        //        };
        //    else if (limit == -1)
        //        //offset>0
        //        LimitOffset = enumerable => enumerable.Distinct<SparqlResult>().Skip(offset);
        //    else LimitOffset = enumerable =>
        //    {
        //        foreach (var res in enumerable)
        //        {
        //            if (results.Contains(res)) continue;
        //            results.Add(res);
        //            if (results.Count == offset + limit) break;
        //        }
        //        return results.Skip(offset);
        //    };
        //}
        public Func<IEnumerable<SparqlResult>, IEnumerable<SparqlResult>> LimitOffset;

    }
}
