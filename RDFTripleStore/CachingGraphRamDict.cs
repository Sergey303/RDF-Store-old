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
    public class CachingGraphRamDict : IGraph
    {
        public GraphCascadingInt Graph { get; set; }

        //private readonly Dictionary<ObjectVariants, IEnumerable<>> sCache = new Dictionary<ObjectVariants>();
        //private readonly Dictionary<ObjectVariants> pCache = new Dictionary<ObjectVariants>();
        //private readonly Dictionary<ObjectVariants> oCache = new Dictionary<ObjectVariants>();
        private readonly Dictionary<KeyValuePair<int, int>, ObjectVariants[]> spCache = new Dictionary<KeyValuePair<int, int>, ObjectVariants[]>();
        //private readonly Dictionary<KeyValuePair<ObjectVariants, ObjectVariants>> soCache = new Dictionary<KeyValuePair<ObjectVariants, ObjectVariants>>();
        //private readonly Dictionary<KeyValuePair<ObjectVariants, ObjectVariants>> poCache = new Dictionary<KeyValuePair<ObjectVariants, ObjectVariants>>();
        //private readonly Dictionary<Tuple<ObjectVariants, ObjectVariants, ObjectVariants>> spo = new Dictionary<Tuple<ObjectVariants, ObjectVariants, ObjectVariants>>();

        public string Name { get { return Graph.Name; } set { Graph.Name = value; }}

        public NodeGenerator NodeGenerator
        {
            get { return Graph.NodeGenerator; }          
        }

        public void Clear()
        {
           Graph.Clear();
        }

        public IEnumerable<TripleOVStruct> GetTriplesWithObject(ObjectVariants o)
        {
            return Graph.GetTriplesWithObject(o);
        }

        public IEnumerable<TripleOVStruct> GetTriplesWithPredicate(ObjectVariants p)
        {
            return Graph.GetTriplesWithObject(p);
        }

        public IEnumerable<TripleOVStruct> GetTriplesWithSubject(ObjectVariants s)
        {
            return Graph.GetTriplesWithSubject(s);
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            ObjectVariants[] objects;
            var key = new KeyValuePair<int, int>(((OV_iriint) subj).code, ((OV_iriint) pred).code);
            if (spCache.TryGetValue(key, out objects))
            return objects;
             objects = Graph.GetTriplesWithSubjectPredicate(subj, pred).ToArray();
            spCache.Add(key, objects);
            return objects;
        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subj, ObjectVariants obj)
        {
            return Graph.GetTriplesWithSubjectObject(subj, obj);
        }

        public IEnumerable<ObjectVariants> GetSubjects(ObjectVariants pred, ObjectVariants obj)
        {
            return Graph.GetSubjects(pred, obj);
        }

        public IEnumerable<T> GetTriples<T>(Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns)
        {
            return Graph.GetTriples(returns);
        }

        public void Add(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
            throw new NotImplementedException();
        }

        public bool Contains(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
        {
            return Graph.Contains(subject, predicate, obj);
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
            Graph.FromTurtle(gString);
        }

        public void Build(IEnumerable<TripleStrOV> triples)
        {
            throw new NotImplementedException();
        }

        public void FromTurtle(Stream inputStream)
        {
            Graph.FromTurtle(inputStream);
        }

        public void Warmup()
        {
          Graph.Warmup();
        }       
    }
}
