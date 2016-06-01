﻿using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.Query.Result;

namespace SparqlParseRun.SparqlClasses.Expressions
{
    public abstract class SparqlExpression
    {

        public Func<SparqlResult, ObjectVariants> TypedOperator;

        // private ExpressionTypeEnum typeEnum;
        public VariableDependenceGroupLevel AggregateLevel;

        public enum VariableDependenceGroupLevel
        {
            Const=0,
            UndependableFunc,
             SimpleVariable,
            Group,
            GroupOfGroups
        }

        public SparqlExpression()
        {
            AggregateLevel=VariableDependenceGroupLevel.Const;
            IsStoreUsed = false;
        }
        public SparqlExpression(VariableDependenceGroupLevel aggregateLevel, bool isStoreUsed)
        {
            AggregateLevel = aggregateLevel;
            IsStoreUsed = isStoreUsed;
        }

        public static VariableDependenceGroupLevel SetAggregateLevel(params VariableDependenceGroupLevel[] groupLevels)
        {
           var gg = groupLevels.Any(level => level == VariableDependenceGroupLevel.GroupOfGroups);
           var g = groupLevels.Any(level => level == VariableDependenceGroupLevel.Group);
           var s = groupLevels.Any(level => level == VariableDependenceGroupLevel.SimpleVariable);
           var u = groupLevels.Any(level => level == VariableDependenceGroupLevel.UndependableFunc);
           if (gg)
           {
               if(g || s)
                   throw new Exception("variable aggregation level");
               return  VariableDependenceGroupLevel.GroupOfGroups;
           }
           if (g)
           {
               if (s)
                   throw new Exception("variable aggregation level");
               return VariableDependenceGroupLevel.Group;
               
           }
           if (s)
           {
               return VariableDependenceGroupLevel.SimpleVariable; 
           }
           if (u)
           {
               return VariableDependenceGroupLevel.UndependableFunc;
               
           }
           return VariableDependenceGroupLevel.Const;

        }
        //internal static SparqlExpression EqualsExpression(SparqlExpression l, SparqlExpression r)
        //{
        //  return new SparqlEqualsExpression(l, r, TODO);
        //}

        //public static SparqlExpression NotEquals(SparqlExpression l, SparqlExpression r)
        //{
        // return new SparqlNotEqualsExpression(l, r);
        //}

      

        public static SparqlExpression Smaller(SparqlExpression l, SparqlExpression r)
        {
            var sparqlBinaryExpression = new SparqlBinaryExpression<OV_bool>(l, r, (o, o1) => o<o1, b=>new OV_bool(b));//((IComparable) o).CompareTo((IComparable)o1) == -1
         //   sparqlBinaryExpression.SetExprType(ObjectVariantEnum.Bool);
            return sparqlBinaryExpression;
        }
        


        public static SparqlExpression Greather(SparqlExpression l, SparqlExpression r)
        {
            //Func<object, object, object> @operator;
            //if (l.RealType == ObjectVariantEnum.Int || r.RealType == ObjectVariantEnum.Int)
            //    @operator = (o, o1) => (int)o > (int)o1;
            //else 
            //    if (l.RealType == ObjectVariantEnum.Date|| r.RealType == ObjectVariantEnum.Date)
            //    @operator = (o, o1) => (DateTimeOffset)o > (DateTimeOffset)o1;
            //else                                                          throw new NotImplementedException();
            var sparqlBinaryExpression = new SparqlBinaryExpression<OV_bool>(l, r, (o, o1) =>o>o1, b => new OV_bool(b));// ((IComparable)o).CompareTo(o1) == 1
       //     sparqlBinaryExpression.SetExprType(ObjectVariantEnum.Bool);
            return sparqlBinaryExpression;
        }

        internal static SparqlExpression SmallerOrEquals(SparqlExpression l, SparqlExpression r)
        {
            var sparqlBinaryExpression = new SparqlBinaryExpression<OV_bool>(l, r, (o, o1) => o<=o1, b => new OV_bool(b));//((IComparable)o).CompareTo(o1) != 1
          //  sparqlBinaryExpression.SetExprType(ObjectVariantEnum.Bool);
            return sparqlBinaryExpression;
        }

        public static SparqlExpression GreatherOrEquals(SparqlExpression l, SparqlExpression r)
        {
            var sparqlBinaryExpression = new SparqlBinaryExpression<OV_bool>(l, r, (o, o1) => o>=o1, b => new OV_bool(b));//((IComparable)o).CompareTo(o1) != -1
         //   sparqlBinaryExpression.SetExprType(ObjectVariantEnum.Bool);
            return sparqlBinaryExpression;
        }

        internal SparqlExpression InCollection(List<SparqlExpression> collection)
        {
            return new SparqlInCollectionExpression(this, collection);
        }

        internal SparqlExpression NotInCollection(List<SparqlExpression> collection)
        {
            var notInCollection = InCollection(collection);
            
            return notInCollection;
        }

        public static SparqlExpression operator +(SparqlExpression l, SparqlExpression r)
        {
        //    l.SetExprType(ExpressionTypeEnum.numeric);
            var sparqlBinaryExpression = new SparqlBinaryExpression(l, r, (o, o1) => o + o1);
            sparqlBinaryExpression.Create();
            //  sparqlBinaryExpression.SetExprType(l);            
            return sparqlBinaryExpression;
        }

        public static SparqlExpression operator -(SparqlExpression l, SparqlExpression r)
        {
           // l.SetExprType(ExpressionTypeEnum.numeric);
            var sparqlBinaryExpression = new SparqlBinaryExpression(l, r, (o, o1) => o - o1);
            sparqlBinaryExpression.Create();
            return sparqlBinaryExpression;
        }

        public static SparqlExpression operator *(SparqlExpression l, SparqlExpression r)
        {
         //   l.SetExprType(ExpressionTypeEnum.numeric);
            var sparqlBinaryExpression = new SparqlBinaryExpression(l, r, (o, o1) => o * o1);
            sparqlBinaryExpression.Create();
         //   sparqlBinaryExpression.SetExprType(l);
            return sparqlBinaryExpression;   
        }

        public static SparqlExpression operator /(SparqlExpression l, SparqlExpression r)
        {
          //  l.SetExprType(ExpressionTypeEnum.numeric);
            //but xsd:decimal if both operands are xsd:integeк

            var sparqlBinaryExpression = new SparqlDivideExpression(l, r);
          //  sparqlBinaryExpression.SetExprType(l);
            sparqlBinaryExpression.Create();
            return sparqlBinaryExpression;
        }

        public static SparqlExpression operator !(SparqlExpression e)
        {
        //    e.SetExprType(ObjectVariantEnum.@Bool);
            var opLogicalNot = new SparqlUnaryExpression(o => !o, e);
            //opLogicalNot.SetExprType(ObjectVariantEnum.Bool);

            return opLogicalNot;
        }

        public static SparqlExpression operator -(SparqlExpression e)
        {
         //   e.SetExprType(ExpressionTypeEnum.numeric);
            var uminus = new SparqlUnaryExpression(o => -o, e);
       //     uminus.SetExprType(e);
            return uminus;
        }

        internal bool Test(SparqlResult result)
        {
            return (bool)Operator(result);
        }

        //public Func<SparqlResult, ObjectVariants> FunkClone
        //{
        //    get
        //    {
        //        return (Func<SparqlResult, ObjectVariants>)TypedOperator.Clone();
        //    }
        //}

        public ObjectVariants Const { get; set; }

        public Func<SparqlResult, dynamic> Operator { get; set; }
        public readonly bool IsStoreUsed;

        // public void SetExprType(ObjectVariantEnum variant)
        // {
        //     RealType = variant;
        // }
        // public void SetExprType(ExpressionTypeEnum typeEnum)
        // {
        //     this.typeEnum = typeEnum;
        // }
        // private void SetExprType(SparqlExpression variant)
        // {

        // }                                       

        // private ExpressionTypeClass exprTypeObj;

        //public class ExpressionTypeClass
        //{
        //    private ExpressionTypeEnum generalType;
        //    public ObjectVariantEnum listType;
        //}        
        // public enum ExpressionTypeEnum
        // {
        //      numeric, 
        //     stringOrWithLang,
        //     literal,
        //     Date,
        // }

    }
}
