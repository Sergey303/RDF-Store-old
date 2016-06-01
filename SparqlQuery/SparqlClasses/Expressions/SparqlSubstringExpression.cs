using System;

namespace SparqlQuery.SparqlClasses.Expressions
{
   public class SparqlSubstringExpression  : SparqlExpression
   {
       private SparqlExpression strExpression, startExpression;
       private RdfQuery11Translator q;

       internal void SetString(SparqlExpression value)
       {


           strExpression = value;
       }

       public void SetStartPosition(SparqlExpression value)
       {

           startExpression = value;
           switch (NullablePairExt.Get(strExpression.Const, startExpression.Const))
           {
               case NP.bothNull:
                   Operator = result => strExpression.Operator(result).Substring(startExpression.Operator(result));
                   TypedOperator =
                       result =>
                           strExpression.TypedOperator(result)
                               .Change(o => o.Substring(startExpression.Operator(result)));
                   AggregateLevel = SetAggregateLevel(strExpression.AggregateLevel, startExpression.AggregateLevel);
                   break;
               case NP.leftNull:
                   Operator = result => strExpression.Operator(result).Substring(startExpression.Const.Content);
                   TypedOperator =
                       result => startExpression.Const.
                           Change(o => strExpression.Operator(result).Substring(o));
                   AggregateLevel = strExpression.AggregateLevel;
                   break;
               case NP.rigthNull:
                   Operator = result => ((string) strExpression.Const.Content).Substring(startExpression.Operator(result));
                   TypedOperator = result => strExpression.Const.Change(o => o.Substring(startExpression.Operator(result)));
                   AggregateLevel = startExpression.AggregateLevel;
                   break;
               case NP.bothNotNull:
                   Const = strExpression.Const.Change(o => o.Substring(startExpression.Const.Content));
                   break;
               default:
                   throw new ArgumentOutOfRangeException();
           }
       }

       internal void SetLength(SparqlExpression lengthExpression)
       {
           switch (NullableTripleExt.Get(strExpression.Const, startExpression.Const, lengthExpression.Const))
           {
               case NT.AllNull:
                   TypedOperator = result => strExpression.TypedOperator(result).Change(o => o.Substring(startExpression.Operator(result), lengthExpression.Operator(result)));
                   Operator = result => strExpression.Operator(result).Substring(startExpression.Operator(result), lengthExpression.Operator(result));
                   AggregateLevel = SetAggregateLevel(strExpression.AggregateLevel, startExpression.AggregateLevel, lengthExpression.AggregateLevel);
                   break;
               case NT.FirstNotNull :
                   TypedOperator = result => strExpression.Const.Change(o => o.Substring(startExpression.Operator(result), lengthExpression.Operator(result)));
                   Operator = result => ((string)strExpression.Const.Content).Substring(startExpression.Operator(result), lengthExpression.Operator(result));
                   AggregateLevel = SetAggregateLevel(startExpression.AggregateLevel, lengthExpression.AggregateLevel);                   
                   break;
               case NT.SecondNotNull:
                   TypedOperator = result => strExpression.TypedOperator(result).Change(o => o.Substring(startExpression.Const.Content, lengthExpression.Operator(result)));
                   Operator = result => strExpression.Operator(result).Substring(startExpression.Const.Content, lengthExpression.Operator(result));
                   AggregateLevel = SetAggregateLevel(strExpression.AggregateLevel, lengthExpression.AggregateLevel);                   
                   break;
               case NT.ThirdNotNull:     
                   TypedOperator = result => strExpression.TypedOperator(result).Change(o => o.Substring(startExpression.Operator(result), lengthExpression.Const.Content));
                   Operator = result => strExpression.Operator(result).Substring(startExpression.Operator(result), lengthExpression.Const.Content);
                   AggregateLevel = SetAggregateLevel(strExpression.AggregateLevel, startExpression.AggregateLevel);                   
                   break;
               case (NT)254: //~ NT.FirstNotNull 
                   TypedOperator = result => strExpression.TypedOperator(result).Change(o => o.Substring(startExpression.Const.Content, lengthExpression.Const.Content));
                   Operator = result => strExpression.Operator(result).Substring(startExpression.Const.Content, lengthExpression.Const.Content);
                   AggregateLevel = strExpression.AggregateLevel;
                   break;
               case (NT)253://~NT.SecondNotNull 
                   TypedOperator = result => strExpression.Const.Change(o => o.Substring(startExpression.Operator(result), lengthExpression.Const.Content));
                   Operator = result => ((string)strExpression.Const.Content).Substring(startExpression.Operator(result), (int) lengthExpression.Const.Content);
                   AggregateLevel = startExpression.AggregateLevel;
                   break;
               case (NT)252: //~ NT.ThirdNotNull 
                   TypedOperator = result => strExpression.Const.Change(o => o.Substring(startExpression.Const.Content, lengthExpression.Operator(result)));
                   Operator = result => ((string)strExpression.Const.Content).Substring((int) startExpression.Const.Content, lengthExpression.Operator(result));
                   AggregateLevel = lengthExpression.AggregateLevel;
                   break;
               case (NT)255: //~NT.AllNull
                   Const = strExpression.Const.Change(o => o.Substring(startExpression.Const.Content, lengthExpression.Const.Content));
                   break;
               default:
                   throw new ArgumentOutOfRangeException();
           }
       }
    }
}
