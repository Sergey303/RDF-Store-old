﻿using System.Linq;
using RDFCommon.OVns;
using SparqlQuery.SparqlClasses.Query.Result;

namespace SparqlQuery.SparqlClasses.SparqlAggregateExpression
{
    class SparqlGroupConcatExpression : SparqlAggregateExpression
    {
        protected override void Create()
        {
            if (Expression.Const != null)
            {
                Operator = result => string.Join(Separator, ((SparqlGroupOfResults)result).Group.Select(r=>Expression.Const.Content));
                TypedOperator = result => new OV_string(string.Join(Separator, ((SparqlGroupOfResults)result).Group.Select(r=>Expression.Const.Content)));
            }
            else
            {
                Operator = result => string.Join(Separator, ((SparqlGroupOfResults) result).Group.Select(Expression.Operator));
                TypedOperator = result => new OV_string(string.Join(Separator, ((SparqlGroupOfResults)result).Group.Select(Expression.Operator)));
            }
        }
    }
}