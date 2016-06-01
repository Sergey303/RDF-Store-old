using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlUuid : SparqlExpression
    {
        public SparqlUuid()    :base(VariableDependenceGroupLevel.UndependableFunc, false)
        {                          
          
           Operator = TypedOperator = result => new OV_iri("urn:uuid:" + Guid.NewGuid());
        }
    }
}
