define('auth',
    ['jquery'],
    function ($) {
    
        var setupAjaxCalls = function () {
            var tokenKey = 'accessToken';
            var token = localStorage.getItem(tokenKey);
            var headers = {};
            if (token) {
                headers.Authorization = 'Bearer ' + token;
            }
            
            $.ajaxSetup({
                headers: headers
            });
        };
        
        var updateToken = function (accessToken) {
            localStorage.setItem(tokenKey, accessToken);
            setupAjaxCalls();
        };

        return {
            updateToken: updateToken,
            setupAjaxAuthorization: setupAjaxCalls
        };
});