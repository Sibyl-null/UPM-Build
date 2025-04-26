import jetbrains.buildServer.configs.kotlin.*
import jetbrains.buildServer.configs.kotlin.buildFeatures.perfmon
import jetbrains.buildServer.configs.kotlin.buildSteps.script

/*
The settings script is an entry point for defining a TeamCity
project hierarchy. The script should contain a single call to the
project() function with a Project instance or an init function as
an argument.

VcsRoots, BuildTypes, Templates, and subprojects can be
registered inside the project using the vcsRoot(), buildType(),
template(), and subProject() methods respectively.

To debug settings scripts in command-line, run the

    mvnDebug org.jetbrains.teamcity:teamcity-configs-maven-plugin:generate

command and attach your debugger to the port 8000.

To debug in IntelliJ Idea, open the 'Maven Projects' tool window (View
-> Tool Windows -> Maven Projects), find the generate task node
(Plugins -> teamcity-configs -> teamcity-configs:generate), the
'Debug' option is available in the context menu for the task.
*/

version = "2025.03"

project {

    buildType(UpmBuild_AndroidDebug)
}

object UpmBuild_AndroidDebug : BuildType({
    id("AndroidDebug")
    name = "Android - 底包 - Debug"

    vcs {
        root(DslContext.settingsRoot)
    }

    steps {
        script {
            name = "UnityPack"
            id = "UnityPack"
            scriptContent = """
                "C:\Program Files\Unity\Hub\Editor\2022.3.24f1c1\Editor\Unity.exe" ^
                	-projectPath "BuildSandbox" ^
                	-executeMethod Editor.Build.Runner.BuildRunner.RunByCiCd ^
                    -quit ^
                	-batchmode ^
                    -buildTarget Android ^
                    -logFile - ^
                    -- ^
                    --appStore=Google ^
                    --mode=Debug ^
                    --isAppBundle=false
            """.trimIndent()
        }
    }

    features {
        perfmon {
        }
    }
})
