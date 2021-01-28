let diceRollerShowFpsWindow: boolean = false


function hideDieIn(dieObject, ms: number) {
	dieObject.hideTime = performance.now() + ms;
	dieObject.needToHideDie = true;
}

enum DieRollType {
	Player,
	Viewer
}

enum WildMagic {
	none,
	wildMagicMinute,
	seeInvisibleCreatures,
	modronAppearsOneMinute,
	regain5hpPerTurnForOneMinute,
	castMagicMissile,
	castFireball,
	heightChange,
	castConfusionOnSelf,
	beardOfFeathers,
	castGreaseCenteredOnSelf,
	spellTargetsDisadvantagedSavingThrowForOneMinute,
	skinTurnsBlue,
	thirdEyeAdvantageWisdomChecks,
	castTimeBonusActionOneMinute,
	teleportUpTo60Feet,
	astralPlaneUntilEndOfNextTurn,
	maximizeDamageOnSpellCastInNextMinute,
	ageChange,
	flumphs,
	regainHitPoints,
	pottedPlant,
	teleportUpTo20FeetBonusActionOneMinute,
	castLevitateOnSelf,
	unicorn,
	cannotSpeakPinkBubbles,
	spectralShieldPlus2ArmorClassNextMinute,
	alcoholImmunity,
	hairFallsOutGrowsBack24Hours,
	fireTouchOneMinute,
	regainLowestLevelExpendedSpellSlot,
	shoutWhenSpeakingOneMinute,
	castFogCloudCenteredOnSelf,
	lightningDamageUpToThreeCreatures,
	frightenedByNearestCreatureUntilEndOfNextTurn,
	allCreatures30FeetInvisibleOneMinute,
	resistanceToAllDamageNextMinute,
	randomCreaturePoisoned1d4Hours,
	glowBrightOneMinuteCreaturesEndingTurn5FeetBlinded,
	castPolymorphToSheepOnSelf,
	butterfliesAndPetals10FeetOneMinute,
	takeOneAdditionalActionImmediately,
	allCreaturesWithin30FeetTake1d10NecroticDamage,
	castMirrorImage,
	castFlyOnRandomCreatureWithin60Feet,
	invisibleSilentNextMinute,
	immortalOneMinute,
	increaseSizeOneMinute,
	allCreatures30FeetVulnerableToPiercingDamageOneMinute,
	faintEtheralMusicOneMinute,
	regainSorceryPoints
}

class RollResults {
	constructor(public d20RollValue: Map<number, number>, public toHitModifier: number, public singlePlayerId: number, public luckValue: Map<number, number>,
		public playerIdForTextMessages: number, public skillSavingModifier: number, public totalDamage: number,
		public totalHealth: number, public totalExtra: number, public maxDamage: number,
		public damageSummary: Map<DamageType, number>) {
	}
}

class PlayerRoll {
	constructor(public roll: number, public name: string, public id: number, public data: string, public modifier: number = 0, public success: boolean = false, public isCrit: boolean = false, public isCompleteFail: boolean = false) {
	}
}

class MockDie {
	constructor(public name: string, public topNumber: number) {
		this.isD20 = true;
		this.inPlay = true;
	}
	isD20: boolean;
	inPlay: boolean;
	getTopNumber(): number {
		return this.topNumber;
	}
	getObject(): any {
		return null;
	}
}

interface IDieObject {
	body;
	position;
	quaternion;
	scale;
	hideTime: number;
	needToHideDie;
	isHidden;
	localToWorld(screenVector);
	needToDrop: boolean;
	hideOnScaleStop: boolean;
	needToStartEffect: boolean;
	effectStartOffset: number;
	effectKind: DieEffect;
	removeTime: number;
}

interface IDie {
	getUpsideValue();
	name: string;
	topNumber: number;
	scale: number;
	sparks: Array<SpriteProxy>;
	attachedDamage: boolean;
	damageStr: string;
	damageType: DamageType;
	values: number;  // The number of faces on the die
	rollType: DieCountsAs;
	playerName: string;
	dataStr: string;
	dieType: string;
	kind: VantageKind;
	attachedSprites: Array<SpriteProxy>;
	attachedLabels: Array<TextEffect>;
	group: DiceGroup;
	origins: Array<Vector>;
	lastPos: Array<Vector>;
	isLucky: boolean;
	lastPrintOnLeft: boolean;
	inPlay: boolean;
	scoreHasBeenTallied: boolean;
	isD20: boolean;
	playerID: number;
	diceColor: string;
	hasNotHitFloorYet: boolean;
	getObject();
	updateBodyFromMesh();
	clear(dieRoller: DieRoller);
	getTopNumber(): number;
}

enum DieCountsAs {
	totalScore,
	inspiration,
	damage,
	health,
	extra,
	bonus,
	bentLuck
}

enum DieEffect {
	Ring,
	Lucky,
	Fireball,
	FuturisticGroundPortal,
	SmokeyPortal,
	Shockwave,
	ColoredSmoke,
	Bomb,
	GroundBurst,
	SteamPunkTunnel,
	HandGrab,
	Random
}

//`!-------------------------------------------------------
const diceSounds = new DiceSounds('GameDev/Assets/DragonH/SoundEffects');

class DieRoller {
	constructor(private groupName: string) {

	}
	//const showDieValues = false;
	diceRollData: DiceRollData;
	totalDamagePlusModifier = 0;
	totalHealthPlusModifier = 0;
	totalExtraPlusModifier = 0;
	totalBonus = 0;
	d20RollValue = -1;
	attemptedRollWasSuccessful = false;
	wasCriticalHit = false;
	attemptedRollWasNarrowlySuccessful = false;
	static readonly diceToRoll: number = 10;
	static readonly secondsBetweenRolls: number = 12;
	static readonly removeDiceImmediately: boolean = false;
	static readonly dieScale: number = 1.5;
	camera;
	renderer;
	onBonusThrow = false;
	setupBonusRoll = false;
	waitingForBonusRollToComplete = false;
	bonusRollStartTime = 0;
	randomDiceThrowIntervalId = 0;
	damageModifierThisRoll = 0;
	healthModifierThisRoll = 0;
	extraModifierThisRoll = 0;
	additionalDieRollMessage = '';
	container;
	scene;
	controls;
	stats;
	world;
	dice = [];
	scalingDice: IDie[] = [];
	specialDice: IDie[] = [];

	waitingForSettle = false;
	allDiceHaveStoppedRolling = false;
	firstStopTime: number;
	diceValues = [];

	setNormalGravity() {
		this.world.gravity.set(0, -9.82 * 20, 0);
	}

	restoreDieScale() {
		for (let i = 0; i < this.dice.length; i++) {
			const die = this.dice[i].getObject();
			if (die)
				die.scale.set(1, 1, 1);
		}
	}

	clearAllDice() {
		if (!this.dice || this.dice.length === 0)
			return;
		for (let i = 0; i < this.dice.length; i++) {
			const die: IDie = this.dice[i];
			const dieObject: IDieObject = die.getObject();
			if (dieObject) {
				this.scene.remove(dieObject);
				die.clear(this);
			}
		}
		this.dice = [];
		this.specialDice = [];
		this.scalingDice = [];
	}

	getScreenCoordinates(die: IDie): Vector {
		const dieObject: IDieObject = die.getObject();
		if (dieObject === null)
			return null;
		// @ts-ignore - THREE
		const screenVector = new THREE.Vector3();
		dieObject.localToWorld(screenVector);

		screenVector.project(this.camera);

		const x = Math.round((screenVector.x + 1) * this.renderer.domElement.offsetWidth / 2);
		const y = Math.round((1 - screenVector.y) * this.renderer.domElement.offsetHeight / 2);

		return new Vector(x, y);
	}



	clearBeforeRoll(diceGroup: DiceGroup) {
		this.additionalDieRollMessage = '';
		diceLayer.clearResidualEffects(diceGroup);
		this.scalingDice = [];
		this.specialDice = [];
		this.restoreDieScale();
		this.setNormalGravity();
		this.waitingForSettle = true;
		this.setupBonusRoll = false;
		this.waitingForBonusRollToComplete = false;
		this.onBonusThrow = false;
		this.diceValues = [];
		this.totalDamagePlusModifier = 0;
		this.totalHealthPlusModifier = 0;
		this.totalExtraPlusModifier = 0;
		this.diceRollData.totalRoll = 0;
		this.d20RollValue = -1;
		this.totalBonus = 0;
		this.damageModifierThisRoll = 0;
		//console.log('damageModifierThisRoll = 0;');
		this.healthModifierThisRoll = 0;
		this.extraModifierThisRoll = 0;
		this.clearAllDice();
		this.allDiceHaveStoppedRolling = false;
	}

	fpsIntervalDiceRoller: number;
	//let startTimeDiceRoller: number;
	lastDrawTimeDiceRoller: number;

	changeFramerateDiceRoller(fps: number): any {
		if (fps === -1)
			return;
		this.fpsIntervalDiceRoller = 1000 / fps;
		this.lastDrawTimeDiceRoller = Date.now();
	}

	dieFirstHitsFloor(die) {
		if (this.diceRollData.onFirstContactSound) {
			diceSounds.safePlayMp3(this.diceRollData.onFirstContactSound);
			this.diceRollData.onFirstContactSound = null;
		}

		if (die.rollType === DieCountsAs.inspiration) {
			diceSounds.safePlayMp3('inspiration');
		}

		if (die.rollType === DieCountsAs.totalScore || die.rollType === DieCountsAs.inspiration) {
			// Move the effect closer to the center of the screen...
			const percentageOnDie = 0.7;
			const percentageOffDie: number = 1 - percentageOnDie;


			if (this.diceRollData.onFirstContactEffect) {
				const pos: Vector = this.getScreenCoordinates(die);
				if (pos) {
					const x: number = pos.x * percentageOnDie + percentageOffDie * 960;
					const y: number = pos.y * percentageOnDie + percentageOffDie * 540;

					diceLayer.AddEffect(die, this.diceRollData.onFirstContactEffect, x, y, this.diceRollData.effectScale,
						this.diceRollData.effectHueShift, this.diceRollData.effectSaturation,
						this.diceRollData.effectBrightness, this.diceRollData.effectRotation);
				}
			}
		}
	}

	trailsSparks(diceRollData: DiceRollData): boolean {
		// mkm diceRollData.type == DiceRollType.WildMagic || 
		return diceRollData.type === DiceRollType.BendLuckAdd || diceRollData.type === DiceRollType.BendLuckSubtract;
	}

	handleDieCollision(e) {
		// @ts-ignore - DiceManager
		if (this.diceManager.throwRunning || this.dice.length === 0)
			return;

		const relativeVelocity: number = Math.abs(Math.round(e.contact.getImpactVelocityAlongNormal()));
		//console.log(e.target.name + ' -> ' + e.body.name + ' at ' + relativeVelocity + 'm/s');

		if (e.target.name === "die" && e.body.name === "die")
			diceSounds.playDiceHit(relativeVelocity / 10);
		else if (e.target.name === "die" && e.body.name === "floor") {
			if (e.target.parentDie.hasNotHitFloorYet) {
				e.target.parentDie.hasNotHitFloorYet = false;
				this.dieFirstHitsFloor(e.target.parentDie);
			}
			if (relativeVelocity < 8) {
				diceSounds.playSettle();
			}
			else {
				diceSounds.playFloorHit(relativeVelocity / 35);

				if (this.trailsSparks(this.diceRollData)) {
					if (relativeVelocity > 12) {
						if (!e.target.parentDie.sparks)
							e.target.parentDie.sparks = [];
						const pos: Vector = this.getScreenCoordinates(e.target.parentDie);
						if (pos)
							e.target.parentDie.sparks.push(diceLayer.smallSpark(e.target.parentDie, pos.x, pos.y));
						diceSounds.safePlayMp3('Dice/Zap[4]');
					}
				}
			}
		}
		else if (e.target.name === "die" && e.body.name === "wall")
			diceSounds.playWallHit(relativeVelocity / 40);
	}

	needToClearD20s = true;

	getRandomEffect() {
		const random: number = Math.random() * 100;
		if (random < 13)
			return DieEffect.Bomb;
		else if (random < 22)
			return DieEffect.ColoredSmoke;
		else if (random < 35)
			return DieEffect.SmokeyPortal;
		else if (random < 50)
			return DieEffect.GroundBurst;
		else if (random < 65)
			return DieEffect.SteamPunkTunnel;
		else if (random < 80)
			return DieEffect.FuturisticGroundPortal;
		else
			return DieEffect.Random;
	}

	getDieEffectDistance(): number {
		return Math.round(Math.random() * 5) * 40 + 40;
	}

	removeDie(die: IDie, dieEffectInterval: number, effectOverride: DieEffect = undefined) {
		if (!die.inPlay)
			return;

		if (effectOverride === undefined || effectOverride === DieEffect.Random) {
			let numTries = 0;
			effectOverride = this.getRandomEffect();
			while (effectOverride === DieEffect.Random && numTries < 10) {
				effectOverride = this.getRandomEffect();
				numTries++;
			}
			if (effectOverride === DieEffect.Random)
				effectOverride = DieEffect.Bomb;
		}

		const dieObject: IDieObject = die.getObject();
		if (dieObject) {
			dieObject.removeTime = performance.now();
			dieObject.effectKind = effectOverride;
			dieEffectInterval += this.getDieEffectDistance();
			dieObject.effectStartOffset = dieEffectInterval;
			dieObject.needToStartEffect = true;
			this.specialDice.push(die);  // DieEffect.Portal too???

			if (effectOverride === DieEffect.FuturisticGroundPortal) {
				dieObject.hideOnScaleStop = true;
				dieObject.needToDrop = true;
				this.scalingDice.push(die);

				//console.log('die.body.collisionResponse: ' + die.body.collisionResponse);
				//die.body.collisionResponse = 0;
				//die.body.mass = 0;
				//world.gravity.set(0, -9.82 * 2000, 0);
				//die.position.x = -99999;
				// @ts-ignore - CANNON
				//var localVelocity = new CANNON.Vec3(0, 00, 0);
				//die.body.velocity = localVelocity;
				//let factor: number = 5;
				//let xSpin: number = Math.random() * factor - factor / 2;
				//let ySpin: number = Math.random() * factor - factor / 2;
				//let zSpin: number = Math.random() * factor - factor / 2;
				//// @ts-ignore - THREE
				//die.body.angularVelocity.set(xSpin, ySpin, zSpin);
				//setTimeout(spinDie.bind(die), 200);
			}
			else if (effectOverride === DieEffect.HandGrab) {
				dieObject.hideOnScaleStop = true;
				dieObject.needToDrop = true;
				this.scalingDice.push(die);
			}
			else if (effectOverride === DieEffect.SteamPunkTunnel) {
				dieObject.hideOnScaleStop = true;
				dieObject.needToDrop = true;
				this.scalingDice.push(die);
			}
		}

		//console.log(`removeDie - die.inPlay = false;`);
		die.inPlay = false;
		return dieEffectInterval;
	}

	removeDieEffectsForSingleDie(die: IDie) {
		const now: number = performance.now();
		if (die.attachedSprites)
			for (let i = 0; i < die.attachedSprites.length; i++) {
				const sprite: SpriteProxy = die.attachedSprites[i];
				const fadeOutTime = 1200;
				sprite.expirationDate = now + fadeOutTime;
				sprite.fadeOutTime = fadeOutTime;
			}
		die.attachedSprites = [];

		if (die.attachedLabels)
			for (let i = 0; i < die.attachedLabels.length; i++) {
				const textEffect: TextEffect = die.attachedLabels[i];
				const fadeOutTime = 1200;
				textEffect.expirationDate = now + fadeOutTime;
				textEffect.fadeOutTime = fadeOutTime;
			}
		die.attachedLabels = [];

		die.origins = [];
	}

	removeSingleDieWithEffect(die: IDie, dieEffect: DieEffect, effectInterval: number) {
		//console.log(`removeSingleDieWithEffect `);
		this.removeDie(die, effectInterval, dieEffect);
		for (let i = 0; i < this.dice.length; i++) {
			if (this.dice[i] === die) {
				die.inPlay = false; // This line appears to be redundant.
				this.removeDieEffectsForSingleDie(die);
			}
		}
	}

	handRemoveDie(die: IDie, effectInterval = 0) {
		if (!die)
			return;
		this.removeSingleDieWithEffect(die, DieEffect.HandGrab, effectInterval);
	}

	removeNonVantageDieNow(die: IDie) {
		if (!die)
			return;
		this.diceRollData.appliedVantage = true;
		this.handRemoveDie(die, 0);
	}

	isAttack(diceRollData: DiceRollData): boolean {
		return diceRollData.type === DiceRollType.Attack || diceRollData.type === DiceRollType.ChaosBolt;
	}

	getMostRecentDiceRollData() {
		if (this.diceRollData.secondRollData)
			return this.diceRollData.secondRollData;
		else
			return this.diceRollData;
	}

	removeMultiplayerD20s(): void {
		this.needToClearD20s = false;
		const localDiceRollData: DiceRollData = this.getMostRecentDiceRollData();
		const isWildMagic: boolean = localDiceRollData.type === DiceRollType.WildMagic;
		if (isWildMagic || !localDiceRollData.itsAD20Roll)
			return;
		const vantageTextDelay = 900;

		const playerEdgeRolls: Array<number> = [];
		const otherPlayersDie: Array<IDie> = [];
		const maxSlots = 60;
		const midPoint: number = maxSlots / 2;  // Negative player id's will be less than this midpoint. Positive will be higher.
		for (let j = 0; j < maxSlots; j++) {
			playerEdgeRolls.push(-1);
			otherPlayersDie.push(null);
		}

		for (let i = 0; i < this.dice.length; i++) {
			const die: IDie = this.dice[i];
			const playerId: number = midPoint + die.playerID;
			const topNumber = die.getTopNumber();

			if (die.isD20) {
				if (playerEdgeRolls[playerId] === -1)
					playerEdgeRolls[playerId] = topNumber;
				else if (die.kind === VantageKind.Advantage) {
					if (playerEdgeRolls[playerId] <= topNumber) {
						this.removeNonVantageDieNow(otherPlayersDie[playerId]);
						const centerPos: Vector = this.getScreenCoordinates(otherPlayersDie[playerId]);
						diceLayer.addAdvantageText(centerPos, vantageTextDelay, false, this.diceRollData.diceGroup);
						playerEdgeRolls[playerId] = topNumber;
					}
					else {
						this.removeNonVantageDieNow(die);
						const centerPos: Vector = this.getScreenCoordinates(die);
						diceLayer.addAdvantageText(centerPos, vantageTextDelay, false, this.diceRollData.diceGroup);
					}
				}
				else if (die.kind === VantageKind.Disadvantage) {
					if (playerEdgeRolls[playerId] >= topNumber) {
						this.removeNonVantageDieNow(otherPlayersDie[playerId]);
						const centerPos: Vector = this.getScreenCoordinates(otherPlayersDie[playerId]);
						diceLayer.addDisadvantageText(centerPos, vantageTextDelay, false, this.diceRollData.diceGroup);
						playerEdgeRolls[playerId] = topNumber;
					}
					else {
						this.removeNonVantageDieNow(die);
						const centerPos: Vector = this.getScreenCoordinates(die);
						diceLayer.addDisadvantageText(centerPos, vantageTextDelay, false, this.diceRollData.diceGroup);
					}
				}
				otherPlayersDie[playerId] = die;
			}
		}
	}

	flameOn(die: IDie, scale = 1) {
		const dieObject = die.getObject();

		const screenPos: Vector = this.getScreenCoordinates(dieObject);
		if (!screenPos)
			return;

		diceLayer.addFireball(screenPos.x, screenPos.y, scale);
		diceLayer.attachDamageFire(die, scale, diceLayer.getHueShift(die.playerID) - 15);
		diceSounds.playFireball();
	}

	groundBurstDie(die: IDie, effectInterval = 0) {
		if (!die)
			return;
		this.removeSingleDieWithEffect(die, DieEffect.GroundBurst, effectInterval);
	}

	removeWildMagicRollsGreaterThanOne(): void {
		//let localDiceRollData: DiceRollData = getMostRecentDiceRollData(diceRollData);
		let interval = 200;
		const timeBetweenDieRemoval = 300;
		const intervalVariance = 100;
		const dieScale: number = this.getDieScale();
		for (let i = 0; i < this.dice.length; i++) {
			const die: IDie = this.dice[i];

			const topNumber = die.getTopNumber();

			if (topNumber === 1) {
				this.flameOn(die, dieScale);
			}
			else {
				//handRemoveDie(die, interval);
				this.groundBurstDie(die, interval);
				interval += timeBetweenDieRemoval + Random.plusMinus(intervalVariance);
			}
		}
	}

	removeD20s(): number {
		//console.log('removeD20s...');
		this.needToClearD20s = false;
		let edgeRollValue = -1;
		const localDiceRollData: DiceRollData = this.getMostRecentDiceRollData();
		const isLuckRollHigh: boolean = localDiceRollData.type === DiceRollType.LuckRollHigh;
		const isLuckRollLow: boolean = localDiceRollData.type === DiceRollType.LuckRollLow;
		const isLuckRoll: boolean = isLuckRollHigh || isLuckRollLow;
		const isWildMagic: boolean = localDiceRollData.type === DiceRollType.WildMagic;

		if (localDiceRollData.type === DiceRollType.WildMagicD20Check) {
			this.removeWildMagicRollsGreaterThanOne();
			return;
		}

		if (isWildMagic || !localDiceRollData.itsAD20Roll)
			return edgeRollValue;
		let otherDie = null;
		const vantageTextDelay = 900;
		//console.log('diceRollData.appliedVantage: ' + diceRollData.appliedVantage);

		if (!this.diceRollData.appliedVantage) {
			for (let i = 0; i < this.dice.length; i++) {
				const die: IDie = this.dice[i];
				if (die.isLucky)
					continue;

				// TODO: Check behavior of Sorcerer's luck and bent luck rolls and see if we need to change those.
				//const isNormal: boolean = die.kind == DiceRollKind.Normal;
				//if (isNormal && !isLuckRoll)
				//	continue;

				const topNumber = die.getTopNumber();

				if (die.isD20) {
					if (edgeRollValue === -1)
						edgeRollValue = topNumber;
					else if (die.kind === VantageKind.Advantage) {
						if (edgeRollValue <= topNumber) {
							this.removeNonVantageDieNow(otherDie);
							const centerPos: Vector = this.getScreenCoordinates(otherDie);
							diceLayer.addAdvantageText(centerPos, vantageTextDelay, false, this.diceRollData.diceGroup);
							edgeRollValue = topNumber;
						}
						else {  // Disadvantage
							this.removeNonVantageDieNow(die);
							const centerPos: Vector = this.getScreenCoordinates(die);
							diceLayer.addAdvantageText(centerPos, vantageTextDelay, false, this.diceRollData.diceGroup);
						}
					}
					else if (die.kind === VantageKind.Disadvantage) {
						if (edgeRollValue >= topNumber) {
							this.removeNonVantageDieNow(otherDie);
							//if (!localDiceRollData.showedVantageMessage) {
							//	localDiceRollData.showedVantageMessage = true;
							const centerPos: Vector = this.getScreenCoordinates(otherDie);
							diceLayer.addDisadvantageText(centerPos, vantageTextDelay, false, this.diceRollData.diceGroup);
							//}
							edgeRollValue = topNumber;
						}
						else {
							this.removeNonVantageDieNow(die);
							//if (!localDiceRollData.showedVantageMessage) {
							//	localDiceRollData.showedVantageMessage = true;
							const centerPos: Vector = this.getScreenCoordinates(die);
							diceLayer.addDisadvantageText(centerPos, vantageTextDelay, false, this.diceRollData.diceGroup);
							//}
						}
					}
					otherDie = die;
				}
			}
		}

		if (isLuckRoll) {
			edgeRollValue = -1;
			otherDie = null;
			for (let i = 0; i < this.dice.length; i++) {
				const die: IDie = this.dice[i];
				if (!die.inPlay)
					continue;
				const topNumber: number = die.getTopNumber();

				if (die.isD20) {
					if (edgeRollValue === -1)
						edgeRollValue = topNumber;
					else if (isLuckRollHigh) {
						if (edgeRollValue <= topNumber) {
							this.removeNonVantageDieNow(otherDie);
							//if (!localDiceRollData.showedVantageMessage) {
							//	localDiceRollData.showedVantageMessage = true;
							if (!otherDie.isLucky) {
								const centerPos: Vector = this.getScreenCoordinates(otherDie);
								diceLayer.addAdvantageText(centerPos, vantageTextDelay, true, this.diceRollData.diceGroup);
							}
							//}
							edgeRollValue = topNumber;
						}
						else {
							this.removeNonVantageDieNow(die);
							//if (!localDiceRollData.showedVantageMessage) {
							//	localDiceRollData.showedVantageMessage = true;
							if (!die.isLucky) {
								const centerPos: Vector = this.getScreenCoordinates(die);
								diceLayer.addAdvantageText(centerPos, vantageTextDelay, true, this.diceRollData.diceGroup);
							}
							//}
						}
					}
					else if (isLuckRollLow) {
						if (edgeRollValue >= topNumber) {
							this.removeNonVantageDieNow(otherDie);
							//if (!localDiceRollData.showedVantageMessage) {
							//	localDiceRollData.showedVantageMessage = true;
							if (!otherDie.isLucky) {
								const centerPos: Vector = this.getScreenCoordinates(otherDie);
								diceLayer.addDisadvantageText(centerPos, vantageTextDelay, true, this.diceRollData.diceGroup);
							}
							//}
							edgeRollValue = topNumber;
						}
						else {
							this.removeNonVantageDieNow(die);
							//if (!localDiceRollData.showedVantageMessage) {
							//	localDiceRollData.showedVantageMessage = true;
							if (!die.isLucky) {
								const centerPos: Vector = this.getScreenCoordinates(die);
								diceLayer.addDisadvantageText(centerPos, vantageTextDelay, true, this.diceRollData.diceGroup);
							}
							//}
						}
					}
					otherDie = die;
				}
			}
		}
		return edgeRollValue;
	}

	showSpecialLabels() {
		/*
				1 / Acid
				2 / Cold
				3 / Fire
				4 / Force
				5 / Lightning
				6 / Poison
				7 / Psychic
				8 / Thunder 
		*/

		let changedMessage = false;
		if (this.diceRollData.type === DiceRollType.ChaosBolt) {
			changedMessage = this.showChaosBoltLabels(changedMessage);
		}
	}

	static readonly bubbleId: string = 'bubble';

	private showChaosBoltLabels(changedMessage: boolean) {
		for (let i = 0; i < this.dice.length; i++) {
			const die: IDie = this.dice[i];
			if (die.rollType === DieCountsAs.damage && die.attachedDamage !== true && die.values === 8) {
				const topNumber = die.getTopNumber();
				let message = '';
				switch (topNumber) {
					case 1:
						message = 'Acid';
						diceLayer.attachDamageAcid(die);
						break;
					case 2:
						message = 'Cold';
						diceLayer.attachDamageCold(die);
						break;
					case 3:
						message = 'Fire';
						diceLayer.attachDamageFire(die);
						break;
					case 4:
						message = 'Force';
						diceLayer.attachDamageForce(die);
						break;
					case 5:
						message = 'Lightning';
						diceLayer.attachDamageLightning(die);
						break;
					case 6:
						message = 'Poison';
						diceLayer.attachDamagePoison(die);
						break;
					case 7:
						message = 'Psychic';
						diceLayer.attachDamagePsychic(die);
						break;
					case 8:
						message = 'Thunder';
						diceLayer.attachDamageThunder(die);
						break;
				}
				if (message) {
					if (this.additionalDieRollMessage)
						this.additionalDieRollMessage += ', ';
					else
						this.additionalDieRollMessage = '(';
					changedMessage = true;
					this.additionalDieRollMessage += message;
					const centerPos: Vector = this.getScreenCoordinates(die);
					diceLayer.addDieTextAfter(centerPos, message, diceLayer.activePlayerDieColor, diceLayer.activePlayerDieFontColor, 900, 7000, 1, this.diceRollData.diceGroup);
				}
			}
		}
		if (this.additionalDieRollMessage && changedMessage)
			this.additionalDieRollMessage += ')';
		return changedMessage;
	}

	markBubble(sprite: SpriteProxy) {
		if (!(sprite.data instanceof DieSpriteData))
			sprite.data = new DieSpriteData();

		sprite.data.bubbleId = DieRoller.bubbleId;
	}

	isBubble(sprite: SpriteProxy): boolean {
		if (sprite.data instanceof DieSpriteData)
			return sprite.data.bubbleId === DieRoller.bubbleId;
		return false;
	}

	popFrozenDice() {
		for (let i = 0; i < this.dice.length; i++) {
			const die: IDie = this.dice[i];
			if (!die.inPlay)
				continue;
			for (let j = 0; j < die.attachedSprites.length; j++) {
				const sprite: SpriteProxy = die.attachedSprites[j];
				if (this.isBubble(sprite)) {
					sprite.expirationDate = performance.now();

					let hueShift = 0;
					if (die.playerID >= 0)
						hueShift = diceLayer.getHueShift(die.playerID);

					die.attachedSprites[j] = diceLayer.addFeezePop(sprite.x, sprite.y, hueShift, 100, 100);
					die.origins[j] = new Vector(diceLayer.freezePop.originX, diceLayer.freezePop.originY);
					diceSounds.safePlayMp3('ice/crack[3]');
				}
			}
		}
	}

	modifyTotalRollForTestingPurposes() {
		//d20RollValue = 20; // critical hit.
		//totalRoll = 20;
		//totalRoll = 35; // age change
	}

	getModifier(diceRollData: DiceRollData, player: Character): number {
		if (player)
			if (diceRollData.type === DiceRollType.Initiative)
				return player.initiative;
			else if (diceRollData.type === DiceRollType.SkillCheck) {
				return player.getSkillMod(diceRollData.skillCheck);
			}
			else if (diceRollData.type === DiceRollType.SavingThrow) {
				return player.getSavingThrowMod(diceRollData.savingThrow);
			}
		return 0;
	}

	getMaxDamage() {
		let maxDamage: number;
		for (let i = 0; i < this.dice.length; i++) {
			const die: IDie = this.dice[i];
			if (die.rollType === DieCountsAs.damage)
				maxDamage += die.values;
		}
		return maxDamage;
	}

	//function initializeForScoring() {
	//	for (let i = 0; i < dice.length; i++) {
	//		const die: IDie = dice[i];
	//		die.scoreHasBeenTallied = false;
	//		// Workaround for state bug where multiple instances of the same die can apparently get inside the dice array.
	//	}
	//}

	addDieToMultiPlayerSummary(die: IDie, topNumber: number) {
		const logProgress = true;
		if (this.diceRollData.multiplayerSummary === null)
			this.diceRollData.multiplayerSummary = [];
		// diceRollData.multiplayerSummary.find((value, index, obj) => value.id === die.playerID);
		const playerRoll: PlayerRoll = this.diceRollData.multiplayerSummary.find((value: PlayerRoll, index, obj) => value.name === die.playerName);

		if (playerRoll) {
			playerRoll.roll += topNumber;
			playerRoll.success = playerRoll.roll + playerRoll.modifier >= this.diceRollData.hiddenThreshold;
			if (logProgress) {
				console.log(`Found playerRoll for ${die.playerName}. Roll: ${playerRoll.roll}`);
			}
		}
		else {
			if (logProgress) {
				console.log(`playerRoll not found for ${die.playerName}.`);
			}

			let modifier = 0;
			if (die.playerID < 0) {
				if (logProgress) {
					console.log(`It's an in-game creature.`);
				}
				for (let i = 0; i < this.diceRollData.diceDtos.length; i++) {
					if (this.diceRollData.diceDtos[i].CreatureId === die.playerID) {
						if (logProgress) {
							console.log(`Found modifier.`);
						}
						modifier = this.diceRollData.diceDtos[i].Modifier;
						//break;
					}
				}
			}
			else if (diceLayer.players && diceLayer.players.length > 0) {
				const player: Character = diceLayer.getPlayer(die.playerID);
				modifier = this.getModifier(this.diceRollData, player);
			}

			const success: boolean = topNumber + modifier >= this.diceRollData.hiddenThreshold;
			let critHit: boolean;
			if (this.diceRollData && this.diceRollData.minCrit)
				critHit = topNumber >= this.diceRollData.minCrit;
			else
				critHit = topNumber >= 20;
			const critFail: boolean = topNumber === 1;
			this.diceRollData.multiplayerSummary.push(new PlayerRoll(topNumber, die.playerName, die.playerID, die.dataStr, modifier, success, critHit, critFail));
			if (logProgress) {
				console.log(this.diceRollData.multiplayerSummary[0].roll);
			}
		}

		if (logProgress) {
			console.log(this.diceRollData.multiplayerSummary);
		}
	}

	attachDieLabel(die: IDie) {
		let scaleAdjust = 1;
		if (die.rollType === DieCountsAs.inspiration) {
			scaleAdjust = 0.7;
		}

		let labelAlreadyExists = false;
		if (die.attachedLabels)
			die.attachedLabels.forEach((label: TextEffect) => {
				if (label.text === die.playerName) {
					labelAlreadyExists = true;
					return;
				}
			});

		if (labelAlreadyExists)
			return;

		if (!die.playerName) {
			die.playerName = diceLayer.getPlayerName(die.playerID);
		}

		const centerPos: Vector = this.getScreenCoordinates(die);
		diceLayer.addDieTextAfter(centerPos, die.playerName, diceLayer.getDieColor(die.playerID), diceLayer.activePlayerDieFontColor, 0, 8000, scaleAdjust, this.diceRollData.diceGroup);
	}

	getRollResults(tallyResults: boolean): RollResults {
		const logProgress = false;
		const totalScores: Map<number, number> = new Map<number, number>();
		const inspirationValue: Map<number, number> = new Map<number, number>();
		const luckValue: Map<number, number> = new Map<number, number>();
		const damageMap: Map<DamageType, number> = new Map<DamageType, number>();
		let totalDamage = 0;
		let totalHealth = 0;
		let totalExtra = 0;
		this.diceRollData.totalRoll = 0;

		let singlePlayerId = 0;
		let playerIdForTextMessages = -1;

		const initializeLocalArrays = (playerID: number) => {
			if (!totalScores.get(playerID))
				totalScores.set(playerID, 0);

			if (!inspirationValue.get(playerID))
				inspirationValue.set(playerID, 0);

			if (!luckValue.get(playerID))
				luckValue.set(playerID, 0);
		}

		const tallyTotals = (playerID: number, die: IDie, topNumber: number) => {
			initializeLocalArrays(playerID);
			if (logProgress) {
				console.log(`tallyTotals, Before - totalScores.get(${playerID}): ` + totalScores.get(playerID));
			}

			let extraDamage: number = this.damageModifierThisRoll;
			if (logProgress) {
				console.log('die.rollType: ' + DieCountsAs[die.rollType]);
			}
			switch (die.rollType) {
				case DieCountsAs.totalScore:
					if (logProgress) {
						console.log(`DieCountsAs.totalScore (${topNumber})`);
					}
					if (this.diceRollData.type === DiceRollType.WildMagicD20Check) {
						if (topNumber === 1)
							totalScores.set(playerID, 1);
						else if (!totalScores.get(playerID))
							totalScores.set(playerID, 0);
					}
					else
						totalScores.set(playerID, totalScores.get(playerID) + topNumber);
					break;
				case DieCountsAs.inspiration:
					if (logProgress) {
						console.log(`DieCountsAs.inspiration (${topNumber})`);
					}
					inspirationValue.set(playerID, inspirationValue.get(playerID) + topNumber);
					break;
				case DieCountsAs.bentLuck:
					if (logProgress) {
						console.log(`DieCountsAs.bentLuck (${topNumber * this.diceRollData.bentLuckMultiplier})`);
					}
					luckValue.set(playerID, luckValue.get(playerID) + topNumber * this.diceRollData.bentLuckMultiplier);
					break;
				case DieCountsAs.bonus:
					if (logProgress) {
						console.log(`DieCountsAs.bonus (${topNumber})`);
					}
					this.totalBonus += topNumber;
					break;
				case DieCountsAs.damage:
					if (logProgress) {
						console.log(`DieCountsAs.damage (${topNumber})`);
					}
					{
						const damageType: DamageType = die.damageType || DamageType.None;

						const currentDamage: number = damageMap.get(damageType);
						if (currentDamage) {
							//console.log(`New damage for ${DamageType[damageType]}: ${currentDamage + topNumber}`);
							damageMap.set(damageType, currentDamage + topNumber);
						}
						else {
							//console.log(`New damage for ${DamageType[damageType]}: ${topNumber}`);
							damageMap.set(damageType, topNumber + extraDamage);
							extraDamage = 0;
						}

						totalDamage += topNumber;
						break;
					}
				case DieCountsAs.health:
					if (logProgress) {
						console.log(`DieCountsAs.health (${topNumber})`);
					}
					totalHealth += topNumber;
					break;
				case DieCountsAs.extra:
					if (logProgress) {
						console.log(`DieCountsAs.extra (${topNumber})`);
					}
					totalExtra += topNumber;
					break;
				default:
					if (logProgress) {
						console.log(`DieCountsAs.extra (${topNumber})`);
					}
			}

			if (logProgress) {
				console.log(`tallyTotals, After - totalScores.get(${playerID}): ` + totalScores.get(playerID));
			}
		}

		if (logProgress) {
			console.log('diceRollData.hasMultiPlayerDice: ' + this.diceRollData.hasMultiPlayerDice);
			console.log(`getRollResults: dice.length = ${this.dice.length}`);
		}

		const maxDamage: number = this.getMaxDamage();

		for (let i = 0; i < this.dice.length; i++) {
			const die: IDie = this.dice[i];
			if (!die.inPlay) {
				if (logProgress) {
					console.log(`die is not in play.`);
				}
				continue;
			}

			if (die.scoreHasBeenTallied) {
				if (logProgress) {
					console.log(`scoreHasBeenTallied`);
				}
				continue;
			}

			if (tallyResults) {
				die.scoreHasBeenTallied = true;
				//console.log(`dice[${i}].scoreHasBeenTallied = true`);
			}

			const topNumber: number = die.getTopNumber();

			if (this.diceRollData.hasMultiPlayerDice) {
				this.addDieToMultiPlayerSummary(die, topNumber);
				this.attachDieLabel(die);
			}

			this.diceRollData.individualRolls.push(new IndividualRoll(topNumber, die.values, die.dieType, die.damageType));

			const playerID: number = die.playerID;
			if (logProgress) {
				console.log('playerID: ' + playerID);
			}
			if (playerID === undefined) {
				this.diceRollData.totalRoll += topNumber;
				continue;
			}

			if (this.diceRollData.hasSingleIndividual) {
				singlePlayerId = playerID;
				playerIdForTextMessages = playerID;
			}

			tallyTotals(playerID, die, topNumber);
		} // for

		let skillSavingModifier = 0;
		if (this.diceRollData.type === DiceRollType.SkillCheck || this.diceRollData.type === DiceRollType.SavingThrow) {
			const player: Character = diceLayer.getPlayer(singlePlayerId);
			skillSavingModifier = this.getModifier(this.diceRollData, player);
		}

		let toHitModifier = 0;
		if (inspirationValue.has(singlePlayerId))
			toHitModifier += inspirationValue.get(singlePlayerId);
		if (luckValue.has(singlePlayerId))
			toHitModifier += luckValue.get(singlePlayerId);
		if (this.diceRollData.modifier)
			toHitModifier += this.diceRollData.modifier;
		if (skillSavingModifier)
			toHitModifier += skillSavingModifier;

		if (logProgress) {
			console.log('toHitModifier: ' + toHitModifier);
			console.log('diceRollData.hasMultiPlayerDice: ' + this.diceRollData.hasMultiPlayerDice);
		}

		if (totalScores.get(singlePlayerId))
			this.diceRollData.totalRoll += totalScores.get(singlePlayerId) + toHitModifier;
		else {
			this.diceRollData.totalRoll += toHitModifier;
			if (logProgress) {
				console.log(`totalScores does not contain singlePlayerId (${singlePlayerId})`);
			}
		}

		if (!this.diceRollData.hasMultiPlayerDice && totalScores.get(singlePlayerId) > 0) {
			if (this.diceRollData.type === DiceRollType.SkillCheck && this.totalBonus)
				this.diceRollData.totalRoll += this.totalBonus;
		}

		this.modifyTotalRollForTestingPurposes();

		if (logProgress) {
			console.log('diceRollData.totalRoll: ' + this.diceRollData.totalRoll);
		}
		this.attemptedRollWasSuccessful = this.diceRollData.totalRoll >= this.diceRollData.hiddenThreshold;
		if (logProgress) {
			console.log(`attemptedRollWasSuccessful: ${this.attemptedRollWasSuccessful} (totalRoll = ${this.diceRollData.totalRoll}, diceRollData.hiddenThreshold = ${this.diceRollData.hiddenThreshold})`);
		}
		this.attemptedRollWasNarrowlySuccessful = this.attemptedRollWasSuccessful && (this.diceRollData.totalRoll - this.diceRollData.hiddenThreshold < 2);
		if (logProgress) {
			console.log('damageModifierThisRoll: ' + this.damageModifierThisRoll);
		}
		this.totalDamagePlusModifier = totalDamage + this.damageModifierThisRoll;
		this.totalHealthPlusModifier = totalHealth + this.healthModifierThisRoll;
		this.totalExtraPlusModifier = totalExtra + this.extraModifierThisRoll;

		return new RollResults(totalScores, toHitModifier, singlePlayerId, luckValue, playerIdForTextMessages, skillSavingModifier, totalDamage, totalHealth, totalExtra, maxDamage, damageMap);
	}

	bonusRollDealsDamage(damageStr: string, description = '', playerID = -1): void {
		const bonusRoll: BonusRoll = this.diceRollData.addBonusRoll(damageStr, description, playerID, DiceLayer.damageDieBackgroundColor, DiceLayer.damageDieFontColor);
		bonusRoll.dieCountsAs = DieCountsAs.damage;
	}

	getFirstPlayerId() {
		let playerID = -1;
		if (this.diceRollData.playerRollOptions.length > 0)
			playerID = this.diceRollData.playerRollOptions[0].PlayerID;
		return playerID;
	}

	anyDamageDiceThisRoll(): boolean {
		for (let i = 0; i < this.dice.length; i++) {
			const die: IDie = this.dice[i];
			if (die.rollType === DieCountsAs.damage)
				return true;
		}
		return false;
	}

	checkAttackBonusRolls() {
		if (this.isAttack(this.diceRollData)) {
			this.wasCriticalHit = this.d20RollValue >= this.diceRollData.minCrit;
			//console.log('isCriticalHit: ' + isCriticalHit);
			if (this.wasCriticalHit && !this.diceRollData.secondRollData && this.anyDamageDiceThisRoll()) {
				//console.log('diceRollData.damageHealthExtraDice: ' + diceRollData.damageHealthExtraDice);
				this.bonusRollDealsDamage(this.diceRollData.damageHealthExtraDice, '', this.getFirstPlayerId());
				//console.log('checkAttackBonusRolls(1) - Roll Bonus Dice: ' + diceRollData.bonusRolls.length);
			}
			//console.log('Calling getRollResults() from checkAttackBonusRolls...');
			this.getRollResults(false);  // Needed to set globals for code below. I know. It's not great.
			if (this.attemptedRollWasSuccessful && this.diceRollData.minDamage > 0 && !this.diceRollData.secondRollData) {
				let extraRollStr = "";
				for (let i = 0; i < this.dice.length; i++) {
					const die: IDie = this.dice[i];
					if (die.rollType === DieCountsAs.damage && die.getTopNumber() < this.diceRollData.minDamage) {
						let damageStr = "";
						if (die.damageStr) {
							damageStr = `(${die.damageStr}:damage)`;
						}
						extraRollStr += `1d${die.values}${damageStr},`;
						this.removeDie(die, 0, DieEffect.HandGrab);
						const greatWeaponFightingTextDelay = 900;
						const centerPos: Vector = this.getScreenCoordinates(die);
						diceLayer.addDieTextAfter(centerPos, 'Great Weapon Fighting', diceLayer.activePlayerDieColor, diceLayer.activePlayerDieColor, greatWeaponFightingTextDelay, 1500, 1, this.diceRollData.diceGroup);
						die.inPlay = false;
						this.removeDieEffectsForSingleDie(die);
					}
				}
				if (extraRollStr.endsWith(',')) {
					extraRollStr = extraRollStr.substr(0, extraRollStr.length - 1);
				}
				if (extraRollStr) {
					//console.log('extraRollStr: ' + extraRollStr);
					this.bonusRollDealsDamage(extraRollStr, '', this.getFirstPlayerId());
					//console.log('checkAttackBonusRolls(2) - Roll Bonus Dice: ' + diceRollData.bonusRolls.length);
				}
			}
		}
	}

	nat20SkillCheckBonusRoll: boolean;

	checkSkillCheckBonusRolls(): void {
		this.nat20SkillCheckBonusRoll = false;
		if (this.diceRollData.type === DiceRollType.SkillCheck) {
			for (let i = 0; i < this.dice.length; i++) {
				const die: IDie = this.dice[i];
				if (die.inPlay && die.rollType === DieCountsAs.totalScore && die.isD20 && die.getTopNumber() === 20) {
					this.nat20SkillCheckBonusRoll = true;
					let dieColor: string = diceLayer.activePlayerDieColor;
					let dieTextColor: string = diceLayer.activePlayerDieFontColor;
					if (die.playerID >= 0) {
						dieColor = diceLayer.getDieColor(die.playerID);
						dieTextColor = diceLayer.getDieFontColor(die.playerID);
					}
					this.diceRollData.addBonusRoll('1d20', '', die.playerID, dieColor, dieTextColor, die.playerName);
				}
			}
		}
	}

	announceWildMagicResult(totalRoll: number) {
		let mp3BaseName = '99-00';
		if (totalRoll > 0 && totalRoll < 99) {
			const secondNumber: number = Math.floor((totalRoll + 1) / 2) * 2;
			const firstNumber: number = secondNumber - 1;
			let firstStr: string = firstNumber.toString();
			if (firstNumber < 10)
				firstStr = '0' + firstStr;
			let secondStr: string = secondNumber.toString();
			if (secondNumber < 10)
				secondStr = '0' + secondStr;
			mp3BaseName = firstStr + '-' + secondStr;
		}
		diceSounds.playMp3In(1800, 'Announcer/Wild Magic/' + mp3BaseName);
		diceSounds.safePlayMp3('WildMagic/' + mp3BaseName);
	}

	checkWildMagicBonusRolls() {
		if (this.diceRollData.type === DiceRollType.WildMagic) {
			let rollValue = 0;
			for (let i = 0; i < this.dice.length; i++) {
				const die: IDie = this.dice[i];
				if (die.inPlay && (die.rollType === DieCountsAs.totalScore || die.rollType === DieCountsAs.inspiration))
					rollValue += die.getTopNumber();
			}
			this.diceRollData.totalRoll = rollValue + this.diceRollData.modifier;
			this.modifyTotalRollForTestingPurposes();
			this.diceRollData.playBonusSoundAfter = 2500;
			this.announceWildMagicResult(this.diceRollData.totalRoll);
			if (this.diceRollData.totalRoll === 0 || this.diceRollData.totalRoll === 99)
				this.diceRollData.wildMagic = WildMagic.regainSorceryPoints;
			else if (this.diceRollData.totalRoll < 3)
				this.diceRollData.wildMagic = WildMagic.wildMagicMinute;
			else if (this.diceRollData.totalRoll < 5)
				this.diceRollData.wildMagic = WildMagic.seeInvisibleCreatures;
			else if (this.diceRollData.totalRoll < 7)
				this.diceRollData.wildMagic = WildMagic.modronAppearsOneMinute;
			else if (this.diceRollData.totalRoll < 9)
				this.diceRollData.wildMagic = WildMagic.castFireball;
			else if (this.diceRollData.totalRoll < 11)
				this.diceRollData.wildMagic = WildMagic.castMagicMissile;
			else if (this.diceRollData.totalRoll < 13) {
				this.diceRollData.addBonusRoll('1d10', 'Inches Changed: ');
				this.diceRollData.wildMagic = WildMagic.heightChange;
				this.diceRollData.playBonusSoundAfter = 700;
			}
			else if (this.diceRollData.totalRoll < 15)
				this.diceRollData.wildMagic = WildMagic.castConfusionOnSelf;
			else if (this.diceRollData.totalRoll < 17)
				this.diceRollData.wildMagic = WildMagic.regain5hpPerTurnForOneMinute;
			else if (this.diceRollData.totalRoll < 19)
				this.diceRollData.wildMagic = WildMagic.beardOfFeathers;
			else if (this.diceRollData.totalRoll < 21)
				this.diceRollData.wildMagic = WildMagic.castGreaseCenteredOnSelf;
			else if (this.diceRollData.totalRoll < 23)
				this.diceRollData.wildMagic = WildMagic.spellTargetsDisadvantagedSavingThrowForOneMinute;
			else if (this.diceRollData.totalRoll < 25)
				this.diceRollData.wildMagic = WildMagic.skinTurnsBlue;
			else if (this.diceRollData.totalRoll < 27)
				this.diceRollData.wildMagic = WildMagic.thirdEyeAdvantageWisdomChecks;
			else if (this.diceRollData.totalRoll < 29)
				this.diceRollData.wildMagic = WildMagic.castTimeBonusActionOneMinute;
			else if (this.diceRollData.totalRoll < 31)
				this.diceRollData.wildMagic = WildMagic.teleportUpTo60Feet;
			else if (this.diceRollData.totalRoll < 33)
				this.diceRollData.wildMagic = WildMagic.astralPlaneUntilEndOfNextTurn;
			else if (this.diceRollData.totalRoll < 35)
				this.diceRollData.wildMagic = WildMagic.maximizeDamageOnSpellCastInNextMinute;
			else if (this.diceRollData.totalRoll < 37) {
				this.diceRollData.addBonusRoll('1d10', 'Years Changed: ');
				this.diceRollData.wildMagic = WildMagic.ageChange;
				this.diceRollData.playBonusSoundAfter = 700;
			}
			else if (this.diceRollData.totalRoll < 39) {
				this.diceRollData.addBonusRoll('1d6', 'Flumphs: ');
				this.diceRollData.wildMagic = WildMagic.flumphs;
			}
			else if (this.diceRollData.totalRoll < 41) {
				this.diceRollData.addBonusRoll('2d10', 'HP Regained: ');
				this.diceRollData.wildMagic = WildMagic.regainHitPoints;
			}
			else if (this.diceRollData.totalRoll < 43)
				this.diceRollData.wildMagic = WildMagic.pottedPlant;
			else if (this.diceRollData.totalRoll < 45)
				this.diceRollData.wildMagic = WildMagic.teleportUpTo20FeetBonusActionOneMinute;
			else if (this.diceRollData.totalRoll < 47)
				this.diceRollData.wildMagic = WildMagic.castLevitateOnSelf;
			else if (this.diceRollData.totalRoll < 49)
				this.diceRollData.wildMagic = WildMagic.unicorn;
			else if (this.diceRollData.totalRoll < 51)
				this.diceRollData.wildMagic = WildMagic.cannotSpeakPinkBubbles;
			else if (this.diceRollData.totalRoll < 53)
				this.diceRollData.wildMagic = WildMagic.spectralShieldPlus2ArmorClassNextMinute;
			else if (this.diceRollData.totalRoll < 55) {
				this.diceRollData.addBonusRoll('5d6', 'Days Immune: ');
				this.diceRollData.wildMagic = WildMagic.alcoholImmunity;
			}
			else if (this.diceRollData.totalRoll < 57)
				this.diceRollData.wildMagic = WildMagic.hairFallsOutGrowsBack24Hours;
			else if (this.diceRollData.totalRoll < 59)
				this.diceRollData.wildMagic = WildMagic.fireTouchOneMinute;
			else if (this.diceRollData.totalRoll < 61)
				this.diceRollData.wildMagic = WildMagic.regainLowestLevelExpendedSpellSlot;
			else if (this.diceRollData.totalRoll < 63)
				this.diceRollData.wildMagic = WildMagic.shoutWhenSpeakingOneMinute;
			else if (this.diceRollData.totalRoll < 65)
				this.diceRollData.wildMagic = WildMagic.castFogCloudCenteredOnSelf;
			else if (this.diceRollData.totalRoll < 67) {
				this.diceRollData.wildMagic = WildMagic.lightningDamageUpToThreeCreatures;
				this.diceRollData.addBonusDamageRoll('4d10(lightning)', 'Lightning Damage: ');
			}
			else if (this.diceRollData.totalRoll < 69)
				this.diceRollData.wildMagic = WildMagic.frightenedByNearestCreatureUntilEndOfNextTurn;
			else if (this.diceRollData.totalRoll < 71)
				this.diceRollData.wildMagic = WildMagic.allCreatures30FeetInvisibleOneMinute;
			else if (this.diceRollData.totalRoll < 73)
				this.diceRollData.wildMagic = WildMagic.resistanceToAllDamageNextMinute;
			else if (this.diceRollData.totalRoll < 75) {
				this.diceRollData.addBonusRoll('1d4', 'Hours Poisoned: ');
				this.diceRollData.wildMagic = WildMagic.randomCreaturePoisoned1d4Hours;
			}
			else if (this.diceRollData.totalRoll < 77)
				this.diceRollData.wildMagic = WildMagic.glowBrightOneMinuteCreaturesEndingTurn5FeetBlinded;
			else if (this.diceRollData.totalRoll < 79)
				this.diceRollData.wildMagic = WildMagic.castPolymorphToSheepOnSelf;
			else if (this.diceRollData.totalRoll < 81)
				this.diceRollData.wildMagic = WildMagic.butterfliesAndPetals10FeetOneMinute;
			else if (this.diceRollData.totalRoll < 83)
				this.diceRollData.wildMagic = WildMagic.takeOneAdditionalActionImmediately;
			else if (this.diceRollData.totalRoll < 85) {
				this.diceRollData.addBonusDamageRoll('1d10(necrotic)', 'Necrotic Damage: ');
				this.diceRollData.wildMagic = WildMagic.allCreaturesWithin30FeetTake1d10NecroticDamage;
			}
			else if (this.diceRollData.totalRoll < 87)
				this.diceRollData.wildMagic = WildMagic.castMirrorImage;
			else if (this.diceRollData.totalRoll < 89)
				this.diceRollData.wildMagic = WildMagic.castFlyOnRandomCreatureWithin60Feet;
			else if (this.diceRollData.totalRoll < 91)
				this.diceRollData.wildMagic = WildMagic.invisibleSilentNextMinute;
			else if (this.diceRollData.totalRoll < 93)
				this.diceRollData.wildMagic = WildMagic.immortalOneMinute;
			else if (this.diceRollData.totalRoll < 95)
				this.diceRollData.wildMagic = WildMagic.increaseSizeOneMinute;
			else if (this.diceRollData.totalRoll < 97)
				this.diceRollData.wildMagic = WildMagic.allCreatures30FeetVulnerableToPiercingDamageOneMinute;
			else if (this.diceRollData.totalRoll < 99)
				this.diceRollData.wildMagic = WildMagic.faintEtheralMusicOneMinute;
		}
	}

	needToRollBonusDice() {
		if (this.onBonusThrow || this.setupBonusRoll)
			return false;

		this.modifyTotalRollForTestingPurposes();
		this.checkAttackBonusRolls();
		this.checkSkillCheckBonusRolls();
		this.checkWildMagicBonusRolls();

		return this.diceRollData.bonusRolls && this.diceRollData.bonusRolls.length > 0;
	}

	freezeDie(die: IDie) {
		const dieObject: IDieObject = die.getObject();
		if (!dieObject)
			return;
		const body = dieObject.body;
		body.mass = 0;
		body.updateMassProperties();
		body.velocity.set(0, 0, 0);
		body.angularVelocity.set(0, 0, 0);
	}

	freezeExistingDice() {
		for (let i = 0; i < this.dice.length; i++) {
			const die: IDie = this.dice[i];
			this.freezeDie(die);
			if (die.attachedSprites && die.inPlay) {
				let hueShift = 0;
				if (die.playerID >= 0)
					hueShift = diceLayer.getHueShift(die.playerID);
				else
					hueShift = diceLayer.activePlayerHueShift;

				const bubble: SpriteProxy = diceLayer.addFreezeBubble(960, 540, hueShift, 100, 100);

				this.markBubble(bubble);
				die.attachedSprites.push(bubble);
				diceSounds.safePlayMp3('ice/Freeze[5]');
				die.origins.push(new Vector(diceLayer.freeze.originX, diceLayer.freeze.originY));
			}
		}
	}

	damageTypeFromStr(str: string): DamageType {
		str = str.toLowerCase();
		if (str === 'acid')
			return DamageType.Acid;

		if (str === 'bludgeoning')
			return DamageType.Bludgeoning;

		if (str === 'cold')
			return DamageType.Cold;

		if (str === 'fire')
			return DamageType.Fire;

		if (str === 'force')
			return DamageType.Force;

		if (str === 'lightning')
			return DamageType.Lightning;

		if (str === 'superiority')
			return DamageType.Superiority;

		if (str === 'necrotic')
			return DamageType.Necrotic;

		if (str === 'piercing')
			return DamageType.Piercing;

		if (str === 'poison')
			return DamageType.Poison;

		if (str === 'psychic')
			return DamageType.Psychic;

		if (str === 'radiant')
			return DamageType.Radiant;

		if (str === 'slashing')
			return DamageType.Slashing;

		if (str === 'thunder')
			return DamageType.Thunder;

		return DamageType.None;
	}

	prepareBaseDie(diceGroup: DiceGroup, die: IDie, throwPower: number, xPositionModifier = 0) {
		const dieObject: IDieObject = die.getObject();
		this.scene.add(dieObject);
		//console.log(`prepareBaseDie - die.inPlay = true;`);
		die.inPlay = true;
		die.attachedSprites = [];
		die.attachedLabels = [];
		die.group = diceGroup;
		die.origins = [];
		this.dice.push(die);

		die.hasNotHitFloorYet = true;

		const index: number = this.dice.length;
		const yVelocityModifier: number = Math.random() * 10 * throwPower;
		let zVelocityMultiplier = 1;

		const dieScale: number = DieRoller.dieScale * die.scale;

		if (diceGroup === DiceGroup.Viewers) {
			xPositionModifier = 40; // throw from the right.
			dieObject.position.y = 10 + index * dieScale;
			dieObject.position.z = 9 + index * dieScale;
			zVelocityMultiplier = -0.1;
		}
		else {  // Players
			dieObject.position.y = 4 + Math.floor(index / 3) * dieScale;
			dieObject.position.z = -13 + (index % 3) * dieScale;
		}

		dieObject.position.x = xPositionModifier + -15 - (index % 3) * dieScale;

		dieObject.quaternion.x = (Math.random() * 180 - 90) * Math.PI / 180;
		dieObject.quaternion.z = (Math.random() * 180 - 90) * Math.PI / 180;
		let xVelocityMultiplier = 1;
		if (xPositionModifier !== 0)
			xVelocityMultiplier = -1;
		die.updateBodyFromMesh();
		const xVelocityModifier: number = Math.random() * 20 * throwPower;
		const zVelocityModifier: number = Math.random() * 20 * throwPower;
		dieObject.body.velocity.set(xVelocityMultiplier * (35 + xVelocityModifier), 10 + yVelocityModifier, zVelocityMultiplier * (25 + zVelocityModifier));
		const angularModifierLimit: number = 20 * throwPower;
		const angularModifierHalfLimit: number = angularModifierLimit / 2;
		const xAngularRotationModifier: number = Math.random() * angularModifierLimit;
		const yAngularRotationModifier: number = Math.random() * angularModifierLimit;
		const zAngularRotationModifier: number = Math.random() * angularModifierLimit;
		dieObject.body.angularVelocity.set(xAngularRotationModifier - angularModifierHalfLimit, yAngularRotationModifier - angularModifierHalfLimit, zAngularRotationModifier - angularModifierHalfLimit);
		dieObject.body.name = 'die';
		dieObject.body.addEventListener("collide", this.handleDieCollision.bind(this));
		die.lastPos = [];
		die.lastPos.push(new Vector(-100, -100));
		die.lastPrintOnLeft = false;
	}

	prepareDie(diceGroup: DiceGroup, die: IDie, throwPower: number, xPositionModifier = 0) {
		this.prepareBaseDie(diceGroup, die, throwPower, xPositionModifier);

		const newValue: number = Math.floor(Math.random() * die.values + 1);
		this.diceValues.push({ dice: die, value: newValue });
	}

	prepareD10x10Die(diceGroup: DiceGroup, die: IDie, throwPower: number, xPositionModifier = 0) {
		this.prepareBaseDie(diceGroup, die, throwPower, xPositionModifier);
		const newValue: number = Math.floor(Math.random() * die.values) + 1;
		this.diceValues.push({ dice: die, value: newValue });
	}

	prepareD10x01Die(diceGroup: DiceGroup, die: IDie, throwPower: number, xPositionModifier = 0) {
		this.prepareBaseDie(diceGroup, die, throwPower, xPositionModifier);
		const newValue: number = Math.floor(Math.random() * die.values) + 1;
		this.diceValues.push({ dice: die, value: newValue });
	}

	attachLabel(die: IDie, textColor: string, backgroundColor: string) {
		if (die.dieType && die.dieType.startsWith('"')) {
			diceLayer.attachLabel(die, die.dieType, textColor, backgroundColor, 1, this.diceRollData.diceGroup);
		}
	}

	addInspirationParticles(die: IDie, playerID: number, hueShiftOffset: number, rotationDegeesPerSecond: number) {
		die.attachedSprites.push(diceLayer.addInspirationParticles(die, 960, 540, rotationDegeesPerSecond, diceLayer.getHueShift(playerID) + hueShiftOffset));
		die.origins.push(diceLayer.inspirationParticles.getOrigin());
	}

	createDie(diceGroup: DiceGroup, quantity: number, numSides: number, damageType: DamageType, rollType: DieCountsAs, backgroundColor: string, textColor: string, throwPower = 1, xPositionModifier = 0, isMagic = false, playerID = -1, scale = 1, dieType = ''): IDie[] {
		const allDice: IDie[] = [];
		const magicRingHueShift: number = Math.floor(Math.random() * 360);
		const dieScale: number = DieRoller.dieScale * scale;
		//console.log('scale: ' + scale);
		//console.log('DieRoller.dieScale: ' + DieRoller.dieScale);
		//console.log('dieScale: ' + dieScale);
		for (let i = 0; i < quantity; i++) {
			let die: IDie = null;
			switch (numSides) {
				case 4:
					// @ts-ignore - DiceD4
					die = new DiceD4({ size: dieScale, backColor: backgroundColor, fontColor: textColor }, this.diceManager);
					break;
				case 6:
					// @ts-ignore - DiceD6
					die = new DiceD6({ size: dieScale, backColor: backgroundColor, fontColor: textColor }, this.diceManager);
					break;
				case 8:
					// @ts-ignore - DiceD8
					die = new DiceD8({ size: dieScale, backColor: backgroundColor, fontColor: textColor }, this.diceManager);
					break;
				case 10:
					// @ts-ignore - DiceD10
					die = new DiceD10({ size: dieScale, backColor: backgroundColor, fontColor: textColor }, this.diceManager);
					break;
				case 1001:
					// @ts-ignore - DiceD10x01
					die = new DiceD10x01({ size: dieScale, backColor: backgroundColor, fontColor: textColor }, this.diceManager);
					break;
				case 1010:
					// @ts-ignore - DiceD10x10
					die = new DiceD10x10({ size: dieScale, backColor: backgroundColor, fontColor: textColor }, this.diceManager);
					break;
				case 12:
					// @ts-ignore - DiceD12
					die = new DiceD12({ size: dieScale, backColor: backgroundColor, fontColor: textColor }, this.diceManager);
					break;
				case 20:
					// @ts-ignore - DiceD20
					die = new DiceD20({ size: dieScale, backColor: backgroundColor, fontColor: textColor }, this.diceManager);
					die.isD20 = true;
					break;
			}
			die.scale = scale;
			allDice.push(die);
			if (die === null) {
				throw new Error(`Die with ${numSides} sides was not found. Unable to throw dice.`);
			}
			die.dieType = dieType;
			die.playerID = playerID;
			this.prepareDie(diceGroup, die, throwPower, xPositionModifier);
			this.attachLabel(die, textColor, backgroundColor);
			if (die) {
				die.damageType = damageType;
				if (rollType === DieCountsAs.damage || rollType === DieCountsAs.bonus) {
					switch (damageType) {
						case DamageType.Fire:
							diceLayer.attachDamageFire(die);
							die.damageStr = 'fire';
							break;
						case DamageType.Cold:
							diceLayer.attachDamageCold(die);
							die.damageStr = 'cold';
							break;
						case DamageType.Necrotic:
							diceLayer.attachDamageNecrotic(die);
							die.damageStr = 'necrotic';
							break;
						case DamageType.Acid:
							diceLayer.attachDamageAcid(die);
							die.damageStr = 'acid';
							break;
						case DamageType.Piercing:
							diceLayer.attachDamagePiercing(die);
							die.damageStr = 'piercing';
							break;
						case DamageType.Radiant:
							diceLayer.attachDamageRadiant(die);
							die.damageStr = 'radiant';
							break;
						case DamageType.Poison:
							diceLayer.attachDamagePoison(die);
							die.damageStr = 'poison';
							break;
						case DamageType.Slashing:
							diceLayer.attachDamageSlashing(die);
							die.damageStr = 'slashing';
							break;
						case DamageType.Thunder:
							diceLayer.attachDamageThunder(die);
							die.damageStr = 'thunder';
							break;
						case DamageType.Force:
							diceLayer.attachDamageForce(die);
							die.damageStr = 'force';
							break;
						case DamageType.Psychic:
							diceLayer.attachDamagePsychic(die);
							die.damageStr = 'psychic';
							break;
						case DamageType.Bludgeoning:
							diceLayer.attachDamageBludgeoning(die);
							die.damageStr = 'bludgeoning';
							break;
						case DamageType.Lightning:
							diceLayer.attachDamageLightning(die);
							die.damageStr = 'lightning';
							break;
						case DamageType.Superiority:
							diceLayer.attachSuperiority(die);
							die.damageStr = 'superiority';
							break;
					}
				}
				else if ((rollType === DieCountsAs.health || rollType === DieCountsAs.totalScore || rollType === DieCountsAs.extra) && damageType === DamageType.Superiority) {
					diceLayer.attachSuperiority(die);
				}
				if (rollType === DieCountsAs.health) {
					diceLayer.attachHealth(die);
				}
			}
			die.rollType = rollType;
			if (die.rollType === DieCountsAs.inspiration) {
				const rotation: number = Random.between(4, 8);
				const hueShift: number = Random.between(20, 30);
				this.addInspirationParticles(die, playerID, hueShift, rotation);
				this.addInspirationParticles(die, playerID, -hueShift, -rotation);
				die.attachedSprites.push(diceLayer.addInspirationSmoke(die, 960, 540, Math.floor(Math.random() * 360)));
				die.origins.push(diceLayer.inspirationSmoke.getOrigin());
			}
			if (isMagic) {
				die.attachedSprites.push(diceLayer.addMagicRing(960, 540, die.scale, magicRingHueShift + Random.plusMinusBetween(10, 25)));
				die.origins.push(diceLayer.magicRingRed.getOrigin());
			}
		}
		return allDice;
	}

	addDie(diceGroup: DiceGroup, dieStr: string, damageType: DamageType, rollType: DieCountsAs, backgroundColor: string, textColor: string, throwPower = 1, xPositionModifier = 0, isMagic = false, playerID = -1, dieType = ''): IDie[] {
		const countPlusDie: string[] = dieStr.split('d');
		if (countPlusDie.length !== 2)
			throw new Error(`Issue with die format string: "${dieStr}". Unable to throw dice.`);
		let quantity = 1;
		if (countPlusDie[0])
			quantity = +countPlusDie[0];
		const numSides: number = +countPlusDie[1];
		return this.createDie(diceGroup, quantity, numSides, damageType, rollType, backgroundColor, textColor, throwPower, xPositionModifier, isMagic, playerID, 1, dieType);
	}

	addDieFromStr(diceGroup: DiceGroup, playerID: number, diceStr: string, dieCountsAs: DieCountsAs, throwPower: number, xPositionModifier = 0, backgroundColor: string = undefined, fontColor: string = undefined, isMagic = false): IDie[] {
		let lastDieAdded: IDie[] = null;
		if (!diceStr)
			return lastDieAdded;
		const allDice: string[] = diceStr.split(',');
		if (backgroundColor === undefined)
			backgroundColor = DiceLayer.damageDieBackgroundColor;
		if (fontColor === undefined)
			fontColor = DiceLayer.damageDieFontColor;
		let modifier = 0;
		let damageType: DamageType = this.diceRollData.damageType;

		allDice.forEach((dieSpec: string) => {
			let dieType = '';
			let thisBackgroundColor: string = backgroundColor;
			let thisFontColor: string = fontColor;
			let thisDieCountsAs: DieCountsAs = dieCountsAs;

			const parenIndex: number = dieSpec.indexOf('(');
			if (parenIndex >= 0) {
				let damageStr: string = dieSpec.substring(parenIndex + 1);
				const closeParenIndex: number = damageStr.indexOf(')');
				if (closeParenIndex >= 0) {
					damageStr = damageStr.substr(0, closeParenIndex);
					const colonIndex: number = damageStr.indexOf(':');
					if (colonIndex >= 0) {
						const rollTypeOverride: string = damageStr.substr(colonIndex + 1);
						const rollTypeOverrideLowerCase: string = rollTypeOverride.toLowerCase();
						//console.log('rollTypeOverride: ' + rollTypeOverride);
						if (rollTypeOverrideLowerCase === 'health') {
							thisDieCountsAs = DieCountsAs.health;
							thisBackgroundColor = DiceLayer.healthDieBackgroundColor;
							thisFontColor = DiceLayer.healthDieFontColor;
							isMagic = false;
						}
						else if (rollTypeOverrideLowerCase === 'damage') {
							thisDieCountsAs = DieCountsAs.damage;
							thisBackgroundColor = DiceLayer.damageDieBackgroundColor;
							thisFontColor = DiceLayer.damageDieFontColor;
							isMagic = false;
						}
						else if (rollTypeOverrideLowerCase === 'roll') {
							thisDieCountsAs = DieCountsAs.totalScore;
							thisBackgroundColor = diceLayer.activePlayerDieColor;
							thisFontColor = diceLayer.activePlayerDieFontColor;
						}
						damageStr = damageStr.substr(0, colonIndex);
						dieType = rollTypeOverride;
						//console.log('damageStr: ' + damageStr);
						damageType = this.damageTypeFromStr(damageStr);
					}
					else {
						damageType = this.damageTypeFromStr(damageStr);
						if (damageType === DamageType.None)
							dieType = damageStr;
					}
				}
				dieSpec = dieSpec.substr(0, parenIndex);
			}

			if (damageType !== DamageType.None)
				isMagic = false;  // No magic rings around damage die.

			const dieAndModifier = dieSpec.split('+');

			if (dieAndModifier.length === 2)
				modifier += +dieAndModifier[1];
			const dieStr: string = dieAndModifier[0];
			lastDieAdded = this.addDie(diceGroup, dieStr, damageType, thisDieCountsAs, thisBackgroundColor, thisFontColor, throwPower, xPositionModifier, isMagic, playerID, dieType);
		});

		this.damageModifierThisRoll += modifier;
		//console.log(`damageModifierThisRoll += modifier; (${damageModifierThisRoll})`);
		this.healthModifierThisRoll += modifier;
		this.extraModifierThisRoll += modifier;
		return lastDieAdded;
	}

	rollBonusDice() {
		this.onBonusThrow = true;

		if (!this.diceRollData.bonusRolls)
			return;
		//console.log('rollBonusDice() - Roll Bonus Dice: ' + diceRollData.bonusRolls.length);
		for (let i = 0; i < this.diceRollData.bonusRolls.length; i++) {
			const bonusRoll: BonusRoll = this.diceRollData.bonusRolls[i];
			//console.log('bonusRoll.dieCountsAs: ' + bonusRoll.dieCountsAs);
			const allDice: IDie[] = this.addDieFromStr(this.diceRollData.diceGroup, bonusRoll.playerID, bonusRoll.diceStr, bonusRoll.dieCountsAs, Random.between(1.2, 2.2), 0, bonusRoll.dieBackColor, bonusRoll.dieTextColor, bonusRoll.isMagic);
			allDice.forEach((die: IDie) => {
				die.playerName = bonusRoll.playerName;
			});
		}
		this.bonusRollStartTime = performance.now();
		this.waitingForBonusRollToComplete = true;
	}

	playAnnouncerCommentary(type: DiceRollType, d20RollValue: number, d20Modifier: number, totalDamage = 0, maxDamage = 0, damageType = DamageType.None, damageSummary: Map<DamageType, number> = null): void {
		//console.log(`playAnnouncerCommentary`);
		//console.log('d20RollValue: ' + d20RollValue);
		//console.log('d20Modifier: ' + d20Modifier);
		if (this.diceRollData.hasMultiPlayerDice) {
			diceSounds.playMultiplayerCommentary(type, d20RollValue);
			return;
		}
		if (this.diceRollData.type === DiceRollType.WildMagic) {
			diceSounds.playWildMagicCommentary(type, d20RollValue);
			return;
		}

		if (this.diceRollData.type === DiceRollType.PercentageRoll) {
			diceSounds.playPercentageRollCommentary(type, d20RollValue);
			return;
		}

		if (this.diceRollData.type === DiceRollType.DamageOnly) {
			diceSounds.playDamageCommentaryAsync(totalDamage, damageType, damageSummary);
			return;
		}

		if (this.diceRollData.type === DiceRollType.HealthOnly) {
			diceSounds.playHealthCommentary(type, this.totalHealthPlusModifier);
			return;
		}

		if (this.diceRollData.type === DiceRollType.ExtraOnly) {
			diceSounds.playExtraCommentary(type, d20RollValue);
			return;
		}

		if (this.diceRollData.type === DiceRollType.Attack) {
			diceSounds.playAttackPlusDamageCommentaryAsync(d20RollValue, d20Modifier, totalDamage, maxDamage, damageType, damageSummary, this.diceRollData.minCrit, this.attemptedRollWasSuccessful);
			// TODO: Follow with Damage announcer sound files.
			return;
		}

		if (this.diceRollData.type === DiceRollType.SkillCheck) {
			diceSounds.playSkillCheckCommentary(d20RollValue, d20Modifier, this.diceRollData.skillCheck, this.attemptedRollWasSuccessful);
			return;
		}

		if (this.diceRollData.type === DiceRollType.SavingThrow) {
			diceSounds.playSavingThrowCommentary(d20RollValue, d20Modifier, this.diceRollData.savingThrow, this.attemptedRollWasSuccessful);
			return;
		}

		if (this.diceRollData.type === DiceRollType.ChaosBolt) {
			diceSounds.playChaosBoltCommentary(d20RollValue, this.diceRollData.savingThrow);
			return;
		}

		if (this.diceRollData.type === DiceRollType.FlatD20) {
			diceSounds.playFlatD20Commentary(d20RollValue);
			return;
		}

		if (this.diceRollData.type === DiceRollType.WildMagicD20Check) {
			diceSounds.playWildMagicD20CheckCommentary(d20RollValue);
			return;
		}
	}

	getSkillCheckName() {
		if (this.diceRollData.skillCheck === Skills.none)
			return 'Skill';
		const enumAsStr: string = Object.keys(Skills).find(key => Skills[key] === this.diceRollData.skillCheck);
		let initialCapEnum = '';
		if (enumAsStr)
			initialCapEnum = enumAsStr.charAt(0).toUpperCase() + enumAsStr.slice(1);
		else
			initialCapEnum = 'Skill';
		if (this.diceRollData.skillCheck === Skills.animalHandling)
			initialCapEnum = 'Animal Handling';
		else if (this.diceRollData.skillCheck === Skills.sleightOfHand)
			initialCapEnum = 'Sleight of Hand';
		else if (this.diceRollData.skillCheck === Skills.randomShit)
			initialCapEnum = 'Random Shit';
		return initialCapEnum;
	}

	getSavingThrowName() {
		if (this.diceRollData.savingThrow === Ability.none)
			return "";
		const enumAsStr: string = Object.keys(Ability).find(key => Ability[key] === this.diceRollData.savingThrow);
		let initialCapEnum = '';
		if (enumAsStr)
			initialCapEnum = enumAsStr.charAt(0).toUpperCase() + enumAsStr.slice(1);
		else
			initialCapEnum = 'Ability';
		return initialCapEnum;
	}

	isOdd(num) {
		return num % 2;
	}

	showSuccessFailMessages(title: string, rawD20RollValue: number, diceGroup = DiceGroup.Players) {
		if (title)
			title += ' ';
		if (!this.diceRollData.hasMultiPlayerDice && this.diceRollData.type !== DiceRollType.WildMagic &&
			this.diceRollData.type !== DiceRollType.PercentageRoll &&
			this.diceRollData.type !== DiceRollType.DamageOnly &&
			this.diceRollData.type !== DiceRollType.HealthOnly &&
			this.diceRollData.type !== DiceRollType.ExtraOnly) {
			if (this.attemptedRollWasSuccessful)
				if (rawD20RollValue >= this.diceRollData.minCrit) {
					diceLayer.showResult(title + this.diceRollData.critSuccessMessage, this.attemptedRollWasSuccessful, diceGroup);
				}
				else {
					diceLayer.showResult(title + this.diceRollData.successMessage, this.attemptedRollWasSuccessful, diceGroup);
				}
			else if (rawD20RollValue === 1)
				diceLayer.showResult(title + this.diceRollData.critFailMessage, this.attemptedRollWasSuccessful, diceGroup);
			else
				diceLayer.showResult(title + this.diceRollData.failMessage, this.attemptedRollWasSuccessful, diceGroup);
		}
	}

	playSecondaryAnnouncerCommentary(type: DiceRollType, d20RollValue: number, totalDamage: number = 0, maxDamage: number = 0): void {
		//console.log(`playSecondaryAnnouncerCommentary...`);
		// TODO: Implement this.
	}

	reportRollResults(rollResults: RollResults) {
		const d20RollValue: Map<number, number> = rollResults.d20RollValue;
		const singlePlayerId: number = rollResults.singlePlayerId;
		const luckValue: Map<number, number> = rollResults.luckValue;
		const playerIdForTextMessages: number = rollResults.playerIdForTextMessages;
		const skillSavingModifier: number = rollResults.skillSavingModifier;
		const totalDamage: number = rollResults.totalDamage;
		const totalHealth: number = rollResults.totalHealth;
		const totalExtra: number = rollResults.totalExtra;
		let maxDamage: number = rollResults.maxDamage;

		let title = '';
		if (this.diceRollData.type === DiceRollType.Initiative)
			title = 'Initiative:';
		else if (this.diceRollData.type === DiceRollType.NonCombatInitiative)
			title = 'Non-combat Initiative:';
		else if (this.diceRollData.type === DiceRollType.SkillCheck)
			title = `${this.getSkillCheckName()} Check:`;
		else if (this.diceRollData.type === DiceRollType.SavingThrow)
			title = `${this.getSavingThrowName()} Saving Throw:`;

		if (this.diceRollData.multiplayerSummary) {
			this.diceRollData.multiplayerSummary.sort((a, b) => (b.roll + b.modifier) - (a.roll + a.modifier));
			diceLayer.showMultiplayerResults(title, this.diceRollData.multiplayerSummary, this.diceRollData.hiddenThreshold);
		}

		if (!this.diceRollData.hasMultiPlayerDice && d20RollValue.get(singlePlayerId) > 0) {
			if (this.diceRollData.modifier !== 0)
				diceLayer.showRollModifier(this.diceRollData.modifier, luckValue.get(singlePlayerId), playerIdForTextMessages, this.diceRollData.diceGroup);
			if (skillSavingModifier !== 0)
				diceLayer.showRollModifier(skillSavingModifier, luckValue.get(singlePlayerId), playerIdForTextMessages, this.diceRollData.diceGroup);
			if (this.diceRollData.dieTotalMessage) {
				const centerDice: Vector = this.getCenterDicePosition();
				diceLayer.showDieTotalMessage(this.diceRollData.totalRoll, this.diceRollData.dieTotalMessage, centerDice, this.diceRollData.textFillColor, this.diceRollData.textOutlineColor, this.diceRollData.diceGroup);
			}
			else
				diceLayer.showDieTotal(`${this.diceRollData.totalRoll}`, playerIdForTextMessages, this.diceRollData.diceGroup);
		}

		if (this.totalBonus > 0 && !this.diceRollData.hasMultiPlayerDice && this.diceRollData.type !== DiceRollType.SkillCheck) {
			let bonusRollStr = 'Bonus Roll: ';
			const bonusRollOverrideStr: string = this.diceRollData.getFirstBonusRollDescription();
			if (bonusRollOverrideStr)
				bonusRollStr = bonusRollOverrideStr;
			switch (this.diceRollData.wildMagic) {
				case WildMagic.heightChange:
				case WildMagic.ageChange:
					if (this.isOdd(this.totalBonus))
						this.totalBonus = -this.totalBonus;
					break;
			}
			diceLayer.showBonusRoll(`${bonusRollStr}${this.totalBonus}`, DiceLayer.bonusRollFontColor, DiceLayer.bonusRollDieColor, this.diceRollData.diceGroup);
		}

		if (totalDamage > 0) {
			diceLayer.showTotalHealthDamage(this.totalDamagePlusModifier.toString(), this.attemptedRollWasSuccessful, 'Damage: ', DiceLayer.damageDieBackgroundColor, DiceLayer.damageDieFontColor, this.diceRollData.diceGroup);
			diceLayer.showDamageHealthModifier(this.damageModifierThisRoll, this.attemptedRollWasSuccessful, DiceLayer.damageDieBackgroundColor, DiceLayer.damageDieFontColor, this.diceRollData.diceGroup);
		}
		if (totalHealth > 0) {
			diceLayer.showTotalHealthDamage('+' + this.totalHealthPlusModifier.toString(), this.attemptedRollWasSuccessful, 'Health: ', DiceLayer.healthDieBackgroundColor, DiceLayer.healthDieFontColor, this.diceRollData.diceGroup);
			diceLayer.showDamageHealthModifier(this.healthModifierThisRoll, this.attemptedRollWasSuccessful, DiceLayer.healthDieBackgroundColor, DiceLayer.healthDieFontColor, this.diceRollData.diceGroup);
		}
		if (totalExtra > 0) {
			diceLayer.showTotalHealthDamage(this.totalExtraPlusModifier.toString(), this.attemptedRollWasSuccessful, '', DiceLayer.extraDieBackgroundColor, DiceLayer.extraDieFontColor, this.diceRollData.diceGroup);
			diceLayer.showDamageHealthModifier(this.extraModifierThisRoll, this.attemptedRollWasSuccessful, DiceLayer.extraDieBackgroundColor, DiceLayer.extraDieFontColor, this.diceRollData.diceGroup);
		}
		this.showSuccessFailMessages(title, d20RollValue.get(singlePlayerId), this.diceRollData.diceGroup);
		//console.log('d20RollValue.get(singlePlayerId): ' + d20RollValue.get(singlePlayerId));
		maxDamage += this.damageModifierThisRoll;
		if (this.diceRollData.secondRollData)
			this.playSecondaryAnnouncerCommentary(this.diceRollData.secondRollData.type, d20RollValue.get(singlePlayerId), this.totalDamagePlusModifier, maxDamage);
		else {
			let d20RollTotal: number = d20RollValue.get(singlePlayerId);
			if (this.diceRollData.type === DiceRollType.SkillCheck)
				d20RollTotal = this.diceRollData.totalRoll - rollResults.toHitModifier;

			this.playAnnouncerCommentary(this.diceRollData.type, d20RollTotal, rollResults.toHitModifier, this.totalDamagePlusModifier, maxDamage, this.diceRollData.damageType, rollResults.damageSummary); return maxDamage;
		}
	}

	getCenterDicePosition(): Vector {
		let x: number = undefined;
		let y: number = undefined;
		this.dice.forEach((die: IDie) => {
			const diePos: Vector = this.getScreenCoordinates(die);
			if (x === undefined) {
				x = diePos.x;
				y = diePos.y;
			}
			else {
				x += diePos.x;
				y += diePos.y;
			}
		});
		if (x === undefined) {
			return new Vector(960, 540);
		}
		return new Vector(x / this.dice.length, y / this.dice.length);
	}

	playFinalRollSoundEffects() {
		if (!this.diceRollData)
			return;
		switch (this.diceRollData.wildMagic) {
			case WildMagic.wildMagicMinute: ; break;
			case WildMagic.seeInvisibleCreatures: ; break;
			case WildMagic.modronAppearsOneMinute: diceSounds.playWildMagic('modron'); break;
			case WildMagic.regain5hpPerTurnForOneMinute: ; break;
			case WildMagic.castMagicMissile: diceSounds.playWildMagic('magicMissile'); break;
			case WildMagic.castFireball: diceSounds.playWildMagic('fireball'); break;
			case WildMagic.heightChange:
			case WildMagic.ageChange:
				if (this.isOdd(this.totalBonus))
					diceSounds.playWildMagic('slideWhistleDown');
				else
					diceSounds.playWildMagic('slideWhistleUp');
				break;
			case WildMagic.castConfusionOnSelf: ; break;
			case WildMagic.beardOfFeathers: ; break;
			case WildMagic.castGreaseCenteredOnSelf: ; break;
			case WildMagic.spellTargetsDisadvantagedSavingThrowForOneMinute: ; break;
			case WildMagic.skinTurnsBlue: ; break;
			case WildMagic.thirdEyeAdvantageWisdomChecks: ; break;
			case WildMagic.castTimeBonusActionOneMinute: ; break;
			case WildMagic.teleportUpTo60Feet: ; break;
			case WildMagic.astralPlaneUntilEndOfNextTurn: ; break;
			case WildMagic.maximizeDamageOnSpellCastInNextMinute: ; break;
			case WildMagic.flumphs: ; break;
			case WildMagic.regainHitPoints: ; break;
			case WildMagic.pottedPlant: ; break;
			case WildMagic.teleportUpTo20FeetBonusActionOneMinute: ; break;
			case WildMagic.castLevitateOnSelf: ; break;
			case WildMagic.unicorn: ; break;
			case WildMagic.cannotSpeakPinkBubbles: ; break;
			case WildMagic.spectralShieldPlus2ArmorClassNextMinute: ; break;
			case WildMagic.alcoholImmunity: ; break;
			case WildMagic.hairFallsOutGrowsBack24Hours: ; break;
			case WildMagic.fireTouchOneMinute: ; break;
			case WildMagic.regainLowestLevelExpendedSpellSlot: ; break;
			case WildMagic.shoutWhenSpeakingOneMinute: ; break;
			case WildMagic.castFogCloudCenteredOnSelf: ; break;
			case WildMagic.lightningDamageUpToThreeCreatures: ; break;
			case WildMagic.frightenedByNearestCreatureUntilEndOfNextTurn: ; break;
			case WildMagic.allCreatures30FeetInvisibleOneMinute: ; break;
			case WildMagic.resistanceToAllDamageNextMinute: ; break;
			case WildMagic.randomCreaturePoisoned1d4Hours: ; break;
			case WildMagic.glowBrightOneMinuteCreaturesEndingTurn5FeetBlinded: ; break;
			case WildMagic.castPolymorphToSheepOnSelf: ; break;
			case WildMagic.butterfliesAndPetals10FeetOneMinute: ; break;
			case WildMagic.takeOneAdditionalActionImmediately: ; break;
			case WildMagic.allCreaturesWithin30FeetTake1d10NecroticDamage: ; break;
			case WildMagic.castMirrorImage: ; break;
			case WildMagic.castFlyOnRandomCreatureWithin60Feet: ; break;
			case WildMagic.invisibleSilentNextMinute: ; break;
			case WildMagic.immortalOneMinute: ; break;
			case WildMagic.increaseSizeOneMinute: ; break;
			case WildMagic.allCreatures30FeetVulnerableToPiercingDamageOneMinute: ; break;
			case WildMagic.faintEtheralMusicOneMinute: ; break;
			case WildMagic.regainSorceryPoints: ; break;
		}
	}

	calculateFinalMessage(): void {
		switch (this.diceRollData.wildMagic) {
			case WildMagic.wildMagicMinute:
				this.additionalDieRollMessage = 'Perform a wild magic roll at the start of each of your turns for the next minute, ignoring this result on subsequent rolls.'; break;
			case WildMagic.seeInvisibleCreatures:
				this.additionalDieRollMessage = 'For the next minute, you can see any invisible creature if you have line of sight to it.'; break;
			case WildMagic.modronAppearsOneMinute:
				this.additionalDieRollMessage = 'A modron chosen and controlled by the DM appears in an unoccupied space within 5 feet of you, then disappears 1 minute later.'; break;
			case WildMagic.castFireball: this.additionalDieRollMessage = 'You cast Fireball as a 3rd-level spell centered on yourself.'; break;
			case WildMagic.castMagicMissile: this.additionalDieRollMessage = 'You cast Magic Missile as a 5th-level spell.'; break;
			case WildMagic.heightChange:
				this.additionalDieRollMessage = 'Your height changes by a number of inches equal to the d10 roll. If the roll is odd, you shrink. If the roll is even, you grow.'; break;
			case WildMagic.castConfusionOnSelf:
				this.additionalDieRollMessage = 'You cast Confusion centered on yourself.'; break;
			case WildMagic.regain5hpPerTurnForOneMinute:
				this.additionalDieRollMessage = 'For the next minute, you regain 5 hit points at the start of each of your turns.'; break;
			case WildMagic.beardOfFeathers:
				this.additionalDieRollMessage = 'You grow a long beard made of feathers that remains until you sneeze, at which point the feathers explode out from your face.'; break;
			case WildMagic.castGreaseCenteredOnSelf:
				this.additionalDieRollMessage = 'You cast Grease centered on yourself.'; break;
			case WildMagic.spellTargetsDisadvantagedSavingThrowForOneMinute:
				this.additionalDieRollMessage = 'Creatures have disadvantage on saving throws against the next spell you cast in the next minute that involves a saving throw.'; break;
			case WildMagic.skinTurnsBlue:
				this.additionalDieRollMessage = 'Your skin turns a vibrant shade of blue. A Remove Curse spell can end this effect.'; break;
			case WildMagic.thirdEyeAdvantageWisdomChecks:
				this.additionalDieRollMessage = 'An eye appears on your forehead for the next minute. During that time, you have advantage on Wisdom (Perception) checks that rely on sight.'; break;
			case WildMagic.castTimeBonusActionOneMinute:
				this.additionalDieRollMessage = 'For the next minute, all your spells with a casting time of 1 action have a casting time of 1 bonus action.'; break;
			case WildMagic.teleportUpTo60Feet:
				this.additionalDieRollMessage = 'You teleport up to 60 feet to an unoccupied space of your choice that you can see.'; break;
			case WildMagic.astralPlaneUntilEndOfNextTurn:
				this.additionalDieRollMessage = 'You are transported to the Astral Plane until the end of your next turn, after which time you return to the space you previously occupied or the nearest unoccupied space if that space is occupied.'; break;
			case WildMagic.maximizeDamageOnSpellCastInNextMinute:
				this.additionalDieRollMessage = 'Maximize the damage of the next damaging spell you cast within the next minute.'; break;
			case WildMagic.ageChange:
				this.additionalDieRollMessage = 'Your age changes by a number of years equal to the d10 roll. If the roll is odd, you get younger (minimum 1 year old). If the roll is even, you get older.'; break;
			case WildMagic.flumphs:
				this.additionalDieRollMessage = '1d6 flumphs controlled by the DM appear in unoccupied spaces within 60 feet of you and are frightened of you. They vanish after 1 minute.'; break;
			case WildMagic.regainHitPoints:
				this.additionalDieRollMessage = 'You regain 2d10 hit points.'; break;
			case WildMagic.pottedPlant:
				this.additionalDieRollMessage = 'You turn into a potted plant until the start of your next turn. While a plant, you are incapacitated and have vulnerability to all damage. If you drop to 0 hit points, your pot breaks, and your form reverts.'; break;
			case WildMagic.teleportUpTo20FeetBonusActionOneMinute:
				this.additionalDieRollMessage = 'For the next minute, you can teleport up to 20 feet as a bonus action on each of your turns.'; break;
			case WildMagic.castLevitateOnSelf:
				this.additionalDieRollMessage = 'You cast Levitate on yourself.'; break;
			case WildMagic.unicorn:
				this.additionalDieRollMessage = 'A unicorn controlled by the DM appears in a space within 5 feet of you, then disappears 1 minute later.'; break;
			case WildMagic.cannotSpeakPinkBubbles:
				this.additionalDieRollMessage = "You can't speak for the next minute.Whenever you try, pink bubbles float out of your mouth."; break;
			case WildMagic.spectralShieldPlus2ArmorClassNextMinute:
				this.additionalDieRollMessage = 'A spectral shield hovers near you for the next minute, granting you a +2 bonus to AC and immunity to Magic Missile.'; break;
			case WildMagic.alcoholImmunity:
				this.additionalDieRollMessage = 'You are immune to being intoxicated by alcohol for the next 5d6 days.'; break;
			case WildMagic.hairFallsOutGrowsBack24Hours:
				this.additionalDieRollMessage = 'Your hair falls out but grows back within 24 hours.'; break;
			case WildMagic.fireTouchOneMinute:
				this.additionalDieRollMessage = "For the next minute, any flammable object you touch that isn't being worn or carried by another creature bursts into flame."; break;
			case WildMagic.regainLowestLevelExpendedSpellSlot:
				this.additionalDieRollMessage = 'You regain your lowest-level expended spell slot.'; break;
			case WildMagic.shoutWhenSpeakingOneMinute:
				this.additionalDieRollMessage = 'For the next minute, you must shout when you speak.'; break;
			case WildMagic.castFogCloudCenteredOnSelf:
				this.additionalDieRollMessage = 'You cast Fog Cloud centered on yourself.'; break;
			case WildMagic.lightningDamageUpToThreeCreatures:
				this.additionalDieRollMessage = 'Up to three creatures you choose within 30 feet of you take 4d10 lightning damage.'; break;
			case WildMagic.frightenedByNearestCreatureUntilEndOfNextTurn:
				this.additionalDieRollMessage = 'You are frightened by the nearest creature until the end of your next turn.'; break;
			case WildMagic.allCreatures30FeetInvisibleOneMinute:
				this.additionalDieRollMessage = 'Each creature within 30 feet of you becomes invisible for the next minute. The invisibility ends on a creature when it attacks or casts a spell.'; break;
			case WildMagic.resistanceToAllDamageNextMinute:
				this.additionalDieRollMessage = 'You gain resistance to all damage for the next minute.'; break;
			case WildMagic.randomCreaturePoisoned1d4Hours:
				this.additionalDieRollMessage = 'A random creature within 60 feet of you becomes poisoned for 1d4 hours.'; break;
			case WildMagic.glowBrightOneMinuteCreaturesEndingTurn5FeetBlinded:
				this.additionalDieRollMessage = 'You glow with bright light in a 30-foot radius for the next minute. Any creature that ends its turn within 5 feet of you is blinded until the end of its next turn.'; break;
			case WildMagic.castPolymorphToSheepOnSelf:
				this.additionalDieRollMessage = "You cast Polymorph on yourself. If you fail the saving throw, you turn into a sheep for the spell's duration."; break;
			case WildMagic.butterfliesAndPetals10FeetOneMinute:
				this.additionalDieRollMessage = 'Illusory butterflies and flower petals flutter in the air within 10 feet of you for the next minute.'; break;
			case WildMagic.takeOneAdditionalActionImmediately:
				this.additionalDieRollMessage = 'You can take one additional action immediately.'; break;
			case WildMagic.allCreaturesWithin30FeetTake1d10NecroticDamage:
				this.additionalDieRollMessage = 'Each creature within 30 feet of you takes 1d10 necrotic damage. You regain hit points equal to the sum of the necrotic damage dealt.'; break;
			case WildMagic.castMirrorImage:
				this.additionalDieRollMessage = 'You cast Mirror Image.'; break;
			case WildMagic.castFlyOnRandomCreatureWithin60Feet:
				this.additionalDieRollMessage = 'You cast Fly on a random creature within 60 feet of you.'; break;
			case WildMagic.invisibleSilentNextMinute:
				this.additionalDieRollMessage = "You become invisible for the next minute. During that time, other creatures can't hear you. The invisibility ends if you attack or cast a spell."; break;
			case WildMagic.immortalOneMinute:
				this.additionalDieRollMessage = 'If you die within the next minute, you immediately come back to life as if by the Reincarnate spell.'; break;
			case WildMagic.increaseSizeOneMinute:
				this.additionalDieRollMessage = 'Your size increases by one size category for the next minute.'; break;
			case WildMagic.allCreatures30FeetVulnerableToPiercingDamageOneMinute:
				this.additionalDieRollMessage = 'You and all creatures within 30 feet of you gain vulnerability to piercing damage for the next minute.'; break;
			case WildMagic.faintEtheralMusicOneMinute:
				this.additionalDieRollMessage = 'You are surrounded by faint, ethereal music for the next minute.'; break;
			case WildMagic.regainSorceryPoints:
				this.additionalDieRollMessage = 'You regain all expended sorcery points.'; break;
		}
		//diceRollData
		// 
	}

	onSuccess() {

	}

	onFailure() {

	}


	removeDieEffects() {
		for (let i = 0; i < this.dice.length; i++) {
			const die: IDie = this.dice[i];
			this.removeDieEffectsForSingleDie(die);
		}
	}

	animationsShouldBeDone: boolean;
	throwHasStarted: boolean;
	static readonly hiddenDieScale: number = 0.01;

	removeRemainingDice(): boolean {
		// TODO: Make sure we can call this robustly at any time.
		let dieEffectInterval = 0;
		this.animationsShouldBeDone = false;
		this.removeDieEffects();
		let diceInPlayWereRemoved = false;
		const effectOverride = this.getRandomEffect();
		for (let i = 0; i < this.dice.length; i++) {
			if (this.dice[i].inPlay) {
				diceInPlayWereRemoved = true;
				dieEffectInterval = this.removeDie(this.dice[i], dieEffectInterval, effectOverride);
			}
		}
		return diceInPlayWereRemoved;
	}

	lastRollDiceData;

	reportDieRollBackToMainApp(playerId: number) {
		// Corresponds to DiceStoppedRollingData 
		this.lastRollDiceData = {
			'wasCriticalHit': this.wasCriticalHit,
			'playerID': playerId,
			'success': this.attemptedRollWasSuccessful,
			'roll': this.diceRollData.totalRoll,
			'hiddenThreshold': this.diceRollData.hiddenThreshold,
			'spellName': this.diceRollData.spellName,
			'damage': this.totalDamagePlusModifier,
			'health': this.totalHealthPlusModifier,
			'extra': this.totalExtraPlusModifier,
			'multiplayerSummary': this.diceRollData.multiplayerSummary,
			'individualRolls': this.diceRollData.individualRolls,
			'type': this.diceRollData.type,
			'skillCheck': this.diceRollData.skillCheck,
			'savingThrow': this.diceRollData.savingThrow,
			'bonus': this.totalBonus,
			'additionalDieRollMessage': this.additionalDieRollMessage,
			'diceGroup': this.diceRollData.diceGroup,
			'rollId': this.diceRollData.rollId,
		};

		diceHaveStoppedRolling(JSON.stringify(this.lastRollDiceData));
	}

	getFirstCreatureId(diceRollDto: DiceRollData): number {
		if (diceRollDto.diceDtos && diceRollDto.diceDtos.length > 0)
			return diceRollDto.diceDtos[0].CreatureId;
		return Number.MIN_VALUE;
	}

	hasMultiPlayerDice(diceRollDto: DiceRollData): boolean {
		let lastCreatureId: number = Number.MAX_VALUE;
		let lastPlayerName: string = null;
		diceRollDto.diceDtos.forEach((item: DiceDto) => {
			if (lastCreatureId === Number.MAX_VALUE)
				lastCreatureId = item.CreatureId;
			if (lastPlayerName === null)
				lastPlayerName = item.PlayerName;

			if (lastPlayerName !== item.PlayerName)
				return true;

			if (lastCreatureId !== item.CreatureId)
				return true;

		});
		return false;
	}

	onDiceRollStopped() {
		//console.log('onDiceRollStopped...');
		//console.log(`diceRollData.totalRoll = ${this.diceRollDataPlayer.totalRoll}`);
		this.calculateFinalMessage();
		this.onBonusThrow = false;
		this.setupBonusRoll = false;
		this.needToClearD20s = true;
		this.allDiceHaveStoppedRolling = true;
		//console.log('Dice have stopped rolling!');
		//diceHaveStoppedRolling(null);

		if (this.diceRollData.onStopRollingSound)
			diceSounds.safePlayMp3(this.diceRollData.onStopRollingSound);

		if (this.attemptedRollWasSuccessful) {
			this.onSuccess();
		}
		else {
			this.onFailure();
		}

		let playerId: number = diceLayer.playerID;
		if (this.diceRollData.playerRollOptions.length === 1)
			playerId = this.diceRollData.playerRollOptions[0].PlayerID;

		if (!this.hasMultiPlayerDice(this.diceRollData)) {
			const creatureId: number = this.getFirstCreatureId(this.diceRollData);
			if (creatureId !== Number.MIN_VALUE)
				playerId = creatureId;
		}

		// Connects to DiceStoppedRollingData in DiceStoppedRollingData.cs:
		//console.log('diceRollData.type: ' + diceRollData.type);
		//console.log(diceRollData.multiplayerSummary);
		//console.log(diceRollData.individualRolls);
		this.reportDieRollBackToMainApp(playerId);

		if (DieRoller.removeDiceImmediately)
			this.removeRemainingDice();
	}

	async sayNat20BonusRoll() {
		await diceSounds.playSoundFileAsync('Announcer/Reactions/NaturalTwenty[4]');
		await diceSounds.playSoundFileAsync('Announcer/Reactions/BonusRoll[5]');
	}

	diceDefinitelyStoppedRolling() {
		if (this.needToClearD20s) {
			if (this.diceRollData.hasMultiPlayerDice) {
				this.removeMultiplayerD20s();
			}
			else {
				this.d20RollValue = this.removeD20s();
			}
			this.showSpecialLabels();
		}
		if (this.needToRollBonusDice()) {
			if (!this.setupBonusRoll) {
				this.setupBonusRoll = true;
				//if (diceRollData.type == DiceRollType.WildMagic)
				//	showRollTotal();
				if (!this.diceRollData.startedBonusDiceRoll) {
					this.freezeExistingDice();
					this.diceRollData.startedBonusDiceRoll = true;
					if (this.isAttack(this.diceRollData) && this.d20RollValue >= this.diceRollData.minCrit) {
						diceLayer.indicateBonusRoll('Damage Bonus!', this.diceRollData.diceGroup);
						this.wasCriticalHit = true;
					}
					else
						diceLayer.indicateBonusRoll('Bonus Roll!', this.diceRollData.diceGroup);
					setTimeout(this.rollBonusDice.bind(this), 2500);
					if (this.nat20SkillCheckBonusRoll) {
						this.nat20SkillCheckBonusRoll = false;
						this.sayNat20BonusRoll();
					}
				}
			}
		}
		else {
			this.showSpecialLabels();
			this.popFrozenDice();
			//console.log('Calling getRollResults() from diceDefinitelyStoppedRolling...');
			this.reportRollResults(this.getRollResults(true));
			if (this.diceRollData.playBonusSoundAfter)
				setTimeout(this.playFinalRollSoundEffects.bind(this), this.diceRollData.playBonusSoundAfter);
			this.onDiceRollStopped();
		}
	}

	diceJustStoppedRolling(now: number) {
		const thisTime: number = now;
		if ((thisTime - this.firstStopTime) / 1000 > 1.5) {
			this.diceDefinitelyStoppedRolling();
		}
	}

	anyDiceStillRolling(): boolean {
		if (this.dice === null)
			return false;
		for (let i = 0; i < this.dice.length; i++) {
			if (!this.dice[i].isFinished())
				return true;
		}
		return false;
	}

	checkStillRolling() {
		if (this.allDiceHaveStoppedRolling)
			return;

		if (this.anyDiceStillRolling()) {
			this.waitingForSettle = true;
			return;
		}

		if (this.setupBonusRoll && !this.waitingForBonusRollToComplete) {
			this.waitingForSettle = true;
			return;
		}

		const now: number = performance.now();

		if (this.waitingForBonusRollToComplete && this.bonusRollStartTime !== 0) {
			if (now - this.bonusRollStartTime < 1000) {
				return;
			}
			this.waitingForBonusRollToComplete = false;
			this.setupBonusRoll = false;
		}

		if (this.waitingForSettle) {
			this.waitingForSettle = false;
			this.firstStopTime = now;
		}
		else {
			this.diceJustStoppedRolling(now);
		}
	}

	clearAllDiceIfHidden() {
		for (let i = 0; i < this.dice.length; i++) {
			const die: IDie = this.dice[i];
			const dieObject: IDieObject = die.getObject();
			if (dieObject === null)
				continue;
			if (!dieObject.isHidden)
				return;
		}
		this.clearAllDice();
	}

	hideDie(dieObject) {
		dieObject.scale.set(DieRoller.hiddenDieScale, DieRoller.hiddenDieScale, DieRoller.hiddenDieScale);
		dieObject.isHidden = true;
		this.clearAllDiceIfHidden();
	}

	removeSingleDieFromArray(dieToRemove: IDie, listToChange: IDie[]) {
		for (let i = 0; i < listToChange.length; i++) {
			if (dieToRemove === listToChange[i]) {
				listToChange.splice(i, 1);
				return;
			}
		}
	}

	removeDiceFromArray(dieToRemove: IDie[], listToChange: IDie[]) {
		for (let i = 0; i < dieToRemove.length; i++) {
			this.removeSingleDieFromArray(dieToRemove[i], listToChange);
		}
	}

	clearTheseDice(diceList: IDie[]) {
		if (!diceList || diceList.length === 0)
			return;
		for (let i = 0; i < diceList.length; i++) {
			const die: IDie = diceList[i];
			if (!die)
				continue;
			const dieObject: IDieObject = die.getObject();
			if (!dieObject)
				continue;
			this.scene.remove(dieObject);
			die.clear(this);
		}
	}

	highlightSpecialDice() {
		if (!this.specialDice || this.specialDice.length === 0)
			return;

		const now: number = performance.now();

		const hiddenDie: IDie[] = [];

		const magicRingHueShift: number = Math.floor(Math.random() * 360);
		
		const scale = this.getDieScale();

		for (let i = 0; i < this.specialDice.length; i++) {
			const thisSpecialDie: IDie = this.specialDice[i];
			const dieObject: IDieObject = thisSpecialDie.getObject();
			if (dieObject === null)
				continue;

			if (dieObject.needToHideDie) {
				if (dieObject.hideTime < now) {
					dieObject.needToHideDie = false;
					this.hideDie(dieObject);
					hiddenDie.push(this.specialDice[i]);
				}
			}

			if (dieObject.needToStartEffect) {
				const effectStartTime: number = dieObject.removeTime + dieObject.effectStartOffset;
				if (now > effectStartTime && dieObject.needToStartEffect) {

					dieObject.needToStartEffect = false;

					// die.dieValue is also available.
					const screenPos: Vector = this.getScreenCoordinates(thisSpecialDie);
					if (!screenPos)
						continue;

					if (dieObject.effectKind === DieEffect.Lucky) {
						diceLayer.addLuckyRing(thisSpecialDie, screenPos.x, screenPos.y);
					}
					if (dieObject.effectKind === DieEffect.Ring) {
						diceLayer.addMagicRing(screenPos.x, screenPos.y, scale, magicRingHueShift + Random.plusMinusBetween(10, 25));
					}
					else if (dieObject.effectKind === DieEffect.Fireball) {
						//diceLayer.addD20Fire(screenPos.x, screenPos.y);
						diceLayer.addFireball(screenPos.x, screenPos.y, scale);
						diceSounds.playFireball();
					}
					else if (dieObject.effectKind === DieEffect.Bomb) {
						const hueShift: number = Math.floor(Math.random() * 360);
						let saturation = 75;  // Reduce saturation for significant hue shifts 
						if (hueShift < 15 || hueShift > 345)
							saturation = 100;
						diceLayer.addDiceBomb(screenPos.x, screenPos.y, scale, hueShift, saturation, 100);
						diceSounds.playDieBomb();
						hideDieIn(dieObject, 700);
					}
					else if (dieObject.effectKind === DieEffect.GroundBurst) {
						diceLayer.addGroundBurst(this.specialDice[i], screenPos, dieObject);
					}
					else if (dieObject.effectKind === DieEffect.SmokeyPortal) {
						diceLayer.addSmokeyPortal(this.specialDice[i], screenPos, dieObject);
					}
					else if (dieObject.effectKind === DieEffect.SteamPunkTunnel) {
						diceLayer.addSteampunkTunnel(screenPos.x, screenPos.y, scale, Math.floor(Math.random() * 360), 100, 100);
						//diceLayer.playSteampunkTunnel();
						//hideDieIn(die, 700);
					}
					else if (dieObject.effectKind === DieEffect.HandGrab) {
						diceLayer.grabDiceWithHand(screenPos.x, screenPos.y, scale, Math.floor(Math.random() * 360), 100, 100);
						//diceSounds.playHandGrab();
						//hideDieIn(die, 41 * 30);
					}
					else if (dieObject.effectKind === DieEffect.ColoredSmoke) {
						let saturation = 100;
						let brightnessBase = 50;
						const rand: number = Math.random() * 100;
						if (rand < 5) {
							saturation = 0;
							brightnessBase = 110;
						}
						else if (rand < 10) {
							saturation = 25;
							brightnessBase = 100;
						}
						else if (rand < 25) {
							saturation = 50;
							brightnessBase = 90;
						}
						else if (rand < 75) {
							saturation = 75;
							brightnessBase = 70;
						}

						diceLayer.blowColoredSmoke(screenPos.x, screenPos.y, scale, Math.floor(Math.random() * 360), saturation, brightnessBase + Math.random() * 80);
						this.hideDie(dieObject);
						hiddenDie.push(this.specialDice[i]);
						diceSounds.playDiceBlow();
					}
				}
			}
		}
		this.removeDiceFromArray(hiddenDie, this.specialDice);
		this.removeDiceFromArray(hiddenDie, this.dice);
		this.clearTheseDice(hiddenDie);
	}

	private getDieScale() {
		if (this.diceRollData.diceDtos && this.diceRollData.diceDtos.length > 0) {
			return this.diceRollData.diceDtos[0].Scale;
		}
		return 1;
	}

	diceRemainingInPlay(): number {
		let count = 0;
		for (const i in this.dice) {
			if (this.dice[i].inPlay) {
				count++;
			}
		}
		return count;
	}


	diceRollerTimeBetweenFramesQueue: Array<number> = [];
	diceRollerDrawTimeForEachFrameQueue: Array<number> = [];
	diceRollerLastFrameUpdate: number;
	diceRollerFpsWindow: FpsWindow;

	calculateFramerate(startUpdate: number, endUpdate: number): any {
		if (this.diceRollerLastFrameUpdate) {
			const timeBetweenFrames: number = endUpdate - this.diceRollerLastFrameUpdate;
			this.diceRollerTimeBetweenFramesQueue.push(timeBetweenFrames);
			if (this.diceRollerTimeBetweenFramesQueue.length > Game.fpsHistoryCount)
				this.diceRollerTimeBetweenFramesQueue.shift();
		}

		const drawTimeForThisFrame: number = endUpdate - startUpdate;
		this.diceRollerDrawTimeForEachFrameQueue.push(drawTimeForThisFrame);
		if (this.diceRollerDrawTimeForEachFrameQueue.length > Game.fpsHistoryCount)
			this.diceRollerDrawTimeForEachFrameQueue.shift();

		this.diceRollerLastFrameUpdate = endUpdate;
	}

	addLights() {
		// @ts-ignore - THREE
		const ambient = new THREE.AmbientLight('#ffffff', 0.35);
		this.scene.add(ambient);

		// @ts-ignore - THREE
		const directionalLight = new THREE.DirectionalLight('#ffffff', 0.25);
		directionalLight.position.x = -1000;
		directionalLight.position.y = 1000;
		directionalLight.position.z = 1000;
		this.scene.add(directionalLight);

		// @ts-ignore - THREE
		const light = new THREE.SpotLight(0xefdfd5, 0.7);
		light.position.x = 10;
		light.position.y = 100;
		light.position.z = 10;
		light.target.position.set(0, 0, 0);
		light.castShadow = true;
		light.shadow.camera.near = 50;
		light.shadow.camera.far = 110;
		light.shadow.mapSize.width = 1024;
		light.shadow.mapSize.height = 1024;

		this.scene.add(light);
	}

	startAnimatingDiceRoller(fps: number, animateDiceRollerFps: FrameRequestCallback): void {
		this.changeFramerateDiceRoller(fps);
		requestAnimationFrame(animateDiceRollerFps);
	}

	// @ts-ignore - DiceManagerClass
	diceManager: DiceManagerClass;

	init() {
		// @ts-ignore - DiceManagerClass
		this.diceManager = new DiceManagerClass();
		diceLayer = new DiceLayer();
		// SCENE
		// @ts-ignore - THREE
		this.scene = new THREE.Scene();

		const SCREEN_WIDTH = 1920, SCREEN_HEIGHT = 1080;
		const lensFactor = 5;
		const VIEW_ANGLE = 45 / lensFactor, ASPECT = SCREEN_WIDTH / SCREEN_HEIGHT, NEAR = 0.01, FAR = 20000;

		// @ts-ignore - THREE
		this.camera = new THREE.PerspectiveCamera(VIEW_ANGLE, ASPECT, NEAR, FAR);
		this.scene.add(this.camera);
		this.camera.position.set(0, 30 * lensFactor, 0);

		// RENDERER
		// @ts-ignore - THREE
		this.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
		this.renderer.setClearColor(0x000000, 0);

		this.renderer.setSize(SCREEN_WIDTH, SCREEN_HEIGHT);
		this.renderer.shadowMap.enabled = true;
		// @ts-ignore - THREE
		this.renderer.shadowMap.type = THREE.PCFSoftShadowMap;

		this.container = document.getElementById(`ThreeJS.${this.groupName}`);
		this.container.appendChild(this.renderer.domElement);

		// EVENTS
		// CONTROLS
		// @ts-ignore - THREE
		this.controls = new THREE.OrbitControls(this.camera, this.renderer.domElement);

		//// STATS
		//stats = new Stats();
		//stats.domElement.style.position = 'absolute';
		//stats.domElement.style.bottom = '0px';
		//stats.domElement.style.zIndex = 100;
		//container.appendChild(stats.domElement);

		this.addLights();

		// @ts-ignore - THREE
		const material = new THREE.ShadowMaterial();
		material.opacity = 0.5;
		// @ts-ignore - THREE
		const geometry = new THREE.PlaneGeometry(1000, 1000, 1, 1);
		// @ts-ignore - THREE
		const mesh = new THREE.Mesh(geometry, material);
		mesh.receiveShadow = true;
		mesh.rotation.x = -Math.PI / 2;
		this.scene.add(mesh);


		////////////
		// CUSTOM //
		////////////
		// @ts-ignore - CANNON
		this.world = new CANNON.World();

		this.setNormalGravity();
		// @ts-ignore - CANNON
		this.world.broadphase = new CANNON.NaiveBroadphase();
		this.world.solver.iterations = 32;

		// @ts-ignore - DiceManager
		this.diceManager.setWorld(this.world);

		const wallHeight = 68;

		const wallThickness = 1;
		const leftWallWidth = 80;
		const leftWallX = -21.5;

		const dmLeftWallWidth = 40;
		const dmLeftWallX = 13;
		const dmLeftWallZ = -26;

		const topWallWidth = 80;
		const topWallZ = -10.5;

		const playerTopWallWidth = 58;
		const playerTopWallZ = 5;
		const playerTopWallX = -2;

		const viewerFloorX = 10;
		const viewerFloorY = -4;
		const viewerFloorZ = 8.25;

		const dmBottomWallWidth = 9;
		const dmBottomWallZ = -6;
		const dmBottomWallX = 17;

		const showWalls = false;		// To drag the camera around and be able to see the walls, see the note in DieRoller.cshtml.
		const addPlayerWall = true;
		const addViewerRollFloor = true;
		const addDungeonMasterWalls = true;
		if (showWalls) {
			this.addWallsToScene(wallThickness, leftWallWidth, leftWallX, topWallWidth, topWallZ, addPlayerWall, addViewerRollFloor, playerTopWallWidth, wallHeight, playerTopWallX, playerTopWallZ, viewerFloorX, viewerFloorY, viewerFloorZ, addDungeonMasterWalls, dmBottomWallWidth, dmBottomWallX, dmBottomWallZ, dmLeftWallWidth, dmLeftWallX, dmLeftWallZ);
		}

		// Floor
		// @ts-ignore - CANNON
		const floorBody = new CANNON.Body({ mass: 0, shape: new CANNON.Plane(), material: this.diceManager.floorBodyMaterial });
		// @ts-ignore - CANNON
		floorBody.quaternion.setFromAxisAngle(new CANNON.Vec3(1, 0, 0), -Math.PI / 2);
		floorBody.name = 'floor';
		this.world.add(floorBody);

		//Walls
		// @ts-ignore - CANNON
		const rightWall = new CANNON.Body({ mass: 0, shape: new CANNON.Plane(), material: this.diceManager.barrierBodyMaterial });
		rightWall.name = 'wall';
		// @ts-ignore - CANNON
		rightWall.quaternion.setFromAxisAngle(new CANNON.Vec3(0, 1, 0), -Math.PI / 2);
		rightWall.position.x = 20.5;
		this.world.add(rightWall);

		// @ts-ignore - CANNON
		const wallDiceContactMaterial = new CANNON.ContactMaterial(this.diceManager.barrierBodyMaterial, this.diceManager.diceBodyMaterial, { friction: 0.0, restitution: 0.9 });
		this.world.addContactMaterial(wallDiceContactMaterial);

		//let leftWall = new CANNON.Body({ mass: 0, shape: new CANNON.Plane(), material: this.diceManager.floorBodyMaterial });
		//leftWall.quaternion.setFromAxisAngle(new CANNON.Vec3(0, 1, 0), -Math.PI / 2);
		//leftWall.position.x = -20;
		//world.add(leftWall);

		// @ts-ignore - CANNON & DiceManager
		const topCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(topWallWidth, wallHeight, wallThickness)), material: this.diceManager.barrierBodyMaterial });
		topCannonWall.name = 'wall';
		topCannonWall.position.z = topWallZ;
		// @ts-ignore - CANNON 
		topCannonWall.quaternion.setFromEuler(-Math.PI / 4, 0, 0);
		this.world.add(topCannonWall);

		// @ts-ignore - CANNON & DiceManager
		const leftCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(wallThickness, wallHeight, leftWallWidth)), material: this.diceManager.barrierBodyMaterial });
		leftCannonWall.name = 'wall';
		leftCannonWall.position.x = leftWallX;
		this.world.add(leftCannonWall);

		if (addPlayerWall) {
			// @ts-ignore - CANNON & DiceManager
			const playerTopCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(playerTopWallWidth, wallHeight, wallThickness)), material: this.diceManager.barrierBodyMaterial });
			playerTopCannonWall.name = 'wall';
			playerTopCannonWall.position.x = playerTopWallX;
			playerTopCannonWall.position.z = playerTopWallZ;
			this.world.add(playerTopCannonWall);
		}

		if (addViewerRollFloor) {
			// @ts-ignore - CANNON & DiceManager
			const playerRightCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(wallHeight, wallHeight, wallThickness)), material: this.diceManager.barrierBodyMaterial });
			playerRightCannonWall.quaternion.setFromEuler(Math.PI / 4, 0, 0);
			playerRightCannonWall.name = 'wall';
			playerRightCannonWall.position.x = viewerFloorX;
			playerRightCannonWall.position.y = viewerFloorY;
			playerRightCannonWall.position.z = viewerFloorZ;
			this.world.add(playerRightCannonWall);
		}

		if (addDungeonMasterWalls) {
			// @ts-ignore - CANNON & DiceManager
			const dmBottomCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(dmBottomWallWidth * 0.5, wallHeight, wallThickness)), material: this.diceManager.barrierBodyMaterial });
			dmBottomCannonWall.name = 'wall';
			dmBottomCannonWall.position.x = dmBottomWallX;
			dmBottomCannonWall.position.z = dmBottomWallZ;
			this.world.add(dmBottomCannonWall);

			// @ts-ignore - CANNON & DiceManager
			const dmLeftCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(wallThickness, wallHeight, dmLeftWallWidth * 0.5)), material: this.diceManager.barrierBodyMaterial });
			dmLeftCannonWall.name = 'wall';
			dmLeftCannonWall.position.x = dmLeftWallX;
			dmLeftCannonWall.position.z = dmLeftWallZ;
			this.world.add(dmLeftCannonWall);
		}
		this.startAnimatingDiceRoller(30, this.animateDiceRollerFps.bind(this));
	}

	allDiceShouldBeDestroyedByNow() {
		allDiceHaveBeenDestroyed(JSON.stringify(this.lastRollDiceData));
	}

	scaleFallingDice() {
		if (!this.scalingDice || this.scalingDice.length === 0)
			return;

		const dieScale = this.getDieScale();

		const hiddenDie: IDie[] = [];
		//let numDiceScaling = 0;
		if (this.scalingDice && this.scalingDice.length > 0) {
			for (let i = 0; i < this.scalingDice.length; i++) {
				const thisScalingDie: IDie = this.scalingDice[i];
				const dieObject: IDieObject = thisScalingDie.getObject();
				if (dieObject === null)
					continue;

				const portalOpenTime: number = dieObject.removeTime + dieObject.effectStartOffset;

				const now: number = performance.now();
				let waitToFallTime: number;
				if (dieObject.effectKind === DieEffect.SteamPunkTunnel) {
					waitToFallTime = 35 * 30;
				}
				else if (dieObject.effectKind === DieEffect.HandGrab) {
					waitToFallTime = 34 * 30;
				}
				else { // DieEffect.Portal
					waitToFallTime = 700;
				}

				if (now > portalOpenTime && dieObject.needToStartEffect) {
					dieObject.needToStartEffect = false;
					const screenPos: Vector = this.getScreenCoordinates(thisScalingDie);

					if (dieObject.effectKind === DieEffect.SteamPunkTunnel) {
						if (screenPos) {
							diceLayer.addSteampunkTunnel(screenPos.x, screenPos.y, dieScale, Math.floor(Math.random() * 360), 100, 100);
						}
						diceSounds.playSteampunkTunnel();
					}
					else if (dieObject.effectKind === DieEffect.HandGrab) {
						let saturation = 100;
						let hueShift: number;

						if (DiceLayer.matchOozeToDieColor)
							if (this.scalingDice[i].rollType !== DieCountsAs.totalScore && this.scalingDice[i].rollType !== DieCountsAs.inspiration)
								hueShift = 0;
							else {
								const hsl: HueSatLight = HueSatLight.fromHex(this.scalingDice[i].diceColor);
								if (hsl)
									hueShift = hsl.hue * 360;
								else
									hueShift = diceLayer.activePlayerHueShift;
							}
						else {
							hueShift = this.getRandomRedBlueHueShift();
							if (Math.random() < 0.1)
								saturation = 0;
						}

						if (screenPos)
							diceLayer.grabDiceWithHand(screenPos.x, screenPos.y, dieScale, hueShift, saturation, 100);
						diceSounds.playHandGrab();
					}
					else {  // DieEffect.Portal
						if (screenPos)
							diceLayer.addPortal(screenPos.x, screenPos.y, dieScale, this.getRandomRedBlueHueShift(), 100, 100);
						diceSounds.playOpenDiePortal();
					}
				}

				const startFallTime: number = dieObject.removeTime + waitToFallTime + dieObject.effectStartOffset;

				if (now < startFallTime)
					continue;

				let totalFrames: number;
				const fps30 = 33; // ms

				let totalScaleDistance: number;

				const elapsedTime: number = now - startFallTime;

				if (dieObject.effectKind === DieEffect.SteamPunkTunnel) {
					totalFrames = 45;
					totalScaleDistance = 0.9;
				}
				else if (dieObject.effectKind === DieEffect.HandGrab) {
					totalFrames = 30;
					totalScaleDistance = 0.99;
				}
				else {
					totalFrames = 40;
					totalScaleDistance = 0.99;
				}

				const totalTimeToScale: number = fps30 * totalFrames;  // ms

				if (elapsedTime > totalTimeToScale) {
					if (dieObject.hideOnScaleStop) {
						dieObject.hideOnScaleStop = false;
						this.hideDie(dieObject);
						hiddenDie.push(this.scalingDice[i]);
					}
					continue;
				}

				if (dieObject.needToDrop === true) {

					dieObject.needToDrop = false;
				}

				const percentTraveled: number = elapsedTime / totalTimeToScale;

				const distanceTraveled: number = percentTraveled * totalScaleDistance;

				const newScale: number = 1 - distanceTraveled;

				if (newScale <= DieRoller.hiddenDieScale) {
					this.hideDie(dieObject);
					hiddenDie.push(this.scalingDice[i]);
				}
				else {
					//numDiceScaling++;
					dieObject.scale.set(newScale, newScale, newScale);
				}
				//if (newScale < 0.35) {
				//  bodiesToRemove.push(die);
				//  // @ts-ignore - this.diceManager
				//  //die.body.collisionResponse = 1;
				//  //die.body.mass = 1;
				//  //this.diceManager.world.remove(die.body);
				//}
			}
		}
		this.removeDiceFromArray(hiddenDie, this.scalingDice);
		this.removeDiceFromArray(hiddenDie, this.dice);
		this.clearTheseDice(hiddenDie);
	}

	getRandomRedBlueHueShift() {
		let hueShift = Math.floor(Math.random() * 360);
		let tryCount = 0;
		while (hueShift > 30 && hueShift < 160 && tryCount++ < 20) {
			hueShift = Math.floor(Math.random() * 360);
		}
		return hueShift;
	}

	updatePhysics() {
		//  if (bodiesToRemove && bodiesToRemove.length > 0) {
		//    console.log('removing bodies...');

		//    bodiesToRemove.forEach(function (body) {
		//      body.collisionResponse = true;
		//      body.mass = 1;
		//      world.remove(body);
		//    });
		//    bodiesToRemove = [];
		//  }

		this.world.step(1.0 / 60.0);

		for (const i in this.dice) {
			this.dice[i].updateMeshFromBody();
		}
		this.checkStillRolling();
		this.scaleFallingDice();
		this.highlightSpecialDice();

		const numDiceStillInPlay: number = this.diceRemainingInPlay();
		const stillScaling: boolean = this.scalingDice !== null && this.scalingDice.length > 0;
		const stillHaveSpecialDice: boolean = this.specialDice !== null && this.specialDice.length > 0;
		//console.log(`numDiceStillInPlay = ${numDiceStillInPlay}, animationsShouldBeDone = ${animationsShouldBeDone}, allDiceHaveStoppedRolling = ${allDiceHaveStoppedRolling}, stillScaling = ${stillScaling}`);

		if (this.throwHasStarted && !this.animationsShouldBeDone && numDiceStillInPlay === 0 && this.allDiceHaveStoppedRolling &&!stillScaling && !stillHaveSpecialDice) {
			this.animationsShouldBeDone = true;
			this.throwHasStarted = false;
			//console.log('animationsShouldBeDone = true;');
			this.diceRollData = null;
			this.dice = [];
			setTimeout(this.allDiceShouldBeDestroyedByNow.bind(this), 3000);
		}
	}

	update() {
		this.controls.update();
		if (this.stats) {
			this.stats.update();
		}
	}

	movePointAtAngle(point: Vector, angleInDegrees: number, distance): Vector {
		const angleInRadians: number = angleInDegrees * Math.PI / 180;
		return new Vector(point.x + (Math.sin(angleInRadians) * distance), point.y - (Math.cos(angleInRadians) * distance));
	}

	getDieSpeed(die: IDie): number {
		const dieObject = die.getObject();
		const velocity = dieObject.body.velocity;
		return Math.sqrt(velocity.x * velocity.x + velocity.y * velocity.y + velocity.z * velocity.z);
	}

	static readonly highestDieSpeedWeHaveSeen: number = 128;

	// Returns a number between 0 and 1, where 1 is max speed and 0 is no movement.
	getNormalizedDieSpeed(die: IDie): number {
		const dieSpeed: number = this.getDieSpeed(die);
		//trackDieVelocities(dieSpeed);
		return MathEx.clamp(dieSpeed, 0, DieRoller.highestDieSpeedWeHaveSeen) / DieRoller.highestDieSpeedWeHaveSeen;
	}

	positionTrailingSprite(die: IDie, trailingEffect: TrailingEffect, index = 0): SpriteProxy {
		if (die.rollType === DieCountsAs.totalScore || die.rollType === DieCountsAs.inspiration || die.rollType === DieCountsAs.bentLuck) {
			const pos: Vector = this.getScreenCoordinates(die);
			if (!pos)
				return null;

			const dieNormalizedSpeed: number = this.getNormalizedDieSpeed(die);

			let scaleFactor = 1;

			let spriteScale = 1;

			if (trailingEffect.ScaleVariance !== 0)
				spriteScale = Math.max(0.01, trailingEffect.Scale * (1 + Random.plusMinus(trailingEffect.ScaleVariance)));
			else
				spriteScale = trailingEffect.Scale;

			if (trailingEffect.ScaleWithVelocity) {
				spriteScale = spriteScale * dieNormalizedSpeed;
			}

			spriteScale = MathEx.clamp(spriteScale, trailingEffect.MinScale, trailingEffect.MaxScale);

			spriteScale *= die.scale;

			scaleFactor = spriteScale / trailingEffect.Scale;

			if (die.lastPos.length <= index)
				die.lastPos.push(new Vector(-100, -100));
			const deltaX: number = pos.x - die.lastPos[index].x;
			const deltaY: number = pos.y - die.lastPos[index].y;

			//` <formula 2; \sqrt{deltaX^2 + deltaY^2}>
			let distanceSinceLastPoint: number = Math.sqrt(deltaX * deltaX + deltaY * deltaY);


			// ![](44408656431640F1B13DDCA10C7B507D.png;;;0.04947,0.04744)
			if (distanceSinceLastPoint > trailingEffect.MinForwardDistanceBetweenPrints * scaleFactor) {

				//` <formula 3; \frac{atan2(deltaY,deltaX) * 180}{\pi} + 90^{\circ}>
				const angle = Math.atan2(deltaY, deltaX) * 180 / Math.PI + 90;
				let angleToMovePawPrint = 90;
				if (die.lastPrintOnLeft)
					angleToMovePawPrint = -90;
				die.lastPrintOnLeft = !die.lastPrintOnLeft;
				const printPos: Vector = this.movePointAtAngle(pos, angle + angleToMovePawPrint, trailingEffect.LeftRightDistanceBetweenPrints * scaleFactor);

				const spriteProxy: SpriteProxy = diceLayer.AddTrailingEffect(die, trailingEffect, printPos.x, printPos.y, angle, spriteScale);
				die.lastPos[index] = pos;
				return spriteProxy;
			}
		}
		return null;
	}

	addTrailingEffects(die: IDie, trailingEffects: Array<TrailingEffect>) {
		for (let j = 0; j < trailingEffects.length; j++) {
			const trailingEffect: TrailingEffect = trailingEffects[j];

			if (this.positionTrailingSprite(die, trailingEffect, j)) {
				if (trailingEffect.OnPrintPlaySound) {
					const parts: string[] = trailingEffect.OnPrintPlaySound.split(';');
					for (let i = 0; i < parts.length; i++) {
						const part: string = parts[i].trim();
						let soundFileName: string = part;
						const parenPos: number = part.indexOf('(');
						let intervalBetweenSounds = 0;
						if (parenPos > 0) {
							soundFileName = part.substring(0, parenPos);
							if (trailingEffect.intervalBetweenSounds.has(soundFileName))
								intervalBetweenSounds = trailingEffect.intervalBetweenSounds.get(soundFileName);

							if (intervalBetweenSounds === 0) {
								let parameters: string = part.substring(parenPos + 1).trim();
								if (parameters.endsWith(')'))
									parameters = parameters.substring(0, parameters.length - 1);

								const params: string[] = parameters.split(',');
								if (params.length !== 2)
									break;
								const medianSoundInterval: number = +params[0];
								//console.log('medianSoundInterval: ' + medianSoundInterval);
								const plusMinusSoundInterval: number = +params[1];
								//console.log('plusMinusSoundInterval: ' + plusMinusSoundInterval);
								intervalBetweenSounds = medianSoundInterval + Random.plusMinus(plusMinusSoundInterval);
								trailingEffect.intervalBetweenSounds.set(soundFileName, intervalBetweenSounds);
							}
						}
						//console.log('soundFileName: ' + soundFileName);
						//console.log('intervalBetweenSounds: ' + intervalBetweenSounds);
						if (diceSounds.safePlayMp3(soundFileName, intervalBetweenSounds))
							trailingEffect.intervalBetweenSounds.set(soundFileName, 0);
						//if (trailingEffect.intervalBetweenSounds === 0)
						//	trailingEffect.intervalBetweenSounds = trailingEffect.MedianSoundInterval + Random.plusMinus(trailingEffect.PlusMinusSoundInterval);

						//if (diceSounds.safePlayMp3(trailingEffect.OnPrintPlaySound, trailingEffect.intervalBetweenSounds)) {
						//	trailingEffect.intervalBetweenSounds = trailingEffect.MedianSoundInterval + Random.plusMinus(trailingEffect.PlusMinusSoundInterval);
					}
				}
			}
		}
	}

	// TODO: For goodness sakes, Mark, do something with this.
	old_positionTrailingSprite(die: IDie, addPrintFunc: (x: number, y: number, scale: number, angle: number) => SpriteProxy, minForwardDistanceBetweenPrints: number, leftRightDistanceBetweenPrints: number = 0, index: number = 0): SpriteProxy {
		if (die.rollType === DieCountsAs.totalScore || die.rollType === DieCountsAs.inspiration || die.rollType === DieCountsAs.bentLuck) {
			const pos: Vector = this.getScreenCoordinates(die);
			if (!pos)
				return null;
			if (die.lastPos.length <= index)
				die.lastPos.push(new Vector(-100, -100));
			const deltaX: number = pos.x - die.lastPos[index].x;
			const deltaY: number = pos.y - die.lastPos[index].y;

			//` <formula 2; \sqrt{deltaX^2 + deltaY^2}>
			const distanceSinceLastPoint: number = Math.sqrt(deltaX * deltaX + deltaY * deltaY);


			// ![](44408656431640F1B13DDCA10C7B507D.png;;;0.04947,0.04744)
			if (distanceSinceLastPoint > minForwardDistanceBetweenPrints) {

				//` <formula 3; \frac{atan2(deltaY,deltaX) * 180}{\pi} + 90^{\circ}>
				const angle = Math.atan2(deltaY, deltaX) * 180 / Math.PI + 90;
				let angleToMovePawPrint = 90;
				if (die.lastPrintOnLeft)
					angleToMovePawPrint = -90;
				die.lastPrintOnLeft = !die.lastPrintOnLeft;
				const printPos: Vector = this.movePointAtAngle(pos, angle + angleToMovePawPrint, leftRightDistanceBetweenPrints);
				const spriteProxy: SpriteProxy = addPrintFunc(printPos.x, printPos.y, die.scale, angle);
				//diceLayer.addPawPrint(pawPrintPos.x, pawPrintPos.y, angle);
				die.lastPos[index] = pos;
				return spriteProxy;
			}
		}
		return null;
	}

	updateDieRollSpecialEffects() {
		for (let i = 0; i < this.dice.length; i++) {
			const die: IDie = this.dice[i];

			if (die.rollType === DieCountsAs.bentLuck)
				this.addTrailingEffects(die, this.diceRollData.secondRollData.trailingEffects);
			else
				this.addTrailingEffects(die, this.diceRollData.trailingEffects);

			if (die.rollType === DieCountsAs.inspiration) {
				const distanceBetweenRipples = 80;
				const ripple: ColorShiftingSpriteProxy = this.old_positionTrailingSprite(die, diceLayer.addRipple.bind(diceLayer), distanceBetweenRipples, 0, this.diceRollData.trailingEffects.length) as ColorShiftingSpriteProxy;

				if (ripple) {
					ripple.opacity = 0.5;
					ripple.hueShift = diceLayer.getHueShift(die.playerID) + Random.plusMinus(30);
				}
			}

			const screenPos: Vector = this.getScreenCoordinates(die);

			if (die.attachedSprites && die.attachedSprites.length > 0 && screenPos) {
				for (let j = 0; j < die.attachedSprites.length; j++) {
					const centerX: number = screenPos.x - die.origins[j].x;
					const centerY: number = screenPos.y - die.origins[j].y;
					const sprite: SpriteProxy = die.attachedSprites[j];
					sprite.x = centerX;
					sprite.y = centerY;
				}
			}

			if (die.attachedLabels && die.attachedLabels.length > 0 && screenPos) {
				for (let k = 0; k < die.attachedLabels.length; k++) {
					const textEffect: TextEffect = die.attachedLabels[k];
					textEffect.x = screenPos.x;
					textEffect.y = screenPos.y;
					textEffect.startX = screenPos.x;
					textEffect.startY = screenPos.y;
				}
			}

			if (die.sparks && screenPos) {
				const newX: number = screenPos.x - diceLayer.diceSparks.originX;
				const newY: number = screenPos.y - diceLayer.diceSparks.originY;
				die.sparks.forEach((spark: SpriteProxy) => {
					const newLocationWeight = 0.1;
					spark.x = spark.x * (1 - newLocationWeight) + newX * newLocationWeight;
					spark.y = spark.y * (1 - newLocationWeight) + newY * newLocationWeight;
				});
			}
		}
	}

	animateDiceRollerFps(nowMs: DOMHighResTimeStamp) {
		try {
			const now: number = Date.now();
			const elapsed: number = now - this.lastDrawTimeDiceRoller;

			const okayToDrawImages: boolean = elapsed > this.fpsIntervalDiceRoller;
			if (!okayToDrawImages)
				return;

			const startUpdate: number = performance.now();
			const testingDiceRoller = false;

			if (!testingDiceRoller) {
				this.updatePhysics();
				this.renderer.render(this.scene, this.camera);
			}

			this.lastDrawTimeDiceRoller = now - (elapsed % this.fpsIntervalDiceRoller);
			this.updateDieRollSpecialEffects();
			diceLayer.renderCanvas();

			this.update();

			if (diceRollerShowFpsWindow) {
				if (!this.diceRollerFpsWindow) {
					this.diceRollerFpsWindow = new FpsWindow('Dice', 1);
				}
				this.diceRollerFpsWindow.showAllFramerates(this.diceRollerTimeBetweenFramesQueue, this.diceRollerDrawTimeForEachFrameQueue, diceLayer.diceFrontContext, now);
			}


			const endUpdate: number = performance.now();
			this.calculateFramerate(startUpdate, endUpdate);
		}
		finally {
			requestAnimationFrame(this.animateDiceRollerFps.bind(this));
		}
	}

	addWallsToScene(wallThickness: number, leftWallWidth: number, leftWallX: number, topWallWidth: number, topWallZ: number, addPlayerWall: boolean, addViewerRollFloor: boolean, playerTopWallWidth: number, wallHeight: number, playerTopWallX: number, playerTopWallZ: number, viewerFloorX: number, viewerFloorY: number, viewerFloorZ: number, addDungeonMasterWalls: boolean, dmBottomWallWidth: number, dmBottomWallX: number, dmBottomWallZ: number, dmLeftWallWidth: number, dmLeftWallX: number, dmLeftWallZ: number) {
		const leftWallHeight = 68;
		const topWallHeight = 146;


		const redWallMaterial =
			// @ts-ignore - THREE
			new THREE.MeshLambertMaterial(
				{
					color: 0xA00050
				});

		const tealWallMaterial =
			// @ts-ignore - THREE
			new THREE.MeshLambertMaterial(
				{
					color: 0x00de7b
				});

		const blueWallMaterial =
			// @ts-ignore - THREE
			new THREE.MeshLambertMaterial(
				{
					color: 0x2000C0
				});

		// @ts-ignore - THREE
		const leftWall = new THREE.Mesh(new THREE.BoxGeometry(wallThickness, leftWallHeight, leftWallWidth), redWallMaterial);
		leftWall.position.x = leftWallX;
		this.scene.add(leftWall);

		// @ts-ignore - THREE
		const topWall = new THREE.Mesh(new THREE.BoxGeometry(topWallWidth, topWallHeight, wallThickness), tealWallMaterial);
		topWall.position.z = topWallZ;
		// @ts-ignore - THREE
		topWall.rotateOnAxis(new THREE.Vector3(1, 0, 0), -45);
		this.scene.add(topWall);

		if (addPlayerWall) {
			// @ts-ignore - THREE
			const playerTopWall = new THREE.Mesh(new THREE.BoxGeometry(playerTopWallWidth, wallHeight, wallThickness), redWallMaterial);
			playerTopWall.position.x = playerTopWallX;
			playerTopWall.position.z = playerTopWallZ;
			this.scene.add(playerTopWall);
		}

		if (addViewerRollFloor) {
			// @ts-ignore - THREE
			const viewerRollFloor = new THREE.Mesh(new THREE.BoxGeometry(wallHeight * 2, wallHeight, 1), blueWallMaterial);
			// @ts-ignore - THREE
			viewerRollFloor.rotateOnAxis(new THREE.Vector3(1, 0, 0), 45);
			viewerRollFloor.position.x = viewerFloorX;
			viewerRollFloor.position.y = viewerFloorY;
			viewerRollFloor.position.z = viewerFloorZ;
			this.scene.add(viewerRollFloor);
		}

		if (addDungeonMasterWalls) {
			// @ts-ignore - THREE
			const dmBottomWall = new THREE.Mesh(new THREE.BoxGeometry(dmBottomWallWidth, wallHeight, wallThickness), redWallMaterial);
			dmBottomWall.position.x = dmBottomWallX;
			dmBottomWall.position.z = dmBottomWallZ;
			this.scene.add(dmBottomWall);

			// @ts-ignore - THREE
			const dmLeftWall = new THREE.Mesh(new THREE.BoxGeometry(wallThickness, wallHeight, dmLeftWallWidth), redWallMaterial);
			dmLeftWall.position.x = dmLeftWallX;
			dmLeftWall.position.z = dmLeftWallZ;
			this.scene.add(dmLeftWall);
		}
	}

	//var maxDieSpeed: number = 0;
	//var minDieSpeed: number = 9999999;
	//function trackDieVelocities(dieSpeed: number) {
	//	if (dieSpeed > maxDieSpeed) {
	//		maxDieSpeed = dieSpeed;
	//		console.log(`maxDieSpeed = ${maxDieSpeed}`);
	//	}

	//	if (dieSpeed < minDieSpeed) {
	//		minDieSpeed = dieSpeed;
	//		//console.log(`minDieSpeed = ${minDieSpeed}`);
	//	}
	//}

	//function placePuff(die: any) {
	//  if (!die.isDamage) {
	//    let pos: Vector = getScreenCoordinates(die.getObject());
	//    let deltaX: number = pos.x - die.lastPos.x;
	//    let deltaY: number = pos.y - die.lastPos.y;

	//    //` <formula 2; \sqrt{deltaX^2 + deltaY^2}>
	//    let distanceSinceLastPoint: number = Math.sqrt(deltaX * deltaX + deltaY * deltaY);


	//    const minForwardDistanceBetweenPuffs = 150;
	//    if (distanceSinceLastPoint > minForwardDistanceBetweenPuffs) {

	//      //` <formula 3; \frac{atan2(deltaY,deltaX) * 180}{\pi} + 90^{\circ}>
	//      let angle = Math.atan2(deltaY, deltaX) * 180 / Math.PI + 90;
	//      diceLayer.addPuff(pos.x, pos.y, angle);
	//      die.lastPos = pos;
	//    }
	//  }
	//}

	//function animate() {
	//	if (!testing) {
	//		updatePhysics();
	//		render();
	//		update();
	//	}

	//	requestAnimationFrame(animate);
	//}


	// @ts-ignore - Called from Connection.ts
	handleFpsChangeDiceRoller(frameRateChangeData: FrameRateChangeData) {
		this.changeFramerateDiceRoller(frameRateChangeData.FrameRate);
	}

	//! Keep for diagnostics
	getDiceValue(die: IDie) {
		for (let i = 0; i < this.diceValues.length; i++) {
			const thisDiceValueEntry = this.diceValues[i];
			if (thisDiceValueEntry.dice === die) {
				if (thisDiceValueEntry.value !== die.getUpsideValue())
					console.error(`thisDiceValueEntry.value (${thisDiceValueEntry.value}) != dice.getUpsideValue() (${die.getUpsideValue()})`);
				return thisDiceValueEntry.value;
			}
		}
		return 0;
	}

	queueRoll(diceRollData: DiceRollData) {
		console.log('queueRoll - TODO');
		// TODO: queue this roll for later when the current roll has stopped.
	}

	addD100(diceRollData: DiceRollData, backgroundColor: string, textColor: string, playerID: number, throwPower = 1, xPositionModifier = 0) {
		const magicRingHueShift: number = Math.floor(Math.random() * 360);
		// @ts-ignore - DiceD10x10
		const die10 = new DiceD10x10({ size: DieRoller.dieScale, backColor: backgroundColor, fontColor: textColor }, this.diceManager);
		die10.scale = 1;
		die10.playerID = playerID;
		this.prepareD10x10Die(diceRollData.diceGroup, die10, throwPower, xPositionModifier);
		die10.rollType = DieCountsAs.totalScore;
		if (diceRollData.isMagic) {
			die10.attachedSprites.push(diceLayer.addMagicRing(960, 540, die10.scale, magicRingHueShift));
			die10.origins.push(new Vector(diceLayer.magicRingRed.originX, diceLayer.magicRingRed.originY));
		}

		// @ts-ignore - DiceD10x01
		const die01 = new DiceD10x01({ size: DieRoller.dieScale, backColor: backgroundColor, fontColor: textColor }, this.diceManager);
		die01.scale = 1;
		die01.playerID = playerID;
		this.prepareD10x01Die(diceRollData.diceGroup, die01, throwPower, xPositionModifier);
		die01.rollType = DieCountsAs.totalScore;
		if (diceRollData.isMagic) {
			die01.attachedSprites.push(diceLayer.addMagicRing(960, 540, die01.scale, magicRingHueShift + Random.plusMinusBetween(10, 25)));
			die01.origins.push(new Vector(diceLayer.magicRingRed.originX, diceLayer.magicRingRed.originY));
		}
	}

	//function render() {
	//	renderer.render(scene, camera);

	//	updateDieRollSpecialEffects();

	//	diceLayer.renderCanvas();
	//}

	rollingOnlyAddOnDice: boolean;
	secondRollTryCount = 0;

	isLuckBent(localDiceRollData: DiceRollData) {
		return localDiceRollData.type === DiceRollType.BendLuckAdd || localDiceRollData.type === DiceRollType.BendLuckSubtract;
	}

	addD20(diceGroup: DiceGroup, diceRollData: DiceRollData, d20BackColor: string, d20FontColor: string, xPositionModifier: number, playerID = -1): IDie {
		// @ts-ignore - DiceD20
		const die: IDie = new DiceD20({ size: DieRoller.dieScale, backColor: d20BackColor, fontColor: d20FontColor }, this.diceManager);
		die.scale = 1;
		die.isD20 = true;
		this.prepareDie(diceGroup, die, diceRollData.throwPower, xPositionModifier);
		die.rollType = DieCountsAs.totalScore;
		die.playerID = playerID;
		return die;
	}

	addBadLuckEffects(die: IDie) {
		die.attachedSprites.push(diceLayer.addBadLuckRing(die, 960, 540));
		die.origins.push(new Vector(diceLayer.badLuckRing.originX, diceLayer.badLuckRing.originY));
	}

	addGoodLuckEffects(die: IDie) {
		const sprite: SpriteProxy = diceLayer.addLuckyRing(die, 960, 540);
		die.attachedSprites.push(sprite);
		die.origins.push(new Vector(diceLayer.cloverRing.originX, diceLayer.cloverRing.originY));
	}

	rollAddOnDice() {
		if (!this.diceRollData)
			return;

		this.allDiceHaveStoppedRolling = false;

		let xPositionModifier = 0;

		if (Math.random() * 100 < 50)
			xPositionModifier = 26;  // Throw from the right to the left.

		const localDiceRollData: DiceRollData = this.getMostRecentDiceRollData();

		let allDiceAdded: IDie[] = [];
		const throwPower: number = this.diceRollData.throwPower * 1.2;

		let dieBack: string;
		let dieFont: string;

		if (this.diceRollData.bentLuckMultiplier < 0) {
			// Bad luck
			dieBack = DiceLayer.badLuckDieColor;
			dieFont = DiceLayer.badLuckFontColor;
		}
		else {
			dieBack = DiceLayer.goodLuckDieColor;
			dieFont = DiceLayer.goodLuckFontColor;
		}

		if (this.isLuckBent(localDiceRollData)) {
			allDiceAdded = this.addDie(localDiceRollData.diceGroup, 'd4', DamageType.None, DieCountsAs.bentLuck, dieBack, dieFont, throwPower, xPositionModifier, false);
			allDiceAdded.forEach((die: IDie) => {
				die.isLucky = true;
			});
		}
		else {
			if (localDiceRollData.itsAD20Roll) {   // TODO: Send itsAD20Roll from DM app.
				allDiceAdded.push(this.addD20(localDiceRollData.diceGroup, localDiceRollData, dieBack, dieFont, xPositionModifier));
				allDiceAdded[0].isLucky = true;  // TODO: Send IsLucky from DM app.
			}
			else {
				//let saveDamageModifier: number = damageModifierThisRoll;
				this.addDieFromStr(localDiceRollData.diceGroup, localDiceRollData.playerRollOptions[0].PlayerID, localDiceRollData.damageHealthExtraDice, DieCountsAs.totalScore, throwPower);
				//damageModifierThisRoll += saveDamageModifier;
			}
		}

		const isGoodLuck: boolean = this.isLuckBent(localDiceRollData) && this.diceRollData.bentLuckMultiplier > 0 || localDiceRollData.type === DiceRollType.LuckRollHigh;
		const isBadLuck: boolean = this.isLuckBent(localDiceRollData) && this.diceRollData.bentLuckMultiplier < 0 || localDiceRollData.type === DiceRollType.LuckRollLow;

		if (isGoodLuck) {
			//console.log('addGoodLuckEffects...');
			allDiceAdded.forEach((die: IDie) => {
				this.addGoodLuckEffects(die);
			});

		}
		else if (isBadLuck) {
			//console.log('addBadLuckEffects...');
			allDiceAdded.forEach((die: IDie) => {
				this.addBadLuckEffects(die);
			});
		}
	}

	throwAdditionalDice() {
		this.secondRollTryCount++;

		if (!this.rollingOnlyAddOnDice && !this.allDiceHaveStoppedRolling && this.secondRollTryCount < 30) {
			setTimeout(this.throwAdditionalDice.bind(this), 300);
			return;
		}

		this.freezeExistingDice();
		diceLayer.clearTextEffects(this.diceRollData.diceGroup);

		const localDiceRollData: DiceRollData = this.getMostRecentDiceRollData();

		if (this.isLuckBent(localDiceRollData))
			diceLayer.reportAddOnRoll(this.diceRollData.secondRollTitle, this.diceRollData.bentLuckMultiplier, this.diceRollData.diceGroup);
		else {
			if (!this.diceRollData.damageHealthExtraDice)
				localDiceRollData.itsAD20Roll = true;
			else if (localDiceRollData.type === DiceRollType.LuckRollHigh || localDiceRollData.type === DiceRollType.LuckRollLow)
				localDiceRollData.itsAD20Roll = true;

			diceLayer.reportAddOnRoll(this.diceRollData.secondRollTitle, this.diceRollData.bentLuckMultiplier, this.diceRollData.diceGroup);
		}

		diceSounds.safePlayMp3('PaladinThunder');
		if (this.rollingOnlyAddOnDice)
			this.rollAddOnDice();
		else
			setTimeout(this.rollAddOnDice.bind(this), 2500);
	}

	needToRollAddOnDice(diceRollDto: DiceRollData, bentLuckMultiplier = 0) {
		if (this.diceRollData && this.diceRollData.secondRollData)
			return;

		if (!this.diceRollData || this.diceRollData === undefined) {
			console.log(`setting this.diceRollData to ${diceRollDto}...`);
			this.diceRollData = diceRollDto;
			this.rollingOnlyAddOnDice = true;
		}

		this.diceRollData.bentLuckMultiplier = bentLuckMultiplier;
		this.diceRollData.secondRollData = diceRollDto;
		this.diceRollData.secondRollData.vantageKind = this.diceRollData.vantageKind;

		this.secondRollTryCount = 0;
		this.throwAdditionalDice();
	}

	getDiceInPlay(): number {
		let diceInPlay = 0;
		for (let i = 0; i < this.dice.length; i++) {
			const die: IDie = this.dice[i];
			if (die.inPlay) {
				diceInPlay++;
			}
		}

		return diceInPlay;
	}

	testD20Removal(roll1: number, roll2: number, roll3: number, vantage: VantageKind, type: DiceRollType): MockDie {
		this.dice = [];
		this.diceRollData.vantageKind = vantage;
		this.diceRollData.type = type;
		this.diceRollData.itsAD20Roll = true;
		this.dice.push(new MockDie('A', roll1));
		this.dice.push(new MockDie('B', roll2));

		let diceInPlay: number = this.getDiceInPlay();

		if (diceInPlay !== 2)
			console.error('a) diceInPlay: ' + diceInPlay);
		this.removeD20s();
		diceInPlay = this.getDiceInPlay();
		if (diceInPlay !== 1)
			console.error('b) diceInPlay: ' + diceInPlay);

		this.dice.push(new MockDie('C', roll3));
		this.removeD20s();
		diceInPlay = 0;
		let returnDie: MockDie;
		for (let i = 0; i < this.dice.length; i++) {
			const die: IDie = this.dice[i];
			if (die.inPlay) {
				diceInPlay++;
				returnDie = die;
			}
		}
		if (diceInPlay !== 1)
			console.error('c) diceInPlay: ' + diceInPlay);
		return returnDie;
	}

	//! Keep for diagnostics
	d20RollIncludes(topNumber: number): boolean {
		for (let i = 0; i < this.dice.length; i++) {
			const die: IDie = this.dice[i];

			if (die.isD20 && topNumber === die.getTopNumber())
				return true;
		}
		return false;
	}

	addD20sForPlayer(playerID: number, xPositionModifier: number, kind: VantageKind, inspiration = '', numD20s = 1, dieLabelOverride: string = null) {
		const d20BackColor: string = diceLayer.getDieColor(playerID);
		const d20FontColor: string = diceLayer.getDieFontColor(playerID);
		if (kind !== VantageKind.Normal)
			numD20s = 2;

		const magicRingHueShift: number = Math.floor(Math.random() * 360);

		for (let i = 0; i < numD20s; i++) {
			const die = this.addD20(this.diceRollData.diceGroup, this.diceRollData, d20BackColor, d20FontColor, xPositionModifier);
			if (dieLabelOverride)
				die.dieType = dieLabelOverride;
			else
				die.dieType = DiceRollType[this.diceRollData.type];
			this.attachLabel(die, d20FontColor, d20BackColor);
			die.playerID = playerID;
			die.playerName = diceLayer.getPlayerName(playerID);
			//console.log('die.playerName: ' + die.playerName);
			die.kind = kind;
			if (this.diceRollData.isMagic) {
				die.attachedSprites.push(diceLayer.addMagicRing(960, 540, die.scale, magicRingHueShift + Random.plusMinusBetween(10, 25)));
				die.origins.push(new Vector(diceLayer.magicRingRed.originX, diceLayer.magicRingRed.originY));
			}
			if (this.diceRollData.numHalos > 0) {
				const angleDelta: number = 360 / this.diceRollData.numHalos;
				let angle: number = Math.random() * 360;
				for (let j = 0; j < this.diceRollData.numHalos; j++) {
					die.attachedSprites.push(diceLayer.addHaloSpin(960, 540, die.scale, diceLayer.activePlayerHueShift + Random.plusMinus(30), angle));
					die.origins.push(diceLayer.haloSpinRed.getOrigin());
					angle += angleDelta;
				}
			}
		}

		if (inspiration) {
			this.addDieFromStr(this.diceRollData.diceGroup, playerID, inspiration, DieCountsAs.inspiration, this.diceRollData.throwPower, xPositionModifier, d20BackColor, d20FontColor, this.diceRollData.isMagic);
		}
	}

	addDiceFromDto(diceRollDto: DiceRollData, diceDto: DiceDto, xPositionModifier: number) {
		// TODO: Check DieCountsAs.totalScore - do we want to set that from C# side of things?
		//console.log('addDiceFromDto - diceDto.DieCountsAs: ' + DieCountsAs[diceDto.DieCountsAs].toString());
		if (diceDto.Sides === 20) {
			diceRollDto.itsAD20Roll = true;
		}

		if (diceDto.Vantage !== VantageKind.Normal && diceDto.Quantity === 1 && diceDto.Sides === 20) {
			diceDto.Quantity = 2;
		}
		const allDice: IDie[] = this.createDie(diceRollDto.diceGroup, diceDto.Quantity, diceDto.Sides, diceDto.DamageType, diceDto.DieCountsAs, diceDto.BackColor, diceDto.FontColor, this.diceRollData.throwPower, xPositionModifier, diceDto.IsMagic, diceDto.CreatureId, diceDto.Scale);

		//console.log('addDiceFromDto - diceDto.PlayerName: ' + diceDto.PlayerName);
		//console.log('diceDto.DamageType: ' + DamageType[diceDto.DamageType].toString());

		allDice.forEach((die: IDie) => {
			die.playerName = diceDto.PlayerName;
			die.dataStr = diceDto.Data;
			die.rollType = diceDto.DieCountsAs;
			die.dieType = DiceRollType[DiceRollType.None];
			if (diceDto.Label)
				diceLayer.attachLabel(die, diceDto.Label, diceDto.FontColor, diceDto.BackColor, diceDto.Scale, this.diceRollData.diceGroup); // So the text matches the die color.
			die.kind = diceDto.Vantage;
			if (die.kind === VantageKind.Advantage) {
				//console.log(`die has advantage!`);
			}
			else if (die.kind === VantageKind.Disadvantage) {
				//console.log(`die has disadvantage!`);
			}
			if (diceDto.IsMagic) {
				die.attachedSprites.push(diceLayer.addMagicRing(960, 540, die.scale, Random.max(360)));
				die.origins.push(new Vector(diceLayer.magicRingRed.originX, diceLayer.magicRingRed.originY));
			}
		});
		//if (diceRollData.numHalos > 0) {
		//	let angleDelta: number = 360 / diceRollData.numHalos;
		//	let angle: number = Math.random() * 360;
		//	for (var j = 0; j < diceRollData.numHalos; j++) {
		//		die.attachedSprites.push(diceLayer.addHaloSpin(960, 540, diceLayer.activePlayerHueShift + Random.plusMinus(30), angle));
		//		die.origins.push(diceLayer.haloSpinRed.getOrigin());
		//		angle += angleDelta;
		//	}
		//}
	}

	prepareDiceDtoRoll(diceRollDto: DiceRollData, xPositionModifier: number) {
		//console.log(diceRollDto.diceDtos);

		for (let i = 0; i < diceRollDto.diceDtos.length; i++) {
			const diceDto: DiceDto = diceRollDto.diceDtos[i];
			this.addDiceFromDto(diceRollDto, diceDto, xPositionModifier);
		}

		this.diceRollData.hasMultiPlayerDice = this.hasMultiPlayerDice(diceRollDto);  // Any DiceDtos (even one) will go in a multiplayerSummary!
		this.diceRollData.hasSingleIndividual = !this.diceRollData.hasMultiPlayerDice;

		//console.log('prepareDiceDtoRoll - diceRollData.hasMultiPlayerDice: ' + this.diceRollDataPlayer.hasMultiPlayerDice);
	}

	prepareLegacyRoll(xPositionModifier: number) {
		let playerID = -1;
		if (this.diceRollData.playerRollOptions.length === 1)
			playerID = this.diceRollData.playerRollOptions[0].PlayerID;
		if (this.diceRollData.type === DiceRollType.WildMagic) {
			this.diceRollData.modifier = 0;
			this.diceRollData.itsAD20Roll = false;
			this.addD100(this.diceRollData, diceLayer.activePlayerDieColor, diceLayer.activePlayerDieFontColor, playerID, this.diceRollData.throwPower, xPositionModifier);
		}
		else if (this.diceRollData.type === DiceRollType.PercentageRoll) {
			this.diceRollData.modifier = 0;
			this.diceRollData.itsAD20Roll = false;
			if (this.diceRollData.rollScope === RollScope.ActivePlayer) {
				this.addD100(this.diceRollData, diceLayer.activePlayerDieColor, diceLayer.activePlayerDieFontColor, playerID, this.diceRollData.throwPower, xPositionModifier);
				this.diceRollData.hasSingleIndividual = true;
			}
			else if (this.diceRollData.rollScope === RollScope.Individuals) {
				this.diceRollData.playerRollOptions.forEach((playerRollOption: PlayerRollOptions) => {
					this.addD100(this.diceRollData, diceLayer.activePlayerDieColor, diceLayer.activePlayerDieFontColor, playerRollOption.PlayerID, this.diceRollData.throwPower, xPositionModifier);
				});
				this.diceRollData.hasMultiPlayerDice = this.diceRollData.playerRollOptions.length > 1;
				//console.log('prepareLegacyRoll, RollScope.Individuals - diceRollData.hasMultiPlayerDice: ' + this.diceRollDataPlayer.hasMultiPlayerDice);

				this.diceRollData.hasSingleIndividual = this.diceRollData.playerRollOptions.length === 1;
			}
		}
		else if (this.diceRollData.type === DiceRollType.Initiative || this.diceRollData.type === DiceRollType.NonCombatInitiative) {
			this.diceRollData.modifier = 0;
			this.diceRollData.maxInspirationDiceAllowed = 4;
			this.diceRollData.itsAD20Roll = true;
			for (let i = 0; i < diceLayer.players.length; i++) {
				const player: Character = diceLayer.players[i];
				if (player.Hidden)
					continue;
				let initiativeBonus: number;
				if (this.diceRollData.type === DiceRollType.NonCombatInitiative)
					initiativeBonus = 0;
				else
					initiativeBonus = player.rollInitiative;
				this.addD20sForPlayer(player.playerID, xPositionModifier, initiativeBonus, player.inspiration);
			}
			this.diceRollData.hasMultiPlayerDice = true;
		}
		else if (this.diceRollData.type === DiceRollType.DamageOnly) {
			this.diceRollData.modifier = 0;
			this.diceRollData.itsAD20Roll = false;
			this.addDieFromStr(this.diceRollData.diceGroup, playerID, this.diceRollData.damageHealthExtraDice, DieCountsAs.damage, this.diceRollData.throwPower, xPositionModifier, undefined, undefined, this.diceRollData.isMagic);
		}
		else if (this.diceRollData.type === DiceRollType.HealthOnly) {
			this.diceRollData.modifier = 0;
			this.diceRollData.itsAD20Roll = false;
			this.addDieFromStr(this.diceRollData.diceGroup, playerID, this.diceRollData.damageHealthExtraDice, DieCountsAs.health, this.diceRollData.throwPower, xPositionModifier, DiceLayer.healthDieBackgroundColor, DiceLayer.healthDieFontColor, this.diceRollData.isMagic);
		}
		else if (this.diceRollData.type === DiceRollType.ExtraOnly) {
			this.diceRollData.modifier = 0;
			this.diceRollData.itsAD20Roll = false;
			this.addDieFromStr(this.diceRollData.diceGroup, playerID, this.diceRollData.damageHealthExtraDice, DieCountsAs.extra, this.diceRollData.throwPower, xPositionModifier, DiceLayer.extraDieBackgroundColor, DiceLayer.extraDieFontColor, this.diceRollData.isMagic);
		}
		else if (this.diceRollData.type === DiceRollType.InspirationOnly) {
			this.diceRollData.modifier = 0;
			this.diceRollData.itsAD20Roll = false;
			for (let i = 0; i < this.diceRollData.playerRollOptions.length; i++) {
				const playerRollOptions: PlayerRollOptions = this.diceRollData.playerRollOptions[i];
				this.addD20sForPlayer(playerRollOptions.PlayerID, xPositionModifier, playerRollOptions.VantageKind, playerRollOptions.Inspiration, 0);
			}
		}
		else if (this.diceRollData.damageHealthExtraDice.indexOf('d20') >= 0 && this.diceRollData.vantageKind === VantageKind.Normal) {
			let dieStr: string = this.diceRollData.damageHealthExtraDice;
			if (dieStr === '1d20("Wild Magic Check")') {
				const numD20s: number = this.diceRollData.modifier;
				this.diceRollData.modifier = 0;
				dieStr = numD20s.toString() + 'd20("Wild Magic Check")';
			}
			const d20BackColor: string = diceLayer.getDieColor(playerID);
			const d20FontColor: string = diceLayer.getDieFontColor(playerID);
			//addDieFromStr(playerID, dieStr, DieCountsAs.totalScore, diceRollData.throwPower, xPositionModifier, diceLayer.activePlayerDieColor, diceLayer.activePlayerDieFontColor);
			this.addDieFromStr(this.diceRollData.diceGroup, playerID, dieStr, DieCountsAs.totalScore, this.diceRollData.throwPower, xPositionModifier, d20BackColor, d20FontColor);
		}
		else {
			//console.log(`diceRollData.itsAD20Roll = true;`);
			this.diceRollData.itsAD20Roll = true;
			if (this.diceRollData.rollScope === RollScope.ActivePlayer) {
				const activePlayerRollOptions: PlayerRollOptions = this.diceRollData.playerRollOptions[0];
				// TODO: I think there's a bug here active player's inspiration needs to be used.
				let vantageKind: VantageKind = this.diceRollData.vantageKind;
				if (this.diceRollData.playerRollOptions.length === 1) {
					vantageKind = activePlayerRollOptions.VantageKind;
				}
				let numD20s = 1;
				let dieLabel = '';
				if (this.diceRollData.type === DiceRollType.WildMagicD20Check) {
					numD20s = this.diceRollData.modifier;
					this.diceRollData.modifier = 0;
					dieLabel = '"Wild Magic Check"';
				}
				let playerId: number = diceLayer.playerID;
				if (activePlayerRollOptions)
					playerId = activePlayerRollOptions.PlayerID;
				this.addD20sForPlayer(playerId, xPositionModifier, vantageKind, this.diceRollData.groupInspiration, numD20s, dieLabel);
				this.diceRollData.hasSingleIndividual = true;
			}
			else if (this.diceRollData.rollScope === RollScope.Individuals) {
				this.diceRollData.playerRollOptions.forEach((playerRollOption: PlayerRollOptions) => {
					this.addD20sForPlayer(playerRollOption.PlayerID, xPositionModifier, playerRollOption.VantageKind, playerRollOption.Inspiration);
				});
				this.diceRollData.hasMultiPlayerDice = this.diceRollData.playerRollOptions.length > 1;
				//console.log('prepareLegacyRoll, RollScope.Individuals (2) - diceRollData.hasMultiPlayerDice: ' + this.diceRollDataPlayer.hasMultiPlayerDice);
				this.diceRollData.hasSingleIndividual = this.diceRollData.playerRollOptions.length === 1;
			}
			if (this.isAttack(this.diceRollData)) {
				this.addDieFromStr(this.diceRollData.diceGroup, playerID, this.diceRollData.damageHealthExtraDice, DieCountsAs.damage, this.diceRollData.throwPower, xPositionModifier);
			}
		}
		return playerID;
	}

	//! Called from Connection.ts
	pleaseRollDice(diceRollDto: DiceRollData) {
		//console.log(`pleaseRollDice...`);
		DiceLayer.numFiltersOnDieCleanup = 0;
		DiceLayer.numFiltersOnRoll = 0;
		//testing = true;
		//diceRollData = diceRollDto;

		//if (testD20Removal(3, 2, 1, DiceRollKind.Disadvantage, DiceRollType.LuckRollHigh).name != 'A') debugger;
		//if (testD20Removal(4, 5, 7, DiceRollKind.Disadvantage, DiceRollType.LuckRollLow).name != 'B') debugger;
		//return;

		this.animationsShouldBeDone = false;
		this.throwHasStarted = false;
		this.rollingOnlyAddOnDice = false;

		if (diceRollDto.type === DiceRollType.BendLuckAdd || diceRollDto.type === DiceRollType.LuckRollHigh) {
			this.needToRollAddOnDice(diceRollDto, +1);
			return;
		}
		else if (diceRollDto.type === DiceRollType.BendLuckSubtract || diceRollDto.type === DiceRollType.LuckRollLow) {
			this.needToRollAddOnDice(diceRollDto, -1);
			return;
		}
		else if (diceRollDto.type === DiceRollType.AddOnDice) {
			this.needToRollAddOnDice(diceRollDto, 0);
			return;
		}

		this.diceRollData = diceRollDto;
		this.diceRollData.timeLastRolledMs = performance.now();
		this.attemptedRollWasSuccessful = false;
		this.wasCriticalHit = false;
		this.attemptedRollWasNarrowlySuccessful = false;

		if (this.randomDiceThrowIntervalId !== 0) {
			clearInterval(this.randomDiceThrowIntervalId);
			this.randomDiceThrowIntervalId = 0;
		}

		// @ts-ignore - DiceManager
		if (this.diceManager.throwRunning) {
			this.queueRoll(this.diceRollData);
			return;
		}

		this.clearBeforeRoll(this.diceRollData.diceGroup);

		let xPositionModifier = 0;

		if (Math.random() * 100 < 50)
			xPositionModifier = 26;  // Throw from the right to the left.

		if (diceRollDto.diceDtos && diceRollDto.diceDtos.length > 0) {
			//console.log('prepareDiceDtoRoll...');
			this.prepareDiceDtoRoll(diceRollDto, xPositionModifier);
			//console.log(this.dice);
		}


		if (!diceRollDto.suppressLegacyRoll) {
			//console.log('prepareLegacyRoll...');
			this.prepareLegacyRoll(xPositionModifier);
		}
		//console.log(this.dice);

		try {
			// @ts-ignore - DiceManager
			this.diceManager.prepareValues(this.diceValues);
			this.throwHasStarted = true;
		}
		catch (ex) {
			console.log('exception on call to this.diceManager.prepareValues: ' + ex);
		}

		if (this.diceRollData.onThrowSound) {
			diceSounds.safePlayMp3(this.diceRollData.onThrowSound);
		}
		//startedRoll = true;
	}
}

diceRollerPlayers = new DieRoller('Players');
diceRollerPlayers.init();

diceRollerViewers = new DieRoller('Viewers');
diceRollerViewers.init();

