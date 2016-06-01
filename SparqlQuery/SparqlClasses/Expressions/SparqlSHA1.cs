using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SparqlQuery.SparqlClasses.Expressions
{

    class SparqlSHA1 : SparqlHashExpression
    {
        private readonly SHA1 hash;
             public SparqlSHA1(SparqlExpression value)    :base(value)
        {
        Create(value);    
        }

        protected override string CreateHash(string f)
        {
            return string.Join("",
                hash.ComputeHash(Encoding.UTF8.GetBytes(f)).Select( b => b.ToString("x2")));
        }
    }
}
