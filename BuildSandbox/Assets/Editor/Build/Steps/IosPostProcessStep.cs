using System;
using System.IO;
using Build.Editor;
using Editor.Build.Runner;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Editor.Build.Steps
{
    public class IosPostProcessStep : BaseBuildStep<BuildArgs>
    {
        public override void Execute()
        {
            BuildReport report = Context.Get<BuildReport>(BuildContextKey.BuildReport);
            string path = report.summary.outputPath;
            
            CopyPodfile(path);
            SetXcodePBXInfo(path);
            SetTransportSecurityForEnableHttp(path);

            WriteIpaName();
        }
        
        private void CopyPodfile(string path)
        {
            if (File.Exists("Assets/Plugins/iOS/Podfile"))
            {
                string destFile = Path.Combine(path, "Podfile");
                File.Copy("Assets/Plugins/iOS/Podfile", destFile, true);
                Debug.Log("Copying iOS Podfile succeed");
            }
            else
            {
                Debug.LogWarning("Copying iOS Podfile failed");
            }
        }

        private void SetXcodePBXInfo(string path)
        {
            string pbxPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(pbxPath);

            // 设置证书
            string target = pbxProject.GetUnityMainTargetGuid();
            pbxProject.SetBuildProperty(target, "DEVELOPMENT_TEAM", PlayerSettings.iOS.appleDeveloperTeamID);

            // 禁用代码压缩
            string frameworkTarget = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.SetBuildProperty(frameworkTarget, "ENABLE_BITCODE", "NO");

            // 解决 Unity2021 上传 TestFlight 失败的问题
            pbxProject.SetBuildProperty(frameworkTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
            pbxProject.WriteToFile(pbxPath);
        }

        private void SetTransportSecurityForEnableHttp(string path)
        {
            string plistPath = path + "/Info.plist";
            PlistDocument plistDoc = new PlistDocument();
            plistDoc.ReadFromString(File.ReadAllText(plistPath));
            
            PlistElementDict plistRootDict = plistDoc.root;
            plistRootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);
            
            PlistElementDict securityDict = plistRootDict["NSAppTransportSecurity"].AsDict();
            securityDict.values.Clear();
            securityDict.SetBoolean("NSAllowsArbitraryLoads", true);
            
            File.WriteAllText(plistPath, plistDoc.WriteToString());
        }

        private void WriteIpaName()
        {
            string appName = PlayerSettings.productName.Replace(" ", "_").ToLower();
            string tag = Args.IsDebug ? "dev" : "rel";
            string time = DateTime.Now.ToString("MMdd_HHmm");
            string ipaName = $"{appName}_{tag}_v{Args.Config.AppVersion}_r{Args.Config.VersionCode}_{time}";
            
            string exportPath = "../build/new";
            string ipaPath = Path.Combine(exportPath, "IpaName");
            File.WriteAllText(ipaPath, ipaName);
        }
    }
}