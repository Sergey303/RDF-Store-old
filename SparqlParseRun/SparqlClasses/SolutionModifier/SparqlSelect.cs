using System.Collections.Generic;
using System.Linq;
using SparqlParseRun.SparqlClasses.Expressions;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.SolutionModifier
{
    public class SparqlSelect : List<IVariableNode>
    {

        private bool isAll;
        internal bool IsReduced;
        private RdfQuery11Translator q;

        public SparqlSelect(RdfQuery11Translator q)
        {
            this.q = q;
        }


        internal bool IsDistinct { get; set; }
    

        internal void IsAll()
        {
            isAll = true;
        }


        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings, SparqlResultSet resultSet, bool isGrouped)
        {
            List<VariableNode> selected=null;

            if (isAll)
            {
                selected = resultSet.Variables.Values.Where(v => !(v is SparqlBlankNode)).ToList();

            }
            else
            {
                var asExpressions = this.Select(varOrExpr => varOrExpr as SparqlExpressionAsVariable).ToArray();
                if (isGrouped)
                {
                    if (asExpressions.All(exp => exp != null))
                    {
                        if (asExpressions.All(exp =>
                            exp.sparqlExpression.AggregateLevel == SparqlExpression.VariableDependenceGroupLevel.GroupOfGroups))

                            return OneRowResult(variableBindings, asExpressions);
                    }
                    else
                    {
                        //todo
                    }
                }
                else
                {
                    if (asExpressions.All(exp => exp != null))
                    {
                        //if(asExpressions.All(exp=>exp.sparqlExpression.AggregateLevel==SparqlExpression.VariableDependenceGroupLevel.Const || exp.sparqlExpression.AggregateLevel==SparqlExpression.VariableDependenceGroupLevel.UndependableFunc))
                        if (asExpressions.All(exp =>
                            exp.sparqlExpression.AggregateLevel == SparqlExpression.VariableDependenceGroupLevel.Group))
                            return OneRowResult(variableBindings, asExpressions);
                    }
                }
          
            selected = new List<VariableNode>();

            foreach ( IVariableNode variable in this)
                {
                    var expr = variable as SparqlExpressionAsVariable;
                    if (expr != null)
                    {
                            variableBindings = isGrouped ? expr.Run4Grouped(variableBindings) : expr.Run(variableBindings);
                        selected.Add(expr.variableNode);
                    }
                    else selected.Add((VariableNode) variable);
                }
               }
            variableBindings = variableBindings.Select(result =>
                {
                    result.SetSelection(selected);
                    return result;
                });
            if (IsDistinct)
                variableBindings = Distinct(variableBindings);
            if (IsReduced)
                variableBindings = Reduce(variableBindings);

            return variableBindings;
        }

        private IEnumerable<SparqlResult> OneRowResult(IEnumerable<SparqlResult> variableBindings, SparqlExpressionAsVariable[] asExpressions)
        {
            var oneRowResult = new SparqlResult(q);
            oneRowResult.SetSelection(asExpressions.Select(exprVar => exprVar.variableNode));
            foreach (var sparqlExpressionAsVariable in asExpressions)
                oneRowResult.Add(sparqlExpressionAsVariable
                    .RunExpressionCreateBind(new SparqlGroupOfResults(q)
                    {
                        Group = variableBindings
                    }),
                    sparqlExpressionAsVariable.variableNode);
            return Enumerable.Range(0, 1).Select(i => oneRowResult);
        }

        private static IEnumerable<SparqlResult> Reduce(IEnumerable<SparqlResult> results)
        {
            var duplicated=new Dictionary<SparqlResult, bool>();
            foreach (var res in results)
            {
                if (duplicated.ContainsKey(res))
                {
                    if(duplicated[res]) continue;
                    duplicated[res] = true;
                }
                else duplicated.Add(res, false);
                yield return res;
            }
        }
        private static IEnumerable<SparqlResult> Distinct(IEnumerable<SparqlResult> results)
        {
            var history = new HashSet<SparqlResult>();
            foreach (var res in results.Where(res => !history.Contains(res)))
            {
                history.Add(res);
                yield return res;
            }
        }

        class SelectedComparer :  IEqualityComparer<SparqlResult>
        {
         

            public bool Equals(SparqlResult x, SparqlResult y)
            {
                //if (x.Count != y.Count) return false;
           
                return x.GetSelected((var, value) 
                    => 
                    (value == null && y[var]==null) || value!=null && value.Equals(y[var])).All(b=>b);
            }

            public int GetHashCode(SparqlResult obj)
            {
                unchecked
                {
                    return obj.GetHashCode();
                }
            }
        }

        

    }
}
