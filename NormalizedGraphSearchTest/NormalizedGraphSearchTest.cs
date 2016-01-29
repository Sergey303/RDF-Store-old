using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFCommon;
using RDFCommon.OVns;
using RDFTripleStore;

namespace NormalizedGraphSearchTestNS
{
    [TestClass]
    public class NormalizedGraphSearchTest
    {
        private StoreCascadingInt store;
        private NormalizedGraph4Search normalizedGraph4Search;
        private KeyValuePair<int, int>[] edges;
        private int[] Children;
        private int[][] Parents;
        private ObjectVariants inCollectionPredicate;
        private ObjectVariants collectionItempredicate;
        private IEnumerable<int> vertexes;

        public NormalizedGraphSearchTest()
        {
            store = new StoreCascadingInt(@"..\..\..\Databases\int based\");
            store.ClearAll();

            //Performance.ComputeTime(() => store.ReloadFrom(Config.Source_data_folder_path + "10M.ttl"), "load 10 млн ", true);
            store.ReloadFrom(@"C:\deployed\triples.ttl");
            store.NodeGenerator.TryGetUri(new OV_iri("http://fogid.net/o/collection-item"), out collectionItempredicate);
            var triplesCount = store.GetTriplesCount();
            store.NodeGenerator.TryGetUri(new OV_iri("http://fogid.net/o/in-collection"), out inCollectionPredicate);
            edges = (from triple in store.GetTriplesWithPredicate(collectionItempredicate)
                     from item in store.GetTriplesWithSubjectPredicate(triple.Subject, inCollectionPredicate)
                select new KeyValuePair<int, int>((int)item.WritableValue, (int)triple.Object.WritableValue)).ToArray();
            vertexes = edges.Select(pair => pair.Key).Concat(edges.Select(pair => pair.Value)).Distinct();
            normalizedGraph4Search = new NormalizedGraph4Search();
            normalizedGraph4Search.ReCreate(edges);

            var allSubTree = normalizedGraph4Search.GetAllSubTree(edges[0].Key);


        }


        [TestMethod]
        public void TestChildren()
        {
            foreach (var vertex in vertexes)
            {
                Children = normalizedGraph4Search.GetChildren(vertex);
                var mustBe = store.GetSubjects(inCollectionPredicate, new OV_iriint(vertex
                    , null))
                       .Select(collectMember =>
                               store.GetTriplesWithSubjectPredicate(collectMember, collectionItempredicate).First())
                       .Select(obj => obj.WritableValue)
                       .Cast<int>()
                       .ToArray();

                for (int i = 0; i < Children.Length; i++)
                {
                    Assert.AreEqual(mustBe[i], Children[i]);
                }
            }
        }


        [TestMethod]
        public void TestConnection()
        {
            Assert.IsTrue(normalizedGraph4Search.TestConnection(edges[0].Key, edges[0].Value));
            Assert.IsTrue(normalizedGraph4Search.TestConnection(edges[0].Value, edges[0].Key));
            Assert.IsTrue(normalizedGraph4Search.TestConnection(edges[1].Key, edges[0].Value));
            Assert.IsTrue(normalizedGraph4Search.TestConnection(edges[2].Key, edges[0].Value));
            Assert.IsFalse(normalizedGraph4Search.TestConnection(Children[1], Children[0]));
        }

        [TestMethod]
        public void TestParents()
        {
            Parents = normalizedGraph4Search.GetParents(Children[0]).ToArray();
        }
    }
}
