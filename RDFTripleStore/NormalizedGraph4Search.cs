using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolarDB;
using UniversalIndex;

namespace RDFTripleStore
{
   public class NormalizedGraph4Search : Tree4Search
   {

        protected readonly IndexDynamic<int, IndexToSortableTableImmutable<int>> otherParentsCoding;

        protected override void CreateCoding(int node, int code, Dictionary<int, HashSet<int>> direct)
       {
           var codePa = codingIndex.GetAllByKey(node).ToArray();
           if (codePa.Any())
           {
             otherParentsCoding.Table.AppendValue(new object[]{ node, code});
           }
           else
           {
               base.CreateCoding(node, code, direct);
           }

        }

        protected override void AddInIndex(PaEntry entry)
        {
            int node;
            if (entry.Type.Vid == PTypeEnumeration.integer)
            {
                node = (int) entry.Get();
                if (!searchIndex.ContainsKey(node))
                    searchIndex.Add(node, entry.offset);
            }
            else
            {
                node = (int) entry.Field(0).Get();
                
                if (!searchIndex.ContainsKey(node))
                    searchIndex.Add(node, entry.offset);
                foreach (var chiledEntry in entry.Field(1).Elements())
                {
                    AddInIndex(chiledEntry);
                }
            }
        }

       protected override object CreateTreeObject(int node, int level, Dictionary<int, HashSet<int>> direct)
       {
           if (level != height)
           {
               var treeObject = new object[2];
               treeObject[0] = node;
               var direct1 = new Dictionary<int, HashSet<int>>();
               if (direct1.ContainsKey(node))
               {
                   var objects = direct1[node].Select(c => CreateTreeObject(c, level + 1, direct1)).ToArray();
                   treeObject[1] = objects;
                   direct1.Remove(node);
               }
               else treeObject[1] = new object[0];
               return treeObject;
           }
           else return node;
       }

       public override bool TestConnection( int node1, int node2)
        {
            var codes1 = codingIndex.GetAllByKey(node1).ToArray();
            if (!codes1.Any()) return false;
            var codes2 = codingIndex.GetAllByKey(node2).ToArray();
            if (!codes2.Any()) return false;
           int code1 = (int) codes1[0].Get();
           int code2 = (int) codes1[0].Get();
            List<int> others1, others2;
            if (otherParentsCoding.TryGetValue(node1, out others1))
            {
                if (otherParentsCoding.TryGetValue(node2, out others2))
                    return others1.Any(c1 => base.TestConnectionByCodes(c1, code2)) ||
                    others2.Any(c2 => base.TestConnectionByCodes(code1, c2)) ||
                    others1.Any(c1 => others2.Any(c2=>TestConnectionByCodes(c1, code2)));
                else
                    return others1.Any(c1 => TestConnectionByCodes(c1, code2));
            }
            else if (otherParentsCoding.TryGetValue(node2, out others2))
                return others2.Any(c2 => TestConnectionByCodes(code1, c2));
            return false;
        }

       public IEnumerable<int[]> GetParents(int node)
       {
            yield return base.GetParents(node).ToArray();

            List<int> codes;
           if (otherParentsCoding.TryGetValue(node, out codes))
               foreach (var result in codes.Select(i => GetParentsByCode(i).ToArray()))
                   yield return result;

       }

       public NormalizedGraph4Search(string path) : base(path)
       {
       }
   }

  }
