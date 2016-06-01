using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlAndExpression : SparqlExpression
    {
      


        public SparqlAndExpression(SparqlExpression l, SparqlExpression r)
        {      
            //l.SetExprType(ObjectVariantEnum.Bool); 
            //r.SetExprType(ObjectVariantEnum.Bool);
            //SetExprType(ObjectVariantEnum.Bool);      
            switch (NullablePairExt.Get(l.Const, r.Const))
            {
                case NP.bothNull:
                    Operator = res => l.Operator(res) && r.Operator(res);
                    break;
                case NP.leftNull:
                    if (!(bool)l.Const.Content)
                        Const = new OV_bool(false);
                    else
                    {
                        Operator = r.Operator;
                        AggregateLevel = r.AggregateLevel;
                    }
                    break;
                case NP.rigthNull:
                    if (!(bool)r.Const.Content)
                        Const = new OV_bool(false);
                    else
                    {
                        Operator = l.Operator;
                        AggregateLevel = l.AggregateLevel;
                    }
                    break;
                case NP.bothNotNull:
                    Const = new OV_bool((bool)l.Const.Content && (bool)r.Const.Content);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            TypedOperator = result => new OV_bool(Operator(result));
        }

     
    }
 
}
