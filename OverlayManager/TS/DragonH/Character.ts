class Character {
  name: string;
  level: number;
  inspiration: number;
  experience: number;
  raceClass: string;
  alignment: string;
  armorClass: number;
  initiative: number;
  speed: number;
  hitPoints: number;
  tempHitPoints: number;
  hitDice: string;
  deathSaveLife1: boolean;
  deathSaveLife2: boolean;
  deathSaveLife3: boolean;
  deathSaveDeath1: boolean;
  deathSaveDeath2: boolean;
  deathSaveDeath3: boolean;

  private _wisdomMod: number;
  
  get wisdomMod(): number {
  	return this._wisdomMod;
  }

  private _wisdom: number;
  
  get wisdom(): number {
  	return this._wisdom;
  }
  
  set wisdom(newValue: number) {
    this._wisdom = newValue;
    this._wisdomMod = this.getModFromAbility(this._wisdom);
  }
  
  private _charismaMod: number;
  
  get charismaMod(): number {
  	return this._charismaMod;
  }

  private _charisma: number;
  
  get charisma(): number {
  	return this._charisma;
  }
  
  set charisma(newValue: number) {
    this._charisma = newValue;
    this._charismaMod = this.getModFromAbility(this._charisma);
  }
  
  private _intelligenceMod: number;
  
  get intelligenceMod(): number {
  	return this._intelligenceMod;
  }

  private _intelligence: number;
  
  get intelligence(): number {
  	return this._intelligence;
  }
  
  set intelligence(newValue: number) {
    this._intelligence = newValue;
    this._intelligenceMod = this.getModFromAbility(this._intelligence);
  }
  
  private _strengthMod: number;
  
  get strengthMod(): number {
  	return this._strengthMod;
  }

  private _strength: number;
  
  get strength(): number {
  	return this._strength;
  }
  
  set strength(newValue: number) {
    this._strength = newValue;
    this._strengthMod = this.getModFromAbility(this._strength);
  }
  
  private _dexterityMod: number;
  
  get dexterityMod(): number {
  	return this._dexterityMod;
  }

  private _dexterity: number;
  
  get dexterity(): number {
  	return this._dexterity;
  }
  
  set dexterity(newValue: number) {
    this._dexterity = newValue;
    this._dexterityMod = this.getModFromAbility(this._dexterity);
  }
  
  private _constitutionMod: number;
  
  get constitutionMod(): number {
  	return this._constitutionMod;
  }

  private _constitution: number;
  
  get constitution(): number {
  	return this._constitution;
  }
  
  set constitution(newValue: number) {
    this._constitution = newValue;
    this._constitutionMod = this.getModFromAbility(this._constitution);
  }
  
  getModFromAbility(abilityScore: number): number {
    return Math.floor((abilityScore - 10) / 2);
  }

	constructor() {
		
  }

  static rollDie(sides: number): number {
    return Random.intBetween(1, sides + 1);
  }

  static getAbilityScore(): number {
    return Character.rollDie(6) + Character.rollDie(6) + Character.rollDie(6);
  }

  getPropValue(name: string): number | string | boolean {
    return this[name];
  }

  static newRoryTestElf(): Character {
    let elf: Character = new Character();
    elf.name = 'Rorificent';
    elf.raceClass = 'Wood Elf Barbarian';
    elf.alignment = 'Lawful Good';
    elf.armorClass = 12;
    elf.charisma = Character.getAbilityScore();
    elf.constitution = Character.getAbilityScore();
    elf.dexterity = Character.getAbilityScore();
    elf.wisdom = Character.getAbilityScore();
    elf.intelligence = Character.getAbilityScore();
    elf.strength = Character.getAbilityScore();
    elf.hitDice = '1 d10';
    return elf;
  }
}