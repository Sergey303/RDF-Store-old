﻿using System;
using RDFCommon;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlUri : SparqlExpression
    {
        public SparqlUri(SparqlExpression value, RdfQuery11Translator q)  :base(value.AggregateLevel, value.IsStoreUsed)
        {
            if (value.Const != null)
                Const = new OV_iri(q.prolog.GetFromString((string)value.Const.Content));
            else
            {
               Operator = result => new OV_iri(q.prolog.GetFromString((string) value.Operator(result).Content));
                //SetExprType(ObjectVariantEnum.Iri);
                TypedOperator =
                    result => new OV_iri(q.prolog.GetFromString((string) value.TypedOperator(result).Content));
            }
        }
    }
}
