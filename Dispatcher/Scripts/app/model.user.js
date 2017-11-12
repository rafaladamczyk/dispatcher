define('model.user',
    ['ko'],
    function (ko) {
        var
            settings = {
                imageBasePath: '../content/images/photos/',
                unknownPersonImageSource: 'unknown_person.jpg',
                twitterUrl: 'http://twitter.com/',
                twitterRegEx: /[@]([A-Za-z0-9_]{1,15})/i,
                urlRegEx: /\b((?:[a-z][\w-]+:(?:\/{1,3}|[a-z0-9%])|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}\/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'".,<>?«»“”‘’]))/i
            },

            User = function () {
                var self = this;
                self.id = ko.observable();
                self.name = ko.observable().extend({ required: true });
                self.gender = ko.observable();
                self.roles = ko.observableArray();
                self.isNullo = false;
                return self;
            };

        User.Nullo = new User()
            .id(0)
            .name('Nullo User')
            .gender('M')
            .roles([]);
        User.Nullo.isNullo = true;

        return User;
    });