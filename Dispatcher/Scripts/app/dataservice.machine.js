define('dataservice.machine',
    ['amplify', 'utils'],
    function (amplify, utils) {
        var
            init = function () {
                amplify.request.define('machine', 'ajax', {
                    url: utils.url('/api/Machines/{id}'),
                    dataType: 'json',
                    type: 'GET'
                });

                amplify.request.define('machines', 'ajax', {
                    url: utils.url('/api/Machines'),
                    dataType: 'json',
                    type: 'GET'
                });
            },

            getMachine = function (callbacks) {
                return amplify.request({
                    resourceId: 'machine',
                    success: callbacks.success,
                    error: callbacks.error
                });
            },

            getMachines = function (callbacks) {
                return amplify.request({
                    resourceId: 'machines',
                    success: callbacks.success,
                    error: callbacks.error
                });
            };

        init();

        return {
            getMachine: getMachine,
            getMachines: getMachines
        };
    });