using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.SparqlAggregateExpression
{
    class SparqlSampleExpression : SparqlAggregateExpression
    {
        protected override void Create()
        {
            Const = Expression.Const;
            Random random=new Random();
            TypedOperator = result =>
            {
                    var spraqlGroupOfResults = (result as SparqlGroupOfResults);
                if (spraqlGroupOfResults != null)
                    return
                        Expression.TypedOperator(
                            spraqlGroupOfResults.Group.ElementAt(random.Next(spraqlGroupOfResults.Group.Count())));
                else throw new Exception();
            };
          
                Operator = result =>
                {                      
                    var spraqlGroupOfResults = (result as SparqlGroupOfResults);
                    if (spraqlGroupOfResults != null)
                        return
                            Expression.Operator(spraqlGroupOfResults.Group.ElementAt(random.Next(spraqlGroupOfResults.Group.Count())));
                    else throw new Exception();
                };
              
         
        }

     
    }
}
