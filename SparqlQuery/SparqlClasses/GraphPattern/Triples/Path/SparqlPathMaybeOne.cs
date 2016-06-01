using System.Collections.Generic;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.GraphPattern.Triples.Path
{
    public class SparqlPathMaybeOne : SparqlPathTranslator
    {
        private readonly SparqlPathTranslator path;

        public SparqlPathMaybeOne(SparqlPathTranslator path) : base(path.predicate)
        {
            this.path = path;
        }
                                                    
        public override IEnumerable<ISparqlGraphPattern> CreateTriple(ObjectVariants subject, ObjectVariants @object, RdfQuery11Translator q)
        {
            var subjectNode = IsInverse ? @object : subject;
            var objectNode = IsInverse ? subject : @object;
            //if (subjectNode is ObjectVariants && objectNode is ObjectVariants)

            yield return
                new SparqlMayBeOneTriple(path.CreateTriple((ObjectVariants) subjectNode, objectNode, q),
                    (ObjectVariants) subjectNode, (ObjectVariants) objectNode, q);
            //else                                                                                             
            //foreach (var t in path.CreateTriple((ObjectVariants) subjectNode, objectNode, q))
            //    yield return t;
        }
    }
}