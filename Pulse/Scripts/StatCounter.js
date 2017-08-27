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
        this.localPlaybackOffset = 5;
        this.numberFormat = new Intl.NumberFormat('en-NZ', {
            style: 'currency',
            currency: 'NZD',
            minimumFractionDigits: 0
        });
        this.updateRate = 0;
        this.frameLengthMilliseconds = 50;
    }
    StatCounter.prototype.startEventsService = function () {
        var _this = this;
        setInterval(function () { _this.updateEvents(); }, this.frameLengthMilliseconds);
    };
    StatCounter.prototype.getNewEvents = function (startDate, endDate) {
        var currentInstance = this;
        var sdString = startDate.toISOString();
        var edString = endDate.toISOString();
        var startOfDay = moment().startOf("day").toISOString();
        $.get("/api/events/newdealergms", { startDate: sdString, endDate: edString }, function (cdata) {
            if (cdata)
                currentInstance.totalDealerGmsStats = $.map(cdata, function (x) {
                    return new StatModel(x);
                });
            //calculate the rate of increase per frame/loop
            if (currentInstance.totalDealerGmsStats.length > 0) {
                var maxAmount = currentInstance.totalDealerGmsStats[currentInstance.totalDealerGmsStats.length - 1].StartStat;
                var minAmount = currentInstance.totalDealerGmsStats[0].StartStat;
                var deltaAmount = maxAmount - minAmount;
                var numberOfFrames = currentInstance.localPlaybackOffset * 60 * (1000 / currentInstance.frameLengthMilliseconds);
                currentInstance.updateRate = Math.round(deltaAmount / numberOfFrames);
            }
        });
        $.get("/api/statistics/totaldealergms", { startDate: startOfDay, endDate: sdString }, function (cdata) {
            if (cdata)
                currentInstance.totalDealerGms = cdata;
        });
    };
    StatCounter.prototype.updateEvents = function () {
        this.currentTime = moment();
        var offsetTime = moment().subtract(this.localPlaybackOffset, "m");
        //dy/dx
        this.totalDealerGms += this.updateRate;
        if (this.currentTime.isSameOrAfter(this.nextUpdateDue)) {
            this.getNewEvents(offsetTime, this.currentTime);
            // set when last update occured
            this.nextUpdateDue = moment(this.currentTime).add(this.localPlaybackOffset, "m");
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
    return StatCounter;
}());
//# sourceMappingURL=StatCounter.js.map