using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Update
{
    public class UpdateGraph
    {
        public SparqlGrpahRefTypeEnum SparqlGrpahRefTypeEnum;
        public string Name;
                                                   
        public UpdateGraph(string uriNode)
        {
            // TODO: Complete member initialization
            this.Name = uriNode;
            SparqlGrpahRefTypeEnum = SparqlGrpahRefTypeEnum.Setted;
        }

        public UpdateGraph(SparqlGrpahRefTypeEnum sparqlGrpahRefTypeEnum1)
        {
            // TODO: Complete member initialization
            this.SparqlGrpahRefTypeEnum = sparqlGrpahRefTypeEnum1;
        }

      

       
    }
}