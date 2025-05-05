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
        private BuildBehaviors _behaviors;
        
        private void OnGUI()
        {
            _appStore = (BuildAppStore)EditorGUILayout.EnumPopup("App Store", _appStore);
            _mode = (BuildMode)EditorGUILayout.EnumPopup("Build Mode", _mode);
            _behaviors = (BuildBehaviors)EditorGUILayout.EnumPopup("Build Behaviors", _behaviors);

            EditorGUILayout.Space();
            if (GUILayout.Button("Build"))
            {
                BuildArgs args = new BuildArgs
                {
                    AppStore = _appStore,
                    Mode = _mode,
                    Behaviors = _behaviors
                };
                
                BuildRunner.RunByEditor(args);
            }
        }
    }
}