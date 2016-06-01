using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.GraphPattern.Triples
{                                               
    public class SparqlTripletsStoreCallsCache : ISparqlTripletsStoreCalls
    {
        private readonly IStore store;
        private readonly Cache<ObjectVariants, ObjectVariants, IEnumerable<ObjectVariants>> spOCache;
        private readonly Cache<ObjectVariants, ObjectVariants, IEnumerable<ObjectVariants>> sPoCache;
        private readonly Cache<ObjectVariants, ObjectVariants, IEnumerable<ObjectVariants>> SpoCache;
        private readonly Cache<ObjectVariants, IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>>> sPOCache;
        private readonly Cache<ObjectVariants, IEnumerable<Triple<ObjectVariants,ObjectVariants,ObjectVariants>>> SpOCache;
        private readonly Cache<ObjectVariants, IEnumerable<Triple<ObjectVariants,ObjectVariants,ObjectVariants>>> SPoCache;
        private readonly Cache<ObjectVariants, ObjectVariants, ObjectVariants, bool> spoCache;
       // private bool hasSPOCache;

        private readonly Cache<ObjectVariants, ObjectVariants, ObjectVariants, IGrouping<ObjectVariants, ObjectVariants>> spOgCache;
        private readonly Cache<ObjectVariants, ObjectVariants, ObjectVariants, IGrouping<ObjectVariants, ObjectVariants>> sPogCache;
        private readonly Cache<ObjectVariants, ObjectVariants, ObjectVariants, IGrouping<ObjectVariants, ObjectVariants>> SpogCache;
        private readonly Cache<ObjectVariants, ObjectVariants, IGrouping<ObjectVariants, Triple<ObjectVariants, ObjectVariants, ObjectVariants>>> sPOgCache;
        private readonly Cache<ObjectVariants, ObjectVariants, IGrouping<ObjectVariants, Triple<ObjectVariants, ObjectVariants, ObjectVariants>>> SpOgCache;
        private readonly Cache<ObjectVariants, ObjectVariants, IGrouping<ObjectVariants, Triple<ObjectVariants, ObjectVariants, ObjectVariants>>> SPogCache;
        private readonly Cache<ObjectVariants, ObjectVariants, ObjectVariants, ObjectVariants, bool> spogCache;
      //  private Cache<IUriNode,bool> SPOgCache;

        private readonly Cache<ObjectVariants, ObjectVariants, List<IGrouping<ObjectVariants, ObjectVariants>>> spOGCache;
        private readonly Cache<ObjectVariants, ObjectVariants, List<IGrouping<ObjectVariants, ObjectVariants>>> sPoGCache;
        private readonly Cache<ObjectVariants, ObjectVariants,  List<IGrouping<ObjectVariants, ObjectVariants>>> SpoGCache;
        private readonly Cache<ObjectVariants, List<IGrouping<ObjectVariants, Triple<ObjectVariants,ObjectVariants,ObjectVariants>>>> sPOGCache;
        private readonly Cache<ObjectVariants, List<IGrouping<ObjectVariants, Triple<ObjectVariants,ObjectVariants,ObjectVariants>>>> SpOGCache;
        private readonly Cache<ObjectVariants, List<IGrouping<ObjectVariants, Triple<ObjectVariants,ObjectVariants,ObjectVariants>>>> SPoGCache;
        private readonly Cache<ObjectVariants, ObjectVariants, ObjectVariants, DataSet> spoGCache;
        //private bool SPOGCache;
          public SparqlTripletsStoreCallsCache(IStore store)
        {
            this.store = store;
              spOCache = new Cache<ObjectVariants, ObjectVariants, IEnumerable<ObjectVariants>>(
                  (s, p) =>
                  {
                      var nodes = store.GetTriplesWithSubjectPredicate(s, p).ToArray();
                      foreach (var o in nodes)
                          spoCache.Add(s, p, o, true);
                      return nodes;
                  });
            sPoCache = new Cache<ObjectVariants, ObjectVariants, IEnumerable<ObjectVariants>>(
                (s, o) =>
                {
                    var predicates = store.GetTriplesWithSubjectObject(s, o).ToArray();
                    foreach (var p in predicates)
                        spoCache.Add(s, p, o, true);
                    return predicates;
                });
            SpoCache = new Cache<ObjectVariants, ObjectVariants, IEnumerable<ObjectVariants>>(
                (p, o) =>
                {
                    var subjectNodes = store.GetTriplesWithPredicateObject(p, o).ToArray();
                    foreach (var s in subjectNodes)
                        spoCache.Add(s, p, o, true);
                    return subjectNodes;
                });

            sPOCache = new Cache<ObjectVariants, IEnumerable<Triple<ObjectVariants,ObjectVariants,ObjectVariants>>>(s => { 
                                                                             var triplesWithSubject = store.GetTriplesWithSubject(s,
                                                                             foreach (var pg in triplesWithSubject.GroupBy(t => t.Predicate))
                                                                                 spOCache.Add(s, pg.Key,
                                                                                     pg.Select(t => t.Object));
                                                                             foreach (var pg in triplesWithSubject.GroupBy(t => t.Object))
                                                                                 sPoCache.Add(s, pg.Key,
                                                                                     pg.Select(t => t.Predicate));
                                                                             foreach (var triple in triplesWithSubject)
                                                                                 spoCache.Add(triple.Subject, triple.Predicate, triple.Object, true);
                                                                             return triplesWithSubject; });
            SpOCache = new Cache<ObjectVariants, IEnumerable<Triple<ObjectVariants,ObjectVariants,ObjectVariants>>>(p => {
                var triples = store.GetTriplesWithPredicate(p).ToArray();
                foreach (var pg in triples.GroupBy(t => t.Object))
                    SpoCache.Add(p, pg.Key,
                        pg.Select(t => t.Subject));
                foreach (var pg in triples.GroupBy(t => t.Subject))
                    spOCache.Add(pg.Key, p,
                        pg.Select(t => t.Object));
                foreach (var triple in triples)
                    spoCache.Add(triple.Subject, triple.Predicate, triple.Object, true); 
                return triples;
            });
            SPoCache = new Cache<ObjectVariants, IEnumerable<Triple<ObjectVariants,ObjectVariants,ObjectVariants>>>(o => {
                var triples = store.GetTriplesWithObject(o).ToArray();
                foreach (var pg in triples.GroupBy(t => t.Predicate))
                    SpoCache.Add(pg.Key, o, 
                        pg.Select(t => t.Subject));
                foreach (var pg in triples.GroupBy(t => t.Subject))
                    sPoCache.Add(pg.Key, o,
                        pg.Select(t => t.Predicate));
                foreach (var triple in triples)
                    spoCache.Add(triple.Subject, triple.Predicate, triple.Object, true);    
                return triples;
            });
            spoCache = new Cache<ObjectVariants, ObjectVariants, ObjectVariants, bool>(store.Contains);
           // SPOCache = new Cache<ObjectVariants, IUriNode, IEnumerable<INode>>(store.GetTriplesWithSubjectPredicate);
            spOgCache = new Cache<ObjectVariants, ObjectVariants, ObjectVariants, IGrouping<ObjectVariants, ObjectVariants>>(null);
            sPogCache = new Cache<ObjectVariants, ObjectVariants, ObjectVariants, IGrouping<ObjectVariants, ObjectVariants>>(null);
            SpogCache = new Cache<ObjectVariants, ObjectVariants, ObjectVariants, IGrouping<ObjectVariants, ObjectVariants>>(null);

            sPOgCache = new Cache<ObjectVariants, ObjectVariants, IGrouping<ObjectVariants, Triple<ObjectVariants, ObjectVariants, ObjectVariants>>>(null);

            SpOgCache = new Cache<ObjectVariants, ObjectVariants, IGrouping<ObjectVariants, Triple<ObjectVariants, ObjectVariants, ObjectVariants>>>(null);
            SPogCache = new Cache<ObjectVariants, ObjectVariants, IGrouping<ObjectVariants, Triple<ObjectVariants, ObjectVariants, ObjectVariants>>>(null);

            spogCache = new Cache<ObjectVariants, ObjectVariants, ObjectVariants, ObjectVariants, bool>(null);

            spOGCache = new Cache<ObjectVariants, ObjectVariants, List<IGrouping<ObjectVariants, ObjectVariants>>>((node,  arg3) => new List<IGrouping<ObjectVariants, ObjectVariants>>());
            sPoGCache = new Cache<ObjectVariants, ObjectVariants, List<IGrouping<ObjectVariants, ObjectVariants>>>((node,  arg3) => new List<IGrouping<ObjectVariants, ObjectVariants>>());
            SpoGCache = new Cache<ObjectVariants, ObjectVariants, List<IGrouping<ObjectVariants, ObjectVariants>>>((node,  arg3) => new List<IGrouping<ObjectVariants, ObjectVariants>>());

            sPOGCache = new Cache<ObjectVariants, List<IGrouping<ObjectVariants, Triple<ObjectVariants,ObjectVariants,ObjectVariants>>>>((node) => new List<IGrouping<ObjectVariants, Triple<ObjectVariants,ObjectVariants,ObjectVariants>>>());

            SpOGCache = new Cache<ObjectVariants, List<IGrouping<ObjectVariants, Triple<ObjectVariants,ObjectVariants,ObjectVariants>>>>((node) => new List<IGrouping<ObjectVariants, Triple<ObjectVariants,ObjectVariants,ObjectVariants>>>());
            SPoGCache = new Cache<ObjectVariants, List<IGrouping<ObjectVariants, Triple<ObjectVariants,ObjectVariants,ObjectVariants>>>>((node) => new List<IGrouping<ObjectVariants, Triple<ObjectVariants,ObjectVariants,ObjectVariants>>>());

            spoGCache = new Cache<ObjectVariants, ObjectVariants, ObjectVariants, DataSet>((node, p, arg3) => new DataSet());  
          // SPOgCache=new Cache<IUriNode, bool>(set => false);
        }
        public IEnumerable<SparqlResult> spO(ObjectVariants subjNode, ObjectVariants predicateNode,
            VariableNode obj, SparqlResult variablesBindings)
        {   
            return spOCache.Get(subjNode,predicateNode)
                    .Select(node => new SparqlResult(variablesBindings, node, obj));
        }


       


        // from merged named graphs
             public  IEnumerable<SparqlResult> spOVarGraphs( ObjectVariants subjNode, ObjectVariants predicateNode,
            VariableNode obj, SparqlResult variablesBindings, VariableDataSet variableDataSet)
             {
                 return spOCacheGraphs(subjNode, predicateNode, variableDataSet)
                //.GetGraphUri(variablesBindings) if graphs is empty, Gets All named graphs
                .SelectMany(grouping =>            
                        grouping.Select(node =>
                            new SparqlResult(variablesBindings, node, obj, grouping.Key, variableDataSet.Variable))); // if graphVariable is null, ctor check this.
            
            }

             private IEnumerable<IGrouping<ObjectVariants, ObjectVariants>> spOCacheGraphs(ObjectVariants subjNode, ObjectVariants predicateNode, DataSet variableDataSet)
        {
            if (variableDataSet.Any() )     //search in all named graphs
                if (spOGCache.Contains(subjNode, predicateNode))
                    return spOGCache.Get(subjNode, predicateNode);
                else
                {
                    var result = store.NamedGraphs.GetTriplesWithSubjectPredicateFromGraphs(subjNode, predicateNode,
                        variableDataSet).ToList();
                    spOGCache.Add(subjNode, predicateNode, result);
                    foreach (var grouping in result)
                    {
                        spOgCache.Add(subjNode, predicateNode, grouping.Key, grouping);
                        foreach (var o in grouping)
                            spoGCache.Get(subjNode, predicateNode, o).Add(grouping.Key);
                    }
                    return result;
                }
            var res = new List<IGrouping<ObjectVariants, ObjectVariants>>();
            var gList = new DataSet();
            foreach (var g in variableDataSet)
                if (spOgCache.Contains(subjNode, predicateNode, g))
                    res.Add(spOgCache.Get(subjNode, predicateNode, g));
                else
                    gList.Add(g);
            foreach (var gRes in store.NamedGraphs.GetTriplesWithSubjectPredicateFromGraphs(subjNode, predicateNode, gList))
            {
                spOgCache.Add(subjNode, predicateNode, gRes.Key,gRes);
                foreach (var o in gRes)
                    spogCache.Add(subjNode, predicateNode, o, gRes.Key,true);
                res.Add(gRes);
            }
            return res;
        }


        public  IEnumerable<SparqlResult> spOGraphs( ObjectVariants subjNode, ObjectVariants predicateNode,
            VariableNode obj, SparqlResult variablesBindings, DataSet graphs)
        {
            return spOCacheGraphs(subjNode, predicateNode,graphs)
                .SelectMany(grouping =>
                        grouping.Select(node => new SparqlResult(variablesBindings, node, obj)));     
        }


        public  IEnumerable<SparqlResult> Spo( VariableNode subj, ObjectVariants predicateNode, ObjectVariants objectNode, SparqlResult variablesBindings)
        {
            return SpoCache.Get(predicateNode, objectNode)
                .Select(node => new SparqlResult(variablesBindings, node, subj));

            // from merged named graphs
        }



        public  IEnumerable<SparqlResult> SpoGraphs( VariableNode subj, ObjectVariants predicateNode,
            ObjectVariants objectNode, SparqlResult variablesBindings, DataSet graphs)
        {
            return SpoCacheGraphs(predicateNode, objectNode, graphs)
                // if graphs is empty, Gets All named graphs
                .SelectMany(grouping =>
                        grouping.Select(node =>
                            new SparqlResult(variablesBindings, node, subj)));
                // if graphVariable is null, ctor check this.
        }
        public  IEnumerable<SparqlResult> SpoVarGraphs( VariableNode subj, ObjectVariants predicateNode,
            ObjectVariants objectNode, SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            return SpoCacheGraphs(predicateNode, objectNode, variableDataSet) //.GetGraphUri(variablesBindings) if graphs is empty, Gets All named graphs
                .SelectMany(grouping =>
                   grouping.Select(node =>
                       new SparqlResult(variablesBindings, node, subj, grouping.Key, variableDataSet.Variable))); // if graphVariable is null, ctor check this.

        }
   
        private IEnumerable<IGrouping<ObjectVariants, ObjectVariants>> SpoCacheGraphs(ObjectVariants predicateNode, ObjectVariants objectNode, DataSet variableDataSet)
        {
            if (variableDataSet.Any())     //search in all named graphs
                if (SpoGCache.Contains(predicateNode, objectNode))
                    return SpoGCache.Get(predicateNode, objectNode);
                else
                {
                    var result = store.NamedGraphs.GetTriplesWithPredicateObjectFromGraphs(predicateNode, objectNode,
                        variableDataSet).ToList();
                    SpoGCache.Add(predicateNode, objectNode, result);
                    foreach (var grouping in result)
                    {
                        SpogCache.Add(predicateNode, objectNode, grouping.Key, grouping);
                        foreach (var s in grouping)
                            spoGCache.Get(s, predicateNode, objectNode).Add(grouping.Key);
                    }
                    return result;
                }
            var res = new List<IGrouping<ObjectVariants, ObjectVariants>>();
            var gList = new DataSet();
            foreach (var g in variableDataSet)
                if (SpogCache.Contains(predicateNode, objectNode, g))
                    res.Add(SpogCache.Get(predicateNode, objectNode, g));
                else
                    gList.Add(g);
            foreach (var gRes in store.NamedGraphs.GetTriplesWithPredicateObjectFromGraphs(predicateNode, objectNode, gList))
            {
                SpogCache.Add(predicateNode, objectNode, gRes.Key, gRes);
                foreach (var s in gRes)
                    spoGCache.Get(s, predicateNode, objectNode).Add(gRes.Key);
                res.Add(gRes);
            }
            return res;
        }

        public  IEnumerable<SparqlResult> sPo( ObjectVariants subj, VariableNode pred, ObjectVariants obj, SparqlResult variablesBindings)
        {
            return sPoCache.Get(subj, obj)
                .Select(newObj => new SparqlResult(variablesBindings, newObj, pred));

        }

        public  IEnumerable<SparqlResult> sPoGraphs( ObjectVariants subj, VariableNode pred, ObjectVariants obj, SparqlResult variablesBindings, DataSet graphs)
        {
            return sPoCacheGraphs(subj, obj, graphs) 
                .SelectMany(grouping =>
                        grouping.Select(node =>
                            new SparqlResult(variablesBindings, node, pred)));
        }
        public  IEnumerable<SparqlResult> sPoVarGraphs( ObjectVariants subj, VariableNode pred, ObjectVariants obj, SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            return sPoCacheGraphs(subj, obj, variableDataSet) //.GetGraphUri(variablesBindings)
               .SelectMany(grouping =>
                       grouping.Select(node =>
                           new SparqlResult(variablesBindings, node, pred, grouping.Key, variableDataSet.Variable)));
        }
        private IEnumerable<IGrouping<ObjectVariants, ObjectVariants>> sPoCacheGraphs(ObjectVariants subjectNode, ObjectVariants objectNode, DataSet variableDataSet)
        {
            if (variableDataSet.Any())     //search in all named graphs
                if (sPoGCache.Contains(subjectNode, objectNode))
                    return sPoGCache.Get(subjectNode, objectNode);
                else
                {
                    var result = store.NamedGraphs.GetTriplesWithSubjectObjectFromGraphs(subjectNode, objectNode,
                        variableDataSet).ToList();
                    sPoGCache.Add(subjectNode, objectNode, result);
                    foreach (var grouping in result)
                    {
                        sPogCache.Add(subjectNode, objectNode, grouping.Key, grouping);
                        foreach (var p in grouping)
                            spoGCache.Get(subjectNode, p, objectNode).Add(grouping.Key);
                    }
                    return result;
                }
            var res = new List<IGrouping<ObjectVariants, ObjectVariants>>();
            var gList = new DataSet();
            foreach (var g in variableDataSet)
                if (sPogCache.Contains(subjectNode, objectNode, g))
                    res.Add(sPogCache.Get(subjectNode, objectNode, g));
                else
                    gList.Add(g);
            foreach (var gRes in store.NamedGraphs.GetTriplesWithSubjectObjectFromGraphs(subjectNode, objectNode, gList))
            {
                sPogCache.Add(subjectNode, objectNode, gRes.Key, gRes);
                foreach (var p in gRes)
                    spoGCache.Get(subjectNode, p, objectNode).Add(gRes.Key);
                res.Add(gRes);
            }
            return res;
        }

        public IEnumerable<SparqlResult> SpO(VariableNode s, ObjectVariants predicate, VariableNode o, SparqlResult variablesBindings)
        {
            return SpOCache.Get(predicate)
                .Select(triple => new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
                {
                    {s, new SparqlVariableBinding(s,triple.Subject)},
                    {o, new SparqlVariableBinding(o,triple.Object)}
                }));
        }

        public IEnumerable<SparqlResult> SpOGraphs(VariableNode s, ObjectVariants predicate, VariableNode o, SparqlResult variablesBindings, DataSet graphs)
        {
            return SpOCacheGraphs(predicate, graphs)
             .SelectMany(grouping =>
                     grouping
                        .Select(triple => new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {s, new SparqlVariableBinding(s,triple.Subject)},
                  {o, new SparqlVariableBinding(o,triple.Object)}
              })));
        }

        public IEnumerable<SparqlResult> SpOVarGraphs(VariableNode s, ObjectVariants predicate, VariableNode o, SparqlResult variablesBindings, VariableDataSet graphs)
        {
            return SpOCacheGraphs(predicate, graphs) //.GetGraphUri(variablesBindings))
             .SelectMany(grouping =>
                     grouping
                        .Select(triple => new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {s, new SparqlVariableBinding(s,triple.Subject)},
                  {o, new SparqlVariableBinding(o,triple.Object)}, 
                  {graphs.Variable, new SparqlVariableBinding(graphs.Variable,grouping.Key)}
              })));
        }
        private IEnumerable<IGrouping<ObjectVariants, Triple<ObjectVariants,ObjectVariants,ObjectVariants>>> SpOCacheGraphs(ObjectVariants predicateNode, DataSet variableDataSet)
        {
            if (variableDataSet.Any())
                if (SpOGCache.Contains(predicateNode))
                    return SpOGCache.Get(predicateNode);
                else
                {
                    var result = store.NamedGraphs.GetTriplesWithPredicateFromGraphs(predicateNode, variableDataSet).ToList();
                    SpOGCache.Add(predicateNode, result);
                    foreach (var grouping in result)
                    {
                        SpOgCache.Add(predicateNode, grouping.Key,  grouping);
                        foreach (var tripleGroup in grouping.GroupBy(triple => triple.Subject))
                        {
                            var oGroup = new Grouping<ObjectVariants, ObjectVariants>(grouping.Key,
                                tripleGroup.Select(triple => triple.Object));
                            spOgCache.Add(tripleGroup.Key, predicateNode, grouping.Key,oGroup);
                         
                            spOGCache.Get(tripleGroup.Key, predicateNode).Add(oGroup);
                            foreach (var o in oGroup)
                            {
                                spoGCache.Get(tripleGroup.Key, predicateNode, o).Add(grouping.Key);
                                spogCache.Add(tripleGroup.Key, predicateNode, o, grouping.Key, true);
                            }
                        }
                        foreach (var tripleGroup in grouping.GroupBy(triple => triple.Object))
                        {
                            var sGroup = new Grouping<ObjectVariants, ObjectVariants>(grouping.Key,
                                tripleGroup.Select(triple => triple.Subject));
                            SpogCache.Add(predicateNode, tripleGroup.Key, grouping.Key, sGroup);
                            SpoGCache.Get(predicateNode, tripleGroup.Key).Add(sGroup); 
                        }
                    }       
                    return result;
                }
            var res = new List<IGrouping<ObjectVariants, Triple<ObjectVariants,ObjectVariants,ObjectVariants>>>();
            var gList = new DataSet();
            foreach (var g in variableDataSet)
                if (SpOgCache.Contains(predicateNode, g))
                    res.Add(SpOgCache.Get(predicateNode, g));
                else
                    gList.Add(g);
            foreach (var gRes in store.NamedGraphs.GetTriplesWithPredicateFromGraphs(predicateNode,  gList))
            {
                SpOgCache.Add(predicateNode,  gRes.Key, gRes);
                res.Add(gRes);
                foreach (var tripleGroup in gRes.GroupBy(triple => triple.Subject))
                {
                    var oGroup = new Grouping<ObjectVariants, ObjectVariants>(gRes.Key,
                        tripleGroup.Select(triple => triple.Object));
                    spOgCache.Add(tripleGroup.Key, predicateNode, gRes.Key, oGroup);

                    spOGCache.Get(tripleGroup.Key, predicateNode).Add(oGroup);
                    foreach (var o in oGroup)
                    {
                        spoGCache.Get(tripleGroup.Key, predicateNode, o).Add(gRes.Key);
                        spogCache.Add(tripleGroup.Key, predicateNode, o, gRes.Key, true);
                    }
                }
                foreach (var tripleGroup in gRes.GroupBy(triple => triple.Object))
                {
                    var sGroup = new Grouping<ObjectVariants, ObjectVariants>(gRes.Key,
                        tripleGroup.Select(triple => triple.Subject));
                    SpogCache.Add(predicateNode, tripleGroup.Key, gRes.Key, sGroup);
                    SpoGCache.Get(predicateNode, tripleGroup.Key).Add(sGroup);
                }  
            }
            return res;
        }

        public  IEnumerable<SparqlResult> sPO( ObjectVariants subj, VariableNode pred, VariableNode obj, SparqlResult variablesBindings)
        {
            return sPOCache.Get(subj)
              .Select(triple => new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {pred, new SparqlVariableBinding(pred,triple.Predicate)},
                  {obj, new SparqlVariableBinding(obj,triple.Object)}
              }));
        }

        public  IEnumerable<SparqlResult> sPOGraphs( ObjectVariants subj, VariableNode pred,
            VariableNode obj, SparqlResult variablesBindings, DataSet graphs)
        {
            return sPOCacheGraphs(subj, graphs)
               .SelectMany(grouping =>
                       grouping
                          .Select(triple => new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {pred, new SparqlVariableBinding(pred,triple.Predicate)},
                  {obj, new SparqlVariableBinding(obj,triple.Object)}
              })));
        }
        public  IEnumerable<SparqlResult> sPOVarGraphs( ObjectVariants subj, VariableNode pred,
           VariableNode obj, SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            return sPOCacheGraphs(subj, variableDataSet)//.GetGraphUri(variablesBindings)
             .SelectMany(grouping =>
                     grouping
                        .Select(triple => new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {pred, new SparqlVariableBinding(pred,triple.Predicate)},
                  {obj, new SparqlVariableBinding(obj,triple.Object)},
                  {variableDataSet.Variable, new SparqlVariableBinding(variableDataSet.Variable,grouping.Key)},
              })));
        }
        private IEnumerable<IGrouping<ObjectVariants, Triple<ObjectVariants,ObjectVariants,ObjectVariants>>> sPOCacheGraphs(ObjectVariants subjectNode, DataSet variableDataSet)
        {
            if (variableDataSet.Any())
                if (sPOGCache.Contains(subjectNode))
                    return sPOGCache.Get(subjectNode);
                else
                {
                    var result = store.NamedGraphs.GetTriplesWithSubjectFromGraphs(subjectNode, variableDataSet).ToList();
                    sPOGCache.Add(subjectNode, result);
                    foreach (var grouping in result)
                    {
                        sPOgCache.Add(subjectNode, grouping.Key, grouping);
                        foreach (var tripleGroup in grouping.GroupBy(triple => triple.Predicate))
                        {
                            var oGroup = new Grouping<ObjectVariants, ObjectVariants>(grouping.Key,
                                tripleGroup.Select(triple => triple.Object));
                            spOgCache.Add(subjectNode, tripleGroup.Key, grouping.Key, oGroup);

                            spOGCache.Get(subjectNode, tripleGroup.Key).Add(oGroup);
                            foreach (var o in oGroup)
                            {
                                spoGCache.Get(subjectNode, tripleGroup.Key, o).Add(grouping.Key);
                                spogCache.Add(subjectNode, tripleGroup.Key, o, grouping.Key, true);
                            }
                        }
                        foreach (var tripleGroup in grouping.GroupBy(triple => triple.Object))
                        {
                            var pGroup = new Grouping<ObjectVariants, ObjectVariants>(grouping.Key,
                                tripleGroup.Select(triple => triple.Predicate));
                            sPogCache.Add(subjectNode, tripleGroup.Key, grouping.Key, pGroup);
                            sPoGCache.Get(subjectNode, tripleGroup.Key).Add(pGroup);
                        }
                    }
                    return result;
                }
            var res = new List<IGrouping<ObjectVariants, Triple<ObjectVariants,ObjectVariants,ObjectVariants>>>();
            var gList = new DataSet();
            foreach (var g in variableDataSet)
                if (sPOgCache.Contains(subjectNode, g))
                    res.Add(sPOgCache.Get(subjectNode, g));
                else
                    gList.Add(g);
            foreach (var gRes in store.NamedGraphs.GetTriplesWithSubjectFromGraphs(subjectNode, gList))
            {
                sPOgCache.Add(subjectNode, gRes.Key, gRes);
                res.Add(gRes);
                foreach (var tripleGroup in gRes.GroupBy(triple => triple.Predicate))
                {
                    var oGroup = new Grouping<ObjectVariants, ObjectVariants>(gRes.Key,
                        tripleGroup.Select(triple => triple.Object));
                    spOgCache.Add(subjectNode, tripleGroup.Key, gRes.Key, oGroup);

                    spOGCache.Get(subjectNode, tripleGroup.Key).Add(oGroup);
                    foreach (var o in oGroup)
                    {
                        spoGCache.Get(subjectNode, tripleGroup.Key, o).Add(gRes.Key);
                        spogCache.Add(subjectNode, tripleGroup.Key, o, gRes.Key, true);
                    }
                }
                foreach (var tripleGroup in gRes.GroupBy(triple => triple.Object))
                {
                    var pGroup = new Grouping<ObjectVariants, ObjectVariants>(gRes.Key,
                        tripleGroup.Select(triple => triple.Predicate));
                    sPogCache.Add(subjectNode, tripleGroup.Key, gRes.Key, pGroup);
                    sPoGCache.Get(subjectNode, tripleGroup.Key).Add(pGroup);
                }
            }
            return res;
        }
        public  IEnumerable<SparqlResult> SPo( VariableNode subj, VariableNode predicate, ObjectVariants obj, SparqlResult variablesBindings)
        {
            return SPoCache.Get(obj)
             .Select(triple => new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {predicate, new SparqlVariableBinding(predicate,triple.Predicate)},
                  {subj, new SparqlVariableBinding(subj,triple.Subject)}
              }));
        }

        public  IEnumerable<SparqlResult> SPoGraphs( VariableNode subj, VariableNode pred,
    ObjectVariants obj, SparqlResult variablesBindings, DataSet graphs)
        {
            return SPoCacheGraphs(obj, graphs)
             .SelectMany(grouping =>
                     grouping
                        .Select(triple => new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {pred, new SparqlVariableBinding(pred,triple.Predicate)},
                  {subj, new SparqlVariableBinding(subj,triple.Subject)}
              })));
        }
        public  IEnumerable<SparqlResult> SPoVarGraphs( VariableNode subj, VariableNode pred,
           ObjectVariants obj, SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            return SPoCacheGraphs(obj, variableDataSet) //.GetGraphUri(variablesBindings)
             .SelectMany(grouping =>
                     grouping
                        .Select(triple => new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {pred, new SparqlVariableBinding(pred,triple.Predicate)},
                  {subj, new SparqlVariableBinding(subj,triple.Subject)},
                  {variableDataSet.Variable, new SparqlVariableBinding(variableDataSet.Variable,grouping.Key)},
              })));
        }
        private IEnumerable<IGrouping<ObjectVariants, Triple<ObjectVariants,ObjectVariants,ObjectVariants>>> SPoCacheGraphs(ObjectVariants objectNode, DataSet variableDataSet)
        {
            if (variableDataSet.Any())
                if (SPoGCache.Contains(objectNode))
                    return SPoGCache.Get(objectNode);
                else
                {
                    var result = store.NamedGraphs.GetTriplesWithObjectFromGraphs(objectNode, variableDataSet).ToList();
                    SPoGCache.Add(objectNode, result);
                    foreach (var grouping in result)
                    {
                        SPogCache.Add(objectNode, grouping.Key, grouping);
                        foreach (var tripleGroup in grouping.GroupBy(triple => triple.Subject))
                        {
                            var pGroup = new Grouping<ObjectVariants, ObjectVariants>(grouping.Key,
                                tripleGroup.Select(triple => triple.Predicate));
                            sPogCache.Add(tripleGroup.Key, objectNode, grouping.Key, pGroup);

                            sPoGCache.Get(tripleGroup.Key, objectNode).Add(pGroup);
                            foreach (var p in pGroup)
                            {
                                spoGCache.Get(tripleGroup.Key, p, objectNode).Add(grouping.Key);
                                spogCache.Add(tripleGroup.Key, p, objectNode, grouping.Key, true);
                            }
                        }
                        foreach (var tripleGroup in grouping.GroupBy(triple => triple.Predicate))
                        {
                            var sGroup = new Grouping<ObjectVariants, ObjectVariants>(grouping.Key,
                                tripleGroup.Select(triple => triple.Subject));
                            SpogCache.Add(tripleGroup.Key, objectNode, grouping.Key, sGroup);
                            SpoGCache.Get(tripleGroup.Key, objectNode).Add(sGroup);
                        }
                    }
                    return result;
                }
            var res = new List<IGrouping<ObjectVariants, Triple<ObjectVariants,ObjectVariants,ObjectVariants>>>();
            var gList = new DataSet();
            foreach (var g in variableDataSet)
                if (SPogCache.Contains(objectNode, g))
                    res.Add(SPogCache.Get(objectNode, g));
                else
                    gList.Add(g);
            foreach (var gRes in store.NamedGraphs.GetTriplesWithObjectFromGraphs(objectNode, gList))
            {
                SPogCache.Add(objectNode, gRes.Key, gRes);
                res.Add(gRes);
                foreach (var tripleGroup in gRes.GroupBy(triple => triple.Subject))
                {
                    var pGroup = new Grouping<ObjectVariants, ObjectVariants>(gRes.Key,
                        tripleGroup.Select(triple => triple.Predicate));
                    sPogCache.Add(tripleGroup.Key, objectNode, gRes.Key, pGroup);

                    sPoGCache.Get(tripleGroup.Key, objectNode).Add(pGroup);
                    foreach (var p in pGroup)
                    {
                        spoGCache.Get(tripleGroup.Key, p, objectNode).Add(gRes.Key);
                        spogCache.Add(tripleGroup.Key, p, objectNode, gRes.Key, true);
                    }
                }
                foreach (var tripleGroup in gRes.GroupBy(triple => triple.Predicate))
                {
                    var sGroup = new Grouping<ObjectVariants, ObjectVariants>(gRes.Key,
                        tripleGroup.Select(triple => triple.Subject));
                    SpogCache.Add(tripleGroup.Key, objectNode, gRes.Key, sGroup);
                    SpoGCache.Get(tripleGroup.Key, objectNode).Add(sGroup);
                }
            }
            return res;
        }
     
        public  IEnumerable<SparqlResult> SPO( VariableNode subj, VariableNode predicate, VariableNode obj, SparqlResult variablesBindings)
        {
            return store
             .GetTriples()
             .Select(triple => new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {subj, new SparqlVariableBinding(subj,triple.Subject)} ,
                  {predicate, new SparqlVariableBinding(predicate,triple.Predicate)},
                  {obj, new SparqlVariableBinding(obj,triple.Object)}
              }));

        }
        public IEnumerable<SparqlResult> SPOGraphs(VariableNode subj, VariableNode predicate, VariableNode obj, SparqlResult variablesBindings, DataSet graphs)
        {
            return store
                 .NamedGraphs
             .GetTriplesFromGraphs(graphs)
                .SelectMany(grouping =>
                     grouping
             .Select(triple => new SparqlResult(new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
              {
                  {subj, new SparqlVariableBinding(subj,triple.Subject)} ,
                  {predicate, new SparqlVariableBinding(predicate,triple.Predicate)},
                  {obj, new SparqlVariableBinding(obj,triple.Object)}
              })));

        }

        public IEnumerable<SparqlResult> SPOVarGraphs(VariableNode subj, VariableNode predicate, VariableNode obj,
            SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            return store
                .NamedGraphs
                .GetTriplesFromGraphs(variableDataSet) //.GetGraphUri(variablesBindings)
                .SelectMany(grouping =>
                    grouping
                        .Select(
                            triple =>
                                new SparqlResult(
                                    new Dictionary<VariableNode, SparqlVariableBinding>(variablesBindings.row)
                                    {
                                        {subj, new SparqlVariableBinding(subj, triple.Subject)},
                                        {predicate, new SparqlVariableBinding(predicate, triple.Predicate)},
                                        {obj, new SparqlVariableBinding(obj, triple.Object)}
                                    })));
        }


        public IEnumerable<SparqlResult> spoGraphs(ObjectVariants subjectNode, ObjectVariants predicateNode, ObjectVariants objectNode,
              SparqlResult variablesBindings, DataSet graphs)
        {
            if (spoGraphsCache(subjectNode, predicateNode, objectNode, graphs).Any())
              yield return  variablesBindings;

        }

        public IEnumerable<SparqlResult> spoVarGraphs(ObjectVariants subjectNode, ObjectVariants predicateNode, ObjectVariants objectNode,
            SparqlResult variablesBindings, VariableDataSet graphs)
        {
            return spoGraphsCache(subjectNode, predicateNode, objectNode, graphs)  
              .Select(g => new SparqlResult(variablesBindings, g, graphs.Variable)) ;
        }

        public IEnumerable<SparqlResult> spo(ObjectVariants subjectNode, ObjectVariants predicateNode, ObjectVariants objNode, SparqlResult variablesBindings)
        {
            if (spoCache.Get(subjectNode, predicateNode, objNode))
                yield return variablesBindings;
        }

        private IEnumerable<ObjectVariants> spoGraphsCache(ObjectVariants subjectNode, ObjectVariants predicateNode, ObjectVariants objectNode, DataSet graphs)
        {
            if (graphs.Count == 0)
            {
                if (spoGCache.Contains(subjectNode, predicateNode, objectNode))
                    foreach (var g in spoGCache.Get(subjectNode, predicateNode, objectNode))
                        yield return g;
                else
                {
                    var resultGraphs = store.NamedGraphs.GetGraphs(subjectNode, predicateNode, objectNode, graphs);

                    spoGCache.Add(subjectNode, predicateNode, objectNode, resultGraphs);
                    foreach (var resultGraph in resultGraphs)
                        spogCache.Add(subjectNode, predicateNode, objectNode, resultGraph, true);
                }
            }
            else
            {
                DataSet ask = new DataSet();
                DataSet trues = new DataSet();
                foreach (var graph in graphs)
                    if (spogCache.Contains(subjectNode, predicateNode, objectNode, graph))
                    {
                        if (spogCache.Get(subjectNode, predicateNode, objectNode, graph))
                        {
                            trues.Add(graph);
                            yield return graph;
                        }       
                    }
                    else
                        ask.Add(graph);
                var resultGraphs = store.NamedGraphs.GetGraphs(subjectNode, predicateNode, objectNode, ask);

                trues.AddRange(resultGraphs);
                spoGCache.Add(subjectNode, predicateNode, objectNode, trues);
                foreach (var g in ask) 
                    spogCache.Add(subjectNode, predicateNode, objectNode, g, resultGraphs.Contains(g));

            }


        }

        private IEnumerable<IGrouping<ObjectVariants, Triple<ObjectVariants,ObjectVariants,ObjectVariants>>> SPOCacheGraphs(DataSet variableDataSet)
        {
          //   if (variableDataSet.Any())
                //if (SPOGCache.Contains(objectNode))
                //    return SPOGCache.Get(objectNode);
                //else
                {
                    var result = store.NamedGraphs.GetTriplesFromGraphs(variableDataSet);
                    //SPOGCache.Add(objectNode, result);                            .ToList()
                    //foreach (IGrouping<IUriNode, Triple<ObjectVariants,IUriNode,INode>> grouping in result)
                    //{
                    //    SPOgCache.Add(grouping.Key, true);
                    //    foreach (var tripleGroup in grouping.GroupBy(triple => triple.Subject))
                    //    {
                    //        var pGroup = new Grouping<IUriNode, IUriNode>(grouping.Key,
                    //           tripleGroup.Select(triple => triple.Predicate));
                    //        sPOgCache.Add(tripleGroup.Key,grouping.Key,tripleGroup);
                           
                    //        sPogCache.Add(tripleGroup.Key, objectNode, grouping.Key, pGroup);

                    //        sPoGCache.Get(tripleGroup.Key, objectNode).Add(pGroup);
                    //        foreach (var p in pGroup)
                    //        {
                    //            spoGCache.Get(tripleGroup.Key, p, objectNode).Add(grouping.Key);
                    //            spogCache.Add(tripleGroup.Key, p, objectNode, grouping.Key, true);
                    //        }
                    //    }
                    //    foreach (var tripleGroup in grouping.GroupBy(triple => triple.Predicate))
                    //    {
                    //        var sGroup = new Grouping<IUriNode, ObjectVariants>(grouping.Key,
                    //            tripleGroup.Select(triple => triple.Subject));
                    //        SpogCache.Add(tripleGroup.Key, objectNode, grouping.Key, sGroup);
                    //        SpoGCache.Get(tripleGroup.Key, objectNode).Add(sGroup);
                    //    }
                    //}
                    return result;
                }
            //var res = new List<IGrouping<IUriNode, Triple<ObjectVariants,IUriNode,INode>>>();
            //var gList = new List<IUriNode>();
            //foreach (var g in variableDataSet)
            //    if (SPogCache.Contains(objectNode, g))
            //        res.Add(SPogCache.Get(objectNode, g));
            //    else
            //        gList.Add(g);
            //foreach (var gRes in store.NamedGraphs.GetTriplesWithObjectFromGraphs(objectNode, gList))
            //{
            //    SPogCache.Add(objectNode, gRes.Key, gRes);
            //    res.Add(gRes);
            //    foreach (var tripleGroup in gRes.GroupBy(triple => triple.Subject))
            //    {
            //        var pGroup = new Grouping<IUriNode, IUriNode>(gRes.Key,
            //            tripleGroup.Select(triple => triple.Predicate));
            //        sPogCache.Add(tripleGroup.Key, objectNode, gRes.Key, pGroup);

            //        sPoGCache.Get(tripleGroup.Key, objectNode).Add(pGroup);
            //        foreach (var p in pGroup)
            //        {
            //            spoGCache.Get(tripleGroup.Key, p, objectNode).Add(gRes.Key);
            //            spogCache.Add(tripleGroup.Key, p, objectNode, gRes.Key, true);
            //        }
            //    }
            //    foreach (var tripleGroup in gRes.GroupBy(triple => triple.Predicate))
            //    {
            //        var sGroup = new Grouping<IUriNode, ObjectVariants>(gRes.Key,
            //            tripleGroup.Select(triple => triple.Subject));
            //        SpogCache.Add(tripleGroup.Key, objectNode, gRes.Key, sGroup);
            //        SpoGCache.Get(tripleGroup.Key, objectNode).Add(sGroup);
            //    }
            //}
            //return res;
        }
    }

}