using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlQuery.SparqlClasses.GraphPattern.Triples.Node;
using SparqlQuery.SparqlClasses.Query.Result;

namespace SparqlQuery.SparqlClasses.GraphPattern.Triples.Path
{
    public class SparqlMayBeOneTriple:  ISparqlGraphPattern
    {
        private readonly IEnumerable<ISparqlGraphPattern> triples;
        private readonly ObjectVariants sNode;
        private readonly ObjectVariants oNode;               
        private readonly RdfQuery11Translator q;

        public SparqlMayBeOneTriple(IEnumerable<ISparqlGraphPattern> triples, ObjectVariants s, ObjectVariants o, RdfQuery11Translator q)
        {
            oNode = o;
            this.q = q;
            this.triples = triples;
            this.sNode = s;
        }

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            var firstVar = sNode as VariableNode;
            var secondVar = oNode as VariableNode;

            foreach (var variableBinding in variableBindings)
            {                                         
                ObjectVariants s = firstVar == null ? sNode : variableBinding[firstVar];
                ObjectVariants o = secondVar == null ? oNode : variableBinding[secondVar];
                
                switch (NullablePairExt.Get(s, o))
                {
                    case NP.bothNull:
                        foreach (var subjectNode in q.Store.GetAllSubjects())
                        {
                            yield return variableBinding.Add(subjectNode, firstVar, subjectNode, secondVar);
                            variableBinding[secondVar] = null;
                            foreach (var tr in triples.Aggregate(Enumerable.Repeat(variableBinding, 1),
                     (enumerable, triple) => triple.Run(enumerable)))
                                yield return tr;
                        }                       
                    continue;
                    case NP.leftNull:
                        yield return variableBinding.Add(o, firstVar);
                        variableBinding[firstVar] = null;
                        break;
                    case NP.rigthNull:
                           yield return variableBinding.Add(s, secondVar);
                           variableBinding[secondVar] = null;
                        break;
                    case NP.bothNotNull:
                        if (s.Equals(o)) yield return variableBinding;                   
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                 //todo optimize
                foreach (var tr in triples.Aggregate(Enumerable.Repeat(variableBinding, 1),
                    (enumerable, triple) => triple.Run(enumerable)))
                    yield return tr;
            }

        }

        public SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.SparqlTriple;} }
    }
}