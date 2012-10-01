require(["Scripts/facebook"], function (FB) {
    FB.deffered.then(function(value) {
        console.log("User's facebook acces token is " + value);
    });
    
});