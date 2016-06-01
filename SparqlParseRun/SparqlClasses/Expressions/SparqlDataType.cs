using System;
using RDFCommon;
using RDFCommon.OVns;


namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlDataType : SparqlExpression
    {
        public SparqlDataType(SparqlExpression value)     : base(value.AggregateLevel, value.IsStoreUsed)
        {
            //SetExprType(ObjectVariantEnum.Iri);
        // value.SetExprType(ExpressionTypeEnum.literal);
            if (value.Const != null)
            {
                Const = new OV_iri(((ILiteralNode) value.Const).DataType);
            }
            else
            {
                Operator=
                TypedOperator = result =>
                {
                    var r = value.TypedOperator(result);
                    var literalNode = r as ILiteralNode;
                    if (literalNode != null)
                        return new OV_iri(literalNode.DataType);
                    throw new ArgumentException();
                };
            }
        }
    }
}