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

    vcs {
        root(DslContext.settingsRoot)
    }
    
    params {
        select("Mode", "Mode", options = listOf("Debug", "Release"))
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
                    --appStore Google ^
                    --mode %Mode% ^
                    --isAppBundle false
            """.trimIndent()
        }
    }

    features {
        perfmon {
        }
    }
})
