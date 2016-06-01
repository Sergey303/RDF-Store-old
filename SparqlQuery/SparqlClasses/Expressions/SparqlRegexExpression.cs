using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RDFCommon;
using RDFCommon.OVns;
using SparqlQuery.SparqlClasses.GraphPattern.Triples.Node;
using SparqlQuery.SparqlClasses.Query.Result;

namespace SparqlQuery.SparqlClasses.Expressions
{
    public class SparqlRegexExpression : SparqlExpression
    {
        private SparqlExpression variableExpression;
        private IStore store;
        internal void SetVariableExpression(SparqlExpression sparqlExpression, IStore store)
        {
            variableExpression = sparqlExpression;
            this.store = store;
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
                        string pattern = (string) patternExpression.TypedOperator(result).Content;
                            regex = CreateRegex(pattern);        
                        return regex.IsMatch((string) varConst.Content);
                    };
                    AggregateLevel = patternExpression.AggregateLevel;
                    break;
                case NP.rigthNull:
                    var content = (string)patternConst.Content;
                    regex = CreateRegex(content);
                    var varExpression = variableExpression as SparqlVarExpression;
                    if (varExpression != null && isSimple.IsMatch(content))
                    {
                        Operator = result =>
                        {
                            if(result.ContainsKey(varExpression.Variable))
                            return regex.IsMatch(varExpression.Operator(result));
                            return store.GetTriplesWithTextObject(varExpression.Operator(result))
                                    .Select(Selector(result, varExpression.Variable));
                            
                        };
                    }
                    else
                {
                    Operator = result => regex.IsMatch(variableExpression.Operator(result));
                }
                    AggregateLevel = variableExpression.AggregateLevel;
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

        private Func<TripleIntOV, SparqlResult> Selector(SparqlResult sparqlResult, VariableNode variable)
        {
            return t =>
            {
            sparqlResult[variable] = t.Object;
                return sparqlResult;
            };
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
        private static Regex isSimple=new Regex(@"[\w\n\d\s\t_\.\,\!""']*");
    }
}
