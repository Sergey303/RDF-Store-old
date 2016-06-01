using System;
using System.ComponentModel;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlIf : SparqlExpression
    {
        public SparqlIf(SparqlExpression conditionExpression1, SparqlExpression sparqlExpression2, SparqlExpression sparqlExpression3)
        { 
            switch (NullableTripleExt.Get(conditionExpression1.Const, sparqlExpression2.Const, sparqlExpression3.Const))
            {
                case NT.AllNull:
                    Operator = result =>
                    {
                        var condition = conditionExpression1.Operator(result);
                        if (condition is bool)
                        {
                            return (bool)condition ? sparqlExpression2.Operator(result) : sparqlExpression3.Operator(result);
                        } throw new ArgumentException();
                    };
                    TypedOperator = result =>
                    {
                        var condition = conditionExpression1.Operator(result);
                        if (condition is bool)
                        {
                            return (bool)condition ? sparqlExpression2.TypedOperator(result) : sparqlExpression3.TypedOperator(result);
                        } throw new ArgumentException();
                    };
                    AggregateLevel = SetAggregateLevel(conditionExpression1.AggregateLevel, sparqlExpression2.AggregateLevel, sparqlExpression3.AggregateLevel);
                    break;
                case NT.FirstNotNull:
                    if ((bool) conditionExpression1.Const.Content)
                    {
                        Operator = sparqlExpression2.Operator;
                        TypedOperator = sparqlExpression2.TypedOperator;
                    }
                    else
                    {
                        Operator = sparqlExpression3.Operator;
                        TypedOperator = sparqlExpression3.TypedOperator;   
                    }
                    AggregateLevel = SetAggregateLevel(sparqlExpression2.AggregateLevel, sparqlExpression3.AggregateLevel);

                    break;
                case NT.SecondNotNull:
             Operator = result =>
                    {
                        var condition = conditionExpression1.Operator(result);
                        if (condition is bool)
                        {
                            return (bool)condition ? sparqlExpression2.Const.Content : sparqlExpression3.Operator(result);
                        } throw new ArgumentException();
                    };
                    TypedOperator = result =>
                    {
                        var condition = conditionExpression1.Operator(result);
                        if (condition is bool)
                        {
                            return (bool)condition ? sparqlExpression2.Const : sparqlExpression3.TypedOperator(result);
                        } throw new ArgumentException();
                    };
                    AggregateLevel = SetAggregateLevel(conditionExpression1.AggregateLevel, sparqlExpression3.AggregateLevel);

                    break;
                case NT.ThirdNotNull:
                   Operator = result =>
                    {
                        var condition = conditionExpression1.Operator(result);
                        if (condition is bool)
                        {
                            return (bool)condition ? sparqlExpression2.Operator(result) : sparqlExpression3.Const.Content;
                        } throw new ArgumentException();
                    };
                    TypedOperator = result =>
                    {
                        var condition = conditionExpression1.Operator(result);
                        if (condition is bool)
                        {
                            return (bool)condition ? sparqlExpression2.TypedOperator(result) : sparqlExpression3.Const;
                        } throw new ArgumentException();
                    };
                    AggregateLevel = SetAggregateLevel(conditionExpression1.AggregateLevel, sparqlExpression2.AggregateLevel);

                    break;
                case (NT)252://~NT.ThirdNotNull:
                    if ((bool)conditionExpression1.Const.Content)
                    {
                        Const = sparqlExpression2.Const;
                    }
                    else
                    {
                        Operator = sparqlExpression3.Operator;
                        TypedOperator = sparqlExpression3.TypedOperator;
                    }
                     AggregateLevel =sparqlExpression3.AggregateLevel;
                    break;
                case (NT)253://~NT.SecondNotNull:
                    if ((bool)conditionExpression1.Const.Content)
                    {
                        Operator = sparqlExpression2.Operator;
                        TypedOperator = sparqlExpression2.TypedOperator;
                    }
                    else
                    {
                        Const = sparqlExpression3.Const;
                    }
                    AggregateLevel = sparqlExpression2.AggregateLevel;
                    break;
                case (NT)254://~NT.FirstNotNull:
                  Operator = result =>
                    {
                        var condition = conditionExpression1.Operator(result);
                        if (condition is bool)
                        {
                            return (bool)condition ? sparqlExpression2.Const.Content : sparqlExpression3.Const.Content;
                        } throw new ArgumentException();
                    };
                    TypedOperator = result =>
                    {
                        var condition = conditionExpression1.Operator(result);
                        if (condition is bool)
                        {
                            return (bool)condition ? sparqlExpression2.Const : sparqlExpression3.Const;
                        } throw new ArgumentException();
                    };
                    AggregateLevel = conditionExpression1.AggregateLevel;                    
                    break;
                case (NT)255://~NT.AllNull:
                    Const = (bool) conditionExpression1.Const.Content ? sparqlExpression2.Const : sparqlExpression3.Const;
                    AggregateLevel = VariableDependenceGroupLevel.Const;                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
           
        }
    }
}
