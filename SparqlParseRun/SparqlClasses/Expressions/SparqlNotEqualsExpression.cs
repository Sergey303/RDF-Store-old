using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    internal class SparqlNotEqualsExpression : SparqlExpression
    {
        public SparqlNotEqualsExpression(SparqlExpression l, SparqlExpression r, NodeGenerator ng)
        {

            var lc = l.Const;
            var rc = r.Const;
             
            switch (NullablePairExt.Get(lc, rc))
            {
                case NP.bothNull:
                    Operator = result => !l.TypedOperator(result).Equals(r.TypedOperator(result));
                    AggregateLevel = SetAggregateLevel(l.AggregateLevel, r.AggregateLevel);
                    break;
                case NP.leftNull:
                    if (rc.Variant == ObjectVariantEnum.Iri)
                    {
                        OV_iriint rcCoded = null;
                        Operator = result =>
                        {
                            var lVal = l.TypedOperator(result);
                            if (lVal.Variant == ObjectVariantEnum.Iri)
                                return !lVal.Equals(rc);
                            if (lVal.Variant == ObjectVariantEnum.IriInt)
                            {
                                if (rcCoded == null)
                                    rcCoded = (OV_iriint) ng.GetUri((string) rc.Content);
                                return ((OV_iriint) lVal).code != rcCoded.code;
                            }
                            else throw new AggregateException();
                        };
                    }
                    else
                        Operator = result => !l.TypedOperator(result).Equals(rc);

                    AggregateLevel = l.AggregateLevel;
                    break;
                case NP.rigthNull:
                    if (lc.Variant == ObjectVariantEnum.Iri)
                    {
                        OV_iriint lcCoded = null;
                        Operator = result =>
                        {
                            var rVal = r.TypedOperator(result);
                            if (rVal.Variant == ObjectVariantEnum.Iri)
                                return !rVal.Equals(lc);
                            if (rVal.Variant == ObjectVariantEnum.IriInt)
                            {
                                if (lcCoded == null)
                                    lcCoded = (OV_iriint)ng.GetUri((string)lc.Content);
                                return ((OV_iriint)rVal).code != lcCoded.code;
                            }
                            else throw new AggregateException();
                        };
                    }
                    else
                        Operator = result => !lc.Equals(r.TypedOperator(result));
                    AggregateLevel = r.AggregateLevel;
                    break;
                case NP.bothNotNull:
                    Const = new OV_bool(!lc.Equals(rc));
                    break;
            }
            TypedOperator = result => new OV_bool(Operator(result));

        }

    }
}