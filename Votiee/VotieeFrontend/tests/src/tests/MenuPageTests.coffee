module.exports =
   tags: ['MenuPage']

   'Create survey (anonymous)': (client) ->
        client
            .setup()
            .click(".editor-button")
            .click(".create-button")
            .waitForStateReady()
            .assert.elementPresent(".survey-page")
            
    'Load survey (anonymous)': (client) ->
        client
            .back()
            .click(".load-survey-button .input-button-toggle")
            .setValue(".load-survey-button .input-button-input", "TESTSURV")
            .click(".load-survey-button .input-button-load")
            .waitForStateReady()
            .assert.elementPresent(".survey-page")
            
    'Load non existing survey (anonymous)': (client) ->
        client
            .back()
            .assert.elementNotPresent(".error-message")
            .click(".load-survey-button .input-button-toggle")
            .setValue(".load-survey-button .input-button-input", "NonExisting")
            .click(".load-survey-button .input-button-load")
            .waitForStateReady()
            .assert.elementPresent(".error-message")
            
    'Use as Presenter (anonymous)': (client) ->
        client
            .click(".presenter-button .input-button-toggle")
            #JOHNSON is a seeded session
            .setValue(".presenter-button .input-button-input", "JOHNS")
            .click(".presenter-button .input-button-load")
            .waitForStateReady()
            .assert.elementPresent(".presenter-page")
            
    'Use as Presenter with non existing session (anonymous)': (client) ->
        client
            .back()
            .assert.elementNotPresent(".error-message")
            .click(".presenter-button .input-button-toggle")
            .setValue(".presenter-button .input-button-input", "NonExisting")
            .click(".presenter-button .input-button-load")
            .waitForStateReady()
            .assert.elementPresent(".error-message")
            
    'Participate button (anonymous)': (client) ->
        client
            .click(".participate-button .input-button-toggle")
            #JOHNSON is a seeded session
            .setValue(".participate-button .input-button-input", "JOHNS")
            .click(".participate-button .input-button-load")
            .waitForStateReady()
            .assert.elementPresent(".voting-page")
            
    'Participate with non existing session (anonymous)': (client) ->
        client
            .back()
            .assert.elementNotPresent(".error-message")
            .click(".participate-button .input-button-toggle")
            .setValue(".participate-button .input-button-input", "NonExisting")
            .click(".participate-button .input-button-load")
            .waitForStateReady()
            .assert.elementPresent(".error-message")
            
    'Login button (anonymous)': (client) ->
        client
            .click(".login-button")
            .waitForStateReady()
            .assert.elementPresent(".login-page")
            .setValue(".email-input", client.globals.testMail)
            .setValue(".password-input", client.globals.testPassword)
            .click(".login-button")
            .waitForElementPresent(".menu-page")
            
    'Create survey (registered)': (client) ->
        client
            .click(".create-button")
            .waitForStateReady()
            .assert.elementPresent(".survey-page")

    'Load survey (registered)': (client) ->
        client
            .back()
            .click(".load-button")
            .waitForStateReady()
            .assert.elementPresent(".surveys-overview-page")

    'Statistics (registered)': (client) ->
        client
            .back()
            .click(".statistics-button")
            .waitForStateReady()
            .assert.elementPresent(".statistics-overview-page")

    'Use as Presenter (registered)': (client) ->
        client
            .back()
            .click(".presenter-button .input-button-toggle")
            #JOHNSON is a seeded session
            .setValue(".presenter-button .input-button-input", "JOHNS")
            .click(".presenter-button .input-button-load")
            .waitForStateReady()
            .assert.elementPresent(".presenter-page")
            
    'Use as Presenter with non existing session (registered)': (client) ->
        client
            .back()
            .assert.elementNotPresent(".error-message")
            .click(".presenter-button .input-button-toggle")
            .setValue(".presenter-button .input-button-input", "NonExisting")
            .click(".presenter-button .input-button-load")
            .waitForStateReady()
            .assert.elementPresent(".error-message")
            
    'Participate button (registered)': (client) ->
        client
            .click(".participate-button .input-button-toggle")
            #JOHNSON is a seeded session
            .setValue(".participate-button .input-button-input", "JOHNS")
            .click(".participate-button .input-button-load")
            .waitForStateReady()
            .assert.elementPresent(".voting-page")
            
    'Participate with non existing session (registered)': (client) ->
        client
            .back()
            .assert.elementNotPresent(".error-message")
            .click(".participate-button .input-button-toggle")
            .setValue(".participate-button .input-button-input", "NonExisting")
            .click(".participate-button .input-button-load")
            .waitForStateReady()
            .assert.elementPresent(".error-message")

    'Log out (registered)': (client) ->
        client
            .click(".logout-button")
            .waitForStateReady()
            .assert.elementPresent(".front-page")
            .end()