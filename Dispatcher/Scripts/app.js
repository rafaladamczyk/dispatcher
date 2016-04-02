﻿var ViewModel = function () {
    var self = this;
    self.activeRequests = ko.observableArray();
    self.usersAndRoles = ko.observableArray();
    self.requestTypes = ko.observableArray();
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

    self.newRequestTypeInput = ko.observable();
    
	self.disableButton = function (element) {
	    $(element).removeClass("btn-info btn-success").addClass("btn-default").text("Czekaj").prop('disabled', true);
	}

	self.enableButton = function(element, text) {
        $(element).addClass("btn-info").removeClass("btn-default").text(text).prop('disabled', false);
	}

    self.hideAllPages = function() {
        self.registerVisible(false);
        self.loginVisible(false);
        self.requestsVisible(false);
        self.administrationVisible(false);
        self.createRequestsVisible(false);
    }

    self.gotoDefault = function() {
        if (!self.user()) {
            self.gotoLogin();
        }

        if (!location.hash) {
            if (self.isAdmin())
                self.gotoRequests();
            else if (self.isRequester())
                self.gotoCreateRequests();
            else
                self.gotoRequests();
        }
    }

    self.gotoLogin = function() {
        location.hash = 'Logowanie';
    }

    self.gotoRequests = function () {
        location.hash = 'Obsługa Zleceń';
    }

    self.gotoCreateRequests = function() {
        location.hash = 'Tworzenie Zleceń';
    }
    
    self.getActiveRequests = function () {
        self.getData('/api/ActiveRequests/', self.activeRequests, function () { self.activeRequests.removeAll(); });
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

        var originalText = event.target.textContent;
        self.disableButton(event.target);
        
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
        }).always(function() {
            self.enableButton(event.target, originalText);
        });
    }

    self.getRequestTypes = function() {
        self.getData('/api/DispatchRequestTypes/', self.requestTypes);
    }
    
    self.getData = function (uri, callback, errorCallback) {
	     var token = localStorage.getItem(tokenKey);
	     var headers = {};
	     if (token) {
            headers.Authorization = 'Bearer ' + token;
        }
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
        }).fail(function (jx) {
            showError(jx);
        }).always(function () {
            self.getActiveRequests();
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
        }).fail(function (jx) {
            showError(jx);
        }).always(function() {
            self.getActiveRequests();
        });
    }


    self.deleteRequest = function (request, event) {
        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        self.disableButton(event.target);

        $.ajax({
            type: 'DELETE',
            url: '/api/DeleteRequest/' + request.Id,
            headers: headers
        }).fail(function (jx) {
            showError(jx);
        }).always(function () {
            self.getActiveRequests();
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
        }).fail(function (err) {
            showError(err);
        }).always(function () {
            self.getActiveRequests();
        });
    }

    self.createRequest = function (requestType) {
        self.getData('/api/CreateRequest/' + requestType.Id, function() {self.getActiveRequests()});
    }

    self.createRequestType = function(form) {
        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        var originalText = form[1].textContent;
        self.disableButton(form[1]);
        $.ajax({
            type: 'POST',
            url: '/api/DispatchRequestTypes/',
            data: { Name: self.newRequestTypeInput() },
            headers: headers
        }).fail(function (jx) {
            showError(jx);
        }).always(function () {
            location.hash = 'Administracja';
            self.getRequestTypes();
            self.newRequestTypeInput(null);
            self.enableButton(form[1], originalText);
        });
    }

    function showError(jqXHR) {
        var text = '';
        if (jqXHR.responseJSON) {
            var err = jqXHR.responseJSON.error_description;
            var msg = jqXHR.responseJSON.Message;
            text = err ? err : (msg ? msg : '');
        }
        window.scrollTo(0, 0);
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
            if (callback) {
                callback();
            }
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
            self.getUserInfo(function() { self.getActiveRequests(); self.gotoDefault(); });
        }).fail(function (error) {
            showError(error);
            self.gotoLogin();
        });
    }

    self.loggedInUser = ko.pureComputed(function () {
        return self.user() ? self.user() + "  " : "Niezalogowany  ";
    }, self);
    self.loginMessage = ko.pureComputed(function () {
        return self.user() ? "Zalogowany jako: " + self.user() : "Niezalogowany";
    }, self);

    self.isAdmin = ko.pureComputed(function () {
        return self.userRoles().indexOf('Admin') > -1;
    }, self);

    self.isServiceProvider = ko.pureComputed(function () {
        return self.userRoles().indexOf('ObslugaZlecen') > -1;
    }, self);

    self.isRequester = ko.pureComputed(function () {
        return self.userRoles().indexOf('TworzenieZlecen') > -1;
    }, self);

    self.requestsAssignedToMe = ko.pureComputed(function () {
        return self.activeRequests().filter(function (el) {
            return el.ProvidingUserName === self.user();
        });
    }, self);

    self.requestsCreatedByMe = ko.pureComputed(function() {
        return self.activeRequests().filter(function (el) {
            return el.RequestingUserName === self.user();
        });
    }, self);

    self.requestTypeActiveForMe = function(id) {
        var temp = self.requestsCreatedByMe().filter(function (el) {
            return el.Type.Id === id;
        });
        return temp.length > 0;
    }
    
    self.logout = function () {
        self.clearUserInfo();
        self.usersAndRoles.removeAll();
        localStorage.removeItem(tokenKey);
        self.gotoLogin();
    }

    // Initialize the navigation
    Sammy(function () {
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
            if (self.isRequester()) {
                self.getRequestTypes();
            }
            self.currentPage('Tworzenie Zleceń');
            self.hideAllPages();
            self.createRequestsVisible(true);
        });
        this.get('#Administracja', function () {
            if (self.isAdmin()) {
                self.getUsersAndRoles();
            }
            self.currentPage('Administracja');
            self.hideAllPages();
            self.administrationVisible(true);
        });
    }).run();

    // Fetch the initial data.
    self.getUserInfo(function() {
        if (self.isAdmin()) {
            self.getUsersAndRoles();
        }
        self.gotoDefault()
    });
    self.getActiveRequests();
    self.getRequestTypes();
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

   