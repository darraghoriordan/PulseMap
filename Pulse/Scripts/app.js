/// <reference path="typings/google.maps.d.ts" />
/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/signalr/signalr.d.ts" />
var PulseApp = (function () {
    function PulseApp() {
        var _this = this;
        this.wellington = new google.maps.LatLng(-41.28, 174.77);
        this.markers = new Array();
        this.pulseSocketConnection = $.hubConnection();
        this.pulseSocketConnection.logging = true;
        this.pulseSocketHub = this.pulseSocketConnection.createHubProxy('pulseSocketHub');
        this.pulseSocketHub.on('updateStandaloneEvents', function (events) {
            events.forEach(function (event) {
                _this.addMarker(new google.maps.LatLng(event.Latitude, event.Longitude));
            });
        });
        $.extend(this.pulseSocketHub.client, {
            updateStandaloneEvents: function (events) {
                var _this = this;
                events.forEach(function (event) {
                    _this.addMarker(new google.maps.LatLng(event.Latitude, event.Longitude));
                });
            }
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
            _this.addMarker(event.latLng);
        });
        return this.map;
    };
    // Add a marker to the map and push to the array.
    PulseApp.prototype.addMarker = function (location) {
        var classString = 'mapPointPulse ';
        var rand = Math.floor((Math.random() * 5) + 1);
        switch (rand) {
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
            map: this.map,
            clickable: false,
            icon: {
                path: google.maps.SymbolPath.CIRCLE,
                scale: 0
            },
            labelClass: classString
        });
        this.markers.push(marker);
        setTimeout(function () {
            marker.setMap(null);
            delete marker;
        }, 3000);
        return marker;
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