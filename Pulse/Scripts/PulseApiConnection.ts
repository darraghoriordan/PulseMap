/// <reference path="PulseMap.ts" />
class StandAloneEvent {
    constructor(jsonObject: any) {
        this.OccuredOn = moment(jsonObject.OccuredOn);
        this.Latitude = jsonObject.Latitude;
        this.Longitude = jsonObject.Longitude;
        this.Region = jsonObject.Region;
        this.Suburb = jsonObject.Suburb;
        this.CategoryId = jsonObject.CategoryId;
    }
    public OccuredOn: moment.Moment
    public Latitude: number
    public Longitude: number
    public Region: string
    public Suburb: string
    public CategoryId: number
}

class InteractionEvent {
    constructor(jsonData: any) {
        this.StartRegion = jsonData.StartRegion;
        this.StartSuburb = jsonData.StartSuburb;
        this.EndRegion = jsonData.EndRegion;
        this.EndSuburb = jsonData.EndSuburb;
        this.CategoryId = jsonData.CategoryId;
        this.OccuredOn = moment(jsonData.OccuredOn);
        this.StartLatitude = jsonData.StartLatitude;
        this.StartLongitude = jsonData.StartLongitude;
        this.EndLatitude = jsonData.EndLatitude;
        this.EndLongitude = jsonData.EndLongitude;
    }

    public StartRegion: string
    public StartSuburb: string
    public EndRegion: string
    public EndSuburb: string
    public CategoryId: number
    public OccuredOn: moment.Moment
    public StartLatitude: number
    public StartLongitude: number
    public EndLatitude: number
    public EndLongitude: number
}

class PulseApiConnection {
    currentTime: moment.Moment
    standAloneEvents: StandAloneEvent[];
    interactionEvents: InteractionEvent[];
    commentEvents: InteractionEvent[];
    nextUpdateDue: moment.Moment
    pulseMap: PulseMap
    newListings: number;
    soldListings: number;
    constructor() {
        this.pulseMap = new PulseMap();
        this.standAloneEvents = new Array<StandAloneEvent>();
        this.commentEvents = new Array<InteractionEvent>();
        this.interactionEvents = new Array<InteractionEvent>();
        this.newListings = 0;
        this.soldListings = 0;
        this.nextUpdateDue = moment('2015-10-15')
    }
    startEventsService() {
        setInterval(() => { this.updateEvents(); }, 200);
    }

    createMap() {
        this.pulseMap.createMap();
    }
    getNewEvents(startDate: moment.Moment, endDate: moment.Moment) {
        var currentInstance = this;
        let sdString = startDate.toISOString();
        let edString = endDate.toISOString();

        $.get(
            "/api/events/standalone", {startDate: sdString, endDate: edString},
            function (data) {
                if (data)
                    currentInstance.standAloneEvents = $.map(data, (x) => {
                        return new StandAloneEvent(x);
                    });
            }
        );
        $.get(
            "/api/events/interaction", { startDate: sdString, endDate: edString },
            function (idata) {
                if (idata)
                    currentInstance.interactionEvents = $.map(idata, (x) => {
                        return new InteractionEvent(x);
                    });
            }
        );
        $.get(
            "/api/events/comments", { startDate: sdString, endDate: edString },
            function (cdata) {

                if (cdata)
                    currentInstance.commentEvents = $.map(cdata, (x) => {
                        return new InteractionEvent(x);
                    });
            }
        );
        $.get(
            "/api/statistics/newlistings", { startDate: sdString, endDate: edString },
            function (cdata) {
                if (cdata)
                    currentInstance.newListings = cdata;
            }
        );
        $.get(
            "/api/statistics/soldListings", { startDate: sdString, endDate: edString },
            function (cdata) {
                if (cdata)
                    currentInstance.soldListings = cdata;
            }
        );

    }

    updateEvents() {
        this.setTime();
        let offsetTime = moment(this.currentTime).subtract(5, "m");
        let c = this.currentTime.toISOString();
        let nu = this.nextUpdateDue.toISOString();
        let ot = offsetTime.toISOString();
        //do we need new data?
        if (this.currentTime.isSameOrAfter(this.nextUpdateDue)) {
            this.getNewEvents(offsetTime, this.currentTime);            
            // set when last update occured
            this.nextUpdateDue = moment(this.currentTime).add(5, "m");
        }

        // this.pulseMap.clearUsedLines();
        //  this.pulseMap.clearUsedMarkers();
        // this is kind of broken in that we are dependent on getting 5 mins from api too. It works ok but only cause we control
        // both ends. Would be better to have the api query configurable to match the offsets.

        for (var i = 0; i < this.standAloneEvents.length; i++) {
            let event = this.standAloneEvents[i];
            if (offsetTime.isSameOrAfter(event.OccuredOn)) {
                    this.pulseMap.addMarker(new google.maps.LatLng(event.Latitude, event.Longitude), 1, 'normal');                
                this.newListings++;
                this.standAloneEvents.splice(i, 1);
            }
        }

        for (var i = 0; i < this.interactionEvents.length; i++) {
            let ievent = this.interactionEvents[i];
            if (offsetTime.isSameOrAfter(ievent.OccuredOn)) {
                this.pulseMap.addInteraction(new google.maps.LatLng(ievent.StartLatitude, ievent.StartLongitude), new google.maps.LatLng(ievent.EndLatitude, ievent.EndLongitude));
                this.soldListings++;
                this.interactionEvents.splice(i, 1);
            }
        }
        for (var i = 0; i < this.commentEvents.length; i++) {
            let event = this.commentEvents[i];
            if (offsetTime.isSameOrAfter(event.OccuredOn)) {
                this.pulseMap.addComment(new google.maps.LatLng(event.StartLatitude, event.StartLongitude), new google.maps.LatLng(event.EndLatitude, event.EndLongitude));
                this.commentEvents.splice(i, 1);
            }
        }

        $('#itemsListedStat .statsValueText').text(this.newListings);
        $('#itemsSoldStat .statsValueText').text(this.soldListings);
    }

    setTime() {
        this.currentTime = moment();
        $('#timeStat .statsValueText').text(this.currentTime.format('HH:mm:ss'));

    }
}
