using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpookilySharp;

namespace RDFCommon
{
    public static class HashFunctions
    {
        public static int GetHashSome(this char[] key)
        {
            uint hash, i;
            for (hash = i = 0; i < key.Length; ++i)
            {
                hash += key[i];
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }
            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);
            return (int) hash;
        }

        public static uint RSHash(this char[] str)
        {
            uint b = 378551;
            uint a = 63689;
            uint hash = 0;
            uint i = 0;

            for (i = 0; i < str.Length; i++)
            {
                hash = hash*a + str[i];
                a = a*b;
            }
            return hash;
        }

        public static uint SpookyHash(this string str)
        {
              SpookilySharp. SpookyHash hash=new SpookyHash();
            hash.Update(str);
            return (uint) hash.Final().UHash2;
        }

        public static uint BobKenkLookup3(this string str, uint previous)
        {
            return BJL3.bob_lookup3_hashlittle(str, previous);
            
        }
    }

    public class BJL3
    {
        private static uint rot(uint x, ushort k)
        {
            return ((x) << (k)) ^ ((x) >> (32 - (k)));
        }

 private static void mix(ref uint a, ref uint b, ref uint c) { 
	a -= c;  a ^= rot (c,  4);  c += b; 
	b -= a;  b ^= rot (a,  6);  a += c; 
	c -= b;  c ^= rot (b,  8);  b += a; 
	a -= c;  a ^= rot (c, 16);  c += b; 
	b -= a;  b ^= rot (a, 19);  a += c; 
	c -= b;  c ^= rot (b,  4);  b += a; }
 
static  void final(ref uint a, ref uint b, ref uint c) {  
	c ^= b; c -= rot (b, 14); 
	a ^= c; a -= rot (c, 11); 
	b ^= a; b -= rot (a, 25); 
	c ^= b; c -= rot (b, 16); 
	a ^= c; a -= rot (c, 4);  
	b ^= a; b -= rot (a, 14); 
	c ^= b; c -= rot (b, 24); }

        static Random random=new Random();
public static uint bob_lookup3_hash1(string str, uint initval)
{
	  uint a,b,c;
    var key = str;

  /* Set up the internal state */
    //uint initval=(uint) random.Next();
    a = b = c = 0xdeadbeef + (((uint)str.Length)<<2) + initval;

  /*------------------------------------------------- handle most of the key */
    int length = str.Length;
    int i = 0;
    while (length > 3)
  {
    a += key[i];
    b += key[i+1];
    c += key[i+2];
    mix(ref a,ref b,ref c);
    length -= 3;
    i += 3;
  }

  /*------------------------------------------- handle the last 3 uint's */
  switch(length)                     /* all the case statements fall through */
  { 
  case 3 : c+=key[i+2];   break;
  case 2 : b+=key[i+1];   break;
  case 1 : a+=key[i];        
    break;
  case 0:     /* case 0: nothing left to add */
    break;
  }
    final(ref a,ref b,ref c);

  /*------------------------------------------------------ report the result */
  return c;
}

        public static uint bob_lookup3_hashlittle( string key, uint initval)
        {
            uint a, b, c; /* internal state */


            /* Set up the internal state */
            a = b = c = 0xdeadbeef + ((uint) key.Length) + initval; /* need to read the key one byte at a time */


            /*--------------- all but the last block: affect some 32 bits of (a,b,c) */
            uint length = (uint) key.Length;
            int i = 0;
            while (length > 12)
            {
                a += key[i+0];
                a += ((uint) key[i+1]) << 8;
                a += ((uint) key[i+2]) << 16;
                a += ((uint) key[i+3]) << 24;
                b += key[i+4];
                b += ((uint) key[i+5]) << 8;
                b += ((uint) key[i+6]) << 16;
                b += ((uint) key[i+7]) << 24;
                c += key[i+8];
                c += ((uint) key[i+9]) << 8;
                c += ((uint) key[i+10]) << 16;
                c += ((uint) key[i+11]) << 24;
                mix(ref a, ref b, ref c);
                length -= 12;
                i += 12;
            }

            /*-------------------------------- last block: affect all 32 bits of (c) */
            switch (key.Length) /* all the case statements fall through */
            {
                case 12:
                    c += ((uint) key[i+11]) << 24;
                    break;
                case 11:
                    c += ((uint) key[i+10]) << 16;
                    break;
                case 10:
                    c += ((uint) key[i+9]) << 8;
                    break;
                case 9:
                    c += key[i+8];
                    break;
                case 8:
                    b += ((uint) key[i+7]) << 24;
                    break;
                case 7:
                    b += ((uint) key[i+6]) << 16;
                    break;
                case 6:
                    b += ((uint) key[i+5]) << 8;
                    break;
                case 5:
                    b += key[i+4];
                    break;
                case 4:
                    a += ((uint) key[i+3]) << 24;
                    break;
                case 3:
                    a += ((uint) key[i+2]) << 16;
                    break;
                case 2:
                    a += ((uint) key[i+1]) << 8;
                    break;
                case 1:
                    a += key[i+0];
                    break;
                case 0:
                    return c;
            }

            final(ref a, ref b, ref c);
            return c;
        }
    }
}
