var GravityGames = (function () {
    function GravityGames() {
        this.planets = [];
        var earth = new Planet('Earth', 9.8, 149.6, 5.97, 12756, 5514, 11.2, 23.9, 24, 365.2, 29.8, 0, 0.017, 23.4, 15, 1, 1, false, true, 'earth.png');
        var mercury = new Planet('Mercury', 3.7, 57.9, 0.33, 4879, 5427, 4.3, 1407.6, 4222.6, 88, 47.4, 7, 0.205, 0.034, 167, 0, 0, false, true, 'mercury.png');
        var venus = new Planet('Venus', 8.9, 108.2, 4.87, 12104, 5243, 10.4, -5832.5, 2802, 224.7, 35, 3.4, 0.007, 177.4, 464, 92, 0, false, false, 'venus.png');
        var mars = new Planet('Mars', 3.7, 227.9, 0.642, 6792, 3933, 5, 24.6, 24.7, 687, 24.1, 1.9, 0.094, 25.2, -65, 0.01, 2, false, false, 'mars.png');
        var pluto = new Planet('Pluto', 0.7, 5906.4, 0.0146, 2370, 2095, 1.3, -153.3, 153.3, 90560, 4.7, 17.2, 0.244, 122.5, -225, 0.00001, 5, false, undefined, 'pluto.png');
        var sun = new Planet('Sun', 274, 0, 1.94 * Math.pow(10, 27), 1.39 * Math.pow(10, 6), 1410, 618, 609.6, undefined, undefined, undefined, undefined, undefined, undefined, 5506.85, 8.68 * Math.pow(10, -4), 0, false, true, 'sun.png');
        var uranus = new Planet('Uranus', 8.7, 2872.5, 86.8, 51118, 1271, 21.3, -17.2, 17.2, 30589, 6.8, 0.8, 0.046, 97.8, -195, undefined, 27, true, true, 'uranus.png');
        var neptune = new Planet('Neptune', 11, 4495.1, 102, 49528, 1638, 23.5, 16.1, 16.1, 59800, 5.4, 1.8, 0.011, 28.3, -200, undefined, 14, true, true, 'neptune.png');
        var jupiter = new Planet('Jupiter', 23.1, 778.6, 1898, 142984, 1326, 59.5, 9.9, 9.9, 4331, 13.1, 1.3, 0.049, 3.1, -110, undefined, 67, true, true, 'jupiter.png');
        var moon = new Planet('Moon', 1.6, 0.384, 0.073, 3475, 3340, 2.4, 655.7, 708.7, 27.3, 1, 5.1, 0.055, 6.7, -20, 0, 0, false, false, 'moon.png');
        var saturn = new Planet('Saturn', 9, 1433.5, 568, 120536, 687, 35.5, 10.7, 10.7, 10747, 9.7, 2.5, 0.057, 26.7, -140, undefined, 62, true, true, 'saturn.png');
        this.planets.push(earth);
        this.planets.push(mercury);
        this.planets.push(venus);
        this.planets.push(mars);
        this.planets.push(pluto);
        this.planets.push(sun);
        this.planets.push(uranus);
        this.planets.push(neptune);
        this.planets.push(jupiter);
        this.planets.push(saturn);
        this.planets.push(moon);
    }
    return GravityGames;
}());
//# sourceMappingURL=GravityGames.js.map