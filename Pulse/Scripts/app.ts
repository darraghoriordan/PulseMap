/// <reference path="typings/google.maps.d.ts" />
/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/signalr/signalr.d.ts" />

class PulseApp {
    wellington = new google.maps.LatLng(-41.28, 174.77);
    map: google.maps.Map;
    markers: google.maps.Marker[] = new Array<google.maps.Marker>();
    lines:google.maps.Polyline[] = new Array<google.maps.Polyline>();
    pulseSocketConnection: any;
    pulseSocketHub:any;


    constructor() {
        this.pulseSocketConnection = $.hubConnection();
        this.pulseSocketConnection.logging = true;
        this.pulseSocketHub = this.pulseSocketConnection.createHubProxy('pulseSocketHub');
        //this.pulseSocketHub.on('updateStandaloneEvents',(events: any[]) => {
        //    events.forEach((event) => {
        //        this.addMarker(new google.maps.LatLng(event.Latitude, event.Longitude),1);
        //    });
        //});
        this.pulseSocketHub.on('updateInteractionEvents',(events: any[]) => {
            events.forEach((event) => {
                this.addInteraction(new google.maps.LatLng(event.StartLatitude, event.StartLongitude), new google.maps.LatLng(event.EndLatitude, event.EndLongitude));
            });
        });
    }


    createMap():google.maps.Map {
        var mapOptions: google.maps.MapOptions = {
            center: this.wellington,
            zoom: 6,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        this.map = new google.maps.Map(document.getElementById('map-canvas'),
            mapOptions);

        this.map.set('styles', [
           {
               "stylers": [
                   { "visibility": 'simplified' }
               ]
           }, {
               "stylers": [
                   { "visibility": 'simplified' },
                   { "hue": '#007fff' },
                   { "saturation": 62 },
                   { "gamma": 0.35 }
               ]
           }, {
               "featureType": 'water',
               "stylers": [
                   { "lightness": 4 },
                   { "color": '#1b1a3e' }
               ]
           }, {
               "elementType": 'labels.text',
               "stylers": [
                   { "visibility": 'off' }
               ]
           }
        ]);

        // This event listener will call addMarker() when the map is clicked.
        google.maps.event.addListener(this.map, 'click', event => {
            this.addMarker(event.latLng,1);
    });

    return this.map;
}
    addInteraction(startLocation, endLocation) {
        this.addMarker(startLocation, 3);
        this.addLineAnimation(startLocation, endLocation);
       this.addMarker(endLocation, 3);
    }
    addLineAnimation(startLocation, endLocation) {
        var path = new google.maps.Polyline({
            path: [startLocation,endLocation],
            geodesic: true,
            strokeColor: '#FFFFFF',
            strokeOpacity: 0.8,
            strokeWeight: 2
        });
        this.lines.push(path);
        path.setMap(this.map);

        setTimeout(() => {
            path.setMap(null);
            delete path;
        }, 3000);
    }
    // Add a marker to the map and push to the array.
    addMarker(location, color:number) {
        var classString = 'mapPointPulse ';
       // var rand = Math.floor((Math.random() * 5) + 1);
        switch (color) {
            case 1:
                classString = classString + 'pink';
                break;
            case 2:
                classString = classString + 'red';
                break;
            case 3:
                classString = classString + 'green';
                break;
            case 4:
                classString = classString + 'blue';
                break;
            case 5:
                classString = classString + 'yellow';
                break;

            default:
           }

        var marker = new MarkerWithLabel({
            position: location,
         
            clickable: false,
            icon: {
                path: google.maps.SymbolPath.CIRCLE,
                scale: 0
            },
            labelClass: classString
        });
        marker.setMap(this.map);
    this.markers.push(marker);

    setTimeout(() => {
        marker.setMap(null);
        delete marker;
        }, 3000);

       // return marker;

    }

    startEventsService() {
        this.pulseSocketConnection.start();
    }
}
interface SignalR {
    tradeMeEventBroadcaster: any
}

// ReSharper disable once InconsistentNaming
declare function MarkerWithLabel(opts: any): void;

    $(document).ready(() => {
        var mapHelper = new PulseApp();
        mapHelper.createMap();
        mapHelper.startEventsService();
});