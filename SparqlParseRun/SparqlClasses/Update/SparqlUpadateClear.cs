using System;
using RDFCommon;

namespace SparqlParseRun.SparqlClasses.Update
{
    public class SparqlUpdateClear : SparqlUpdateSilent 
    {
       public UpdateGraph   Graph;

        public override void RunUnSilent(IStore store)
        {
            switch (Graph.SparqlGrpahRefTypeEnum)
            {
                case SparqlGrpahRefTypeEnum.Setted:
                    store.NamedGraphs.Clear(Graph.Name);
                    break;
                case SparqlGrpahRefTypeEnum.Default:
                    store.Clear();
                    break;
                case SparqlGrpahRefTypeEnum.Named:
                    store.NamedGraphs.ClearAllNamedGraphs();
                    break;
                case SparqlGrpahRefTypeEnum.All:
                    store.ClearAll();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}
