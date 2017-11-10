define('binder',
    ['jquery', 'ko', 'config', 'vm'],
    function ($, ko, config, vm) {
        var
            ids = config.viewIds,

            bind = function () {
                ko.applyBindings(vm.shell, getView(ids.shellTop));
                ko.applyBindings(vm.acount, getView(ids.account));
                ko.applyBindings(vm.admin, getView(ids.admin));
                ko.applyBindings(vm.requests, getView(ids.requests));
                ko.applyBindings(vm.machines, getView(ids.machines));
            },

            getView = function (viewName) {
                return $(viewName).get(0);
            };

        return {
            bind: bind
        };
    });