define('dataservice',
    [
        'dataservice.user',
        'dataservice.machine',
        'dataservice.request',
        'dataservice.requestType'
    ],
    function (user, machine, request, requestType) {
        return {
            user: user,
            machine: machine,
            request: request,
            requestType: requestType
        };
    });