module.exports = function () {
    var webroot = "C:\\inetpub\\wwwroot";
    var config = {
        sitecoreRoot: webroot + "\\xp901.sc\\Website",
        engineAuthoringRoot: webroot + "\\CommerceAuthoring_Sc9",
        engineShopsRoot: webroot + "\\CommerceAuthoring_Sc9",
        engineOpsRoot: webroot + "\\CommerceAuthoring_Sc9",
        engineMinionsRoot: webroot + "\\CommerceAuthoring_Sc9",
        solutionName: "Rob.Commerce",
        buildConfiguration: "Debug",
        buildToolsVersion: 15.0,
        buildMaxCpuCount: 0,
        buildVerbosity: "minimal",
        buildPlatform: "Any CPU",
        publishPlatform: "AnyCpu",
        runCleanBuilds: true
    };
    return config;
}