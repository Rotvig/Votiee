module.exports.command = () ->
  return @
    .pause(100)
    .waitForElementPresent(".state-ready")
    