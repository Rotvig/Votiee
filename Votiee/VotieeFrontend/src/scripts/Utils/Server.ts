module Utils {

    var baseUrl = "http://localhost:50977/";

    var hubProxy: HubProxy;

    var tokenKey = 'accessToken';

    export interface method {
        name: string;
        method: Function;
    }

    export interface signalR extends server {
        methods?: Array<method>;
        hubName: string;
    }

    export interface server {
        methodName?: string;
        data?: {};
        onSuccess?: Function;
        onFail?: Function;
    }

    export interface restApi extends server {
        url: string;
        onSuccess?: (data: any, textStatus: string, jqXHR: any) => void;
        onFail?: (jqXHR: any, textStatus: string, errorThrown: String) => void;
    }

    export interface restApiLogin extends server {
        onSuccess?: (data: any, textStatus: string, jqXHR: any) => void;
        onFail?: (jqXHR: any, textStatus: string, errorThrown: String) => void;
    }

    export class Server {

        public call(callInfo: restApi) {
            if (callInfo.data == null) {
                callInfo.data = {};
            }
            var error = callInfo.onFail == null ? this.onErrorDefault : callInfo.onFail;
            var success = callInfo.onSuccess == null ? this.onSuccessDefault : callInfo.onSuccess;

            // Authorization: If we already have a bearer token, set the Authorization header.
            var token = localStorage.getItem(tokenKey);
            var headers = { Authorization: ""};
            if (token) {
                headers.Authorization = 'Bearer ' + token;
            }

            (<any>window).stateCalling = true;
            //Create AJAX call to server
            jQuery.ajax({
                url: baseUrl + callInfo.url,
                method: callInfo.methodName,
                data: JSON.stringify(callInfo.data),
                dataType: "json",
                contentType: "application/json",
                error: (jqXHR: any, textStatus: string, errorThrown: String) => {
                    (<any>window).stateCalling = false;
                    (<any>window).loading = false;
                    error(jqXHR, textStatus, errorThrown);
                },
                success: (data: any, textStatus: string, jqXHR: any) => {
                    (<any>window).stateCalling = false;
                    success(data, textStatus, jqXHR);
                },
                headers: (<any>headers)
            });
        }

        private onErrorDefault(jqXHR: any, textStatus: string, errorThrown: String) {
        }

        public login(callInfo: restApiLogin) {
            if (callInfo.data == null) {
                callInfo.data = {};
            }
            var error = callInfo.onFail == null ? this.onErrorDefault : callInfo.onFail;
            var success = callInfo.onSuccess == null ? this.onSuccessDefault : callInfo.onSuccess;
            (<any>window).stateCalling = true;
            //Create AJAX call to server
            jQuery.ajax({
                url: baseUrl + 'Token',
                method: "POST",
                data: callInfo.data,
                xhrFields: {
                    withCredentials: true
                },
                error: (jqXHR: any, textStatus: string, errorThrown: String) => {
                    (<any>window).stateCalling = false;
                    (<any>window).loading = false;
                    error(jqXHR, textStatus, errorThrown);
                },
                success: (data: any, textStatus: string, jqXHR: any) => {
                    (<any>window).stateCalling = false;
                    success(data, textStatus, jqXHR);
                }
            });
        }


        setupHubProxy(hub: signalR) {
            var fail = hub.onFail == null ? this.onFailDefault : hub.onFail;
            var success = hub.onSuccess == null ? this.onSuccessDefault : hub.onSuccess;
            var connection = $.hubConnection();
            hubProxy = connection.createHubProxy(hub.hubName);

            hub.methods.forEach(x=> { hubProxy.on(x.name, (args)=> x.method(args)); });

            connection.url = baseUrl + "signalr";
            connection.start().done(() => success()).fail((x) => {
                (<any>window).loading = false;
                fail(x);
            });
        }

        invokeMethodOnHub(invokeMethodObject: server) {
            var fail = invokeMethodObject.onFail == null ? this.onFailDefault : invokeMethodObject.onFail;
            var success = invokeMethodObject.onSuccess == null ? this.onSuccessDefault : invokeMethodObject.onSuccess;
            if (invokeMethodObject.data != null) {
                hubProxy.invoke(invokeMethodObject.methodName, invokeMethodObject.data).done(x => success(x)).fail(error => {
                    (<any>window).loading = false;
                    fail(error);
                });

            } else {
                hubProxy.invoke(invokeMethodObject.methodName).done(x => success(x)).fail(error => {
                    (<any>window).loading = false;
                    fail(error);
                });
            }
        }

        private onFailDefault(error: any) {
        }

        private onSuccessDefault() {
        }

    }

}

/*      How To Use the Server class
        var foo = new Utils.Server();
        
        foo.call({
                url: "api/conference/submitNewConference",
                methodName: "GET",
                onSuccess: this.onSucces,
                onFail: this.onError
            }
        
        foo.call({
                url: "api/conference/submitNewConference",
                methodName: "POST",
                onSuccess: this.onSucces,
                data: {
                    "QuestionText": this.state.questionText,
                    "AnswerList": this.state.answerList
                },
                onFail: this.onError
            }

        onSucces(args) {
            //Do something here
        }

        onError(args) {
            //Do something here
        }
*/


/*      How to use setupHub()

        interface HubState {
            hubIsConnected: boolean;
        }

        state: HubState =
        {
            hubIsConnected: false
        }

        componentDidMount() {
            foo.setupHubProxy({
                hubName: "MyHub1",
                methods: [{ name: "Hello", method: ()=> console.log("hello") }, { name: "recieveSomeTestData", method: (json: string)=> console.log(JSON.parse(json)) }],
                onSuccess: ()=> this.setState({ hubIsConnected: true })
            });
        }

        How to use invokeMthodOnhub()

         if (this.state.hubIsConnected) {
                    foo.invokeMethodOnHub({
                        methodName: "Hello",
                        onSuccess: ()=> console.log("invoked succesfull")
                    });

                    foo.invokeMethodOnHub({
                        methodName: "GetSomeTestData",
                        onSuccess: () => console.log("invoked succesfull"),
                        data: "sometestValuetodebugOn"
                    });
                }

*/