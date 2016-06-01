using System;
using System.Collections.Generic;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.GraphPattern.Triples.Path
{
    public class SparqlPathTranslator :ObjectVariants 
    {
        internal readonly ObjectVariants predicate;

        public SparqlPathTranslator(ObjectVariants predicate)
        {
            // TODO: Complete member initialization
            this.predicate = predicate;
        }

    

        internal bool IsInverse { get; set; }

        public SparqlPathTranslator Inverse()
        {
            IsInverse = !IsInverse;
            return this;
        }

        public virtual IEnumerable<ISparqlGraphPattern> CreateTriple(ObjectVariants subject, ObjectVariants @object, RdfQuery11Translator q)
       {
           var subjectNode = IsInverse ? @object : subject;
           var objectNode = IsInverse ? subject : @object;
           yield return new SparqlTriple((ObjectVariants) subjectNode, predicate, (ObjectVariants) objectNode, q);
       }       
   
        internal virtual SparqlPathTranslator AddAlt(SparqlPathTranslator sparqlPathTranslator)
        {
            return new SparqlPathAlternative(this,sparqlPathTranslator);
        }

        internal virtual SparqlPathTranslator AddSeq(SparqlPathTranslator sparqlPathTranslator)
        {
            return new SparqlPathSequence(this, sparqlPathTranslator);
        }


        public override ObjectVariantEnum Variant
        {
            get { throw new NotImplementedException(); }
        }

        public override object WritableValue
        {
            get { throw new NotImplementedException(); }
        }

        public override object Content
        {
            get { throw new NotImplementedException(); }
        }

        public override ObjectVariants Change(Func<dynamic, dynamic> changing)
        {
            throw new NotImplementedException();
        }
    }
}