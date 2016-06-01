using System;
using System.Text;
using System.Xml.Linq;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Update
{
    public class SparqlUpdateLoad : SparqlUpdateSilent
    {
        private ObjectVariants from;
        public string Graph;

        internal void SetIri(ObjectVariants sparqlUriNode)
        {
           from = sparqlUriNode;
        }

        internal void Into(string sparqlUriNode)
        {
            Graph = sparqlUriNode;
        }


        public override void RunUnSilent(IStore store)
        {
            using (LongWebClient wc = new LongWebClient() { })
            {
                //  wc.Headers[HttpRequestHeader.ContentType] = "application/sparql-query"; //"query="+ 
                var gData = wc.DownloadData((string) @from.Content);
                string gString = Encoding.UTF8.GetString(gData);
                var graph = (Graph != null)
                    ? store.NamedGraphs.CreateGraph(Graph)
                    : store;
                try
                {
                    var gXml = XElement.Parse(gString);
                    graph.AddFromXml(gXml);
                }
                catch (Exception)
                {
                    try
                    {
                        graph.FromTurtle(gString);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
