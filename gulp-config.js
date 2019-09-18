module.exports = function () {
    var webroot = "C:\\inetpub\\wwwroot";
    var siteName = "xc92";
    var sitecoreRoot = webroot + "\\" + siteName + ".sc";
    var config = {
        sitecoreRoot: sitecoreRoot,
        engineProjectPath: "./src/Project/Sitecore.Commerce.Engine",
        buildConfiguration: "Release",
        buildToolsVersion: 15.0,
        buildMaxCpuCount: 0,
        buildVerbosity: "minimal",
        buildPlatform: "Any CPU",
        publishPlatform: "AnyCpu",
        runCleanBuilds: true,
        xcDatabaseServer: ".",
        xcCertificateThumbprint: "27A86A14AC2C98D8E531DA2D0C68B31A4DD3D2B1",
        engineConnectIncludeDir: sitecoreRoot + '\\App_Config\\Include\\Y.Commerce.Engine',
        engineHostname: "commerceauthoring." + siteName + ".com",
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
        ],
        publishProfile: "Local_Publish"
    };
    return config;
}