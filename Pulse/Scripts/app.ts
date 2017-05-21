/// <reference path="typings/google.maps.d.ts" />
/// <reference path="typings/jquery/jquery.d.ts" />


class PulseApp {
    wellington = new google.maps.LatLng(-41.28, 174.77);
    map: google.maps.Map;
    markers: google.maps.Marker[] = new Array<google.maps.Marker>();
    lines: google.maps.Polyline[] = new Array<google.maps.Polyline>();
    pulseSocketConnection: any;
    pulseSocketHub: any;
    animationTimeout: number = 1500; //milliseconds to delete animations
    currentTime = new Date();

    constructor() {
        this.pulseSocketConnection = $.hubConnection();
        this.pulseSocketConnection.logging = false;
        this.pulseSocketHub = this.pulseSocketConnection.createHubProxy('pulseSocketHub');
        this.pulseSocketHub.on('updateStandaloneEvents',(events: any[]) => {
            this.clearUsedLines();
            this.clearUsedMarkers();
            events.forEach((event) => {
                this.addMarker(new google.maps.LatLng(event.Latitude, event.Longitude), 1, 'normal');
            });
        });
        this.pulseSocketHub.on('updateInteractionEvents',(events: any[]) => {
            events.forEach((event) => {
                this.addInteraction(new google.maps.LatLng(event.StartLatitude, event.StartLongitude), new google.maps.LatLng(event.EndLatitude, event.EndLongitude));
            });

        });
        this.pulseSocketHub.on('updateCommentEvents',(events: any[]) => {
            events.forEach((event) => {
                this.addComment(new google.maps.LatLng(event.StartLatitude, event.StartLongitude), new google.maps.LatLng(event.EndLatitude, event.EndLongitude));
            });

        });
        this.pulseSocketHub.on('updateNewListingsStat',(stat: string) => {
            $('#itemsListedStat .statsValueText').text(stat);
        });
        this.pulseSocketHub.on('updateSoldTodayStat',(stat: string) => {
            $('#itemsSoldStat .statsValueText').text(stat);
        });

        setInterval(() => { this.setTime(); }, 1000);


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

    createMap(): google.maps.Map {
        var mapOptions: google.maps.MapOptions = {
            center: this.wellington,
            zoom: 6,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        this.map = new google.maps.Map(document.getElementById('map-canvas'),
            mapOptions);

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
            }
            , {
                "featureType": 'road',
                "elementType": 'labels',
                "stylers": [
                    { "visibility": 'off' }
                ]
            }
            , {
                "elementType": 'labels.text',
                "stylers": [
                    { "visibility": 'off' }
                ]
            }
            ,
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
        ]
            );

        //This event listener will call addMarker() when the map is clicked.
        //google.maps.event.addListener(this.map, 'click', event => {
        //    this.addMarker(event.latLng, 1, 'normal');
        //});

        return this.map;
    }
    addInteraction(startLocation, endLocation) {
        this.addMarker(startLocation, 3, 'normal');
        this.addLineAnimation(startLocation, endLocation, '#00FFCD');
        this.addMarker(endLocation, 3, 'normal');
    }

    addComment(startLocation, endLocation) {
        this.addMarker(startLocation, 6, 'small');
        this.addLineAnimation(startLocation, endLocation, '#FFF');
        this.addMarker(endLocation, 6, 'small');
    }

    lineSymbol = {
        path: google.maps.SymbolPath.CIRCLE,
        scale: 4,
        fillOpacity: 1.0,
        strokeColor: '#fff'
    };

    addLineAnimation(startLocation, endLocation, strokeColor) {
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
        var interval = window.setInterval(() => {
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

    }

    // Add a marker to the map and push to the array.
    addMarker(location, color: number, size: string) {
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

        setTimeout((markers: Array<google.maps.Marker>) => {
            marker.setMap(null);
            marker = null;

        }, this.animationTimeout);
        // return marker;

    }

    startEventsService() {
        this.pulseSocketConnection.start();
    }
    clearUsedMarkers() {
        for (var i = 0; i < this.markers.length; i++) {
            if (this.markers[i].getMap() == null) {
                this.markers.splice(i, 1);
                break;
            }
        }
    }
    clearUsedLines() {
        for (var i = 0; i < this.lines.length; i++) {
            if (this.lines[i].getMap() == null) {
                this.lines.splice(i, 1);
                break;
            }
        }
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