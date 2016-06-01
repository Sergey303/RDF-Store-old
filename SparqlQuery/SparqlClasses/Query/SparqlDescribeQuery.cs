using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlQuery.SparqlClasses.GraphPattern;
using SparqlQuery.SparqlClasses.GraphPattern.Triples.Node;
using SparqlQuery.SparqlClasses.Query.Result;
using SparqlQuery.SparqlClasses.SolutionModifier;

namespace SparqlQuery.SparqlClasses.Query
{
    public class SparqlDescribeQuery : SparqlQuery
    {
        private readonly List<ObjectVariants> nodeList = new List<ObjectVariants>();
        private bool isAll;


        public SparqlDescribeQuery(RdfQuery11Translator q) : base(q)
        {
            
        }

        internal void Add(ObjectVariants sparqlNode)
        {
            nodeList.Add(sparqlNode);
        }

        internal void IsAll()
        {
            isAll = true;
        }

        internal void Create(ISparqlGraphPattern sparqlWhere)
        {
            this.sparqlWhere = sparqlWhere;
        }

        internal void Create(SparqlSolutionModifier sparqlSolutionModifier)
        {
            this.SparqlSolutionModifier = sparqlSolutionModifier;
        }

        public override SparqlResultSet Run()
         {
            base.Run();
            var rdfInMemoryGraph = q.Store.CreateTempGraph();
            if (isAll)
                foreach (ObjectVariants node in ResultSet.Results.SelectMany(result => q.Variables.Values.Select(v=>result[v])))
                    //.Where(node => node is ObjectVariants).Cast<ObjectVariants>()))
                {
                    ObjectVariants node1 = node;
                    var ovIri = node as OV_iri;
                    if (ovIri != null)
                        if (!q.Store.NodeGenerator.TryGetUri(ovIri, out node1)) continue;
                    foreach (var t in q.Store.GetTriplesWithSubject(node1))
                    {
                        rdfInMemoryGraph.Add(node1, t.Predicate, t.Object);
                    }
                    foreach (var t in q.Store.GetTriplesWithObject(node1))
                    {
                        rdfInMemoryGraph.Add(t.Subject, t.Predicate, node1);
                    }
                }
            else
            {
                foreach (ObjectVariants node in nodeList
                    .Where(node => node is VariableNode)
                    .Cast<VariableNode>()
                    .SelectMany(v=> ResultSet.Results.Select(result => result[v])))
                        //.Where(node => node is ObjectVariants).Cast<ObjectVariants>()
                {
                    ObjectVariants node1 = node;
                    var ovIri = node as OV_iri;
                    if (ovIri != null)
                        if (!q.Store.NodeGenerator.TryGetUri(ovIri, out node1)) continue;
                    foreach (var t in q.Store.GetTriplesWithSubject(node1))
                    {
                        rdfInMemoryGraph.Add(node1, t.Predicate, t.Object);
                    }
                    foreach (var t in q.Store.GetTriplesWithObject(node1))
                    {
                        rdfInMemoryGraph.Add(t.Subject, t.Predicate, node1);
                    }
                }
                foreach (ObjectVariants node in nodeList.Where(node => !(node is VariableNode)))

                    {
                    ObjectVariants node1 = node;
                        var ovIri = node as OV_iri;
                        if (ovIri != null)
                        if(!q.Store.NodeGenerator.TryGetUri(ovIri, out node1)) continue;
                        foreach (var t in q.Store.GetTriplesWithSubject(node1))
                        {
                            rdfInMemoryGraph.Add(node1, t.Predicate, t.Object);
                        }
                        foreach (var t in q.Store.GetTriplesWithObject(node1))
                        {
                            rdfInMemoryGraph.Add(t.Subject, t.Predicate, node1);
                        }
                    }
            }
            ResultSet.ResultType = ResultType.Describe;
            ResultSet.GraphResult = rdfInMemoryGraph;
            return ResultSet;
        }

        //public override SparqlQueryTypeEnum QueryType
        //{
        //    get { return SparqlQueryTypeEnum.Describe; }
        //}
    }
}
