define('config',
    ['toastr', 'infuser', 'ko'],
    function (toastr, infuser, ko) {

        var
            // properties
            //-----------------

            currentUserId = -1;
            currentUser = ko.observable(),
            hashes = {
                favorites: '#/favorites',
                favoritesByDate: '#/favorites/date',
                sessions: '#/sessions',
                speakers: '#/speakers'
            },
            logger = toastr, // use toastr for the logger
            messages = {
                viewModelActivated: 'viewmodel-activation'
            },
            stateKeys = {
                lastView: 'state.active-hash'
            },
                
            throttle = 400,
            title = 'CodeCamper > ',
            toastrTimeout = 2000,

            viewIds = {
                admin: '#admin-view',
                account: '#account-view',
                machines: '#machines-view',
                requests: '#requests-view',
                shellTop: '#shellTop-view'
            },

            toasts = {
                changesPending: 'Please save or cancel your changes before leaving the page.',
                errorSavingData: 'Data could not be saved. Please check the logs.',
                errorGettingData: 'Could not retrieve data.  Please check the logs.',
                invalidRoute: 'Cannot navigate. Invalid route',
                retreivedData: 'Data retrieved successfully',
                savedData: 'Data saved successfully'
            },

            // methods
            //-----------------

            validationInit = function () {
                ko.validation.configure({
                    registerExtenders: true,    //default is true
                    messagesOnModified: true,   //default is true
                    insertMessages: true,       //default is true
                    parseInputAttributes: true, //default is false
                    writeInputAttributes: true, //default is false
                    messageTemplate: null,      //default is null
                    decorateElement: true       //default is false. Applies the .validationElement CSS class
                });
            },

            configureExternalTemplates = function () {
                infuser.defaults.templatePrefix = "_";
                infuser.defaults.templateSuffix = ".tmpl.html";
                infuser.defaults.templateUrl = "/Tmpl";
            },

            init = function () {
                toastr.options.timeOut = toastrTimeout;
                configureExternalTemplates();
                validationInit();
            };

        init();

        return {
            currentUserId: currentUserId,
            currentUser: currentUser,
            hashes: hashes,
            logger: logger,
            messages: messages,
            stateKeys: stateKeys,
            throttle: throttle,
            title: title,
            toasts: toasts,
            viewIds: viewIds,
            window: window
        };
    });
