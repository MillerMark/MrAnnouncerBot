class Planet {
  constructor(
    private name: string,
    private gravity: number,
    private distanceFromSun: number,
    private mass: number,
    private diameter: number,
    private density: number,
    private escapeVelocity: number,
    private rotationPeriod: number,
    private dayLength: number,
    private orbitalPeriod: number,
    private orbitalVelocity: number,
    private orbitalInclination: number,
    private orbitalEccentricity: number,
    private obliquityToOrbit: number,
    private meanTemperature: number,
    private surfacePressure: number,
    private numberOfMoons: number,
    private ringSystem: boolean,
    private globalMagneticField: boolean,
    private imageFileName: string) {
	}
}