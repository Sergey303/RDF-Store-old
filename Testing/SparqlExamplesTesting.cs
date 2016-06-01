using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFCommon;
using RDFTripleStore;
using SparqlQuery.SparqlClasses;
using SparqlQuery.SparqlClasses.Query.Result;

namespace TestingNs
{
    [TestClass]
    public class SparqlExamplesTesting
    {  
        [TestMethod]
        public void Examples()
        {
            DirectoryInfo examplesRoot = new DirectoryInfo(@"..\..\examples");
            foreach (var exampleDir in examplesRoot.GetDirectories().Skip(0))
            //  var exampleDir = new DirectoryInfo(@"..\..\examples\bsbm");
            {
                Console.WriteLine("example: " + exampleDir.Name);
                //if (exampleDir.Name != @"federated subquery"
                //    //&& rqQueryFile.FullName != @"C:\Users\Admin\Source\Repos\SparqlWpf\UnitTestDotnetrdf_test\examples\insert where\query2.rq"
                //  ) continue;
                //var nameGraphsDir = new DirectoryInfo(Path.Combine(exampleDir.FullName, "named graphs"));
                //if (nameGraphsDir.Exists) continue;
                foreach (var ttlDatabase in exampleDir.GetFiles("*.ttl"))
                {
                    var store = new Store(exampleDir.FullName + "/tmp");
                    store.ClearAll();
                    //using (StreamReader reader = new StreamReader(ttlDatabase.FullName))
                    store.ReloadFrom(ttlDatabase.FullName);
                    //  store.Start();
                    var nameGraphsDir = new DirectoryInfo(Path.Combine(exampleDir.FullName, "named graphs"));
                    if (nameGraphsDir.Exists)
                        foreach (var namedGraphFile in nameGraphsDir.GetFiles())
                        {
                            IGraph graph;
                            using (StreamReader reader = new StreamReader(namedGraphFile.FullName))
                            {
                                var readLine = reader.ReadLine();
                                if (readLine == null) continue;
                                var headComment = readLine.Trim();
                                if (!headComment.StartsWith("#")) continue;
                                headComment = headComment.Substring(1);
                                //Uri uri;
                                //if (!Uri.TryCreate(headComment, UriKind.Absolute, out uri)) continue;Prologue.SplitUri(uri.AbsoluteUri).FullName
                                graph = store.NamedGraphs.CreateGraph(headComment);

                            }
                            graph.FromTurtle(namedGraphFile.FullName);
                        }

                    foreach (var rqQueryFile in exampleDir.GetFiles("*.rq"))
                    {

                        Console.WriteLine("query file: " + rqQueryFile);
                        var outputFile = rqQueryFile.FullName + "expected results.xml";
                        SparqlResultSet sparqlResultSet = null;
                        //  try
                        var query = rqQueryFile.OpenText().ReadToEnd();

                        SparqlQuery.SparqlClasses.Query.SparqlQuery sparqlQuery = null;
                        {
                            //Perfomance.ComputeTime(() =>
                            {
                                sparqlQuery = SparqlQueryParser.Parse(store, query);
                            } //, exampleDir.Name+" "+rqQueryFile.Name+" parse ", true);

                            if (sparqlQuery != null)
                            // Perfomance.ComputeTime(() =>
                            {
                                sparqlResultSet = sparqlQuery.Run();
                                File.WriteAllText(outputFile, sparqlResultSet.ToXml().ToString());
                            } //, exampleDir.Name + " " + rqQueryFile.Name + " run ", true);
                                //var exprectedResults= SparqlResultSet.FromXml(XElement.Load(), )
                            Assert.AreEqual(true, true, rqQueryFile.Name);
                            //File.ReadAllText(rqQueryFile.FullName + " expected results.txt"),
                            //      File.ReadAllText(outputFile));
                        }
                        //  catch (Exception e)
                        {
                            // Assert.(e.Message);
                        }
                    }
                }
            }
        }
    }
}
