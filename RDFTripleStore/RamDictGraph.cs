using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDFCommon;
using RDFCommon.OVns;

namespace RDFTripleStore
{
    class RamDictGraph :IGraph
    {
        private NodeGenerator ng = RDFCommon.OVns.NodeGenerator.Create();
        private Dictionary<ObjectVariants,
            KeyValuePair<Dictionary<ObjectVariants, ObjectVariants>, Dictionary<ObjectVariants, HashSet<ObjectVariants>>>> triples = new Dictionary<ObjectVariants, KeyValuePair<Dictionary<ObjectVariants, ObjectVariants>, Dictionary<ObjectVariants, HashSet<ObjectVariants>>>>();

        public string Name { get; set; }

        public NodeGenerator NodeGenerator
        {
            get { return ng; }          
        }

        public void Clear()
        {
           triples.Clear();
        }

        public IEnumerable<TripleOVStruct> GetTriplesWithObject(ObjectVariants o)
        {
            KeyValuePair<Dictionary<ObjectVariants, ObjectVariants>, Dictionary<ObjectVariants, HashSet<ObjectVariants>>> finded;
            if (triples.TryGetValue(o, out finded)) return Enumerable.Empty<TripleOVStruct>();
            return null;
        }

        public IEnumerable<TripleOVStruct> GetTriplesWithPredicate(ObjectVariants p)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TripleOVStruct> GetTriplesWithSubject(ObjectVariants s)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subj, ObjectVariants obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectVariants> GetSubjects(ObjectVariants pred, ObjectVariants obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetTriples<T>(Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns)
        {
            throw new NotImplementedException();
        }

        public void Add(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
            throw new NotImplementedException();
        }

        public bool Contains(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
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

        public void FromTurtle(string gString)
        {
            throw new NotImplementedException();
        }

        public void Build(IEnumerable<TripleStrOV> triples)
        {
            throw new NotImplementedException();
        }

        public void FromTurtle(Stream inputStream)
        {
            throw new NotImplementedException();
        }

        public void Warmup()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
