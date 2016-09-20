module.exports =
   tags: ['FrontPage']

   'Can go to VotingPage': (client) ->
        client
            .setup()
            #FOOTEST is a seeded surveySession
            .setValue(".participate-button .input-button-input", "FOOOO")
            .click(".participate-button .input-button-load")
            .waitForStateReady()
            .waitForElementPresent(".voting-page")
            .assert.urlContains("voting-page/FOOOO")
            
   'Can handle non existing SurveySession': (client) ->
        client
            .back()
            .assert.elementNotPresent(".survey-not-found-text")
            .setValue(".participate-button .input-button-input", "SOMERANDOMSTUFFNOTEXISTING")
            .click(".participate-button .input-button-load")
            .waitForStateReady()
            .assert.elementPresent(".session-not-found-text")
            
   'Can go to Main Menu': (client) ->
        client
            .click(".editor-button")
            .assert.elementPresent(".menu-page")
            .end()