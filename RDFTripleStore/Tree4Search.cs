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
       protected Dictionary<int, long> searchIndex;
       protected PaCell paCell;
        private int maxChildrenCount;
       protected readonly Dictionary<int, int> coding = new Dictionary<int, int>();
       protected readonly Dictionary<int, int> decoding = new Dictionary<int, int>();
       protected int height;
       protected Dictionary<int, HashSet<int>> direct = new Dictionary<int, HashSet<int>>();
       protected readonly List<int> roots=new List<int>();


        public virtual void ReCreate(KeyValuePair<int, int>[] edges)
        {

            ReCreateDirectAndRoots(edges);
                        maxChildrenCount = direct.Max(pair => pair.Value.Count);


            height = direct.Any() ? roots.Max(i => GetTreeHeight(i)) : 0;//edges.Count();

            if (roots.Count > maxChildrenCount)
                maxChildrenCount = roots.Count;

            coding.Clear();
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
        }

       protected virtual void ReCreateDirectAndRoots(KeyValuePair<int, int>[] edges)
       {
           var inverse = new HashSet<int>();
            
            foreach (var pair in edges)
           {
               if (!direct.ContainsKey(pair.Key)) direct.Add(pair.Key, new HashSet<int>());
               // if (inverse.ContainsKey(pair.Value)) throw new Exception();
               direct[pair.Key].Add(pair.Value);
                if (!inverse.Contains(pair.Value))
                    inverse.Add(pair.Value);
            }


           roots.Clear();
           roots.AddRange(direct.Keys.Where(dirNode => !inverse.Contains(dirNode)));
           if (!roots.Any()) throw new Exception("roots empty");
       }

       protected virtual void CreateCoding(int node, int code)
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


        private void ReCreate()
        {
            paCell.Clear();
            object content = null;
            if (height == 0)
            {
                content = 0;

            }
            else content = new object[] {0, roots.Select(c => CreateTreeObject(c, 1)).ToArray()};
            paCell.Fill(content);
            searchIndex = new Dictionary<int, long>();
            if(paCell.Root.Type.Vid != PTypeEnumeration.integer)
                foreach (var subroot in paCell.Root.Field(1).Elements())
                    AddInIndex(subroot);
        }

       protected virtual void AddInIndex(PaEntry entry)
        {
            int node;
            if (entry.Type.Vid == PTypeEnumeration.integer)
            {
                node = (int) entry.Get();
             
                    searchIndex.Add(node, entry.offset);
            }
            else
            {
                node = (int) entry.Field(0).Get();
                    searchIndex.Add(node, entry.offset);
                foreach (var chiledEntry in entry.Field(1).Elements())
                    AddInIndex(chiledEntry);
            }

        }


       protected virtual object CreateTreeObject(int node, int level)
       {
           return level != height
               ? (object)
                   new object[]
                   {
                       node,
                       direct.ContainsKey(node)
                           ? direct[node].Select(c => CreateTreeObject(c, level + 1)).ToArray()
                           : new object[0]
                   }
               : node;
       }

       private int GetTreeHeight(int node) => direct.ContainsKey(node)
           ? direct[node].Max(x1 => GetTreeHeight(x1) + 1)
           : 1;

        public int[]   GetChildren(int node)
        {
            if (paCell.Root.Type.Vid == PTypeEnumeration.integer)
                return new int[0];
            var paEntry = paCell.Root;
            if (searchIndex.ContainsKey(node))
              {
                paEntry.offset = searchIndex[node];
                var elements = paEntry.Field(1).Elements().Select(entry => (int)entry.Field(0).Get()).ToArray();
                return elements;
            }
            else
                return new int[0];
        }




        public virtual bool TestConnection( int node1, int node2)
        {
            int code1, code2;
            if (!coding.TryGetValue(node1, out code1)) return false;
            if (!coding.TryGetValue(node2, out code2)) return false;
            return TestConnectionByCodes(code1, code2);
        }

       protected bool TestConnectionByCodes(int code1, int code2)
       {
           int div, res;
           if (code2 > code1)
           {
               div = code2;
               res = code1;
           }
           else
           {
               div = code1;
               res = code2;
           }
           
               while ((div = div / maxChildrenCount) > 0)
                if (res == div) return true;
            return false;
        }

       public virtual IEnumerable<int> GetParents(int node)
        {
            int code;
            if (!coding.TryGetValue(node, out code))
                return Enumerable.Empty<int>();
            return GetParentsByCode(code);
        }

       protected  virtual IEnumerable<int> GetParentsByCode(int code)
       {
           while ((code = code / maxChildrenCount) > 0)
           {
               yield return decoding[code];
           }
       }

       public Tree<int> GetAllSubTree(int node)
                {
           var children = GetChildren(node);
           return new Tree<int>()
           {
               Item = node,
               Children = children.Select(GetAllSubTree).ToArray()
           };
       }
    }
}
