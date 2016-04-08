﻿var siteRoot = "/Dispatcher";
var ViewModel = function () {
    var self = this;
    self.activeRequests = ko.observableArray();
    self.usersAndRoles = ko.observableArray();
    self.requestTypes = ko.observableArray();
    self.specialRequestTypes = ko.observableArray();
    self.providersAndTheirTasks = ko.observableArray();
    self.error = ko.observable();
    self.connectionError = ko.observable();
    self.errorTimeout = null;

    self.currentPage = ko.observable();
	self.loginVisible = ko.observable();
	self.requestsVisible = ko.observable();
    self.createRequestsVisible = ko.observable();
    self.administrationVisible = ko.observable();
	self.registerVisible = ko.observable();
    self.availabilityVisible = ko.observable();
    
    self.pages = ko.observableArray([
        {
            Name: 'Admin',
            Enabled: ko.pureComputed(function(){return self.isAdmin()})
        },
        {
            Name: 'Rejestracja',
            Enabled: ko.pureComputed(function () { return true; })
        },
        {
            Name: 'Logowanie',
            Enabled: ko.pureComputed(function () { return true; })
        },
        {
            Name: 'Tworzenie',
            Enabled: ko.pureComputed(function () { return self.isRequester() })
        },
        {
            Name: 'Zlecenia',
            Enabled: ko.pureComputed(function () { return true; })
        },
        {
            Name: 'Status',
            Enabled: ko.pureComputed(function () { return self.isServiceProvider() || self.isAdmin(); })
        }
    ]);
	
    var tokenKey = 'accessToken';

    self.user = ko.observable();
    self.userRoles = ko.observableArray();

    self.registerUsername = ko.observable();
    self.registerPassword = ko.observable();
    self.registerPassword2 = ko.observable();

    self.loginUsername = ko.observable();
    self.loginPassword = ko.observable();

    self.newRequestTypeInput = ko.observable();
    self.newSpecialRequestTypeInput = ko.observable();
    
	self.disableButton = function (element) {
	    $(element).removeClass("btn-info btn-success btn-danger").addClass("btn-default").text("Czekaj").prop('disabled', true);
	}

	self.enableButton = function(element, text, css) {
        $(element).addClass(css).removeClass("btn-default").text(text).prop('disabled', false);
	}

    self.hideAllPages = function() {
        self.registerVisible(false);
        self.loginVisible(false);
        self.requestsVisible(false);
        self.administrationVisible(false);
        self.createRequestsVisible(false);
        self.availabilityVisible(false);
    }

    self.gotoDefault = function() {
        if (!location.hash) {
            if (!self.user()) 
                self.gotoLogin();
            else if (self.isAdmin())
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
        location.hash = 'Zlecenia';
    }

    self.gotoCreateRequests = function() {
        location.hash = 'Tworzenie';
    }
    
    self.getActiveRequests = function () {
        self.getData(siteRoot + '/api/ActiveRequests/', self.updateActiveRequests, function () { self.activeRequests.removeAll(); });
    }

    self.updateActiveRequests = function(data) {
        var sortedData = data.sort(function(left, right) { return (isEmpty(left.ProvidingUserName) && isEmpty(right.ProvidingUserName)) ? 0 : (isEmpty(left.ProvidingUserName) ? -1 : 1) });
        self.activeRequests(sortedData);
        self.updateTasks();
    };

    self.updateTasks = function()
    {
        var perUserDict = {};

        self.usersAndRoles().forEach(function (user) {
            perUserDict[user.Name] = { Name: user.Name, Tasks: [], SpecialTasks: [] };
        });

        self.requestsAssignedToSomeone().forEach(function (element) {
            if (!perUserDict.hasOwnProperty(element.ProvidingUserName)) {
                perUserDict[element.ProvidingUserName] = { Name: element.ProvidingUserName, Tasks: [], SpecialTasks: [] };
            }
            var existingEntry = perUserDict[element.ProvidingUserName];
            if (element.Type.ForSelf) {
                existingEntry.SpecialTasks.push(element);
            } else {
                existingEntry.Tasks.push(element);
            }
        });

        var arrayOfTasksPerUser = [];
        for (var key in perUserDict) {
            if (perUserDict.hasOwnProperty(key)) {
                arrayOfTasksPerUser.push(perUserDict[key]);
            }
        }
        self.providersAndTheirTasks(arrayOfTasksPerUser);
    }
    
    self.getUsersAndRoles = function () {
        self.getData(siteRoot + '/api/Account/UsersAndRoles/', self.updateUsersAndRoles);
    }

    self.updateUsersAndRoles = function(data) {
        var mappedUsers = $.map(data, function(item) { return new UserWithRoles(item) });
        self.usersAndRoles(mappedUsers);
        self.updateTasks();
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
            url: siteRoot + '/api/Account/UsersAndRoles',
            headers: headers,
            data: ko.toJSON(self.usersAndRoles),
            contentType: "application/json"
        }).done(function() {
            self.getUserInfo();
        }).fail(function(jx) {
            showError(jx);
        }).always(function() {
            self.enableButton(event.target, originalText, "btn-info");
        })
    }

    self.getRequestTypes = function() {
        self.getData(siteRoot + '/api/DispatchRequestTypes/', self.requestTypes);
    }

    self.getSpecialRequestTypes = function() {
        self.getData(siteRoot + '/api/selfRequestTypes/', self.specialRequestTypes);
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
        
        var originalText = event.target.textContent;
        self.disableButton(event.target);

        $.ajax({
            type: 'PUT',
            url: siteRoot + '/api/CompleteRequest/' + request.Id,
            headers: headers
        }).fail(function(jx) {
            showError(jx);
        }).always(function() {
            self.enableButton(event.target, originalText, "btn-success");
        });
    }

    self.cancelRequest = function (request, event) {
        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        var originalText = event.target.textContent;
        self.disableButton(event.target);

        $.ajax({
            type: 'PUT',
            url: siteRoot + '/api/CancelRequest/' + request.Id,
            headers: headers
        }).fail(function(jx) {
            showError(jx);
        }).always(function () {
            self.enableButton(event.target, originalText, "btn-danger");
        });
    }

    self.deleteRequest = function (request, event) {
        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        var originalText = event.target.textContent;
        self.disableButton(event.target);

        $.ajax({
            type: 'DELETE',
            url: siteRoot + '/api/DeleteRequest/' + request.Id,
            headers: headers
        }).fail(function(jx) {
            showError(jx);
        }).always(function () {
            self.enableButton(event.target, originalText, "btn-danger");
        });
    }
	
    self.acceptRequest = function (request, event) {
        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        var originalText = event.target.textContent;
        self.disableButton(event.target);

        $.ajax({
            type: 'PUT',
            url: siteRoot + '/api/AcceptRequest/' + request.Id,
            headers: headers
        }).fail(function(err) {
            showError(err);
        }).always(function () {
            self.enableButton(event.target, originalText, "btn-success");
        });
    }

    self.createRequest = function (requestType) {
        self.getData(siteRoot + '/api/CreateRequest/' + requestType.Id);
    }

    self.createSpecialRequest = function(requestType) {
        self.getData(siteRoot + '/api/CreateSpecialRequest/' + requestType.Id);
    }

    self.createSpecialRequestType = function(form) {
        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        var originalText = form[1].textContent;
        self.disableButton(form[1]);
        $.ajax({
            type: 'POST',
            url: siteRoot + '/api/DispatchRequestTypes/',
            data: { Name: self.newSpecialRequestTypeInput(), ForSelf: true },
            headers: headers
        }).fail(function (jx) {
            showError(jx);
        }).always(function () {
            location.hash = 'Admin';
            self.getSpecialRequestTypes();
            self.newSpecialRequestTypeInput(null);
            self.enableButton(form[1], originalText, "btn-info");
        });
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
            url: siteRoot + '/api/DispatchRequestTypes/',
            data: { Name: self.newRequestTypeInput() },
            headers: headers
        }).fail(function (jx) {
            showError(jx);
        }).always(function () {
            location.hash = 'Admin';
            self.getRequestTypes();
            self.newRequestTypeInput(null);
            self.enableButton(form[1], originalText, "btn-info");
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
            if (callback)
                callback();
            return;
        }
        var headers = {};
        headers.Authorization = 'Bearer ' + token;
        
        $.ajax({
            type: 'GET',
            url: siteRoot + '/api/Account/UserInfo',
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
            url: siteRoot + '/api/Account/Register',
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
            url: siteRoot + '/Token',
            data: loginData
        }).done(function (data) {
            localStorage.setItem(tokenKey, data.access_token);
            self.clearForms();
            self.getUserInfo(function () {
                location.hash = '';
                self.gotoDefault();
            });
        }).fail(function (error) {
            showError(error);
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

    self.requestsAssignedToSomeone = ko.pureComputed(function () {
        return self.activeRequests().filter(function (el) {
            return !isEmpty(el.ProvidingUserName);
        });
    });

    self.requestsAssignedToMe = ko.pureComputed(function () {
        return self.requestsAssignedToSomeone().filter(function (el) {
            return el.ProvidingUserName === self.user();
        });
    });

    self.requestsCreatedByMe = ko.pureComputed(function() {
        return self.activeRequests().filter(function (el) {
            return el.RequestingUserName === self.user();
        });
    });

    self.requestTypeActiveForMe = function(id) {
        var temp = self.requestsCreatedByMe().filter(function (el) {
            return el.Type.Id === id;
        });
        return temp.length > 0;
    }

    self.specialRequestTypeActiveForMe = function(id) {
        var temp = self.requestsAssignedToMe().filter(function(el) {
            return el.TypeId === id;
        });
        return temp.length > 0;
    }
    
    self.logout = function () {
        self.clearUserInfo();
        localStorage.removeItem(tokenKey);
        self.gotoLogin();
    }

    // Initialize the navigation
    Sammy(function () {
        this.get('#Status', function() {
            self.currentPage('Status');
            self.hideAllPages();
            self.availabilityVisible(true);
        });
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
        this.get('#Zlecenia', function () {
            if (self.isServiceProvider()) {
                self.getSpecialRequestTypes();
            }
            self.currentPage('Zlecenia');
            self.hideAllPages();
            self.requestsVisible(true);
        });
        this.get('#Tworzenie', function () {
            if (self.isRequester()) {
                self.getRequestTypes();
            }
            self.currentPage('Tworzenie');
            self.hideAllPages();
            self.createRequestsVisible(true);
        });
        this.get('#Admin', function () {
            if (self.isAdmin()) {
                self.getRequestTypes();
                self.getSpecialRequestTypes();
            }
            self.currentPage('Admin');
            self.hideAllPages();
            self.administrationVisible(true);
        });
        this._checkFormSubmission = function(form) {
            return false;
        }
    }).run();

    // Fetch the initial data.
    self.getUserInfo(function () {
        self.gotoDefault();
    });
    self.getUsersAndRoles();
    self.getActiveRequests();
    self.getRequestTypes();
    self.getSpecialRequestTypes();
};

function UserWithRoles(data) {
    this.Name = data.Name;
    this.IsAdmin = ko.observable(data.Roles.indexOf('Admin') > -1);
    this.IsServiceProvider = ko.observable(data.Roles.indexOf('ObslugaZlecen') > -1);
    this.IsRequester = ko.observable(data.Roles.indexOf('TworzenieZlecen') > -1);
}

var viewModel = new ViewModel();
ko.applyBindings(viewModel);

var hub = $.connection.requestsHub;
hub.client.updateActiveRequests = function (data) {
    viewModel.updateActiveRequests(data);
};
hub.client.updateUsersAndRoles = function(data) {
    viewModel.updateUsersAndRoles(data);
}

// Start the connection.
$.connection.hub.start().done(function () { viewModel.connectionError(null) });

$.connection.hub.reconnecting(function () {
    viewModel.connectionError("Utracono połączenie z serwerem");
});

$.connection.hub.reconnected(function () {
    viewModel.connectionError(null);
});

// Reconnect after disconnect
$.connection.hub.disconnected(function () {
    setTimeout(function () {
        $.connection.hub.start().done(function () { viewModel.connectionError(null) });
    }, 5000); // Restart connection after 5 seconds.
});



moment.locale('pl');
$("a.collapse-menu-after-click").click(function() {
    $(".navbar-collapse.in").collapse('hide');
});

function parseDate(date) {
    if (date) {
        return moment.utc(date).local().fromNow();
    } else {
        return "";
    }
}

function parseTask(task) {
    if (!task)
        return null;
    var started = moment.utc(task.PickedUpDate);
    var now = moment.utc();
    var diff = moment.duration(now.diff(started)).humanize();
    var text = task.Type.Name + ' (' + diff + ')';
    return text.split(' ').join(String.fromCharCode(160));
}

function isEmpty(str) {
    return (!str || 0 === str.length);
}

