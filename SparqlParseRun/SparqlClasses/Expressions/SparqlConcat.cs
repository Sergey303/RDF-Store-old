using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlConcat : SparqlExpression
    {
        public SparqlConcat(List<SparqlExpression> list, NodeGenerator q)
        {
          
            //SetExprType(ExpressionTypeEnum.stringOrWithLang);
            //foreach (var sb in list)
            //    sb.SetExprType(ExpressionTypeEnum.stringOrWithLang);

            if (list.Count == 0)
            {
                Const = new OV_string(string.Empty);
                AggregateLevel=VariableDependenceGroupLevel.Const;
                //TypedOperator = r=>new OV_string(string.Empty);
            }
            else
            {
                if (list.All(expression => expression.Const != null))
                {
                    var values = list.Select(expression => expression.Const).ToArray();
                    if (values.All(o => o is OV_langstring))
                    {
                        var commonLang = ((OV_langstring) values[0]).Lang;
                        if (values.Cast<OV_langstring>().All(ls => ls.Lang.Equals(commonLang)))
                        {
                            Const= new OV_langstring(string.Concat(values.Select(o => o.Content)), commonLang);
                          AggregateLevel=VariableDependenceGroupLevel.Const;
                        }
                    }
                    else if (values.All(o => o is OV_string))
                        Const = new OV_string(string.Concat(values.Select(v => v.Content).Cast<string>()));
                }
                else
                {
                    Operator = result =>
                    {
                        var values = list.Select(expression => expression.Const ?? expression.TypedOperator(result)).ToArray();
                        if (values.All(o => o is OV_langstring))
                        {
                            var commonLang = ((OV_langstring)values[0]).Lang;
                            if (values.Cast<OV_langstring>().All(ls => ls.Lang.Equals(commonLang)))
                                return string.Concat(values.Select(o => o.Content));
                        }
                        else if (values.All(o => o is OV_string))
                            return string.Concat(values.Select(v => v.Content).Cast<string>());
                        throw new ArgumentException();
                        //return q.CreateLiteralNode(string.Concat(values.Select(s => s.Content)));
                    };
                  AggregateLevel = SetAggregateLevel(list.Select(v=>v.AggregateLevel).ToArray());
                TypedOperator = result =>
                {
                    var values = list.Select(expression => expression.Const ?? expression.TypedOperator(result)).ToArray();
                    if (values.All(o => o is OV_langstring))
                    {
                        var commonLang = ((OV_langstring) values[0]).Lang;
                        if (values.Cast<OV_langstring>().All(ls => ls.Lang.Equals(commonLang)))
                            return new OV_langstring(string.Concat(values.Select(o => o.Content)), commonLang);
                    }
                    else if (values.All(o => o is OV_string))
                        return new OV_string(string.Concat(values.Select(v => v.Content).Cast<string>()));
                    throw new ArgumentException();
                    //return q.CreateLiteralNode(string.Concat(values.Select(s => s.Content)));
                };}
            }

        }
    }
}
