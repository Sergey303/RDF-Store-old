using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PolarDB;
using RDFCommon;
using RDFCommon.Interfaces;
using RDFCommon.OVns;
using RDFTurtleParser;
using UniversalIndex;


namespace RDFTripleStore
{
    public class GraphCascadingInt :  IGraph
    {

        public void Add(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
         //po_index.
        }
        public bool Contains(ObjectVariants subj, ObjectVariants pred, ObjectVariants obj)
        {
            return GetTriplesWithSubjectPredicate(subj, pred)
                .Any(o => o.CompareTo(obj) == 0);
        }

        public void Delete(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
        {
        
        }

        public IEnumerable<ObjectVariants> GetAllSubjects()
        {
            return
                ps_index.GetRecordsAll()
                    .Cast<object[]>()
                    .Select(row => (int) row[0])
                    .Distinct()
                    .Select(uri => NodeGenerator.GetUri(uri));
        }

        public long GetTriplesCount()
        {
            return table.Elements().Count(row => !(bool) row.Field(0).Get());
        }

        public bool Any()
        {
            return table.Elements().Any(row => !(bool)row.Field(0).Get());            
        }

        public void FromTurtle(string gString)
        {
            Build(new TripleGeneratorBuffered(gString,null));
            NodeGenerator.Build();
        }

        public void FromTurtle(Stream inputStream)
        {
            Build(new TripleGeneratorBufferedParallel(inputStream, null));
            NodeGenerator.Build();
            
        }

        public IEnumerable<T> GetTriples<T>(Func<ObjectVariants,ObjectVariants,ObjectVariants, T> returns)
        {
            if (Table.TableCell.Root.Count()==0) return Enumerable.Empty<T>();
            return ps_index.GetRecordsAll()
                .Cast<object[]>()   
                .Select(rec => returns(NodeGenerator.GetUri(rec[0]), NodeGenerator.GetUri(rec[1]), rec[2].ToOVariant(NodeGenerator)));
        }

  
        public IEnumerable<TripleOVStruct> GetTriplesWithSubject(ObjectVariants subj)
        {
            return ps_index.GetRecordsWithKey2(((OV_iriint)subj).code)
                .Cast<object[]>()

                 .Select(rec => new TripleOVStruct(null, NodeGenerator.GetUri(rec[1]), rec[2].ToOVariant(NodeGenerator)));
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            return ps_index.GetRecordsWithKeys(((OV_iriint)pred).code, ((OV_iriint)subj).code)
                .Cast<object[]>()
                
                .Select(rec => rec[2].ToOVariant(NodeGenerator));
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subj, ObjectVariants obj)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<ObjectVariants> GetSubjects(ObjectVariants pred, ObjectVariants obj)
        {
            return po_index.GetRecordsWithKeys(((OV_iriint) pred).code, obj)
                .Cast<object[]>()

                .Select(rec => NodeGenerator.GetUri(rec[0]));
        }
        public IEnumerable<TripleOVStruct> GetTriplesWithObject(ObjectVariants obj)
        {
            return po_index.GetRecordsWithKey2(obj)
                .Cast<object[]>()
                .Select(rec => new TripleOVStruct(NodeGenerator.GetUri(rec[0]), NodeGenerator.GetUri(rec[1]), null));
        }

        public IEnumerable<TripleOVStruct> GetTriplesWithPredicate(ObjectVariants pred)
        {
            return ps_index.GetRecordsWithKey1(((OV_iriint)pred).code)
                .Cast<object[]>()                                                                
                             .Select(rec => new TripleOVStruct(NodeGenerator.GetUri(rec[0]), null, rec[2].ToOVariant(NodeGenerator)));
        }


        public TableView table;
        public TableView Table { get { return table; } }
        private IndexCascadingDynamic<int> ps_index;
        private IndexCascadingDynamic<ObjectVariants> po_index;
        public GraphCascadingInt(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            PType tp_triple = new PTypeRecord(
                new NamedType("subj", new PType(PTypeEnumeration.integer)),
                new NamedType("pred", new PType(PTypeEnumeration.integer)),
                new NamedType("obj", ObjectVariantsPolarType.ObjectVariantPolarType));
            table = new TableView(path + "triples", tp_triple);
            ps_index = new IndexCascadingDynamic<int>(path + "ps_index",
                table,
                ob => (int)((object[])((object[])ob)[1])[1],
                ob => (int)((object[])((object[])ob)[1])[0],
                i => i);
            po_index = new IndexCascadingDynamic<ObjectVariants>(path + "po_index",
                table,
                ob => (int)((object[])((object[])ob)[1])[1],
                ob => ((object[])((object[])ob)[1])[2].ToOVariant(NodeGenerator),
                ov => ov.GetHashCode());
        }
        public void Start() 
        { 
            ps_index.CreateDiscaleDictionary();
            po_index.CreateDiscaleDictionary();
        }
        

        public string Name { get; set; }
        public NodeGenerator NodeGenerator { get; set; }
        
      

        public void Warmup() { table.Warmup(); }

        public void Build(IGenerator<List<TripleStrOV>> generator)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
          
            table.Clear();
            table.Fill(new object[0]);
            var ng = NodeGenerator as NodeGeneratorInt;
          //  ng.coding_table.Clear();

            generator.Start(ProcessPortion);
            table.TableCell.Flush();
             if(!table.TableCell.Root.Elements().Any()) return;

            sw.Stop();
            Console.WriteLine("Load data and nametable ok. Duration={0}", sw.ElapsedMilliseconds);
            sw.Restart();
            
            ng.Build();
            ps_index.Build();

            sw.Stop();
            Console.WriteLine("ps_index.Build() ok. Duration={0}", sw.ElapsedMilliseconds);
            sw.Restart();

         //   po_index.Build();
            
            sw.Stop();
            Console.WriteLine("Build index ok. Duration={0}", sw.ElapsedMilliseconds);
            sw.Restart();
        }
        public void ActivateCache()
        {
            var ng = NodeGenerator as NodeGeneratorInt;
            ng.coding_table.ActivateCache();
            table.ActivateCache();
            ps_index.ActivateCache();
         //   po_index.ActivateCache();
        }
        private void ProcessPortion(List<TripleStrOV> buff)
        {
            // Пополнение таблицы имен
            var ng = NodeGenerator as NodeGeneratorInt;
            var dic = ng.coding_table.InsertPortion(buff.SelectMany(t =>
            {
                ObjectVariants ov = t.Object;
                if(ov==null) {Console.WriteLine(t.Subject);
                    return Enumerable.Empty<string>();
                } 
                else
                if (ov.Variant == ObjectVariantEnum.Iri)
                {
                    return new string[] { t.Subject, t.Predicate, ((OV_iri)ov).Name };
                }
                else
                {
                    return new string[] { t.Subject, t.Predicate };
                }
            }));
            // Пополнение триплетов
            table.Add(buff.Where(t=>t.Object!=null).Select(t =>
            {
                ObjectVariants ov = t.Object;
                if (ov.Variant == ObjectVariantEnum.Iri)
                {
                    OV_iriint ovi = new OV_iriint(dic[((OV_iri)ov).Name], ng.coding_table.GetStringByCode);
                    return new object[] { dic[t.Subject], dic[t.Predicate], ovi.ToWritable() };
                }
                else
                {
                    return new object[] { dic[t.Subject], dic[t.Predicate], ov.ToWritable() };
                }
            }));
        }
        public void Build(IEnumerable<TripleStrOV> triples)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            int portion = 1000000;
            List<TripleStrOV> buff = new List<TripleStrOV>();
            foreach (TripleStrOV tri in triples)
            {
                buff.Add(tri);
                if (buff.Count >= portion)
                {
                    ProcessPortion(buff);
                    buff.Clear();
                }
            }
            if (buff.Count > 0) ProcessPortion(buff);
            table.TableCell.Flush();

         

            sw.Stop();
            Console.WriteLine("Load data and nametable ok. Duration={0}", sw.ElapsedMilliseconds);
            sw.Restart();

            ps_index.Build();

            sw.Stop();
            Console.WriteLine("ps_index.Build() ok. Duration={0}", sw.ElapsedMilliseconds);
            sw.Restart();

            po_index.Build();

            sw.Stop();
            Console.WriteLine("Build index ok. Duration={0}", sw.ElapsedMilliseconds);
            sw.Restart();
        }

        public void Clear()
        {
            table.Clear();
            table.Fill(new object[0]);
            var ng = NodeGenerator as NodeGeneratorInt;
            ng.Clear();
            ps_index.DropIndex();
            po_index.DropIndex();
        }
    }
}
