using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlIsNum : SparqlExpression
    {
        public SparqlIsNum(SparqlExpression value)
            :base(value.AggregateLevel, value.IsStoreUsed)
        {
            if (value.Const != null)
                Const = new OV_bool(value.Const is INumLiteral);
            else     {
                Operator = result => value.TypedOperator(result) is INumLiteral; 
                TypedOperator = result => new OV_bool(value.TypedOperator(result) is INumLiteral); //todo
            }
                    //f is double ||
                    //   f is long ||
                    //   f is int ||
                    //   f is short ||
                    //   f is byte ||
                    //   f is ulong ||
                    //   f is uint ||
                    //   f is ushort;

        }
    }
}
// xsd:nonPositiveInteger
//xsd:negativeInteger
//xsd:long
//xsd:int
//xsd:short
//xsd:byte
//xsd:nonNegativeInteger
//xsd:unsignedLong
//xsd:unsignedInt
//xsd:unsignedShort
//xsd:unsignedByte
//xsd:positiveInteger;