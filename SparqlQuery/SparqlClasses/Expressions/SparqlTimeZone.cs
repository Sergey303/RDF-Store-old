using System;
using RDFCommon.OVns;

namespace SparqlQuery.SparqlClasses.Expressions
{
    class SparqlTimeZone : SparqlUnaryExpression<OV_dayTimeDuration>
    {
        private SparqlExpression sparqlExpression;

        public SparqlTimeZone(SparqlExpression value)
            :base(f =>
            {
                if (f is DateTime)
                    return TimeZoneInfo.Utc.GetUtcOffset((DateTime)f);
                else if (f is DateTimeOffset)
                    return ((DateTimeOffset)f).Offset;
                throw new ArgumentException();
            }, value, f=>new OV_dayTimeDuration(f))
        {  
          
        }
    }
}
