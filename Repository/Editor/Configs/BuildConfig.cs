﻿using System;
using UnityEditor;
using UnityEngine;

namespace Build.Editor.Configs
{
    [CreateAssetMenu(menuName = "Meevii/BuildConfig")]
    public class BuildConfig : ScriptableObject
    {
        public BuildAppStore AppStore;
        public int VersionCode = 1;
        public string Version = "0.1.0";

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