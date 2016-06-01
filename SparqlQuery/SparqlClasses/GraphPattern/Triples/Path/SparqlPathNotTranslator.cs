using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlQuery.SparqlClasses.Expressions;

namespace SparqlQuery.SparqlClasses.GraphPattern.Triples.Path
{
    public class SparqlPathNotTranslator : SparqlPathTranslator
    {
        public readonly List<SparqlPathTranslator> alt=new List<SparqlPathTranslator>();

        public SparqlPathNotTranslator(SparqlPathTranslator path)
            : base(path.predicate)
        {
            // TODO: Complete member initialization
            alt.Add(path); 
        }                                               
        public override IEnumerable<ISparqlGraphPattern> CreateTriple(ObjectVariants subject, ObjectVariants @object, RdfQuery11Translator q)
        {
                var subjectNode = IsInverse ? @object : subject;
           var objectNode = IsInverse ? subject : @object;
            var directed = alt.Where(translator => !translator.IsInverse).ToList();
            var anyDirected = directed.Any();
            if (anyDirected)
            {
                var variableNode = q.CreateBlankNode();
                yield return new SparqlTriple((ObjectVariants) subjectNode, variableNode, (ObjectVariants) objectNode, q);
                foreach (var pathTranslator in directed)
                    yield return
                        new SparqlFilter(new SparqlNotEqualsExpression(new SparqlVarExpression(variableNode),
                            new SparqlIriExpression(pathTranslator.predicate), q.Store.NodeGenerator));
            }

            //TODO drop subject object variables
            var inversed = alt.Where(translator => translator.IsInverse).ToList();
            var anyInversed = inversed.Any();
            if (anyInversed)
            {
                var variableNode =q.CreateBlankNode();
                yield return new SparqlTriple((ObjectVariants) objectNode, variableNode, (ObjectVariants) subjectNode, q);
                foreach (var pathTranslator in inversed)
                    yield return
                        new SparqlFilter(new SparqlNotEqualsExpression(new SparqlVarExpression(variableNode),
                            new SparqlIriExpression(pathTranslator.predicate), q.Store.NodeGenerator));
            }
        }
    }
}
