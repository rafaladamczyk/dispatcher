define('dataprimer',
    ['ko', 'datacontext', 'config'],
    function (ko, datacontext, config) {

        var logger = config.logger,

            fetch = function () {

                return $.Deferred(function (def) {

                    var data = {
                        users: ko.observableArray(),
                        machines: ko.observableArray(),
                        requests: ko.observableArray(),
                        requestTypes: ko.observableArray()
                    };

                    $.when(
                        datacontext.users.getCurrent(),
                        datacontext.users.getData({results: data.users}),
                        datacontext.machines.getData({results: data.machines}),
                        datacontext.requests.getData({results: data.requests}),
                        datacontext.requestTypes.getData({param: config.currentUserId, results: data.requestTypes})
                    )
                        .pipe(function () {
                            logger.success('Fetched data for: '
                                + '<div>' + data.users().length + ' users </div>'
                                + '<div>' + data.machines().length + ' machines </div>'
                                + '<div>' + data.requests().length + ' requests </div>'
                                + '<div>' + data.requestTypes().length + ' requestTypes </div>'
                            );
                        })
                        .fail(function () {
                            def.reject();
                        })
                        .done(function () {
                            def.resolve();
                        });

                }).promise();
            };

        return {
            fetch: fetch
        };
    });