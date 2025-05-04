import jetbrains.buildServer.configs.kotlin.*
import jetbrains.buildServer.configs.kotlin.buildFeatures.perfmon
import jetbrains.buildServer.configs.kotlin.buildSteps.script

version = "2025.03"

project {

    buildType(UpmBuild_AndroidDebug)
}

object UpmBuild_AndroidDebug : BuildType({
    id("AndroidDebug")
    name = "Android - 底包 - Debug"

    params {
        select(
            name = "Mode",
            value = "Debug",
            options = listOf("Debug", "Release"))

        checkbox(
            name = "IsAppBundle",
            value = "false",
            checked = "true",
            unchecked = "false")
    }

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
                    --mode=%Mode% ^
                    --isAppBundle=%IsAppBundle%
            """.trimIndent()
        }
    }

    features {
        perfmon {
        }
    }
})
