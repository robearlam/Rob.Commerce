module.exports = function () {
    var webroot = "C:\\inetpub\\wwwroot";
    var siteName = "xc901";
    var config = {
        sitecoreRoot: webroot + "\\" + siteName + ".sc",
        engineAuthoringRoot: webroot + "\\" + siteName + ".CommerceAuthoring",
        engineShopsRoot: webroot + "\\" + siteName + ".CommerceShops",
        engineOpsRoot: webroot + "\\" + siteName + ".CommerceOps",
        engineMinionsRoot: webroot + "\\" + siteName + ".CommerceMinions",
        solutionName: "Rob.Commerce",
        dbSuffix: siteName,
        engineProjectPath: "./src/Project/Engine/Sitecore.Commerce.Engine",
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