using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using RDFCommon;
using RDFCommon.OVns;
using RDFTripleStore;
using SparqlParseRun.SparqlClasses;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace TestingNs
{
    class Class1
    {
        private static void RunAllBerlinByOne()
        {
            var Store = new StoreCascadingInt("../../../Databases/int based/");
            Performance.ComputeTime(() => Store.ReloadFrom(Config.Source_data_folder_path + "1M.ttl"), "load 1млн ", true);
            //  return;
            //  Store.ActivateCache();
            Store.Start();

            //for (int i = 0; i < 12; i++)
            //{
            //    SparqlTesting.OneBerlinParametrized(Store, i + 1, 100);
            //}
        }

        private static void RunAllLUBMByOne()
        {
            var Store = new StoreCascadingInt("../../../Databases/int based/");
            if (false)
            {
                Store.Clear();
                foreach (
                    var owlFile in
                        new DirectoryInfo(@"C:\Users\Admin\Source\Repos\RDF-Store-Polar-DB-Lab\Testing\examples\lubm")
                            .GetFiles("*.owl"))
                {
                    Store.AddFromXml(XElement.Load(owlFile.FullName));
                }
            }
            else
            {
                Console.WriteLine(Store.table.Elements().Count());
                //   Store.ActivateCache();
                Store.Start();
            }
            for (int i = 0; i < 14; i++)
            {
                SparqlTesting.OneLUMB(Store, i + 1, 100);
            }
        }

        private static void MainPersons(string[] args)
        {
            TestingPhotoPersons.Npersons = 40 * 1000;
            string path = "../../../Databases/int based/" + TestingPhotoPersons.Npersons / 1000 + "/";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            StoreCascadingInt store = new StoreCascadingInt(path);
            using (StreamWriter perfomance = new StreamWriter("../../Perfomance.txt"))
                perfomance.WriteLine(TestingPhotoPersons.Npersons);
            if (true)
                Performance.ComputeTime(() => Reload(store), "load " + TestingPhotoPersons.Npersons + " ", true);
            TestingPhotoPersons.Run((q) =>
            {
                var sparqlQuery = SparqlQueryParser.Parse(store, q).Run();
                if (sparqlQuery.ResultType == ResultType.Ask)
                { var b = sparqlQuery.AnyResult; }
                else
                    sparqlQuery.Results.Count();
            });
        }

        private static void Reload(StoreCascadingInt store)
        {
            store.Build(TestingPhotoPersons.data.Generate().SelectMany(ele =>
            {
                string id = ele.Name + ele.Attribute("id").Value;
                var seq = Enumerable.Repeat(
                    new TripleStrOV(id, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type",
                        new OV_iri(ele.Name.LocalName)), 1)
                    .Concat(ele.Elements().Select(subele =>
                    {
                        XAttribute ratt = subele.Attribute("ref");
                        TripleStrOV triple = null;
                        if (ratt != null)
                        {
                            string r = (subele.Name == "reflected" ? "person" : "photo_doc") +
                                       ratt.Value;
                            triple = new TripleStrOV(id, subele.Name.LocalName, new OV_iri(r));
                        }
                        else
                        {
                            string value = subele.Value; // Нужны языки и другие варианты!
                            bool possiblenumber = string.IsNullOrEmpty(value) ? false : true;
                            if (possiblenumber)
                            {
                                char c = value[0];
                                if (char.IsDigit(c) || c == '-')
                                {
                                }
                                else possiblenumber = false;
                            }
                            triple = new TripleStrOV(id, subele.Name.LocalName,
                                possiblenumber
                                    ? (ObjectVariants)new OV_int(int.Parse(value))
                                    : (ObjectVariants)new OV_string(value));
                        }
                        return triple;
                    }));
                return seq;
            }));
            //  File.WriteAllText("../../../Databases/string based/photoPersons.ttl", store.ToTurtle());
        }

        public static int Millions = 1;
    }
}

