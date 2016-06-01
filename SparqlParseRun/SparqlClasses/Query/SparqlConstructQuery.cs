using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples;
using SparqlParseRun.SparqlClasses.Query.Result;
using SparqlParseRun.SparqlClasses.SolutionModifier;

namespace SparqlParseRun.SparqlClasses.Query
{
    public class SparqlConstructQuery :SparqlQuery
    {
      
        private SparqlGraphPattern constract;

        public SparqlConstructQuery(RdfQuery11Translator q) : base(q)
        {
           
        } 
        internal void Create(SparqlSolutionModifier sparqlSolutionModifier)
        {
            this.sparqlSolutionModifier = sparqlSolutionModifier;
        }

        internal void Create(SparqlGraphPattern sparqlTriples)
        {         
            sparqlWhere = sparqlTriples;
        }


        internal void Create(SparqlGraphPattern sparqlTriples, ISparqlGraphPattern sparqlWhere, SparqlSolutionModifier sparqlSolutionModifier)
        {
            constract = sparqlTriples;
            this.sparqlWhere = sparqlWhere;
            this.sparqlSolutionModifier = sparqlSolutionModifier;
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
