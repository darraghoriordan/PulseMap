var StatModel = (function () {
    function StatModel(jsonObject) {
        this.StartStat = jsonObject.StartStat;
        this.OccuredOn = moment(jsonObject.OccuredOn);
    }
    return StatModel;
}());
var StatCounter = (function () {
    function StatCounter() {
        this.nextUpdateDue = moment('2015-10-15');
        this.totalDealerGms = 0;
        this.totalDealerElement = $('#totalDealerGmsStat .statsValueText');
        this.totalDealerGmsStats = new Array();
        this.numberFormat = new Intl.NumberFormat('en-NZ', {
            style: 'currency',
            currency: 'NZD',
            minimumFractionDigits: 0
        });
    }
    StatCounter.prototype.startEventsService = function () {
        var _this = this;
        setInterval(function () { _this.updateEvents(); }, 500);
    };
    StatCounter.prototype.getNewEvents = function (startDate, endDate) {
        var currentInstance = this;
        var edString = endDate.toISOString();
        var sdString = startDate.toISOString();
        var startOfDay = moment().startOf("day").toISOString();
        $.get("/api/events/newdealergms", { startDate: sdString, endDate: edString }, function (cdata) {
            if (cdata)
                currentInstance.totalDealerGmsStats = $.map(cdata, function (x) {
                    return new StatModel(x);
                });
        });
        $.get("/api/statistics/totaldealergms", { startDate: startOfDay, endDate: sdString }, function (cdata) {
            if (cdata)
                currentInstance.totalDealerGms = cdata;
        });
    };
    StatCounter.prototype.updateEvents = function () {
        this.setTime();
        var offsetTime = moment(this.currentTime).subtract(5, "m");
        if (this.currentTime.isSameOrAfter(this.nextUpdateDue)) {
            this.getNewEvents(offsetTime, this.currentTime);
            // set when last update occured
            this.nextUpdateDue = moment(this.currentTime).add(5, "m");
        }
        for (var i = 0; i < this.totalDealerGmsStats.length; i++) {
            var event_1 = this.totalDealerGmsStats[i];
            var offset = offsetTime.toISOString();
            var occ = event_1.OccuredOn.toISOString();
            if (offsetTime.isSameOrAfter(event_1.OccuredOn)) {
                this.totalDealerGms += event_1.StartStat;
                this.totalDealerGmsStats.splice(i, 1);
            }
        }
        if (this.totalDealerGms > 0) {
            this.totalDealerElement.text(this.numberFormat.format(this.totalDealerGms));
        }
    };
    StatCounter.prototype.setTime = function () {
        this.currentTime = moment();
        // this.timeElement.text(this.currentTime.format('HH:mm:ss'));
    };
    return StatCounter;
}());
//# sourceMappingURL=StatCounter.js.map