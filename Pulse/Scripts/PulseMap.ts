class PulseMap {
    wellington = new google.maps.LatLng(-41.28, 174.77);
    map: google.maps.Map;
    markers: google.maps.Marker[] = new Array<google.maps.Marker>();
    lines: google.maps.Polyline[] = new Array<google.maps.Polyline>();
    animationTimeout: number = 1500; //milliseconds to delete animations

    createMap(): google.maps.Map {
        var mapOptions: google.maps.MapOptions = {
            center: this.wellington,
            zoom: 6,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        this.map = new google.maps.Map(document.getElementById("map-canvas"),
            mapOptions);

        this.map.set("styles", [

            {
                "featureType": "all",
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
        google.maps.event.addListener(this.map, 'click', event => {
            this.addMarker(event.latLng, 1, 'normal');
        });

        return this.map;
    }
    addInteraction(startLocation, endLocation) {
        if (document.hidden) {
            return;
        }
        this.addMarker(startLocation, 3, 'normal');
        this.addLineAnimation(startLocation, endLocation, '#00FFCD');
        this.addMarker(endLocation, 3, 'normal');
    }

    addComment(startLocation, endLocation) {
        if (document.hidden) {
            return;
        }
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

        setTimeout(() => {
            path.setMap(null);
        }, this.animationTimeout);

    }

    // Add a marker to the map and push to the array.
    addMarker(location, color: number, size: string) {
        if (document.hidden) {
            return;
        }
        var classString = "mapPointPulse ";
        var labelAnchorOffset = 0;
        switch (size) {
            case "small":
                classString = classString + "pulseSizeSmall ";
                labelAnchorOffset = 7;
                break;
            case "normal":
                classString = classString + "pulseSizeNormal ";
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
       
        setTimeout(() => {
            marker.setMap(null);
        }, this.animationTimeout);
        // return marker;

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

// ReSharper disable once InconsistentNaming
declare function MarkerWithLabel(opts: any): void;