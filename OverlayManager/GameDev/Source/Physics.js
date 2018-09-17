var pixelsPerMeter = 50;
var Physics = (function () {
    function Physics() {
    }
    Physics.metersToPixels = function (meters) {
        return meters * pixelsPerMeter;
    };
    Physics.getDisplacement = function (time, initialVelocity, acceleration) {
        return initialVelocity * time + acceleration * time * time / 2;
    };
    Physics.getFinalVelocity = function (time, initialVelocity, acceleration) {
        return initialVelocity + acceleration * time;
    };
    return Physics;
}());
//# sourceMappingURL=Physics.js.map