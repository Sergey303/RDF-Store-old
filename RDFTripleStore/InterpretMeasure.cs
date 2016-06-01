using System;
using System.Collections.Generic;
using System.Linq;
using GoTripleStore;
using RDFCommon;
using RDFCommon.Interfaces;
using RDFCommon.OVns;
using RDFTripleStore;
using RDFTurtleParser;
using IGraph = RDFCommon.IGraph;

namespace TestingNs
{
    public class InterpretMeasure : SecondStringGraph, IGraph
    {
        public readonly Queue<object> history = new Queue<object>();
      

      public bool TrainingMode { get; set; }
        public InterpretMeasure(string path):base(path)
        {

        
        }

   
        public override IEnumerable<T> GetTriplesWithObject<T>(ObjectVariants o,
            Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            if (TrainingMode)
            {
                var cacheList = new List<KeyValuePair<ObjectVariants, ObjectVariants>>();
                foreach (var t in base.GetTriplesWithObject(o, (s, p) =>
                {
                    cacheList.Add(new KeyValuePair<ObjectVariants, ObjectVariants>(s, p));
                    return createResult(s, p);
                }).ToArray())
                    yield return t;
                history.Enqueue(cacheList);
            }
            else
                foreach (var pair in (List<KeyValuePair<ObjectVariants,ObjectVariants>>)history.Dequeue())
                    yield return createResult(pair.Key, pair.Value);
        }

        public override IEnumerable<T> GetTriplesWithPredicate<T>(ObjectVariants p, Func<ObjectVariants, ObjectVariants, T> createResult)
        {

            if (TrainingMode)
            {
                var cacheList = new List<KeyValuePair<ObjectVariants, ObjectVariants>>();
                foreach (var t in base.GetTriplesWithPredicate(p, (s, o) =>
                {
                    cacheList.Add(new KeyValuePair<ObjectVariants, ObjectVariants>(s, o));
                    return createResult(s, o);
                }).ToArray())
                    yield return t;
                history.Enqueue(cacheList);
            }
            else
                foreach (var pair in (List<KeyValuePair<ObjectVariants, ObjectVariants>>)history.Dequeue())
                    yield return createResult(pair.Key, pair.Value);

        }

        public override IEnumerable<T> GetTriplesWithSubject<T>(ObjectVariants s,
            Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            if (TrainingMode)
            {
                var cacheList = new List<KeyValuePair<ObjectVariants, ObjectVariants>>();
                foreach (var t in base.GetTriplesWithSubject(s, (p, o) =>
                {
                    cacheList.Add(new KeyValuePair<ObjectVariants, ObjectVariants>(p, o));
                    return createResult(p, o);
                }).ToArray())
                    yield return t;
                history.Enqueue(cacheList);
            }
            else
                foreach (var pair in (List<KeyValuePair<ObjectVariants, ObjectVariants>>)history.Dequeue())
                    yield return createResult(pair.Key, pair.Value);
        }

        public override IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            if (TrainingMode)
            {
                var res = base.GetTriplesWithSubjectPredicate(subj, pred).ToArray();


                history.Enqueue(res);
                return res;
            }
            else
                return (ObjectVariants[]) history.Dequeue();
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subj, ObjectVariants obj)
        {
            throw new NotImplementedException();
       
        }

        public override IEnumerable<ObjectVariants> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
            if (TrainingMode)
            {
                var results = base.GetTriplesWithPredicateObject(pred, obj).ToArray();
                history.Enqueue(results);
                return results;
            }
            else
                return (ObjectVariants[]) history.Dequeue();
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

        public override bool Contains(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
        {
            if (TrainingMode)
            {
                var contains = base.Contains(subject, predicate, obj);

                history.Enqueue(contains);
                return contains;
            }
            else
                return (bool)history.Dequeue();
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
            throw new NotImplementedException();

        }

        public bool Any()
        {
            throw new NotImplementedException();
        }

        public void FromTurtle(string path)
        {
            //table.Clear();
          base.FromTurtle(path);

        }

    }
}