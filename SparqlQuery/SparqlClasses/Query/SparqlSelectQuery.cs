using System.Linq;
using SparqlQuery.SparqlClasses.GraphPattern;
using SparqlQuery.SparqlClasses.Query.Result;
using SparqlQuery.SparqlClasses.SolutionModifier;

namespace SparqlQuery.SparqlClasses.Query
{
    public class SparqlSelectQuery : SparqlQuery
    {
    
        public SparqlSelectQuery(RdfQuery11Translator q) : base(q)
        {
            ResultSet.ResultType = ResultType.Select;
          
        }

      
      
       

        internal void Create( SparqlGraphPattern sparqlWhere, SparqlSolutionModifier sparqlSolutionModifier)
        {
            this.sparqlWhere = sparqlWhere;
            this.SparqlSolutionModifier = sparqlSolutionModifier;
            //  this.sparqlSolutionModifier.IsDistinct=sparqlSelect.IsDistinct;
            //if (this.sparqlSolutionModifier.IsDistinct)
            //    sparqlSelect.IsDistinct = false;
        }

        public override SparqlResultSet Run()
        {
            ResultSet.Variables = base.q.Variables;
            ResultSet.Results=Enumerable.Repeat(new SparqlResult(q), 1);
            ResultSet.Results = sparqlWhere.Run(ResultSet.Results);
            
            if (SparqlSolutionModifier != null )
                ResultSet.Results = SparqlSolutionModifier.Run(ResultSet.Results, ResultSet);
          
            return ResultSet;
        }

        //public override SparqlQueryTypeEnum QueryType
        //{
        //    get { return SparqlQueryTypeEnum.Select; }
        //}
    }
}
