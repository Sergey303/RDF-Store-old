using System;

namespace SparqlQuery.SparqlClasses.Expressions
{
    public class SparqlCeil : SparqlUnaryExpression
    {                            
        public SparqlCeil(SparqlExpression value)
            : base(o => Math.Ceiling(o), value)
        {
        //    value.SetExprType(ExpressionTypeEnum.numeric);
         //   SetExprType(ExpressionTypeEnum.numeric);        
        }

    }
}