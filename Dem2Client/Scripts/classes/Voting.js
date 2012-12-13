define(["./VotableItem"], function (VotableItem) {
    ko.defferedComputed = function (object) {
        object.deferEvaluation = true;
        return ko.computed(object);
    };

    return function (ent) {
        var r = VotableItem(ent)();
        r.scrapedPrint = ko.observable(ent.scrapedPrint);
        r.State = ko.observable(ent.State);
        r.votingEndDate = ko.observable(ent.votingEndDate);

        r.typeOfPrintMapped = ko.computed(function () {
            var types = {
                1: "novela zákona",
                2: "Mezinárodní smlouva",
                3: "Výroční zpráva"
            }

            var thisType = r.scrapedPrint().type;
            if (types.hasOwnProperty(thisType)) {
                return types[thisType];
            }else {
                return "neznámý typ dokumentu";
            }
        });

        r.yesText = ko.defferedComputed({
            read: function () {
                //if (vote.agrees() == true){
                //    return "Hlasovali jste pro návrh v " + new Date();
                //}
                //if (vote.agrees() == false){
                //    return "Změnit hlas na ano";
                //}
                return "Ano";
            }
        });
        r.noText = ko.defferedComputed({
            read: function () {
                //if (vote.agrees() == false){
                //    return "Hlasovali jste proti návrhu v " + new Date();
                //}
                //if (vote.agrees() == true){
                //    return "Změnit hlas na ne";
                //}
                return "Ne";
            }
        });

        return ko.observable(r);
    };
});

