using RDFCommon;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlLang  : SparqlExpression
    {
        public SparqlLang(SparqlExpression value)  :base(value.AggregateLevel, value.IsStoreUsed)
        {
            if (value.Const != null)
                Const = new OV_string(((ILanguageLiteral) value.Const).Lang.Substring(1));
            else
            {
                TypedOperator = result => new OV_language(Operator(result));
                Operator= result => 
                {
                    var f = value.TypedOperator(result);
                        return new OV_string(((ILanguageLiteral) f).Lang.Substring(1));
                };
            }
        }
    }
}
