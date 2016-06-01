using System.Collections.Generic;
using System.Linq;
using SparqlParseRun.SparqlClasses.Query.Result;
using SparqlParseRun.SparqlClasses.SolutionModifier;

namespace SparqlParseRun.SparqlClasses.GraphPattern
{
    public class SparqlSubSelect : SparqlQuery, ISparqlGraphPattern
    {   

        public SparqlSubSelect(ISparqlGraphPattern sparqlWhere, SparqlSolutionModifier sparqlSolutionModifier, ISparqlGraphPattern sparqlValueDataBlock, RdfQuery11Translator q) 
            :base(q)
        {
            // TODO: Complete member initialization
           this. sparqlWhere= sparqlWhere;
            this.sparqlSolutionModifier = sparqlSolutionModifier;
            this.valueDataBlock = sparqlValueDataBlock;
            //   this.sparqlValueDataBlock = sparqlValueDataBlock;
        
        }

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            foreach (var sparqlResult in variableBindings)
            {
                Seed = Enumerable.Repeat<SparqlResult>(sparqlResult,1);
                var sparqlResultSet = base.Run();
                foreach (var result in sparqlResultSet.Results)
                    yield return result;
            }
            
        }

        public SparqlGraphPatternType PatternType { get{return SparqlGraphPatternType.SubSelect;}}
    }
}
