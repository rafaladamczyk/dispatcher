var ViewModel = function () {
    var self = this;
    self.activeRequests = ko.observableArray();
    self.myRequests = ko.observableArray();
    self.usersAndRoles = ko.observableArray();
	self.error = ko.observable();
    self.errorTimeout = null;

    self.pages = ['Administracja','Rejestracja', 'Logowanie', 'Tworzenie Zleceń', 'Obsługa Zleceń'];
	self.visibiliy = {};
    self.currentPage = ko.observable();
	self.loginVisible = ko.observable();
	self.requestsVisible = ko.observable();
    self.createRequestsVisible = ko.observable();
	self.administrationVisible = ko.observable();
    self.registerVisible = ko.observable();
    
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
    
    self.hideAllPages = function() {
        self.registerVisible(false);
        self.loginVisible(false);
        self.requestsVisible(false);
        self.administrationVisible(false);
        self.createRequestsVisible(false);
    }

    self.gotoDefault = function() {
        if (self.user) 
            self.gotoRequests();
        else
            self.gotoLogin();
    }
    
	self.gotoLogin = function() {
        location.hash = 'Logowanie';
    }

    self.gotoRequests = function () {
        location.hash = 'Obsługa Zleceń';
    }
    
    Sammy(function() {
        this.get('#Rejestracja', function () {
            self.currentPage('Rejestracja');
            self.hideAllPages();
            self.registerVisible(true);
        });
        this.get('#Logowanie', function () {
            self.currentPage('Logowanie');
            self.hideAllPages();
            self.loginVisible(true);
        });
        this.get('#Obsługa Zleceń', function () {
            self.currentPage('Obsługa Zleceń');
            self.hideAllPages();
            self.requestsVisible(true);
        });
        this.get('#Tworzenie Zleceń', function () {
            self.currentPage('Tworzenie Zleceń');
            self.hideAllPages();
            self.createRequestsVisible(true);
        });
        this.get('#Administracja', function () {
            self.getUsersAndRoles();
            self.currentPage('Administracja');
            self.hideAllPages();
            self.administrationVisible(true);
        });
    }).run();
    
    self.getActiveRequests = function () {
        self.getData('/api/ActiveRequests/', self.activeRequests, function () { self.activeRequests.removeAll(); });
    }
	
    self.getMyRequests = function () {
        self.getData('/api/MyRequests/', self.myRequests, function() { self.myRequests.removeAll(); });
    }

    self.getUsersAndRoles = function () {
        self.getData('/api/Account/UsersAndRoles/', function(data) {
            var mappedUsers = $.map(data, function (item) { return new UserWithRoles(item) });
            self.usersAndRoles(mappedUsers);
        });
    }

    self.saveRoles = function (request, event) {
        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }
        
        $.ajax({
            type: 'POST',
            url: '/api/Account/UsersAndRoles',
            headers: headers,
            data: ko.toJSON(self.usersAndRoles),
            contentType: "application/json"
        }).done(function() {
            self.getUsersAndRoles();
            self.getUserInfo();
        }).fail(function(jx) {
            showError(jx);
        });
    }


    self.getData = function (uri, callback, errorCallback) {
	     var token = localStorage.getItem(tokenKey);
	     if (!token)
	         return;
	     var headers = {};
	     headers.Authorization = 'Bearer ' + token;

	     $.ajax({
	         type: 'GET',
	         url: uri,
	         headers: headers
	     }).done(function (data) {
	         if (callback) {
	             callback(data);
	         }
	     }).fail(function (err) {
	         if (errorCallback) {
	             errorCallback(err);
	         }
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
        if (self.errorTimeout != null) {
            clearTimeout(self.errorTimeout);
        }
        self.errorTimeout = setTimeout(function() { self.error(null);self.errorTimeout = null; }, 5000);
    }
    
    self.clearUserInfo = function() {
        self.user(null);
        self.userRoles.removeAll();
    }

    self.getUserInfo = function (callback) {
        var token = localStorage.getItem(tokenKey);
        if (!token) {
            self.gotoLogin();
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
            self.gotoDefault();
        }).fail(function (error) {
            showError(error);
            self.gotoLogin();
        });
    }

    self.loginMessage = ko.pureComputed(function() {
        return self.user() ? "Zalogowany jako: " + self.user() : "Niezalogowany";
    }, self);

    self.isAdmin = ko.pureComputed(function() {
        return self.userRoles().indexOf('Admin') > -1;
    }, self);

    self.isServiceProvider = ko.pureComputed(function () {
        return self.userRoles().indexOf('ObslugaZlecen') > -1;
    }, self);

    self.isRequester = ko.pureComputed(function () {
        return self.userRoles().indexOf('TworzenieZlecen') > -1;
    }, self);
    
    self.logout = function () {
        self.clearUserInfo();
        self.usersAndRoles.removeAll();
		self.myRequests.removeAll();
		localStorage.removeItem(tokenKey);
        self.gotoLogin();
    }

   // Fetch the initial data.
    self.getUserInfo(function (){self.getMyRequests(), self.gotoDefault()});
    self.getActiveRequests();
};

function UserWithRoles(data) {
    this.Name = data.Name;
    this.IsAdmin = ko.observable(data.Roles.indexOf('Admin') > -1);
    this.IsServiceProvider = ko.observable(data.Roles.indexOf('ObslugaZlecen') > -1);
    this.IsRequester = ko.observable(data.Roles.indexOf('TworzenieZlecen') > -1);
}

var viewModel = new ViewModel();
window.setInterval(viewModel.getActiveRequests, 5000);
ko.applyBindings(viewModel);

moment.locale('pl');

function parseDate(date) {
    return moment.utc(date).local().fromNow();
}

   