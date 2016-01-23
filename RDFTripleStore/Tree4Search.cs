using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolarDB;

namespace RDFTripleStore
{
   public class Tree4Search
    {
        private Dictionary<int, long> searchIndex;
        private readonly PaCell paCell;
        private int maxChildrenCount;
        private readonly Dictionary<int, int> coding;
        private readonly Dictionary<int, int> decoding = new Dictionary<int, int>();
        private readonly int height;
        private Dictionary<int, HashSet<int>> direct;
        private Dictionary<int, HashSet<int>> inverse;
        private readonly List<int> roots;


        public Tree4Search(KeyValuePair<int, int>[] edges)
        {
            coding = new Dictionary<int, int>();
            direct = new Dictionary<int, HashSet<int>>();
            inverse = new Dictionary<int, HashSet<int>>();
            foreach (var pair in edges)
            {
                if(!direct.ContainsKey(pair.Key)) direct.Add(pair.Key, new HashSet<int>());
                if (!inverse.ContainsKey(pair.Value)) inverse.Add(pair.Value, new HashSet<int>());
                direct[pair.Key].Add(pair.Value);
                inverse[pair.Value].Add(pair.Key);
            }


            roots = direct.Keys.Where(dirNode => !inverse.ContainsKey(dirNode)).ToList();
            if(!roots.Any()) throw new Exception("roots empty");
            
            
            maxChildrenCount = direct.Max(pair => pair.Value.Count);


            height = direct.Any() ? roots.Max(i => GetTreeHeight(i)) : 0;//edges.Count();
             
                for (int i = 1; i < roots.Count + 1; i++)
            {
                CreateCoding(roots[i - 1], i);
            }

            PType treePType = Enumerable.Range(0, height).
                Aggregate(new PType(PTypeEnumeration.integer),
                    (PType res, int level) =>
                        new PTypeRecord(new NamedType("value", new PType(PTypeEnumeration.integer)),
                            new NamedType("children", new PTypeSequence(res))));
            paCell = new PaCell(treePType, "../../tree.pa", false);
            ReCreate();
       
            direct.Clear();
            inverse.Clear();
        }

        private void CreateCoding(int node, int code)
        {
            coding.Add(node, code);
            decoding.Add(code, node);

            int i = 1;
            if(direct.ContainsKey(node))
            foreach (var child in direct[node])
            {
                CreateCoding(child, code* maxChildrenCount + (i++));
            }
        }

        private IEnumerable<int> GetDirect(int node)
        {
            if (paCell.Root.Type.Vid == PTypeEnumeration.integer)
                return Enumerable.Empty<int>();
            var paEntry = paCell.Root;
            paEntry.offset = searchIndex[node];
            return paEntry.Field(1).Elements().Select(entry => (int) entry.Field(0).Get());
        }


        public void ReCreate()
        {
            paCell.Clear();
            object content = null;
            if (height == 0)
            {
                content = 0;

            }
            else content = new object[] {0, roots.Select(c => CreateTreeObject(coding[c], c, 1)).ToArray()};
            paCell.Fill(content);
            searchIndex = new Dictionary<int, long>();
            AddInIndex(paCell.Root);
        }

        private void AddInIndex(PaEntry entry)
        {
            int code;
            if (entry.Type.Vid == PTypeEnumeration.integer)
            {
                code = (int) entry.Get();
                if (code != 0)
                    searchIndex.Add(decoding[code], entry.offset);
            }
            else
            {
                code = (int) entry.Field(0).Get();
                if (code != 0)
                    searchIndex.Add(decoding[code], entry.offset);
                foreach (var chiledEntry in entry.Field(1).Elements())
                {
                    AddInIndex(chiledEntry);
                }
            }

        }


       public object CreateTreeObject(int code, int node, int level)
       {
           return level != height
               ? (object)
                   new object[]
                   {
                       code,
                       direct.ContainsKey(node)
                           ? direct[node].Select(c => CreateTreeObject(coding[c], c, level + 1)).ToArray()
                           : new object[0]
                   }
               : coding[node];
       }

       private int GetTreeHeight(int node) => direct.ContainsKey(node)
           ? direct[node].Max(x1 => GetTreeHeight(x1) + 1)
           : 1;

        public int[] GetChildren(int node)
        {
            if (paCell.Root.Type.Vid == PTypeEnumeration.integer)
                return new int[0];
            var paEntry = paCell.Root;
            if (searchIndex.ContainsKey(node))
            {
                paEntry.offset = searchIndex[node];return paEntry.Field(1).Elements().Select(entry => (int)entry.Field(0).Get())
                .Select(c => decoding[c]).ToArray();}
            else 
                return new int[0];
        }




        public bool TestConnection( int node1, int node2)
        {
            int code1, code2, div;
            if (!coding.TryGetValue(node1, out code1)) return false;
            if (!coding.TryGetValue(node2, out code2)) return false;
            if (code2 > code1)
            {
                div = code2;
                return
                    Enumerable.Range(1, height)
                        .Any(i =>
                        {
                            div = div / maxChildrenCount;
                            return code1 == div;
                        });
            }
            else
                div = code1;
            return
                Enumerable.Range(1, height)
                    .Any(i =>
                    {
                        div = div / maxChildrenCount;
                        return code2 == div;
                    });
        }

        public IEnumerable<int> GetParents(int node)
        {
            int code;
            if (!coding.TryGetValue(node, out code))
                yield break;
            while (code > 0)
            {
                code = code / maxChildrenCount;
                yield return decoding[code];
            }
        }
    }

    public static class PaCellCopy
    {
        public static PaCell Copy(this PaCell cell, string oldPath, string newPath)
        {
            cell.Close();
            File.Copy(oldPath, newPath);
            
            cell=new PaCell(cell.Type, oldPath, false);
            return new PaCell(cell.Type, newPath, false);
        }
    }
}
