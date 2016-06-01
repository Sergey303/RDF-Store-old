using System;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlRand : SparqlExpression
    {
        static Random r = new Random();
        public SparqlRand()  : base(VariableDependenceGroupLevel.UndependableFunc, false)
        {
            Operator = res => r.NextDouble();
            TypedOperator = result => new OV_double(r.NextDouble());
        }
    }
}