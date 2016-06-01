using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Update
{
    public class SparqlUpdateMove : SparqlUpdateSilent
    {
        public string To;
        public string From;     
        public override void RunUnSilent(IStore store)
        {
            if ((From == null && To == null) || (From != null && From.Equals(To))) return;
            IGraph fromGraph;
            if (From == null) fromGraph = store;
            else
            {
                fromGraph = store.NamedGraphs.GetGraph(From);
                if (!fromGraph.Any()) throw new Exception(From);
            }
            if (To == null)
            {
                fromGraph.GetTriples((s, p, o) =>
                {
                    store.Add(s, p, o);
                    return true;
                });
                    
                fromGraph.Clear();
            }
            else //if (!store.NamedGraphs.ContainsGraph(To)) 
                store.NamedGraphs.AddGraph(To, fromGraph);
            //else store.NamedGraphs.ReplaceGraph(To,fromGraph);
        }
    }
}
