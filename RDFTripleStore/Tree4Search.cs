using System;
using System.Collections.Generic;
using System.Linq;
using PolarDB;
using UniversalIndex;

namespace RDFTripleStore
{
   public class Tree4Search
    {
       protected Dictionary<int, long> searchIndex;
       protected PaCell paCell;
       private int maxChildrenCount;
       protected readonly IndexDynamic<int, IndexToSortableTableImmutable<int>> codingIndex;
       protected readonly IndexDynamic<int, IndexToSortableTableImmutable<int>> decodingIndex;
    //  protected readonly Dictionary<int, int> coding = new Dictionary<int, int>();
      // protected readonly Dictionary<int, int> decoding = new Dictionary<int, int>();
       protected int height;

       public Tree4Search(string path)
       {

           this.codingIndex = new IndexDynamic<int, IndexToSortableTableImmutable<int>>(true,
               new IndexToSortableTableImmutable<int>(new TableView(path + " coding.pa",
                   new PTypeRecord(new NamedType("node", new PType(PTypeEnumeration.integer)),
                       new NamedType("code", new PType(PTypeEnumeration.integer)))),
                       o => (int)((object[])o)[0])) { Scale = new ScaleCell(path + "coding scale.pa")};
            this.decodingIndex = new IndexDynamic<int, IndexToSortableTableImmutable<int>>(true,
               new IndexToSortableTableImmutable<int>(new TableView(path + " decoding.pa",
                   new PTypeRecord(new NamedType("node", new PType(PTypeEnumeration.integer)),
                       new NamedType("code", new PType(PTypeEnumeration.integer)))),
                       o => (int)((object[])o)[1])) { Scale = new ScaleCell(path + "decoding scale.pa")};
        }


       public virtual void ReCreate(KeyValuePair<int, int>[] edges)
        {
            Dictionary<int, HashSet<int>> direct;
            var roots = ReCreateDirectAndRoots(edges, out direct);

            maxChildrenCount = direct.Max(pair => pair.Value.Count);

            height = direct.Any() ? roots.Max(i => GetTreeHeight(i)) : 0;//edges.Count();

            if (roots.Count > maxChildrenCount)
                maxChildrenCount = roots.Count;

            codingIndex.IndexArray.Table.Clear();
            for (int i = 1; i < roots.Count + 1; i++)
            {
                CreateCoding(roots[i - 1], i, direct);
            }

            PType treePType = Enumerable.Range(0, height).
                Aggregate(new PType(PTypeEnumeration.integer),
                    (PType res, int level) =>
                        new PTypeRecord(new NamedType("value", new PType(PTypeEnumeration.integer)),
                            new NamedType("children", new PTypeSequence(res))));
            paCell = new PaCell(treePType, "../../tree.pa", false);
            ReCreate(direct);
       
            direct.Clear();
        }

       protected virtual List<int> ReCreateDirectAndRoots(KeyValuePair<int, int>[] edges, out Dictionary<int, HashSet<int>> direct)
       {
           var inverse = new HashSet<int>();

            direct=new Dictionary<int, HashSet<int>>();
           foreach (var pair in edges)
           {
               if (!direct.ContainsKey(pair.Key)) direct.Add(pair.Key, new HashSet<int>());
                // if (inverse.ContainsKey(pair.Value)) throw new Exception();
                direct[pair.Key].Add(pair.Value);
                if (!inverse.Contains(pair.Value))
                    inverse.Add(pair.Value);
            }


           var roots1 = new List<int>();
           roots1.Clear();
           roots1.AddRange(direct.Keys.Where(dirNode => !inverse.Contains(dirNode)));
           if (!roots1.Any()) throw new Exception("roots empty");
           return roots1;
       }

       protected virtual void CreateCoding(int node, int code, Dictionary<int, HashSet<int>> direct)
        {
            codingIndex.Build();
            codingIndex.Table.AppendValue(new object[] { node, code });
            decodingIndex.Table.AppendValue(new object[] { node, code });
            

            int i = 1;
        
           if(direct.ContainsKey(node))
            foreach (var child in direct[node])
            {
                CreateCoding(child, code* maxChildrenCount + (i++), direct);
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


        private void ReCreate(Dictionary<int, HashSet<int>> direct)
        {
            paCell.Clear();
            object content = null;
            if (height == 0)
            {
                content = 0;

            }
            else content = new object[] {0, new List<int>().Select(c => CreateTreeObject(c, 1, direct)).ToArray()};
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


       protected virtual object CreateTreeObject(int node, int level, Dictionary<int, HashSet<int>> direct)
       {
           return level != height
               ? (object)
                   new object[]
                   {
                       node,
                       direct.ContainsKey(node)
                           ? direct[node].Select(c => CreateTreeObject(c, level + 1, direct)).ToArray()
                           : new object[0]
                   }
               : node;
       }

       private int GetTreeHeight(int node) => new Dictionary<int, HashSet<int>>().ContainsKey(node)
           ? new Dictionary<int, HashSet<int>>()[node].Max(x1 => GetTreeHeight(x1) + 1)
           : 1;

        public int[]   GetChildren(int node)
        {
            if (paCell.Root.Type.Vid == PTypeEnumeration.integer)
                return new int[0];
            var paEntry = paCell.Root.Field(1).Element(0);
            if (searchIndex.ContainsKey(node))
              {
                paEntry.offset = searchIndex[node];
                var elements = paEntry.Field(1).Elements().Select(entry => (int)entry.Field(0).Get()).ToArray();
                return elements;
            }
            return new int[0];
        }




        public virtual bool TestConnection( int node1, int node2)
        {
            
            var codes1 = codingIndex.GetAllByKey(node1).ToArray();
            if (!codes1.Any()) return false;
            var codes2 = codingIndex.GetAllByKey(node2).ToArray(); 
            if (!codes2.Any()) return false;
            return TestConnectionByCodes((int) codes1[0].Get(), (int) codes2[0].Get());
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
            
           var code = codingIndex.GetAllByKey(node).ToArray();
           if (!code.Any())
                return Enumerable.Empty<int>();
            return GetParentsByCode((int) code[0].Get());
        }

       protected  virtual IEnumerable<int> GetParentsByCode(int code)
       {
           while ((code = code / maxChildrenCount) > 0)
           {
               yield return (int) decodingIndex.GetAllByKey(code).First().Get();
           }
       }

       public Tree<int> GetAllSubTree(int node)
                {
           var children = GetChildren(node);
           return new Tree<int>
           {
               Item = node,
               Children = children.Select(GetAllSubTree).ToArray()
           };
       }
    }

    public class Tree<T>
    {
        public T Item { get; set; }
        public Tree<T>[] Children { get; set; }
    }
}
