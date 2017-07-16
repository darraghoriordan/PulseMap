/// <reference path="typings/google.maps.d.ts" />
/// <reference path="typings/jquery/jquery.d.ts" />
var PulseApp = (function () {
    function PulseApp() {
        var _this = this;
        this.wellington = new google.maps.LatLng(-41.28, 174.77);
        this.markers = new Array();
        this.lines = new Array();
        this.animationTimeout = 1500; //milliseconds to delete animations
        this.currentTime = new Date();
        this.lineSymbol = {
            path: google.maps.SymbolPath.CIRCLE,
            scale: 4,
            fillOpacity: 1.0,
            strokeColor: '#fff'
        };
        this.pulseSocketConnection = $.hubConnection();
        this.pulseSocketConnection.logging = false;
        this.pulseSocketHub = this.pulseSocketConnection.createHubProxy('pulseSocketHub');
        this.pulseSocketHub.on('updateStandaloneEvents', function (events) {
            _this.clearUsedLines();
            _this.clearUsedMarkers();
            events.forEach(function (event) {
                _this.addMarker(new google.maps.LatLng(event.Latitude, event.Longitude), 1, 'normal');
            });
        });
        this.pulseSocketHub.on('updateInteractionEvents', function (events) {
            events.forEach(function (event) {
                _this.addInteraction(new google.maps.LatLng(event.StartLatitude, event.StartLongitude), new google.maps.LatLng(event.EndLatitude, event.EndLongitude));
            });
        });
        this.pulseSocketHub.on('updateCommentEvents', function (events) {
            events.forEach(function (event) {
                _this.addComment(new google.maps.LatLng(event.StartLatitude, event.StartLongitude), new google.maps.LatLng(event.EndLatitude, event.EndLongitude));
            });
        });
        this.pulseSocketHub.on('updateNewListingsStat', function (stat) {
            $('#itemsListedStat .statsValueText').text(stat);
        });
        this.pulseSocketHub.on('updateSoldTodayStat', function (stat) {
            $('#itemsSoldStat .statsValueText').text(stat);
        });
        setInterval(function () { _this.setTime(); }, 1000);
    }
    PulseApp.prototype.checkTime = function (i) {
        return (i < 10) ? '0' + i : i;
    };
    PulseApp.prototype.setTime = function () {
        this.currentTime = new Date();
        var h = this.checkTime(this.currentTime.getHours());
        var m = this.checkTime(this.currentTime.getMinutes());
        var s = this.checkTime(this.currentTime.getSeconds());
        $('#timeStat .statsValueText').text(h + ':' + m + ':' + s);
    };
    PulseApp.prototype.createMap = function () {
        var mapOptions = {
            center: this.wellington,
            zoom: 6,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        this.map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);
        this.map.set('styles', [
            {
                "featureType": 'all',
                "elementType": 'labels.text.fill',
                "stylers": [
                    {
                        "color": '#ffffff'
                    }
                ]
            },
            {
                "featureType": 'all',
                "elementType": 'labels.text.stroke',
                "stylers": [
                    {
                        "color": '#000000'
                    },
                    {
                        "lightness": 13
                    }
                ]
            },
            {
                featureType: 'poi',
                elementType: 'labels',
                stylers: [
                    { visibility: 'off' }
                ]
            },
            {
                "featureType": 'road',
                "elementType": 'labels',
                "stylers": [
                    { "visibility": 'off' }
                ]
            },
            {
                "elementType": 'labels.text',
                "stylers": [
                    { "visibility": 'off' }
                ]
            },
            {
                "featureType": 'administrative',
                "elementType": 'geometry.fill',
                "stylers": [
                    {
                        "color": '#000000'
                    }
                ]
            },
            {
                "featureType": 'administrative',
                "elementType": 'geometry.stroke',
                "stylers": [
                    {
                        "color": '#144b53'
                    },
                    {
                        "lightness": 14
                    },
                    {
                        "weight": 1.4
                    }
                ]
            },
            {
                "featureType": 'landscape',
                "elementType": 'all',
                "stylers": [
                    {
                        "color": '#08304b'
                    }
                ]
            },
            {
                "featureType": 'poi',
                "elementType": 'geometry',
                "stylers": [
                    {
                        "color": '#0c4152'
                    },
                    {
                        "lightness": 5
                    }
                ]
            },
            {
                "featureType": 'road.highway',
                "elementType": 'geometry.fill',
                "stylers": [
                    {
                        "color": '#000000'
                    }
                ]
            },
            {
                "featureType": 'road.highway',
                "elementType": 'geometry.stroke',
                "stylers": [
                    {
                        "color": '#0b434f'
                    },
                    {
                        "lightness": 25
                    }
                ]
            },
            {
                "featureType": 'road.arterial',
                "elementType": 'geometry.fill',
                "stylers": [
                    {
                        "color": '#000000'
                    }
                ]
            },
            {
                "featureType": 'road.arterial',
                "elementType": 'geometry.stroke',
                "stylers": [
                    {
                        "color": '#0b3d51'
                    },
                    {
                        "lightness": 16
                    }
                ]
            },
            {
                "featureType": 'road.local',
                "elementType": 'geometry',
                "stylers": [
                    {
                        "color": '#000000'
                    }
                ]
            },
            {
                "featureType": 'transit',
                "elementType": 'all',
                "stylers": [
                    {
                        "color": '#146474'
                    }
                ]
            },
            {
                "featureType": 'water',
                "elementType": 'all',
                "stylers": [
                    {
                        "color": '#072232'
                    }
                ]
            }
        ]);
        //This event listener will call addMarker() when the map is clicked.
        //google.maps.event.addListener(this.map, 'click', event => {
        //    this.addMarker(event.latLng, 1, 'normal');
        //});
        return this.map;
    };
    PulseApp.prototype.addInteraction = function (startLocation, endLocation) {
        this.addMarker(startLocation, 3, 'normal');
        this.addLineAnimation(startLocation, endLocation, '#00FFCD');
        this.addMarker(endLocation, 3, 'normal');
    };
    PulseApp.prototype.addComment = function (startLocation, endLocation) {
        this.addMarker(startLocation, 6, 'small');
        this.addLineAnimation(startLocation, endLocation, '#FFF');
        this.addMarker(endLocation, 6, 'small');
    };
    PulseApp.prototype.addLineAnimation = function (startLocation, endLocation, strokeColor) {
        var path = new google.maps.Polyline({
            path: [startLocation, endLocation],
            icons: [{
                    icon: this.lineSymbol,
                    offset: '0%'
                }],
            geodesic: true,
            strokeColor: strokeColor,
            strokeOpacity: 0.2,
            strokeWeight: 2
        });
        var count = 0;
        var interval = window.setInterval(function () {
            count = (count + 1);
            var icons = path.get('icons');
            icons[0].offset = count + '%';
            path.set('icons', icons);
            if (count >= 100) {
                icons[0].icon = null;
                path.setMap(null);
                clearInterval(interval);
            }
        }, this.animationTimeout / 150);
        this.lines.push(path);
        path.setMap(this.map);
        //window.setInterval(() => {
        //        path.setMap(null);
        //      delete path;
        //}, this.animationTimeout);
    };
    // Add a marker to the map and push to the array.
    PulseApp.prototype.addMarker = function (location, color, size) {
        var classString = 'mapPointPulse ';
        var labelAnchorOffset = 0;
        switch (size) {
            case 'small':
                classString = classString + 'pulseSizeSmall ';
                labelAnchorOffset = 7;
                break;
            case 'normal':
                classString = classString + 'pulseSizeNormal ';
                labelAnchorOffset = 15;
                break;
            default:
        }
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
            case 6:
                classString = classString + 'white';
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
            labelAnchor: new google.maps.Point(labelAnchorOffset, labelAnchorOffset)
        });
        marker.setMap(this.map);
        setTimeout(function (markers) {
            marker.setMap(null);
            marker = null;
        }, this.animationTimeout);
        // return marker;
    };
    PulseApp.prototype.startEventsService = function () {
        this.pulseSocketConnection.start();
    };
    PulseApp.prototype.clearUsedMarkers = function () {
        for (var i = 0; i < this.markers.length; i++) {
            if (this.markers[i].getMap() == null) {
                this.markers.splice(i, 1);
                break;
            }
        }
    };
    PulseApp.prototype.clearUsedLines = function () {
        for (var i = 0; i < this.lines.length; i++) {
            if (this.lines[i].getMap() == null) {
                this.lines.splice(i, 1);
                break;
            }
        }
    };
    return PulseApp;
}());
$(document).ready(function () {
    var mapHelper = new PulseApp();
    mapHelper.createMap();
    mapHelper.startEventsService();
});
//# sourceMappingURL=app.js.map