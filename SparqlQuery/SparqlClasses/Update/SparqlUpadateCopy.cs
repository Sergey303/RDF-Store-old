using RDFCommon;

namespace SparqlQuery.SparqlClasses.Update
{
    public class SparqlUpdateCopy : SparqlUpdateSilent
    {
        SparqlUpdateClear clear = new SparqlUpdateClear(){Graph = new UpdateGraph(SparqlGrpahRefTypeEnum.Default)};
        SparqlUpdateAdd add = new SparqlUpdateAdd();

        public override void RunUnSilent(IStore store)
        {
            clear.RunUnSilent(store);
            add.RunUnSilent(store);
        }

        public string To{
            set
            {
                clear.Graph.Name = value;
                clear.Graph.SparqlGrpahRefTypeEnum = SparqlGrpahRefTypeEnum.Setted;
                add.To = value;
            }
        }

        public string From           
        {
            set
            {
                add.From = value;
            }
        }
    }
}
