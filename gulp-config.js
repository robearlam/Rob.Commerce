module.exports = function () {
    var webroot = "C:\\inetpub\\wwwroot";
    var siteName = "xc91";
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
        xcCertificateThumbprint: "4D24F1FE8DE82073BD8CF1DE7FD5B04DFD6AEEBA",
        engineConnectIncludeDir: sitecoreRoot + '\\App_Config\\Include\\Y.Commerce.Engine',
        engineHostname: "commerceauthoring." + siteName + ".local",
        identityServerHostname: siteName + ".identityserver",
        adminUser: "admin",
        adminPassword: "b",
        engineRoles: [
            {
                path: webroot + "\\CommerceAuthoring_" + siteName,
                environmentName: 'HabitatAuthoring'
            },
            {
                path: webroot + "\\CommerceShops_" + siteName,
                environmentName: 'HabitatShops'
            },
            {
                path: webroot + "\\CommerceOps_" + siteName,
                environmentName: 'HabitatAuthoring'
            },
            {
                path: webroot + "\\CommerceMinions_" + siteName,
                environmentName: 'HabitatMinions'
            },
        ]
    };
    return config;
}