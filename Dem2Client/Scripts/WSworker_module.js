define(function () {
    var worker = new Worker('Scripts/wsworker.js');   //worker handling server comunication
    return worker;
});