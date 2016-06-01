using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using SparqlQuery.SparqlClasses.Expressions;
using SparqlQuery.SparqlClasses.GraphPattern.Triples.Node;
using SparqlQuery.SparqlClasses.Query.Result;

namespace SparqlQuery.SparqlClasses.SolutionModifier
{         
    public class SparqlOrderCondition 
    {
        private readonly Func<dynamic, dynamic> orderCondition = node =>
        {
            //if (node is SparqlUnDefinedNode) return string.Empty;
            //if (node is IBlankNode) return node.ToString();
            //if (node is ILiteralNode) return node.ToString();
            //if (node is IIriNode) return node.ToString();  
            return node;
        };
        private readonly Func<SparqlResult, dynamic> getNode;

        private readonly Func<dynamic, int> orderByTypeCondition = node =>
        {
            if (node is SparqlUnDefinedNode)
                return 0;
            if (node is IBlankNode)
                return 1;
            if (node is IIriNode )
                return 2;
            if (node is IStringLiteralNode)
                return 3;
            return 4;
        };
        private SparqlOrderDirection direction=SparqlOrderDirection.Asc;
        private readonly SparqlExpression.VariableDependenceGroupLevel AggregateLevel;
        private RdfQuery11Translator q;

        public SparqlOrderCondition(SparqlExpression sparqlExpression, string dir, RdfQuery11Translator q)
        {
            this.q = q;
            // TODO: Complete member initialization
            switch (dir.ToLower())
            {
                case "desc":
                    direction = SparqlOrderDirection.Desc;
                    break;
                case "asc":
                default:
                    direction = SparqlOrderDirection.Asc;
                    break;
            }
            getNode = sparqlExpression.Operator;
            AggregateLevel = sparqlExpression.AggregateLevel;
        }

        private enum SparqlOrderDirection
        {
            Desc                                                 ,
            Asc
        }

        public SparqlOrderCondition(SparqlExpression sparqlExpression, RdfQuery11Translator q)
        {
            this.q = q;
            // TODO: Complete member initialization
            getNode = sparqlExpression.Operator;
            AggregateLevel = sparqlExpression.AggregateLevel;

        }

        public SparqlOrderCondition(SparqlFunctionCall sparqlFunctionCall, RdfQuery11Translator q)
        {
            this.q = q;
            // TODO: Complete member initialization
            getNode = sparqlFunctionCall.Operator;
            AggregateLevel = sparqlFunctionCall.AggregateLevel;
        }

        public SparqlOrderCondition(VariableNode variableNode, RdfQuery11Translator q)
        {
            this.q = q;
            // TODO: Complete member initialization
            getNode = result => result[variableNode] ?? new SparqlUnDefinedNode();
            AggregateLevel=SparqlExpression.VariableDependenceGroupLevel.SimpleVariable;
        }

        public IEnumerable<SparqlResult> Order(IEnumerable<SparqlResult> resultSet)
       {
            if (AggregateLevel == SparqlExpression.VariableDependenceGroupLevel.Const)
                return resultSet;
            SparqlResult[] toOrderArray = resultSet as SparqlResult[] ??
                                          resultSet.Select(result => result.Clone()).ToArray();
            if (toOrderArray.Length < 2) return toOrderArray;
                if (AggregateLevel == SparqlExpression.VariableDependenceGroupLevel.Group)
                {
                   //if (toOrderArray is SparqlGroupsCollection) return toOrderArray.Cast<SparqlGroupOfResults>().Select(g => { g.Group = Order(g.Group); return g;});
                   // else
                        toOrderArray = new SparqlResult[] {new SparqlGroupOfResults(q) {Group = toOrderArray}};
                }
                else if (AggregateLevel == SparqlExpression.VariableDependenceGroupLevel.GroupOfGroups)
                {
                    if (!(toOrderArray[0] is SparqlGroupOfResults)) throw new Exception("requested grouping");
                    toOrderArray = new SparqlResult[] {new SparqlGroupOfResults(q) {Group = toOrderArray}};
                }
            switch (direction)
           {
               case SparqlOrderDirection.Desc:
                   return from r in toOrderArray
                              let node=getNode(r)
                              orderby orderByTypeCondition(node) descending, orderCondition(node) descending 
                              select r;
                   break;
               case SparqlOrderDirection.Asc:
               default:
                   return from r in toOrderArray
                          let node = getNode(r)
                          orderby orderByTypeCondition(node), orderCondition(node)
                          select r;
                   break;
           }
        }
        public IEnumerable<SparqlResult> Order4Grouped(IEnumerable<SparqlResult> resultSet)
        {
            if (AggregateLevel == SparqlExpression.VariableDependenceGroupLevel.Const)
                return resultSet;
            var toOrderArray = resultSet.Select(result => result.Clone());
                if (AggregateLevel == SparqlExpression.VariableDependenceGroupLevel.GroupOfGroups)
                    toOrderArray = new SparqlResult[] {new SparqlGroupOfResults(q) {Group = toOrderArray}};
            switch (direction)
            {
                case SparqlOrderDirection.Desc:
                    return from r in toOrderArray
                           let node = getNode(r)
                           orderby orderByTypeCondition(node) descending, orderCondition(node) descending
                           select r;
                    break;
                case SparqlOrderDirection.Asc:
                default:
                    return from r in toOrderArray
                           let node = getNode(r)
                           orderby orderByTypeCondition(node), orderCondition(node)
                           select r;
                    break;
            }
        }
     

     
    }
}
