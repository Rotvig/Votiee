module.exports =
   tags: ['TopBar']

   'Can not navigate Back from FrontPage': (client) ->
        client
            .setup()
            .click(".back-button")
            .waitForStateReady()
            .assert.attributeEquals(".back-button", "disabled", "true")
            .assert.elementPresent(".front-page")
            .end()
            
   'Can go to login Page': (client) ->
        client
            .setup()
            .click(".account-button")
            .waitForStateReady()
            .assert.elementPresent(".login-page")
            
   'Top bar contains page-name': (client) ->
        client
            .waitForStateReady()
            .assert.containsText(".topbar > .page-name", "Login")
            .end()