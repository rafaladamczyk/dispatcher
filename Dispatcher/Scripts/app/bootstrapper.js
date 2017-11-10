define('bootstrapper',
    ['jquery', 'moment', 'route-config', 'presenter', 'dataprimer', 'binder', 'auth'],
    function ($, moment, routeConfig, presenter, dataprimer, binder, auth) {
        var
            run = function () {
                presenter.toggleActivity(true);
                auth.setupAjaxAuthorization();
                moment.locale('pl');

                $.when(dataprimer.fetch())
                    .done(binder.bind)
                    .done(routeConfig.register)
                    .always(function () {
                        presenter.toggleActivity(false);
                    });
            };

        return {
            run: run
        };
    });