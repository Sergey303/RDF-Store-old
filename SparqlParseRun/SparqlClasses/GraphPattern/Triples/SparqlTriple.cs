using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Expressions;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.GraphPattern.Triples
{
    public class SparqlTriple : ISparqlGraphPattern
    {
        public ObjectVariants Subject { get; private set; }
        public ObjectVariants Predicate { get; private set; }
        public ObjectVariants Object { get; private set; }
        private DataSet graphs;
        private bool isGKnown;

        private readonly VariableNode sVariableNode;
        private readonly VariableNode pVariableNode;
        private readonly VariableNode oVariableNode;
        private readonly VariableDataSet variableDataSet;
        private readonly RdfQuery11Translator q;
        private readonly bool isDefaultGraph;
        private readonly List<SparqlFilter> listOfFilters=new List<SparqlFilter>();
        private bool isFirstCall=true;
        private bool isAny=true;


        public SparqlTriple(ObjectVariants subj, ObjectVariants pred, ObjectVariants obj, RdfQuery11Translator q)
        {
            this.q = q;
            Subject = subj;
            Predicate = pred;
            Object = obj;
            //if(!(subj is ObjectVariants)) throw new ArgumentException();
            graphs = q.ActiveGraphs;
            //this.Graph = graph;
            sVariableNode = subj as VariableNode;
            pVariableNode = pred as VariableNode;
            oVariableNode = obj as VariableNode;
            variableDataSet = (q.ActiveGraphs as VariableDataSet);
            isDefaultGraph = variableDataSet == null && graphs.Count == 0;

        }


        public virtual IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        // var backup = result.BackupMask();
        {
            PrepareConstants();
            if (!isAny) return Enumerable.Empty<SparqlResult>();
            return variableBindings.SelectMany(CreateBindings);
            //  result.Restore(backup);
        }

        public SparqlGraphPatternType PatternType { get { return SparqlGraphPatternType.SparqlTriple; } }

        private IEnumerable<SparqlResult> CreateBindings(SparqlResult variableBinding)
        {
            Subject = sVariableNode != null ? variableBinding[sVariableNode] : Subject;
            Predicate = pVariableNode != null ? variableBinding[pVariableNode] : Predicate;
            Object = oVariableNode != null ? variableBinding[oVariableNode] : Object;


            if (!PrepareKnownVarValues()) return Enumerable.Empty<SparqlResult>();
            if (!isDefaultGraph && variableDataSet != null)
            {
                var graphFromVar = variableBinding[variableDataSet.Variable];
                graphs = graphFromVar != null ? new DataSet() { graphFromVar } : null;
                isGKnown = graphs != null;
            }
            else {isGKnown = true;}

            int @case = ((Subject != null ? 0 : 1) << 2) | ((Predicate != null ? 0 : 1) << 1) | (Object != null ? 0 : 1);
            if (!isDefaultGraph)
                @case |= 1 << (isGKnown ? 3 : 4);
            return ClearNewValues(Subject == null, Predicate==null, Object==null, !isGKnown, variableBinding,
                listOfFilters.Count>0 ? SetVariablesValuesWithFilters(variableBinding, (StoreCallCase)@case) :  SetVariablesValues(variableBinding, (StoreCallCase)@case));

        }

        void PrepareConstants()
        {
            if (isFirstCall)
            {
                isFirstCall = false;
                ObjectVariants temp = null;

                if (sVariableNode == null)
                {
                    var sAsStr = Subject as OV_iri;
                    if (sAsStr != null)
                    {
                        if (!q.Store.NodeGenerator.TryGetUri(sAsStr, out temp)) isAny = false;
                        Subject = temp;
                    }
                }

                if (pVariableNode == null)
                {
                    var predicateAsString = Predicate as OV_iri;
                    if(predicateAsString!= null)
                    {
                        
                        if (!q.Store.NodeGenerator.TryGetUri(predicateAsString, out temp)) isAny = false;

                        Predicate = temp;
                    }
                }
                if (oVariableNode == null)
                {
                    if (Object.Variant == ObjectVariantEnum.Iri)
                    {
                        if (!q.Store.NodeGenerator.TryGetUri((OV_iri) Object, out temp)) isAny = false;
                        Object = temp;
                    }
                }
                if (!isDefaultGraph && graphs.Any())
                {
//todo filter not existing graphs                                                    
                }
            }
        }
        bool PrepareKnownVarValues()
        { 
                ObjectVariants temp = null;
            if (sVariableNode != null && Subject!=null && Subject.Variant==ObjectVariantEnum.Iri)
                {
                    if (!q.Store.NodeGenerator.TryGetUri((OV_iri) Subject, out temp))
                    {   
                        return false;
                    }
                    Subject = temp;
                }

                if (pVariableNode != null && Predicate !=null && Predicate.Variant==ObjectVariantEnum.Iri)
                {
                    if (!q.Store.NodeGenerator.TryGetUri((OV_iri) Predicate, out temp))
                    {
                    return false;
                    }

                    Predicate = temp;
                }
                if (oVariableNode == null && Object != null && Object.Variant == ObjectVariantEnum.Iri)
                {
                    if (!q.Store.NodeGenerator.TryGetUri((OV_iri) Object, out temp))
                    {
                        return false;
                    }
                    Object = temp;
                }
            return true;
            }
        

        private IEnumerable<SparqlResult> ClearNewValues(bool clearSubject, bool clearPredicate, bool clearObject, bool clearGraph, SparqlResult sourceResult, IEnumerable<SparqlResult> sparqlResults)
        {
            foreach (var result in sparqlResults)
                yield return result;
            if (clearSubject)
                sourceResult[sVariableNode] = null;
            if (clearPredicate)
                sourceResult[pVariableNode] = null;
            if (clearObject)
                sourceResult[oVariableNode] = null;
            if (clearGraph)
                sourceResult[variableDataSet.Variable] = null;

        }

        private enum StoreCallCase
        {
            spo = 0, spO = 1, sPo = 2, sPO = 3, Spo = 4, SpO = 5, SPo = 6, SPO = 7,
            gspo = 8, gspO = 9, gsPo = 10, gsPO = 11, gSpo = 12, gSpO = 13, gSPo = 14, gSPO = 15,
            Gspo = 16, GspO = 17, GsPo = 18, GsPO = 19, GSpo = 20, GSpO = 21, GSPo = 22, GSPO = 23,
        }


        private IEnumerable<SparqlResult> SetVariablesValues(SparqlResult variableBinding, StoreCallCase @case)
        {
            switch (@case)
            {
                case StoreCallCase.spo:
                    return spo(Subject, Predicate, Object, variableBinding);
                case StoreCallCase.spO:
                    return q.Store.GetTriplesWithSubjectPredicate(Subject, Predicate).Select(o=>variableBinding.Add(o, oVariableNode));
                case StoreCallCase.sPo:
                    return q.Store.GetTriplesWithSubjectObject(Subject, Object).Select(p => variableBinding.Add(p, pVariableNode));
                    
                case StoreCallCase.sPO:
                    return q.Store.GetTriplesWithSubject(Subject).Select(t => SetValues(variableBinding, t));
                    
                case StoreCallCase.Spo:
                    return q.Store.GetSubjects(Predicate, Object).Select(s => variableBinding.Add(s, sVariableNode));
                    
                case StoreCallCase.SpO:
                    return q.Store.GetTriplesWithPredicate(Predicate).Select(t =>     SetValues(variableBinding, t));
                    
                case StoreCallCase.SPo:
                    return q.Store.GetTriplesWithObject(Object).Select(t => SetValues(variableBinding, t));
                    
                case StoreCallCase.SPO:
                    return q.Store.GetTriples((s, p, o) => variableBinding.Add(s, sVariableNode, p, pVariableNode, o, oVariableNode));
                    

                case StoreCallCase.gspo:
                    return spoGraphs(variableBinding); 
                case StoreCallCase.gspO:
                    return graphs.SelectMany(graph =>
                        q.Store.NamedGraphs
                            .GetObject(Subject, Predicate, graph)
                            
                            .Select(o => variableBinding.Add(o, oVariableNode)));
                case StoreCallCase.gsPo:
                    return graphs.SelectMany(graph =>
                        q.Store.NamedGraphs
                            .GetPredicate(Subject, Object, graph)
                            
                            .Select(p => variableBinding.Add(p, pVariableNode)));
                case StoreCallCase.gsPO:
                    return graphs.SelectMany(g => 
                        q.Store
                        .NamedGraphs
                        .GetTriplesWithSubjectFromGraph(Subject, g)
                        
                        .Select(quad => SetValues(variableBinding, quad)));
                case StoreCallCase.gSpo:
                    return graphs.SelectMany(graph =>
                        q.Store.NamedGraphs
                            .GetSubject(Predicate, Object, graph)
                            
                            .Select(s => variableBinding.Add(s, sVariableNode)));
                case StoreCallCase.gSpO:
                    return graphs.SelectMany(g =>
                        q.Store
                            .NamedGraphs
                            .GetTriplesWithPredicateFromGraph(Predicate, g)
                            
                            .Select(quad => SetValues(variableBinding, quad)));
                case StoreCallCase.gSPo:
                    return graphs.SelectMany(g =>
                        q.Store
                            .NamedGraphs
                            .GetTriplesWithObjectFromGraph(Object, g)
                            
                            .Select(quad => SetValues(variableBinding, quad)));
                case StoreCallCase.gSPO:
                    return graphs.SelectMany(g=> q.Store.NamedGraphs.GetTriplesFromGraph(g, (s, p, o) => variableBinding.Add(s, sVariableNode, p, pVariableNode, o, oVariableNode))); 

                case StoreCallCase.Gspo:
                    return (variableDataSet.Any()
                        ? variableDataSet.Where(g => q.Store.NamedGraphs.Contains(Subject, Predicate, Object, g))
                        : q.Store.NamedGraphs.GetGraph(Subject, Predicate, Object))
                        
                        .Select(g =>variableBinding.Add( g, variableDataSet.Variable)); 
                case StoreCallCase.GspO:
                    if(variableDataSet.Any())           
                        return  variableDataSet.SelectMany(g => q.Store
                            .NamedGraphs
                            .GetObject(Subject, Predicate, g)
                        
                            .Select(o =>variableBinding.Add( o, oVariableNode, g, variableDataSet.Variable)));
                    else return q.Store
                        .NamedGraphs
                        .GetTriplesWithSubjectPredicate(Subject, Predicate)
                        
                            .Select(quad => SetValues(variableBinding, quad));

                    // if graphVariable is null, ctor check this.
                case StoreCallCase.GsPo:
                    if (variableDataSet.Any())
                        return variableDataSet.SelectMany(g => 
                            q.Store.NamedGraphs.GetPredicate(Subject, Object, g)
                                                    
                            .Select(p =>variableBinding.Add( p, pVariableNode, g, variableDataSet.Variable)));
                    else   return q.Store
                        .NamedGraphs
                        .GetTriplesWithSubjectObject(Subject, Object)
                                                
                            .Select(quad => SetValues(variableBinding, quad));
                case StoreCallCase.GsPO:
                    if (variableDataSet.Any())
                        return variableDataSet.SelectMany(g => q.Store
                            .NamedGraphs
                            .GetTriplesWithSubjectFromGraph(Subject, g)
                                                    
                            .Select(quad => SetValues(variableBinding, quad)));
                    else
                        return q.Store
                            .NamedGraphs
                            .GetTriplesWithSubject(Subject)
                                                    
                            .Select(quad => SetValues(variableBinding, quad));
                case StoreCallCase.GSpo:
                    if (variableDataSet.Any())
                        return variableDataSet.SelectMany(g => q.Store
                            .NamedGraphs
                            .GetSubject(Predicate, Object, g)
                                    
                            .Select(s =>variableBinding.Add( s, sVariableNode, g, variableDataSet.Variable)));
                    else 
                        return q.Store
                            .NamedGraphs
                            .GetTriplesWithPredicateObject(Predicate, Object)
                            
                            .Select(quad => SetValues(variableBinding, quad));
                case StoreCallCase.GSpO:
                    ObjectVariants predicate = Predicate;
                    if (variableDataSet.Any())
                        return
                            variableDataSet.SelectMany(
                                g => q.Store.NamedGraphs.GetTriplesWithPredicateFromGraph(predicate, g)
                                    
                                    .Select(quad => SetValues(variableBinding, quad)));
                    else
                        return q.Store.NamedGraphs.GetTriplesWithPredicate(predicate)
                                                    
                            .Select(quad => SetValues(variableBinding, quad));
                case StoreCallCase.GSPo:
                    if (variableDataSet.Any())
                        return variableDataSet.SelectMany(g => q.Store
                            .NamedGraphs
                            .GetTriplesWithObjectFromGraph(Object, g))
                            
                            .Select(quad => SetValues(variableBinding, quad));
                    return q.Store
                        .NamedGraphs
                        .GetTriplesWithObject(Object)
                        
                        .Select(quad=>SetValues(variableBinding, quad));
                case StoreCallCase.GSPO:
                        if (variableDataSet.Any())
                return variableDataSet.SelectMany(g =>
                    q.Store
                        .NamedGraphs
                        .GetTriplesFromGraph(g, (s, p, o) =>
                           variableBinding.Add( s, sVariableNode, p, pVariableNode, o, oVariableNode, g,
                                variableDataSet.Variable)));
                        return q.Store
                .NamedGraphs
                .GetAll((s, p, o, g) =>
                   variableBinding.Add(s, sVariableNode, p, pVariableNode, o, oVariableNode, g,
                        variableDataSet.Variable));
                default:
                    throw new ArgumentOutOfRangeException("case");
            }
        }
        private IEnumerable<SparqlResult> SetVariablesValuesWithFilters(SparqlResult variableBinding, StoreCallCase @case)
        {
            switch (@case)
            {
                case StoreCallCase.spo:
                    return spo(Subject, Predicate, Object, variableBinding);
                case StoreCallCase.spO:
                    return ApplyFilters(variableBinding, oVariableNode, q.Store.GetTriplesWithSubjectPredicate(Subject, Predicate));
                case StoreCallCase.sPo:
                    return ApplyFilters(variableBinding, pVariableNode, q.Store.GetTriplesWithSubjectObject(Subject, Object));

                case StoreCallCase.sPO:
                    return ApplyFilters(variableBinding,  q.Store.GetTriplesWithSubject(Subject));

                case StoreCallCase.Spo:
                    return ApplyFilters(variableBinding, sVariableNode, q.Store.GetSubjects(Predicate, Object));

                case StoreCallCase.SpO:
                    return ApplyFilters(variableBinding, q.Store.GetTriplesWithPredicate(Predicate));

                case StoreCallCase.SPo:
                    return ApplyFilters(variableBinding, q.Store.GetTriplesWithObject(Object));

                case StoreCallCase.SPO:
                    return ApplyFilters(variableBinding, q.Store.GetTriples((s, p, o) => new TripleOVStruct(s, p, o)));


                case StoreCallCase.gspo:
                    return spoGraphs(variableBinding);
                case StoreCallCase.gspO:
                    return ApplyFilters(variableBinding, oVariableNode, graphs.SelectMany(graph =>
                        q.Store.NamedGraphs
                            .GetObject(Subject, Predicate, graph)));
                case StoreCallCase.gsPo:
                    return ApplyFilters(variableBinding, pVariableNode, graphs.SelectMany(graph =>
                        q.Store.NamedGraphs
                            .GetPredicate(Subject, Object, graph)));
                case StoreCallCase.gsPO:
                    return ApplyFilters( variableBinding,graphs.SelectMany(g =>
                        q.Store
                        .NamedGraphs
                        .GetTriplesWithSubjectFromGraph(Subject, g)));
                case StoreCallCase.gSpo:
                    return ApplyFilters( variableBinding, sVariableNode, graphs.SelectMany(graph =>
                        q.Store.NamedGraphs
                            .GetSubject(Predicate, Object, graph)));
                case StoreCallCase.gSpO:
                    return ApplyFilters( variableBinding,graphs.SelectMany(g =>
                        q.Store
                            .NamedGraphs
                            .GetTriplesWithPredicateFromGraph(Predicate, g)));
                case StoreCallCase.gSPo:
                    return ApplyFilters( variableBinding,graphs.SelectMany(g =>
                        q.Store
                            .NamedGraphs
                            .GetTriplesWithObjectFromGraph(Object, g)));
                case StoreCallCase.gSPO:
                    return ApplyFilters( variableBinding, graphs.SelectMany(g => q.Store.NamedGraphs.GetTriplesFromGraph(g, (s, p, o) => new TripleOVStruct(s, p, o))));

                case StoreCallCase.Gspo:
                    return ApplyFilters( variableBinding, variableDataSet.Variable,
                        (variableDataSet.Any()
                        ? variableDataSet.Where(g => q.Store.NamedGraphs.Contains(Subject, Predicate, Object, g))
                        : q.Store.NamedGraphs.GetGraph(Subject, Predicate, Object)));
                case StoreCallCase.GspO:
                    if (variableDataSet.Any())
                        return ApplyFilters( variableBinding, oVariableNode, variableDataSet.SelectMany(g => q.Store
                            .NamedGraphs
                            .GetObject(Subject, Predicate, g)));
                    else return ApplyFilters( variableBinding,q.Store
                        .NamedGraphs
                        .GetTriplesWithSubjectPredicate(Subject, Predicate));

                // if graphVariable is null, ctor check this.
                case StoreCallCase.GsPo:
                    if (variableDataSet.Any())
                        return ApplyFilters( variableBinding, pVariableNode, variableDataSet.SelectMany(g =>
                            q.Store.NamedGraphs.GetPredicate(Subject, Object, g)));
                    else return ApplyFilters( variableBinding,q.Store
                      .NamedGraphs
                      .GetTriplesWithSubjectObject(Subject, Object));
                case StoreCallCase.GsPO:
                    if (variableDataSet.Any())
                        return ApplyFilters( variableBinding,variableDataSet.SelectMany(g => q.Store
                            .NamedGraphs
                            .GetTriplesWithSubjectFromGraph(Subject, g)));
                    else
                        return ApplyFilters( variableBinding,q.Store
                            .NamedGraphs
                            .GetTriplesWithSubject(Subject));
                case StoreCallCase.GSpo:
                    if (variableDataSet.Any())
                        return ApplyFilters(variableBinding, sVariableNode, variableDataSet.SelectMany(g => q.Store
                            .NamedGraphs
                            .GetSubject(Predicate, Object, g)));
                    else
                        return ApplyFilters( variableBinding,q.Store
                            .NamedGraphs
                            .GetTriplesWithPredicateObject(Predicate, Object));
                case StoreCallCase.GSpO:
                    if (variableDataSet.Any())
                        return ApplyFilters( variableBinding, 
                            variableDataSet.SelectMany(g => q.Store.NamedGraphs.GetTriplesWithPredicateFromGraph(Predicate, g)));
                    else
                        return ApplyFilters( variableBinding, q.Store.NamedGraphs.GetTriplesWithPredicate(Predicate));
                case StoreCallCase.GSPo:
                    if (variableDataSet.Any())
                        return ApplyFilters( variableBinding,variableDataSet.SelectMany(g => q.Store
                            .NamedGraphs
                            .GetTriplesWithObjectFromGraph(Object, g)));
                    return ApplyFilters( variableBinding,q.Store
                        .NamedGraphs
                        .GetTriplesWithObject(Object));
                case StoreCallCase.GSPO:
                    if (variableDataSet.Any())
                        return ApplyFilters( variableBinding,variableDataSet.SelectMany(g =>
                            q.Store
                                .NamedGraphs
                                .GetTriplesFromGraph(g, (s, p, o) => new QuadOVStruct(s, p, o, g))));
                    return ApplyFilters( variableBinding, 
                        q.Store
                        .NamedGraphs
                        .GetAll((s, p, o, g) => 
                          new QuadOVStruct(s, p, o, g)));
                default:
                    throw new ArgumentOutOfRangeException("case");
            }
        }

        public void Substitution(SparqlResult variableBinding, Action<ObjectVariants, ObjectVariants, ObjectVariants> actTriple)
        {
            var subject = sVariableNode is IBlankNode
                 ? q.Store.NodeGenerator.CreateBlankNode(((IBlankNode)sVariableNode).Name+variableBinding.Id)
                 : (sVariableNode != null ? variableBinding[sVariableNode] : Subject);

            var predicate = pVariableNode != null ? variableBinding[pVariableNode] : Predicate;

            var @object = oVariableNode is IBlankNode
                 ? q.Store.NodeGenerator.CreateBlankNode(((IBlankNode)oVariableNode).Name+variableBinding.Id)
                 : (oVariableNode != null ? variableBinding[oVariableNode] : Object);
            actTriple(subject, predicate, @object);
        }

        public void Substitution(SparqlResult variableBinding, ObjectVariants g, Action<ObjectVariants, ObjectVariants, ObjectVariants, ObjectVariants> actQuard)
        {
            var subject = sVariableNode is IBlankNode
                ? q.Store.NodeGenerator.CreateBlankNode(((IBlankNode)sVariableNode).Name + variableBinding.Id, ((IIriNode)g).UriString)
                : (sVariableNode != null ? variableBinding[sVariableNode] : Subject);
            var predicate = pVariableNode != null ? variableBinding[pVariableNode] : Predicate;
            var @object = Object == null && oVariableNode is IBlankNode
                  ? q.Store.NodeGenerator.CreateBlankNode(((IBlankNode)oVariableNode).Name + variableBinding.Id, ((IIriNode)g).UriString)
                  : (oVariableNode != null ? variableBinding[oVariableNode] : Object);

            actQuard(g, subject, predicate, @object);
        }

        public void Substitution(SparqlResult variableBinding, VariableNode gVariableNode,
            Action<ObjectVariants, ObjectVariants, ObjectVariants, ObjectVariants> actQuard)
        {
            ObjectVariants g;
            g = variableBinding[gVariableNode];
            if (g == null)
                throw new Exception("graph hasn't value");

            Substitution(variableBinding, g, actQuard);
        }

        public void AddFilter(SparqlFilter node)
        {
            listOfFilters.Add(node);
        }
        public IEnumerable<SparqlResult> ApplyFilters(SparqlResult variablesBindings, VariableNode variable,
   IEnumerable<ObjectVariants> baseStoreCall)
        {
            return listOfFilters.Aggregate(baseStoreCall
                .Select(node => new KeyValuePair<ObjectVariants, SparqlResult>(node, variablesBindings.Add(node, variable))),
                (current, sparqlFilter) => current.Where(valueAndResult => sparqlFilter.SparqlExpression.Test(valueAndResult.Value)))
                        .Select(pair => pair.Key)
                        .ToArray()
                        .Select(node => variablesBindings.Add(node, variable));
        }
        public IEnumerable<SparqlResult> ApplyFilters(SparqlResult variablesBindings, IEnumerable<TripleOVStruct> baseStoreCall)
        {
            return listOfFilters.Aggregate(baseStoreCall
                .Select(t => new KeyValuePair<TripleOVStruct, SparqlResult>(t, SetValues(variablesBindings, t))),
                (current, sparqlFilter) =>
                    current.Where(valueAndResult => sparqlFilter.SparqlExpression.Test(valueAndResult.Value)))
                .Select(pair => pair.Key)
                .ToArray()
                .Select(t => SetValues(variablesBindings, t));
        }

        public IEnumerable<SparqlResult> ApplyFilters(SparqlResult variablesBindings, IEnumerable<QuadOVStruct> baseStoreCall)
        {
            return listOfFilters.Aggregate(baseStoreCall
                .Select(t => new KeyValuePair<QuadOVStruct, SparqlResult>(t, SetValues(variablesBindings, t))),
                (current, sparqlFilter) =>
                    current.Where(valueAndResult => sparqlFilter.SparqlExpression.Test(valueAndResult.Value)))
                .Select(pair => pair.Key)
                .ToArray()
                .Select(t => SetValues(variablesBindings, t));
        }

        private SparqlResult SetValues(SparqlResult result, TripleOVStruct triple)
        {
            if (Subject == null)
            {
                result.Add(triple.Subject, sVariableNode);
            }
            if (Predicate == null)
            {
                result.Add(triple.Predicate, pVariableNode);
            }
            if (Object == null)
            {
                result.Add(triple.Object, oVariableNode);
            }
            return result;
        }
        private SparqlResult SetValues(SparqlResult result, QuadOVStruct quad)
        {
            if (Subject == null)
            {
                result.Add(quad.Subject, sVariableNode);
            }
            if (Predicate == null)
            {
                result.Add(quad.Predicate, pVariableNode);
            }
            if (Object == null)
            {
                result.Add(quad.Object, oVariableNode);
            }
            if (!isGKnown)
            {
                result.Add(quad.Graph, variableDataSet.Variable);
            }
            return result;
        }

     

        private IEnumerable<SparqlResult> spoGraphs(SparqlResult variablesBindings)
        {
            if (graphs.Any(g => q.Store.NamedGraphs.Contains(Subject, Predicate, Object, g)))
                yield return variablesBindings;
        }

        private IEnumerable<SparqlResult> spo(ObjectVariants subjectNode, ObjectVariants predicateNode, ObjectVariants objNode, SparqlResult variablesBindings)
        {
            if (q.Store.Contains(subjectNode, predicateNode, objNode))
                yield return variablesBindings;
        }
    }
}