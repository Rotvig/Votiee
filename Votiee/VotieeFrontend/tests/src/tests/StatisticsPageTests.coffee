module.exports =
   tags: ['StatisticsPage']

   'Load statistics for archived survey': (client) ->
        client
            .setup()
            .click(".account-button")
            .waitForElementPresent(".login-page")
            .setValue(".email-input", client.globals.testMail)
            .setValue(".password-input", client.globals.testPassword)
            .click(".login-button")
            .waitForStateReady()
            .click(".statistics-button")
            .waitForStateReady()        
            .click(".statistic-overview-item-view:nth-child(1) .toggle-button")
            .waitForStateReady()
            .click(".statistic-overview-item-view:nth-child(1) .show-stats-button")
            .waitForElementPresent(".survey-name")
            .assert.containsText(".survey-name", "MyFirstTestSurvey")
            .assert.containsText(".survey-template-name", "MyTestSurvey")
            .assert.containsText(".survey-item-view:nth-child(1) .question-text", "What is first?")
            .assert.containsText(".survey-item-view:nth-child(1) .answer-text", "This question is!")
            
    'Reactivate Survey Session from statistics': (client) ->
        client
            .back()        
            .waitForElementPresent(".statistic-overview-item-view:nth-child(1) .toggle-button")  
            .click(".statistic-overview-item-view:nth-child(1) .toggle-button")
            .waitForStateReady()
            .click(".statistic-overview-item-view:nth-child(1) .show-stats-button")
            .waitForElementPresent(".statistics-page")
            .click(".reactivate-button")
            .waitForElementPresent("h4.survey-name > span:nth-child(1)")
            .assert.containsText("h4.survey-name > span:nth-child(1)", "MyFirstTestSurvey")

    'Delete a Survey': (client) ->
        client
            .url("http://localhost:2799/#statistics-overview-page/")        
            .waitForElementPresent(".statistic-overview-item-view:nth-child(1) .toggle-button")  
            .click(".statistic-overview-item-view:nth-child(1) .toggle-button")
            .waitForStateReady()
            .waitForElementPresent(".glyphicon-trash")
            .click(".glyphicon-trash")
            .waitForElementPresent(".cancel")
            .click(".cancel")
            .waitForElementPresent(".glyphicon-trash")
            .click(".glyphicon-trash")
            .waitForElementPresent(".delete")
            .click(".delete")
            .waitForElementNotPresent(".statistic-overview-view")
            .end()
  
            
