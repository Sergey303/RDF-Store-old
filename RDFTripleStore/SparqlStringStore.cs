using System;
using System.IO;
using RDFCommon;
using SparqlParseRun;
using SparqlParseRun.SparqlClasses.Query.Result;
using RDFTripleStore;
using Antlr4.Runtime;

namespace TestingNs
{
    public class SecondStringStore : SecondStringGraph , IStore //CacheMeasure  GraphCached   InterpretMeasure
    {

        public SecondStringStore(string path)
                   //        : base(new SecondStringGraph(path)) 
         :   base(path)
        {
        }

        public void ReloadFrom(string fileName)
        {
          //  ClearAll();
            FromTurtle(fileName);    
        }

     

        public void ReloadFrom(Stream baseStream)
        {
          //  ClearAll();
          //base.FromTurtle(baseStream);    
        }

       
        private SparqlResultSet Run(SparqlQuery queryContext)
        {
             return queryContext.Run();
        }


        public IStoreNamedGraphs NamedGraphs { get; private set; }
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
