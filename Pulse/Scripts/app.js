/// <reference path="typings/google.maps.d.ts" />
/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="PulseApiConnection.ts" />
$(document).ready(function () {
    var mapHelper = new PulseApiConnection();
    mapHelper.createMap();
    mapHelper.startEventsService();
});
//# sourceMappingURL=app.js.map