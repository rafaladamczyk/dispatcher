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
    
	self.disableButton = function (element) {
	    $(element).removeClass("btn-info btn-success").addClass("btn-default").text("Czekaj").prop('disabled', true);
	}

    self.getActiveRequests = function () {
        var token = localStorage.getItem(tokenKey);
        if (!token)
            return;
        var headers = {};
        headers.Authorization = 'Bearer ' + token;

        $.ajax({
            type: 'GET',
            url: '/api/ActiveRequests/',
            headers: headers
        }).done(function (data) {
            self.activeRequests(data);
        }).fail(function (err) {
            self.activeRequests.removeAll();
            showError(err);
        });
    }
	
	 self.getMyRequests = function() {
        var token = localStorage.getItem(tokenKey);
        if (!token)
            return;
        var headers = {};
        headers.Authorization = 'Bearer ' + token;

        $.ajax({
            type: 'GET',
            url: '/api/MyRequests/',
            headers: headers
        }).done(function(data) {
            self.myRequests(data);
        }).fail(function(err) {
            self.myRequests.removeAll();
            showError(err);
        });
    }

    self.completeRequest = function(request, event) {
        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }
        
        self.disableButton(event.target);

        $.ajax({
            type: 'PUT',
            url: '/api/CompleteRequest/' + request.Id,
            headers: headers
        }).done(function () {
            self.getActiveRequests();
        }).fail(function (jx) {
            showError(jx);
        }).always(function () {
            self.getMyRequests();
        });
    }

    self.cancelRequest = function (request, event) {
        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        self.disableButton(event.target);

        $.ajax({
            type: 'PUT',
            url: '/api/CancelRequest/' + request.Id,
            headers: headers
        }).done(function () {
            self.getActiveRequests();
        }).fail(function (jx) {
            showError(jx);
        }).always(function() {
            self.getMyRequests();
        });
    }
	
    self.acceptRequest = function (request, event) {
        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        self.disableButton(event.target);

        $.ajax({
            type: 'PUT',
            url: '/api/AcceptRequest/' + request.Id,
            headers: headers
        }).done(function () {
            self.getMyRequests();
        }).fail(function (jx, message, error) {
            showError(jx);
        }).always(function () {
            self.getActiveRequests();
        });
    }

    self.createRequest = function() {
        var requesterId = Math.floor((Math.random() * 3) + 1);
        var requestType = Math.floor((Math.random() * 2));
        $.getJSON('/api/CreateRequest/' + requesterId + '/' + requestType, self.getActiveRequests).fail(showError);
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

    self.getUserInfo = function () {
        self.user(null);

        var token = localStorage.getItem(tokenKey);
        if (!token) {
            return;
        }
        var headers = {};
        headers.Authorization = 'Bearer ' + token;
        
        $.ajax({
            type: 'GET',
            url: '/api/Account/UserInfo',
            headers: headers
        }).done(function (data) {
            self.user(data.UserName);
        }).fail(function(error) {
            self.user(null);
            showError(error);
        });
    }

    self.clearForms = function () {
        self.registerUsername('');
        self.registerPassword('');
        self.registerPassword2('');
        self.loginUsername('');
        self.loginPassword('');
    }

    self.register = function () {
        
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
        }).done(function() {
            self.clearForms();
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
            self.clearForms();
            self.user(data.userName);
            // Cache the access token in session storage.
            localStorage.setItem(tokenKey, data.access_token);
            self.getActiveRequests();
			self.getMyRequests();
        }).fail(showError);
    }

    self.loginMessage = ko.pureComputed(function() {
        return self.user() ? "Zalogowany jako: " + self.user() : "Niezalogowany";
    }, self);
    
    self.logout = function () {
        self.user(null);
		self.myRequests.removeAll();
        localStorage.removeItem(tokenKey);
    }

   // Fetch the initial data.
    self.getUserInfo();
	self.getMyRequests();
    self.getActiveRequests();
};

var viewModel = new ViewModel();
window.setInterval(viewModel.getActiveRequests, 5000);
ko.applyBindings(viewModel);

moment.locale('pl');

function parseDate(date) {
    return moment(date).fromNow();
}

   