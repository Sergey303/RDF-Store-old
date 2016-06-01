using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolarDB;
using RDFCommon.OVns;
using UniversalIndex;

namespace RDFTripleStore
{
   public class TextObjectIndex 
    {

       private DictionaryLong<string, List<long>> nametable;
        //TableView spOffset;
        //private TableView Strings;
        public TextObjectIndex(ulong count , RDFGraph graph)
        {
            nametable = new DictionaryLong<string, List<long>>(s => s.GetULongHashSpooky(), count);
        }
        public void FillPortion(IEnumerable<TableRow> rows)
        {
            foreach (var row in
                    rows.Select(row => new {ov = (((object[]) ((object[]) row.Row)[1]))[2].ToOVariant(),
                                            offset = row.Offset})
                        .Where(arg => arg.ov.Variant == ObjectVariantEnum.Str))
            {
                List<long> list;
                var s = row.ov.ToString();
                if (nametable.TryGetValue(s, out list))
                    list.Add(row.offset);
                else nametable.Add(s, new List<long>());
            }
        }

      public List<long> FindText(string s)
      {
          List<long> list;
          return nametable.TryGetValue(s, out list) ? list : new List<long>();
      }
    }
}
