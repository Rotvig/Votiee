testWrongPassword = "Wrongkode12"

module.exports =
   tags: ['LoginPage']

   'Register New User with wrong password confirmation': (client) ->
        client
            .setup()
            .click(".account-button")
            .waitForElementPresent(".login-page")
            .click(".toggle-buttons .register")
            .waitForElementPresent(".register-part")
            .assert.elementNotPresent(".error-message")  
            .setValue(".email-input", client.globals.loginTestMail)
            .setValue(".write-password", client.globals.testPassword)
            .setValue(".repeat-password", testWrongPassword)
            .click(".register-button")
            .waitForStateReady()
            .assert.elementPresent(".error-message")  
            
    'Register New User': (client) ->
        client
            .clearValue(".repeat-password")         
            .setValue(".repeat-password", client.globals.testPassword)
            .click(".register-button")
            .waitForStateReady()
            .assert.urlContains("menu-page")
            
    'Log in with wrong password (after loggin out)': (client) ->
        client
            .click(".logout-button")
            .waitForElementPresent(".front-page")
            .click(".account-button")
            .waitForElementPresent(".login-page")
            .assert.elementNotPresent(".error-message")  
            .setValue(".email-input", client.globals.loginTestMail)
            .setValue(".password-input", testWrongPassword)
            .click(".login-button")
            .waitForStateReady()
            .assert.elementPresent(".error-message")
            
    'Log in with existing user': (client) ->
        client
            .clearValue(".password-input")         
            .setValue(".password-input", client.globals.testPassword)
            .click(".login-button")
            .waitForStateReady()
            .assert.urlContains("menu-page")
            .end()