using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.SparqlAggregateExpression
{
    class SparqlAvgExpression : SparqlAggregateExpression
    { 
        protected override void Create()
        {
            Const = Expression.Const;
            if (IsDistinct)
            {
                Operator = result =>
                {
                    if (result is SparqlGroupOfResults)
                    {
                        var @group = (result as SparqlGroupOfResults).Group.ToArray();

                        if (group.Length == 0) return 0;
                        if (group.Length == 1) return Expression.Operator(group[0]);
                        return @group.Select(Expression.Operator).Distinct().Average(o1 => o1);
                    }
                    else throw new Exception();
                };
                TypedOperator = result =>
                {
                    if (result is SparqlGroupOfResults)
                    {
                        var @group = (result as SparqlGroupOfResults).Group.ToArray();
                        //.Select(sparqlResult => sparqlResult.Clone())
                        if (group.Length == 0) return new OV_int(0);
                        if (group.Length == 1) return Expression.TypedOperator(group[0]);
                        var firstTyped = Expression.TypedOperator(@group[0]);
                        var list = new List<dynamic>() { firstTyped.Content };
                        list.AddRange(@group.Skip(1).Select(Expression.Operator));

                        return firstTyped.Change(o => list.Distinct().Average(o1 => o1));

                    }
                    else throw new Exception();
                };
            }
            else
            {
                Operator = result =>
                {
                    if (result is SparqlGroupOfResults)
                    {
                        var @group = (result as SparqlGroupOfResults).Group.ToArray();

                        if (group.Length == 0) return 0;
                        if (group.Length == 1) return Expression.Operator(group[0]);

                        return @group.Select(Expression.Operator).Average(o1 => o1);
                    }
                    else throw new Exception();
                };
                TypedOperator = result =>
                {
                    if (result is SparqlGroupOfResults)
                    {
                        var @group = (result as SparqlGroupOfResults).Group.ToArray();
                        //.Select(sparqlResult => sparqlResult.Clone())
                        if (group.Length == 0) return new OV_int(0);
                        if (group.Length == 1) return Expression.TypedOperator(group[0]);
                        var firstTyped = Expression.TypedOperator(@group[0]);
                        var list = new List<dynamic>() { firstTyped.Content };
                        list.AddRange(@group.Skip(1).Select(Expression.Operator));
                        return firstTyped.Change(o => list.Average(o1 => o1));
                    }
                    else throw new Exception();
                };
            }
        }
    }
}
