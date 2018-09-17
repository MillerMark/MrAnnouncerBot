class GravityGames {
  planets: Planet[] = [];
  constructor() {
    
    //`![](ADA02D1B4E37D1836A73BE11024DE9AD.png;;;0.00872,0.00872)
    var earth: Planet = new Planet('Earth', 9.8, 149.6, 5.97, 12756, 5514, 11.2, 23.9, 24, 365.2, 29.8, 0, 0.017, 23.4, 15, 1, 1, false, true, 'earth.png');
    //`![](2EA5331AC1B69756EDB184EB5997EFAF.png;;;0.00872,0.00872)
    var mercury: Planet = new Planet('Mercury', 3.7, 57.9, 0.33, 4879, 5427, 4.3, 1407.6, 4222.6, 88, 47.4, 7, 0.205, 0.034, 167, 0, 0, false, true, 'mercury.png');
    //`![](BB492F653A571E63E425D56AA5512DD3.png;;;0.00872,0.00872)
    var venus: Planet = new Planet('Venus', 8.9, 108.2, 4.87, 12104, 5243, 10.4, -5832.5, 2802, 224.7, 35, 3.4, 0.007, 177.4, 464, 92, 0, false, false, 'venus.png');
    //`![](A96C922E7566C724B375C5D2F7A895E9.png;;;0.00872,0.00872)
    var mars: Planet = new Planet('Mars', 3.7, 227.9, 0.642, 6792, 3933, 5, 24.6, 24.7, 687, 24.1, 1.9, 0.094, 25.2, -65, 0.01, 2, false, false, 'mars.png');
    //`![](600E64298818BB51AD56847BA5020676.png;;;0.00872,0.00872)
    var pluto: Planet = new Planet('Pluto', 0.7, 5906.4, 0.0146, 2370, 2095, 1.3, -153.3, 153.3, 90560, 4.7, 17.2, 0.244, 122.5, -225, 0.00001, 5, false, undefined, 'pluto.png');
    //`![](D56CFE4BC8A6D254685FE11E9346FBE9.png;;;0.00872,0.00872)
    var sun: Planet = new Planet('Sun', 274, 0, 1.94 * Math.pow(10, 27), 1.39 * Math.pow(10, 6), 1410, 618, 609.6, undefined, undefined, undefined, undefined, undefined, undefined, 5506.85, 8.68 * Math.pow(10, - 4), 0, false, true, 'sun.png');
    //`![](750FB5A3C4996042B8CB0497C7FF88BD.png;;;0.00872,0.00872)
    var uranus: Planet = new Planet('Uranus', 8.7, 2872.5, 86.8, 51118, 1271, 21.3, -17.2, 17.2, 30589, 6.8, 0.8, 0.046, 97.8, -195, undefined, 27, true, true, 'uranus.png');
    //`![](64417F2A450A7A56419E229A49E378CD.png;;;0.00872,0.00872)
    var neptune: Planet = new Planet('Neptune', 11, 4495.1, 102, 49528, 1638, 23.5, 16.1, 16.1, 59800, 5.4, 1.8, 0.011, 28.3, -200, undefined, 14, true, true, 'neptune.png');
    //`![](D4B38549C7C0A76715A8C4C000ACE2F3.png;;;0.00872,0.00872)
    var jupiter: Planet = new Planet('Jupiter', 23.1, 778.6, 1898, 142984, 1326, 59.5, 9.9, 9.9, 4331, 13.1, 1.3, 0.049, 3.1, -110, undefined, 67, true, true, 'jupiter.png');
    //`![](BAEBE18F0FC74D6C9E8BB1E70BC5AF63.png;;;0.00872,0.00872)
    var moon: Planet = new Planet('Moon', 1.6, 0.384, 0.073, 3475, 3340, 2.4, 655.7, 708.7, 27.3, 1, 5.1, 0.055, 6.7, -20, 0, 0, false, false, 'moon.png');
    //`![](30A344602A04E9959730D7DCC5F55370.png;;;0.00872,0.00872)
    var saturn: Planet = new Planet('Saturn', 9, 1433.5, 568, 120536, 687, 35.5, 10.7, 10.7, 10747, 9.7, 2.5, 0.057, 26.7, -140, undefined, 62, true, true, 'saturn.png');

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
}