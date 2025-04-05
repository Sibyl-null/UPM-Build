using UnityEditor;
using UnityEngine;

namespace Editor.Build.Runner
{
    public class BuildWindow : EditorWindow
    {
        [MenuItem("Tools/Open Build Window")]
        public static void Open()
        {
            GetWindow<BuildWindow>();
        }

        private BuildAppStore _appStore;
        private BuildMode _mode;
        private bool _isAppBundle;
        
        private void OnGUI()
        {
            _appStore = (BuildAppStore)EditorGUILayout.EnumPopup("App Store", _appStore);
            _mode = (BuildMode)EditorGUILayout.EnumPopup("Build Mode", _mode);

            if (_appStore == BuildAppStore.Google)
                _isAppBundle = GUILayout.Toggle(_isAppBundle, "Is App Bundle");

            EditorGUILayout.Space();
            if (GUILayout.Button("Build"))
            {
                BuildArgs args = new BuildArgs
                {
                    AppStore = _appStore,
                    Mode = _mode,
                    IsAppBundle = _isAppBundle
                };
                
                BuildRunner.RunByEditor(args);
            }
        }
    }
}