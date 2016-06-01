using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlQuery.SparqlClasses.GraphPattern.Triples.Node;
using SparqlQuery.SparqlClasses.Update;

namespace SparqlQuery.SparqlClasses.Query.Result
{
    public class SparqlResultSet 
    {
        private readonly RdfQuery11Translator q;
        public IEnumerable<SparqlResult> Results ;
        public IGraph GraphResult;
        public ResultType ResultType;
        public SparqlQuery Query;
      

        public bool AnyResult
        {
            get { return Results.Any(); }
        }

        public string UpdateMessage;

        public SparqlUpdateStatus UpdateStatus;

        internal Dictionary<string, VariableNode> Variables = new Dictionary<string, VariableNode>();
        private readonly Prologue prologue;

        public SparqlResultSet(SparqlQuery query, Prologue prologue=null)
        {
            Query = query;
            this.prologue = prologue;
        }

        public XElement ToXml()
        {
            XNamespace xn = "http://www.w3.org/2005/sparql-results#";
            switch (ResultType)
            {
                case ResultType.Select:
                    return new XElement(xn + "sparql", new XAttribute(XNamespace.Xmlns + "ns", xn),
                        new XElement(xn + "head",
                            Variables.Select(v => new XElement(xn + "variable", new XAttribute(xn + "name", v.Key)))),
                        new XElement(xn + "results",
                            Results.Select(result =>
                                new XElement(xn + "result",
                                    result.GetSelected((var, value)=> 
                                        new XElement(xn + "binding",    
                                            new XAttribute(xn + "name", var.VariableName),
                                            BindingToXml(xn, value)))))));
                case ResultType.Describe:
                case ResultType.Construct:
                    return GraphResult.ToXml(prologue);
                case ResultType.Ask:
                    return new XElement(xn + "sparql", //new XAttribute(XNamespace.Xmlns , xn),
                        new XElement(xn + "head",
                            Variables.Select(v => new XElement(xn + "variable", new XAttribute(xn + "name", v.Key)))),
                        new XElement(xn + "boolean", AnyResult));
                case ResultType.Update:
                    return new XElement("update", new XAttribute("status", UpdateStatus.ToString()));
                default:                                       
                    throw new ArgumentOutOfRangeException();
            }
        }

        //public JsonConvert ToJson()

        private XElement BindingToXml(XNamespace xn, ObjectVariants b)
        {
            if (b is IIriNode)
            {
                return new XElement(xn + "uri", ((IIriNode)b).UriString);
            }     
            else if (b is IBlankNode)
            {
                return new XElement(xn + "bnode", ((IBlankNode)b).Name);
            }
            else if (b is ILiteralNode)
            {
                var literalNode = ((ILiteralNode) b);
                if (literalNode is ILanguageLiteral)
                {
                    return new XElement(xn + "literal",
                        new XAttribute(xn + "lang", ((ILanguageLiteral) literalNode).Lang), literalNode.Content);
                }
                else if (literalNode is IStringLiteralNode)
                {
                    return new XElement(xn + "literal", literalNode.Content);
                }
                else //if (literalNode == LiteralType.TypedObject)
                {
                    return new XElement(xn + "literal", new XAttribute(xn + "type", (literalNode).DataType),
                        literalNode.Content);
                }
            }

            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public override string ToString()
        {
            switch (ResultType)
            {
                case ResultType.Describe:
                case ResultType.Construct:
                    return GraphResult.ToString();
                case ResultType.Select:
                    //  return Results.ag.ToString();
                case ResultType.Ask:
                    return AnyResult.ToString();

                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        public string ToJson()
        {
            string headVars;
            switch (ResultType)
            {
                case ResultType.Select:
                        headVars = string.Format(@"""head"": {{ ""vars"": [ {0} ] }}",
                            string.Join("," + Environment.NewLine,
                                Variables.Keys.Select(v => string.Format("\"{0}\"", v))));
                    return
                     string.Format(@"{{ {0}, ""results"": {{ ""bindings"" : [{1}] }} }}", headVars,
                         string.Join("," + Environment.NewLine, Results.Select(result => string.Format("{{{0}}}",
                             string.Join("," + Environment.NewLine,
                                 result.GetSelected((var, value) =>
                                     string.Format("\"{0}\" : {1}", var.VariableName,
                                         value.ToJson())))))) );
                case ResultType.Describe:
                case ResultType.Construct:
                    return GraphResult.ToJson();
                case ResultType.Ask:
                    headVars = string.Format(@"""head"": {{ ""vars"": [ {0} ] }}",
                        string.Join("," + Environment.NewLine,
                            Variables.Keys.Select(v => string.Format("\"{0}\"", v))));
                    return string.Format("{{ {0}, \"boolean\" : {1}}}", headVars, AnyResult);
                case ResultType.Update:
                    headVars = string.Format(@"""head"": {{ ""vars"": [ {0} ] }}",
                        string.Join("," + Environment.NewLine,
                            Variables.Keys.Select(v => string.Format("\"{0}\"", v))));
                    return string.Format("{{ {0}, \"status\" : \"{1}\"}}", headVars, UpdateStatus);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static string GetValue(VariableNode vari, ObjectVariants value)
        {
            return value.ToString();
        }
        public string ToCommaSeparatedValues()
        {
            string headVars;
            switch (ResultType)
            {
                case ResultType.Select:
                    headVars = string.Join(",", Variables.Keys);
                    return
                      headVars+Environment.NewLine+
                         string.Join(Environment.NewLine, 
                         Results.Select(result=>
                             string.Join(",",
                             result.GetSelected((var, value) => value))));
                case ResultType.Describe:
                case ResultType.Construct:
                    return GraphResult.ToTurtle();
                case ResultType.Ask:
                    return AnyResult.ToString();
                case ResultType.Update:
                    return UpdateStatus.ToString();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public string FromCommaSeparatedValues()
        {
         
            throw new NotImplementedException();
        }

        public static IEnumerable<SparqlResult> FromXml(XElement load, RdfQuery11Translator q)
        {
            XNamespace xn = "http://www.w3.org/2005/sparql-results#";

            return load
                .Element(xn + "results")
                .Elements()
                .Select(xResult => new SparqlResult(q).Add(xResult.Elements()
                    .Select(xb =>
                    {
                        var variable = q.GetVariable(xb.Attribute(xn + "name").Value);
                        var node = xb.Elements().FirstOrDefault();
                        return new KeyValuePair<VariableNode, ObjectVariants>(variable, Xml2Node(xn, node, q));
                    })));
        }

        private static ObjectVariants Xml2Node(XNamespace xn, XElement b, RdfQuery11Translator q)
        {
            if (b.Name == xn + "uri")
            {
                return new OV_iri(q.prolog.GetFromString(b.Value));
            }
            else if (b.Name == xn + "bnode")
            {
                return q.CreateBlankNode(b.Value);
            }
            else if (b.Name == xn + "literal")
            {
                var lang = b.Attribute(xn + "lang");
                var type = b.Attribute(xn + "type");
                if (lang != null)
                    return new OV_langstring(b.Value, lang.Value);
                else if (type != null)
                    return q.Store.NodeGenerator.CreateLiteralNode(b.Value, q.prolog.GetFromString(type.Value));
                else return new OV_string(b.Value);
            }
            throw new ArgumentOutOfRangeException();
        }
    
    }


    public enum ResultType
    {
        Describe, Select, Construct, Ask,
        Update
    }

   
}