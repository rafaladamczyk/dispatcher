define('dataservice.request',
    ['amplify', 'utils'],
    function (amplify, utils) {
        var
            init = function () {
                amplify.request.define('user', 'ajax', {
                    url: utils.url('/api/Account/User'),
                    dataType: 'json',
                    type: 'GET'
                });

                amplify.request.define('users', 'ajax', {
                    url: utils.url('/api/Account/Users'),
                    dataType: 'json',
                    type: 'GET'
                });

                amplify.request.define('userUpdate', 'ajax', {
                    url: utils.url('/api/Account/Users'),
                    dataType: 'json',
                    type: 'PUT',
                    contentType: 'application/json; charset=utf-8'
                });

                amplify.request.define('userDelete', 'ajax', {
                    url: utils.url('/api/Account/Users/{id}'),
                    dataType: 'json',
                    type: 'DELETE'
                });
            },

            getCurrentUser = function (callbacks) {
                return amplify.request({
                    resourceId: 'user',
                    success: callbacks.success,
                    error: callbacks.error
                });
            },

            getUsers = function (callbacks) {
                return amplify.request({
                    resourceId: 'users',
                    success: callbacks.success,
                    error: callbacks.error
                });
            },

            updateUser = function (callbacks, data) {
                return amplify.request({
                    resourceId: 'userUpdate',
                    data: data,
                    success: callbacks.success,
                    error: callbacks.error
                });
            },

            deleteUser = function (callbacks, id) {
                return amplify.request({
                    resourceId: 'userDelete',
                    data: { id: id },
                    success: callbacks.success,
                    error: callbacks.error
                });
            };

        init();

        return {
            getCurrentUser: getCurrentUser,
            getUsers: getUsers,
            updateUser: updateUser,
            deleteUser: deleteUser
        };
    });