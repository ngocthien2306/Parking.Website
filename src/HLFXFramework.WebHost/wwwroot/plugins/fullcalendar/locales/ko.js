(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? module.exports = factory() :
        typeof define === 'function' && define.amd ? define(factory) :
            (global = global || self, (global.FullCalendarLocales = global.FullCalendarLocales || {}, global.FullCalendarLocales.ko = factory()));
}(this, function () {
    'use strict';

    var ko = {
        code: "VN",
        buttonText: {
            prev: "Previous",
            next: "Next",
            today: "Today",
            month: "Month",
            week: "Week",
            day: "Day",
            list: "List"
        },
        weekLabel: "Week",
        allDayText: "All day",
        eventLimitText: "more",
        noEventsMessage: "No events"
    };

    return ko;

}));
