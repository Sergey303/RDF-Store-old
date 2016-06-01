using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using RDFCommon.OVns;
using RDFCommon;

namespace RDFTripleStore
{
    public class NamedGraphsByFolders : RdfNamedGraphs
    {

        public NamedGraphsByFolders(DirectoryInfo directory, NodeGenerator ng, Func<DirectoryInfo, IGraph> graphCtor, Action<DirectoryInfo> graphDrop)
            : base(ng, s => graphCtor(new DirectoryInfo(directory + "/named graph " + CodeGraphName2DirName(s))), s=> graphDrop(new DirectoryInfo(directory + "/named graph " + CodeGraphName2DirName(s))))
        {
            if(!directory.Exists) directory.Create();
            foreach (var graphDir in directory.EnumerateDirectories("named graph *", SearchOption.TopDirectoryOnly))
                named.Add(DecodeDirName2GraphName(graphDir.Name.Substring(12)), graphCtor(graphDir));
        }

        private static string CodeGraphName2DirName(string name)
        {
          //  var codeGraphName2DirName = Convert.ToString(Encoding.UTF32.GetBytes(name));
            return  HttpUtility.UrlEncode(name);
        }

        private static string DecodeDirName2GraphName(string fileName)
        {
            //return Encoding.UTF32.GetString(Convert.FromBase64String(fileName));
            return HttpUtility.UrlDecode(fileName);

        }
    }
}