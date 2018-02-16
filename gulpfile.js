let { clean, restore, build, test, pack, publish, run } = require('gulp-dotnet-cli');
let gulp = require('gulp');
let debug = require("gulp-debug");
let runSequence = require("run-sequence");
let fs = require('fs');

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
        callback);
});

gulp.task("01-Build-Commerce-Engine", function (callback) {
    return runSequence(
        "Build-Solution", callback);
});

gulp.task("Copy-SC-Assemblies-To-Lib", function () {
    console.log("Copying Sitecore Assemblies");
    return gulp.src(config.sitecoreRoot + "\\bin").pipe(gulp.dest("./lib"));
});

gulp.task("Build-Solution", function () {
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

gulp.task("Publish-Commerce-Engine", function() {
    publishStream("./3. Project/Project.Commerce/Sitecore.Commerce.Engine/Sitecore.Commerce.Engine.csproj",
        "C:\\inetpub\\wwwroot\\_CommercePublish");
});

//gulp.task('publish', ['test'], () => {
//    return gulp.src('src/TestWebProject.csproj', { read: false })
//        .pipe(publish({ configuration: configuration, version: version }));
//});


var publishStream = function (stream, dest) {
    var targets = ["Build"];
    return gulp.src(stream, { read: false })
        .pipe(publish({
            configuration: config.buildConfiguration,
            ouptut: dest,
            verbosity: config.buildVerbosity,
            targets: targets,
            logCommand: false,
            stdout: true,
            errorOnFail: true,
            maxcpucount: config.buildMaxCpuCount,
            nodeReuse: false,
            toolsVersion: config.buildToolsVersion,
            properties: {
                Platform: config.buildPlatform
            }
        }));
}