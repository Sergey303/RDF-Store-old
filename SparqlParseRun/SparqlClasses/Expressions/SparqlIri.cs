using System;

using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlIri : SparqlExpression
    {
        public SparqlIri(SparqlExpression value, NodeGenerator q)
        {

            IsAggragate = value.IsAggragate;
            IsDistinct = value.IsDistinct;
         Func = result =>
            {
                var f = value.Func(result);
                if (f is IUriNode)
                    return f;
                if(f is ILiteralNode)      //TODO
                    return q.CreateUriNode(f.Content);
                throw new ArgumentException();  
            }; 
        }
    }
}
