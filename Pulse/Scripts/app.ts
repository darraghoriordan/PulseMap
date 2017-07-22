/// <reference path="typings/google.maps.d.ts" />
/// <reference path="typings/jquery/jquery.d.ts" />

$(document).ready(() => {
    var mapHelper = new PulseHubConnection();
    mapHelper.createMap();
    mapHelper.startEventsService();

});