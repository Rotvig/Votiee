module.exports =
   tags: ['VotingPage']

   'Can Vote': (client) ->
        client
            .setup()
            #FOOTEST is a seeded surveySession
            .setValue(".participate-button .input-button-input", "FOOOO")
            .click(".participate-button .input-button-load")
            .waitForStateReady()
            .waitForElementPresent(".question-text")
            .assert.containsText(".question-text", "test")
            .assert.containsText(".answer-button", "test")
            .click(".answer-button")
            .waitForStateReady()
            .assert.elementPresent(".selected-view")

    'Can not Vote again': (client) ->
        client
            .refresh()
            .pause(100)
            .assert.elementPresent(".voting-done-text")
            .end()
            
    'Can not vote when voting is not open in session': (client) ->
        client
            .setup()
            #CLOSEDTEST is a seeded surveySession
            .setValue(".participate-button .input-button-input", "CLOSE")
            .click(".participate-button .input-button-load")
            .waitForStateReady()
            .assert.containsText(".question-text", "test")
            .waitForStateReady()
            .assert.elementPresent(".voting-closed-view")
            .click(".answer-button")
            .waitForStateReady()
            .assert.elementNotPresent(".selected-view")
            .end()
            
    'Can not vote when no SurveyItem is active': (client) ->
        client
            .setup()
            #INACTIVETEST is a seeded surveySession
            .setValue(".participate-button .input-button-input", "INACT")
            .click(".participate-button .input-button-load")
            .waitForStateReady()
            .assert.elementPresent(".item-not-active")
            .assert.elementNotPresent(".answer-button")
            .end()
            
    'Results are shown': (client) ->
        client
            .setup()
            #SHOWRESULTSTEST is a seeded surveySession
            .setValue(".participate-button .input-button-input", "RESUL")
            .click(".participate-button .input-button-load")
            .waitForStateReady()    
            .assert.elementPresent(".show-results-view")
            .assert.elementNotPresent(".answer-button")
            .end()