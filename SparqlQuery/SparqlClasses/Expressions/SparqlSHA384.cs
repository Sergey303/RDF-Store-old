using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SparqlQuery.SparqlClasses.Expressions
{
    internal class SparqlSHA384 : SparqlHashExpression
    {
        private readonly SHA384 hash = new SHA384CryptoServiceProvider();

        public SparqlSHA384(SparqlExpression value)   :base(value)
        {
            //SetExprType(ObjectVariantEnum.Str);
            //value.SetExprType(ObjectVariantEnum.Str);

            Create(value);
        }

        protected override string CreateHash(string f)
        {
            return string.Join("",
                hash.ComputeHash(Encoding.UTF8.GetBytes(f)).Select(b => b.ToString("x2")));
        }
    }
}
