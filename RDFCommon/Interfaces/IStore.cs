using System.Collections.Generic;

namespace RDFCommon
{
    public interface IStore : IGraph 
    {
        //IStoreNamedGraphs NamedGraphs { get; }
        IStoreNamedGraphs NamedGraphs { get; }


        void ClearAll();
        IGraph CreateTempGraph();
        void ReloadFrom(string filePath);
    }
}