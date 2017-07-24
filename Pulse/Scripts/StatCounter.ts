class StatModel {
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
    constructor() {
        this.nextUpdateDue = moment('2015-10-15');
        this.totalDealerGms = 0;
        this.totalDealerElement = $('#totalDealerGmsStat .statsValueText');
        this.totalDealerGmsStats = new Array<StatModel>();

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
        let edString = endDate.toISOString();
        let sdString = startDate.toISOString();
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
        this.setTime();
        let offsetTime = moment(this.currentTime).subtract(5, "m");

        if (this.currentTime.isSameOrAfter(this.nextUpdateDue)) {
            this.getNewEvents(offsetTime, this.currentTime);
            // set when last update occured
            this.nextUpdateDue = moment(this.currentTime).add(5, "m");
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

    setTime() {
        this.currentTime = moment();
        // this.timeElement.text(this.currentTime.format('HH:mm:ss'));

    }
}