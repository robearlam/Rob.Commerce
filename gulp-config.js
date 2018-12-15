module.exports = function () {
    var webroot = "C:\\inetpub\\wwwroot";
    var siteName = "xc903";
    var sitecoreRoot = webroot + "\\" + siteName + ".sc";
    var config = {
        sitecoreRoot: sitecoreRoot,
        engineProjectPath: "./src/Project/Sitecore.Commerce.Engine",
        buildConfiguration: "Debug",
        buildToolsVersion: 15.0,
        buildMaxCpuCount: 0,
        buildVerbosity: "minimal",
        buildPlatform: "Any CPU",
        publishPlatform: "AnyCpu",
        runCleanBuilds: true,
        xcDatabaseServer: ".",
        xcCertificateThumbprint: "DDFD90315A364DBFF278AE24CB62B7A65D22865A",
        engineConnectIncludeDir: sitecoreRoot + '\\App_Config\\Include\\Y.Commerce.Engine',
        engineRoles: [
            {
                path: webroot + "\\" + siteName + ".CommerceAuthoring",
                environmentName: 'HabitatAuthoring'
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
        ],
        defaultEnvironments: [
            "AdventureWorksAuthoring",
            "AdventureWorksMinions",
            "AdventureWorksShops",
            "HabitatAuthoring",
            "HabitatMinions",
            "HabitatShops"
        ]
    };
    return config;
}