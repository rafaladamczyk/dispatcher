var ViewModel = function () {
    var self = this;
    self.activeRequests = ko.observableArray();
    self.error = ko.observable();

    var tokenKey = 'accessToken';

    self.result = ko.observable();
    self.user = ko.observable();

    self.registerUsername = ko.observable();
    self.registerPassword = ko.observable();
    self.registerPassword2 = ko.observable();

    self.loginUsername = ko.observable();
    self.loginPassword = ko.observable();



    var requestsUri = '/api/ActiveRequests/';
    var acceptRequestUri = '/api/AcceptRequest/';
	var completeRequestUri = '/api/CompleteRequest/';
    var createRequestUri = '/api/CreateRequest/';

    self.getActiveRequests = function() {
        $.getJSON(requestsUri, function(data) {
            self.activeRequests(data);
        });
    }

    self.completeRequest = function(request) {
        var token = sessionStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }
        $.ajax(completeRequestUri + request.Id, {
            type: "PUT", contentType: "application/json",
            headers: headers, 
            success: self.getActiveRequests,
            error: function (jx, message, error) {alert(message);}
        });
    }
	
    self.acceptRequest = function (request) {
        var token = sessionStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }
        $.ajax(acceptRequestUri + request.Id, {
            type: "PUT", contentType: "application/json",
            headers: headers,
            success: self.getActiveRequests,
            error: function (jx, message, error) {alert(message);}
        });
    }

    self.createRequest = function() {
        var requesterId = Math.floor((Math.random() * 3) + 1);
        var requestType = Math.floor((Math.random() * 2));
        $.getJSON(createRequestUri + requesterId + '/' + requestType, self.getActiveRequests);
    }

    function showError(jqXHR) {
        self.result(jqXHR.status + ': ' + jqXHR.statusText);
    }

    self.callApi = function () {
        self.result('');

        var token = sessionStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        $.ajax({
            type: 'GET',
            url: '/api/Account/UserInfo',
            headers: headers
        }).done(function (data) {
            self.result(data.UserName);
        }).fail(showError);
    }

    self.register = function () {
        self.result('');

        var data = {
            UserName: self.registerUsername(),
            Password: self.registerPassword(),
            ConfirmPassword: self.registerPassword2()
        };

        $.ajax({
            type: 'POST',
            url: '/api/Account/Register',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(data)
        }).done(function (data) {
            self.result("Done!");
        }).fail(showError);
    }

    self.login = function () {
        self.result('');

        var loginData = {
            grant_type: 'password',
            username: self.loginUsername(),
            password: self.loginPassword()
        };

        $.ajax({
            type: 'POST',
            url: '/Token',
            data: loginData
        }).done(function (data) {
            self.user(data.userName);
            // Cache the access token in session storage.
            sessionStorage.setItem(tokenKey, data.access_token);
        }).fail(showError);
    }

    self.logout = function () {
        self.user('');
        sessionStorage.removeItem(tokenKey)
    }

    // Fetch the initial data.

    self.getActiveRequests();
};

var viewModel = new ViewModel();
window.setInterval(viewModel.getActiveRequests, 1000);
ko.applyBindings(viewModel);