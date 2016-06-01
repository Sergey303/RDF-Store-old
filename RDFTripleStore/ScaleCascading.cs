using System;
using System.Collections.Generic;
using System.Linq;
using PolarDB;
using Task15UniversalIndex;

namespace RDFTripleStore
{
    public class ScaleCascadingPa<Tkey> : IIndexCommon where Tkey : IComparable
    {
        //Решением является "ячеечное" построение шкалы. Но я хочу это сделать в виде одной ячейки. Это делается на основе следующего наблюдения: не рационально делать длину массива шкалы больше длины отсортированного массива, который шкалируется. Более того, по умолчанию, принят фактор 32 - т.е. длина массива шкалы в 32 раза меньше, чем у индексного массива. Но пусть они совпадают. Это означает, что мы в одну ячейку шкалы можем "запихать" все массивчики шкал, разнящихся значеним первого ключа. Будет даже простая арифметика вычисления места нахождения отдельной подшкалы. Единственная проблема в том, что я не смог проанализировать логику построения комбинированного массива шкал для случая, когда мы вводим фактор (напр. 32), т.е. пропорциональное уменьшение количества элементов в подшкалах. В таком уменьшении проблем не видно, но формулы и логика должны быть правильными.
        // Tkey - тип второго ключа, int - тип первого ключа
        private PaCell index_cell;
        public PaCell IndexCell { get { return index_cell; } }
        private PaCell groups_index;
        public ScaleCascadingPa(string path_name, Func<Tkey, int> half2Producer)
        {
            Half2Producer = half2Producer;
        }

        public Func<object, int> Key1Producer { get; set; }
        public Func<object, int> Key2Producer { get; set; }
        internal class GroupElement : IComparable
        {
            public int Key1 { get { return this.key1; } }
            public int HKey2 { get { return this.hkey2; } }
            private int key1;
            private int hkey2;
            private Func<Tkey> GetKey2 = null;
            internal GroupElement(int key1, int hkey2, Func<Tkey> getkey2)
            {
                this.key1 = key1;
                this.hkey2 = hkey2;
                this.GetKey2 = getkey2;
            }
            private bool key2exists = false;
            private Tkey key2;
            private Tkey Key2
            {
                get
                {
                    if (!key2exists)
                    {
                        key2exists = true;
                        key2 = GetKey2();
                    }
                    return key2;
                }
            }
            public int CompareTo(object obj)
            {
                GroupElement rec = (GroupElement)obj;
                int cmp = key1.CompareTo(rec.Key1);
                if (cmp != 0) return cmp;
                cmp = hkey2.CompareTo(rec.HKey2);
                if (cmp != 0) return cmp;
                //Tkey key2 = GetKey2();
                //cmp = key2.CompareTo(rec.GetKey2());
                cmp = Key2.CompareTo(rec.Key2);
                return cmp;
            }
        }

        // Если не найден, то будет Diapason.Empty
        public Diapason GetDiapasonByKey1(int key1)
        {
            Tuple<Diapason, ScaleInMemory> tup;
            if (gr_discale.TryGetValue(key1, out tup))
            {
                return tup.Item1;
            }
            else return Diapason.Empty;
        }
        private Diapason GetLocalDiapason(int key1, Tkey key2)
        {
            Tuple<Diapason, ScaleInMemory> tup;
            var groupIndex = scaleF(key1);
            if (groupIndex > Count || groupIndex < 0) return Diapason.Empty;
            var groupCell = index_cell.Root.Element(groupIndex);
            if (groupCell.IsEmpty) return Diapason.Empty;

            long min = (long)groupCell.Element(0).Get();
            long max = (long)groupCell.Element(0).Get();
            long n_scale = groupCell.Count();
            long ind = (int)(((long)key1 - (long)min) * (long)(n_scale - 1) / (long)((long)max - (long)min));
            long sta = (long)groupCell.Element(ind+2).Get(); ;
            long num = ind < n_scale - 1 ? (long)groupCell.Element(ind + 1).Get() - sta : Count - sta + start;
                return new Diapason() { start = sta, numb = num };
            if (gr_discale.TryGetValue(key1, out tup))
            {
                int hk = Half2Producer(key2);
                return tup.Item2.GetDiapason(hk);
            }
            else return Diapason.Empty;
        }
        public IEnumerable<object> GetAllByKeys(int key1, Tkey key2)
        {
            Diapason dia = GetLocalDiapason(key1, key2);
            return GetAllInDiap(dia, key2);
        }
        public IEnumerable<int> GetKey1All()
        {
            return gr_discale.Keys;
        }
        /// <summary>
        /// Получение триплетов из диапазона, соответствующих заданному второму ключу
        /// </summary>
        /// <param name="dia">Диапазон в индексном массиве. Есть специальные требования к диапазону</param>
        /// <param name="key2">Ключ2 по которому фильтруется результарующее множество</param>
        /// <returns>Возвращает поток триплетов в объектной форме</returns>
        public IEnumerable<object> GetAllInDiap(Diapason dia, Tkey key2)
        {
            if (dia.IsEmpty()) return Enumerable.Empty<object>();
            int hkey = Half2Producer(key2);
            PaEntry entry = Table.Element(0);
            var query1 = index_cell.Root.BinarySearchAll(dia.start, dia.numb, ent =>
            {
                object[] va = (object[])ent.Get();
                int hk = (int)va[2];
                int cmp = hk.CompareTo(hkey);
                return cmp;
            }).Select(ent => ent.Get())
                .Where(va => (int)((object[])va)[2] == hkey);
            var query2 = index_cell.Root.ElementValues(dia.start, dia.numb)
                .Where(va => (int)((object[])va)[2] == hkey);
            IEnumerable<object> query = dia.numb > 30 ? query1 : query2;

            return query
                .Select(va =>
                {
                    long off = (long)((object[])va)[0];
                    entry.offset = off;
                    return entry.Get();
                })
                .Where(two => !(bool)((object[])two)[0] && Key2Producer(two).CompareTo(key2) == 0)
                .Select(two => ((object[])((object[])two)[1]));
        }
        public IEnumerable<object> GetAllInDiap(Diapason dia)
        {
            if (dia.IsEmpty()) return Enumerable.Empty<object>();
            PaEntry entry = Table.Element(0);

            var query2 = index_cell.Root.ElementValues(dia.start, dia.numb)
                .Select(va =>
                {
                    long off = (long)((object[])va)[0];
                    entry.offset = off;
                    return entry.Get();
                })
                .Where(two => !(bool)((object[])two)[0])
                .Select(two => ((object[])((object[])two)[1]));
            return query2;
        }
        // Получение потока всех элементов в отсортированном виде
        // Альтернатива - выдача всех записей из таблицы Table
        public IEnumerable<object> GetAll()
        {
            PaEntry entry = Table.Element(0);
            return index_cell.Root.ElementValues()
                .Select(va =>
                {
                    long off = (long)((object[])va)[0];
                    entry.offset = off;
                    return entry.Get();
                })
                .Where(two => !(bool)((object[])two)[0]) // Проверка на неуничтоженность
                .Select(two => ((object[])((object[])two)[1]));
        }
        public void Build()
        {
            index_cell.Clear();
            index_cell.Fill(new object[0]);
            if (Key1Producer == null) throw new Exception("Err: Key1Producer not defined");
            if (Key2Producer == null) throw new Exception("Err: Key2Producer not defined");
            Table.Scan((offset, o) =>
            {
                var k1 = Key1Producer(o);
                var k2 = Key2Producer(o);
                int hk2 = Half2Producer(k2);
                index_cell.Root.AppendElement(new object[] { offset, k1, hk2 });
                return true;
            });
            index_cell.Flush();

            PaEntry entry = Table.Element(0);
            index_cell.Root.SortByKey<GroupElement>(ob =>
                new GroupElement((int)((object[])ob)[1], (int)((object[])ob)[2], () =>
                {
                    long off = (long)((object[])ob)[0];
                    entry.offset = off;
                    return Key2Producer(entry.Get());
                }));
            // BuildGroupsIndexSpecial:
            groups_index.Clear();
            groups_index.Fill(new object[0]);
            int key1 = Int32.MinValue;
            int i = 0; // Теоретически, здесь есть проблема в том, что элементы могут выдаватьс не по индексу.
            foreach (object[] va in index_cell.Root.ElementValues())
            {
                int k1 = (int)va[1];
                if (k1 > key1)
                {
                    groups_index.Root.AppendElement(i);
                    key1 = k1;
                }
                i++;
            }
            groups_index.Flush();
            //CreateGroupDictionary();
            CreateDiscaleDictionary();
        }
        void Warmup() { throw new NotImplementedException("in IndexCascadingImmutable"); }
        // Второй вариант группового словаря: получаем пару - диапазон и ScaleInMemory
        private Dictionary<int, Tuple<Diapason, ScaleInMemory>> gr_discale = null;
        private Func<Tkey, int> Half2Producer;
        private PaEntry Table;
        private Func<int, int> scaleF;
        private int Count;

        public void CreateDiscaleDictionary()
        {
            gr_discale = new Dictionary<int, Tuple<Diapason, ScaleInMemory>>();
           
            long start0 = -1;
            long start = -1;
            int key = -1;
            long sta = -1, num = -1, nscale = -1;
            foreach (int ind in groups_index.Root.ElementValues())
            {
                start = ind;
                long off = (long)index_cell.Root.Element(ind).Field(0).Get();
                entry.offset = off;
                if (key != -1)
                {
                    sta = start0;
                    num = start - start0;
                    nscale = num / 32;
                    ScaleInMemory sim = new ScaleInMemory(index_cell.Root, sta, num, ob => (int)((object[])ob)[2], (int)nscale);
                    sim.Build();
                    gr_discale.Add(key, new Tuple<Diapason, ScaleInMemory>(
                        new Diapason() { start = sta, numb = num }, sim));
                }
                key = Key1Producer(entry.Get());
                start0 = start;
            }
            sta = start0;
            num = index_cell.Root.Count() - start0;
            nscale = num / 32;
            ScaleInMemory sim0 = new ScaleInMemory(index_cell.Root, sta, num, ob => (int)((object[])ob)[2], (int)nscale);
            sim0.Build();
            gr_discale.Add(key, new Tuple<Diapason, ScaleInMemory>(
                new Diapason() { start = sta, numb = num }, sim0));
        }

        public void OnAppendElement(PolarDB.PaEntry entry) { throw new NotImplementedException(); }
        public void DropIndex() { throw new NotImplementedException(); }
    }
}
