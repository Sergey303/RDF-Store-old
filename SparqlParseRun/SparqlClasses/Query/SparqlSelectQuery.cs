﻿using System;
using System.Linq;
using RDFCommon;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.Query.Result;
using SparqlParseRun.SparqlClasses.SolutionModifier;

namespace SparqlParseRun.SparqlClasses.Query
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
            this.sparqlSolutionModifier = sparqlSolutionModifier;
            //  this.sparqlSolutionModifier.IsDistinct=sparqlSelect.IsDistinct;
            //if (this.sparqlSolutionModifier.IsDistinct)
            //    sparqlSelect.IsDistinct = false;
        }

        public override SparqlResultSet Run()
        {
            ResultSet.Variables = base.q.Variables;
            ResultSet.Results=Enumerable.Repeat(new SparqlResult(q), 1);
            ResultSet.Results = sparqlWhere.Run(ResultSet.Results);
            
            if (sparqlSolutionModifier != null )
                ResultSet.Results = sparqlSolutionModifier.Run(ResultSet.Results, ResultSet);
          
            return ResultSet;
        }

        //public override SparqlQueryTypeEnum QueryType
        //{
        //    get { return SparqlQueryTypeEnum.Select; }
        //}
    }
}
