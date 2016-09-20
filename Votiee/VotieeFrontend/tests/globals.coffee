HtmlReporter = require('nightwatch-html-reporter')
reporter = new HtmlReporter
  openBrowser: false,
  reportsDirectory: './reports',
  reportFilename: 'generatedReport.html',
  themeName: 'default',
  hideSuccess: false

module.exports =
    #Settings
    waitForConditionTimeout: 120000
    #HTML reporter
    reporter: reporter.fn
    
    loginTestMail: "login@testmail.com"
    testMail: "test@testmail.com"
    testPassword: "Testkode12_"

    before: (cb) ->
      console.log('GLOBAL BEFORE')
      cb()

    beforeEach: (cb) ->
      console.log('GLOBAL BEFORE EACH')
      cb()

    after: (cb) ->
      console.log('GLOBAL AFTER')
      cb()

    afterEach: (cb) ->
      console.log('GLOBAL AFTER EACH')
      cb()
