module.exports.command = (element = ".front-page") ->
  return this
    .session('DELETE')
    .deleteCookies()
    .url('http://localhost:2799')
    .waitForElementPresent('body')
    .waitForElementPresent(element)
    