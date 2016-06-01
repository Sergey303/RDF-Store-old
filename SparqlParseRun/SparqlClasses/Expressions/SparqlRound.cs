using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    internal class SparqlRound : SparqlUnaryExpression
    {
        public SparqlRound(SparqlExpression value) : base(o => Math.Round(o), value)
        {

        }
    }
}