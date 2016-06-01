using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Polar.Data;
using RDFCommon.OVns;
using SparqlParseRun;
using SparqlParseRun.SparqlClasses;

namespace TestingNs
{
   public class TestingPhotoPersons
   {
       private static int npersons = 4000 * 1000;
       public static TestDataGenerator data;
       private static Random rnd=new Random();

       public static int Npersons
       {
           get { return npersons; }
           set { npersons = value; data = new TestDataGenerator(npersons, 2378459); }
       }

       public string QDescribePerson()
       {
           return @"DESCRIBE person" + rnd.Next(npersons - 1);
       }
       public static string QGetPerson3123Info()
       {
           return @"SELECT ?property ?value WHERE { <person3123> ?property ?value }";
       }
       public static string QGetPersonInfo()
       {
           return string.Format(@"SELECT ?property ?value WHERE {{ <person{0}> ?property ?value }}", rnd.Next(npersons - 1));
       }
       public static string QGetPersonName()
       {
           return string.Format(@"SELECT ?name WHERE {{ <person{0}> <name> ?name }}", rnd.Next(npersons - 1));
       }
       public static string QContainsPersonType()
       {
           return string.Format(@"ASK WHERE {{ <person{0}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <person> }}", rnd.Next(npersons - 1));
       }
       public static string QGetPersonPhotoNames()
       {
           return string.Format(@"SELECT ?name WHERE {{ ?reflection <reflected> <person{0}>   .
                                                  ?reflection  <in_doc>   ?doc                 .
                                                  ?doc <name> ?name  }}", rnd.Next(npersons - 1));
       }



       public static void Run(Action<string> runQueryReturnCount)
       {     
       
           //Console.WriteLine(runQueryReturnCount(QGetPerson3123Info()));  
           Performance.ComputeTime(() =>
           {
               for (int i = 0; i < 1000; i++)
               {
                   runQueryReturnCount(QGetPersonInfo());
               }
           }, "1000 sPO ok. duration=", true);

           Performance.ComputeTime(() =>
           {
               for (int i = 0; i < 1000; i++)
               {
                   runQueryReturnCount(QGetPersonName());
               }
           }, string.Format("1000 spO ok. duration="), true);

       
           Performance.ComputeTime(() =>
           {                                 
               for (int i = 0; i < 1000; i++)
               {
                    runQueryReturnCount(QContainsPersonType());
                   //if (!exists) throw new Exception("438723");
               }
           }, "1000 spo ok duration=", true);

           Performance.ComputeTime(() =>
           {
               for (int i = 0; i < 1000; i++)
               {
                   runQueryReturnCount(QGetPersonPhotoNames());
               }
           }, string.Format("1000 portraits ok. duration="), true);
         
       }
    }
}
