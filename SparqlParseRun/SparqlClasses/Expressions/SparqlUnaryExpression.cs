using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlUnaryExpression : SparqlExpression
    {


        public SparqlUnaryExpression(Func<dynamic, dynamic> @operator, SparqlExpression child)
            : base(child.AggregateLevel, child.IsStoreUsed)
        {
            var childConst = child.Const;

            if (childConst != null) Const = childConst.Change(@operator(childConst.Content));
            else
            {
                Operator = result => @operator(child.Operator(result));   
                TypedOperator = results => child.TypedOperator(results).Change(@operator);
            }
        }
    }

    internal class SparqlUnaryExpression<T> : SparqlExpression where T : ObjectVariants
    {
        public SparqlUnaryExpression(Func<dynamic, dynamic> @operator, SparqlExpression child,
            Func<dynamic, ObjectVariants> ctor)
            : base(child.AggregateLevel, child.IsStoreUsed)
        {
            Operator = @operator;
         
            var childConst = child.Const;
            if (childConst != null) Const = ctor(@operator(childConst.Content));
            else
            {
                Operator = result => @operator(child.Operator(result));
                TypedOperator = results => ctor(Operator(results).Change(@operator));
            }
        }
    }
}
