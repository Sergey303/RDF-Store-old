using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    public class SparqlInCollectionExpression : SparqlExpression
    {
        public SparqlInCollectionExpression (SparqlExpression itemExpression, List<SparqlExpression> collection)
        {
                var cConsts = collection.Select(expression => expression.Const).ToArray();
            if (itemExpression.Const != null)
            {
                if (cConsts.Contains(itemExpression.Const))
                {
                    Const = new OV_bool(true);
                    return;
                }
                if (cConsts.All(c => c != null))
                {
                    Const = new OV_bool(false);
                    return;
                }
                AggregateLevel = SetAggregateLevel(collection.Select(c => c.AggregateLevel).ToArray());
            }
            else
                AggregateLevel = SetAggregateLevel(itemExpression.AggregateLevel,
                    SetAggregateLevel(collection.Select(c => c.AggregateLevel).ToArray()));

            Operator = result =>
            {
                var o = itemExpression.Operator(result);
                return collection.Any(element => element.Operator(result).Equals(o));
            };
            TypedOperator = o => new OV_bool(Operator(o));
            //SetExprType(ObjectVariantEnum.Bool);
        }
    }
}