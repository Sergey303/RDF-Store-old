using System;
using System.Collections.Generic;
using System.Linq;
using SparqlQuery.SparqlClasses.Expressions;
using SparqlQuery.SparqlClasses.Query.Result;

namespace SparqlQuery.SparqlClasses.SolutionModifier
{
    public class SparqlSolutionModifierHaving : List<SparqlExpression>
    {
        internal IEnumerable<SparqlResult> Having(IEnumerable<SparqlResult> resultsGroups, RdfQuery11Translator q)
        {
            //   IEnumerable<SparqlResult> resultsGroups = enumerable as SparqlResult[] ?? enumerable.ToArray();
            //if (!resultsGroups.Any()) return resultsGroups; //todo

            return this.Aggregate(resultsGroups, (result, testExpr) =>
            {
                switch (testExpr.AggregateLevel)
                {
                    case SparqlExpression.VariableDependenceGroupLevel.Const:
                        return (bool) testExpr.Const.Content ? result : Enumerable.Empty<SparqlResult>();
                    case SparqlExpression.VariableDependenceGroupLevel.UndependableFunc:
                        return testExpr.Test(null).Any() ? result : Enumerable.Empty<SparqlResult>();
                    case SparqlExpression.VariableDependenceGroupLevel.SimpleVariable:
                        return result.SelectMany(testExpr.Test);
                    case SparqlExpression.VariableDependenceGroupLevel.Group:
                        return testExpr.Test(new SparqlGroupOfResults(q) {Group = result });
                    case SparqlExpression.VariableDependenceGroupLevel.GroupOfGroups:
                    default:
                        throw new Exception("requested grouping");
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        internal IEnumerable<SparqlGroupOfResults> Having4CollectionGroups(
            IEnumerable<SparqlGroupOfResults> resultsGroups, RdfQuery11Translator q)
        {
            return this.Aggregate(resultsGroups, (result, testExpr) =>
            {
                switch (testExpr.AggregateLevel)
                {
                    case SparqlExpression.VariableDependenceGroupLevel.Const:
                        if ((bool) testExpr.Const.Content) return result;
                        else return Enumerable.Empty<SparqlGroupOfResults>();
                    case SparqlExpression.VariableDependenceGroupLevel.UndependableFunc:
                        if ((bool) testExpr.Operator(null)) return result;
                        else return Enumerable.Empty<SparqlGroupOfResults>();
                    case SparqlExpression.VariableDependenceGroupLevel.SimpleVariable:
                    case SparqlExpression.VariableDependenceGroupLevel.Group:
                        SparqlExpression expr = testExpr;
                        return result.Select(res => expr.Test(res));
                    case SparqlExpression.VariableDependenceGroupLevel.GroupOfGroups:
                        return testExpr.Test(new SparqlGroupOfResults(q) {Group = resultsGroups});
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return resultsGroups;
            });
        }
    }
}

