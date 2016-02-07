var ViewModel = function () {
    var self = this;
    self.activeRequests = ko.observableArray();
	self.myRequests = ko.observableArray();
    self.error = ko.observable();

    var tokenKey = 'accessToken';

    self.user = ko.observable();

    self.registerUsername = ko.observable();
    self.registerPassword = ko.observable();
    self.registerPassword2 = ko.observable();

    self.loginUsername = ko.observable();
    self.loginPassword = ko.observable();

    var requestsUri = '/api/ActiveRequests/';
	var createRequestUri = '/api/CreateRequest/';

    self.getActiveRequests = function() {
        $.getJSON(requestsUri, function(data) {
            self.activeRequests(data);
        });
    }
	
	 self.getMyRequests = function() {
        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }		
        $.ajax({
            type: 'GET',
            url: '/api/MyRequests/',
            headers: headers
        }).done(function (data) {
            self.myRequests(data);
        }).fail(function(err) {
            self.myRequests.removeAll();
            showError(err);
        });
    }

    self.completeRequest = function(request) {
        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }
        $.ajax({
            type: 'PUT',
            url: '/api/CompleteRequest/' + request.Id,
            headers: headers
        }).done(function () {
            self.getMyRequests();
            self.getActiveRequests();
        }).fail(function (jx, message, error) {
            showError(jx);
        });
    }
	
    self.acceptRequest = function (request) {
        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }
        $.ajax({
            type: 'PUT',
            url: '/api/AcceptRequest/' + request.Id,
            headers: headers
        }).done(function () {
            self.getMyRequests();
            self.getActiveRequests();
        }).fail(function (jx, message, error) {
            showError(jx);
        });
    }

    self.createRequest = function() {
        var requesterId = Math.floor((Math.random() * 3) + 1);
        var requestType = Math.floor((Math.random() * 2));
        $.getJSON(createRequestUri + requesterId + '/' + requestType, self.getActiveRequests).fail(showError);
    }

    function showError(jqXHR) {
        var text = '';
        if (jqXHR.responseJSON) {
            var err = jqXHR.responseJSON.error_description;
            var msg = jqXHR.responseJSON.Message;
            text = err ? err : (msg ? msg : '');
        }
        self.error(jqXHR.status + ': ' + jqXHR.statusText + '. ' + text);
    }

    self.userInfo = function () {
        self.user('');

        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        $.ajax({
            type: 'GET',
            url: '/api/Account/UserInfo',
            headers: headers
        }).done(function (data) {
            self.user(data.UserName);
        }).fail(function(error) {
            self.user('');
            showError(error);
        });
    }

    self.register = function () {
        self.user('');

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
            self.user("Registered!");
        }).fail(showError);
    }

    self.login = function () {
        self.logout();

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
            localStorage.setItem(tokenKey, data.access_token);
			self.getMyRequests();
        }).fail(showError);
    }

    self.loginMessage = ko.pureComputed(function() {
        return self.user() ? "Zalogowany jako: " + self.user() : "Niezalogowany";
    }, self);

    self.logout = function () {
        self.user('');
		self.myRequests.removeAll();
        localStorage.removeItem(tokenKey)
    }

    // Fetch the initial data.
    self.userInfo();
	self.getMyRequests();
    self.getActiveRequests();
};

var viewModel = new ViewModel();
window.setInterval(viewModel.getActiveRequests, 1000);
ko.applyBindings(viewModel);