using System.IO;
using RDFCommon;
using RDFCommon.OVns;
using SparqlQuery.SparqlClasses.Query.Result;

namespace RDFTripleStore
{
    public class Store : RDFGraph, IStore //CacheMeasure  GraphCached   InterpretMeasure
    {

        public Store(string path)
            //        : base(new SecondStringGraph(path)) 
            : base(path)
        {

            NodeGenerator =
                ng = NodeGeneratorInt.Create(path);
            NamedGraphs = new NamedGraphsByFolders(new DirectoryInfo(path), ng, d => new RDFGraph(d.FullName + "/") { NodeGenerator = NodeGenerator },
                d => { d.Delete(true); });
        }

        private readonly NodeGeneratorInt ng;

        public void ReloadFrom(long iri_Count, string fileName)
        {
            FromTurtle(iri_Count, fileName);
           // ActivateCache();
        }





        private SparqlResultSet Run(SparqlQuery.SparqlClasses.Query.SparqlQuery queryContext)
        {
            return queryContext.Run();
        }


        public IStoreNamedGraphs NamedGraphs { get; set; }
        public void ClearAll()
        {
            base.Clear();
        }

        public IGraph CreateTempGraph()
        {
            return new RamListOfTriplesGraph("temp");
        }

        public void ReloadFrom(string filePath)
        {
            throw new System.NotImplementedException();
        }
    }
}
