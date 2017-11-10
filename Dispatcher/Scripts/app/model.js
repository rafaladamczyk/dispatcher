define('model',
    [
        'model.user',
        'model.request',
        'model.requestType',
        'model.machine'
    ],
    function (user, request, requestType, machine) {
        var
            model = {
                User: user,
                Request: request,
                RequestType: requestType,
                Machine: machine
            };

        model.setDataContext = function (dc) {
            // Model's that have navigation properties 
            // need a reference to the datacontext.
            //model.User.datacontext(dc);
            model.Request.datacontext(dc);
            // model.RequestType.datacontext(dc);
            //model.Machine.datacontext(dc);
        };

        return model;
    });