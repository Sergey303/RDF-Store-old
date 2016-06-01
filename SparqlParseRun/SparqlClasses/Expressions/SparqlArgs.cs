using System.Collections.Generic;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public class SparqlArgs :List<SparqlExpression>
    {
        public bool isDistinct;

        internal void IsDistinct()
        {
            isDistinct=true;
        }

       
    }
}
