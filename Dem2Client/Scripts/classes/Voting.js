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
        r.commentTree = ko.observableArray([]);
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
                var aVote = r.thisClientVote();
                if (aVote) {
                    if (aVote.Agrees()) {
                        return "Hlasovali jste pro návrh v " + aVote.castedTime();
                    }
                    else{
                        return "Změnit hlas na ano";
                    }
                }
                return "Ano";
            }
        });
        r.noText = ko.defferedComputed({
            read: function () {
                var aVote = r.thisClientVote();
                if (aVote) {
                    if (aVote.Agrees()) {
                        return "Změnit hlas na ne";
                    }else{
                        return "Hlasovali jste proti návrhu v " + aVote.castedTime(); 
                    }
                }
                return "Ne";
            }
        });

        return ko.observable(r);
    };
});

