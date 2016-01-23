using System;
using System.Linq;
using UniversalIndex;

namespace RDFCommon.OVns
{
    public class NodeGeneratorInt:NodeGenerator
    {
        public NameTableUniversal coding_table;
        public NodeGeneratorInt(string path, bool empty)
        {     
            coding_table=new NameTableUniversal(path);
            if (empty)
            {
                Clear();
                coding_table.InsertPortion(Enumerable.Repeat(SpecialTypesClass.RdfType, 1));
                coding_table.BuildScale();
            }
            
            SpecialTypes = new SpecialTypesClass(this);

        }

        public void Clear()
        {
             coding_table.Clear();
            coding_table.Fill(new string[0]);
            coding_table.BuildIndexes();
        }
        public override void Build()
        {
           
            //coding_table.InsertPortion(SpecialTypesClass.GetAll());
            
            coding_table.BuildScale();
            
        }
        public static NodeGeneratorInt Create(string path, bool isEmpty)
        {
            var ng = new NodeGeneratorInt(path, isEmpty);
            ng.SpecialTypes = new SpecialTypesClass(ng);
            return ng;
        }

        public override ObjectVariants GetUri(object uri)
        {
            var s = uri as string;
            if (s != null)
            {
                int code = coding_table.GetCodeByString(s);
         
            if (code == -1)
                return new OV_iri(s);
            else return new OV_iriint(code, coding_table.GetStringByCode);
            }
            else  if(uri is int)
                return new OV_iriint((int) uri, coding_table.GetStringByCode);
            throw new ArgumentException();
        }

        public override ObjectVariants AddIri(string iri)
        {
            return new OV_iriint(coding_table.Add(iri), coding_table.GetStringByCode); ;
        }


        //public override ObjectVariants CreateLiteralOtherType(string p, string typeUriNode)
        //{
        //    return new OV_typedint(p, coding_table.Add(typeUriNode), coding_table.GetStringByCode);
        //}

       
        //   public ObjectVariants GetCoded(int code)
        //{
        //    return new OV_iriint(code, coding_table.GetStringByCode);
        //}

        public override bool TryGetUri(OV_iri iriString, out ObjectVariants iriCoded)
        {
            int code = coding_table.GetCodeByString(iriString.UriString);
            iriCoded = iriString;
            if (code == -1)
                return false;
            iriCoded=new OV_iriint(code, coding_table.GetStringByCode);
            return true;
        }

    }
    
}
