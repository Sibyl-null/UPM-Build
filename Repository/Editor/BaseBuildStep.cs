using System.Threading.Tasks;
using Build.Editor.Configs;
using Build.Editor.Contexts;

namespace Build.Editor
{
    public interface IBuildStep
    {
        void Init(BuildContext context);
        Task Execute();
    }
    
    public abstract class BaseBuildStep<TArgs> : IBuildStep where TArgs : BaseBuildArgs
    {
        protected TArgs Args { get; private set; }
        protected BuildConfig Config { get; private set; }
        protected BuildContext Context { get; private set; }

        void IBuildStep.Init(BuildContext context)
        {
            Context = context;
            Args = context.Get<TArgs>(BuiltinContextKey.BuildArgs);
            Config = context.Get<BuildConfig>(BuiltinContextKey.BuildConfig);
        }

        public abstract Task Execute();
    }
}