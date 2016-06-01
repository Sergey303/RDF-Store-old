using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.GraphPattern.Triples.Path
{
    public class SparqlPathAlternative : SparqlPathTranslator
    {
        public readonly List<SparqlPathTranslator> alt = new List<SparqlPathTranslator>();

        public SparqlPathAlternative(SparqlPathTranslator p1, SparqlPathTranslator p2) : base(null)
        {
            alt = new List<SparqlPathTranslator>() {p1, p2};
        }                                                


        public override IEnumerable<ISparqlGraphPattern> CreateTriple(ObjectVariants subject, ObjectVariants @object, RdfQuery11Translator q)
        {
            var subjectNode = IsInverse ? @object : subject;
            var objectNode = IsInverse ? subject : @object;
            return alt.SelectMany(path => path.CreateTriple(subjectNode, objectNode, q));
        }

        internal override SparqlPathTranslator AddAlt(SparqlPathTranslator sparqlPathTranslator)
        {
            alt.Add(sparqlPathTranslator);
            return this;
        }
    }
}