using System.Collections.Generic;

namespace SparqlParseRun.SparqlClasses
{
    public class SparqlValueDataBlock
    {
        private readonly ISparqlDataBlock sparqlDataBlock;

        public SparqlValueDataBlock(ISparqlDataBlock sparqlDataBlock)
        {
            // TODO: Complete member initialization
            this.sparqlDataBlock = sparqlDataBlock;
        }
        public virtual void SetVariables(SparqlResultSet startResults)
        {
            startResults.Results=new List<SparqlResult>(sparqlDataBlock.SetVariables(startResults.Results));
        }
    }
}
