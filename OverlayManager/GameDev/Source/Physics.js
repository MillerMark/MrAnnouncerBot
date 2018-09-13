var pixelsPersMeter = 50;
var Physics = (function () {
    function Physics() {
    }
    Physics.metersToPixels = function (meters) {
        return meters * pixelsPersMeter;
    };
    Physics.getDisplacement = function (time, initialVelocity, acceleration) {
        var metersTravelled = initialVelocity * time + 0.5 * acceleration * time * time;
        return metersTravelled;
    };
    Physics.getFinalVelocity = function (time, initialVelocity, acceleration) {
        var finalVelocity = initialVelocity + acceleration * time;
        return finalVelocity;
    };
    return Physics;
}());
//# sourceMappingURL=Physics.js.map