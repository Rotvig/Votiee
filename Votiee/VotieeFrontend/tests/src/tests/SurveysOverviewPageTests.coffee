module.exports =
   tags: ['SurveysOverviewPage']

   'Open one of the users Surveys from SurveysOverviewPage': (client) ->
        client
            .setup()
            .click(".account-button")
            .waitForElementPresent(".login-page")
            .setValue(".email-input", client.globals.testMail)
            .setValue(".password-input", client.globals.testPassword)
            .click(".login-button")
            .waitForStateReady()
            .click(".load-button")
            .waitForElementPresent(".survey-view:nth-child(2) .edit-button")
            .click(".survey-view:nth-child(2) .edit-button")
            .waitForStateReady()
            .assert.containsText(".survey-name-text", "MyTestSurvey")
            
    'Start a survey session from a Survey': (client) ->
        client
            .back()
            .waitForElementPresent(".surveys-overview-page")
            .click(".survey-view:nth-child(3) .start-button")
            .waitForStateReady()
            .assert.elementPresent(".survey-session-page")

    'Create new Survey from SurveysOverviewPage': (client) ->
        client
            .back()
            .waitForElementPresent(".surveys-overview-page")
            .click(".create-button")
            .waitForElementPresent(".survey-page")
            
    'Delete a Survey': (client) ->
        client
            .back()
            .waitForElementPresent(".survey-items-view > div:nth-child(1) > .button-group > button:nth-child(3)")
            .click(".survey-items-view > div:nth-child(1) > .button-group > button:nth-child(3)")
            .waitForElementPresent(".btn-danger")
            .click(".btn-danger")
            .waitForElementPresent(".survey-items-view > div:nth-child(1) > .button-group > button:nth-child(3)")
            .click(".survey-items-view > div:nth-child(1) > .button-group > button:nth-child(3)")
            .waitForElementPresent("div.survey-items-view > div:nth-child(1) .btn.btn-default")
            .click("div.survey-items-view > div:nth-child(1) .btn.btn-default")
            .waitForStateReady()
            .assert.containsText("div.survey-items-view > div:nth-child(1) > div.survey-label", "MyTestSurvey")
            .assert.containsText("div.survey-items-view > div:nth-child(2) > div.survey-label", "Another Test Survey")
            .end()
            
          