function () {
    if (jQuery && jQuery.fn && jQuery.fn.select2 && jQuery.fn.select2.amd) {
        var n = jQuery.fn.select2.amd;
    }
    n.define("select2/i18n/en", [], function () {
        return {
            errorLoading: function () {
                return "Unable to load results.";
            },
            inputTooLong: function (n) {
                return "Too long. Please delete " + (n.input.length - n.maximum) + " characters.";
            },
            inputTooShort: function (n) {
                return "Too short. Please enter " + (n.minimum - n.input.length) + " more characters.";
            },
            loadingMore: function () {
                return "Loading more...";
            },
            maximumSelected: function (n) {
                return "You can only select up to " + n.maximum + " items.";
            },
            noResults: function () {
                return "No results found.";
            },
            searching: function () {
                return "Searching...";
            },
            removeAllItems: function () {
                return "Remove all items.";
            }
        };
    }),
        n.define, n.require
} ();
