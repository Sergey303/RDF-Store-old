using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlLangMatches : SparqlExpression
    {
        public SparqlLangMatches(SparqlExpression value, SparqlExpression langExpression)
        {
            //     value.SetExprType(ObjectVariantEnum.Str); //todo lang
          //  langExpression.SetExprType(ObjectVariantEnum.Str); //todo lang
            switch (NullablePairExt.Get(value.Const, langExpression.Const))
            {
                case NP.bothNull:  
                    Operator = result =>
            {
                var lang = value.Operator(result);
                var langRange = langExpression.Operator(result);
                return Equals(langRange.Content, "*")
                    ? !string.IsNullOrWhiteSpace(langRange.Content)
                    : Equals(lang, langRange);
            };

                    AggregateLevel = SetAggregateLevel(value.AggregateLevel, langExpression.AggregateLevel);
                    break;
                case NP.leftNull:
                    var rlang = langExpression.Const.Content;
                    Operator = result =>
                    {
                        var lang = value.Operator(result);
                        return Equals(rlang, "*")
                            ? !string.IsNullOrWhiteSpace(lang)
                            : Equals(lang, rlang);
                    };
                    AggregateLevel = value.AggregateLevel;
                    break;
                case NP.rigthNull:
                    var llang = value.Const.Content;
                    if ((llang).Equals("*"))
                    Operator = result => !string.IsNullOrWhiteSpace(langExpression.Operator(result));
                    else 
                    Operator = result => Equals(llang, langExpression.Operator(result));
                    AggregateLevel = langExpression.AggregateLevel;
                    break;
                case NP.bothNotNull:
                    var ll = value.Const.Content;
                    var rl = langExpression.Const.Content;
                    Const = new OV_bool(Equals(rl, "*")
                            ? !string.IsNullOrWhiteSpace((string)ll)
                            : Equals(ll, rl));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            TypedOperator = result =>new OV_bool(Operator(result));
        }
    }
}