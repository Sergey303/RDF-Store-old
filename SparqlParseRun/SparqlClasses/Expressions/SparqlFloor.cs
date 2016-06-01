using System;
using System.Diagnostics;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlFloor : SparqlUnaryExpression
    {
        public SparqlFloor(SparqlExpression value)
            : base(d => Math.Floor(d), value)
            
        {
           
        }
    }
}