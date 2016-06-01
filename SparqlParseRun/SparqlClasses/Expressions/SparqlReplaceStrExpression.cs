using System;
using System.Text.RegularExpressions;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlReplaceStrExpression : SparqlExpression
    {
        private SparqlExpression stringExpression;
        private SparqlExpression parametersExpression;
        private SparqlExpression replacementExpression;
        private SparqlExpression patternExpression;

        internal void SetString(SparqlExpression stringExpression)
        {
            this.stringExpression = stringExpression;
        }

        internal void SetPattern(SparqlExpression patternExpression)
        {
            this.patternExpression = patternExpression;
        }

        internal void SetReplacement(SparqlExpression replacementExpression)
        {
            this.replacementExpression = replacementExpression;

        }

        internal void SetParameters(SparqlExpression parametersExpression)
        {
            this.parametersExpression = parametersExpression;

        }

        public void Create()
        {
            if (parametersExpression == null)
            {
                switch (
                    NullableTripleExt.Get(stringExpression.Const, patternExpression.Const, replacementExpression.Const))
                {
                    case NT.AllNull:
                        Operator = res =>
                            ((string) stringExpression.Operator(res)).Replace(
                                (string) patternExpression.Operator(res),
                                (string) replacementExpression.Operator(res));
                        TypedOperator =
                            res => stringExpression.TypedOperator(res)
                                .Change(o => ((string) o).Replace((string) patternExpression.Operator(res),
                                    (string) replacementExpression.Operator(res)));
                        break;
                    case NT.FirstNotNull:
                        Operator = res =>
                            ((string) stringExpression.Const.Content).Replace(
                                (string) patternExpression.Operator(res),
                                (string) replacementExpression.Operator(res));
                        TypedOperator =
                            res => stringExpression.Const
                                .Change(o => ((string) o).Replace((string) patternExpression.Operator(res),
                                    (string) replacementExpression.Operator(res)));
                        break;
                    case NT.SecondNotNull:
                        Operator = res =>
                            ((string) stringExpression.Operator(res)).Replace(
                                (string) patternExpression.Const.Content,
                                (string) replacementExpression.Operator(res));
                        TypedOperator =
                            res => stringExpression.TypedOperator(res)
                                .Change(o => ((string) o).Replace((string) patternExpression.Const.Content,
                                    (string) replacementExpression.Operator(res)));
                        break;
                    case NT.ThirdNotNull:
                        Operator = res =>
                            ((string) stringExpression.Operator(res)).Replace(
                                (string) patternExpression.Operator(res),
                                (string) replacementExpression.Const.Content);
                        TypedOperator =
                            res => stringExpression.TypedOperator(res)
                                .Change(o => ((string) o).Replace((string) patternExpression.Operator(res),
                                    (string) replacementExpression.Const.Content));
                        break;
                    case ~NT.FirstNotNull:
                        Operator = res =>
                            ((string) stringExpression.Operator(res)).Replace(
                                (string) patternExpression.Const.Content,
                                (string) replacementExpression.Const.Content);
                        TypedOperator =
                            res => stringExpression.TypedOperator(res)
                                .Change(o => ((string) o).Replace((string) patternExpression.Const.Content,
                                    (string) replacementExpression.Const.Content));
                        break;
                    case ~NT.SecondNotNull:
                        Operator = res =>
                            ((string) stringExpression.Const.Content).Replace(
                                (string) patternExpression.Operator(res),
                                (string) replacementExpression.Const.Content);
                        TypedOperator =
                            res => stringExpression.Const
                                .Change(o => ((string) o).Replace((string) patternExpression.Operator(res),
                                    (string) replacementExpression.Const.Content));
                        break;
                    case ~NT.ThirdNotNull:
                        Operator = res =>
                            ((string) stringExpression.Const.Content).Replace(
                                (string) patternExpression.Const.Content,
                                (string) replacementExpression.Operator(res));
                        TypedOperator =
                            res => stringExpression.Const
                                .Change(o => ((string) o).Replace((string) patternExpression.Const.Content,
                                    (string) replacementExpression.Operator(res)));
                        break;
                    case ~NT.AllNull:
                        Const = stringExpression.Const
                            .Change(o => ((string) o).Replace((string) patternExpression.Const.Content,
                                (string) replacementExpression.Const.Content));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                if (parametersExpression.Const != null)
                {
                    var flags = GetRegexOptions((string) parametersExpression.Const.Content);
                    switch (
                        NullableTripleExt.Get(stringExpression.Const, patternExpression.Const,
                            replacementExpression.Const))
                    {
                        case NT.AllNull:
                            Operator = res => Regex.Replace(
                                (string) stringExpression.Operator(res),
                                (string) patternExpression.Operator(res),
                                (string) replacementExpression.Operator(res),
                                flags);
                            TypedOperator =
                                res => stringExpression.TypedOperator(res)
                                    .Change(o => Regex.Replace(((string) o), (string) patternExpression.Operator(res),
                                        (string) replacementExpression.Operator(res),
                                        flags));
                            break;
                        case NT.FirstNotNull:
                            Operator = res => Regex.Replace(
                                ((string) stringExpression.Const.Content),
                                (string) patternExpression.Operator(res),
                                (string) replacementExpression.Operator(res),
                                flags);
                            TypedOperator =
                                res => stringExpression.Const
                                    .Change(o => Regex.Replace(((string) o), (string) patternExpression.Operator(res),
                                        (string) replacementExpression.Operator(res),
                                        flags));
                            break;
                        case NT.SecondNotNull:
                            Operator = res => Regex.Replace(
                                ((string) stringExpression.Operator(res)),
                                (string) patternExpression.Const.Content,
                                (string) replacementExpression.Operator(res),
                                flags);
                            TypedOperator =
                                res => stringExpression.TypedOperator(res)
                                    .Change(o => Regex.Replace(((string) o), (string) patternExpression.Const.Content,
                                        (string) replacementExpression.Operator(res),
                                        flags));
                            break;
                        case NT.ThirdNotNull:
                            Operator = res => Regex.Replace(
                                ((string) stringExpression.Operator(res)),
                                (string) patternExpression.Operator(res),
                                (string) replacementExpression.Const.Content);
                            TypedOperator =
                                res => stringExpression.TypedOperator(res)
                                    .Change(o => Regex.Replace(((string) o), (string) patternExpression.Operator(res),
                                        (string) replacementExpression.Const.Content,
                                        flags));
                            break;
                        case ~NT.FirstNotNull:
                            Operator = res => Regex.Replace(
                                ((string) stringExpression.Operator(res)),
                                (string) patternExpression.Const.Content,
                                (string) replacementExpression.Const.Content,
                                flags);
                            TypedOperator =
                                res => stringExpression.TypedOperator(res)
                                    .Change(o => Regex.Replace(((string) o), (string) patternExpression.Const.Content,
                                        (string) replacementExpression.Const.Content,
                                        flags));
                            break;
                        case ~NT.SecondNotNull:
                            Operator = res => Regex.Replace(
                                ((string) stringExpression.Const.Content),
                                (string) patternExpression.Operator(res),
                                (string) replacementExpression.Const.Content);
                            TypedOperator =
                                res => stringExpression.Const
                                    .Change(o => Regex.Replace(((string) o), (string) patternExpression.Operator(res),
                                        (string) replacementExpression.Const.Content,
                                        flags));
                            break;
                        case ~NT.ThirdNotNull:
                            Operator = res => Regex.Replace(
                                ((string) stringExpression.Const.Content),
                                (string) patternExpression.Const.Content,
                                (string) replacementExpression.Operator(res),
                                flags);
                            TypedOperator =
                                res => stringExpression.Const
                                    .Change(o => Regex.Replace(((string) o), (string) patternExpression.Const.Content,
                                        (string) replacementExpression.Operator(res),
                                        flags));
                            break;
                        case ~NT.AllNull:
                            Const = stringExpression.Const
                                .Change(o => Regex.Replace(((string) o), (string) patternExpression.Const.Content,
                                    (string) replacementExpression.Const.Content,
                                    flags));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                }
                else
                {
                    switch (NullableTripleExt.Get(stringExpression.Const, patternExpression.Const,
                            replacementExpression.Const))
                    {
                        case NT.AllNull:
                            Operator = res => Regex.Replace(
                                (string)stringExpression.Operator(res),
                                (string)patternExpression.Operator(res),
                                (string)replacementExpression.Operator(res),
                                patternExpression.Operator(res));
                            TypedOperator =
                                res => stringExpression.TypedOperator(res)
                                    .Change(o => Regex.Replace(((string)o), (string)patternExpression.Operator(res),
                                        (string)replacementExpression.Operator(res),
                                        patternExpression.Operator(res)));
                            break;
                        case NT.FirstNotNull:
                            Operator = res => Regex.Replace(
                                ((string)stringExpression.Const.Content),
                                (string)patternExpression.Operator(res),
                                (string)replacementExpression.Operator(res),
                                patternExpression.Operator(res));
                            TypedOperator =
                                res => stringExpression.Const
                                    .Change(o => Regex.Replace(((string)o), (string)patternExpression.Operator(res),
                                        (string)replacementExpression.Operator(res),
                                        patternExpression.Operator(res)));
                            break;
                        case NT.SecondNotNull:
                            Operator = res => Regex.Replace(
                                ((string)stringExpression.Operator(res)),
                                (string)patternExpression.Const.Content,
                                (string)replacementExpression.Operator(res),
                                patternExpression.Operator(res));
                            TypedOperator =
                                res => stringExpression.TypedOperator(res)
                                    .Change(o => Regex.Replace(((string)o), (string)patternExpression.Const.Content,
                                        (string)replacementExpression.Operator(res),
                                        patternExpression.Operator(res)));
                            break;
                        case NT.ThirdNotNull:
                            Operator = res => Regex.Replace(
                                ((string)stringExpression.Operator(res)),
                                (string)patternExpression.Operator(res),
                                (string)replacementExpression.Const.Content);
                            TypedOperator =
                                res => stringExpression.TypedOperator(res)
                                    .Change(o => Regex.Replace(((string)o), (string)patternExpression.Operator(res),
                                        (string)replacementExpression.Const.Content,
                                        patternExpression.Operator(res)));
                            break;
                        case ~NT.FirstNotNull:
                            Operator = res => Regex.Replace(
                                ((string)stringExpression.Operator(res)),
                                (string)patternExpression.Const.Content,
                                (string)replacementExpression.Const.Content,
                                patternExpression.Operator(res));
                            TypedOperator =
                                res => stringExpression.TypedOperator(res)
                                    .Change(o => Regex.Replace(((string)o), (string)patternExpression.Const.Content,
                                        (string)replacementExpression.Const.Content,
                                        patternExpression.Operator(res)));
                            break;
                        case ~NT.SecondNotNull:
                            Operator = res => Regex.Replace(
                                ((string)stringExpression.Const.Content),
                                (string)patternExpression.Operator(res),
                                (string)replacementExpression.Const.Content);
                            TypedOperator =
                                res => stringExpression.Const
                                    .Change(o => Regex.Replace(((string)o), (string)patternExpression.Operator(res),
                                        (string)replacementExpression.Const.Content,
                                        patternExpression.Operator(res)));
                            break;
                        case ~NT.ThirdNotNull:
                            Operator = res => Regex.Replace(
                                ((string)stringExpression.Const.Content),
                                (string)patternExpression.Const.Content,
                                (string)replacementExpression.Operator(res),
                                patternExpression.Operator(res));
                            TypedOperator =
                                res => stringExpression.Const
                                    .Change(o => Regex.Replace(((string)o), (string)patternExpression.Const.Content,
                                        (string)replacementExpression.Operator(res),
                                        patternExpression.Operator(res)));
                            break;
                        case ~NT.AllNull:
                            Operator = res => 
                                Regex.Replace((string)stringExpression.Const.Content, (string)patternExpression.Const.Content,
                                    (string)replacementExpression.Const.Content,
                                    patternExpression.Operator(res));
                            TypedOperator =  res=> stringExpression.Const
                                .Change(o => Regex.Replace(((string)o), (string)patternExpression.Const.Content,
                                    (string)replacementExpression.Const.Content,
                                    patternExpression.Operator(res)));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private RegexOptions GetRegexOptions(string flags)
        {
               var  ro = RegexOptions.None;
            if (flags.Contains("s"))
                ro |= RegexOptions.Singleline;
            if (flags.Contains("m"))
                ro |= RegexOptions.Multiline;
            if (flags.Contains("i"))
                ro |= RegexOptions.IgnoreCase;
            if (flags.Contains("x"))
                ro |= RegexOptions.IgnorePatternWhitespace;
            return ro;
        }
        
    }
}
