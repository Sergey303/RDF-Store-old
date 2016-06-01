using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Expressions;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.GraphPattern.Triples
{
    public  class SparqlTripletsStoreCalls 
    {
        private readonly IStore store;
        
        public SparqlTripletsStoreCalls(IStore store)
        {
            this.store = store;
        }
   


        public IEnumerable<SparqlResult> spO(ObjectVariants subjNode, ObjectVariants predicateNode,
            VariableNode obj, SparqlResult variablesBindings)
        {
            return store.GetTriplesWithSubjectPredicate(subjNode, predicateNode)
                .ToArray()
                .Select(node => variablesBindings.Add(node, obj));
        }

        // from merged named graphs
        public IEnumerable<SparqlResult> spOVarGraphs(ObjectVariants subjNode, ObjectVariants predicateNode,
            VariableNode obj, SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            if(variableDataSet.Any())           
                return  variableDataSet.SelectMany(g => store
                .NamedGraphs
                .GetObject(subjNode, predicateNode, g)
                .Select(o =>variablesBindings.Add( o, obj, g, variableDataSet.Variable)));
            else return store
                .NamedGraphs
                .GetTriplesWithSubjectPredicate(subjNode, predicateNode,
                    ((o, g) =>variablesBindings.Add( o, obj, g, variableDataSet.Variable)));
                // if graphVariable is null, ctor check this.
        }

        public IEnumerable<SparqlResult> spOGraphs( ObjectVariants subjNode, ObjectVariants predicateNode,
            VariableNode obj, SparqlResult variablesBindings, DataSet graphs)
        {
            return graphs.SelectMany(graph => store
                .NamedGraphs
                .GetObject(subjNode, predicateNode, graph) // if graphs is empty, Gets All named graphs
                .Select(o =>variablesBindings.Add( o, obj)));
        }


        public IEnumerable<SparqlResult> Spo( VariableNode subj, ObjectVariants predicateNode, ObjectVariants objectNode, SparqlResult variablesBindings)
        {
              return store
                .GetTriplesWithPredicateObject(predicateNode, objectNode)
                .Select(node =>variablesBindings.Add( node, subj));

        }


        public IEnumerable<SparqlResult> SpoGraphs( VariableNode subj, ObjectVariants predicateNode,
            ObjectVariants objectNode, SparqlResult variablesBindings, DataSet graphs)
        {
            return graphs.SelectMany(g=>
                 store.NamedGraphs
                .GetSubject(predicateNode, objectNode, g)
                // if graphs is empty, Gets All named graphs
                .Select(node => variablesBindings.Add(node, subj)));
                // if graphVariable is null, ctor check this.
        }

        public IEnumerable<SparqlResult> SpoVarGraphs( VariableNode subj, ObjectVariants predicateNode,
            ObjectVariants objectNode, SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            if (variableDataSet.Any())
                return variableDataSet.SelectMany(g => store
                .NamedGraphs
                .GetSubject(predicateNode, objectNode, g)
                .Select(s =>variablesBindings.Add( s, subj, g, variableDataSet.Variable)));
            else 
            return store
                .NamedGraphs
                .GetTriplesWithPredicateObject(predicateNode, objectNode,
                    (s, g) =>variablesBindings.Add( s, subj, g, variableDataSet.Variable));
        }

        public IEnumerable<SparqlResult> SpO( VariableNode sVar, ObjectVariants predicate, VariableNode oVar, SparqlResult variablesBindings)
        {
            return store
                .GetTriplesWithPredicate(predicate).
                Select(t=>variablesBindings.Add( t.Subject, sVar, t.Object, oVar));
        }

        public IEnumerable<SparqlResult> SpOGraphs(VariableNode sVar, ObjectVariants predicate, VariableNode oVar, SparqlResult variablesBindings, DataSet graphs)
        {
            return graphs.SelectMany(graph =>
                store
                    .NamedGraphs
                    .GetTriplesWithPredicateFromGraph(predicate, graph,
                        (s, o) =>variablesBindings.Add( s, sVar, o, oVar)));
        }

        public IEnumerable<SparqlResult> SpOVarGraphs(VariableNode sVar, ObjectVariants predicate, VariableNode oVar, SparqlResult variablesBindings, VariableDataSet graphs)
        {
            if (graphs.Any())
                return
                    graphs.SelectMany(g =>
                        store.NamedGraphs.GetTriplesWithPredicateFromGraph(predicate, g, (s, o) =>
                           variablesBindings.Add( s, sVar, o, oVar, g, graphs.Variable)));
            else
                return store.NamedGraphs.GetTriplesWithPredicate(predicate, (s, o, g) =>
                   variablesBindings.Add( s, sVar, o, oVar, g, graphs.Variable));
        }


        public IEnumerable<SparqlResult> sPo( ObjectVariants subj, VariableNode pred, ObjectVariants obj, SparqlResult variablesBindings)
        {
            return store
                .GetTriplesWithSubjectObject(subj, obj)
                .Select(newObj =>variablesBindings.Add( newObj, pred));

        }

        public IEnumerable<SparqlResult> sPoGraphs(ObjectVariants subj, VariableNode pred, ObjectVariants obj, SparqlResult variablesBindings, DataSet graphs)
        {
            return graphs.SelectMany(graph =>
                store
                    .NamedGraphs
                    .GetPredicate(subj, obj, graph)
                    .Select(p =>variablesBindings.Add( p, pred)));
        }

        public IEnumerable<SparqlResult> sPoVarGraphs( ObjectVariants subj, VariableNode pred, ObjectVariants obj, SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            if (variableDataSet.Any())
                return variableDataSet.SelectMany(g =>
                    store.NamedGraphs.GetPredicate(subj, obj, g)
                        .Select(p =>variablesBindings.Add( p, pred, g, variableDataSet.Variable)));
            else   return store
                .NamedGraphs
                .GetTriplesWithSubjectObject(subj, obj, (p, g) =>
                   variablesBindings.Add( p, pred, g, variableDataSet.Variable));
        }


        public IEnumerable<TripleOVStruct> sPO( ObjectVariants subj)
        {
            return store
                .GetTriplesWithSubject(subj);
        }

        public IEnumerable<SparqlResult> sPOGraphs( ObjectVariants subj, VariableNode pred,
            VariableNode obj, SparqlResult variablesBindings, DataSet graphs)
        {
            return graphs.SelectMany(g => store
                .NamedGraphs
                .GetTriplesWithSubjectFromGraph(subj, g, (p, o) =>
                   variablesBindings.Add( p, pred, o, obj)));
        }

        public IEnumerable<SparqlResult> sPOVarGraphs( ObjectVariants subj, VariableNode pred,
           VariableNode obj, SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            if (variableDataSet.Any())
                return variableDataSet.SelectMany(g =>
                    store
                        .NamedGraphs
                        .GetTriplesWithSubjectFromGraph(subj, g, (p, o) =>
                           variablesBindings.Add( p, pred, o, obj, g, variableDataSet.Variable))); 
            else
                return store
                    .NamedGraphs
                    .GetTriplesWithSubject(subj, (p, o, g) =>
                       variablesBindings.Add(   p, pred, o, obj, g, variableDataSet.Variable)); 
        }

        public IEnumerable<SparqlResult> SPo( VariableNode subj, VariableNode predicate, ObjectVariants obj, SparqlResult variablesBindings)
        {
            return store
                .GetTriplesWithObject(obj, (s, p) =>
                   variablesBindings.Add( p, predicate, s, subj));
        }

        public IEnumerable<SparqlResult> SPoGraphs( VariableNode subj, VariableNode pred,
    ObjectVariants obj, SparqlResult variablesBindings, DataSet graphs)
        {
            return graphs.SelectMany(g =>
                store
                    .NamedGraphs
                    .GetTriplesWithObjectFromGraph(obj, g, (s, p) =>
                       variablesBindings.Add( p, pred, s, subj)));
        }

        public IEnumerable<SparqlResult> SPoVarGraphs( VariableNode subj, VariableNode pred,
           ObjectVariants obj, SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            if (variableDataSet.Any())
                return variableDataSet.SelectMany(g =>
                    store
                        .NamedGraphs
                        .GetTriplesWithObjectFromGraph(obj, g, (s, p) =>
                           variablesBindings.Add( p, pred, s, subj, g, variableDataSet.Variable)));
            return store
                .NamedGraphs
                .GetTriplesWithObject(obj, (s, p, g) =>
                   variablesBindings.Add( p, pred, s, subj, g, variableDataSet.Variable));
        }


        public IEnumerable<SparqlResult> SPO( VariableNode subj, VariableNode predicate, VariableNode obj, SparqlResult variablesBindings)
        {
            return store
                .GetTriples((s, p, o)
                    =>variablesBindings.Add( s, subj, p, predicate, o, obj));
        }

        public IEnumerable<SparqlResult> SPOGraphs(VariableNode subj, VariableNode predicate, VariableNode obj, SparqlResult variablesBindings, DataSet graphs)
        {
            return graphs.SelectMany(g =>
                store.NamedGraphs
                    .GetTriplesFromGraph(g, (s, p, o) =>
                       variablesBindings.Add( s, subj, p, predicate, o, obj)));
        }

        public IEnumerable<SparqlResult> SPOVarGraphs( VariableNode subj, VariableNode predicate, VariableNode obj, SparqlResult variablesBindings, VariableDataSet variableDataSet)
        {
            if (variableDataSet.Any())
                return variableDataSet.SelectMany(g =>
                    store
                        .NamedGraphs
                        .GetTriplesFromGraph(g, (s, p, o) =>
                           variablesBindings.Add( s, subj, p, predicate, o, obj, g,
                                variableDataSet.Variable)));
            return store
                .NamedGraphs
                .GetAll((s, p, o, g) =>
                   variablesBindings.Add( s, subj, p, predicate, o, obj, g,
                        variableDataSet.Variable));
        }

        public IEnumerable<SparqlResult> spoGraphs(ObjectVariants subjectNode, ObjectVariants predicateNode, ObjectVariants objectNode, SparqlResult variablesBindings,
            DataSet graphs)
        {                                                                       
            if (graphs.Any(g=> store.NamedGraphs.Contains(subjectNode, predicateNode, objectNode, g)))
              yield return  variablesBindings;
        }

    

        public IEnumerable<SparqlResult> spoVarGraphs(ObjectVariants subjectNode, ObjectVariants predicateNode, ObjectVariants objectNode,
            SparqlResult variablesBindings, VariableDataSet graphs)
        {
            return (graphs.Any()
                ? graphs.Where(g => store.NamedGraphs.Contains(subjectNode, predicateNode, objectNode, g))
                : store.NamedGraphs.GetGraph(subjectNode, predicateNode, objectNode))
                    .Select(g =>variablesBindings.Add( g, graphs.Variable));
        }

        public IEnumerable<SparqlResult> spo(ObjectVariants subjectNode, ObjectVariants predicateNode, ObjectVariants objNode, SparqlResult variablesBindings)
        {
            if (store.Contains(subjectNode, predicateNode, objNode))
                yield return variablesBindings;
        }
    }
}