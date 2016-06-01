using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;

namespace RDFTripleStore
{
    public class GraphCached : IGraph
    {
        private readonly IGraph @base;
        private readonly Cache<ObjectVariants, ObjectVariants, ObjectVariants, bool> spo;
        private readonly Cache<ObjectVariants, ObjectVariants, IEnumerable<ObjectVariants>> spO;
        private readonly Cache<ObjectVariants, ObjectVariants, IEnumerable<ObjectVariants>> sPo;
        private readonly Cache<ObjectVariants, ObjectVariants, IEnumerable<ObjectVariants>> Spo;
        private readonly Cache<ObjectVariants, IEnumerable<KeyValuePair<ObjectVariants, ObjectVariants>>> SPo;
        private readonly Cache<ObjectVariants, IEnumerable<KeyValuePair<ObjectVariants, ObjectVariants>>> SpO;
        private readonly Cache<ObjectVariants, IEnumerable<KeyValuePair<ObjectVariants, ObjectVariants>>> sPO;


        public GraphCached(IGraph @base)
        {
            this.@base = @base;

            spo = new Cache<ObjectVariants, ObjectVariants, ObjectVariants, bool>(
                @base.Contains);
            spO = new Cache<ObjectVariants, ObjectVariants, IEnumerable<ObjectVariants>>(@base.GetTriplesWithSubjectPredicate);
            sPo = new Cache<ObjectVariants, ObjectVariants, IEnumerable<ObjectVariants>>(@base.GetTriplesWithSubjectObject);
            Spo = new Cache<ObjectVariants, ObjectVariants, IEnumerable<ObjectVariants>>(@base.GetTriplesWithPredicateObject);
            SPo = new Cache<ObjectVariants, IEnumerable<KeyValuePair<ObjectVariants, ObjectVariants>>>(o =>
                @base.GetTriplesWithObject(o, (s,p) => 
                    new KeyValuePair<ObjectVariants, ObjectVariants>(s, p)));
            SpO = new Cache<ObjectVariants, IEnumerable<KeyValuePair<ObjectVariants, ObjectVariants>>>(p =>
                @base.GetTriplesWithPredicate(p, (s,o)=>
                        new KeyValuePair<ObjectVariants, ObjectVariants>(s, o)));
            sPO = new Cache<ObjectVariants, IEnumerable<KeyValuePair<ObjectVariants, ObjectVariants>>>(s =>
                @base.GetTriplesWithObject(s, (p,o)=>
                        new KeyValuePair<ObjectVariants, ObjectVariants>(p, o)));
        }

        public string Name { get; private set; }
        public NodeGenerator NodeGenerator { get { return @base.NodeGenerator; } }
        public void Clear()
        {
            //throw new NotImplementedException();
        }

        public IEnumerable<T> GetTriplesWithObject<T>(ObjectVariants o, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return @base.GetTriplesWithObject(o, createResult); //SPo.Get(o).Select(pair => createResult(pair.Key, pair.Value));
        }

        public IEnumerable<T> GetTriplesWithPredicate<T>(ObjectVariants p, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return @base.GetTriplesWithPredicate(p, createResult); //SpO.Get(p).Select(pair => createResult(pair.Key, pair.Value));

        }

        public IEnumerable<T> GetTriplesWithSubject<T>(ObjectVariants s, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return @base.GetTriplesWithSubject(s, createResult);
                //sPO.Get(s).Select(pair => createResult(pair.Key, pair.Value));
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            return spO.Get(subj, pred);
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subj, ObjectVariants obj)
        {
            throw new NotImplementedException();
            return null;
        }

        public IEnumerable<ObjectVariants> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
            return @base.GetTriplesWithPredicateObject(pred,obj);//Spo.Get(pred, obj);
        }

        public IEnumerable<T> GetTriples<T>(Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns)
        {
            throw new NotImplementedException();
        }

        public void Add(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
            throw new NotImplementedException();
        }

        public void Add(Triple<ObjectVariants, ObjectVariants, ObjectVariants> t)
        {
            throw new NotImplementedException();
        }

        public bool Contains(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
        {
            return @base.Contains(subject, predicate, obj); //spo.Get(subject,predicate, obj);
        }

        public void Delete(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectVariants> GetAllSubjects()
        {
            throw new NotImplementedException();
        }

        public long GetTriplesCount()
        {
            return 0;
        }

        public bool Any()
        {
            throw new NotImplementedException();
        }

        public void FromTurtle(string path)
        {
            //table.Clear();
             @base.FromTurtle(path);

        }

        public void FromTurtle(Stream inputStream)
        {
            throw new NotImplementedException();
        }

        void IGraph.Warmup()
        {
            Warmup();
        }

        public void Warmup()
        {
            @base.Warmup();
        }

       
    }
}