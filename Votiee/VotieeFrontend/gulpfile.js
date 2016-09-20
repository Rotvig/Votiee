/// <vs AfterBuild='default' />
var gulp = require('gulp');
var stylus = require('gulp-stylus');
var watch = require('gulp-watch');

gulp.task('stylus', function () {
    gulp.src('./src/stylus/*.styl')
      .pipe(stylus({
          'include css': true
      }))
      .pipe(gulp.dest('build'));
});

gulp.task('watch', function() {
    gulp.watch('./src/stylus/**/*.styl', ['stylus']);
});

gulp.task('default', ['stylus', 'watch']);