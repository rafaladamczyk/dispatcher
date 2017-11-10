define('model.machine',
    ['ko', 'moment'],
    function (ko, moment) {
        var Machine = function () {
            var self = this;
            self.id = ko.observable();
            self.name = ko.observable();
            
            self.isNullo = false;
            return self;
        };

        Machine.Nullo = new Machine().id(-1).name("empty machine");
        Machine.Nullo.isNullo = true;

        return Machine;
    });
