let { clean, restore, build, test, pack, publish, run } = require('gulp-dotnet-cli');
let gulp = require('gulp');
let runSequence = require("run-sequence");
let fs = require('fs');
let gulpCopy = require('gulp-copy');
let del = require('del');

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
        "Delete-Existing-Engine-Files",
        "Publish-Commerce-Engine",
        "Copy-Published-Engine-To-Instances",
        "Transform-Published-Instances",
        callback
    );
});

gulp.task('Copy-Published-Engine-To-Instances', function() {
    return gulp.src("./bin/publish/**/*")
        .pipe(gulpCopy(config.engineAuthoringRoot, options));
    //.pipe(gulpCopy(config.engineShopsRoot, options))
    //.pipe(gulpCopy(config.engineMinionsRoot, options))
    //.pipe(gulpCopy(config.engineOpsRoot, options));
});

gulp.task('Transform-Published-Instances', function() {
    return null;
});

gulp.task('Publish-Commerce-Engine', function() {
    return gulp.src('./3. Project/Project.Commerce/Sitecore.Commerce.Engine/Sitecore.Commerce.Engine.csproj', { read: false })
        .pipe(publish({
            configuration: config.buildConfiguration,
            output: "./bin/publish",
            verbosity: config.buildVerbosity
        }));
});

gulp.task('Delete-Existing-Engine-Files', function() {
    return gulp.pipe(del(config.engineAuthoringRoot + "\\**\\*.*", { force: true }));
});