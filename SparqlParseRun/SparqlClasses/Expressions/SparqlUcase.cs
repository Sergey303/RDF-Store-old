using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlUcase :SparqlUnaryExpression
    {
        public SparqlUcase(SparqlExpression value)
            : base(o => o.ToUpperInvariant(), value)
        {

        }
    }
}
