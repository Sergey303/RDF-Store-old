using System.IO;
using RDFCommon;
using RDFCommon.OVns;

namespace RDFTripleStore
{
    public class Store : RDFGraph, RDFCommon.IStore //CacheMeasure  GraphCached   InterpretMeasure
    {

        public Store(string path)
            //        : base(new SecondStringGraph(path)) 
            : base(path)
        {

       
            NamedGraphs = new NamedGraphsByFolders(new DirectoryInfo(path), ng, d => new RDFGraph(d.FullName + "/") { NodeGenerator = NodeGenerator },
                d => { d.Delete(true); });
        }

        private readonly NodeGeneratorInt ng;

        public void ReloadFrom(long iri_Count, string fileName)
        {
           FromTurtle(iri_Count, fileName);
           // ActivateCache();
        }
        
      


        public void ReloadFrom(string filePath)
        {
            FromTurtle(1000*1000, filePath);

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

     
    }
}
