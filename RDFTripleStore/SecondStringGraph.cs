using System;
using System.Collections.Generic;
using System.IO;
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
    public class SecondStringGraph : GaGraphStringBased, IGraph 
    {
        private readonly NodeGenerator ng = RDFCommon.OVns.NodeGenerator.Create();


        public SecondStringGraph(string path)
            : base(path)
        {
        
 
        }

        public string Name { get; private set; }
        public NodeGenerator NodeGenerator { get { return ng; } }
       

        public virtual IEnumerable<T> GetTriplesWithObject<T>(ObjectVariants o, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return base.GetTriplesWithObject(o)
                .Select(base.Dereference)
                //.ReadWritableTriples()
                .Select(row => createResult(new OV_iri(DecodeIRI(row[0])), new OV_iri((DecodeIRI(row[1])))))
               ;
        }

        public virtual IEnumerable<T> GetTriplesWithPredicate<T>(ObjectVariants p, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return base.GetTriplesWithPredicate(((IIriNode)p).UriString)
             //   .ReadWritableTriples()
                .Select(base.Dereference)
                .Select(row => createResult(new OV_iri(DecodeIRI(row[0])), DecodeOV(row[2])))
                  ;

        }

        public virtual IEnumerable<T> GetTriplesWithSubject<T>(ObjectVariants s, Func<ObjectVariants, ObjectVariants, T> createResult)
        {
            return base.GetTriplesWithSubject(((IIriNode)s).UriString)
                //ReadWritableTriples()
                .Select(base.Dereference)
                
                .Select(row => createResult(new OV_iri(DecodeIRI(row[1])), DecodeOV(row[2])))
                 ;

        }

        public virtual IEnumerable<ObjectVariants> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred)
        {
            return base.GetTriplesWithSubjectPredicate(((IIriNode)subj).UriString, ((IIriNode)pred).UriString)
              //  .ReadWritableTriples()
                .Select(base.Dereference)
                .Select(row => DecodeOV(row[2]))
                ;

        }

        public IEnumerable<ObjectVariants> GetTriplesWithSubjectObject(ObjectVariants subj, ObjectVariants obj)
        {
            throw new NotImplementedException();
            return base.GetTriplesWithSubjectPredicate(((IIriNode)subj).UriString, obj)
                  //.ReadWritableTriples()
                .Select(base.Dereference)
                  .Select(row => new OV_iri(DecodeIRI(row[1])));
        }

        public virtual IEnumerable<ObjectVariants> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj)
        {
         
            return base.GetTriplesWithPredicateObject(((IIriNode)pred).UriString, obj)
               // .ReadWritableTriples()
                 .Select(base.Dereference)
                .Select(row => new OV_iri(DecodeIRI(row[0])))
                 ;

        }

        public IEnumerable<T> GetTriples<T>(Func<ObjectVariants, ObjectVariants, ObjectVariants, T> returns)
        {
            throw new NotImplementedException();
        }

        public void Add(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
            throw new NotImplementedException();
        }

        public void Insert(IEnumerable<Triple<ObjectVariants, ObjectVariants, ObjectVariants>> triples)
        {
            throw new NotImplementedException();
        }

        public void Add(Triple<ObjectVariants, ObjectVariants, ObjectVariants> t)
        {
            throw new NotImplementedException();
        }

        public virtual bool Contains(ObjectVariants subject, ObjectVariants predicate, ObjectVariants obj)
        {
            return base.Contains((object)subject.Content, (object)predicate.Content, obj);
        }

        public void Delete(ObjectVariants s, ObjectVariants p, ObjectVariants o)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObjectVariants> GetAllSubjects()
        {
            return base.GetTriples()
                .Select(base.Dereference)
                .Select(row => row[0])
                .Select(DecodeIRI)
                .Distinct()
                .Select(s => new OV_iri(s));
        }

        public long GetTriplesCount()
        {
           return base.GetTriples().Count();
        }

        public bool Any()
        {
            throw new NotImplementedException();
        }

        public void FromTurtle(string path)
        {
            //table.Clear();
        Build(new TripleGeneratorBufferedParallel(path, "g"));

        }

        public void FromTurtle(Stream inputStream)
        {
            throw new NotImplementedException();
        }
    }
}