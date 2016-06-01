using System.Linq;
using SparqlQuery.SparqlClasses.GraphPattern;
using SparqlQuery.SparqlClasses.GraphPattern.Triples;
using SparqlQuery.SparqlClasses.Query.Result;
using SparqlQuery.SparqlClasses.SolutionModifier;

namespace SparqlQuery.SparqlClasses.Query
{
    public class SparqlConstructQuery :SparqlQuery
    {
      
        private SparqlGraphPattern constract;

        public SparqlConstructQuery(RdfQuery11Translator q) : base(q)
        {
           
        } 
        internal void Create(SparqlSolutionModifier sparqlSolutionModifier)
        {
            this.SparqlSolutionModifier = sparqlSolutionModifier;
        }

        internal void Create(SparqlGraphPattern sparqlTriples)
        {         
            sparqlWhere = sparqlTriples;
        }


        internal void Create(SparqlGraphPattern sparqlTriples, ISparqlGraphPattern sparqlWhere, SparqlSolutionModifier sparqlSolutionModifier)
        {
            constract = sparqlTriples;
            this.sparqlWhere = sparqlWhere;
            this.SparqlSolutionModifier = sparqlSolutionModifier;
        }

        public override SparqlResultSet Run()
        {
           base.Run();
            ResultSet.GraphResult = q.Store.CreateTempGraph();
            foreach (var result in ResultSet.Results)
                foreach (var st in constract.Cast<SparqlTriple>())
                    st.Substitution(result, (s, p, o) =>    ResultSet.GraphResult.Add(s, p, o));
            return  ResultSet;
        }

        //public override SparqlQueryTypeEnum QueryType
        //{
        //    get { return SparqlQueryTypeEnum.Construct; }
        //}
    }
}
