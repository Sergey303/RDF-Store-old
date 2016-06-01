using System;
using RDFCommon;
using RDFCommon.OVns;


namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlBnode : SparqlExpression
    {

        public SparqlBnode(SparqlExpression value, RdfQuery11Translator q)       :base(value.AggregateLevel, value.IsStoreUsed)
        {
            //IsDistinct = value.IsDistinct;
            //value.SetExprType(ObjectVariantEnum.Str);
            //SetExprType(ObjectVariantEnum.Iri);
            var litConst = value.Const;
            if (litConst != null)
            {
                Operator =
                    TypedOperator = result => q.Store.NodeGenerator.CreateBlankNode(value.Const.Content + result.Id);

            }
            else
            {
                Operator =
                    TypedOperator = result => q.Store.NodeGenerator.CreateBlankNode(value.Operator(result) + result.Id);
                //OvConstruction = o => q.Store.NodeGenerator.CreateBlankNode((string) o);
            }
        }

        public SparqlBnode(RdfQuery11Translator q):base(VariableDependenceGroupLevel.UndependableFunc, true)
        {
          //  SetExprType(ObjectVariantEnum.Iri);
            //OvConstruction = o => q.Store.NodeGenerator.CreateBlankNode(); 
            Operator =  TypedOperator = result => q.Store.NodeGenerator.CreateBlankNode();
        }                      
    }
}
