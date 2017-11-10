define('model.request',
    ['ko', 'moment'],
    function (ko, moment) {
    var _dc = null;
        var Request = function () {
            var self = this;
            self.id = ko.observable();
            self.creatorId = ko.observable();
            self.providerId = ko.observable();
            self.typeId = ko.observable();
            self.createdDate = ko.observable();
            self.pickedUpDate = ko.observable();
            self.completedDate = ko.observable();

            self.createdAgo = ko.computed(function () {
                return self.createdDate() ? moment.utc(self.createdDate()).local().fromNow() : ''
            });
            self.pickedUpAgo = ko.computed(function () {
                return self.pickedUpDate() ? moment.utc(self.pickedUpDate()).local().fromNow() : ''
            });
            self.completedAgo = ko.computed(function () {
                return self.completedDate() ? moment.utc(self.completedDate()).local().fromNow() : ''
            });
            
            self.isNullo = false;
            return self;
        };

        Request.Nullo = new Request().id(0).creatorId(0).providerId(0).typeId(0)
            .createdDate(new Date(1985, 1, 1, 1, 0, 0, 0))
            .pickedUpDate(new Date(1986, 1, 1, 1, 0, 0, 0))
            .completedDate(new Date(1987, 1, 1, 1, 0, 0, 0));
        Request.Nullo.isNullo = true;

        Request.datacontext = function (dc) {
            if(dc) { _dc = dc; }
            return _dc;
        };

        // Prototype is available to all instances.
        // It has access to the properties of the instance of Request.
        Request.prototype = function () {
            var dc = Request.datacontext;
            var requestType = function () {
                return dc().requestTypes.getLocalById(this.typeId);
            };
            var creator = function () {
                return dc().users.getLocalById(this.creatorId);
            };
            var provider = function () {
                return dc().users.getLocalById(this.providerId);
            };
            
            return {
                isNullo: false,
                requestType: requestType,
                creator: creator,
                provider: provider
            };
        }();

        return Request;
    });
