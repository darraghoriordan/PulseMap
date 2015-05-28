/// <reference path="typings/google.maps.d.ts" />
/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/signalr/signalr.d.ts" />
var PulseApp = (function () {
    function PulseApp() {
        var _this = this;
        this.wellington = new google.maps.LatLng(-41.28, 174.77);
        this.markers = new Array();
        this.lines = new Array();
        this.animationTimeout = 1500; //milliseconds to delete animations
        this.pulseSocketConnection = $.hubConnection();
        this.pulseSocketConnection.logging = true;
        this.pulseSocketHub = this.pulseSocketConnection.createHubProxy('pulseSocketHub');
        this.pulseSocketHub.on('updateStandaloneEvents', function (events) {
            events.forEach(function (event) {
                _this.addMarker(new google.maps.LatLng(event.Latitude, event.Longitude), 1);
            });
        });
        this.pulseSocketHub.on('updateInteractionEvents', function (events) {
            events.forEach(function (event) {
                _this.addInteraction(new google.maps.LatLng(event.StartLatitude, event.StartLongitude), new google.maps.LatLng(event.EndLatitude, event.EndLongitude));
            });
        });
    }
    PulseApp.prototype.createMap = function () {
        var _this = this;
        var mapOptions = {
            center: this.wellington,
            zoom: 6,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        this.map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
        this.map.set('styles', [
            {
                "stylers": [
                    { "visibility": 'simplified' }
                ]
            },
            {
                "stylers": [
                    { "visibility": 'simplified' },
                    { "hue": '#007fff' },
                    { "saturation": 62 },
                    { "gamma": 0.35 }
                ]
            },
            {
                "featureType": 'water',
                "stylers": [
                    { "lightness": 4 },
                    { "color": '#1b1a3e' }
                ]
            },
            {
                "elementType": 'labels.text',
                "stylers": [
                    { "visibility": 'off' }
                ]
            }
        ]);
        // This event listener will call addMarker() when the map is clicked.
        google.maps.event.addListener(this.map, 'click', function (event) {
            _this.addMarker(event.latLng, 1);
        });
        return this.map;
    };
    PulseApp.prototype.addInteraction = function (startLocation, endLocation) {
        this.addMarker(startLocation, 3);
        this.addLineAnimation(startLocation, endLocation);
        this.addMarker(endLocation, 3);
    };
    PulseApp.prototype.addLineAnimation = function (startLocation, endLocation) {
        var path = new google.maps.Polyline({
            path: [startLocation, endLocation],
            geodesic: true,
            strokeColor: '#C4DF9B',
            strokeOpacity: 0.8,
            strokeWeight: 2
        });
        this.lines.push(path);
        path.setMap(this.map);
        setTimeout(function () {
            path.setMap(null);
            delete path;
        }, this.animationTimeout);
    };
    // Add a marker to the map and push to the array.
    PulseApp.prototype.addMarker = function (location, color) {
        var classString = 'mapPointPulse ';
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
            labelClass: classString,
            labelAnchor: new google.maps.Point(15, 15)
        });
        marker.setMap(this.map);
        this.markers.push(marker);
        setTimeout(function () {
            marker.setMap(null);
            delete marker;
        }, this.animationTimeout);
        // return marker;
    };
    PulseApp.prototype.startEventsService = function () {
        this.pulseSocketConnection.start();
    };
    return PulseApp;
})();
$(document).ready(function () {
    var mapHelper = new PulseApp();
    mapHelper.createMap();
    mapHelper.startEventsService();
});
//# sourceMappingURL=app.js.map