using System.IO;
using RDFCommon;
using RDFCommon.OVns;
using SparqlQuery.SparqlClasses.Query.Result;

namespace RDFTripleStore
{
    public class StoreCascadingInt : GraphCascadingInt, IStore //CacheMeasure  GraphCached   InterpretMeasure
    {

        public StoreCascadingInt(string path)
            //        : base(new SecondStringGraph(path)) 
            : base(path)
        {

            NodeGenerator =
                ng = NodeGeneratorInt.Create(path, table.TableCell.IsEmpty);
            NamedGraphs = new NamedGraphsByFolders(new DirectoryInfo(path), ng, d => new GraphCascadingInt(d.FullName + "/") { NodeGenerator = NodeGenerator },
                d => { d.Delete(true); });
        }

        private readonly NodeGeneratorInt ng;

        public void ReloadFrom(string fileName)
        {
            FromTurtle(fileName);
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



    }
}
