using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolarDB;
using UniversalIndex;

namespace SimpleTripleStore
{
    public class SimpleTripleStore
    {
        private readonly TableView table;
        private readonly IndexCascadingDynamic<int> ps_index;
        private readonly IndexCascadingDynamic<string> po_index;
        private readonly int portion;
        Dictionary<string, int> predicatemapping=new Dictionary<string, int>();
        private string predicatemappingPath;
        
        public SimpleTripleStore(string path, int portion)
        {
            this.portion = portion;

            table=new TableView(path+"table.pac", new PTypeRecord(new NamedType("s", new PType(PTypeEnumeration.integer)),
                new NamedType("p", new PType(PTypeEnumeration.integer)),
                new NamedType("o", new PType(PTypeEnumeration.sstring))));
            ps_index = new IndexCascadingDynamic<int>(path + "ps_index",
                    table,
                    ob => (int)((object[])((object[])ob)[1])[1],
                    ob => (int)((object[])((object[])ob)[1])[0],
                    i => i);
            po_index = new IndexCascadingDynamic<string>(path + "po_index",
                table,
                ob => (int) ((object[]) ((object[]) ob)[1])[1],
                ob => (string) ((object[]) ((object[]) ob)[1])[2],
                s => s.GetHashSpooky());
            predicatemappingPath = path + "predicates";
            if(File.Exists(predicatemappingPath))
            using (StreamReader file = new StreamReader(predicatemappingPath))
                while (!file.EndOfStream)
                {
                    predicatemapping.Add(file.ReadLine(), predicatemapping.Count);
                }
            if (table.TableCell.IsEmpty) return;
            po_index.CreateDiscaleDictionary();
            ps_index.CreateDiscaleDictionary();
            table.Warmup();
            po_index.Warmup();
            ps_index.Warmup();
        }

        public void Build(IEnumerable<Tuple<int, string, string>>  tripleFlow)
        {
            table.Clear();
            table.Fill(new object[0]);

            File.Delete(predicatemappingPath);
            int portionConter=0;
            object[] buffer=new object[portion];
            ps_index.index_arr.FillInit();
            po_index.index_arr.FillInit();
            TableRow[] tableRows;
            foreach (var triple in tripleFlow)
            {
                if (portionConter == portion)
                {
                    tableRows = table.Add(buffer).ToArray();
                    ps_index.index_arr.FillPortion(tableRows);
                    po_index.index_arr.FillPortion(tableRows);
                    Array.Clear(buffer,0, portion);
                    portionConter = 0;
                }
                int pCode;
                if (!predicatemapping.TryGetValue(triple.Item2, out pCode))
                    predicatemapping.Add(triple.Item2, pCode = predicatemapping.Count);
                buffer[portionConter] = new object[] {triple.Item1, pCode, triple.Item3};
                portionConter++;
            }
            tableRows = table.Add(buffer).ToArray();
            ps_index.index_arr.FillPortion(tableRows);
            po_index.index_arr.FillPortion(tableRows);
            table.TableCell.Flush();
            ps_index.index_arr.FillFinish();
            po_index.index_arr.FillFinish();
            ps_index.Build();
            po_index.Build();
            using (StreamWriter file = new StreamWriter(predicatemappingPath))
                foreach (var i in predicatemapping)
                {
                    file.WriteLine(i.Key);
                }
            po_index.CreateDiscaleDictionary();
            ps_index.CreateDiscaleDictionary();
        }

        public IEnumerable<Tuple<string, string>> GetDirects(int subject)
        {
            return ps_index.GetRecordsWithKey2(subject).Select(entry =>
            {
                var row = (object[]) entry;
                return Tuple.Create(predicatemapping.Keys.ElementAt((int) row[1]), (string) row[2]);
            });
        }

        public IEnumerable<int> GetSubjects(string predicate, string @object)
        {
            return po_index.GetRecordsWithKeys(predicatemapping[predicate], @object)
                .Cast<object[]>()
                .Select(o => o[0])
                .Cast<int>();
        }

        public IEnumerable<string> GetObject(int subject, string predicate)
        {
            return ps_index.GetRecordsWithKeys(predicatemapping[predicate], subject)
               .Cast<object[]>()
               .Select(o => o[2])
               .Cast<string>();
        }
    }
}
