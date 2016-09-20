module.exports =
   tags: ['PresenterPage']

   'Can see answers when voting is open': (client) ->
        client
            .setup()
            #FOOTEST is a seeded surveySession
            .url("http://localhost:2799/#presenter-page/FOOOO")
            .waitForElementPresent(".answer-posi > span")
            .assert.containsText(".answer-posi > span", "test")
            
    'Can see answers when voting is closed but the is a currentSurveyItem': (client) ->
        client
            #CLOSEDTEST is a seeded surveySession
            .url("http://localhost:2799/#presenter-page/CLOSE")
            .waitForElementPresent(".answer-posi > span")
            .assert.containsText(".answer-posi > span", "test")
            
    'Can not see votes when not currentsurveyitem is active': (client) ->
        client
            .setup()
            #INACTIVETEST is a seeded surveySession
            .url("http://localhost:2799/#presenter-page/INACT")
            .waitForElementPresent(".waiting")
            
    'Can see result when they are shown': (client) ->
        client
            .setup()
            #SHOWRESULTSTEST is a seeded surveySession
            .url("http://localhost:2799/#presenter-page/RESUL")
            .waitForElementPresent(".piechart")
            .assert.elementPresent(".legends")
            .end()