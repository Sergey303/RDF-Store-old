using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
using SparqlQuery.SparqlClasses.GraphPattern;
using SparqlQuery.SparqlClasses.GraphPattern.Triples;
using SparqlQuery.SparqlClasses.GraphPattern.Triples.Node;
using SparqlQuery.SparqlClasses.Query.Result;

namespace SparqlQuery.SparqlClasses.Update
{
    public class SparqlUpdateModify : ISparqlUpdate    
    {
        private readonly RdfQuery11Translator q;
        private ISparqlGraphPattern @where;
        private ObjectVariants with;
        private SparqlQuadsPattern insert;
        private SparqlQuadsPattern delete;

        public void SetWith(ObjectVariants iri)
        {
            with = iri;
        }
        public SparqlUpdateModify(RdfQuery11Translator q)
        {
            this.q = q;
        }

        public SparqlUpdateModify(SparqlQuadsPattern sparqlUpdateDelete)
        {
            delete = sparqlUpdateDelete;
        }
     

        internal void SetInsert(SparqlQuadsPattern sparqlUpdateInsert)
        {
            insert = sparqlUpdateInsert;
        }

        internal void SetDelete(SparqlQuadsPattern sparqlUpdateDelete)
        {
            delete = sparqlUpdateDelete;
        }

        internal void SetWhere(ISparqlGraphPattern sparqlGraphPattern)
        {
            @where = sparqlGraphPattern;
        }

        public void Run(IStore store)
        {
            var results = @where.Run(Enumerable.Repeat(new SparqlResult(q), 1));
            SparqlTriple[] defaultGraphTriplesInsert = null;
            SparqlTriple[] defaultGraphTriplesDelete = null;
            SparqlGraphGraph[] graphPatternsInsert = null;
            SparqlGraphGraph[] graphPatternsDelete=null;
            if (insert != null)
            {                  
                defaultGraphTriplesInsert =
                    insert.Where(pattern => pattern.PatternType == SparqlGraphPatternType.SparqlTriple)
                        .Cast<SparqlTriple>()
                        .ToArray();
                graphPatternsInsert = insert
                    .Where(pattern => pattern.PatternType == SparqlGraphPatternType.Graph)
                    .Cast<SparqlGraphGraph>()
                    .ToArray();
            }
            if (delete != null)
            {
                defaultGraphTriplesDelete =
                    delete.Where(pattern => pattern.PatternType == SparqlGraphPatternType.SparqlTriple)
                        .Cast<SparqlTriple>()
                        .ToArray();

                graphPatternsDelete = delete
                    .Where(pattern => pattern.PatternType == SparqlGraphPatternType.Graph)
                    .Cast<SparqlGraphGraph>()
                    .ToArray();
            }

            foreach (var result in results)
            {
               
                if (delete != null)
                {
                    if (with == null)
                        foreach (SparqlTriple triple in defaultGraphTriplesDelete)
                            triple.Substitution(result,
                                store.Delete);
                    else
                        foreach (SparqlTriple triple in defaultGraphTriplesDelete)
                            triple.Substitution(result,   
                                with,
                                store.NamedGraphs.Delete);
                    foreach (SparqlGraphGraph sparqlGraphPattern in graphPatternsDelete)
                    {
                        if (sparqlGraphPattern.Name is VariableNode)
                        {
                            var gVariableNode = ((VariableNode)sparqlGraphPattern.Name);
                            foreach (var triple in sparqlGraphPattern.GetTriples())
                                triple.Substitution(result, gVariableNode, store.NamedGraphs.Delete);
                        }
                        else
                            foreach (var triple in sparqlGraphPattern.GetTriples())
                                triple.Substitution(result, sparqlGraphPattern.Name, store.NamedGraphs.Delete);
                    }
                }
                if (insert != null)
                {
                    if (with == null)
                    foreach (SparqlTriple triple in defaultGraphTriplesInsert)
                        triple.Substitution(result,
                            store.Add);
                    else
                        foreach (SparqlTriple triple in defaultGraphTriplesInsert)
                            triple.Substitution(result,
                                with,
                                store.NamedGraphs.Add);
                    foreach (SparqlGraphGraph sparqlGraphPattern in graphPatternsInsert)
                    {
                        if (sparqlGraphPattern.Name is VariableNode)
                        {
                            var gVariableNode = ((VariableNode) sparqlGraphPattern.Name);
                            foreach (var triple in sparqlGraphPattern.GetTriples())
                                triple.Substitution(result, gVariableNode, store.NamedGraphs.Add);
                        }
                        else
                            foreach (var triple in sparqlGraphPattern.GetTriples())
                                triple.Substitution(result, sparqlGraphPattern.Name, store.NamedGraphs.Add);
                    }
                }
            }
        }
    }
}
