/// <reference path="typings/google.maps.d.ts" />
/// <reference path="typings/jquery/jquery.d.ts" />

$(document).ready(() => {
    var mapHelper = new PulseApiConnection();
    mapHelper.createMap();
    mapHelper.startEventsService();

});