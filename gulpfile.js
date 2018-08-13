let { build, publish } = require('gulp-dotnet-cli');
let gulp = require('gulp');
let fs = require('fs');
let del = require('del');
let jsonModify = require('gulp-json-modify');
let exec = require('child_process').exec;
let msbuild = require("gulp-msbuild");
let flatmap = require('gulp-flatmap');
let debug = require("gulp-debug");

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
    exec('Powershell.exe  iisreset /stop', function (err, stdout) {
        console.log(stdout);
        callback(err);
    });
});

gulp.task('Start-Local-IIS', function (callback) {
    exec('Powershell.exe  iisreset /start', function (err, stdout) {
        console.log(stdout);
        callback(err);
    });
});

gulp.task('Copy-Published-Engine-To-All-Instances', function () {
    return gulp.src(config.engineProjectPath + "/bin/publish/**/*")
        .pipe(gulp.dest(config.engineAuthoringRoot))
        .pipe(gulp.dest(config.engineShopsRoot))
        .pipe(gulp.dest(config.engineMinionsRoot))
        .pipe(gulp.dest(config.engineOpsRoot));
});

gulp.task('Transform-All-Engine-Env-Variables', function (callback) {
    //TransformSingleEngineEnvVariables(config.engineAuthoringRoot + "\\wwwroot", "config.json", 'AppSettings.EnvironmentName', 'HabitatAuthoring');
    //TransformSingleEngineEnvVariables(config.engineShopsRoot + "\\wwwroot", "config.json", 'AppSettings.EnvironmentName', 'HabitatShops');
    //TransformSingleEngineEnvVariables(config.engineMinionsRoot + "\\wwwroot", "config.json", 'AppSettings.EnvironmentName', 'HabitatMinions');
    //TransformSingleEngineEnvVariables(config.engineOpsRoot + "\\wwwroot", "config.json", 'AppSettings.EnvironmentName', 'AdventureWorksOpsApi');
    var transformscript = 'Powershell.exe ./scripts/TransformEngineParams.ps1' +
        ' -DatabaseServer ' + config.xcDatabaseServer +
        ' -Thumbprint ' + config.xcCertificateThumbprint +
        ' -EngineConnectIncludeDir ' + config.engineConnectIncludeDir +
        ' -EngineRootPaths ' + config.engineRootPaths;
    exec(transformscript, function (err, stdout) {
        console.log(stdout);
        callback(err);
    });
});

TransformSingleEngineEnvVariables = function (fileLocation, filename, jsonSelector, jsonValue) {
    return gulp.src(fileLocation + "\\" + filename)
        .pipe(jsonModify({ key: jsonSelector, value: jsonValue }))
        .pipe(gulp.dest(fileLocation));
};

gulp.task('Publish-Commerce-Engine', function () {
    return gulp.src(config.engineProjectPath + '/Sitecore.Commerce.Engine.csproj', { read: false })
        .pipe(publish({
            configuration: config.buildConfiguration,
            output: "./bin/publish",
            verbosity: config.buildVerbosity
        }));
});

gulp.task('Delete-Existing-Engine-Files', function () {
    return del(config.engineAuthoringRoot + "\\**\\*.*", { force: true },
        del(config.engineOpsRoot + "\\**\\*.*", { force: true },
            del(config.engineMinionsRoot + "\\**\\*.*", { force: true },
                del(config.engineShopsRoot + "\\**\\*.*", { force: true }))));
});



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
        "Delete-Existing-Engine-Files",
        "Copy-Published-Engine-To-All-Instances",
        "Transform-All-Engine-Env-Variables",
        "Start-Local-IIS", function(done) {
            done();
}));

gulp.task('03-Publish-Website-Projects',
    gulp.series(
        "Publish-Foundation-Projects",
        "Publish-Feature-Projects",
        "Publish-Project-Projects", function (done) {
            done();
        }));

gulp.task("default",
    gulp.series(
        "01-Build-Commerce-Engine",
        "02-Publish-Commerce-Engine-To-Instances",
        "03-Publish-Website-Projects", function (done) {
            done();
}));


var publishProjects = function (location) {
    console.log("publish to " + config.sitecoreRoot + " folder");
    return gulp.src([location + "/**/Website/**/*.csproj"])
        .pipe(flatmap(function (stream, file) {
            return publishStream(stream);
        }));
};

var publishStream = function (stream) {
    var targets = ["Build"];

    return stream
        .pipe(debug({ title: "Building project:" }))
        .pipe(msbuild({
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
                Platform: config.publishPlatform,
                DeployOnBuild: "true",
                DeployDefaultTarget: "WebPublish",
                WebPublishMethod: "FileSystem",
                DeleteExistingFiles: "false",
                publishUrl: config.sitecoreRoot,
                _FindDependencies: "false"
            }
        }));
}