using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace RDFTripleStore
{
    public class FirstIntStore : FirstIntGraph, IStore
    {
        public FirstIntStore(string path) : base(path)// base(new FirstIntGraph(path))
        {
        }

        public void ReloadFrom(string fileName)
        {
            ClearAll();
            FromTurtle(fileName);    
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
