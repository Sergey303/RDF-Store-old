using SparqlQuery.SparqlClasses.GraphPattern;
using SparqlQuery.SparqlClasses.Query.Result;
using SparqlQuery.SparqlClasses.SolutionModifier;

namespace SparqlQuery.SparqlClasses.Query
{
    public class SparqlAsqQuery : SparqlQuery
    {

        public SparqlAsqQuery(RdfQuery11Translator q) : base(q)
        {
         
        }
        internal void Create(SparqlGraphPattern sparqlWhere, SparqlSolutionModifier sparqlSolutionModifier)
        {
            this.sparqlWhere = sparqlWhere;
            this.SparqlSolutionModifier = sparqlSolutionModifier;
        }

        public override SparqlResultSet Run()
        {
            base.Run();
            return ResultSet;

        }

        //public override SparqlQueryTypeEnum QueryType
        //{
        //    get { return SparqlQueryTypeEnum.Ask; }
        //}
    }
}
