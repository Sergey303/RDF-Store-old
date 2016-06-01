using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.GraphPattern.Triples.Path
{
    public class SparqlPathManyTriple : ISparqlGraphPattern
    {
        private readonly SparqlPathTranslator predicatePath;           
        private Dictionary<ObjectVariants, HashSet<ObjectVariants>> bothVariablesCacheBySubject, bothVariablesCacheByObject;
        private KeyValuePair<ObjectVariants, ObjectVariants>[] bothVariablesChache;
        private VariableNode sVariableNode;
        private VariableNode oVariableNode;
        private RdfQuery11Translator q;
        private bool useCache=false;

        public SparqlPathManyTriple(ObjectVariants subject, SparqlPathTranslator pred, ObjectVariants @object, RdfQuery11Translator q)
        {
            this.predicatePath = pred;
            Subject = subject;
            Object = @object;
            this.q = q;
            sVariableNode = Subject as VariableNode;
            oVariableNode = Object as VariableNode;
        }
        

        public IEnumerable<SparqlResult> Run(IEnumerable<SparqlResult> variableBindings)
        {
            var bindings = variableBindings;

            Queue<ObjectVariants> newSubjects = new Queue<ObjectVariants>();
            ObjectVariants[] fromVariable = null;
            ObjectVariants o=null;
            ObjectVariants s=null;
            switch (NullablePairExt.Get(sVariableNode, oVariableNode))
            {
                case NP.bothNull:
                    
                    return TestSOConnection(Subject, Object) ? bindings : Enumerable.Empty<SparqlResult>();
                case NP.leftNull:
                    newSubjects.Enqueue(Subject);
                    return bindings.SelectMany(binding =>
                    {
                          o = binding[oVariableNode];
                        if (o != null)
                            return TestSOConnection(Subject, o)
                                ? Enumerable.Repeat(binding, 1)
                                : Enumerable.Empty<SparqlResult>();
                        else
                        {
                            if (fromVariable == null)
                                fromVariable = GetAllSConnections(Subject).ToArray();
                            return fromVariable.Select(node => binding.Add(node, oVariableNode));
                        }
                    });
                case NP.rigthNull:
                    return bindings.SelectMany(binding =>
                    {
                        s = binding[sVariableNode];
                        if (s != null)
                            return TestSOConnection(s, Object)
                                ? Enumerable.Repeat(binding, 1)
                                : Enumerable.Empty<SparqlResult>();
                        else
                        {
                            if (fromVariable == null)
                                fromVariable = GetAllOConnections(Object).ToArray();
                            return fromVariable.Select(node => binding.Add(node, sVariableNode));
                        }
                    });
                case NP.bothNotNull:
                  

                    return bindings.SelectMany(binding =>
                    {
                        s = binding[sVariableNode];
                        o = binding[oVariableNode];
                        switch (NullablePairExt.Get(s, o))
                        {
                            case NP.bothNotNull:
                                if ((useCache && TestSOConnectionFromCache(s, o)) || (TestSOConnection(s, o)))
                                        return Enumerable.Repeat(binding, 1);
                                    else return Enumerable.Empty<SparqlResult>();
                            case NP.rigthNull:
                                return GetAllSConnections(s).Select(node => binding.Add(node, oVariableNode));
                                break;
                            case NP.leftNull:
                                return GetAllOConnections(o).Select(node => binding.Add(node, sVariableNode));
                                break;
                            case NP.bothNull:
                                useCache = true;
                                bothVariablesChache = predicatePath.CreateTriple(sVariableNode, oVariableNode, q)
                      .Aggregate(Enumerable.Repeat(new SparqlResult(q), 1),
                          (enumerable, triple) => triple.Run(enumerable))
                      .Select(
                          r =>
                              new KeyValuePair<ObjectVariants, ObjectVariants>(r[sVariableNode], r[oVariableNode]))
                      .ToArray();
                                return bothVariablesCacheBySubject.Keys.SelectMany(GetAllSConnections,
                                    (sbj, node) => binding.Add(sbj, sVariableNode, node, oVariableNode));
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    });
                 
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CreateCacheBySubject()
        {
            if (bothVariablesCacheBySubject == null)
            {
                bothVariablesCacheBySubject =
                    new Dictionary<ObjectVariants, HashSet<ObjectVariants>>();
                foreach (var pair in bothVariablesChache)
                {
                    HashSet<ObjectVariants> nodes;
                    if (!bothVariablesCacheBySubject.TryGetValue(pair.Key, out nodes))
                        bothVariablesCacheBySubject.Add(pair.Key,
                            new HashSet<ObjectVariants>() {pair.Value});
                    else nodes.Add(pair.Value);
                }
            }
        }

        public ObjectVariants Subject { get; private set; }
        public ObjectVariants Object { get; private set; }

        private IEnumerable<ObjectVariants> GetAllSConnections(ObjectVariants subj)
        {
           if(useCache) CreateCacheBySubject();
            HashSet<ObjectVariants> history = new HashSet<ObjectVariants>() { subj };
            Queue<ObjectVariants> subjects = new Queue<ObjectVariants>();
            subjects.Enqueue(subj);
            while (subjects.Count > 0)
            {
                IEnumerable<ObjectVariants> objects;
                if (useCache)
                {
                    HashSet<ObjectVariants> objectsSet;
                    objects = bothVariablesCacheBySubject.TryGetValue(subjects.Dequeue(), out objectsSet)
                        ? objectsSet
                        : Enumerable.Empty<ObjectVariants>();}
                else
                    objects = RunTriple(subjects.Dequeue(), oVariableNode)
                        .Select(sparqlResult => sparqlResult[oVariableNode]);
                foreach (var objt in objects
                    .Where(objt =>
                    {
                        var isNewS = !history.Contains(objt);
                        if (isNewS)
                        {
                            history.Add(objt);
                            subjects.Enqueue(objt);
                        }
                        return isNewS;
                    }))
                    yield return objt;}
        }

        private IEnumerable<ObjectVariants> GetAllOConnections(ObjectVariants objt)
        {
            if (useCache)
                CreateCacheByObject();
            HashSet<ObjectVariants> history = new HashSet<ObjectVariants>() { objt };
            Queue<ObjectVariants> objects = new Queue<ObjectVariants>();
            objects.Enqueue(objt);

            while (objects.Count > 0)  
            {
                IEnumerable<ObjectVariants> subjects;
                if (useCache)
                {
                    HashSet<ObjectVariants> subjectsHashSet;
                    subjects = bothVariablesCacheByObject.TryGetValue(objects.Dequeue(), out subjectsHashSet) ? subjectsHashSet : Enumerable.Empty<ObjectVariants>();
                }
                else
                  subjects = RunTriple(sVariableNode, objects.Dequeue())
                    .Select(sparqlResult => sparqlResult[sVariableNode]);
                foreach (var subjt in subjects
                    .Where(subjt =>
                    {
                        var isNewS = !history.Contains(subjt);
                        if (isNewS)
                        {
                            history.Add(subjt);
                            objects.Enqueue(subjt);
                        }
                        return isNewS;
                    }))
                    yield return subjt;
            }
        }

        private void CreateCacheByObject()
        {
            if (bothVariablesCacheByObject == null)
            {
                bothVariablesCacheByObject = new Dictionary<ObjectVariants, HashSet<ObjectVariants>>();
                foreach (var pair in bothVariablesChache)
                {
                    HashSet<ObjectVariants> nodes;
                    if (!bothVariablesCacheByObject.TryGetValue(pair.Value, out nodes))
                        bothVariablesCacheByObject.Add(pair.Value, new HashSet<ObjectVariants>() {pair.Key});
                    else nodes.Add(pair.Key);
                }
            }
        }


        private bool TestSOConnection(ObjectVariants sbj, ObjectVariants objct)
        {
            HashSet<ObjectVariants> history = new HashSet<ObjectVariants>() { Subject };
            Queue<ObjectVariants> newSubjects = new Queue<ObjectVariants>();
            newSubjects.Enqueue(sbj);
            var subject = newSubjects.Peek();
            if (RunTriple(subject, objct).Any())  
                return true;
            var newVariable = (SparqlBlankNode)q.CreateBlankNode();
            while (newSubjects.Count > 0)
                if (RunTriple(newSubjects.Dequeue(), newVariable)
                    .Select(sparqlResult => sparqlResult[newVariable])
                    .Where(o => !history.Contains(o))
                    .Any(o =>
                    {
                        history.Add(o);
                        newSubjects.Enqueue(o);
                        return RunTriple(o, objct).Any();
                    }))    
                    return true;
            return false;
        }

        private bool TestSOConnectionFromCache(ObjectVariants sbj, ObjectVariants objct)
        {
         CreateCacheBySubject();
            HashSet<ObjectVariants> history = new HashSet<ObjectVariants>() { Subject };
            HashSet<ObjectVariants> objects;
            if (bothVariablesCacheBySubject.TryGetValue(sbj, out objects) && objects.Contains(objct))
                return true;
            
            Queue<ObjectVariants> newSubjects = new Queue<ObjectVariants>();
            newSubjects.Enqueue(sbj);

            while (newSubjects.Count > 0)
                if (bothVariablesCacheBySubject.TryGetValue(newSubjects.Dequeue(), out objects)
                    && objects
                        .Where(o => !history.Contains(o))
                        .Any(o =>
                        {
                            history.Add(o);
                            newSubjects.Enqueue(o);
                            return bothVariablesCacheBySubject.TryGetValue(o, out objects) && objects.Contains(objct);
                        }))
                    return true;
            return false;
        }

        private IEnumerable<SparqlResult> RunTriple(ObjectVariants subject, ObjectVariants objct)
        {                                     
                return predicatePath.CreateTriple(subject, objct, q).Aggregate(Enumerable.Repeat(new SparqlResult(q), 1),
                    (enumerable, triple) => triple.Run(enumerable));
           
        }

        public new SparqlGraphPatternType PatternType
        {
            get { return SparqlGraphPatternType.PathTranslator; }
        }
    }
}