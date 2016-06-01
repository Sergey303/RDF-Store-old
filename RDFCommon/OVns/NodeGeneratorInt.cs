using System;
using System.Linq;
using UniversalIndex;

namespace RDFCommon.OVns
{
    public class NodeGeneratorInt:NodeGenerator
    {
        public INametable coding_table;
        public NodeGeneratorInt(string path)
        {     
            coding_table=new NametableLinearBuffered(path);
            
        }

        public void Clear()
        {
             coding_table.Clear();
            SpecialTypes = new SpecialTypesClass(this);
        }
        public override void Build()
        {
                SpecialTypes = new SpecialTypesClass(this);
            coding_table.Save();
            //coding_table.InsertPortion(SpecialTypesClass.GetAll());

            //  coding_table.BuildScale();

        }
        public static NodeGeneratorInt Create(string path)
        {
            var ng = new NodeGeneratorInt(path);
            ng.SpecialTypes = new SpecialTypesClass(ng);
            return ng;
        }

        public override ObjectVariants GetUri(object uri)
        {
            var s = uri as string;
            if (s != null)
            {
                int code = coding_table.GetCode(s);
         
            if (code == -1)
                return new OV_iri(s);
            else return new OV_iriint(code, coding_table.GetString);
            }
            else  if(uri is int)
                return new OV_iriint((int) uri, coding_table.GetString);
            throw new ArgumentException();
        }

        public override ObjectVariants AddIri(string iri)
        {
            return new OV_iriint(coding_table.GetSetCode(iri), coding_table.GetString); ;
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
            int code = coding_table.GetCode(iriString.UriString);
            iriCoded = iriString;
            if (code == -1)
                return false;
            iriCoded=new OV_iriint(code, coding_table.GetString);
            return true;
        }

    }
    
}
