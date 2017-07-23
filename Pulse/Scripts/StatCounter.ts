class StatModel {
    constructor(jsonObject: any) {
        this.StartStat = jsonObject.StartStat;
        this.EndStat = jsonObject.EndStat;
    }
   
    public StartStat: number
    public EndStat: number
}

class StatCounter {
    totalDealerGms: StatModel;
    totalDealerElement: JQuery;
    currentTime: moment.Moment;
    nextUpdateDue: moment.Moment;
    numberFormat: Intl.NumberFormat;
    constructor() {
        this.nextUpdateDue = moment('2015-10-15');
      
        this.totalDealerElement = $('#totalDealerGmsStat .statsValueText');


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
            function (data) {
                if (data) {
                    currentInstance.totalDealerGms = new StatModel(data);
                }
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

        this.totalDealerElement.text(this.numberFormat.format( this.getCurrentGms(this.currentTime, this.nextUpdateDue, this.totalDealerGms)));
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