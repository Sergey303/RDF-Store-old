using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RDFCommon;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    class SparqlMD5 :SparqlHashExpression
    {
        public SparqlMD5(SparqlExpression value) : base(value)
        {
            Create(value);            
        }
        readonly MD5 md5 = new MD5CryptoServiceProvider();
         protected override string CreateHash(string f)
        {
            return string.Join("", md5.ComputeHash(Encoding.UTF8.GetBytes(f)).Select( b => b.ToString("x2")));
        }
    }
}
