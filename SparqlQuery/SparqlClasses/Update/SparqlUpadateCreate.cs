using RDFCommon;

namespace SparqlQuery.SparqlClasses.Update
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
