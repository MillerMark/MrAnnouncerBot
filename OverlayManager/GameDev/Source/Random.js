var Random = (function () {
    function Random() {
    }
    Random.getInt = function (upperBound) {
        return Math.floor(Math.random() * upperBound);
    };
    Random.between = function (lowerBound, upperBound) {
        return lowerBound + Math.floor(Math.random() * (upperBound - lowerBound));
    };
    return Random;
}());
//# sourceMappingURL=Random.js.map