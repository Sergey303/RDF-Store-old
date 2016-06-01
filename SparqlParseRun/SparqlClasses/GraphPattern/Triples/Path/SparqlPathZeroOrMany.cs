﻿using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.GraphPattern.Triples.Path
{
    public class SparqlPathZeroOrMany : SparqlPathTranslator
    {
        private readonly SparqlPathTranslator path;

        public SparqlPathZeroOrMany(SparqlPathTranslator path)
            : base(path.predicate)
        {
            this.path = path;
        }


                                          
        public override IEnumerable<ISparqlGraphPattern> CreateTriple(ObjectVariants subject, ObjectVariants @object, RdfQuery11Translator q)
        {
            var subjectNode = IsInverse ? @object : subject;
            var objectNode = IsInverse ? subject : @object;

            var sparqlPathManyTriple = new SparqlPathManyTriple(subjectNode, path, objectNode, q);
           
           yield return new SparqlMayBeOneTriple(Enumerable.Repeat(sparqlPathManyTriple, 1), subjectNode, objectNode,q);
        }
    }
}