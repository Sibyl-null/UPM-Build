# Build-Unity

package: `com.learnings.unity.build`

Build package for unity

### 目录

[TOC]

### 前言

### 使用说明

操作步骤

        1 根据平台创建脚本，目标路径 Assets/Scripts/Builder/Editor，例如
            GoogleProcess : AbstractAndroidProcess
            AppleProcess : AbstractIOSProcess

        2 创建配置，目标路径 Assets/Scripts/Builder/Config，菜单 Assets->Create->Meevii->BuildConfig
            AppStore：根据商店选取
            VersionCode：版本代码
            Version：版本号
            ProcessFullName：第一步中创建的脚本全名称，例如 Builder.Editor.GoogleProcess
            Sync ProjectSettings：同步版本代码和版本号到 ProjectSettings，方便调试
          其中 VersionCode 和 Version 默认在这里设置，如果通过 Jenkins 控制，覆写 Process 中的 Init 方法

        3 在库中复制流水线，目标路径 Assets/Scripts/Builder/Pipeline，根据产品修改其中「根据配置修改」部分的参数
            AndroidPipeline.jenkinsfile
            iOSPipeline.jenkinsfile

            Android 参数：
            PROJECT_NAME = "tripeaks"
            GIT_PATH = "ssh://ci-agent@gerrit.lexinshengwen.com:29418/${PROJECT_NAME}"
            GIT_CERT = "ci-agent-gerrit"

            UNITY_PATH = "${UNITY2021_3_21}"
            EXECUTE_PATH = "tripeaks-unity"
            EXECUTE_METHOD = "Build.Editor.BuildManager.BuildAuto"
            EXPORT_PATH = "build/new"
            CACHE_PATH = "build"
            BUILD_JSON_PATH = "~/.${PROJECT_NAME}-cache/build-size-android.json"

            BUNDLE_TOOL_PATH = "jenkins/bundletool-all-1.8.2.jar"
            KEYSTORE_PATH = "jenkins/keystore/ywkj.keystore"
            FIREBASE_APP_ID = "google-services.json 中的 mobilesdk_app_id"
            FIREBASE_TOKEN = "安装工具 sudo npm install -g firebase-tools，通过 firebase login:ci 命令生成"

            ROBOT_TOKEN = "飞书机器人 ID，关键词包含'更新'和'包'"
            CI_URL = "https://ci.lexinshengwen.com 打包机地址"
            FIR_URL = "http://fir.lexinshengwen.com/xxxx 对应产品唯一编码"

            iOS 参数：
            PROJECT_NAME = "tripeaks"
            GIT_PATH = "ssh://ci-agent@gerrit.lexinshengwen.com:29418/${PROJECT_NAME}"
            GIT_CERT = "ci-agent-gerrit"

            UNITY_PATH = "${UNITY2021_3_21}"
            EXECUTE_PATH = "tripeaks-unity"
            EXECUTE_METHOD = "Build.Editor.BuildManager.BuildAuto"
            EXPORT_PATH = "build/new"
            CACHE_PATH = "build"
            BUILD_JSON_PATH = "~/.${PROJECT_NAME}-cache/build-size-ios.json"
            ADREVIEW_KEY = "iny0rUIa1b4uEq5K2eOLKffFG5AiIeJR3HTWuLV9T1oEQQoWQb89XB1wyqmNVcrevnHSwXTqTBs0_P3ae9Iygx 不同项目可能有区别，需要和产品确认"

            IPA_OUTPUT_NAME = "包名最后一个单词；Unity2021 之后是产品名称删除空格"
            EXPORT_PLIST_AD_HOC = "jenkins/plist/ad-hoc.plist"
            EXPORT_PLIST_APP_STORE = "jenkins/plist/app-store.plist"
            UPLOAD_TOKEN = "-u xxx@learnings.ai -p zqnq-acyo-psqt-tgfo"

            ROBOT_TOKEN = "飞书机器人 ID，关键词包含'更新'和'包'"
            CI_URL = "https://ci.lexinshengwen.com 打包机地址"
            FIR_URL = "http://fir.lexinshengwen.com/xxxx 对应产品唯一编码"

        4 在库中复制对比包体大小的 python 脚本，源路径 Jenkins/compare-size.py ，目标路径为项目工程的根目录

        5 配置密钥和工具，路径注意大小写。bundle tool 和 plist 库中有模版
            5-1-1 Android Bundle Tool。默认路径 jenkins/bundletool-all-1.8.2.jar，需要 Jenkins 中同步
            5-1-2 Android Keystore。默认路径 jenkins/keystore/ywkj.keystore，需要 Jenkins 中同步

            5-2-1 iOS plist 模版。默认路径 jenkins/plist/ad-hoc.plist 和 jenkins/plist/app-store.plist，
                  需要 Jenkins 中同步。修改 plist 模版中 teamID
            5-2-2 iOS 设置 PlayerSettings 中的 Signing Team ID
            5-2-3 Podfile 路径 Assets/Plugins/iOS/Podfile

        6 配置 Jenkins
            6-1 创建新的 Jenkins 项目，类型选择流水线。也可以选择已经存在的项目通过模版创建
            6-2 第一次运行，Pipeline 类型选择 Pipeline script, 把 *.jenkinsfile 中的代码复制到此处，
                勾选 onlyCheckout 运行
            6-3 然后 Pipeline 类型选择 Pipeline script form SCM，SCM 选择 Git，配置参数
                  URL：ssh://ci-agent@gerrit.lexinshengwen.com:29418/XXX
                  Credentials：根据项目选择，例如 Gerrit 选择 ci-agent-gerrit
                  脚本路径：例如 unity/Assets/Scripts/Builder/Pipeline/*.jenkinsfile
            6-4 配置完成，以后每次打包会先更新流水线

注意事项

    All

        1 打包流程默认不包含拉取 BI 配置，需要手动添加。提供按类型获取 BIUpdateConfig 的方法 GetBiConfig<T>()
        2 自动添加预处理命令
            DEV_BUILD：测试包生效
            MV_DEBUG_CONSOLE：测试工具，测试包生效
            ENABLE_CODECOVERAGE：代码覆盖率工具，测试包生效
            RELEASE_CONFIG：Jenkins 测试包生效，C# 配置时请只在测试包生效
            XXX_BUILD：根据商店自动添加，例如 GOOGLE_BUILD，AMAZON_BUILD，APPLE_BUILD
        3 打包导出路径 ../build/new，缓存路径 ../build，不建议修改，需要 Jenkins 中同步

    Android

        符号表：
        1 Crashlytics Unity SDK v8.6.1 或更高版本
        2 默认只在 Google Release 打包中生成符号表，防止上传到错误的 Firebase。其他商店业务方自行配置
        3 Firebase 上传符号表需要在打包机安装环境 sudo npm install -g firebase-tools。命令 firebase login:ci
          生成 Token (本地生成也可以)，可能需要开启代理，安装遇到问题见[参考]链接
        4 TargetSDK 30 以上需要在 AndroidManifest.xml 中增加新标签(Crashlytics Unity SDK v8.8.0 及以上可以移除标签)：
          <application android:allowNativeHeapPointerTagging="false"/>

    iOS

        证书：
        1 打包机如果有产品对应权限的账号，更新即可，注意账号总证书数量达到上限无法更新。也可以在其他机器上复制证书
        2 证书账号可以和上传账号可以不是同一个，但新增账号则需要必须更新证书才会生效，需要生成专用密钥
        3 导出 ipa 时访问证书可能需要输入登录密码，配置 Keychain Access->双击证书子项->Access Control->
          Allow all application to access this item->Save Changes 即可

        Firebase DebugView
        1 Debug 包默认开启，Release 包默认关闭

        Firebase Symbols
        1 理论上 Crashlytics v8.2.0+ 版本会自动上传符号表，所以此时不需要手动执行流水线 UploadSymbols 步骤，需验证

参考

[Jenkins 语法](https://www.jenkins.io/doc/book/pipeline/syntax/)

[飞书开放文档](https://open.feishu.cn/document/client-docs/bot-v3/add-custom-bot)

[Firebase 符号表配置](https://firebase.google.com/docs/crashlytics/get-deobfuscated-reports?platform=unity&authuser=0#android)

[iOS App 专用密码](https://support.apple.com/zh-cn/HT204397)

### 运行示例

### issues