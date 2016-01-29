using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFTripleStore
{
   public struct Tree<T>
   {
       public T Item;
       public Tree<T>[] Children;
   }
}
