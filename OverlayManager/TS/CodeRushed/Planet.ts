class Planet {
  constructor(
    public name: string,
    public gravity: number,
    public distanceFromSun: number,
    public mass: number,
    public diameter: number,
    public density: number,
    public escapeVelocity: number,
    public rotationPeriod: number,
    public dayLength: number,
    public orbitalPeriod: number,
    public orbitalVelocity: number,
    public orbitalInclination: number,
    public orbitalEccentricity: number,
    public obliquityToOrbit: number,
    public meanTemperature: number,
    public surfacePressure: number,
    public numberOfMoons: number,
    public ringSystem: boolean,
    public globalMagneticField: boolean,
    public imageFileName: string) {
	}
}