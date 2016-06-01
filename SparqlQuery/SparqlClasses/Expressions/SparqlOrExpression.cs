using System;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlOrExpression : SparqlExpression
    {
        public SparqlOrExpression(SparqlExpression l, SparqlExpression r)
            
        {
            //l.SetExprType(ObjectVariantEnum.Bool);
            //r.SetExprType(ObjectVariantEnum.Bool);
            //SetExprType(ObjectVariantEnum.Bool);         
            switch (NullablePairExt.Get(l.Const, r.Const))
            {
                case NP.bothNull:
                    Operator = res => l.Operator(res) || r.Operator(res);
                    AggregateLevel = SetAggregateLevel(l.AggregateLevel, r.AggregateLevel);
                    break;
                case NP.leftNull:
                    if ((bool) l.Const.Content)
                        Const = new OV_bool(true);
                    else
                    {
                        Operator = r.Operator;
                        AggregateLevel = r.AggregateLevel;
                    }
                    break;
                case NP.rigthNull:
                    if ((bool) r.Const.Content)
                        Const = new OV_bool(true);
                    else
                    {
                        Operator = l.Operator;
                        AggregateLevel = l.AggregateLevel;
                    }
                    break;
                case NP.bothNotNull:
                    Const = new OV_bool((bool)l.Const.Content || (bool) r.Const.Content); 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            TypedOperator = result => new OV_bool(Operator(result));
        }

        public static SparqlExpression Create(SparqlExpression l, SparqlExpression r)
        {
            return new SparqlOrExpression(l,r);
        }
    }
}
