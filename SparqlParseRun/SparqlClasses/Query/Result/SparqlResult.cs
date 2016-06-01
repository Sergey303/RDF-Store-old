using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;
using SparqlParseRun.SparqlClasses.GraphPattern.Triples.Node;

namespace SparqlParseRun.SparqlClasses.Query.Result
{
    public class SparqlResult
    {
        //public bool result;
     //   private readonly Dictionary<VariableNode, ObjectVariants> row;
        

        public SparqlResult Add(ObjectVariants newObj, VariableNode variable)
        {   
            rowArray[variable.Index] = newObj;
            return this;
        }
        public SparqlResult Add(ObjectVariants newObj1, VariableNode variable1, ObjectVariants newObj2, VariableNode variable2)
        {
            rowArray[variable1.Index] = newObj1;
            rowArray[variable2.Index] = newObj2;
            return this;
        }

        public SparqlResult(RdfQuery11Translator q)
        {
            this.q = q;
            id = new Lazy<string>(() => this.q.Store.NodeGenerator.BlankNodeGenerateNums());
            rowArray = new ObjectVariants[q.Variables.Count];
        }



        public ObjectVariants this[VariableNode var]
        {
            get
            {
                return rowArray[var.Index];
            }
            set
            {
                rowArray[var.Index] = value;

            }
        }

        public ObjectVariants this[int index]
        {
            get
            {
                return rowArray[index];
            }
            set
            {
                rowArray[index] = value;

            }
        }
     
        public bool ContainsKey(VariableNode var)
        {
            return rowArray[var.Index]!=null;
        }

        //public bool Equals(SparqlResult other)
        //{
        //    // return ((IStructuralComparable) row).CompareTo(other.row, Comparer<INode>.Default)==0;

        //    return TestAll((var, value) => value.Equals(other[var]));
        //}

        //public override bool Equals(object obj)
        //{
        //    var sparqlResult = obj as SparqlResult;
        //    return sparqlResult != null && Equals(sparqlResult);
        //}

      

        public override int GetHashCode()
        {
            unchecked
            {
                int i = 0;
                return selected.Aggregate(0, (current, selectVar) => current*(int) Math.Pow(4*(i++) + 5, this[selectVar] == null ? 0 : this[selectVar].GetHashCode()));
            }
        }

        public override bool Equals(object obj)
        {
            var sparqlResult = obj as SparqlResult;
            return sparqlResult != null && selected.All(v => sparqlResult[v].Equals(this[v]));
        }

        private readonly ObjectVariants[] rowArray;

        public SparqlResult Add(ObjectVariants newObj1, VariableNode variable1, ObjectVariants newObj2, VariableNode variable2, ObjectVariants newObj3, VariableNode variable3)
        {
            rowArray[variable1.Index] = newObj1;
            rowArray[variable2.Index] = newObj2;
            rowArray[variable3.Index] = newObj3;
            return this;
        }

        public SparqlResult Add(ObjectVariants newObj1, VariableNode variable1, ObjectVariants newObj2, VariableNode variable2, ObjectVariants newObj3, VariableNode variable3, ObjectVariants arg4, VariableNode variable4)
        {
            rowArray[variable1.Index] = newObj1;
            rowArray[variable2.Index] = newObj2;
            rowArray[variable3.Index] = newObj3;
            rowArray[variable4.Index] = arg4;
            return this;
        }

        public SparqlResult Add(IEnumerable<KeyValuePair<VariableNode, ObjectVariants>> copy)
        {
            foreach (var pair in copy)
                Add(pair.Value, pair.Key);
            return this;
        }

        public void Add(VariableNode variable, ObjectVariants value)
        {
         rowArray[variable.Index]= value;
        }

        //public IEnumerable<T> GetAll<T>(Func<VariableNode,ObjectVariants, T> selector)
        //{
        //    return q.Variables.Values.Select(v => selector(v, rowArray[v.Index]));
        //}

        public bool TestAll(Func<VariableNode, ObjectVariants, bool> selector)
        {
            return q.Variables.Values.All(v => selector(v, rowArray[v.Index]));
        }

     
        public void SetSelection(IEnumerable<VariableNode> selected)
        {
            this.selected = selected;
        }

        public IEnumerable<T> GetSelected<T>(Func<VariableNode, ObjectVariants, T> selector)
        {
            return selected.Where(v=>rowArray[v.Index]!=null)
                .Select(v => selector(v, rowArray[v.Index]));
        }

       // private readonly Dictionary<string, VariableNode> Variables;

        private IEnumerable<VariableNode> selected;
        private readonly RdfQuery11Translator q;

        private SparqlResult(ObjectVariants[] copy, RdfQuery11Translator q)
        {
            id = new Lazy<string>(() => this.q.Store.NodeGenerator.BlankNodeGenerateNums());
            this.q = q;
            rowArray = copy;
        }

        //public IEnumerator<SparqlResult> Branching() 
        //public bool[] BackupMask()
        //{
        //    return rowArray.Select(v => v != null).ToArray();
        //}

        //public void Restore(bool[] backup)
        //{
        //    for (int i = 0; i < backup.Length; i++)
        //    {
        //        if(backup[i]) continue;
        //        rowArray[i] = null;
        //    }
        //}


        public SparqlResult Clone()
        {
            ObjectVariants[] copy=new ObjectVariants[rowArray.Length];
            rowArray.CopyTo(copy,0);
            return new SparqlResult(copy, q){selected=selected};
        }

        public string Id {get { return id.Value; }}
       private readonly Lazy<string> id;
    }
}