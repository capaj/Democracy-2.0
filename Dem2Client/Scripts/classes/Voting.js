﻿define(["./VotableItem"], function (VotableItem) {
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

        return ko.observable(r);
    };
});

