var ViewModel = function () {
    var self = this;
    self.activeRequests = ko.observableArray();
    self.error = ko.observable();

    var requestsUri = '/api/ActiveRequests/';

    function ajaxHelper(uri, method, data) {
        self.error(''); // Clear error message
        return $.ajax({
            type: method,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null
        }).fail(function (jqXHR, textStatus, errorThrown) {
            self.error(errorThrown);
        });
    }

    function getActiveRequests() {
        ajaxHelper(requestsUri, 'GET').done(function (data) {
            self.activeRequests(data);
        });
    }

    // Fetch the initial data.

    getActiveRequests();
};

ko.applyBindings(new ViewModel());