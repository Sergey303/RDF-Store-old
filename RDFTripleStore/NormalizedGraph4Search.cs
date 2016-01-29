using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolarDB;

namespace RDFTripleStore
{
   public class NormalizedGraph4Search : Tree4Search
   {

       private readonly Dictionary<int, List<int>> otherParentsCoding = new Dictionary<int, List<int>>();

   

       protected override void CreateCoding(int node, int code)
        {
           if (coding.ContainsKey(node))
           {
               List<int> codes;
               if (otherParentsCoding.TryGetValue(node, out codes))
                   codes.Add(code);
               else otherParentsCoding.Add(node, new List<int>() {code});
           }
           else
           {
               base.CreateCoding(node, code);
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

       protected override object CreateTreeObject(int node, int level)
       {
           if (level != height)
           {
               var treeObject = new object[2];
               treeObject[0] = node;
               if (direct.ContainsKey(node))
               {
                   var objects = direct[node].Select(c => CreateTreeObject(c, level + 1)).ToArray();
                   treeObject[1] = objects;
                   direct.Remove(node);
               }
               else treeObject[1] = new object[0];
               return treeObject;
           }
           else return node;
       }

       public override bool TestConnection( int node1, int node2)
        {
            int code1, code2;
            if (!coding.TryGetValue(node1, out code1)) return false;
            if (!coding.TryGetValue(node2, out code2)) return false;
            if (TestConnectionByCodes(code2, code1)) return true;

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
   }

  }
