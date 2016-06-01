using System;
using System.Collections.Generic;
using System.Linq;
using SparqlParseRun.SparqlClasses.Expressions;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.SolutionModifier
{
    public class SparqlSolutionModifierHaving : List<SparqlExpression>
    {
        internal IEnumerable<SparqlResult> Having(IEnumerable<SparqlResult> resultsGroups, RdfQuery11Translator q)
        {
            //   IEnumerable<SparqlResult> resultsGroups = enumerable as SparqlResult[] ?? enumerable.ToArray();
            //if (!resultsGroups.Any()) return resultsGroups; //todo

            foreach (var testExpr in this)
                switch (testExpr.AggregateLevel)
                {
                    case SparqlExpression.VariableDependenceGroupLevel.Const:
                        if ((bool) testExpr.Const.Content) continue;
                        else return Enumerable.Empty<SparqlResult>();
                    case SparqlExpression.VariableDependenceGroupLevel.UndependableFunc:
                        if (testExpr.Test(null)) continue;
                        else return Enumerable.Empty<SparqlResult>();
                    case SparqlExpression.VariableDependenceGroupLevel.SimpleVariable:
                        //if (resultsGroups.Any(testExpr.Test)) continue;
                        //else return Enumerable.Empty<SparqlResult>();//todo
                        resultsGroups = resultsGroups.Where(testExpr.Test);
                        break;
                    case SparqlExpression.VariableDependenceGroupLevel.Group:
                        if (testExpr.Test(new SparqlGroupOfResults(q) {Group = resultsGroups})) continue;
                        else return Enumerable.Empty<SparqlResult>();
                    case SparqlExpression.VariableDependenceGroupLevel.GroupOfGroups:
                        throw new Exception("requested grouping");
                }
            return resultsGroups;
        }

        internal IEnumerable<SparqlGroupOfResults> Having4CollectionGroups(IEnumerable<SparqlGroupOfResults> resultsGroups, RdfQuery11Translator q)
        {
            foreach (var testExpr in this)
                switch (testExpr.AggregateLevel)
                {
                    case SparqlExpression.VariableDependenceGroupLevel.Const:
                        if ((bool) testExpr.Const.Content) continue;
                        else return Enumerable.Empty<SparqlGroupOfResults>();
                    case SparqlExpression.VariableDependenceGroupLevel.UndependableFunc:
                        if ((bool) testExpr.Operator(null)) continue;
                        else return Enumerable.Empty<SparqlGroupOfResults>();
                    case SparqlExpression.VariableDependenceGroupLevel.SimpleVariable:
                    case SparqlExpression.VariableDependenceGroupLevel.Group:
                        SparqlExpression expr = testExpr;
                        resultsGroups = resultsGroups.Where(result => expr.Operator(result));
                        continue;
                    case SparqlExpression.VariableDependenceGroupLevel.GroupOfGroups:
                        if (testExpr.Test(new SparqlGroupOfResults(q) {Group = resultsGroups})) continue;
                        break;
                }
            return resultsGroups;
        }              
    }
}

