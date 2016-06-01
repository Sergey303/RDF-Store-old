using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    public class SparqlBinaryExpression : SparqlExpression
    {
        private readonly SparqlExpression l;
        private readonly SparqlExpression r;
        private readonly Func<dynamic, dynamic, dynamic> @operator;

        protected internal SparqlBinaryExpression(SparqlExpression l, SparqlExpression r, Func<dynamic, dynamic, dynamic> @operator)
        {
            this.l = l;
            this.r = r;
          this. @operator = @operator;
        }

        public virtual void Create()
        {
            var lc = l.Const;
            var rc = r.Const;

            switch (NullablePairExt.Get(lc, rc))
            {
                case NP.bothNull:
                    Operator = result => @operator(l.Operator(result), r.Operator(result));
                    TypedOperator = result => ApplyTyped(l.TypedOperator(result), r.TypedOperator(result));
                    AggregateLevel = SetAggregateLevel(l.AggregateLevel, r.AggregateLevel);
                    break;
                case NP.leftNull:
                    Operator = result => @operator(l.Operator(result), rc.Content);
                    TypedOperator = result => ApplyTyped(l.TypedOperator(result),  rc);
                    AggregateLevel = l.AggregateLevel;
                    break;
                case NP.rigthNull:
                    Operator = result => @operator(lc.Content, r.Operator(result));
                    TypedOperator = result => ApplyTyped(lc,  r.TypedOperator(result));
                    AggregateLevel = r.AggregateLevel;
                    break;
                case NP.bothNotNull:
                    Const = ApplyTyped(lc, rc);
                    break;
            }
        }

        protected virtual ObjectVariants ApplyTyped(ObjectVariants left, ObjectVariants right)
        {
            if (left.Variant == ObjectVariantEnum.Other && right.Variant == ObjectVariantEnum.Other)
            {
                //double dl,dr;
                int il, ir;
                long ll, lr;
                if (int.TryParse((string) left.Content, out il) && int.TryParse((string) right.Content, out ir))
                    return new OV_typed(@operator(il, ir).ToString(), ((OV_typed) left).turi);
                if (long.TryParse((string) left.Content, out ll) && long.TryParse((string) left.Content, out lr))
                    return new OV_typed(@operator(ll, lr).ToString(), ((OV_typed) left).turi);
                else
                    return
                        new OV_typed(
                            @operator(Convert.ToDouble(left.Content), Convert.ToDouble(right.Content)).ToString(),
                            ((OV_typed) left).turi);

            }
            else if (left.Variant == ObjectVariantEnum.Other || right.Variant == ObjectVariantEnum.Other)
            {
                OV_typed t;
                ObjectVariants second;
                if ((left as OV_typed) != null)
                {
                    t = left as OV_typed;
                    second = right;
                }
                else
                {
                    t = right as OV_typed;
                    second = left;
                }
                //double d;
                int i;
                long l;
                if (int.TryParse((string) t.Content, out i) && second.Content is int)
                    return
                        new OV_typed(
                            @operator(Convert.ToInt32(left.Content), Convert.ToInt32(right.Content)).ToString(), t.turi);
                if (long.TryParse((string) t.Content, out l) && second.Content is long)
                    return
                        new OV_typed(
                            @operator(Convert.ToInt64(left.Content), Convert.ToInt64(right.Content)).ToString(), t.turi);
                else
                    return
                        new OV_typed(
                            @operator(Convert.ToDouble(left.Content), Convert.ToDouble(right.Content)).ToString(),
                            t.turi);
            }
            else
            {
                if (left is INumLiteral && right is INumLiteral)
                {
                    if (left.Variant == ObjectVariantEnum.Double || right.Variant == ObjectVariantEnum.Double)
                    {
                        return new OV_double(@operator(Convert.ToDouble(left.Content), Convert.ToDouble(right.Content)));
                    }
                    else if (left.Variant == ObjectVariantEnum.Decimal || right.Variant == ObjectVariantEnum.Decimal)
                    {
                        return
                            new OV_decimal(@operator(Convert.ToDecimal(left.Content), Convert.ToDecimal(right.Content)));
                    }
                    else if (left.Variant == ObjectVariantEnum.Float || right.Variant == ObjectVariantEnum.Float)
                    {
                        return new OV_float(@operator(Convert.ToSingle(left.Content), Convert.ToSingle(right.Content)));
                    }
                    else if (left.Variant == ObjectVariantEnum.Int || right.Variant == ObjectVariantEnum.Int)
                    {
                        return new OV_integer(@operator(left.Content, right.Content));
                    }
                    //todo
                }
            }
            throw new NotImplementedException();
        }
                                                                                                 
    }

    class SparqlDivideExpression: SparqlBinaryExpression
    {
        public SparqlDivideExpression(SparqlExpression l, SparqlExpression r) : base(l, r, (o, o1) =>
        {
            if (o is int && o1 is int)
                return Convert.ToDecimal(o)/o1;
            return o/o1;
        })
        {
        }

        protected override ObjectVariants ApplyTyped(ObjectVariants left, ObjectVariants right)
        {
            if(left.Variant==ObjectVariantEnum.Int && right.Variant==ObjectVariantEnum.Int)
                return new OV_decimal((Convert.ToDecimal(left.Content)) / (int)right.Content);
            return base.ApplyTyped(left, right);
        }
    }

    public class SparqlBinaryExpression<T> : SparqlExpression
    {
        public SparqlBinaryExpression(SparqlExpression l, SparqlExpression r, Func<dynamic, dynamic, dynamic> @operator, Func<dynamic, T> typedCtor)
        {
            var lc = l.Const;
            var rc = r.Const;

            switch (NullablePairExt.Get(lc, rc))
            {
                case NP.bothNull:
                    Operator = result => @operator(l.Operator(result), r.Operator(result));
                    AggregateLevel = SetAggregateLevel(l.AggregateLevel, r.AggregateLevel);
                    break;
                case NP.leftNull:
                    Operator = result => @operator(l.Operator(result), rc.Content);
                    AggregateLevel = l.AggregateLevel;
                    break;
                case NP.rigthNull:
                    Operator = result => @operator(lc.Content, r.Operator(result));
                    AggregateLevel = r.AggregateLevel;
                    break;
                case NP.bothNotNull:
                    Const = typedCtor(@operator(lc.Content, rc.Content));
                    break;
            }
            TypedOperator = res => typedCtor(Operator(res));
        }


    }
}