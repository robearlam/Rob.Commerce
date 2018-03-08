let { build, publish } = require('gulp-dotnet-cli');
let gulp = require('gulp');
let runSequence = require("run-sequence");
let fs = require('fs');
let del = require('del');
let jsonModify = require('gulp-json-modify');
let exec = require('child_process').exec;

var config;
if (fs.existsSync('./gulp-config.js.user')) {
    config = require("./gulp-config.js.user")();
}
else {
    config = require("./gulp-config.js")();
}
module.exports.config = config;

gulp.task("default", function (callback) {
    return runSequence(
        "01-Build-Commerce-Engine",
        "02-Publish-Commerce-Engine-To-Instances",
        callback);
});

gulp.task("01-Build-Commerce-Engine", function (callback) {
    var targets = ["Build"];
    if (config.runCleanBuilds) {
        targets = ["Clean", "Build"];
    }

    var solution = "./" + config.solutionName + ".sln";
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

gulp.task('02-Publish-Commerce-Engine-To-Instances', function(callback) {
    return runSequence(
       // "Stop-Local-IIS",
        "Delete-Existing-Engine-Files",
        "Publish-Commerce-Engine",
        "Copy-Published-Engine-To-All-Instances",
        "Transform-All-Engine-Env-Variables",
        //"Start-Local-IIS",
        callback
    );
});

gulp.task('Copy-Published-Engine-To-All-Instances', function() {
    return gulp.src(config.engineProjectPath + "/bin/publish/**/*")
        .pipe(gulp.dest(config.engineAuthoringRoot))
        .pipe(gulp.dest(config.engineShopsRoot))
        .pipe(gulp.dest(config.engineMinionsRoot))
        .pipe(gulp.dest(config.engineOpsRoot));
});

gulp.task('Transform-All-Engine-Env-Variables', function(cb) {
    TransformSingleEngineEnvVariables(config.engineAuthoringRoot + "\\wwwroot", "config.json", 'AppSettings.EnvironmentName', 'HabitatAuthoring');
    TransformSingleEngineEnvVariables(config.engineShopsRoot + "\\wwwroot", "config.json", 'AppSettings.EnvironmentName', 'HabitatShops');
    TransformSingleEngineEnvVariables(config.engineMinionsRoot + "\\wwwroot", "config.json", 'AppSettings.EnvironmentName', 'HabitatMinions');
    TransformSingleEngineEnvVariables(config.engineOpsRoot + "\\wwwroot", "config.json", 'AppSettings.EnvironmentName', 'AdventureWorksOpsApi');
    cb();
});

TransformSingleEngineEnvVariables = function(fileLocation, filename, jsonSelector, jsonValue) {
    return gulp.src(fileLocation + "\\" + filename)
        .pipe(jsonModify({ key: jsonSelector, value: jsonValue }))
        .pipe(gulp.dest(fileLocation));
};

gulp.task('Publish-Commerce-Engine', function() {
    return gulp.src(config.engineProjectPath + '/Sitecore.Commerce.Engine.csproj', { read: false })
        .pipe(publish({
            configuration: config.buildConfiguration,
            output: "./bin/publish",
            verbosity: config.buildVerbosity
        }));
});

gulp.task('Delete-Existing-Engine-Files', function() {
    return del(config.engineAuthoringRoot + "\\**\\*.*", { force: true },
            del(config.engineOpsRoot + "\\**\\*.*", { force: true },
            del(config.engineMinionsRoot + "\\**\\*.*", { force: true },
            del(config.engineShopsRoot + "\\**\\*.*", { force: true }))));
});

gulp.task('Stop-Local-IIS', function(callback) {
    exec('Powershell.exe  iisreset /timeout:0 /stop', function(err, stdout) {
        console.log(stdout);
        callback(err);
    });
});

gulp.task('Start-Local-IIS', function (callback) {
    exec('Powershell.exe  iisreset /timeout:0 /start', function (err, stdout) {
        console.log(stdout);
        callback(err);
    });
});