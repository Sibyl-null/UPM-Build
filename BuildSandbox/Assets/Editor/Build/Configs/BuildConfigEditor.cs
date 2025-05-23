﻿using Editor.Build.Runner;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Build.Configs
{
    [CustomEditor(typeof(BuildConfig), true)]
    public class BuildConfigEditor : UnityEditor.Editor
    {
        [SerializeField] VisualTreeAsset _editorAsset;
        
        private BuildConfig Target => (BuildConfig)target;
        
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = _editorAsset.CloneTree();

            EnumField appStoreField = root.Q<EnumField>("AppStoreField");
            appStoreField.bindingPath = nameof(Target.AppStore);

            IntegerField versionCodeField = root.Q<IntegerField>("VersionCodeField");
            versionCodeField.bindingPath = nameof(Target.VersionCode);

            TextField versionField = root.Q<TextField>("AppVersionField");
            versionField.bindingPath = nameof(Target.AppVersion);

            Button syncButton = root.Q<Button>("SyncButton");
            syncButton.clicked += OnSyncClick;
            
            return root;
        }

        private void OnSyncClick()
        {
            bool isApple = Target.AppStore == BuildAppStore.Apple;
            if (isApple)
            {
                PlayerSettings.iOS.buildNumber = Target.VersionCode.ToString();
            }
            else
            {
                PlayerSettings.Android.bundleVersionCode = Target.VersionCode;
            }

            PlayerSettings.bundleVersion = Target.AppVersion;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[BuildConfigEditor] Update version is success");
        }
    }
}