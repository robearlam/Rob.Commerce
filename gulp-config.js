module.exports = function () {
    var webroot = "C:\\inetpub\\wwwroot";
    var siteName = "xc902";
    var sitecoreRoot = webroot + "\\" + siteName + ".sc";
    var config = {
        sitecoreRoot: sitecoreRoot,
        engineProjectPath: "./src/Project/Engine/Sitecore.Commerce.Engine",
        buildConfiguration: "Debug",
        buildToolsVersion: 15.0,
        buildMaxCpuCount: 0,
        buildVerbosity: "minimal",
        buildPlatform: "Any CPU",
        publishPlatform: "AnyCpu",
        runCleanBuilds: true,
        xcDatabaseServer: ".",
        xcCertificateThumbprint: "58DC94A32702431CDD5F3B7FEE49FF8FA2BF8CEA",
        engineConnectIncludeDir: sitecoreRoot + '\\App_Config\\Include\\Y.Commerce.Engine',
        engineRoles: [
            {
                path: webroot + "\\" + siteName + ".CommerceAuthoring",
                environmentName: 'HabitatAuthoringBOBOB'
            },
            {
                path: webroot + "\\" + siteName + ".CommerceShops",
                environmentName: 'HabitatShops'
            },
            {
                path: webroot + "\\" + siteName + ".CommerceOps",
                environmentName: 'AdventureWorksOpsApi'
            },
            {
                path: webroot + "\\" + siteName + ".CommerceMinions",
                environmentName: 'HabitatMinions'
            },
        ]
    };
    return config;
}