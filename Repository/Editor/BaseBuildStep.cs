using System.Threading.Tasks;
using Build.Editor.Contexts;

namespace Build.Editor
{
    public interface IBuildStep
    {
        void Init(BuildContext context);
        Task Execute();
    }
    
    public abstract class BaseBuildStep<TArgs> : IBuildStep where TArgs : IBuildArgs
    {
        protected TArgs Args { get; private set; }
        protected BuildContext Context { get; private set; }

        void IBuildStep.Init(BuildContext context)
        {
            Context = context;
            Args = context.Get<TArgs>(IBuildArgs.ContextKey);
        }

        public abstract Task Execute();
    }
}