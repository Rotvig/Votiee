firstAnswer = ".answer-input-view:nth-child(2) .answer-input"
firstQuestionInput = "div.survey-items-view > div:nth-child(1) .survey-item-background.collapse.in > div.survey-item-input > input"
SecondQuestionInput = "div.survey-items-view > div:nth-child(2) .survey-item-background.collapse.in > div.survey-item-input > input"
module.exports =
   tags: ['SurveyPage']

   'Create Survey': (client) ->
        client
            .setup()
            .waitForElementPresent(".editor-button")
            .click(".editor-button")
            .waitForElementPresent(".menu-page")
            .click(".create-button")
            .waitForElementPresent(".three-quarters-loader")
            .waitForElementNotPresent(".three-quarters-loader")
            
    'Can not Start empty Survey Session': (client) -> 
        client
            .assert.elementNotPresent("error-message")
            .click(".start-button")
            .waitForStateReady()
            .assert.elementPresent(".error-message")
               
    'Change Survey Name': (client) ->
        client
          .click(".survey-name-change-button")
          .clearValue(".survey-name-input")
          .setValue(".survey-name-input", "a name")
          .setValue(".survey-name-input", client.Keys.ENTER)
          .pause(100)
          .waitForElementPresent(".state-ready")
          .refresh()
          .waitForElementPresent(".survey-name-text")
          .assert.containsText(".survey-name-text-field .name", "a name")
          
    'Write Question': (client) ->
        client
          .setValue(".question-input", "a question")
          .setValue(".question-input", client.Keys.ENTER)
          .pause(100)
          .waitForElementPresent(".state-ready")
          .refresh()
          .waitForElementPresent(".question-input")
          .assert.valueContains(".question-input", "a question")
          
    'Write Answer': (client) ->
        client
          .setValue(firstAnswer, "an answer")
          .setValue(firstAnswer, client.Keys.ENTER)
          .pause(100)
          .waitForElementPresent(".state-ready")
          .refresh()
          .waitForElementPresent(firstAnswer)
          .assert.valueContains(firstAnswer, "an answer")
            
    'Create New Answer': (client) -> 
        client
            .click(".survey-item .add-button")
            .pause(100)
            .waitForElementPresent(".state-ready")
            .assert.containsText(".survey-item div.answer-input-view:nth-child(4) > label","Answer 3")  
 
    'Create New Question': (client) -> 
        client
            .click("div.survey-page-menu > .add-button")
            .pause(100)
            .waitForElementPresent(".state-ready")
            .assert.containsText(".survey-items-view > div:nth-child(2) label","Question 2")  
            
    'Delete Answer': (client) -> 
        client
            .assert.elementPresent(".answer-input-view:nth-child(4) .answer-label")
            .setValue("div:nth-child(1) > div > div.survey-item-background.collapse.in > div:nth-child(3) input", "Delete Me")
            .setValue(firstAnswer, client.Keys.ENTER)
            .click(".survey-items-view > div:nth-child(1) .survey-item-background.collapse.in > div:nth-child(3) button")
            .waitForElementPresent(".delete-button")
            .click(".delete-button")
            .waitForStateReady()
            .assert.elementNotPresent(".answer-input-view:nth-child(4) .answer-label") 
            .assert.value("div:nth-child(1) > div > div.survey-item-background.collapse.in > div:nth-child(3) input", "")
            
    'Delete Question': (client) -> 
        client
            .assert.elementPresent(".survey-item-view:nth-child(2) .question-label")
            .click(".survey-items-view > div:nth-child(2) div.expand-toolbar button")  
            .waitForElementPresent(".delete-button")
            .click(".delete-button")
            .waitForStateReady()
            .assert.elementNotPresent(".survey-item-view:nth-child(2) .answer-label")  
            
   'Check for Banner with Edit Code (Not Logged In)': (client) -> 
        client
            .assert.elementPresent(".code-message")
            
   'Survey spinner is shown when backend is called': (client) -> 
        client
            .click("div.survey-page-menu > .add-button")
            .waitForElementPresent(".survey-spinner")
            .assert.elementPresent(".spinner-text")
            
    'Start Survey Session': (client) -> 
        client
            .click(".start-button")
            .pause(100)
            .waitForElementPresent(".state-ready")
            .assert.urlContains("survey-session-page")
            .end()