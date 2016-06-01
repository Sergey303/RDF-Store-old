using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.GraphPattern
{
    public class SparqlServicePattern : ISparqlGraphPattern
    {
        private bool isSilent;
        private ObjectVariants uri;
        private string sparqlGraphPattern;
        private string prolog;
        private RdfQuery11Translator q;

        public void IsSilent()
        {
            this.isSilent = true;
        }

        internal void Create( ObjectVariants iri, string graphPattern, string prolog, RdfQuery11Translator q)
        {
            this.q = q;
            this.prolog = prolog;
            this.sparqlGraphPattern = graphPattern;
            this.uri = (ObjectVariants) iri;
        }

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            //HashSet<VariableNode> vars = new HashSet<VariableNode>(q.Variables.Select(pair => pair.Value)); 
            var bindings = variableBindings as SparqlResult[] ?? variableBindings.Select(result => result.Clone()).ToArray();
            //foreach (var pair in bindings.SelectMany(variableBinding => variableBinding.row.Where(pair => !vars.Contains(pair.Key))))
            //    vars.Add(pair.Key);
            // string s = string.Format("VALUES ({0})", variableBindings.SelectMany(r=>r.row.Values).Select(binding => binding.));
            
            string query = string.Format("{0} SELECT * WHERE {1}   VALUES ({2}) {{ {3} }}", 
                prolog, sparqlGraphPattern, string.Join(" ", q.Variables.Values.Select(pv => pv.VariableName)),
                string.Join(Environment.NewLine, 
                        bindings.Select(variableBinding =>  "(" + string.Join(" ",
                            q.Variables.Values.Select(v => ToSparqlInput(variableBinding[v] ?? SparqlUnDefinedNode.Undef )))+ ")")));                   
           
                try
                {
                    return Download(bindings, query).ToArray();
                }
                catch (Exception)
                {
                    if (isSilent) return Enumerable.Empty<SparqlResult>();
                    throw;
                }
               //TODO blank nodes     graph names parallel
        }

        private string ToSparqlInput(ObjectVariants objectVariants)
        {             
            switch (objectVariants.Variant)
            {
                case ObjectVariantEnum.Iri:
                case ObjectVariantEnum.IriInt:

                    return "<" + ((IIriNode) objectVariants).UriString + ">";
                    
                case ObjectVariantEnum.Str:
                    return "\"" + objectVariants.Content +"\"";
                case ObjectVariantEnum.Lang:
                    return "\"" + objectVariants.Content + "\"@"+((OV_langstring)objectVariants).Lang;
                case ObjectVariantEnum.Decimal:
                case ObjectVariantEnum.Float:
                case ObjectVariantEnum.Int:
                case ObjectVariantEnum.DateTimeZone:
                case ObjectVariantEnum.DateTime:
                case ObjectVariantEnum.Date:
                case ObjectVariantEnum.Time:
                case ObjectVariantEnum.Other:
                case ObjectVariantEnum.OtherIntType:
                case ObjectVariantEnum.Double:
                    return "\"" + objectVariants.Content + "\"^^<"+((ILiteralNode)objectVariants).DataType+">" ;

                case ObjectVariantEnum.Undef:
                    return objectVariants.ToString();
                case ObjectVariantEnum.Index:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerable<SparqlResult> Download(IEnumerable<SparqlResult> variableBindings, string query)
        {
             var variableUri = uri as VariableNode;
            if (variableUri != null)
                return variableBindings
                    .Select(binding => binding[variableUri])
                    .Where(uriFromVar => (uriFromVar != null))
                    .SelectMany(uriFromVar => DownloadOne(query, uriFromVar.ToString()));

            else
                return DownloadOne(query, (string) uri.Content);
        }

        private IEnumerable<SparqlResult> DownloadOne(string query, string urlString)
        {
            using (WebClient wc = new WebClient())
            {
               // wc.Headers[HttpRequestHeader.ContentType] = "application/application/sparql-results+xml"; //"query="+ 
                string HtmlResult = wc.UploadString(urlString, query);

                var load = XElement.Parse(HtmlResult);
                foreach (var sparqlResult in SparqlResultSet.FromXml(load, q)) yield return sparqlResult;
            }
        }

       

        //private IEnumerable<SparqlResult> FromJson(string load)
        //{
        //    XNamespace xn = "http://www.w3.org/2005/sparql-results#";

        //    return load
        //        .Element(xn + "results")
        //        .Elements()
        //        .Select(xResult => new SparqlResult(xResult.Elements()
        //            .Select(xb =>
        //            {
        //                var variable = q.GetVariable(xb.Attribute(xn + "name").Value);
        //                var node = xb.Elements().FirstOrDefault();
        //                return new SparqlVariableBinding(variable,
        //                    Xml2Node(xn, node));
        //            })));
        //}

        //private ObjectVariants Json2Node(string b)
        //{
        //    dynamic decode = System.Web.Helpers.Json.Decode(b);
        //    if (decode.type == "uri")
        //        return q.Store.NodeGenerator.GetUri(decode.value);
        //        if(decode.type=="literal")
        //        {
        //            if (b.Contains("datatype"))
        //                try
        //                {
        //                    return q.Store.NodeGenerator.CreateLiteralNode(decode.value, decode.datatype);
        //                }
        //                catch (Exception)
        //                {
        //                }
        //            if(b.Contains(lang))
        //        }
        //    if (decode.type == "bnode")
        //        return q.Store.NodeGenerator.CreateBlankNode(decode.value);//
        //    else if (b is ILiteralNode)
        //    {
        //        var literalNode = ((ILiteralNode) b);
        //        if (literalNode is ILanguageLiteral)
        //        {
        //            return "{ \"type\" : \"literal\", \"value\" : \"" + literalNode.Content +
        //                   "\", \"xml:lang\": \"" + ((ILanguageLiteral) literalNode).Lang + "\" }";
        //        }
        //        else if (literalNode is IStringLiteralNode)
        //        {
        //            return "{ \"type\" : \"literal\", \"value\" : \"" + literalNode.Content + "\" }";
        //        }
        //        else
        //        {
        //            return "{ \"type\" : \"literal\", \"value\" : \"" + literalNode.Content +
        //                   "\", \"datatype\": \"" + literalNode.DataType + "\" }";
        //        }
        //    }
        //    else if (b is IBlankNode)
        //    {
        //        return "{ \"type\" : \"bnode\", \"value\" : \"" + b + "\" }";
        //    }
        //    else if (b == null)
        //        return "";
        //    else
        //    {
        //        throw new ArgumentOutOfRangeException();
        //    }
        //}

        public SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.Federated;} }
    }
}