var ViewModel = function () {
    var self = this;
    self.activeRequests = ko.observableArray();
    self.error = ko.observable();

    var requestsUri = '/api/ActiveRequests/';
    var completeRequestUri = '/api/CompleteRequest/';
    var createRequestUri = '/api/CreateRequest/';

    self.getActiveRequests = function() {
        $.getJSON(requestsUri, function(data) {
            self.activeRequests(data);
        });
    }

    self.completeRequest = function(request) {
        $.ajax(completeRequestUri + request.Id, {
            type: "PUT", contentType: "application/json",
            success: self.getActiveRequests,
            error: function (jx, message, error) {alert(message);}
        });
    }

    self.createRequest = function() {
        var requesterId = Math.floor((Math.random() * 3) + 1);
        var requestType = Math.floor((Math.random() * 2));
        $.getJSON(createRequestUri + requesterId + '/' + requestType, self.getActiveRequests);
    }

    // Fetch the initial data.

    self.getActiveRequests();
};

var viewModel = new ViewModel();
window.setInterval(viewModel.getActiveRequests, 1000);
ko.applyBindings(viewModel);