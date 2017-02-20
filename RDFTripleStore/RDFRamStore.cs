using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDFCommon;
using RDFCommon.OVns;

namespace RDFTripleStore
{
   public class RDFRamStore :RamListOfTriplesGraph, IStore
    {

        public IStoreNamedGraphs NamedGraphs { get; }
        public void ClearAll()
        {
            Clear();
        }

        public IGraph CreateTempGraph()
        {
            return new RamListOfTriplesGraph("temp");
        }

        public void ReloadFrom(string filePath)
        {
            throw new NotImplementedException();
        }
        
    }
}
