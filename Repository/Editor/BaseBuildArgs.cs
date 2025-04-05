using CommandLine;

namespace Build.Editor
{
    public abstract class BaseBuildArgs
    {
        public const string ContextKey = "BuildArgs";
        
        // Unity 命令行参数
        [Option('p')] public string ProjectPath { get; set; }
        [Option('e')] public string ExecuteMethod { get; set; }
        [Option('b')] public bool BatchMode { get; set; }
        [Option('l')] public string LogFile { get; set; }
        [Option('q')] public string Quit { get; set; }

        public abstract void Init();
    }
}