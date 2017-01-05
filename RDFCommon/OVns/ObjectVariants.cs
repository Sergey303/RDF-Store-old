using System;
using UniversalIndex;

namespace RDFCommon.OVns
{
    public abstract class ObjectVariants   : IComparable
    {
        public  abstract ObjectVariantEnum Variant { get; }
        public abstract object WritableValue { get; }

        public virtual object[] ToWritable()
        {
            return new object[]{(int)Variant, WritableValue};
        }
        

        public virtual int CompareTo(object obj)
        {
            if (obj is ObjectVariants)
            {
                var other = (ObjectVariants)obj;
                return Variant.CompareTo(other.Variant);  
            }
            throw new ArgumentException();
        }

        public override string ToString()
        {
            return "\""+Content+"\"^^<"+((ILiteralNode)this).DataType+">";
        }

        public override int GetHashCode()
        {
            return (Content.ToString() + Variant).GetHashModifiedBernstein();
        }

        public abstract object Content { get; }
        public abstract ObjectVariants Change(Func<dynamic, dynamic> changing);

        public static string GenerateBlankIri(string graphName)
        {
            return "generated_blank_iri_of_" + graphName + "_" + Guid.NewGuid().ToString();
        }
        public static string GenerateBlankIri(string graphName, string blank_name)
        {
            return "generated_blank_iri_of_" + graphName + "_" + blank_name;
        }
    }
}