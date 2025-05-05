import jetbrains.buildServer.configs.kotlin.*
import jetbrains.buildServer.configs.kotlin.buildSteps.script
import jetbrains.buildServer.configs.kotlin.buildTypeChartsOrder
import jetbrains.buildServer.configs.kotlin.vcs.GitVcsRoot

version = "2025.03"

project {

    vcsRoot(GitGithubComSibylNullUpmBuildGit)

    template(IosTemp)
    template(AndroidTemp)

    params {
        text("ProjectPath", "BuildSandbox", readOnly = true)
        text("UnityPath", """C:\Program Files\Unity\Hub\Editor\2022.3.24f1c1\Editor\Unity.exe""", readOnly = true)
    }

    subProject(Android)
    subProject(Ios)
}

object AndroidTemp : Template({
    name = "AndroidTemp"

    artifactRules = """
        +:build/new/*.aab => Android/%Mode%
        +:build/new/*.apk => Android/%Mode%
    """.trimIndent()
    publishArtifacts = PublishMode.SUCCESSFUL

    vcs {
        root(GitGithubComSibylNullUpmBuildGit)
    }

    steps {
        script {
            name = "UnityBuildRunner"
            id = "UnityBuildRunner"
            scriptContent = """
                "%UnityPath%" ^
                  -projectPath "%ProjectPath%" ^
                  -executeMethod Editor.Build.Runner.BuildRunner.RunByCiCd ^
                  -quit ^
                  -batchmode ^
                  -buildTarget Android ^
                  -logFile - ^
                  -- ^
                  --appStore=%AppStore% ^
                  --mode=%Mode% ^
                  --behaviors=%Behaviors%
            """.trimIndent()
        }
        script {
            name = "Notification"
            id = "Notification"
            scriptContent = """echo "构建连接: %env.BUILD_URL%""""
        }
        script {
            name = "AabToApk"
            id = "AabToApk"

            conditions {
                equals("Behaviors", "BuildPlayer")
                equals("AppStore", "Google")
            }
            scriptContent = """echo "AabToApk Test""""
        }
    }

    requirements {
        equals("teamcity.agent.name", "LAPTOP-IC5SMBP1", "RQ_1")
    }
})

object IosTemp : Template({
    name = "IosTemp"

    vcs {
        root(GitGithubComSibylNullUpmBuildGit)
    }

    steps {
        script {
            name = "UnityBuildRunner"
            id = "UnityBuildRunner"
            scriptContent = """
                "%UnityPath%" ^
                  -projectPath "%ProjectPath%" ^
                  -executeMethod Editor.Build.Runner.BuildRunner.RunByCiCd ^
                  -quit ^
                  -batchmode ^
                  -buildTarget iOS ^
                  -logFile - ^
                  -- ^
                  --appStore=%AppStore% ^
                  --mode=%Mode% ^
                  --behaviors=%Behaviors%
            """.trimIndent()
        }
        script {
            name = "Notification"
            id = "Notification"
            scriptContent = """echo "构建连接: %env.BUILD_URL%""""
        }
        script {
            name = "Xcode"
            id = "Xcode"
            scriptContent = """echo "假装是 xcode 构建""""
        }
    }

    requirements {
        equals("teamcity.agent.name", "LAPTOP-IC5SMBP1", "RQ_1")
    }
})

object GitGithubComSibylNullUpmBuildGit : GitVcsRoot({
    name = "git@github.com:Sibyl-null/UPM-Build.git"
    url = "git@github.com:Sibyl-null/UPM-Build.git"
    branch = "refs/heads/main"
    branchSpec = "+:refs/heads/*"
    agentCleanPolicy = GitVcsRoot.AgentCleanPolicy.ALWAYS
    checkoutPolicy = GitVcsRoot.AgentCheckoutPolicy.USE_MIRRORS
    authMethod = defaultPrivateKey {
        userName = "git"
    }
})


object Android : Project({
    name = "Android"

    buildType(Android_Debug)
    buildType(Android_Release)

    features {
        buildTypeChartsOrder {
            id = "PROJECT_EXT_4"
            order = listOf(
                "_Root:BuildDurationNetTime",
                "_Root:SuccessRate",
                "_Root:TestCount",
                "_Root:MaxTimeToFixTestGraph",
                "_Root:InspectionStats",
                "_Root:CodeCoverage",
                "_Root:VisibleArtifactsSize",
                "_Root:TimeSpentInQueue"
            )
        }
    }
})

object Android_Debug : BuildType({
    name = "Debug"

    params {
        select("Behaviors", "BuildBundles", label = "构建行为", display = ParameterDisplay.PROMPT,
            options = listOf("Bundle" to "BuildBundles", "底包" to "BuildPlayer"))
        select("AppStore", "Google", label = "目标渠道",
            options = listOf("Google"))
    }

    vcs {
        root(GitGithubComSibylNullUpmBuildGit)
    }

    steps {
        script {
            name = "UnityBuildRunner"
            id = "UnityBuildRunner"
            scriptContent = """
                "%UnityPath%" ^
                  -projectPath "%ProjectPath%" ^
                  -executeMethod Editor.Build.Runner.BuildRunner.RunByCiCd ^
                  -quit ^
                  -batchmode ^
                  -buildTarget Android ^
                  -logFile - ^
                  -- ^
                  --appStore=%AppStore% ^
                  --mode=Debug ^
                  --behaviors=%Behaviors%
            """.trimIndent()
        }
        script {
            name = "Notification"
            id = "Notification"
            scriptContent = """echo "构建连接: %env.BUILD_URL%""""
        }
    }
})

object Android_Release : BuildType({
    templates(AndroidTemp)
    name = "Release"

    params {
        select("Behaviors", "BuildBundles", display = ParameterDisplay.PROMPT,
            options = listOf("Bundle" to "BuildBundles", "底包" to "BuildPlayer"))
        select("AppStore", "Google",
            options = listOf("Google"))
        select("Mode", "Release", readOnly = true,
            options = listOf("Debug", "Release"))
    }
})


object Ios : Project({
    name = "Ios"

    buildType(Ios_Debug)
    buildType(Ios_Release)
})

object Ios_Debug : BuildType({
    templates(IosTemp)
    name = "Debug"

    params {
        select("Behaviors", "BuildBundles", label = "构建行为", display = ParameterDisplay.PROMPT,
            options = listOf("Bundle" to "BuildBundles", "底包" to "BuildPlayer"))
        select("AppStore", "Apple", label = "目标渠道",
            options = listOf("Apple"))
        select("Mode", "Debug", label = "构建环境", readOnly = true,
            options = listOf("Debug", "Release"))
    }
})

object Ios_Release : BuildType({
    templates(IosTemp)
    name = "Release"

    params {
        select("Behaviors", "BuildBundles", label = "构建行为", display = ParameterDisplay.PROMPT,
            options = listOf("Bundle" to "BuildBundles", "底包" to "BuildPlayer"))
        select("AppStore", "Apple", label = "目标渠道",
            options = listOf("Apple"))
        text("PromptConfirmation", "注意当前处于 Release 构建", label = "确认提示", display = ParameterDisplay.PROMPT, readOnly = true, allowEmpty = true)
        select("Mode", "Release", label = "构建环境", readOnly = true,
            options = listOf("Debug", "Release"))
    }
})
