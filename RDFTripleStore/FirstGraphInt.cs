using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GoTripleStore;
using PolarDB;
using RDFCommon;
using RDFCommon.Interfaces;
using RDFCommon.OVns;
using RDFTurtleParser;
using Task15UniversalIndex;
using IGraph = RDFCommon.IGraph;

namespace RDFTripleStore
{
    public class FirstIntGraph : IGraph
    {
        private TableView table;

        private IndexDynamic<SPO_Troyka, IndexHalfkeyImmutable<SPO_Troyka>> spo_ind;
        private IndexDynamic<PO_Pair, IndexHalfkeyImmutable<PO_Pair>> po_ind;
        private IndexDynamic<PO_Pair, IndexHalfkeyImmutable<PO_Pair>> so_ind;
        private IndexDynamic<SP_Pair, IndexHalfkeyImmutable<SP_Pair>> sp_ind;
        private IndexDynamic<int, IndexKeyImmutable<int>> s_ind;
        private IndexDynamic<int, IndexKeyImmutable<int>> p_ind;
        private IndexDynamic<ObjectVariants, IndexHalfkeyImmutable<ObjectVariants>> o_ind;
        protected NodeGeneratorInt ng;
        public FirstIntGraph(string path)
        {
            PType tp_tabelement = new PTypeRecord(
             new NamedType("subject", new PType(PTypeEnumeration.integer)),
             new NamedType("predicate", new PType(PTypeEnumeration.integer)),
             new NamedType("obj", ObjectVariantsPolarType.ObjectVariantPolarType));
            Func<object, SPO_Troyka> spokeyproducer = v =>
                {                   
                    object[] va = (object[])((object[])v)[1];
                    return new SPO_Troyka((int)va[0], (int)va[1], va[2].ToOVariant(ng.coding_table.GetStringByCode)); //.ToComparable()
                };
            Func<object, PO_Pair> pokeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return new PO_Pair((int)va[1], va[2].ToOVariant(ng.coding_table.GetStringByCode));
            };
            Func<object, PO_Pair> oskeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return new PO_Pair((int)va[0], va[2].ToOVariant(ng.coding_table.GetStringByCode));
            };
            Func<object, SP_Pair> spkeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return new SP_Pair((int)va[0], (int) va[1]);
            };     
             
            Func<object, int> skeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return (int) va[0];
            };
            Func<object, int> pkeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return (int)va[1];
            };
            Func<object, ObjectVariants> okeyproducer = v =>
            {
                object[] va = (object[])((object[])v)[1];
                return va[2].ToOVariant(ng.coding_table.GetStringByCode);
            };     
            // Опорная таблица
            table = new TableView(path + "stable", tp_tabelement);
            if(table.Count()==0) table.Fill(new object[0]);
            ng = new NodeGeneratorInt(path + "coding", table.Count() == 0);
            // Индекс spo
            spo_ind = new IndexDynamic<SPO_Troyka, IndexHalfkeyImmutable<SPO_Troyka>>(false)
            {
                Table = table,
                IndexArray = new IndexHalfkeyImmutable<SPO_Troyka>(path + "spo_ind")
                {
                    Table = table,
                    KeyProducer = spokeyproducer  ,
                    HalfProducer = sp => sp.GetHashCode()
                },
                KeyProducer = spokeyproducer ,
            };
            po_ind = new IndexDynamic<PO_Pair, IndexHalfkeyImmutable<PO_Pair>>(false)
            {
                Table = table,
                IndexArray = new IndexHalfkeyImmutable<PO_Pair>(path + "po_ind")
                {
                    Table = table,
                    KeyProducer = pokeyproducer  ,
                    HalfProducer = sp => sp.GetHashCode()
                },
                KeyProducer = pokeyproducer
            };
            so_ind = new IndexDynamic<PO_Pair, IndexHalfkeyImmutable<PO_Pair>>(false)
            {
                Table = table,
                IndexArray = new IndexHalfkeyImmutable<PO_Pair>(path + "so_ind")
                {
                    Table = table,
                    KeyProducer = oskeyproducer,
                    HalfProducer = sp => sp.GetHashCode()    
                },
                KeyProducer = pokeyproducer
            };
            sp_ind = new IndexDynamic<SP_Pair, IndexHalfkeyImmutable<SP_Pair>>(false)
            {
                Table = table,
                IndexArray = new IndexHalfkeyImmutable<SP_Pair>(path + "sp_ind")
                {
                    Table = table,
                    KeyProducer = spkeyproducer,
                    HalfProducer = sp => sp.GetHashCode()
                },
                KeyProducer = spkeyproducer
            };
            s_ind = new IndexDynamic<int, IndexKeyImmutable<int>>(false)
            {
                Table = table,
                IndexArray = new IndexKeyImmutable<int>(path + "s_ind")
                {
                    Table = table,
                    KeyProducer = skeyproducer,
                },
                KeyProducer = skeyproducer
            };
            p_ind = new IndexDynamic<int, IndexKeyImmutable<int>>(false)
            {
                Table = table,
                IndexArray = new IndexKeyImmutable<int>(path + "p_ind")
                {
                    Table = table,
                    KeyProducer = pkeyproducer,
                },
                KeyProducer = pkeyproducer
            };
            o_ind = new IndexDynamic<ObjectVariants, IndexHalfkeyImmutable<ObjectVariants>>(false)
            {
                Table = table,
                IndexArray = new IndexHalfkeyImmutable<ObjectVariants>(path + "o_ind")
                {
                    Table = table,                                             
                    KeyProducer = okeyproducer,
                    HalfProducer = sp => sp.GetHashCode()
                },
                KeyProducer = okeyproducer
            };
        }

        public void Build(IGenerator<List<TripleStrOV>> generator)
        {
            table.Clear();
            table.Fill(new object[0]);
        
            ng.Build();
            generator.Start(list =>
            {
                IEnumerable<string> ids = list.SelectMany(tri =>
                {
                    IEnumerable<string> iris = new string[] { tri.Subject, tri.Predicate };
                    if (tri.Object.Variant == ObjectVariantEnum.Iri)
                        iris = iris.Concat(new string[] { ((OV_iri)tri.Object).UriString });
                    if (tri.Object.Variant == ObjectVariantEnum.OtherIntType)
                        iris = iris.Concat(new string[] { ((OV_typed)tri.Object). DataType});
                    return iris;
                });
                var dictionary = ng.coding_table.InsertPortion(ids);
                foreach (var tri in list)
                {
                    int isubj = dictionary[tri.Subject];
                    int ipred = dictionary[tri.Predicate];
                    ObjectVariants ov = tri.Object;
                    if (ov.Variant == ObjectVariantEnum.Iri)
                    {
                        int iobj = dictionary[((OV_iri)ov).UriString];
                        ov = new OV_iriint(iobj, ng.coding_table.GetStringByCode);
                    }
                    table.TableCell.Root.AppendElement(new object[] { false, new object[] { isubj, ipred, ov.ToWritable() } });
                }
                });
               
            ng.coding_table.BuildScale();
            table.TableCell.Flush();
            spo_ind.IndexArray.Build();
            po_ind.IndexArray.Build();
            so_ind.IndexArray.Build();
            sp_ind.IndexArray.Build();
            s_ind.IndexArray.Build();
            p_ind.IndexArray.Build();
            o_ind.IndexArray.Build();
            spo_ind.Build();
            po_ind.Build();
            so_ind.Build();
            sp_ind.Build();
            s_ind.Build();
            p_ind.Build();
            o_ind.Build();
        }


        public string Name { get { return "g"; } }
        public NodeGenerator NodeGenerator { get { return ng; }} 

      


        public IEnumerable<T> GetTriplesWithObject<T>(ObjectVariants o, Func<ObjectVariants,ObjectVariants, T> returns)
        {
            return o_ind.GetAllByKey(((ObjectVariants)o))
                .Select(entry => entry.Get())
                .Cast<object[]>()
                .Select(row => returns(ReadSubject(((object[])row[1])), ReadPredicate(((object[])row[1]))));
        }

        private ObjectVariants ReadPredicate(object[] row)
        {
            return ng.GetCoded((int)row[1]);
        }

        private ObjectVariants ReadSubject(object[] row)
        {
            return ng.GetCoded((int)row[0]);
        }
        private ObjectVariants ReadObject(object[] row)
        {
            return ((int)row[2]).ToOVariant(ng.coding_table.GetStringByCode);
        }

        public IEnumerable<T> GetTriplesWithPredicate<T>(ObjectVariants p, Func<ObjectVariants, ObjectVariants, T> returns)
        {
            return p_ind.GetAllByKey(((OV_iriint) p).code)
                .Select(entry => entry.Get())
                .Cast<object[]>()
                .Select(row => returns(ReadSubject(((object[])row[1])), ReadObject(((object[])row[1]))));
        }

        public IEnumerable<T> GetTriplesWithSubject<T>(ObjectVariants s, Func<ObjectVariants, ObjectVariants, T> returns)
        {
            return s_ind.GetAllByKey(((OV_iriint) s).code)
                .Select(entry => entry.Get())
                .Cast<object[]>()
                .Select(row => returns(ReadPredicate(((object[])row[1])), ReadSubject(((object[])row[1]))));
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subject, ObjectVariants predicate)
        {
            IEnumerable<PaEntry> entities = sp_ind.GetAllByKey(new SP_Pair(((OV_iriint)subject).code, ((OV_iriint)predicate).code));
            return entities
                 .Select(entry => entry.Get())
                .Cast<object[]>()
                .Select(row => ((object[])row[1])[2].ToOVariant(ng.coding_table.GetStringByCode));
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subject, ObjectVariants obj)
        {
            int ssubj = (((OV_iriint)subject)).code;
            var objVar = (((ObjectVariants)obj));
            var key_triple = new PO_Pair(ssubj, objVar);
            IEnumerable<PaEntry> entities = so_ind.GetAllByKey(key_triple);
            return entities
                .Select(entry => entry.Get())
                .Cast<object[]>()
                .Select(row => ng.GetCoded((int)((object[])row[1])[1]));
        }

        public IEnumerable<ObjectVariants> GetTriplesWithPredicateObject(ObjectVariants predicate, ObjectVariants obj)
        {
            int pred = (((OV_iriint)predicate)).code;
            var objVar = (((ObjectVariants)obj));
            var key_triple = new PO_Pair(pred, objVar);
            IEnumerable<PaEntry> entities = po_ind.GetAllByKey(key_triple);
            return entities
                  .Select(entry => entry.Get())
                  .Cast<object[]>()
                  .Select(row => ng.GetCoded((int)((object[])row[1])[0]));
        }


        public bool Contains(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
        {
            int ssubj = (((OV_iriint)subject)).code;
            int pred = (((OV_iriint)predicate)).code;
            var objVar = (((ObjectVariants)obj));
            var key_triple = new SPO_Troyka(ssubj, pred, objVar);
            IEnumerable<PaEntry> entities = spo_ind.GetAllByKey(key_triple);
            return entities.Any();
        }
        public IEnumerable<T> GetTriples<T>(Func<ObjectVariants,ObjectVariants,ObjectVariants,T>returns )
        {
            return table.TableCell.Root.ElementValues()
                .Cast<object[]>()
                .Select(ent => returns(ReadSubject(ent), ReadPredicate(ent), ReadObject(ent)));
        }

        public void Add(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
            table.AppendValue(new object[] { ((OV_iriint)s).code, ((OV_iriint)p).code, ((ObjectVariants)o).WritableValue });
        }
        public void Clear()
        {
            table.Clear();
        }
        public void Insert(IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> triples)
        {
            foreach (var triple in triples)
                Add(triple);
        }

        public void Add(Triple<ObjectVariants, ObjectVariants, ObjectVariants> t)
        {
            Add(t.Subject, t.Predicate, t.Object);
        }




        public void Delete(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {         
            int ssubj = (((OV_iriint) s)).code;
            int pred = (((OV_iriint) p)).code;
            if (o is OV_iri) return;
            var key_triple = new SPO_Troyka(ssubj, pred, o);
            IEnumerable<PaEntry> entities = spo_ind.GetAllByKey(key_triple);
            foreach (var ent in entities)
                table.DeleteEntry(ent);
        }

        public IEnumerable<ObjectVariants> GetAllSubjects()
        {
            return new HashSet<int>(table.TableCell.Root.ElementValues()
                .Cast<object[]>()
                .Select(t => t[0])
                .Cast<int>())
                .Select(s => ng.GetCoded(s)); 
        }

        public long GetTriplesCount()
        {
            return table.Count();
        }

        public bool Any()
        {
            return table.Count() > 0;
        }

        public void FromTurtle(string path)
        {
            table.Clear();
            Build(new TripleGeneratorBufferedParallel(path, "g"));
            
        }

        void IGraph.FromTurtle(Stream inputStream)
        {
            FromTurtle(inputStream);
        }

        public void Warmup()
        {
                  table.Warmup();
            s_ind.Warmup();
            sp_ind.Warmup();
            spo_ind.Warmup();
            so_ind.Warmup();
            po_ind.Warmup();
            p_ind.Warmup();
            o_ind.Warmup();
        }

        protected void FromTurtle(Stream baseStream)
        {
            table.Clear();
            Build(new TripleGeneratorBufferedParallel(baseStream, "g"));
        }

      
    }
}
