let { build, publish } = require('gulp-dotnet-cli');
let gulp = require('gulp');
let fs = require('fs');
let del = require('del');
let exec = require('child_process').exec;
var _msbuild = require('msbuild');
let flatmap = require('gulp-flatmap');
let rimraf = require('rimraf');
let path = require("path");

var config;
if (fs.existsSync('./gulp-config.js.user')) {
    config = require("./gulp-config.js.user")();
}
else {
    config = require("./gulp-config.js")();
}
module.exports.config = config;

gulp.task("Publish-Foundation-Projects", function () {
    return publishProjects("./src/Foundation");
});

gulp.task("Publish-Feature-Projects", function () {
    return publishProjects("./src/Feature");
}); 

gulp.task("Publish-Project-Projects", function () {
    return publishProjects("./src/Project");
});

gulp.task('Stop-Local-IIS', function (callback) {
    exec('Powershell.exe iisreset /stop', function (err, stdout) {
        console.log(stdout);
        callback(err);
    });
});

gulp.task('Start-Local-IIS', function (callback) {
    exec('Powershell.exe iisreset /start', function (err, stdout) {
        console.log(stdout);
        callback(err);
    });
});

gulp.task('Copy-Published-Engine-To-All-Instances', function (callback) {
    return config.engineRoles.forEach(function (engineRole) {
        var copyScript = 'Powershell.exe ./scripts/CopyEngine.ps1' +
            ' -RolePath \'' + engineRole.path + '\'';

        console.log(copyScript);
        exec(copyScript, function (err, stdout) {
            console.log(stdout);
        });
     }, callback());
});

gulp.task('Transform-Website', function (callback) {
    var transformscript = 'Powershell.exe ./scripts/TransformWebsite.ps1' +
        ' -Thumbprint \'' + config.xcCertificateThumbprint + "\'" +
        ' -EngineConnectIncludeDir \'' + config.engineConnectIncludeDir + '\'';

    exec(transformscript, function (err, stdout) {
        console.log(stdout);
        callback(err);
    });
});

gulp.task('Transform-All-Engine-Roles', function (callback) {
    return config.engineRoles.forEach(function (engineRole) {
        var transformscript = 'Powershell.exe ./scripts/TransformEngineRole.ps1' +
            ' -Thumbprint \'' + config.xcCertificateThumbprint + "\'" +
            ' -EnvironmentName \'' + engineRole.environmentName + "\'" +
            ' -RolePath \'' + engineRole.path + '\'';

        exec(transformscript, function (err, stdout) {
            console.log(stdout);
        });
    }, callback());
});


gulp.task('Publish-Commerce-Engine', function () {
    return gulp.src(config.engineProjectPath + '/Sitecore.Commerce.Engine.csproj', { read: false })
        .pipe(publish({
            configuration: config.buildConfiguration,
            output: "./bin/publish",
            verbosity: config.buildVerbosity
        }));
});

//gulp.task('Delete-Existing-Engine-Files', function (callback) {
//    return config.engineRoles.forEach(function (engineRole) {
//        del(engineRole.path + "\\**\\*.*", { force: true });
//    }, callback());
//});

gulp.task("01-Build-Commerce-Engine", function (callback) {
    var targets = ["Build"];
    if (config.runCleanBuilds) {
        targets = ["Clean", "Build"];
    }

    var solution = "./" + config.engineProjectPath + '/Sitecore.Commerce.Engine.csproj';
    return gulp.src(solution)
        .pipe(build({
            targets: targets,
            configuration: config.buildConfiguration,
            logCommand: false,
            verbosity: config.buildVerbosity,
            stdout: true,
            errorOnFail: true,
            maxcpucount: config.buildMaxCpuCount,
            nodeReuse: false,
            toolsVersion: config.buildToolsVersion,
            properties: {
                Platform: config.buildPlatform
            }
        }));
});

gulp.task('02-Publish-Commerce-Engine-To-Instances', 
    gulp.series(
        "Stop-Local-IIS",
        "Publish-Commerce-Engine",
        //"Delete-Existing-Engine-Files",
        "Copy-Published-Engine-To-All-Instances",
        "Start-Local-IIS",
        "Transform-All-Engine-Roles", function(done) {
            done();
}));

gulp.task('03-Publish-Website-Projects',
    gulp.series(
        "Publish-Foundation-Projects",
        "Publish-Feature-Projects",
        "Publish-Project-Projects",
        "Transform-Website", function (done) {
            done();
}));

gulp.task("default",
    gulp.series(
        "01-Build-Commerce-Engine",
        "02-Publish-Commerce-Engine-To-Instances",
        //"03-Publish-Website-Projects", 
        function (done) {
            done();
}));

gulp.task("initial-engine-setup", function (callback) {
    var script = 'Powershell.exe ./scripts/FirstDeploy.ps1' +
        ' -EngineHostName \'' + config.engineHostname + "\'" +
        ' -IdentityServerHostname \'' + config.identityServerHostname + "\'" +
        ' -AdminPassword \'' + config.adminPassword + "\'" +
        ' -AdminUser \'' + config.adminUser + '\'';

    console.log(script);
    exec(script, function (err, stdout) {
        if(err)
            console.log(err);

        console.log(stdout);
        callback();
    });
});

gulp.task("first-install",
    gulp.series(
        "default",
        "initial-engine-setup", function (done) {
            done();
}));

var publishProjects = function (location) {
    console.log("publish to " + config.sitecoreRoot + " folder");

    return gulp.src([location + "/**/*Website.csproj"])
               .pipe(flatmap(function (stream, file) {
                   return publishStream(stream, file);
               }));
};

var publishStream = function (stream, file) {
    console.log("publishing: " + file.path);  
    console.log("Using publishing profile: " + config.publishProfile);
    
    var msbuild = new _msbuild(); 
    msbuild.sourcePath = file.path;
    msbuild.configuration = config.buildConfiguration;
    msbuild.publishProfile = config.publishProfile;

    var overrideParams = [];
    overrideParams.push('/p:VisualStudioVersion=' + config.buildToolsVersion.toFixed(1));  
    msbuild.config('overrideParams',overrideParams);
    msbuild.publish(); 

    return stream;
};

/************************/
/* CI Tasks below here  */
/************************/

gulp.task("CI-Clean", function (callback) {
    console.log("Cleaning output folder for CI Build & Publish");
    rimraf.sync(path.resolve("./Output"), callback());
});

gulp.task("CI-Update-Publish-Params", function (callback) {
    console.log("Publish Web Projects to './Output' folder");
    config.sitecoreRoot = path.resolve("./Output");
    config.publishProfile = "CI";
    config.buildConfiguration = "Release";
    fs.mkdirSync(config.sitecoreRoot);
    callback();
});

gulp.task("CI-Run",
    gulp.series(
        "CI-Clean",
        "CI-Update-Publish-Params",
        "Publish-Foundation-Projects",
        "Publish-Feature-Projects",
        "Publish-Project-Projects", function (done) {
            done();
}));