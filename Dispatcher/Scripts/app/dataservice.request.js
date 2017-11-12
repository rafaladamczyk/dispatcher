define('dataservice.request',
    ['amplify', 'utils'],
    function (amplify, utils) {
        var
            init = function () {
                amplify.request.define('request', 'ajax', {
                    url: utils.url('/api/Request'),
                    dataType: 'json',
                    type: 'GET'
                });

                amplify.request.define('requests', 'ajax', {
                    url: utils.url('/api/ActiveRequests'),
                    dataType: 'json',
                    type: 'GET'
                });

                amplify.request.define('updateRequest', 'ajax', {
                    url: utils.url('/api/Request'),
                    dataType: 'json',
                    type: 'PUT',
                    contentType: 'application/json; charset=utf-8'
                });

                amplify.request.define('deleteRequest', 'ajax', {
                    url: utils.url('/api/Request/{id}'),
                    dataType: 'json',
                    type: 'DELETE'
                });

                amplify.request.define('acceptRequest', 'ajax', {
                    url: utils.url('/api/Request/Accept'),
                    dataType: 'json',
                    type: 'PUT',
                    contentType: 'application/json; charset=utf-8'
                });

                amplify.request.define('cancelRequest', 'ajax', {
                    url: utils.url('/api/Request/Cancel'),
                    dataType: 'json',
                    type: 'PUT',
                    contentType: 'application/json; charset=utf-8'
                });

                amplify.request.define('completeRequest', 'ajax', {
                    url: utils.url('/api/Request/Complete'),
                    dataType: 'json',
                    type: 'PUT',
                    contentType: 'application/json; charset=utf-8'
                });
            },

            getRequest = function (callbacks) {
                return amplify.request({
                    resourceId: 'request',
                    success: callbacks.success,
                    error: callbacks.error
                });
            },

            getRequests = function (callbacks) {
                return amplify.request({
                    resourceId: 'requests',
                    success: callbacks.success,
                    error: callbacks.error
                });
            },

            updateRequest = function (callbacks, data) {
                return amplify.request({
                    resourceId: 'updateRequest',
                    data: data,
                    success: callbacks.success,
                    error: callbacks.error
                });
            },

            deleteRequest = function (callbacks, id) {
                return amplify.request({
                    resourceId: 'deleteRequest',
                    data: { id: id },
                    success: callbacks.success,
                    error: callbacks.error
                });
            },
            
            acceptRequest = function (callbacks, data) {
                return amplify.request({
                    resourceId: 'acceptRequest',
                    data: data,
                    success: callbacks.success,
                    error: callbacks.error
                });
            },
            
            cancelRequest = function (callbacks, data) {
                return amplify.request({
                    resourceId: 'cancelRequest',
                    data: data, 
                    success: callbacks.success, 
                    error: callbacks.error
                });
            },
            
            completeRequest = function (callbacks, data) {
                return amplify.request({
                    resourceId: 'completeRequest',
                    data: data,
                    success: callbacks.success, 
                    error: callbacks.error
                })
            };      

        init();

        return {
            getRequest: getRequest,
            getRequests: getRequests,
            updateRequest: updateRequest,
            deleteRequest: deleteRequest,
            acceptRequest: acceptRequest,
            cancelRequest: cancelRequest,
            completeRequest: completeRequest
        };
    });