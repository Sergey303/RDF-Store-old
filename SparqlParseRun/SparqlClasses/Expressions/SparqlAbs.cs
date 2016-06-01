using System;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{

    public class SparqlAbs : SparqlUnaryExpression
    {                                    
        public SparqlAbs(SparqlExpression value)
            : base(o => Math.Abs(o), value)
        {
           // value.SetExprType(ExpressionTypeEnum.numeric);
            //SetExprType(ExpressionTypeEnum.numeric);
        }
    }
}