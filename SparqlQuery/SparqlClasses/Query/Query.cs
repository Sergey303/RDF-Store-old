using System.Collections.Generic;
using System.Linq;
using SparqlQuery.SparqlClasses.GraphPattern;
using SparqlQuery.SparqlClasses.Query.Result;
using SparqlQuery.SparqlClasses.SolutionModifier;

namespace SparqlQuery.SparqlClasses.Query
{
    public class SparqlQuery 
    {
        protected ISparqlGraphPattern sparqlWhere;
        public readonly SparqlResultSet ResultSet;
        protected RdfQuery11Translator q;
        public SparqlSolutionModifier SparqlSolutionModifier;
        protected ISparqlGraphPattern valueDataBlock;
        protected IEnumerable<SparqlResult> Seed;

        public SparqlQuery(RdfQuery11Translator q)
        {
            this.q = q;
            ResultSet=new SparqlResultSet(this,q.prolog);

        }


        public virtual SparqlResultSet Run()
        {
            
            ResultSet.Variables = q.Variables;
            Seed = Enumerable.Repeat(new SparqlResult(q), 1);
            ResultSet.Results = Seed;
                if(valueDataBlock!=null)
          ResultSet.Results=   valueDataBlock.Run(ResultSet.Results);
            ResultSet.Results = sparqlWhere.Run(ResultSet.Results);
         
            if (SparqlSolutionModifier != null)
                ResultSet.Results = SparqlSolutionModifier.Run(ResultSet.Results, ResultSet);   
            return ResultSet;
        }
    //    public SparqlQueryTypeEnum Type { get; set; }
/*
        public abstract SparqlQueryTypeEnum QueryType { get; }
*/


        internal void SetValues(ISparqlGraphPattern valueDataBlock)
        {
            this.valueDataBlock = valueDataBlock;
        }

        public void Create(ISparqlGraphPattern sparqlWhere, SparqlSolutionModifier sparqlSolutionModifier1)
        {
            this.sparqlWhere = sparqlWhere;
            this.SparqlSolutionModifier = sparqlSolutionModifier1;
        }
    }
}
