var StandAloneEvent = (function () {
    function StandAloneEvent(jsonObject) {
        this.OccuredOn = jsonObject.OccuredOn;
        this.Latitude = jsonObject.Latitude;
        this.Longitude = jsonObject.Longitude;
        this.Region = jsonObject.Region;
        this.Suburb = jsonObject.Suburb;
        this.CategoryId = jsonObject.CategoryId;
    }
    return StandAloneEvent;
}());
var InteractionEvent = (function () {
    function InteractionEvent(jsonData) {
        this.StartRegion = jsonData.StartRegion;
        this.StartSuburb = jsonData.StartSuburb;
        this.EndRegion = jsonData.EndRegion;
        this.EndSuburb = jsonData.EndSuburb;
        this.CategoryId = jsonData.CategoryId;
        this.OccuredOn = jsonData.OccuredOn;
        this.StartLatitude = jsonData.StartLatitude;
        this.StartLongitude = jsonData.StartLongitude;
        this.EndLatitude = jsonData.EndLatitude;
        this.EndLongitude = jsonData.EndLongitude;
    }
    return InteractionEvent;
}());
var PulseApiConnection = (function () {
    function PulseApiConnection() {
        var _this = this;
        this.newListings = 0;
        this.soldListings = 0;
        this.pulseMap = new PulseMap();
        this.currentTime = moment();
        this.nextUpdateDue = this.currentTime; //set this to now for the first run
        this.standAloneEvents = new Array();
        this.commentEvents = new Array();
        this.interactionEvents = new Array();
        this.newListings = 0;
        this.soldListings = 0;
        setInterval(function () { _this.setTime(); }, 200);
    }
    PulseApiConnection.prototype.startEventsService = function () {
        var _this = this;
        setInterval(function () { _this.updateEvents(); }, 200);
    };
    PulseApiConnection.prototype.createMap = function () {
        this.pulseMap.createMap();
    };
    PulseApiConnection.prototype.getNewEvents = function () {
        var currentInstance = this;
        $.get("/api/events/standalone", function (data) {
            if (data)
                currentInstance.standAloneEvents = $.map(data, function (x) {
                    return new StandAloneEvent(x);
                });
        });
        $.get("/api/events/interaction", function (idata) {
            if (idata)
                currentInstance.interactionEvents = $.map(idata, function (x) {
                    return new InteractionEvent(x);
                });
        });
        $.get("/api/events/comments", function (cdata) {
            if (cdata)
                currentInstance.commentEvents = $.map(cdata, function (x) {
                    return new InteractionEvent(x);
                });
        });
        $.get("/api/statistics/newlistings", function (cdata) {
            if (cdata)
                currentInstance.newListings = cdata;
        });
        $.get("/api/statistics/soldListings", function (cdata) {
            if (cdata)
                currentInstance.soldListings = cdata;
        });
    };
    PulseApiConnection.prototype.updateEvents = function () {
        //do we need new data?
        if (this.currentTime.isSameOrAfter(this.nextUpdateDue)) {
            this.getNewEvents();
            // set when last update occured
            this.nextUpdateDue = moment().add(5, 'minutes');
        }
        // this.pulseMap.clearUsedLines();
        //  this.pulseMap.clearUsedMarkers();
        // this is kind of broken in that we are dependent on getting 5 mins from api too. It works ok but only cause we control
        // both ends. Would be better to have the api query configurable to match the offsets.
        var offsetTime = this.currentTime.subtract(5, 'minutes');
        for (var i = 0; i < this.standAloneEvents.length; i++) {
            var event_1 = this.standAloneEvents[i];
            if (offsetTime.isSameOrAfter(event_1.OccuredOn)) {
                this.pulseMap.addMarker(new google.maps.LatLng(event_1.Latitude, event_1.Longitude), 1, 'normal');
                this.newListings++;
                this.standAloneEvents.splice(i, 1);
            }
        }
        for (var i = 0; i < this.interactionEvents.length; i++) {
            var ievent = this.interactionEvents[i];
            if (offsetTime.isSameOrAfter(ievent.OccuredOn)) {
                this.pulseMap.addInteraction(new google.maps.LatLng(ievent.StartLatitude, ievent.StartLongitude), new google.maps.LatLng(ievent.EndLatitude, ievent.EndLongitude));
                this.soldListings++;
                this.interactionEvents.splice(i, 1);
            }
        }
        for (var i = 0; i < this.commentEvents.length; i++) {
            var event_2 = this.commentEvents[i];
            if (offsetTime.isSameOrAfter(event_2.OccuredOn)) {
                this.pulseMap.addComment(new google.maps.LatLng(event_2.StartLatitude, event_2.StartLongitude), new google.maps.LatLng(event_2.EndLatitude, event_2.EndLongitude));
                this.commentEvents.splice(i, 1);
            }
        }
        $('#itemsListedStat .statsValueText').text(this.newListings);
        $('#itemsSoldStat .statsValueText').text(this.soldListings);
    };
    PulseApiConnection.prototype.setTime = function () {
        this.currentTime = moment();
        $('#timeStat .statsValueText').text(this.currentTime.format('HH:mm:ss'));
    };
    return PulseApiConnection;
}());
//# sourceMappingURL=PulseApiConnection.js.map