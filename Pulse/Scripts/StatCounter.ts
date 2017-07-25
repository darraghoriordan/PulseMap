﻿class StatModel {
    constructor(jsonObject: any) {
        this.StartStat = jsonObject.StartStat;
        this.OccuredOn = moment(jsonObject.OccuredOn);
    }

    public StartStat: number;
    public OccuredOn: moment.Moment;
}

class StatCounter {
    totalDealerGmsStats: Array<StatModel>;
    totalDealerElement: JQuery;
    totalDealerGms: number;
    currentTime: moment.Moment;
    nextUpdateDue: moment.Moment;
    numberFormat: Intl.NumberFormat;
    localPlaybackOffset:number;
    constructor() {
        this.nextUpdateDue = moment('2015-10-15');
        this.totalDealerGms = 0;
        this.totalDealerElement = $('#totalDealerGmsStat .statsValueText');
        this.totalDealerGmsStats = new Array<StatModel>();
        this.localPlaybackOffset = 5;
        this.numberFormat = new Intl.NumberFormat('en-NZ', {
            style: 'currency',
            currency: 'NZD',
            minimumFractionDigits: 0
        });
    }

    startEventsService() {
        setInterval(() => { this.updateEvents(); }, 500);
    }

    getNewEvents(startDate: moment.Moment, endDate: moment.Moment) {
        var currentInstance = this;
        let sdString = startDate.toISOString();
        let edString = endDate.toISOString();
        var startOfDay = moment().startOf("day").toISOString();
        $.get(
            "/api/events/newdealergms", { startDate: sdString, endDate: edString },
            function (cdata) {

                if (cdata)
                    currentInstance.totalDealerGmsStats = $.map(cdata, (x) => {
                        return new StatModel(x);
                    });
            }
        );

        $.get(
            "/api/statistics/totaldealergms", { startDate: startOfDay, endDate: sdString },
            function (cdata) {
                if (cdata)
                    currentInstance.totalDealerGms = cdata;
            }
        );
    }

    updateEvents() {

        this.currentTime = moment();
        let offsetTime = moment().subtract(this.localPlaybackOffset, "m");

        if (this.currentTime.isSameOrAfter(this.nextUpdateDue)) {
            this.getNewEvents(offsetTime, this.currentTime);
            // set when last update occured
            this.nextUpdateDue = moment(this.currentTime).add(this.localPlaybackOffset, "m");
        }

        for (var i = 0; i < this.totalDealerGmsStats.length; i++) {
            let event = this.totalDealerGmsStats[i];
            let offset = offsetTime.toISOString();
            let occ = event.OccuredOn.toISOString();
            if (offsetTime.isSameOrAfter(event.OccuredOn)) {
                this.totalDealerGms += event.StartStat;
                this.totalDealerGmsStats.splice(i, 1);
            }
        }
        if (this.totalDealerGms > 0) {
            this.totalDealerElement.text(this.numberFormat.format(this.totalDealerGms));
        }
    }
}