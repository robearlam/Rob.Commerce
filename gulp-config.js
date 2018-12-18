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
        xcCertificateThumbprint: "DD31CE8188C9EB6F9124FD9256F7E2A02A718848",
        engineConnectIncludeDir: sitecoreRoot + '\\App_Config\\Include\\Y.Commerce.Engine',
        engineHostname: "localhost",
        enginePort: "5000",
        identityServerHostname: "localhost",
        identityServerPort: "5050",
        adminUser: "admin",
        adminPassword: "b",
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
        ]
    };
    return config;
}