using System.IO;
using Build.Editor;
using Editor.Build.Runner;
using UnityEditor;
using UnityEngine;

namespace Editor.Build.Steps
{
    public class BuildBundleStep : BaseBuildStep<BuildArgs>
    {
        public override void Execute()
        {
            string outputPath = Path.Combine(Application.streamingAssetsPath, "AssetBundles");
            if (Directory.Exists(outputPath))
                Directory.Delete(outputPath, true);
            
            Directory.CreateDirectory(outputPath);

            BuildAssetBundleOptions options = BuildAssetBundleOptions.StrictMode
                                              | BuildAssetBundleOptions.ChunkBasedCompression;

            BuildPipeline.BuildAssetBundles(
                outputPath,
                options,
                Args.GetBuildTarget());
        }
    }
}