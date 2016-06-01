using System;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlUuid : SparqlExpression
    {
        public SparqlUuid()    :base(VariableDependenceGroupLevel.UndependableFunc, false)
        {                          
          
           Operator = TypedOperator = result => new OV_iri("urn:uuid:" + Guid.NewGuid());
        }
    }
}
