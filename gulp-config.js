module.exports = function () {
    var webroot = "C:\\inetpub\\wwwroot";
    var config = {
        sitecoreRoot: webroot + "\\xp901.sc\\Website",
        engineAuthoringRoot: webroot + "\\CommerceAuthoring_Sc9",
        engineShopsRoot: webroot + "\\CommerceShops_Sc9",
        engineOpsRoot: webroot + "\\CommerceOps_Sc9",
        engineMinionsRoot: webroot + "\\CommerceMinions_Sc9",
        solutionName: "Rob.Commerce",
        dbSuffix: "xp901",
        engineProjectPath: "./3. Project/Project.Commerce/Sitecore.Commerce.Engine",
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