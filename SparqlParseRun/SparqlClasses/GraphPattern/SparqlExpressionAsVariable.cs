using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Expressions;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.GraphPattern
{
    public class SparqlExpressionAsVariable : IVariableNode, ISparqlGraphPattern
    {
        public VariableNode variableNode;
        public SparqlExpression sparqlExpression;
        private readonly RdfQuery11Translator q;

        public SparqlExpressionAsVariable(VariableNode variableNode, SparqlExpression sparqlExpression, RdfQuery11Translator q)
        {
            // TODO: Complete member initialization
            this.variableNode = variableNode;
            this.sparqlExpression = sparqlExpression;
            this.q = q;
        }

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            switch (sparqlExpression.AggregateLevel)
            {
                case SparqlExpression.VariableDependenceGroupLevel.Const:
                    return variableBindings.Select(
                        variableBinding =>
                        {
                            variableBinding.Add(variableNode, sparqlExpression.Const);
                            return variableBinding;
                        });
                    break;
                case SparqlExpression.VariableDependenceGroupLevel.UndependableFunc:
                case SparqlExpression.VariableDependenceGroupLevel.SimpleVariable:
                    return variableBindings.Select(
                        variableBinding =>
                        {
                            variableBinding.Add(variableNode, sparqlExpression.TypedOperator(variableBinding));
                            return variableBinding;
                        });
                case SparqlExpression.VariableDependenceGroupLevel.Group:
                    sparqlExpression.TypedOperator(new SparqlGroupOfResults(q) {Group = variableBindings});
                    break;
                case SparqlExpression.VariableDependenceGroupLevel.GroupOfGroups:
                    throw new Exception("groping requested");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return variableBindings.Select(
                variableBinding =>
                {
                    variableBinding.Add(variableNode,RunExpressionCreateBind(variableBinding));
                    return variableBinding;
                });
        }
        public IEnumerable<SparqlResult> Run4Grouped(IEnumerable<SparqlResult> variableBindings)
        {
            switch (sparqlExpression.AggregateLevel)
            {
                case SparqlExpression.VariableDependenceGroupLevel.Const:
                    return variableBindings.Select(
                        variableBinding =>
                        {
                            variableBinding.Add(variableNode, sparqlExpression.Const);
                            return variableBinding;
                        });
                    break;
                case SparqlExpression.VariableDependenceGroupLevel.UndependableFunc:
                case SparqlExpression.VariableDependenceGroupLevel.SimpleVariable:
                 
                case SparqlExpression.VariableDependenceGroupLevel.Group:
                    return RunAddVar(variableBindings);
                    break;
                case SparqlExpression.VariableDependenceGroupLevel.GroupOfGroups:
                    var arr = variableBindings as SparqlResult[] ?? variableBindings.Select(result => result.Clone()).ToArray();
                    var groupOfGroups = new SparqlGroupOfResults(q) {Group = arr};
                    return arr.Select(variableBinding =>
                    {
                        variableBinding.Add(variableNode, sparqlExpression.TypedOperator(groupOfGroups));
                        return variableBinding;
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
           
        }

        public IEnumerable<SparqlResult> RunAddVar(IEnumerable<SparqlResult> variableBindings)
        {
            return variableBindings.Select(
                variableBinding =>
                {
                    variableBinding.Add(variableNode, sparqlExpression.TypedOperator(variableBinding));
                    return variableBinding;
                });
        }

        public SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.Bind;} }

        public ObjectVariants RunExpressionCreateBind(SparqlResult variableBinding)
        {
             return sparqlExpression.TypedOperator(variableBinding);
        }


      
    }

}
