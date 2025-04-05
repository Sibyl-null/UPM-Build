using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Build.Editor;
using Build.Editor.Contexts;
using Editor.Build.Runner;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace Editor.Build.Steps
{
    public class IosPostProcessStep : BaseBuildStep<BuildArgs>
    {
        public override Task Execute()
        {
            BuildReport report = Context.Get<BuildReport>(BuiltinContextKey.BuildReport);
            string path = report.summary.outputPath;
            
            CopyPodfile(path);
            SetXcodePBXInfo(path);
            SetTransportSecurityForEnableHttp(path);
            SetEntitlementsForLuid(path);
            EnableFirebaseDebugView(path);

            WriteIpaName();
            WriteBuildInfo();
            
            OnCustomPostProcess();
            return Task.CompletedTask;
        }
        
        protected virtual void CopyPodfile(string path)
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

        protected virtual void SetXcodePBXInfo(string path)
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
#if UNITY_2021_1_OR_NEWER
            pbxProject.SetBuildProperty(frameworkTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
#endif

            pbxProject.WriteToFile(pbxPath);
        }

        protected virtual void SetTransportSecurityForEnableHttp(string path)
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

        protected virtual void SetEntitlementsForLuid(string path)
        {
            string pbxPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
            string[] nameArray = PlayerSettings.applicationIdentifier.Split('.');
            string appName = nameArray[^1];
            string filePath = $"Unity-iPhone/{appName}.entitlements";
            
            ProjectCapabilityManager projCapability = new ProjectCapabilityManager(pbxPath, filePath, "Unity-iPhone");
            projCapability.AddKeychainSharing(new[] {"$(AppIdentifierPrefix)com.learningsSharingGroup"});
            projCapability.AddPushNotifications(true);
            projCapability.WriteToFile();
        }

        protected virtual void EnableFirebaseDebugView(string path)
        {
            string schemePath = path + "/Unity-iPhone.xcodeproj/xcshareddata/xcschemes/Unity-iPhone.xcscheme";
            XmlDocument schemeProject = new XmlDocument();
            schemeProject.Load(schemePath);
            
            XmlNode launchNode = schemeProject.SelectSingleNode("Scheme/LaunchAction");
            XmlNode commandsNode = launchNode.SelectSingleNode("CommandLineArguments");
            if (commandsNode == null)
            {
                commandsNode = schemeProject.CreateElement("CommandLineArguments");
                launchNode.AppendChild(commandsNode);
            }

            if (FirebaseDebugViewConstraint())
            {
                XmlElement debugElem = schemeProject.CreateElement("CommandLineArgument");
                debugElem.SetAttribute("argument", "-FIRDebugEnabled");
                debugElem.SetAttribute("isEnabled", "YES");
                commandsNode.AppendChild(debugElem);

                XmlElement argElem = schemeProject.CreateElement("CommandLineArgument");
                argElem.SetAttribute("argument", "-FIRAnalyticsDebugEnabled");
                argElem.SetAttribute("isEnabled", "YES");
                commandsNode.AppendChild(argElem);
            }
            else
            {
                var debugElem = schemeProject.CreateElement("CommandLineArgument");
                debugElem.SetAttribute("argument", "-FIRDebugDisabled");
                debugElem.SetAttribute("isEnabled", "YES");
                commandsNode.AppendChild(debugElem);
            }

            schemeProject.Save(schemePath);
        }

        protected virtual bool FirebaseDebugViewConstraint()
        {
            return Args.IsDebug;
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

        private void WriteBuildInfo()
        {
            string tag = Args.IsDebug ? "dev" : "rel";
            string infoStr = $"{Args.Config.AppVersion} {tag}";
            
            string exportPath = "../build/new";
            string infoPath = Path.Combine(exportPath, "BuildInfoTemp");
            File.WriteAllText(infoPath, infoStr);
        }
        
        protected virtual void OnCustomPostProcess()
        {
        }
    }
}