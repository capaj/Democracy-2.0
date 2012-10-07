; ;
define(function () {
    ((function (d) {
        var js;
        var id = 'facebook-jssdk';
        var ref = d.getElementsByTagName('script')[0];

        if(d.getElementById(id)) {
            return;
        }
        js = d.createElement('script');
        js.id = id;
        js.async = true;
        js.src = "//connect.facebook.net/cs_CZ/all.js";
        ref.parentNode.insertBefore(js, ref);
    })(document));
    var aToken;
    var dfd = new jQuery.Deferred();
    window.fbAsyncInit = function () {
        FB.init({
            appId: '152010184924545',
            status: true,
            cookie: true,
            xfbml: true
        });
        FB.Event.subscribe('auth.statusChange', function (response) {
            if(response.authResponse) {
                aToken = response.authResponse.accessToken;
                dfd.resolve(aToken);
                FB.api('/me', function (me) {
                    if(me.name) {
                        document.getElementById('auth-displayname').innerHTML = me.name;
                    }
                });
                document.getElementById('auth-loggedout').style.display = 'none';
                document.getElementById('auth-loggedin').style.display = 'block';
            } else {
                document.getElementById('auth-loggedout').style.display = 'block';
                document.getElementById('auth-loggedin').style.display = 'none';
            }
        });
        document.getElementById('auth-loginlink').addEventListener('click', function () {
            FB.login();
        });
        document.getElementById('auth-logoutlink').addEventListener('click', function () {
            FB.logout();
        });
    };
    return {
        deffered: dfd.promise()
    };
});
