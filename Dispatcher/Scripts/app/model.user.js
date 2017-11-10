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
                self.userName = ko.observable().extend({ required: true });
                self.gender = ko.observable();
                self.roles = ko.observableArray();

                self.speakerSessions = ko.computed({
                    read: function () {
                        return self.id() ? User.datacontext().users.getLocalSpeakerSessions(self.id()) : [];
                    },

                    // Delay the eval til the data needed for the computed is ready
                    deferEvaluation: true
                });

                self.isNullo = false;

                self.dirtyFlag = new ko.DirtyFlag([
                    self.userName,
                    self.roles]);
                return self;
            };

        User.Nullo = new User()
            .id(0)
            .userName('Nullo User')
            .gender('M')
            .roles([]);
        User.Nullo.isNullo = true;
        User.Nullo.dirtyFlag().reset();

        return User;
    });