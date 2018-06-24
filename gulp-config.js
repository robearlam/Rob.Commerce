module.exports = function () {
    var webroot = "C:\\inetpub\\wwwroot";
    var config = {
        sitecoreRoot: webroot + "\\xc901.sc",
        engineAuthoringRoot: webroot + "\\xc901.CommerceAuthoring",
        engineShopsRoot: webroot + "\\xc901.CommerceShops",
        engineOpsRoot: webroot + "\\xc901.CommerceOps",
        engineMinionsRoot: webroot + "\\xc901.CommerceMinions",
        solutionName: "Rob.Commerce",
        dbSuffix: "xc901",
        engineProjectPath: "./src/3. Project/Engine/Sitecore.Commerce.Engine",
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