using RDFCommon;

namespace SparqlParseRun.SparqlClasses.Update
{
    public abstract class SparqlUpdateSilent : ISparqlUpdate       
    {
        protected bool IsSilent;

        internal void Silent()
        {
            IsSilent = true;
        }

        public abstract void RunUnSilent(IStore store);

        public void Run(IStore store)
        {
            if (IsSilent)
                try
                {
                    RunUnSilent(store);
                }
                catch //TODO
                {
                }
            else
                RunUnSilent(store);
        }
    }

    public interface ISparqlUpdate
        {
            void Run(IStore store);
        }
    }

