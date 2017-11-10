define('model.requestType',
    ['ko', 'moment'],
    function (ko, moment) {
        var RequestType = function () {
            var self = this;
            self.id = ko.observable();
            self.name = ko.observable();
            self.ForSelf = ko.observable();

            self.isNullo = false;
            return self;
        };

        RequestType.Nullo = new RequestType().id(-1).name("empty request type").ForSelf(false);
        RequestType.Nullo.isNullo = true;

        return RequestType;
    });
