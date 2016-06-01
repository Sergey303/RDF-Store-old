using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;
using SparqlParseRun.SparqlClasses.SolutionModifier;

namespace SparqlParseRun.SparqlClasses.Query
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
            this.sparqlSolutionModifier = sparqlSolutionModifier;
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
                    foreach (var t in q.Store.GetTriplesWithSubject(node))
                    {
                        rdfInMemoryGraph.Add(node1, t.Predicate, t.Object);
                    }
                    foreach (var t in q.Store.GetTriplesWithObject(node))
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
                    foreach (var t in q.Store.GetTriplesWithSubject(node))
                    {
                        rdfInMemoryGraph.Add(node1, t.Predicate, t.Object);
                    }
                    foreach (var t in q.Store.GetTriplesWithObject(node))
                    {
                        rdfInMemoryGraph.Add(t.Subject, t.Predicate, node1);
                    }
                }
                foreach (ObjectVariants node in nodeList.Where(node => !(node is VariableNode)))

                    {
                        ObjectVariants node1 = node;
                        foreach (var t in q.Store.GetTriplesWithSubject(node))
                        {
                            rdfInMemoryGraph.Add(node1, t.Predicate, t.Object);
                        }
                        foreach (var t in q.Store.GetTriplesWithObject(node))
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
