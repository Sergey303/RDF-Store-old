using System;

namespace RDFCommon.OVns
{
    public class OV_nonNegativeInteger : ObjectVariants, ILiteralNode, INumLiteral
    {
        public readonly int value;

        public OV_nonNegativeInteger(int value)
        {
            this.value = value;
        }

        public OV_nonNegativeInteger(string s) : this(int.Parse(s))
        {
            
        }

        public override ObjectVariantEnum Variant
        {
            get { return ObjectVariantEnum.Double; }
        }

        public override object WritableValue
        {
            get { return value; }
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return value == ((OV_nonNegativeInteger)obj).value;

        }

        
        
        public override object Content { get { return value; } }
        public override ObjectVariants Change(Func<dynamic, dynamic> changing)
        {
            return new OV_nonNegativeInteger(changing(value));
        }

        public string DataType { get { return SpecialTypesClass.Double; } }
        public override string ToString()
        {
            return value.ToString();
        }
        public override int CompareTo(object obj)
        {
            int baseComp = base.CompareTo(obj);
            if (baseComp != 0) return baseComp;
            var otherTyped = (OV_nonNegativeInteger)obj;
            return value.CompareTo(otherTyped.value);
        }
    }
}