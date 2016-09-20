module.exports =
   tags: ['SurveySessionPage']

   'SurveySession renders with the correct data': (client) ->
        client
            .setup()          
            .url("http://localhost:2799/#survey-session-page/JOHNS") #Navigating directly to the SurveySessionPage
            .waitForElementPresent(".survey-session-page")
            .waitForElementPresent("div.survey-items-view > div:nth-child(1) > div:nth-child(1)")
            .assert.containsText("div.survey-items-view > div:nth-child(1) > div:nth-child(1) > p", "NIKO LI FRÆS")
            
   "Can start question": (client) ->
       client
            .click("div.survey-items-view > div:nth-child(1) .active-button")
            .waitForStateReady()
            .assert.containsText("div.survey-items-view > div:nth-child(1) span.label", "Active")     
            
   "Can start and stop question": (client) ->
       client
            .click(".active-button")
            .waitForElementNotPresent(".collapse.in")
            .assert.containsText("div.survey-items-view > div:nth-child(1) span.label", "Not Started")
            .click(".active-button")
            .waitForElementPresent(".collapse.in")
            .assert.containsText("div.survey-items-view > div:nth-child(1) span.label", "Active")
   
   "Can Pause and unpause question": (client) ->
       client
            .click(".pause-button")
            .waitForStateReady()
            .assert.containsText("div.survey-items-view > div:nth-child(1) span.label", "Paused")
            .click(".pause-button")
            .waitForStateReady()
            .assert.containsText("div.survey-items-view > div:nth-child(1) span.label", "Active")
            
    "Can show and hide results": (client) ->
       client
            .click(".show-results-button")
            .waitForStateReady()
            .assert.elementPresent(".label-shows-results")
            .click(".show-results-button")
            .waitForStateReady()
            .assert.elementNotPresent(".label-shows-results")
            
    "Can Stop and Start survey": (client) ->
      client
            .click("div.survey-info-box > button")
            .waitForStateReady()
            .assert.containsText("div.survey-info-box > button", "Start Session")
            .click("div.survey-info-box > button")
            .waitForStateReady()
            .assert.containsText("div.survey-info-box > button", "Stop Session")
            .end()
            
