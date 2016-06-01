using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RDFCommon;
using RDFCommon.OVns;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlSHA512 : SparqlHashExpression
    {
      readonly SHA512 hash=new SHA512CryptoServiceProvider();
            public SparqlSHA512(SparqlExpression value)   :base(value)
        {
                //SetExprType(ObjectVariantEnum.Str);
            //value.SetExprType(ObjectVariantEnum.Str);

            Create(value);
        }

      protected override string CreateHash(string f)
        {
            return string.Join("",
                hash.ComputeHash(Encoding.UTF8.GetBytes(f)).Select( b => b.ToString("x2")));
        }
    }
}
