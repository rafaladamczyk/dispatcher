var ViewModel = function () {
    var self = this;
    self.activeRequests = ko.observableArray();
	self.myRequests = ko.observableArray();
    self.error = ko.observable();

    var tokenKey = 'accessToken';

    self.user = ko.observable();
    self.userRoles = ko.observableArray();

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
        var requestType = Math.floor((Math.random() * 2) + 1);
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

self.clearUserInfo = function() {
    self.user(null);
    self.userRoles.removeAll();
}

    self.getUserInfo = function (callback) {
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
            self.user(data.Name);
            self.userRoles(data.Roles);
            callback();
        }).fail(function(error) {
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
            localStorage.setItem(tokenKey, data.access_token);
            self.clearForms();
            self.getUserInfo(self.getMyRequests);
            self.getActiveRequests();
        }).fail(showError);
    }

    self.loginMessage = ko.pureComputed(function() {
        return self.user() ? "Zalogowany jako: " + self.user() : "Niezalogowany";
    }, self);

    self.isAdmin = ko.pureComputed(function() {
        return self.userRoles().indexOf('Admin') > -1;
    }, self);

    self.isServiceProvider = ko.pureComputed(function() {
        return self.userRoles().indexOf('ServiceProviders') > -1;
    }, self);
    
    self.logout = function () {
        self.clearUserInfo();
		self.myRequests.removeAll();
        localStorage.removeItem(tokenKey);
    }

   // Fetch the initial data.
    self.getUserInfo(self.getMyRequests);
    self.getActiveRequests();
};

var viewModel = new ViewModel();
window.setInterval(viewModel.getActiveRequests, 5000);
ko.applyBindings(viewModel);

moment.locale('pl');

function parseDate(date) {
    return moment(date).fromNow();
}

   