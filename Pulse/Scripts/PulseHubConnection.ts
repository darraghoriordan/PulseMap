
class PulseHubConnection {
    currentTime = new Date();
    pulseSocketConnection: any
    pulseSocketHub: any
    pulseMap: PulseMap
    constructor() {
        this.pulseMap = new PulseMap();
        this.configureSocketHub();
        setInterval(() => { this.setTime(); }, 1000);
    }
    startEventsService() {
        this.pulseSocketConnection.start();
    }
    createMap() {
        this.pulseMap.createMap();
    }
    configureSocketHub() {
        this.pulseSocketConnection = $.hubConnection();
        this.pulseSocketConnection.logging = false;
        this.pulseSocketHub = this.pulseSocketConnection.createHubProxy('pulseSocketHub');
        this.pulseSocketHub.on('updateStandaloneEvents', (events: any[]) => {
            this.pulseMap.clearUsedLines();
            this.pulseMap.clearUsedMarkers();
            events.forEach((event) => {
                this.pulseMap.addMarker(new google.maps.LatLng(event.Latitude, event.Longitude), 1, 'normal');
            });
        });
        this.pulseSocketHub.on('updateInteractionEvents', (events: any[]) => {
            events.forEach((event) => {
                this.pulseMap.addInteraction(new google.maps.LatLng(event.StartLatitude, event.StartLongitude), new google.maps.LatLng(event.EndLatitude, event.EndLongitude));
            });

        });
        this.pulseSocketHub.on('updateCommentEvents', (events: any[]) => {
            events.forEach((event) => {
                this.pulseMap.addComment(new google.maps.LatLng(event.StartLatitude, event.StartLongitude), new google.maps.LatLng(event.EndLatitude, event.EndLongitude));
            });

        });
        this.pulseSocketHub.on('updateNewListingsStat', (stat: string) => {
            $('#itemsListedStat .statsValueText').text(stat);
        });
        this.pulseSocketHub.on('updateSoldTodayStat', (stat: string) => {
            $('#itemsSoldStat .statsValueText').text(stat);
        });
    }

    checkTime(i) {
        return (i < 10) ? '0' + i : i;
    }

    setTime() {
        this.currentTime = new Date();
        var h = this.checkTime(this.currentTime.getHours());
        var m = this.checkTime(this.currentTime.getMinutes());
        var s = this.checkTime(this.currentTime.getSeconds());
        $('#timeStat .statsValueText').text(h + ':' + m + ':' + s);

    }


}
interface SignalR {
    tradeMeEventBroadcaster: any
}

// ReSharper disable once InconsistentNaming
declare function MarkerWithLabel(opts: any): void;