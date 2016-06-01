using System;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.SparqlAggregateExpression
{
    class SparqlCountExpression : SparqlAggregateExpression
    {
        public SparqlCountExpression() :base()
        {          
            Const=null;
            if (IsDistinct)
            {
                Operator = result =>
                {
                    var groupOfResults = result as SparqlGroupOfResults;
                    if (groupOfResults != null)
                        return new OV_int(groupOfResults.Group.Count());
                    else throw new Exception();
                };
            }
            else
            {
                Operator = result =>
                {
                    var groupOfResults = result as SparqlGroupOfResults;
                    if (groupOfResults != null)
                        return new OV_int(groupOfResults.Group.Count());
                    else throw new Exception();
                };                                                 
            }
            TypedOperator = result => new OV_int(Operator(result));
        }

        protected override void Create()
        {
            throw new NotImplementedException();
        }
    }
}
