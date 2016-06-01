using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Update
{
    public class SparqlUpdateCreate : SparqlUpdateSilent
    {
        public string Graph;
        public override void RunUnSilent(IStore store)
        {
            store.NamedGraphs.CreateGraph(Graph);          
        }
    }
}
