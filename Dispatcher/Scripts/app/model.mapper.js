define('model.mapper',
    ['model'],
    function (model) {
        var
            user = {
                getDtoId: function(dto) { return dto.Id; },
                fromDto: function(dto, item) {
                    item = item || new model.User().id(dto.Id);
                    item.name(dto.Name).roles(dto.Roles);
                    return item;
                }
            },

            request = {
                getDtoId: function(dto) { return dto.Id; },
                fromDto: function(dto, item) {
                    item = item || new model.Request().id(dto.Id);
                    item.creatorId(dto.Creator_Id)
                        .providerId(dto.Provider_Id)
                        .typeId(dto.RequestType_Id)
                        .createdDate(dto.CreationDate)
                        .pickedUpDate(dto.PickedUpDate)
                        .completedDate(dto.CompletionDate)
                        .active(dto.Active);
                    return item;
                }
            },

            requestType = {
                getDtoId: function(dto) { return dto.Id; },
                fromDto: function(dto, item) {
                    item = item || new model.RequestType().id(dto.Id);
                    item.name(dto.Name)
                        .forSelf(dto.ForSelf);
                    return item;
                }
            },
            
            machine = {
            getDtoId: function (dto) { return dto.Id; },
                fromDto: function (dto, item) {
                    item = item || new model.Machine().id(dto.Id);
                    item.name(dto.Name);
                    return item;
                }
            };

        return {
            user: user,
            request: request,
            requestType: requestType,
            machine: machine
        };
    });