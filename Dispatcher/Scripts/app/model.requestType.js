define('model.requestType',
    ['ko'],
    function (ko) {
        var RequestType = function () {
            var self = this;
            self.id = ko.observable();
            self.name = ko.observable();
            self.forSelf = ko.observable();

            self.isNullo = false;
            return self;
        };

        RequestType.Nullo = new RequestType().id(-1).name("empty request type").forSelf(false);
        RequestType.Nullo.isNullo = true;

        return RequestType;
    });
