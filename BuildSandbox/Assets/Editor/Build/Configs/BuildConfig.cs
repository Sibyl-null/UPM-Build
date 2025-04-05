using System;
using Editor.Build.Runner;
using UnityEditor;
using UnityEngine;

namespace Editor.Build.Configs
{
    [CreateAssetMenu]
    public class BuildConfig : ScriptableObject
    {
        public BuildAppStore AppStore;
        public int VersionCode = 1;
        public string AppVersion = "0.1.0";

        public static BuildConfig Get(BuildAppStore platform)
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(BuildConfig)}");
            foreach (string guid in guids)
            {
                BuildConfig config = AssetDatabase.LoadAssetAtPath<BuildConfig>(AssetDatabase.GUIDToAssetPath(guid));
                if (config.AppStore == platform)
                    return config;
            }

            throw new Exception("Not Find BuildConfig: " + platform);
        }
    }
}