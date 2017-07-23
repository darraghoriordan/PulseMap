class StatModel {
    constructor(jsonObject: any) {
        this.StartStat = jsonObject.StartStat;
        this.OccuredOn = moment(jsonObject.OccuredOn);
    }
   
    public StartStat: number
    public OccuredOn: moment.Moment
}

class StatCounter {
    totalDealerGmsStats: Array<StatModel>;
    totalDealerElement: JQuery;
    currentTime: moment.Moment;
    nextUpdateDue: moment.Moment;
    numberFormat: Intl.NumberFormat;
    constructor() {
        this.nextUpdateDue = moment('2015-10-15');
      
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

    getNewEvents(startDate: moment.Moment,endDate: moment.Moment) {
        var currentInstance = this;
        let edString = endDate.toISOString();
        let sdString = startDate.toISOString();

        $.get(
            "/api/statistics/totaldealergms", { startDate: sdString, endDate: edString },
            function (cdata) {

                if (cdata)
                    currentInstance.totalDealerGmsStats = $.map(cdata, (x) => {
                        return new StatModel(x);
                    });
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
                this.totalDealerElement.text(this.numberFormat.format(event.StartStat));
                this.totalDealerGmsStats.splice(i, 1);
            }
        }
       // this.totalDealerElement.text(this.numberFormat.format(this.getCurrentGms(this.currentTime, this.nextUpdateDue, this.totalDealerGms)));


    }
    getCurrentGms(startDate: moment.Moment , endDate:moment.Moment, stat:StatModel):number {
        let timeDifference = endDate.diff(startDate, "s")
        return timeDifference;
    }
    setTime() {
        this.currentTime = moment();
        // this.timeElement.text(this.currentTime.format('HH:mm:ss'));

    }
}