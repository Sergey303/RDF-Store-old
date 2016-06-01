using System;
using System.Web;
using RDFCommon;
using RDFCommon.OVns;


namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlEncodeForUri : SparqlExpression
    {
        public SparqlEncodeForUri(SparqlExpression value, RdfQuery11Translator q)   :base(value.AggregateLevel, value.IsStoreUsed)
        {
            //SetExprType(ObjectVariantEnum.Str);
          //  value.SetExprType(ExpressionTypeEnum.stringOrWithLang);
            if (value.Const != null)
                Const = new OV_string(HttpUtility.UrlEncode((string) value.Const.Content));
            else
            {
                Operator = result => HttpUtility.UrlEncode((string) value.Operator(result).Content);
                TypedOperator = result => new OV_string(Operator(result));
            }
        }
    }
}
