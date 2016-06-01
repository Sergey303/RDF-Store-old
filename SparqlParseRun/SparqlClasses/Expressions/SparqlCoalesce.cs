using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlCoalesce : SparqlExpression
    {
        public SparqlCoalesce(List<SparqlExpression> list)
        {
            if(list.Count==0) throw new Exception();
          
            
            Const = list[0].Const;
            AggregateLevel = list[0].AggregateLevel;
            Operator = result =>
            {
                foreach (SparqlExpression expression in list)
                {
                    try
                    {
                        if (expression.Const != null) return expression.Const.Content;
                        return expression.Operator(result);
                    }
                    catch // (Exception e)
                    {
                        // Console.WriteLine(e.Message);
                    }
                }
                throw new Exception("Coalesce ");
            };
            TypedOperator = result =>     
            {
                foreach (var expression in list)
                    try
                    {
                        if (expression.Const != null) return expression.Const;
                        var test = expression.TypedOperator(result);
                      //  if(test is SparqlUnDefinedNode) continue;
                        return test;
                    }
                    catch// (Exception e)
                    {
                       // Console.WriteLine(e.Message);
                    }
                throw new Exception("Coalesce ");
            };
        }
    }
}
