using System;
using System.Collections.Generic;
using RDFCommon;
using SparqlParseRun.SparqlClasses.Query;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.Update
{
    public class SparqlUpdateQuery : SparqlQuery
    {
        public List<ISparqlUpdate> Updates = new List<ISparqlUpdate>();

        public SparqlUpdateQuery(RdfQuery11Translator q) : base(q)
        {
            ResultSet.ResultType = ResultType.Update;
        }

       

        public override SparqlResultSet Run()
        {
            try
            {
                foreach (var sparqlUpdate in Updates)
                    sparqlUpdate.Run(q.Store);
                ResultSet.UpdateStatus = SparqlUpdateStatus.ok;
                ResultSet.UpdateMessage ="ok";
                return ResultSet;
            }
            catch (Exception e)
            {
                ResultSet.UpdateStatus = SparqlUpdateStatus.fail;
                ResultSet.UpdateMessage = e.Message;
            }
            return ResultSet;
        }

        internal void Create(ISparqlUpdate sparqlUpdate)
        {
            Updates.Add(sparqlUpdate);
        }

        internal void Add(SparqlUpdateQuery sparqlUpdateQuery)
        {
        Updates.AddRange(sparqlUpdateQuery.Updates);
        }

        //public override SparqlQueryTypeEnum QueryType
        //{
        // get{ return SparqlQueryTypeEnum.Update;}
        //}
    }
}
