namespace Build.Editor
{
    public interface IBuildArgs
    {
        public const string ContextKey = "BuildArgs";
        
        void Init();
    }
}