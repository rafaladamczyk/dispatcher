﻿var siteRoot = "/Dispatcher";
var ViewModel = function () {
    var self = this;
    self.activeRequests = ko.observableArray();
    self.usersAndRoles = ko.observableArray();
    self.requesters = ko.pureComputed(function() {
        return ko.utils.arrayFilter(self.usersAndRoles(), function(user) {
            return user.IsRequester();
        });
    });

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
    self.statsVisible = ko.observable();
    self.typeDetailsVisible = ko.observable();

    self.selectedRequesterNamesForEditedType = ko.observableArray([]);
    self.editedTypeId = ko.observable();
    self.editedType = ko.computed (function() {
        var type = ko.utils.arrayFirst(self.requestTypes(),
                function (requestType) {
                    return requestType.Id === self.editedTypeId();
            });

        if (type != null) {
            var validRequesters = type.ValidRequesters ? type.ValidRequesters : $.map(self.requesters(), function (u) { return u.Name });
            self.selectedRequesterNamesForEditedType(validRequesters);
        }

        return type;
    });
    
    self.pages = ko.observableArray([
        {
            Name: 'Admin',
            Enabled: ko.pureComputed(function() { return self.isAdmin() })
        },
        {
            Name: 'Rejestracja',
            Enabled: ko.pureComputed(function() { return true; })
        },
        {
            Name: 'Logowanie',
            Enabled: ko.pureComputed(function() { return true; })
        },
        {
            Name: 'Tworzenie',
            Enabled: ko.pureComputed(function() { return self.isRequester() })
        },
        {
            Name: 'Zlecenia',
            Enabled: ko.pureComputed(function() { return true; })
        },
        {
            Name: 'Status',
            Enabled: ko.pureComputed(function() { return self.isServiceProvider() || self.isAdmin(); })
        },
        {
            Name: 'SzczegolyTypu',
            Enabled: ko.pureComputed(function() { return false; })
        },
        {
            Name: 'Statystyki',
            Enabled: ko.pureComputed(function () { return false; })
        }
    ]);
	
    var tokenKey = 'accessToken';

    self.user = ko.observable();
    self.userRoles = ko.observableArray();
    self.requestTypesICanCreate = ko.pureComputed(function() {
        return ko.utils.arrayFilter(self.requestTypes(), function (r) {
            return r.ValidRequesters === null || r.ValidRequesters.indexOf(self.user()) > -1;
        });
    });

    self.registerUsername = ko.observable();
    self.registerPassword = ko.observable();
    self.registerPassword2 = ko.observable();

    self.loginUsername = ko.observable();
    self.loginPassword = ko.observable();

    self.newRequestTypeInput = ko.observable();
    self.newSpecialRequestTypeInput = ko.observable();
    
	self.disableButton = function (element) {
	    $(element).removeClass("btn-info btn-success btn-danger").addClass("btn-default").text("Czekaj").prop('disabled', true);
	};

	self.enableButton = function(element, text, css) {
        $(element).addClass(css).removeClass("btn-default").text(text).prop('disabled', false);
	};

    self.hideAllPages = function() {
        self.registerVisible(false);
        self.loginVisible(false);
        self.requestsVisible(false);
        self.administrationVisible(false);
        self.createRequestsVisible(false);
        self.availabilityVisible(false);
        self.typeDetailsVisible(false);
    };

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
    };

    self.gotoLogin = function() {
        location.hash = 'Logowanie';
    };

    self.gotoRequests = function () {
        location.hash = 'Zlecenia';
    };

    self.gotoCreateRequests = function() {
        location.hash = 'Tworzenie';
    };
    
    self.updateActiveRequests = function(data) {
        var sortedData = data.sort(function (left, right) {
            if (isEmpty(left.ProvidingUserName) && isEmpty(right.ProvidingUserName)) {
                return Date.parse(left.CreationDate) - Date.parse(right.CreationDate);
            } else if (!isEmpty(left.ProvidingUserName) && !isEmpty(right.ProvidingUserName)) {
                return Date.parse(left.CreationDate) - Date.parse(right.CreationDate);
            }
            else {
                return isEmpty(left.ProvidingUserName) ? -1 : 1;
            }
        });
        self.activeRequests(sortedData);
        self.updateTasks();
    };

    self.updateRequestTypes = function(data) {
        var normalTypes = data.filter(function (el) { return !el.ForSelf });
        var specialTypes = data.filter(function (el) { return el.ForSelf });

        self.requestTypes(normalTypes);
        self.specialRequestTypes(specialTypes);
    };

    self.updateTasks = function()
    {
        var perUserDict = {};

        self.usersAndRoles().forEach(function (user) {
            if (user.IsServiceProvider()) {
                perUserDict[user.Name] = { Name: user.Name, Tasks: [], SpecialTasks: [] };
            }
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
    };
    
    self.getUsersAndRoles = function () {
        self.getData(siteRoot + '/api/Account/UsersAndRoles/', self.updateUsersAndRoles);
    };

    self.updateUsersAndRoles = function(data) {
        var mappedUsers = $.map(data, function(item) { return new UserWithRoles(item) });
        self.usersAndRoles(mappedUsers);
        self.updateTasks();
    };

    self.saveRoles = function (userAndRoles, event) {
        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        var originalText = event.target.textContent;
        self.disableButton(event.target);

        $.ajax({
            type: 'POST',
            url: siteRoot + '/api/Account/UserAndRoles',
            headers: headers,
            data: ko.toJSON(userAndRoles),
            contentType: "application/json"
        }).done(function() {
            self.getUserInfo();
        }).fail(function(jx) {
            showError(jx);
        }).always(function() {
            self.enableButton(event.target, originalText, "btn-info");
        });
    };

    self.updateRequestType = function (user, event) {

        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        var originalText = event.target.textContent;
        self.disableButton(event.target);

        var requestType = self.editedType();

        if (self.requesters().length === self.selectedRequesterNamesForEditedType().length) {
            requestType.ValidRequesters = null;
        } else {
            requestType.ValidRequesters = self.selectedRequesterNamesForEditedType();
        }

        $.ajax({
            type: 'PUT',
            url: siteRoot + '/api/DispatchRequestTypes/' + requestType.Id,
            headers: headers,
            data: ko.toJSON(requestType),
            contentType: "application/json"
        }).done(function () {
        }).fail(function (jx) {
            showError(jx);
        }).always(function () {
            self.enableButton(event.target, originalText, "btn-info");
        });
    };
    
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
	 };


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
            self.enableButton(event.target, originalText, "btn-success");
            showError(jx);
        });
    };

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
            self.enableButton(event.target, originalText, "btn-danger");
            showError(jx);
        });
    };

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
            self.enableButton(event.target, originalText, "btn-danger");
            showError(jx);
        });
    };
    
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
            self.enableButton(event.target, originalText, "btn-info");
        });
    };

    self.createRequest = function (requestType) {
        self.getData(siteRoot + '/api/CreateRequest/' + requestType.Id);
    };

    self.createSpecialRequest = function(requestType) {
        self.getData(siteRoot + '/api/CreateSpecialRequest/' + requestType.Id);
    };

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
            self.newSpecialRequestTypeInput(null);
            self.enableButton(form[1], originalText, "btn-info");
        });
    };

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
            self.newRequestTypeInput(null);
            self.enableButton(form[1], originalText, "btn-info");
        });
    };
    
    self.computeStats = function () {
        self.getData(siteRoot + '/api/Stats/')
    };

    self.showErrors = function(data) {
        showError(data);
    };

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
    };

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
    };

    self.displayDeleteRequestTypeConfirmation = function(requestType, event) {
        var dialogDiv = $("#dialog-confirm");
        dialogDiv.find("p").text("Jesteś pewien, że chcesz usunąć typ zlecenia '" + requestType.Name + "'?\n\nSpowoduje to również usunięcie wszystkich obecnych i historycznych zleceń tego typu.");
        dialogDiv.dialog({
            resizable: false,
            height: 290,
            width: 330,
            modal: true,
            title: "Na pewno?",
            buttons: {
                "Usuń": function () {
                    $(this).dialog("close");
                    self.deleteRequestType(requestType, event);
                },
                "Anuluj": function () {
                    $(this).dialog("close");
                }
            }
        });
    };

    self.deleteRequestType = function(requestType, event) {
        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        var originalText = event.target.textContent;
        self.disableButton(event.target);

        $.ajax({
            type: 'DELETE',
            url: siteRoot + '/api/DispatchRequestTypes/' + requestType.Id,
            headers: headers
        }).fail(function (jx) {
            self.enableButton(event.target, originalText, "btn-danger");
            showError(jx);
        });
    }
    
    self.displayDeleteUserConfirmation = function (usersAndRoles,  event){
        var dialogDiv = $( "#dialog-confirm");
        dialogDiv.find("p").text("Jesteś pewien, że chcesz usunąć użytkownika '" + usersAndRoles.Name + "'?\n");
        dialogDiv.dialog({
            resizable: false,
            height:200,
            width: 300,
            modal: true,
            title: "Na pewno?",
            buttons: {
                "Usuń": function() {
                    $(this).dialog( "close" );
                    self.deleteUser(usersAndRoles, event);
                },
                "Anuluj": function() {
                    $(this).dialog( "close" );
                }
            }
        });
    };

    self.deleteUser = function (userAndRoles, event) {
        var token = localStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        var originalText = event.target.textContent;
        self.disableButton(event.target);

        $.ajax({
            type: 'DELETE',
            url: siteRoot + '/api/Account/User/' + userAndRoles.Name,
            headers: headers
        }).fail(function(jx) {
            self.enableButton(event.target, originalText, "btn-danger");
            showError(jx);
        });
    };

    self.clearForms = function () {
        self.registerUsername('');
        self.registerPassword('');
        self.registerPassword2('');
        self.loginUsername('');
        self.loginPassword('');
    };

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
    };

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
    };

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
    
    self.requestsPending = ko.pureComputed(function () {
        return self.activeRequests()
            .filter(function (el) {
                return isEmpty(el.ProvidingUserName);
            })
            .sort(function (left, right) {
                return Date.parse(left.CreationDate) - Date.parse(right.CreationDate);
            });
    });
    
    self.placeInQueue = function (request) {
        var index = self.requestsPending().indexOf(request);
        return index;
    };

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
    };

    self.specialRequestTypeActiveForMe = function(id) {
        var temp = self.requestsAssignedToMe().filter(function(el) {
            return el.TypeId === id;
        });
        return temp.length > 0;
    };
    
    self.logout = function () {
        self.clearUserInfo();
        localStorage.removeItem(tokenKey);
        self.gotoLogin();
    };

    // Initialize the navigation
    Sammy(function () {
        this.get('#Statystyki', function () {
           self.currentPage('Statystyki');
           self.hideAllPages();
           //self.statsVisible(true);
           //self.computeStats();
        });
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
            self.currentPage('Zlecenia');
            self.hideAllPages();
            self.requestsVisible(true);
        });
        this.get('#Tworzenie', function () {
            self.currentPage('Tworzenie');
            self.hideAllPages();
            self.createRequestsVisible(true);
        });
        this.get('#Admin', function () {
            self.currentPage('Admin');
            self.hideAllPages();
            self.administrationVisible(true);
        });
        this.get('#SzczegolyTypu/:typeId', function () {
            self.editedTypeId(parseInt(this.params['typeId']));
            self.currentPage('SzczegolyTypu');
            self.hideAllPages();
            self.typeDetailsVisible(true);
            window.scrollTo(0, 0);
        });
        this._checkFormSubmission = function() {
            return false;
        }
    }).run();

    // Fetch the initial data.
    self.getUserInfo(function () {
        self.gotoDefault();
    });
    self.getUsersAndRoles();
};

function UserWithRoles(data) {
    this.Name = data.Name;
    this.IsAdmin = ko.observable(data.Roles.indexOf('Admin') > -1);
    this.IsServiceProvider = ko.observable(data.Roles.indexOf('ObslugaZlecen') > -1);
    this.IsRequester = ko.observable(data.Roles.indexOf('TworzenieZlecen') > -1);
}

function RequestTypeWithValidUsers(requestType) {
    this.Name = requestType.Name;
    this.ForSelf = requestType.ForSelf;
}

var viewModel = new ViewModel();
ko.applyBindings(viewModel);

var requestsHub = $.connection.requestsHub;
requestsHub.client.updateActiveRequests = function (data) {
    viewModel.updateActiveRequests(data);
};
requestsHub.client.updateUsersAndRoles = function(data) {
    viewModel.updateUsersAndRoles(data);
};
requestsHub.client.updateRequestTypes = function(data) {
    viewModel.updateRequestTypes(data);
};

// Start the connection.
$.connection.hub.start().done(initialize);

$.connection.hub.reconnecting(function () {
    viewModel.connectionError("Utracono połączenie z serwerem");
});

$.connection.hub.reconnected(initialize);

// Reconnect after disconnect
$.connection.hub.disconnected(function () {
    setTimeout(function () {
        $.connection.hub.start().done(initialize);
    }, 5000); // Restart connection after 5 seconds.
});

function initialize() {
    viewModel.connectionError(null);

    requestsHub.server.getActiveRequests()
        .done(function(result) {
            viewModel.updateActiveRequests(result);
        }).fail(function(data) {
            viewModel.showErrors(data);
        });

    requestsHub.server.getRequestTypes()
        .done(function (result) {
            viewModel.updateRequestTypes(result);
        }).fail(function (data) {
            viewModel.showErrors(data);
        });
}


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

function getPendingText(request) {
    var position = viewModel.placeInQueue(request) + 1;
    if (position > 0) {
        return "Oczekuje na miejscu " + position + " w kolejce";
    }
    
    return "";
}

function isEmpty(str) {
    return (!str || 0 === str.length);
}

