var PulseHubConnection = (function () {
    function PulseHubConnection() {
        var _this = this;
        this.currentTime = new Date();
        this.pulseMap = new PulseMap();
        this.configureSocketHub();
        setInterval(function () { _this.setTime(); }, 1000);
    }
    PulseHubConnection.prototype.startEventsService = function () {
        this.pulseSocketConnection.start();
    };
    PulseHubConnection.prototype.createMap = function () {
        this.pulseMap.createMap();
    };
    PulseHubConnection.prototype.configureSocketHub = function () {
        var _this = this;
        this.pulseSocketConnection = $.hubConnection();
        this.pulseSocketConnection.logging = false;
        this.pulseSocketHub = this.pulseSocketConnection.createHubProxy('pulseSocketHub');
        this.pulseSocketHub.on('updateStandaloneEvents', function (events) {
            _this.pulseMap.clearUsedLines();
            _this.pulseMap.clearUsedMarkers();
            events.forEach(function (event) {
                _this.pulseMap.addMarker(new google.maps.LatLng(event.Latitude, event.Longitude), 1, 'normal');
            });
        });
        this.pulseSocketHub.on('updateInteractionEvents', function (events) {
            events.forEach(function (event) {
                _this.pulseMap.addInteraction(new google.maps.LatLng(event.StartLatitude, event.StartLongitude), new google.maps.LatLng(event.EndLatitude, event.EndLongitude));
            });
        });
        this.pulseSocketHub.on('updateCommentEvents', function (events) {
            events.forEach(function (event) {
                _this.pulseMap.addComment(new google.maps.LatLng(event.StartLatitude, event.StartLongitude), new google.maps.LatLng(event.EndLatitude, event.EndLongitude));
            });
        });
        this.pulseSocketHub.on('updateNewListingsStat', function (stat) {
            $('#itemsListedStat .statsValueText').text(stat);
        });
        this.pulseSocketHub.on('updateSoldTodayStat', function (stat) {
            $('#itemsSoldStat .statsValueText').text(stat);
        });
    };
    PulseHubConnection.prototype.checkTime = function (i) {
        return (i < 10) ? '0' + i : i;
    };
    PulseHubConnection.prototype.setTime = function () {
        this.currentTime = new Date();
        var h = this.checkTime(this.currentTime.getHours());
        var m = this.checkTime(this.currentTime.getMinutes());
        var s = this.checkTime(this.currentTime.getSeconds());
        $('#timeStat .statsValueText').text(h + ':' + m + ':' + s);
    };
    return PulseHubConnection;
}());
//# sourceMappingURL=PulseHubConnection.js.map