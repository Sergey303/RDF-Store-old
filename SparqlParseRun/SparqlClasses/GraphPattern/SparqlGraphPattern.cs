using System.Collections.Generic;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Expressions;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Path;

namespace SparqlParseRun.SparqlClasses.GraphPattern
{
    public class SparqlGraphPattern : SparqlQuadsPattern
    {

        internal void CreateTriple( ObjectVariants subj, ObjectVariants predicate, ObjectVariants obj, RdfQuery11Translator q)
        {
            var pathTranslator = predicate as SparqlPathTranslator;
            if(pathTranslator != null)
                AddRange(pathTranslator.CreateTriple(subj, obj, q));
            else
            Add(new SparqlTriple(subj, predicate, obj, q));
        }

    
        //public new void Add(ISparqlGraphPattern node)
        //{
        //    var filter = node as SparqlFilter;
        //    SparqlTriple sparqlTriple;
        //    if (filter == null || Count == 0 || (sparqlTriple = this[Count - 1] as SparqlTriple) == null  || filter.SparqlExpression.IsStoreUsed)
        //        base.Add(node);
        //    else
        //        sparqlTriple.AddFilter(filter);
        //}
    }
}

