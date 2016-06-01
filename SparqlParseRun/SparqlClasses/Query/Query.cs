using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.Interfaces;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.Query;
using SparqlParseRun.SparqlClasses.Query.Result;
using SparqlParseRun.SparqlClasses.SolutionModifier;


namespace SparqlParseRun
{
    public class SparqlQuery 
    {
        protected ISparqlGraphPattern sparqlWhere;
        public readonly SparqlResultSet ResultSet;
        protected RdfQuery11Translator q;
        protected SparqlSolutionModifier sparqlSolutionModifier;
        protected ISparqlGraphPattern valueDataBlock;
        protected IEnumerable<SparqlResult> Seed;

        public SparqlQuery(RdfQuery11Translator q)
        {
            this.q = q;
            ResultSet=new SparqlResultSet(q.prolog);

        }


        public virtual SparqlResultSet Run()
        {
            
            ResultSet.Variables = q.Variables;
            Seed = Enumerable.Repeat(new SparqlResult(q), 1);
            ResultSet.Results = Seed;
                if(valueDataBlock!=null)
          ResultSet.Results=   valueDataBlock.Run(ResultSet.Results);
            ResultSet.Results = sparqlWhere.Run(ResultSet.Results);
         
            if (sparqlSolutionModifier != null)
                ResultSet.Results = sparqlSolutionModifier.Run(ResultSet.Results, ResultSet);   
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
            this.sparqlSolutionModifier = sparqlSolutionModifier1;
        }
    }
}
