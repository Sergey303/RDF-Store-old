using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlRegexExpression : SparqlExpression
    {
        private SparqlExpression variableExpression;

     

        internal void SetVariableExpression(SparqlExpression sparqlExpression)
        {
            variableExpression = sparqlExpression;
        }

        internal void SetRegex(SparqlExpression patternExpression)
        {
            var varConst = variableExpression.Const;
            var patternConst = patternExpression.Const;
            Regex regex;
            switch (NullablePairExt.Get(patternConst, varConst))
            {
                case NP.bothNull:
                    break;
                case NP.leftNull:
                    Operator = result =>
                    {
                        var pattern = patternExpression.TypedOperator(result).Content;
                        regex = CreateRegex((string) pattern);        
                        return regex.IsMatch((string) varConst.Content);
                    };
                    AggregateLevel = patternExpression.AggregateLevel;
                    break;
                case NP.rigthNull:
                    regex = CreateRegex((string)patternConst.Content);
                    if (varConst != null)
                        Const = new OV_bool(regex.IsMatch((string) varConst.Content));
                    else
                    {
                        Operator = result => regex.IsMatch(variableExpression.Operator(result));
                        AggregateLevel = variableExpression.AggregateLevel;
                    }
                    break;
                case NP.bothNotNull:
                    regex = CreateRegex((string) patternConst.Content);
                    Const=new OV_bool(regex.IsMatch((string) variableExpression.Const.Content));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            TypedOperator = result => new OV_bool(Operator(result));
        }

        private Regex CreateRegex(string pattern)
        {
            Regex regex;

            if (parameters == null)
            {
                if (!Regexes.TryGetValue(pattern, out regex))
                {
                    Regexes.Add(pattern, regex = new Regex(pattern));

                }
            }
            else
            {
                var key = new KeyValuePair<string, RegexOptions>(pattern, ignorePatternWhitespace);
                if (!RegexesParameters.TryGetValue(key, out regex))
                    RegexesParameters.Add(key, regex = new Regex(pattern, ignorePatternWhitespace)); 
            }
            return regex;
        }

        private static readonly Dictionary<string, Regex> Regexes = new Dictionary<string, Regex>();
        private static readonly Dictionary<KeyValuePair<string, RegexOptions>, Regex> RegexesParameters = new Dictionary<KeyValuePair<string, RegexOptions>, Regex>();
        private dynamic parameters;
        private RegexOptions ignorePatternWhitespace;

        internal void SetParameters(SparqlExpression paramsExpression)
        {
            parameters = paramsExpression.Const.Content;    
            ignorePatternWhitespace = RegexOptions.None;
            if (parameters.Contains("s"))
                ignorePatternWhitespace |= RegexOptions.Singleline;
            if (parameters.Contains("m"))
                ignorePatternWhitespace |= RegexOptions.Multiline;
            if (parameters.Contains("i"))
                ignorePatternWhitespace |= RegexOptions.IgnoreCase;
            if (parameters.Contains("x"))
                ignorePatternWhitespace |= RegexOptions.IgnorePatternWhitespace;
        }

    }
}
