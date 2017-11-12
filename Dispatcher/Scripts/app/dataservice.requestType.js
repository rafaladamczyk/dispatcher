define('dataservice.requestType',
    ['amplify', 'utils'],
    function (amplify, utils) {
        var
            init = function () {
                amplify.request.define('type', 'ajax', {
                    url: utils.url('/api/RequestTypes/{id}'),
                    dataType: 'json',
                    type: 'GET'
                });

                amplify.request.define('types', 'ajax', {
                    url: utils.url('/api/RequestTypes'),
                    dataType: 'json',
                    type: 'GET'
                });

                amplify.request.define('updateType', 'ajax', {
                    url: utils.url('/api/RequestTypes'),
                    dataType: 'json',
                    type: 'PUT',
                    contentType: 'application/json; charset=utf-8'
                });

                amplify.request.define('deleteType', 'ajax', {
                    url: utils.url('/api/RequestTypes/{id}'),
                    dataType: 'json',
                    type: 'DELETE'
                });
            },

            getType = function (callbacks, id) {
                return amplify.request({
                    resourceId: 'type',
                    data: {id: id},
                    success: callbacks.success,
                    error: callbacks.error
                });
            },

            getTypes = function (callbacks) {
                return amplify.request({
                    resourceId: 'types',
                    success: callbacks.success,
                    error: callbacks.error
                });
            },

            updateType = function (callbacks, data) {
                return amplify.request({
                    resourceId: 'updateType',
                    data: data,
                    success: callbacks.success,
                    error: callbacks.error
                });
            },

            deleteType = function (callbacks, id) {
                return amplify.request({
                    resourceId: 'deleteType',
                    data: { id: id },
                    success: callbacks.success,
                    error: callbacks.error
                });
            };

        init();

        return {
            getType: getType,
            getTypes: getTypes,
            updateType: updateType,
            deleteType: deleteType
        };
    });