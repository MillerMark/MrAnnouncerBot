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
	clear();
	getTopNumber(): number;
}

let diceRollData: DiceRollData;
//const showDieValues = false;
let totalDamagePlusModifier = 0;
let totalHealthPlusModifier = 0;
let totalExtraPlusModifier = 0;
let totalBonus = 0;
let d20RollValue = -1;
let attemptedRollWasSuccessful = false;
let wasCriticalHit = false;
let attemptedRollWasNarrowlySuccessful = false;
const diceToRoll = 10;
const secondsBetweenRolls = 12;
const removeDiceImmediately = false;
const dieScale = 1.5;
const repeatRandomThrow = false;
let onBonusThrow = false;
let setupBonusRoll = false;
let waitingForBonusRollToComplete = false;
let bonusRollStartTime = 0;
let randomDiceThrowIntervalId = 0;
let damageModifierThisRoll = 0;
let healthModifierThisRoll = 0;
let extraModifierThisRoll = 0;
let additionalDieRollMessage = '';

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

let container, scene, camera, renderer, controls, stats, world, dice = [];
let scalingDice: IDie[] = [];
let specialDice: IDie[] = [];
//var bodiesToremove = [];
//let bodiesToFree = [];
const diceSounds = new DiceSounds('GameDev/Assets/DragonH/SoundEffects');

let waitingForSettle = false;
let allDiceHaveStoppedRolling = false;
let firstStopTime: number;
let diceValues = [];

function setNormalGravity() {
	world.gravity.set(0, -9.82 * 20, 0);
}

function restoreDieScale() {
	for (let i = 0; i < dice.length; i++) {
		const die = dice[i].getObject();
		if (die)
			die.scale.set(1, 1, 1);
	}
}

function clearAllDice() {
	if (!dice || dice.length === 0)
		return;
	for (let i = 0; i < dice.length; i++) {
		const die: IDie = dice[i];
		const dieObject: IDieObject = die.getObject();
		if (dieObject) {
			scene.remove(dieObject);
			die.clear();
		}
	}
	dice = [];
	specialDice = [];
	scalingDice = [];
}

function clearBeforeRoll() {
	additionalDieRollMessage = '';
	diceLayer.clearResidualEffects();
	scalingDice = [];
	specialDice = [];
	restoreDieScale();
	setNormalGravity();
	waitingForSettle = true;
	setupBonusRoll = false;
	waitingForBonusRollToComplete = false;
	onBonusThrow = false;
	diceValues = [];
	totalDamagePlusModifier = 0;
	totalHealthPlusModifier = 0;
	totalExtraPlusModifier = 0;
	diceRollData.totalRoll = 0;
	d20RollValue = -1;
	totalBonus = 0;
	damageModifierThisRoll = 0;
	//console.log('damageModifierThisRoll = 0;');
	healthModifierThisRoll = 0;
	extraModifierThisRoll = 0;
	clearAllDice();
	allDiceHaveStoppedRolling = false;
}

let fpsIntervalDiceRoller: number;
//let startTimeDiceRoller: number;
let lastDrawTimeDiceRoller: number;

function changeFramerateDiceRoller(fps: number): any {
	if (fps === -1)
		return;
	fpsIntervalDiceRoller = 1000 / fps;
	lastDrawTimeDiceRoller = Date.now();
	//startTimeDiceRoller = lastDrawTimeDiceRoller;
}

function getScreenCoordinates(dieObject: IDieObject): Vector {
	if (dieObject === null)
		return null;
	// @ts-ignore - THREE
	const screenVector = new THREE.Vector3();
	dieObject.localToWorld(screenVector);

	screenVector.project(camera);

	const x = Math.round((screenVector.x + 1) * renderer.domElement.offsetWidth / 2);
	const y = Math.round((1 - screenVector.y) * renderer.domElement.offsetHeight / 2);

	return new Vector(x, y);
}

function dieFirstHitsFloor(die) {
	if (diceRollData.onFirstContactSound) {
		diceSounds.safePlayMp3(diceRollData.onFirstContactSound);
		diceRollData.onFirstContactSound = null;
	}

	if (die.rollType === DieCountsAs.inspiration) {
		diceSounds.safePlayMp3('inspiration');
	}

	if (die.rollType === DieCountsAs.totalScore || die.rollType === DieCountsAs.inspiration) {
		// Move the effect closer to the center of the screen...
		const percentageOnDie = 0.7;
		const percentageOffDie: number = 1 - percentageOnDie;


		if (diceRollData.onFirstContactEffect) {
			const pos: Vector = getScreenCoordinates(die.getObject());
			if (pos) {
				const x: number = pos.x * percentageOnDie + percentageOffDie * 960;
				const y: number = pos.y * percentageOnDie + percentageOffDie * 540;

				diceLayer.AddEffect(diceRollData.onFirstContactEffect, x, y, diceRollData.effectScale,
					diceRollData.effectHueShift, diceRollData.effectSaturation,
					diceRollData.effectBrightness, diceRollData.effectRotation);
			}
		}
	}
}

function trailsSparks(diceRollData: DiceRollData): boolean {
	// mkm diceRollData.type == DiceRollType.WildMagic || 
	return diceRollData.type === DiceRollType.BendLuckAdd || diceRollData.type === DiceRollType.BendLuckSubtract;
}

function handleDieCollision(e) {
	// @ts-ignore - DiceManager
	if (DiceManager.throwRunning || dice.length === 0)
		return;

	const relativeVelocity: number = Math.abs(Math.round(e.contact.getImpactVelocityAlongNormal()));
	//console.log(e.target.name + ' -> ' + e.body.name + ' at ' + relativeVelocity + 'm/s');

	if (e.target.name === "die" && e.body.name === "die")
		diceSounds.playDiceHit(relativeVelocity / 10);
	else if (e.target.name === "die" && e.body.name === "floor") {
		if (e.target.parentDie.hasNotHitFloorYet) {
			e.target.parentDie.hasNotHitFloorYet = false;
			dieFirstHitsFloor(e.target.parentDie);
		}
		if (relativeVelocity < 8) {
			diceSounds.playSettle();
		}
		else {
			diceSounds.playFloorHit(relativeVelocity / 35);

			if (trailsSparks(diceRollData)) {
				if (relativeVelocity > 12) {
					if (!e.target.parentDie.sparks)
						e.target.parentDie.sparks = [];
					const pos: Vector = getScreenCoordinates(e.target.parentDie.getObject());
					if (pos)
						e.target.parentDie.sparks.push(diceLayer.smallSpark(pos.x, pos.y));
					diceSounds.safePlayMp3('Dice/Zap[4]');
				}
			}
		}
	}
	else if (e.target.name === "die" && e.body.name === "wall")
		diceSounds.playWallHit(relativeVelocity / 40);
}

let needToClearD20s = true;

function getRandomEffect() {
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

function getDieEffectDistance(): number {
	return Math.round(Math.random() * 5) * 40 + 40;
}

function removeDie(die: IDie, dieEffectInterval: number, effectOverride: DieEffect = undefined) {
	if (!die.inPlay)
		return;

	if (effectOverride === undefined || effectOverride === DieEffect.Random) {
		let numTries = 0;
		effectOverride = getRandomEffect();
		while (effectOverride === DieEffect.Random && numTries < 10) {
			effectOverride = getRandomEffect();
			numTries++;
		}
		if (effectOverride === DieEffect.Random)
			effectOverride = DieEffect.Bomb;
	}

	const dieObject: IDieObject = die.getObject();
	if (dieObject) {
		dieObject.removeTime = performance.now();
		dieObject.effectKind = effectOverride;
		dieEffectInterval += getDieEffectDistance();
		dieObject.effectStartOffset = dieEffectInterval;
		dieObject.needToStartEffect = true;
		specialDice.push(die);  // DieEffect.Portal too???

		if (effectOverride === DieEffect.FuturisticGroundPortal) {
			dieObject.hideOnScaleStop = true;
			dieObject.needToDrop = true;
			scalingDice.push(die);

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
			scalingDice.push(die);
		}
		else if (effectOverride === DieEffect.SteamPunkTunnel) {
			dieObject.hideOnScaleStop = true;
			dieObject.needToDrop = true;
			scalingDice.push(die);
		}
	}

	die.inPlay = false;
	return dieEffectInterval;
}

function removeDieEffectsForSingleDie(die: IDie) {
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

function removeSingleDieWithEffect(die: IDie, dieEffect: DieEffect, effectInterval: number) {
	removeDie(die, effectInterval, dieEffect);
	for (let i = 0; i < dice.length; i++) {
		if (dice[i] === die) {
			die.inPlay = false; // This line appears to be redundant.
			removeDieEffectsForSingleDie(die);
		}
	}
}

function handRemoveDie(die: IDie, effectInterval = 0) {
	if (!die)
		return;
	removeSingleDieWithEffect(die, DieEffect.HandGrab, effectInterval);
}

function removeNonVantageDieNow(die: IDie) {
	if (!die)
		return;
	diceRollData.appliedVantage = true;
	handRemoveDie(die, 0);
}

function isAttack(diceRollData: DiceRollData): boolean {
	return diceRollData.type === DiceRollType.Attack || diceRollData.type === DiceRollType.ChaosBolt;
}

function getMostRecentDiceRollData() {
	if (diceRollData.secondRollData)
		return diceRollData.secondRollData;
	else
		return diceRollData;
}

function removeMultiplayerD20s(): void {
	//console.log('removeMultiplayerD20s...');
	needToClearD20s = false;
	const localDiceRollData: DiceRollData = getMostRecentDiceRollData();
	const isWildMagic: boolean = localDiceRollData.type === DiceRollType.WildMagic;
	if (isWildMagic || !localDiceRollData.itsAD20Roll)
		return;
	const vantageTextDelay = 900;
	//console.log('diceRollData.appliedVantage: ' + diceRollData.appliedVantage);

	const playerEdgeRolls: Array<number> = [];
	const otherPlayersDie: Array<IDie> = [];
	for (let j = 0; j < 20; j++) {
		playerEdgeRolls.push(-1);
		otherPlayersDie.push(null);
	}

	for (let i = 0; i < dice.length; i++) {
		const die: IDie = dice[i];
		const playerId: number = die.playerID;
		const topNumber = die.getTopNumber();

		if (die.isD20) {
			if (playerEdgeRolls[playerId] === -1)
				playerEdgeRolls[playerId] = topNumber;
			else if (die.kind === VantageKind.Advantage) {
				if (playerEdgeRolls[playerId] <= topNumber) {
					removeNonVantageDieNow(otherPlayersDie[playerId]);
					diceLayer.addAdvantageText(otherPlayersDie[playerId], vantageTextDelay);
					playerEdgeRolls[playerId] = topNumber;
				}
				else {  // Disadvantage
					removeNonVantageDieNow(die);
					diceLayer.addAdvantageText(die, vantageTextDelay);
				}
			}
			else if (die.kind === VantageKind.Disadvantage) {
				if (playerEdgeRolls[playerId] >= topNumber) {
					removeNonVantageDieNow(otherPlayersDie[playerId]);
					diceLayer.addDisadvantageText(otherPlayersDie[playerId], vantageTextDelay);
					playerEdgeRolls[playerId] = topNumber;
				}
				else {
					removeNonVantageDieNow(die);
					diceLayer.addDisadvantageText(die, vantageTextDelay);
				}
			}
			otherPlayersDie[playerId] = die;
		}
	}
}

function flameOn(die: IDie) {
	const dieObject = die.getObject();

	const screenPos: Vector = getScreenCoordinates(dieObject);
	if (!screenPos)
		return;

	diceLayer.addFireball(screenPos.x, screenPos.y);
	diceLayer.attachDamageFire(die, diceLayer.getHueShift(die.playerID) - 15);
	diceSounds.playFireball();
}

function groundBurstDie(die: IDie, effectInterval = 0) {
	if (!die)
		return;
	removeSingleDieWithEffect(die, DieEffect.GroundBurst, effectInterval);
}

function removeWildMagicRollsGreaterThanOne(): void {
	//let localDiceRollData: DiceRollData = getMostRecentDiceRollData(diceRollData);
	let interval = 200;
	const timeBetweenDieRemoval = 300;
	const intervalVariance = 100;
	for (let i = 0; i < dice.length; i++) {
		const die: IDie = dice[i];

		const topNumber = die.getTopNumber();

		if (topNumber === 1) {
			flameOn(die);
		}
		else {
			//handRemoveDie(die, interval);
			groundBurstDie(die, interval);
			interval += timeBetweenDieRemoval + Random.plusMinus(intervalVariance);
		}
	}
}

function removeD20s(): number {
	//console.log('removeD20s...');
	needToClearD20s = false;
	let edgeRollValue = -1;
	const localDiceRollData: DiceRollData = getMostRecentDiceRollData();
	const isLuckRollHigh: boolean = localDiceRollData.type === DiceRollType.LuckRollHigh;
	const isLuckRollLow: boolean = localDiceRollData.type === DiceRollType.LuckRollLow;
	const isLuckRoll: boolean = isLuckRollHigh || isLuckRollLow;
	const isWildMagic: boolean = localDiceRollData.type === DiceRollType.WildMagic;

	if (localDiceRollData.type === DiceRollType.WildMagicD20Check) {
		removeWildMagicRollsGreaterThanOne();
		return;
	}

	if (isWildMagic || !localDiceRollData.itsAD20Roll)
		return edgeRollValue;
	let otherDie = null;
	const vantageTextDelay = 900;
	//console.log('diceRollData.appliedVantage: ' + diceRollData.appliedVantage);

	if (!diceRollData.appliedVantage) {
		for (let i = 0; i < dice.length; i++) {
			const die: IDie = dice[i];
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
						removeNonVantageDieNow(otherDie);
						diceLayer.addAdvantageText(otherDie, vantageTextDelay);
						edgeRollValue = topNumber;
					}
					else {  // Disadvantage
						removeNonVantageDieNow(die);
						diceLayer.addAdvantageText(die, vantageTextDelay);
					}
				}
				else if (die.kind === VantageKind.Disadvantage) {
					if (edgeRollValue >= topNumber) {
						removeNonVantageDieNow(otherDie);
						//if (!localDiceRollData.showedVantageMessage) {
						//	localDiceRollData.showedVantageMessage = true;
						diceLayer.addDisadvantageText(otherDie, vantageTextDelay);
						//}
						edgeRollValue = topNumber;
					}
					else {
						removeNonVantageDieNow(die);
						//if (!localDiceRollData.showedVantageMessage) {
						//	localDiceRollData.showedVantageMessage = true;
						diceLayer.addDisadvantageText(die, vantageTextDelay);
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
		for (let i = 0; i < dice.length; i++) {
			const die: IDie = dice[i];
			if (!die.inPlay)
				continue;
			const topNumber: number = die.getTopNumber();

			if (die.isD20) {
				if (edgeRollValue === -1)
					edgeRollValue = topNumber;
				else if (isLuckRollHigh) {
					if (edgeRollValue <= topNumber) {
						removeNonVantageDieNow(otherDie);
						//if (!localDiceRollData.showedVantageMessage) {
						//	localDiceRollData.showedVantageMessage = true;
						if (!otherDie.isLucky)
							diceLayer.addAdvantageText(otherDie, vantageTextDelay, true);
						//}
						edgeRollValue = topNumber;
					}
					else {
						removeNonVantageDieNow(die);
						//if (!localDiceRollData.showedVantageMessage) {
						//	localDiceRollData.showedVantageMessage = true;
						if (!die.isLucky)
							diceLayer.addAdvantageText(die, vantageTextDelay, true);
						//}
					}
				}
				else if (isLuckRollLow) {
					if (edgeRollValue >= topNumber) {
						removeNonVantageDieNow(otherDie);
						//if (!localDiceRollData.showedVantageMessage) {
						//	localDiceRollData.showedVantageMessage = true;
						if (!otherDie.isLucky)
							diceLayer.addDisadvantageText(otherDie, vantageTextDelay, true);
						//}
						edgeRollValue = topNumber;
					}
					else {
						removeNonVantageDieNow(die);
						//if (!localDiceRollData.showedVantageMessage) {
						//	localDiceRollData.showedVantageMessage = true;
						if (!die.isLucky)
							diceLayer.addDisadvantageText(die, vantageTextDelay, true);
						//}
					}
				}
				otherDie = die;
			}
		}
	}
	return edgeRollValue;
}

function showSpecialLabels() {
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

	if (diceRollData.type === DiceRollType.ChaosBolt) {
		for (let i = 0; i < dice.length; i++) {
			const die: IDie = dice[i];
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
					if (additionalDieRollMessage)
						additionalDieRollMessage += ', ';
					else
						additionalDieRollMessage = '(';
					changedMessage = true;
					additionalDieRollMessage += message;
					diceLayer.addDieTextAfter(die, message, diceLayer.activePlayerDieColor, diceLayer.activePlayerDieFontColor, 900, 7000);
				}
			}
		}
		if (additionalDieRollMessage && changedMessage)
			additionalDieRollMessage += ')';
	}
}

const bubbleId = 'bubble';

function popFrozenDice() {
	for (let i = 0; i < dice.length; i++) {
		const die: IDie = dice[i];
		if (!die.inPlay)
			continue;
		for (let j = 0; j < die.attachedSprites.length; j++) {
			const sprite: SpriteProxy = die.attachedSprites[j];
			if (sprite.data === bubbleId) {
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

function modifyTotalRollForTestingPurposes() {
	//d20RollValue = 20; // critical hit.
	//totalRoll = 20;
	//totalRoll = 35; // age change
}

function getModifier(diceRollData: DiceRollData, player: Character): number {
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

class RollResults {
	constructor(public d20RollValue: number[], public singlePlayerId: number, public luckValue: number[],
		public playerIdForTextMessages: number, public skillSavingModifier: number, public totalDamage: number,
		public totalHealth: number, public totalExtra: number, public maxDamage: number) {
	}
}

class PlayerRoll {
	constructor(public roll: number, public name: string, public id: number, public data: string, public modifier: number = 0, public success: boolean = false) {
	}
}

function getRollResults(): RollResults {
	const totalScores: Array<number> = [0, 0, 0, 0, 0, 0, 0, 0, 0];
	const inspirationValue: Array<number> = [0, 0, 0, 0, 0, 0, 0, 0, 0];
	const luckValue: Array<number> = [0, 0, 0, 0, 0, 0, 0, 0, 0];
	let totalDamage = 0;
	let maxDamage = 0;
	let totalHealth = 0;
	let totalExtra = 0;
	diceRollData.totalRoll = 0;

	let singlePlayerId = 0;
	let playerIdForTextMessages = -1;

	//console.log('diceRollData.hasMultiPlayerDice: ' + diceRollData.hasMultiPlayerDice);
	console.log(`getRollResults: dice.length = ${dice.length}`);

	for (let i = 0; i < dice.length; i++) {
		const die: IDie = dice[i];
		die.scoreHasBeenTallied = false;  // Workaround for state bug where multiple instances of the same dia can apparently get inside the dice array.
	}

	for (let i = 0; i < dice.length; i++) {
		const die: IDie = dice[i];
		if (!die.inPlay)
			continue;

		if (die.scoreHasBeenTallied)
			continue;

		die.scoreHasBeenTallied = true;

		const topNumber: number = die.getTopNumber();

		if (diceRollData.hasMultiPlayerDice) {
			if (diceRollData.multiplayerSummary === null)
				diceRollData.multiplayerSummary = [];
			// diceRollData.multiplayerSummary.find((value, index, obj) => value.id === die.playerID);
			const playerRoll: PlayerRoll = diceRollData.multiplayerSummary.find((value: PlayerRoll, index, obj) => value.name === die.playerName);

			if (playerRoll) {
				console.log(`Found playerRoll for ${die.playerName}.`);
				playerRoll.roll += topNumber;
				playerRoll.success = playerRoll.roll + playerRoll.modifier >= diceRollData.hiddenThreshold;
			}
			else {
				console.log(`playerRoll not found for ${die.playerName}.`);

				let modifier = 0;
				if (die.playerID < 0) {  // It's an in-game creature.
					for (let i = 0; i < diceRollData.diceDtos.length; i++) {
						if (diceRollData.diceDtos[i].CreatureId === die.playerID) {
							modifier = diceRollData.diceDtos[i].Modifier;
							break;
						}
					}
				}
				else if (diceLayer.players && diceLayer.players.length > 0) {
					const player: Character = diceLayer.getPlayer(die.playerID);
					modifier = getModifier(diceRollData, player);
				}

				const success: boolean = topNumber + modifier >= diceRollData.hiddenThreshold;
				diceRollData.multiplayerSummary.push(new PlayerRoll(topNumber, die.playerName, die.playerID, die.dataStr, modifier, success));
				//console.log(diceRollData.multiplayerSummary[0].roll);
			}

			//console.log(diceRollData.multiplayerSummary);
			//console.log('');

			let scaleAdjust = 1;
			if (die.rollType === DieCountsAs.inspiration) {
				scaleAdjust = 0.7;
			}

			if (!die.playerName) {
				die.playerName = diceLayer.getPlayerName(die.playerID);
			}

			diceLayer.addDieTextAfter(die, die.playerName, diceLayer.getDieColor(die.playerID), diceLayer.activePlayerDieFontColor, 0, 8000, scaleAdjust);
		}

		diceRollData.individualRolls.push(new IndividualRoll(topNumber, die.values, die.dieType, die.damageType));

		const playerID: number = die.playerID;
		if (playerID === undefined || playerID < 0)
			continue;

		if (diceRollData.hasSingleIndividual) {
			singlePlayerId = playerID;
			playerIdForTextMessages = playerID;
		}

		if (!totalScores[playerID])
			totalScores[playerID] = 0;

		if (!inspirationValue[playerID])
			inspirationValue[playerID] = 0;

		if (!luckValue[playerID])
			luckValue[playerID] = 0;

		switch (die.rollType) {
			case DieCountsAs.totalScore:
				//console.log(`DieCountsAs.totalScore (${topNumber})`);
				totalScores[playerID] += topNumber;
				break;
			case DieCountsAs.inspiration:
				//console.log(`DieCountsAs.inspiration (${topNumber})`);
				inspirationValue[playerID] += topNumber;
				break;
			case DieCountsAs.bentLuck:
				//console.log(`DieCountsAs.bentLuck (${topNumber * diceRollData.bentLuckMultiplier})`);
				luckValue[playerID] += topNumber * diceRollData.bentLuckMultiplier;
				break;
			case DieCountsAs.bonus:
				//console.log(`DieCountsAs.bonus (${topNumber})`);
				totalBonus += topNumber;
				break;
			case DieCountsAs.damage:
				//console.log(`DieCountsAs.damage (${topNumber})`);
				totalDamage += topNumber;
				maxDamage += die.values;
				break;
			case DieCountsAs.health:
				//console.log(`DieCountsAs.health (${topNumber})`);
				totalHealth += topNumber;
				break;
			case DieCountsAs.extra:
				//console.log(`DieCountsAs.extra (${topNumber})`);
				totalExtra += topNumber;
				break;
		}
	} // for

	let skillSavingModifier = 0;
	if (diceRollData.type === DiceRollType.SkillCheck || diceRollData.type === DiceRollType.SavingThrow) {
		const player: Character = diceLayer.getPlayer(singlePlayerId);
		skillSavingModifier = getModifier(diceRollData, player);
	}

	diceRollData.totalRoll += totalScores[singlePlayerId] + inspirationValue[singlePlayerId] + luckValue[singlePlayerId] + diceRollData.modifier + skillSavingModifier;

	if (!diceRollData.hasMultiPlayerDice && totalScores[singlePlayerId] > 0) {
		if (diceRollData.type === DiceRollType.SkillCheck)
			diceRollData.totalRoll += totalBonus;
	}

	modifyTotalRollForTestingPurposes();

	attemptedRollWasSuccessful = diceRollData.totalRoll >= diceRollData.hiddenThreshold;
	console.log(`attemptedRollWasSuccessful: ${attemptedRollWasSuccessful} (totalRoll = ${diceRollData.totalRoll}, diceRollData.hiddenThreshold = ${diceRollData.hiddenThreshold})`);
	attemptedRollWasNarrowlySuccessful = attemptedRollWasSuccessful && (diceRollData.totalRoll - diceRollData.hiddenThreshold < 2);
	//console.log('damageModifierThisRoll: ' + damageModifierThisRoll);
	totalDamagePlusModifier = totalDamage + damageModifierThisRoll;
	totalHealthPlusModifier = totalHealth + healthModifierThisRoll;
	totalExtraPlusModifier = totalExtra + extraModifierThisRoll;

	return new RollResults(totalScores, singlePlayerId, luckValue, playerIdForTextMessages, skillSavingModifier, totalDamage, totalHealth, totalExtra, maxDamage);
}

function bonusRollDealsDamage(damageStr: string, description = '', playerID = -1): void {
	const bonusRoll: BonusRoll = diceRollData.addBonusRoll(damageStr, description, playerID, DiceLayer.damageDieBackgroundColor, DiceLayer.damageDieFontColor);
	bonusRoll.dieCountsAs = DieCountsAs.damage;
}

function getFirstPlayerId() {
	let playerID = -1;
	if (diceRollData.playerRollOptions.length > 0)
		playerID = diceRollData.playerRollOptions[0].PlayerID;
	return playerID;
}

function checkAttackBonusRolls() {
	if (isAttack(diceRollData)) {
		wasCriticalHit = d20RollValue >= diceRollData.minCrit;
		//console.log('isCriticalHit: ' + isCriticalHit);
		if (wasCriticalHit && !diceRollData.secondRollData) {
			//console.log('diceRollData.damageHealthExtraDice: ' + diceRollData.damageHealthExtraDice);
			bonusRollDealsDamage(diceRollData.damageHealthExtraDice, '', getFirstPlayerId());
			//console.log('checkAttackBonusRolls(1) - Roll Bonus Dice: ' + diceRollData.bonusRolls.length);
		}
		console.log('Calling getRollResults() from checkAttackBonusRolls...');
		getRollResults();
		if (attemptedRollWasSuccessful && diceRollData.minDamage > 0 && !diceRollData.secondRollData) {
			let extraRollStr = "";
			for (let i = 0; i < dice.length; i++) {
				const die: IDie = dice[i];
				if (die.rollType === DieCountsAs.damage && die.getTopNumber() < diceRollData.minDamage) {
					let damageStr = "";
					if (die.damageStr) {
						damageStr = `(${die.damageStr}:damage)`;
					}
					extraRollStr += `1d${die.values}${damageStr},`;
					removeDie(die, 0, DieEffect.HandGrab);
					const greatWeaponFightingTextDelay = 900;
					diceLayer.addDieTextAfter(die, 'Great Weapon Fighting', diceLayer.activePlayerDieColor, diceLayer.activePlayerDieColor, greatWeaponFightingTextDelay);
					die.inPlay = false;
					removeDieEffectsForSingleDie(die);
				}
			}
			if (extraRollStr.endsWith(',')) {
				extraRollStr = extraRollStr.substr(0, extraRollStr.length - 1);
			}
			if (extraRollStr) {
				//console.log('extraRollStr: ' + extraRollStr);
				bonusRollDealsDamage(extraRollStr, '', getFirstPlayerId());
				//console.log('checkAttackBonusRolls(2) - Roll Bonus Dice: ' + diceRollData.bonusRolls.length);
			}
		}
	}
}

function checkSkillCheckBonusRolls() {
	if (diceRollData.type === DiceRollType.SkillCheck) {
		for (let i = 0; i < dice.length; i++) {
			const die: IDie = dice[i];
			if (die.inPlay && die.rollType === DieCountsAs.totalScore && die.isD20 && die.getTopNumber() === 20) {
				let dieColor: string = diceLayer.activePlayerDieColor;
				let dieTextColor: string = diceLayer.activePlayerDieFontColor;
				if (die.playerID >= 0) {
					dieColor = diceLayer.getDieColor(die.playerID);
					dieTextColor = diceLayer.getDieFontColor(die.playerID);
				}
				diceRollData.addBonusRoll('1d20', '', die.playerID, dieColor, dieTextColor);
			}
		}
	}
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

function announceWildMagicResult(totalRoll: number) {
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

function checkWildMagicBonusRolls() {
	if (diceRollData.type === DiceRollType.WildMagic) {
		let rollValue = 0;
		for (let i = 0; i < dice.length; i++) {
			const die: IDie = dice[i];
			if (die.inPlay && (die.rollType === DieCountsAs.totalScore || die.rollType === DieCountsAs.inspiration))
				rollValue += die.getTopNumber();
		}
		diceRollData.totalRoll = rollValue + diceRollData.modifier;
		modifyTotalRollForTestingPurposes();
		diceRollData.playBonusSoundAfter = 2500;
		announceWildMagicResult(diceRollData.totalRoll);
		if (diceRollData.totalRoll === 0 || diceRollData.totalRoll === 99)
			diceRollData.wildMagic = WildMagic.regainSorceryPoints;
		else if (diceRollData.totalRoll < 3)
			diceRollData.wildMagic = WildMagic.wildMagicMinute;
		else if (diceRollData.totalRoll < 5)
			diceRollData.wildMagic = WildMagic.seeInvisibleCreatures;
		else if (diceRollData.totalRoll < 7)
			diceRollData.wildMagic = WildMagic.modronAppearsOneMinute;
		else if (diceRollData.totalRoll < 9)
			diceRollData.wildMagic = WildMagic.castFireball;
		else if (diceRollData.totalRoll < 11)
			diceRollData.wildMagic = WildMagic.castMagicMissile;
		else if (diceRollData.totalRoll < 13) {
			diceRollData.addBonusRoll('1d10', 'Inches Changed: ');
			diceRollData.wildMagic = WildMagic.heightChange;
			diceRollData.playBonusSoundAfter = 700;
		}
		else if (diceRollData.totalRoll < 15)
			diceRollData.wildMagic = WildMagic.castConfusionOnSelf;
		else if (diceRollData.totalRoll < 17)
			diceRollData.wildMagic = WildMagic.regain5hpPerTurnForOneMinute;
		else if (diceRollData.totalRoll < 19)
			diceRollData.wildMagic = WildMagic.beardOfFeathers;
		else if (diceRollData.totalRoll < 21)
			diceRollData.wildMagic = WildMagic.castGreaseCenteredOnSelf;
		else if (diceRollData.totalRoll < 23)
			diceRollData.wildMagic = WildMagic.spellTargetsDisadvantagedSavingThrowForOneMinute;
		else if (diceRollData.totalRoll < 25)
			diceRollData.wildMagic = WildMagic.skinTurnsBlue;
		else if (diceRollData.totalRoll < 27)
			diceRollData.wildMagic = WildMagic.thirdEyeAdvantageWisdomChecks;
		else if (diceRollData.totalRoll < 29)
			diceRollData.wildMagic = WildMagic.castTimeBonusActionOneMinute;
		else if (diceRollData.totalRoll < 31)
			diceRollData.wildMagic = WildMagic.teleportUpTo60Feet;
		else if (diceRollData.totalRoll < 33)
			diceRollData.wildMagic = WildMagic.astralPlaneUntilEndOfNextTurn;
		else if (diceRollData.totalRoll < 35)
			diceRollData.wildMagic = WildMagic.maximizeDamageOnSpellCastInNextMinute;
		else if (diceRollData.totalRoll < 37) {
			diceRollData.addBonusRoll('1d10', 'Years Changed: ');
			diceRollData.wildMagic = WildMagic.ageChange;
			diceRollData.playBonusSoundAfter = 700;
		}
		else if (diceRollData.totalRoll < 39) {
			diceRollData.addBonusRoll('1d6', 'Flumphs: ');
			diceRollData.wildMagic = WildMagic.flumphs;
		}
		else if (diceRollData.totalRoll < 41) {
			diceRollData.addBonusRoll('2d10', 'HP Regained: ');
			diceRollData.wildMagic = WildMagic.regainHitPoints;
		}
		else if (diceRollData.totalRoll < 43)
			diceRollData.wildMagic = WildMagic.pottedPlant;
		else if (diceRollData.totalRoll < 45)
			diceRollData.wildMagic = WildMagic.teleportUpTo20FeetBonusActionOneMinute;
		else if (diceRollData.totalRoll < 47)
			diceRollData.wildMagic = WildMagic.castLevitateOnSelf;
		else if (diceRollData.totalRoll < 49)
			diceRollData.wildMagic = WildMagic.unicorn;
		else if (diceRollData.totalRoll < 51)
			diceRollData.wildMagic = WildMagic.cannotSpeakPinkBubbles;
		else if (diceRollData.totalRoll < 53)
			diceRollData.wildMagic = WildMagic.spectralShieldPlus2ArmorClassNextMinute;
		else if (diceRollData.totalRoll < 55) {
			diceRollData.addBonusRoll('5d6', 'Days Immune: ');
			diceRollData.wildMagic = WildMagic.alcoholImmunity;
		}
		else if (diceRollData.totalRoll < 57)
			diceRollData.wildMagic = WildMagic.hairFallsOutGrowsBack24Hours;
		else if (diceRollData.totalRoll < 59)
			diceRollData.wildMagic = WildMagic.fireTouchOneMinute;
		else if (diceRollData.totalRoll < 61)
			diceRollData.wildMagic = WildMagic.regainLowestLevelExpendedSpellSlot;
		else if (diceRollData.totalRoll < 63)
			diceRollData.wildMagic = WildMagic.shoutWhenSpeakingOneMinute;
		else if (diceRollData.totalRoll < 65)
			diceRollData.wildMagic = WildMagic.castFogCloudCenteredOnSelf;
		else if (diceRollData.totalRoll < 67) {
			diceRollData.wildMagic = WildMagic.lightningDamageUpToThreeCreatures;
			diceRollData.addBonusDamageRoll('4d10(lightning)', 'Lightning Damage: ');
		}
		else if (diceRollData.totalRoll < 69)
			diceRollData.wildMagic = WildMagic.frightenedByNearestCreatureUntilEndOfNextTurn;
		else if (diceRollData.totalRoll < 71)
			diceRollData.wildMagic = WildMagic.allCreatures30FeetInvisibleOneMinute;
		else if (diceRollData.totalRoll < 73)
			diceRollData.wildMagic = WildMagic.resistanceToAllDamageNextMinute;
		else if (diceRollData.totalRoll < 75) {
			diceRollData.addBonusRoll('1d4', 'Hours Poisoned: ');
			diceRollData.wildMagic = WildMagic.randomCreaturePoisoned1d4Hours;
		}
		else if (diceRollData.totalRoll < 77)
			diceRollData.wildMagic = WildMagic.glowBrightOneMinuteCreaturesEndingTurn5FeetBlinded;
		else if (diceRollData.totalRoll < 79)
			diceRollData.wildMagic = WildMagic.castPolymorphToSheepOnSelf;
		else if (diceRollData.totalRoll < 81)
			diceRollData.wildMagic = WildMagic.butterfliesAndPetals10FeetOneMinute;
		else if (diceRollData.totalRoll < 83)
			diceRollData.wildMagic = WildMagic.takeOneAdditionalActionImmediately;
		else if (diceRollData.totalRoll < 85) {
			diceRollData.addBonusDamageRoll('1d10(necrotic)', 'Necrotic Damage: ');
			diceRollData.wildMagic = WildMagic.allCreaturesWithin30FeetTake1d10NecroticDamage;
		}
		else if (diceRollData.totalRoll < 87)
			diceRollData.wildMagic = WildMagic.castMirrorImage;
		else if (diceRollData.totalRoll < 89)
			diceRollData.wildMagic = WildMagic.castFlyOnRandomCreatureWithin60Feet;
		else if (diceRollData.totalRoll < 91)
			diceRollData.wildMagic = WildMagic.invisibleSilentNextMinute;
		else if (diceRollData.totalRoll < 93)
			diceRollData.wildMagic = WildMagic.immortalOneMinute;
		else if (diceRollData.totalRoll < 95)
			diceRollData.wildMagic = WildMagic.increaseSizeOneMinute;
		else if (diceRollData.totalRoll < 97)
			diceRollData.wildMagic = WildMagic.allCreatures30FeetVulnerableToPiercingDamageOneMinute;
		else if (diceRollData.totalRoll < 99)
			diceRollData.wildMagic = WildMagic.faintEtheralMusicOneMinute;
	}
}

function needToRollBonusDice() {
	if (onBonusThrow || setupBonusRoll)
		return false;

	modifyTotalRollForTestingPurposes();
	checkAttackBonusRolls();
	checkSkillCheckBonusRolls();
	checkWildMagicBonusRolls();

	return diceRollData.bonusRolls && diceRollData.bonusRolls.length > 0;
}

function freezeDie(die: IDie) {
	const dieObject: IDieObject = die.getObject();
	if (!dieObject)
		return;
	const body = dieObject.body;
	body.mass = 0;
	body.updateMassProperties();
	body.velocity.set(0, 0, 0);
	body.angularVelocity.set(0, 0, 0);
}

function freezeExistingDice() {
	for (let i = 0; i < dice.length; i++) {
		const die: IDie = dice[i];
		freezeDie(die);
		if (die.attachedSprites && die.inPlay) {
			let hueShift = 0;
			if (die.playerID >= 0)
				hueShift = diceLayer.getHueShift(die.playerID);
			else
				hueShift = diceLayer.activePlayerHueShift;

			const bubble: SpriteProxy = diceLayer.addFreezeBubble(960, 540, hueShift, 100, 100);

			bubble.data = bubbleId;
			die.attachedSprites.push(bubble);
			diceSounds.safePlayMp3('ice/Freeze[5]');
			die.origins.push(new Vector(diceLayer.freeze.originX, diceLayer.freeze.originY));
		}
	}
}

function damageTypeFromStr(str: string): DamageType {
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

function prepareBaseDie(die: IDie, throwPower: number, xPositionModifier = 0) {
	const dieObject: IDieObject = die.getObject();
	scene.add(dieObject);
	die.inPlay = true;
	die.attachedSprites = [];
	die.attachedLabels = [];
	die.origins = [];
	dice.push(die);

	die.hasNotHitFloorYet = true;

	const index: number = dice.length;
	const yVelocityModifier: number = Math.random() * 10 * throwPower;
	dieObject.position.x = xPositionModifier + -15 - (index % 3) * dieScale;
	dieObject.position.y = 4 + Math.floor(index / 3) * dieScale;
	dieObject.position.z = -13 + (index % 3) * dieScale;
	dieObject.quaternion.x = (Math.random() * 90 - 45) * Math.PI / 180;
	dieObject.quaternion.z = (Math.random() * 90 - 45) * Math.PI / 180;
	let xVelocityMultiplier = 1;
	if (xPositionModifier !== 0)
		xVelocityMultiplier = -1;
	die.updateBodyFromMesh();
	const xVelocityModifier: number = Math.random() * 20 * throwPower;
	const zVelocityModifier: number = Math.random() * 20 * throwPower;
	dieObject.body.velocity.set(xVelocityMultiplier * (35 + xVelocityModifier), 10 + yVelocityModifier, 25 + zVelocityModifier);
	const angularModifierLimit: number = 20 * throwPower;
	const angularModifierHalfLimit: number = angularModifierLimit / 2;
	const xAngularRotationModifier: number = Math.random() * angularModifierLimit;
	const yAngularRotationModifier: number = Math.random() * angularModifierLimit;
	const zAngularRotationModifier: number = Math.random() * angularModifierLimit;
	dieObject.body.angularVelocity.set(xAngularRotationModifier - angularModifierHalfLimit, yAngularRotationModifier - angularModifierHalfLimit, zAngularRotationModifier - angularModifierHalfLimit);
	dieObject.body.name = 'die';
	dieObject.body.addEventListener("collide", handleDieCollision);
	die.lastPos = [];
	die.lastPos.push(new Vector(-100, -100));
	die.lastPrintOnLeft = false;
}

function prepareDie(die: IDie, throwPower: number, xPositionModifier = 0) {
	prepareBaseDie(die, throwPower, xPositionModifier);

	const newValue: number = Math.floor(Math.random() * die.values + 1);
	diceValues.push({ dice: die, value: newValue });
}

function prepareD10x10Die(die: IDie, throwPower: number, xPositionModifier = 0) {
	prepareBaseDie(die, throwPower, xPositionModifier);
	const newValue: number = Math.floor(Math.random() * die.values) + 1;
	diceValues.push({ dice: die, value: newValue });
}

function prepareD10x01Die(die: IDie, throwPower: number, xPositionModifier = 0) {
	prepareBaseDie(die, throwPower, xPositionModifier);
	const newValue: number = Math.floor(Math.random() * die.values) + 1;
	diceValues.push({ dice: die, value: newValue });
}

function attachLabel(die: IDie, textColor: string, backgroundColor: string) {
	if (die.dieType && die.dieType.startsWith('"')) {
		diceLayer.attachLabel(die, die.dieType, textColor, backgroundColor);
	}
}

function addInspirationParticles(die: IDie, playerID: number, hueShiftOffset: number, rotationDegeesPerSecond: number) {
	die.attachedSprites.push(diceLayer.addInspirationParticles(960, 540, rotationDegeesPerSecond, diceLayer.getHueShift(playerID) + hueShiftOffset));
	die.origins.push(diceLayer.inspirationParticles.getOrigin());
}

function createDie(quantity: number, numSides: number, damageType: DamageType, rollType: DieCountsAs, backgroundColor: string, textColor: string, throwPower = 1, xPositionModifier = 0, isMagic = false, playerID = -1, dieType = ''): IDie {
	let lastDieAdded = null;
	const magicRingHueShift: number = Math.floor(Math.random() * 360);
	for (let i = 0; i < quantity; i++) {
		let die: IDie = null;
		switch (numSides) {
			case 4:
				// @ts-ignore - DiceD4
				die = new DiceD4({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
				break;
			case 6:
				// @ts-ignore - DiceD6
				die = new DiceD6({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
				break;
			case 8:
				// @ts-ignore - DiceD8
				die = new DiceD8({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
				break;
			case 10:
				// @ts-ignore - DiceD10
				die = new DiceD10({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
				break;
			case 1001:
				// @ts-ignore - DiceD10x01
				die = new DiceD10x01({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
				break;
			case 1010:
				// @ts-ignore - DiceD10x01
				die = new DiceD10x10({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
				break;
			case 12:
				// @ts-ignore - DiceD12
				die = new DiceD12({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
				break;
			case 20:
				// @ts-ignore - DiceD20
				die = new DiceD20({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
				die.isD20 = true;
				break;
		}
		lastDieAdded = die;
		if (die === null) {
			throw new Error(`Die with ${numSides} sides was not found. Unable to throw dice.`);
		}
		die.dieType = dieType;
		die.playerID = playerID;
		prepareDie(die, throwPower, xPositionModifier);
		attachLabel(die, textColor, backgroundColor);
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
			addInspirationParticles(die, playerID, hueShift, rotation);
			addInspirationParticles(die, playerID, -hueShift, -rotation);
			die.attachedSprites.push(diceLayer.addInspirationSmoke(960, 540, Math.floor(Math.random() * 360)));
			die.origins.push(diceLayer.inspirationSmoke.getOrigin());
		}
		if (isMagic) {
			die.attachedSprites.push(diceLayer.addMagicRing(960, 540, magicRingHueShift + Random.plusMinusBetween(10, 25)));
			die.origins.push(diceLayer.magicRingRed.getOrigin());
		}
	}
	return lastDieAdded;
}

function addDie(dieStr: string, damageType: DamageType, rollType: DieCountsAs, backgroundColor: string, textColor: string, throwPower = 1, xPositionModifier = 0, isMagic = false, playerID = -1, dieType = ''): IDie {
	const countPlusDie: string[] = dieStr.split('d');
	if (countPlusDie.length !== 2)
		throw new Error(`Issue with die format string: "${dieStr}". Unable to throw dice.`);
	let quantity = 1;
	if (countPlusDie[0])
		quantity = +countPlusDie[0];
	const numSides: number = +countPlusDie[1];
	return createDie(quantity, numSides, damageType, rollType, backgroundColor, textColor, throwPower, xPositionModifier, isMagic, playerID, dieType);
}

function addDieFromStr(playerID: number, diceStr: string, dieCountsAs: DieCountsAs, throwPower: number, xPositionModifier = 0, backgroundColor: string = undefined, fontColor: string = undefined, isMagic: boolean = false): any {
	if (!diceStr)
		return;
	const allDice: string[] = diceStr.split(',');
	if (backgroundColor === undefined)
		backgroundColor = DiceLayer.damageDieBackgroundColor;
	if (fontColor === undefined)
		fontColor = DiceLayer.damageDieFontColor;
	let modifier = 0;
	let damageType: DamageType = diceRollData.damageType;
	allDice.forEach(function (dieSpec: string) {
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
					damageType = damageTypeFromStr(damageStr);
				}
				else {
					damageType = damageTypeFromStr(damageStr);
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
		addDie(dieStr, damageType, thisDieCountsAs, thisBackgroundColor, thisFontColor, throwPower, xPositionModifier, isMagic, playerID, dieType);
	});

	damageModifierThisRoll += modifier;
	//console.log(`damageModifierThisRoll += modifier; (${damageModifierThisRoll})`);
	healthModifierThisRoll += modifier;
	extraModifierThisRoll += modifier;
}

function rollBonusDice() {
	onBonusThrow = true;

	if (!diceRollData.bonusRolls)
		return;
	//console.log('rollBonusDice() - Roll Bonus Dice: ' + diceRollData.bonusRolls.length);
	for (let i = 0; i < diceRollData.bonusRolls.length; i++) {
		const bonusRoll: BonusRoll = diceRollData.bonusRolls[i];
		//console.log('bonusRoll.dieCountsAs: ' + bonusRoll.dieCountsAs);
		addDieFromStr(bonusRoll.playerID, bonusRoll.diceStr, bonusRoll.dieCountsAs, Random.between(1.2, 2.2), 0, bonusRoll.dieBackColor, bonusRoll.dieTextColor, bonusRoll.isMagic);
	}
	bonusRollStartTime = performance.now();
	waitingForBonusRollToComplete = true;
}

function playAnnouncerCommentary(type: DiceRollType, d20RollValue: number, totalDamage = 0, maxDamage = 0): void {
	if (diceRollData.hasMultiPlayerDice) {
		diceSounds.playMultiplayerCommentary(type, d20RollValue);
		return;
	}
	if (diceRollData.type === DiceRollType.WildMagic) {
		diceSounds.playWildMagicCommentary(type, d20RollValue);
		return;
	}

	if (diceRollData.type === DiceRollType.PercentageRoll) {
		diceSounds.playPercentageRollCommentary(type, d20RollValue);
		return;
	}

	if (diceRollData.type === DiceRollType.DamageOnly) {
		diceSounds.playDamageCommentary(type, d20RollValue, totalDamage, maxDamage);
		return;
	}

	if (diceRollData.type === DiceRollType.HealthOnly) {
		diceSounds.playHealthCommentary(type, d20RollValue);
		return;
	}

	if (diceRollData.type === DiceRollType.ExtraOnly) {
		diceSounds.playExtraCommentary(type, d20RollValue);
		return;
	}

	if (diceRollData.type === DiceRollType.Attack) {
		diceSounds.playAttackCommentary(d20RollValue, totalDamage, maxDamage);
		return;
	}

	if (diceRollData.type === DiceRollType.SkillCheck) {
		diceSounds.playSkillCheckCommentary(d20RollValue, diceRollData.skillCheck);
		return;
	}

	if (diceRollData.type === DiceRollType.SavingThrow) {
		diceSounds.playSavingThrowCommentary(d20RollValue, diceRollData.savingThrow);
		return;
	}

	if (diceRollData.type === DiceRollType.ChaosBolt) {
		diceSounds.playChaosBoltCommentary(d20RollValue, diceRollData.savingThrow);
		return;
	}

	if (diceRollData.type === DiceRollType.FlatD20) {
		diceSounds.playFlatD20Commentary(d20RollValue);
		return;
	}

	if (diceRollData.type === DiceRollType.WildMagicD20Check) {
		diceSounds.playWildMagicD20CheckCommentary(d20RollValue);
		return;
	}
}

function getSkillCheckName() {
	const enumAsStr: string = Object.keys(Skills).find(key => Skills[key] === diceRollData.skillCheck);
	let initialCapEnum = '';
	if (enumAsStr)
		initialCapEnum = enumAsStr.charAt(0).toUpperCase() + enumAsStr.slice(1);
	else
		initialCapEnum = 'Skill';
	if (diceRollData.skillCheck === Skills.animalHandling)
		initialCapEnum = 'Animal Handling';
	else if (diceRollData.skillCheck === Skills.sleightOfHand)
		initialCapEnum = 'Sleight of Hand';
	return initialCapEnum;
}

function getSavingThrowName() {
	const enumAsStr: string = Object.keys(Ability).find(key => Ability[key] === diceRollData.savingThrow);
	let initialCapEnum = '';
	if (enumAsStr)
		initialCapEnum = enumAsStr.charAt(0).toUpperCase() + enumAsStr.slice(1);
	else
		initialCapEnum = 'Ability';
	return initialCapEnum;
}

function isOdd(num) {
	return num % 2;
}

function showSuccessFailMessages(title: string, rawD20RollValue: number) {
	if (title)
		title += ' ';
	if (!diceRollData.hasMultiPlayerDice && diceRollData.type !== DiceRollType.WildMagic &&
		diceRollData.type !== DiceRollType.PercentageRoll &&
		diceRollData.type !== DiceRollType.DamageOnly &&
		diceRollData.type !== DiceRollType.HealthOnly &&
		diceRollData.type !== DiceRollType.ExtraOnly) {
		if (attemptedRollWasSuccessful)
			if (rawD20RollValue >= diceRollData.minCrit) {
				diceLayer.showResult(title + diceRollData.critSuccessMessage, attemptedRollWasSuccessful);
			}
			else {
				diceLayer.showResult(title + diceRollData.successMessage, attemptedRollWasSuccessful);
			}
		else if (rawD20RollValue === 1)
			diceLayer.showResult(title + diceRollData.critFailMessage, attemptedRollWasSuccessful);
		else
			diceLayer.showResult(title + diceRollData.failMessage, attemptedRollWasSuccessful);
	}
}

function playSecondaryAnnouncerCommentary(type: DiceRollType, d20RollValue: number, totalDamage: number = 0, maxDamage: number = 0): void {
	// TODO: Implement this.
}

function reportRollResults(rollResults: RollResults) {
	const d20RollValue: number[] = rollResults.d20RollValue;
	const singlePlayerId: number = rollResults.singlePlayerId;
	const luckValue: number[] = rollResults.luckValue;
	const playerIdForTextMessages: number = rollResults.playerIdForTextMessages;
	const skillSavingModifier: number = rollResults.skillSavingModifier;
	const totalDamage: number = rollResults.totalDamage;
	const totalHealth: number = rollResults.totalHealth;
	const totalExtra: number = rollResults.totalExtra;
	let maxDamage: number = rollResults.maxDamage;

	let title = '';
	if (diceRollData.type === DiceRollType.Initiative)
		title = 'Initiative:';
	else if (diceRollData.type === DiceRollType.NonCombatInitiative)
		title = 'Non-combat Initiative:';
	else if (diceRollData.type === DiceRollType.SkillCheck)
		title = `${getSkillCheckName()} Check:`;
	else if (diceRollData.type === DiceRollType.SavingThrow)
		title = `${getSavingThrowName()} Saving Throw:`;

	if (diceRollData.multiplayerSummary) {
		diceRollData.multiplayerSummary.sort((a, b) => (b.roll + b.modifier) - (a.roll + a.modifier));
		diceLayer.showMultiplayerResults(title, diceRollData.multiplayerSummary, diceRollData.hiddenThreshold);
	}

	if (!diceRollData.hasMultiPlayerDice && d20RollValue[singlePlayerId] > 0) {
		if (diceRollData.modifier !== 0)
			diceLayer.showRollModifier(diceRollData.modifier, luckValue[singlePlayerId], playerIdForTextMessages);
		if (skillSavingModifier !== 0)
			diceLayer.showRollModifier(skillSavingModifier, luckValue[singlePlayerId], playerIdForTextMessages);
		diceLayer.showDieTotal(`${diceRollData.totalRoll}`, playerIdForTextMessages);
	}

	if (totalBonus > 0 && !diceRollData.hasMultiPlayerDice && diceRollData.type !== DiceRollType.SkillCheck) {
		let bonusRollStr = 'Bonus Roll: ';
		const bonusRollOverrideStr: string = diceRollData.getFirstBonusRollDescription();
		if (bonusRollOverrideStr)
			bonusRollStr = bonusRollOverrideStr;
		switch (diceRollData.wildMagic) {
			case WildMagic.heightChange:
			case WildMagic.ageChange:
				if (isOdd(totalBonus))
					totalBonus = -totalBonus;
				break;
		}
		diceLayer.showBonusRoll(`${bonusRollStr}${totalBonus}`, DiceLayer.bonusRollFontColor, DiceLayer.bonusRollDieColor);
	}

	if (totalDamage > 0) {
		diceLayer.showTotalHealthDamage(totalDamagePlusModifier.toString(), attemptedRollWasSuccessful, 'Damage: ', DiceLayer.damageDieBackgroundColor, DiceLayer.damageDieFontColor);
		diceLayer.showDamageHealthModifier(damageModifierThisRoll, attemptedRollWasSuccessful, DiceLayer.damageDieBackgroundColor, DiceLayer.damageDieFontColor);
	}
	if (totalHealth > 0) {
		diceLayer.showTotalHealthDamage('+' + totalHealthPlusModifier.toString(), attemptedRollWasSuccessful, 'Health: ', DiceLayer.healthDieBackgroundColor, DiceLayer.healthDieFontColor);
		diceLayer.showDamageHealthModifier(healthModifierThisRoll, attemptedRollWasSuccessful, DiceLayer.healthDieBackgroundColor, DiceLayer.healthDieFontColor);
	}
	if (totalExtra > 0) {
		diceLayer.showTotalHealthDamage(totalExtraPlusModifier.toString(), attemptedRollWasSuccessful, '', DiceLayer.extraDieBackgroundColor, DiceLayer.extraDieFontColor);
		diceLayer.showDamageHealthModifier(extraModifierThisRoll, attemptedRollWasSuccessful, DiceLayer.extraDieBackgroundColor, DiceLayer.extraDieFontColor);
	}
	showSuccessFailMessages(title, d20RollValue[singlePlayerId]);
	maxDamage += damageModifierThisRoll;
	if (diceRollData.secondRollData)
		playSecondaryAnnouncerCommentary(diceRollData.secondRollData.type, d20RollValue[singlePlayerId], totalDamagePlusModifier, maxDamage);
	else
		playAnnouncerCommentary(diceRollData.type, d20RollValue[singlePlayerId], totalDamagePlusModifier, maxDamage);
	return maxDamage;
}

function playFinalRollSoundEffects() {
	if (!diceRollData)
		return;
	switch (diceRollData.wildMagic) {
		case WildMagic.wildMagicMinute: ; break;
		case WildMagic.seeInvisibleCreatures: ; break;
		case WildMagic.modronAppearsOneMinute: diceSounds.playWildMagic('modron'); break;
		case WildMagic.regain5hpPerTurnForOneMinute: ; break;
		case WildMagic.castMagicMissile: diceSounds.playWildMagic('magicMissile'); break;
		case WildMagic.castFireball: diceSounds.playWildMagic('fireball'); break;
		case WildMagic.heightChange:
		case WildMagic.ageChange:
			if (isOdd(totalBonus))
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

function calculateFinalMessage(): void {
	switch (diceRollData.wildMagic) {
		case WildMagic.wildMagicMinute:
			additionalDieRollMessage = 'Perform a wild magic roll at the start of each of your turns for the next minute, ignoring this result on subsequent rolls.'; break;
		case WildMagic.seeInvisibleCreatures:
			additionalDieRollMessage = 'For the next minute, you can see any invisible creature if you have line of sight to it.'; break;
		case WildMagic.modronAppearsOneMinute:
			additionalDieRollMessage = 'A modron chosen and controlled by the DM appears in an unoccupied space within 5 feet of you, then disappears 1 minute later.'; break;
		case WildMagic.castFireball: additionalDieRollMessage = 'You cast Fireball as a 3rd-level spell centered on yourself.'; break;
		case WildMagic.castMagicMissile: additionalDieRollMessage = 'You cast Magic Missile as a 5th-level spell.'; break;
		case WildMagic.heightChange:
			additionalDieRollMessage = 'Your height changes by a number of inches equal to the d10 roll. If the roll is odd, you shrink. If the roll is even, you grow.'; break;
		case WildMagic.castConfusionOnSelf:
			additionalDieRollMessage = 'You cast Confusion centered on yourself.'; break;
		case WildMagic.regain5hpPerTurnForOneMinute:
			additionalDieRollMessage = 'For the next minute, you regain 5 hit points at the start of each of your turns.'; break;
		case WildMagic.beardOfFeathers:
			additionalDieRollMessage = 'You grow a long beard made of feathers that remains until you sneeze, at which point the feathers explode out from your face.'; break;
		case WildMagic.castGreaseCenteredOnSelf:
			additionalDieRollMessage = 'You cast Grease centered on yourself.'; break;
		case WildMagic.spellTargetsDisadvantagedSavingThrowForOneMinute:
			additionalDieRollMessage = 'Creatures have disadvantage on saving throws against the next spell you cast in the next minute that involves a saving throw.'; break;
		case WildMagic.skinTurnsBlue:
			additionalDieRollMessage = 'Your skin turns a vibrant shade of blue. A Remove Curse spell can end this effect.'; break;
		case WildMagic.thirdEyeAdvantageWisdomChecks:
			additionalDieRollMessage = 'An eye appears on your forehead for the next minute. During that time, you have advantage on Wisdom (Perception) checks that rely on sight.'; break;
		case WildMagic.castTimeBonusActionOneMinute:
			additionalDieRollMessage = 'For the next minute, all your spells with a casting time of 1 action have a casting time of 1 bonus action.'; break;
		case WildMagic.teleportUpTo60Feet:
			additionalDieRollMessage = 'You teleport up to 60 feet to an unoccupied space of your choice that you can see.'; break;
		case WildMagic.astralPlaneUntilEndOfNextTurn:
			additionalDieRollMessage = 'You are transported to the Astral Plane until the end of your next turn, after which time you return to the space you previously occupied or the nearest unoccupied space if that space is occupied.'; break;
		case WildMagic.maximizeDamageOnSpellCastInNextMinute:
			additionalDieRollMessage = 'Maximize the damage of the next damaging spell you cast within the next minute.'; break;
		case WildMagic.ageChange:
			additionalDieRollMessage = 'Your age changes by a number of years equal to the d10 roll. If the roll is odd, you get younger (minimum 1 year old). If the roll is even, you get older.'; break;
		case WildMagic.flumphs:
			additionalDieRollMessage = '1d6 flumphs controlled by the DM appear in unoccupied spaces within 60 feet of you and are frightened of you. They vanish after 1 minute.'; break;
		case WildMagic.regainHitPoints:
			additionalDieRollMessage = 'You regain 2d10 hit points.'; break;
		case WildMagic.pottedPlant:
			additionalDieRollMessage = 'You turn into a potted plant until the start of your next turn. While a plant, you are incapacitated and have vulnerability to all damage. If you drop to 0 hit points, your pot breaks, and your form reverts.'; break;
		case WildMagic.teleportUpTo20FeetBonusActionOneMinute:
			additionalDieRollMessage = 'For the next minute, you can teleport up to 20 feet as a bonus action on each of your turns.'; break;
		case WildMagic.castLevitateOnSelf:
			additionalDieRollMessage = 'You cast Levitate on yourself.'; break;
		case WildMagic.unicorn:
			additionalDieRollMessage = 'A unicorn controlled by the DM appears in a space within 5 feet of you, then disappears 1 minute later.'; break;
		case WildMagic.cannotSpeakPinkBubbles:
			additionalDieRollMessage = "You can't speak for the next minute.Whenever you try, pink bubbles float out of your mouth."; break;
		case WildMagic.spectralShieldPlus2ArmorClassNextMinute:
			additionalDieRollMessage = 'A spectral shield hovers near you for the next minute, granting you a +2 bonus to AC and immunity to Magic Missile.'; break;
		case WildMagic.alcoholImmunity:
			additionalDieRollMessage = 'You are immune to being intoxicated by alcohol for the next 5d6 days.'; break;
		case WildMagic.hairFallsOutGrowsBack24Hours:
			additionalDieRollMessage = 'Your hair falls out but grows back within 24 hours.'; break;
		case WildMagic.fireTouchOneMinute:
			additionalDieRollMessage = "For the next minute, any flammable object you touch that isn't being worn or carried by another creature bursts into flame."; break;
		case WildMagic.regainLowestLevelExpendedSpellSlot:
			additionalDieRollMessage = 'You regain your lowest-level expended spell slot.'; break;
		case WildMagic.shoutWhenSpeakingOneMinute:
			additionalDieRollMessage = 'For the next minute, you must shout when you speak.'; break;
		case WildMagic.castFogCloudCenteredOnSelf:
			additionalDieRollMessage = 'You cast Fog Cloud centered on yourself.'; break;
		case WildMagic.lightningDamageUpToThreeCreatures:
			additionalDieRollMessage = 'Up to three creatures you choose within 30 feet of you take 4d10 lightning damage.'; break;
		case WildMagic.frightenedByNearestCreatureUntilEndOfNextTurn:
			additionalDieRollMessage = 'You are frightened by the nearest creature until the end of your next turn.'; break;
		case WildMagic.allCreatures30FeetInvisibleOneMinute:
			additionalDieRollMessage = 'Each creature within 30 feet of you becomes invisible for the next minute. The invisibility ends on a creature when it attacks or casts a spell.'; break;
		case WildMagic.resistanceToAllDamageNextMinute:
			additionalDieRollMessage = 'You gain resistance to all damage for the next minute.'; break;
		case WildMagic.randomCreaturePoisoned1d4Hours:
			additionalDieRollMessage = 'A random creature within 60 feet of you becomes poisoned for 1d4 hours.'; break;
		case WildMagic.glowBrightOneMinuteCreaturesEndingTurn5FeetBlinded:
			additionalDieRollMessage = 'You glow with bright light in a 30-foot radius for the next minute. Any creature that ends its turn within 5 feet of you is blinded until the end of its next turn.'; break;
		case WildMagic.castPolymorphToSheepOnSelf:
			additionalDieRollMessage = "You cast Polymorph on yourself. If you fail the saving throw, you turn into a sheep for the spell's duration."; break;
		case WildMagic.butterfliesAndPetals10FeetOneMinute:
			additionalDieRollMessage = 'Illusory butterflies and flower petals flutter in the air within 10 feet of you for the next minute.'; break;
		case WildMagic.takeOneAdditionalActionImmediately:
			additionalDieRollMessage = 'You can take one additional action immediately.'; break;
		case WildMagic.allCreaturesWithin30FeetTake1d10NecroticDamage:
			additionalDieRollMessage = 'Each creature within 30 feet of you takes 1d10 necrotic damage. You regain hit points equal to the sum of the necrotic damage dealt.'; break;
		case WildMagic.castMirrorImage:
			additionalDieRollMessage = 'You cast Mirror Image.'; break;
		case WildMagic.castFlyOnRandomCreatureWithin60Feet:
			additionalDieRollMessage = 'You cast Fly on a random creature within 60 feet of you.'; break;
		case WildMagic.invisibleSilentNextMinute:
			additionalDieRollMessage = "You become invisible for the next minute. During that time, other creatures can't hear you. The invisibility ends if you attack or cast a spell."; break;
		case WildMagic.immortalOneMinute:
			additionalDieRollMessage = 'If you die within the next minute, you immediately come back to life as if by the Reincarnate spell.'; break;
		case WildMagic.increaseSizeOneMinute:
			additionalDieRollMessage = 'Your size increases by one size category for the next minute.'; break;
		case WildMagic.allCreatures30FeetVulnerableToPiercingDamageOneMinute:
			additionalDieRollMessage = 'You and all creatures within 30 feet of you gain vulnerability to piercing damage for the next minute.'; break;
		case WildMagic.faintEtheralMusicOneMinute:
			additionalDieRollMessage = 'You are surrounded by faint, ethereal music for the next minute.'; break;
		case WildMagic.regainSorceryPoints:
			additionalDieRollMessage = 'You regain all expended sorcery points.'; break;
	}
	//diceRollData
	// 
}

function onSuccess() {

}

function onFailure() {

}


function removeDieEffects() {
	for (let i = 0; i < dice.length; i++) {
		const die: IDie = dice[i];
		removeDieEffectsForSingleDie(die);
	}
}

let animationsShouldBeDone: boolean;
const hiddenDieScale = 0.01;

function removeRemainingDice(): boolean {
	// TODO: Make sure we can call this robustly at any time.
	let dieEffectInterval = 0;
	animationsShouldBeDone = false;
	removeDieEffects();
	let diceInPlayWereRemoved = false;
	const effectOverride = getRandomEffect();
	for (let i = 0; i < dice.length; i++) {
		if (dice[i].inPlay) {
			diceInPlayWereRemoved = true;
			dieEffectInterval = removeDie(dice[i], dieEffectInterval, effectOverride);
		}
	}
	return diceInPlayWereRemoved;
}

let lastRollDiceData;

function onDiceRollStopped() {
	console.log('onDiceRollStopped...');
	calculateFinalMessage();
	onBonusThrow = false;
	setupBonusRoll = false;
	needToClearD20s = true;
	allDiceHaveStoppedRolling = true;
	//console.log('Dice have stopped rolling!');
	//diceHaveStoppedRolling(null);

	if (diceRollData.onStopRollingSound)
		diceSounds.safePlayMp3(diceRollData.onStopRollingSound);

	if (attemptedRollWasSuccessful) {
		onSuccess();
	}
	else {
		onFailure();
	}

	let playerId: number = diceLayer.playerID;
	if (diceRollData.playerRollOptions.length === 1)
		playerId = diceRollData.playerRollOptions[0].PlayerID;

	// Connects to DiceStoppedRollingData in DiceStoppedRollingData.cs:
	console.log('diceRollData.type: ' + diceRollData.type);
	console.log(diceRollData.multiplayerSummary);
	console.log(diceRollData.individualRolls);
	lastRollDiceData = {
		'wasCriticalHit': wasCriticalHit,
		'playerID': playerId,
		'success': attemptedRollWasSuccessful,
		'roll': diceRollData.totalRoll,
		'hiddenThreshold': diceRollData.hiddenThreshold,
		'spellName': diceRollData.spellName,
		'damage': totalDamagePlusModifier,
		'health': totalHealthPlusModifier,
		'extra': totalExtraPlusModifier,
		'multiplayerSummary': diceRollData.multiplayerSummary,
		'individualRolls': diceRollData.individualRolls,
		'type': diceRollData.type,
		'skillCheck': diceRollData.skillCheck,
		'savingThrow': diceRollData.savingThrow,
		'bonus': totalBonus,
		'additionalDieRollMessage': additionalDieRollMessage,
	};

	diceHaveStoppedRolling(JSON.stringify(lastRollDiceData));

	if (removeDiceImmediately)
		removeRemainingDice();
}

function diceDefinitelyStoppedRolling() {
	console.log('diceDefinitelyStoppedRolling');
	if (needToClearD20s) {
		if (diceRollData.hasMultiPlayerDice) {
			removeMultiplayerD20s();
		}
		else {
			d20RollValue = removeD20s();
		}
		showSpecialLabels();
	}
	if (needToRollBonusDice()) {
		if (!setupBonusRoll) {
			setupBonusRoll = true;
			//if (diceRollData.type == DiceRollType.WildMagic)
			//	showRollTotal();
			if (!diceRollData.startedBonusDiceRoll) {
				freezeExistingDice();
				diceRollData.startedBonusDiceRoll = true;
				if (isAttack(diceRollData) && d20RollValue >= diceRollData.minCrit) {
					diceLayer.indicateBonusRoll('Damage Bonus!');
					wasCriticalHit = true;
				}
				else
					diceLayer.indicateBonusRoll('Bonus Roll!');
				setTimeout(rollBonusDice, 2500);
			}
		}
	}
	else {
		showSpecialLabels();
		popFrozenDice();
		console.log('Calling getRollResults() from diceDefinitelyStoppedRolling...');
		reportRollResults(getRollResults());
		if (diceRollData.playBonusSoundAfter)
			setTimeout(playFinalRollSoundEffects, diceRollData.playBonusSoundAfter);
		onDiceRollStopped();
	}
}

function diceJustStoppedRolling(now: number) {
	const thisTime: number = now;
	if ((thisTime - firstStopTime) / 1000 > 1.5) {
		diceDefinitelyStoppedRolling();
	}
}

function anyDiceStillRolling(): boolean {
	if (dice === null)
		return false;
	for (let i = 0; i < dice.length; i++) {
		if (!dice[i].isFinished())
			return true;
	}
	return false;
}

function checkStillRolling() {
	if (allDiceHaveStoppedRolling)
		return;

	if (anyDiceStillRolling()) {
		waitingForSettle = true;
		return;
	}

	if (setupBonusRoll && !waitingForBonusRollToComplete) {
		waitingForSettle = true;
		return;
	}

	const now: number = performance.now();

	if (waitingForBonusRollToComplete && bonusRollStartTime !== 0) {
		if (now - bonusRollStartTime < 1000) {
			return;
		}
		waitingForBonusRollToComplete = false;
		setupBonusRoll = false;
	}

	if (waitingForSettle) {
		waitingForSettle = false;
		firstStopTime = now;
	}
	else {
		diceJustStoppedRolling(now);
	}
}

function clearAllDiceIfHidden() {
	for (let i = 0; i < dice.length; i++) {
		const die: IDie = dice[i];
		const dieObject: IDieObject = die.getObject();
		if (dieObject === null)
			continue;
		if (!dieObject.isHidden)
			return;
	}
	clearAllDice();
}

function hideDie(dieObject) {
	dieObject.scale.set(hiddenDieScale, hiddenDieScale, hiddenDieScale);
	dieObject.isHidden = true;
	clearAllDiceIfHidden();
}

function hideDieIn(dieObject, ms: number) {
	dieObject.hideTime = performance.now() + ms;
	dieObject.needToHideDie = true;
}

function removeSingleDieFromArray(dieToRemove: IDie, listToChange: IDie[]) {
	for (let i = 0; i < listToChange.length; i++) {
		if (dieToRemove === listToChange[i]) {
			listToChange.splice(i, 1);
			return;
		}
	}
}

function removeDiceFromArray(dieToRemove: IDie[], listToChange: IDie[]) {
	for (let i = 0; i < dieToRemove.length; i++) {
		removeSingleDieFromArray(dieToRemove[i], listToChange);
	}
}

function clearTheseDice(diceList: IDie[]) {
	if (!diceList || diceList.length === 0)
		return;
	for (let i = 0; i < diceList.length; i++) {
		const die: IDie = diceList[i];
		if (!die)
			continue;
		const dieObject: IDieObject = die.getObject();
		if (!dieObject)
			continue;
		scene.remove(dieObject);
		die.clear();
	}
}

function highlightSpecialDice() {
	if (!specialDice || specialDice.length === 0)
		return;

	const now: number = performance.now();

	const hiddenDie: IDie[] = [];

	const magicRingHueShift: number = Math.floor(Math.random() * 360);

	for (let i = 0; i < specialDice.length; i++) {
		const dieObject: IDieObject = specialDice[i].getObject();
		if (dieObject === null)
			continue;

		if (dieObject.needToHideDie) {
			if (dieObject.hideTime < now) {
				dieObject.needToHideDie = false;
				hideDie(dieObject);
				hiddenDie.push(specialDice[i]);
			}
		}

		if (dieObject.needToStartEffect) {
			const effectStartTime: number = dieObject.removeTime + dieObject.effectStartOffset;
			if (now > effectStartTime && dieObject.needToStartEffect) {

				dieObject.needToStartEffect = false;

				//dieObject.effectKind = DieEffect.Burst;  // MKM - delete this.

				// die.dieValue is also available.
				const screenPos: Vector = getScreenCoordinates(dieObject);
				if (!screenPos)
					continue;

				if (dieObject.effectKind === DieEffect.Lucky) {
					diceLayer.addLuckyRing(screenPos.x, screenPos.y);
				}
				if (dieObject.effectKind === DieEffect.Ring) {
					diceLayer.addMagicRing(screenPos.x, screenPos.y, magicRingHueShift + Random.plusMinusBetween(10, 25));
				}
				else if (dieObject.effectKind === DieEffect.Fireball) {
					//diceLayer.addD20Fire(screenPos.x, screenPos.y);
					diceLayer.addFireball(screenPos.x, screenPos.y);
					diceSounds.playFireball();
				}
				else if (dieObject.effectKind === DieEffect.Bomb) {
					const hueShift: number = Math.floor(Math.random() * 360);
					let saturation = 75;  // Reduce saturation for significant hue shifts 
					if (hueShift < 15 || hueShift > 345)
						saturation = 100;
					diceLayer.addDiceBomb(screenPos.x, screenPos.y, hueShift, saturation, 100);
					diceSounds.playDieBomb();
					hideDieIn(dieObject, 700);
				}
				else if (dieObject.effectKind === DieEffect.GroundBurst) {
					diceLayer.addGroundBurst(specialDice[i], screenPos, dieObject);
				}
				else if (dieObject.effectKind === DieEffect.SmokeyPortal) {
					diceLayer.addSmokeyPortal(specialDice[i], screenPos, dieObject);
				}
				else if (dieObject.effectKind === DieEffect.SteamPunkTunnel) {
					diceLayer.addSteampunkTunnel(screenPos.x, screenPos.y, Math.floor(Math.random() * 360), 100, 100);
					//diceLayer.playSteampunkTunnel();
					//hideDieIn(die, 700);
				}
				else if (dieObject.effectKind === DieEffect.HandGrab) {
					diceLayer.testDiceGrab(screenPos.x, screenPos.y, Math.floor(Math.random() * 360), 100, 100);
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

					diceLayer.blowColoredSmoke(screenPos.x, screenPos.y, Math.floor(Math.random() * 360), saturation, brightnessBase + Math.random() * 80);
					hideDie(dieObject);
					hiddenDie.push(specialDice[i]);
					diceSounds.playDiceBlow();
				}
			}
		}
	}
	removeDiceFromArray(hiddenDie, specialDice);
	removeDiceFromArray(hiddenDie, dice);
	clearTheseDice(hiddenDie);
}

function diceRemainingInPlay(): number {
	let count = 0;
	for (const i in dice) {
		if (dice[i].inPlay) {
			count++;
		}
	}
	return count;
}

const diceRollerTimeBetweenFramesQueue = [];
const diceRollerDrawTimeForEachFrameQueue = [];
let diceRollerLastFrameUpdate: number;
let diceRollerShowFpsWindow: boolean;
let diceRollerFpsWindow: FpsWindow;

function calculateFramerate(startUpdate: number, endUpdate: number): any {
	if (diceRollerLastFrameUpdate) {
		const timeBetweenFrames: number = endUpdate - diceRollerLastFrameUpdate;
		diceRollerTimeBetweenFramesQueue.push(timeBetweenFrames);
		if (diceRollerTimeBetweenFramesQueue.length > Game.fpsHistoryCount)
			diceRollerTimeBetweenFramesQueue.shift();
	}

	const drawTimeForThisFrame: number = endUpdate - startUpdate;
	diceRollerDrawTimeForEachFrameQueue.push(drawTimeForThisFrame);
	if (diceRollerDrawTimeForEachFrameQueue.length > Game.fpsHistoryCount)
		diceRollerDrawTimeForEachFrameQueue.shift();

	diceRollerLastFrameUpdate = endUpdate;
}

function init() { // From Rolling.html example.
	diceLayer = new DiceLayer();
	// SCENE
	// @ts-ignore - THREE
	scene = new THREE.Scene();

	// CAMERA
	//var SCREEN_WIDTH = window.innerWidth, SCREEN_HEIGHT = window.innerHeight;
	const SCREEN_WIDTH = 1920, SCREEN_HEIGHT = 1080;
	const lensFactor = 5;
	const VIEW_ANGLE = 45 / lensFactor, ASPECT = SCREEN_WIDTH / SCREEN_HEIGHT, NEAR = 0.01, FAR = 20000;

	// @ts-ignore - THREE
	camera = new THREE.PerspectiveCamera(VIEW_ANGLE, ASPECT, NEAR, FAR);
	scene.add(camera);
	camera.position.set(0, 30 * lensFactor, 0);


	// RENDERER
	// @ts-ignore - THREE
	renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
	renderer.setClearColor(0x000000, 0);

	renderer.setSize(SCREEN_WIDTH, SCREEN_HEIGHT);
	renderer.shadowMap.enabled = true;
	// @ts-ignore - THREE
	renderer.shadowMap.type = THREE.PCFSoftShadowMap;

	container = document.getElementById('ThreeJS');
	container.appendChild(renderer.domElement);
	// EVENTS
	// CONTROLS
	// @ts-ignore - THREE
	controls = new THREE.OrbitControls(camera, renderer.domElement);

	//// STATS
	//stats = new Stats();
	//stats.domElement.style.position = 'absolute';
	//stats.domElement.style.bottom = '0px';
	//stats.domElement.style.zIndex = 100;
	//container.appendChild(stats.domElement);

	// @ts-ignore - THREE
	const ambient = new THREE.AmbientLight('#ffffff', 0.35);
	scene.add(ambient);

	// @ts-ignore - THREE
	const directionalLight = new THREE.DirectionalLight('#ffffff', 0.25);
	directionalLight.position.x = -1000;
	directionalLight.position.y = 1000;
	directionalLight.position.z = 1000;
	scene.add(directionalLight);

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

	scene.add(light);


	// @ts-ignore - THREE
	const material = new THREE.ShadowMaterial();
	material.opacity = 0.5;
	// @ts-ignore - THREE
	const geometry = new THREE.PlaneGeometry(1000, 1000, 1, 1);
	// @ts-ignore - THREE
	const mesh = new THREE.Mesh(geometry, material);
	mesh.receiveShadow = true;
	mesh.rotation.x = -Math.PI / 2;
	scene.add(mesh);

	////////////
	// CUSTOM //
	////////////
	// @ts-ignore - CANNON
	world = new CANNON.World();

	setNormalGravity();
	// @ts-ignore - CANNON
	world.broadphase = new CANNON.NaiveBroadphase();
	world.solver.iterations = 32;

	// @ts-ignore - DiceManager
	DiceManager.setWorld(world);

	// create the sphere's material
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

	const wallHeight = 68;
	const leftWallHeight = 68;
	const topWallHeight = 146;

	const wallThickness = 1;
	const leftWallWidth = 80;
	const leftWallX = -21.5;

	const dmLeftWallWidth = 40;
	const dmLeftWallX = 13;
	const dmLeftWallZ = -26;

	const topWallWidth = 80;
	const topWallZ = -10.5;

	const playerTopWallWidth = 38;
	const playerTopWallZ = 5;
	const playerTopWallX = -2;

	const playerRightWallWidth = 9;
	const playerRightWallX = 17;
	const playerRightWallZ = 9;

	const dmBottomWallWidth = 9;
	const dmBottomWallZ = -6;
	const dmBottomWallX = 17;

	const showWalls = false;		// To drag the camera around and be able to see the walls, see the note in DieRoller.cshtml.
	const addPlayerWall = true;
	const addDungeonMasterWalls = true;
	if (showWalls) {


		// @ts-ignore - THREE
		const leftWall = new THREE.Mesh(new THREE.BoxGeometry(wallThickness, leftWallHeight, leftWallWidth), redWallMaterial);
		leftWall.position.x = leftWallX;
		scene.add(leftWall);

		// @ts-ignore - THREE
		const topWall = new THREE.Mesh(new THREE.BoxGeometry(topWallWidth, topWallHeight, wallThickness), tealWallMaterial);
		topWall.position.z = topWallZ;
		// @ts-ignore - THREE
		topWall.rotateOnAxis(new THREE.Vector3(1, 0, 0), -45)
		scene.add(topWall);

		if (addPlayerWall) {
			// @ts-ignore - THREE
			const playerTopWall = new THREE.Mesh(new THREE.BoxGeometry(playerTopWallWidth, wallHeight, wallThickness), redWallMaterial);
			playerTopWall.position.x = playerTopWallX;
			playerTopWall.position.z = playerTopWallZ;
			scene.add(playerTopWall);

			// @ts-ignore - THREE
			const playerRightWall = new THREE.Mesh(new THREE.BoxGeometry(wallThickness, wallHeight, playerRightWallWidth), blueWallMaterial);
			playerRightWall.position.x = playerRightWallX;
			playerRightWall.position.z = playerRightWallZ;
			scene.add(playerRightWall);
		}

		if (addDungeonMasterWalls) {
			// @ts-ignore - THREE
			const dmBottomWall = new THREE.Mesh(new THREE.BoxGeometry(dmBottomWallWidth, wallHeight, wallThickness), redWallMaterial);
			dmBottomWall.position.x = dmBottomWallX;
			dmBottomWall.position.z = dmBottomWallZ;
			scene.add(dmBottomWall);

			// @ts-ignore - THREE
			const dmLeftWall = new THREE.Mesh(new THREE.BoxGeometry(wallThickness, wallHeight, dmLeftWallWidth), redWallMaterial);
			dmLeftWall.position.x = dmLeftWallX;
			dmLeftWall.position.z = dmLeftWallZ;
			scene.add(dmLeftWall);
		}
	}


	// Floor
	// @ts-ignore - CANNON
	const floorBody = new CANNON.Body({ mass: 0, shape: new CANNON.Plane(), material: DiceManager.floorBodyMaterial });
	// @ts-ignore - CANNON
	floorBody.quaternion.setFromAxisAngle(new CANNON.Vec3(1, 0, 0), -Math.PI / 2);
	floorBody.name = 'floor';
	world.add(floorBody);

	//Walls
	// @ts-ignore - CANNON
	const rightWall = new CANNON.Body({ mass: 0, shape: new CANNON.Plane(), material: DiceManager.barrierBodyMaterial });
	rightWall.name = 'wall';
	// @ts-ignore - CANNON
	rightWall.quaternion.setFromAxisAngle(new CANNON.Vec3(0, 1, 0), -Math.PI / 2);
	rightWall.position.x = 20.5;
	world.add(rightWall);

	// @ts-ignore - CANNON
	const bottomWall = new CANNON.Body({ mass: 0, shape: new CANNON.Plane(), material: DiceManager.barrierBodyMaterial });
	bottomWall.name = 'wall';
	// @ts-ignore - CANNON
	bottomWall.quaternion.setFromAxisAngle(new CANNON.Vec3(0, 1, 0), -Math.PI / 2 * 2);
	bottomWall.position.z = 11.5;
	world.add(bottomWall);

	// @ts-ignore - CANNON
	const wallDiceContactMaterial = new CANNON.ContactMaterial(DiceManager.barrierBodyMaterial, DiceManager.diceBodyMaterial, { friction: 0.0, restitution: 0.9 });
	world.addContactMaterial(wallDiceContactMaterial);

	//let leftWall = new CANNON.Body({ mass: 0, shape: new CANNON.Plane(), material: DiceManager.floorBodyMaterial });
	//leftWall.quaternion.setFromAxisAngle(new CANNON.Vec3(0, 1, 0), -Math.PI / 2);
	//leftWall.position.x = -20;
	//world.add(leftWall);

	// @ts-ignore - CANNON & DiceManager
	const topCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(topWallWidth, wallHeight, wallThickness)), material: DiceManager.barrierBodyMaterial });
	topCannonWall.name = 'wall';
	topCannonWall.position.z = topWallZ;
	// @ts-ignore - CANNON 
	topCannonWall.quaternion.setFromEuler(-Math.PI / 4, 0, 0);
	world.add(topCannonWall);

	// @ts-ignore - CANNON & DiceManager
	const leftCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(wallThickness, wallHeight, leftWallWidth)), material: DiceManager.barrierBodyMaterial });
	leftCannonWall.name = 'wall';
	leftCannonWall.position.x = leftWallX;
	world.add(leftCannonWall);


	if (addPlayerWall) {
		// @ts-ignore - CANNON & DiceManager
		const playerTopCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(playerTopWallWidth * 0.5, wallHeight, wallThickness)), material: DiceManager.barrierBodyMaterial });
		playerTopCannonWall.name = 'wall';
		playerTopCannonWall.position.x = playerTopWallX;
		playerTopCannonWall.position.z = playerTopWallZ;
		world.add(playerTopCannonWall);

		// @ts-ignore - CANNON & DiceManager
		const playerRightCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(wallThickness, wallHeight, playerRightWallWidth * 0.5)), material: DiceManager.barrierBodyMaterial });
		playerRightCannonWall.name = 'wall';
		playerRightCannonWall.position.x = playerRightWallX;
		playerRightCannonWall.position.z = playerRightWallZ;
		world.add(playerRightCannonWall);
	}

	if (addDungeonMasterWalls) {
		// @ts-ignore - CANNON & DiceManager
		const dmBottomCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(dmBottomWallWidth * 0.5, wallHeight, wallThickness)), material: DiceManager.barrierBodyMaterial });
		dmBottomCannonWall.name = 'wall';
		dmBottomCannonWall.position.x = dmBottomWallX;
		dmBottomCannonWall.position.z = dmBottomWallZ;
		world.add(dmBottomCannonWall);

		// @ts-ignore - CANNON & DiceManager
		const dmLeftCannonWall = new CANNON.Body({ mass: 0, shape: new CANNON.Box(new CANNON.Vec3(wallThickness, wallHeight, dmLeftWallWidth * 0.5)), material: DiceManager.barrierBodyMaterial });
		dmLeftCannonWall.name = 'wall';
		dmLeftCannonWall.position.x = dmLeftWallX;
		dmLeftCannonWall.position.z = dmLeftWallZ;
		world.add(dmLeftCannonWall);
	}

	//var groundShape = new CANNON.Plane();
	//var groundBody = new CANNON.Body({ mass: 0 });
	//groundBody.addShape(groundShape);
	//groundBody.quaternion.setFromAxisAngle(new CANNON.Vec3(1,0,0),-Math.PI/2);
	//world.add(groundBody);


	let needToHookEvents = true;

	const testingDiceRoller = false;

	function allDiceShouldBeDestroyedByNow() {
		allDiceHaveBeenDestroyed(JSON.stringify(lastRollDiceData));
	}

	function getRandomRedBlueHueShift() {
		let hueShift = Math.floor(Math.random() * 360);
		let tryCount = 0;
		while (hueShift > 30 && hueShift < 160 && tryCount++ < 20) {
			hueShift = Math.floor(Math.random() * 360);
		}
		return hueShift;
	}

	function scaleFallingDice() {
		if (!scalingDice || scalingDice.length === 0)
			return;

		const hiddenDie: IDie[] = [];
		//let numDiceScaling = 0;
		if (scalingDice && scalingDice.length > 0) {
			for (let i = 0; i < scalingDice.length; i++) {

				const dieObject: IDieObject = scalingDice[i].getObject();
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
					const screenPos: Vector = getScreenCoordinates(dieObject);

					if (dieObject.effectKind === DieEffect.SteamPunkTunnel) {
						if (screenPos)
							diceLayer.addSteampunkTunnel(screenPos.x, screenPos.y, Math.floor(Math.random() * 360), 100, 100);
						diceSounds.playSteampunkTunnel();
					}
					else if (dieObject.effectKind === DieEffect.HandGrab) {
						let saturation = 100;
						let hueShift: number;
						if (DiceLayer.matchOozeToDieColor)
							if (scalingDice[i].rollType !== DieCountsAs.totalScore && scalingDice[i].rollType !== DieCountsAs.inspiration)
								hueShift = 0;
							else
								hueShift = diceLayer.activePlayerHueShift;
						else {
							hueShift = getRandomRedBlueHueShift();
							if (Math.random() < 0.1)
								saturation = 0;
						}

						if (screenPos)
							diceLayer.testDiceGrab(screenPos.x, screenPos.y, hueShift, saturation, 100);
						diceSounds.playHandGrab();
					}
					else {  // DieEffect.Portal
						if (screenPos)
							diceLayer.addPortal(screenPos.x, screenPos.y, getRandomRedBlueHueShift(), 100, 100);
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
						hideDie(dieObject);
						hiddenDie.push(scalingDice[i]);
					}
					continue;
				}

				if (dieObject.needToDrop === true) {

					dieObject.needToDrop = false;
				}

				const percentTraveled: number = elapsedTime / totalTimeToScale;

				const distanceTraveled: number = percentTraveled * totalScaleDistance;

				const newScale: number = 1 - distanceTraveled;

				if (newScale <= hiddenDieScale) {
					hideDie(dieObject);
					hiddenDie.push(scalingDice[i]);
				}
				else {
					//numDiceScaling++;
					dieObject.scale.set(newScale, newScale, newScale);
				}
				//if (newScale < 0.35) {
				//  bodiesToRemove.push(die);
				//  // @ts-ignore - DiceManager
				//  //die.body.collisionResponse = 1;
				//  //die.body.mass = 1;
				//  //DiceManager.world.remove(die.body);
				//}
			}
		}
		removeDiceFromArray(hiddenDie, scalingDice);
		removeDiceFromArray(hiddenDie, dice);
		clearTheseDice(hiddenDie);
	}

	function updatePhysics() {
		//  if (bodiesToRemove && bodiesToRemove.length > 0) {
		//    console.log('removing bodies...');

		//    bodiesToRemove.forEach(function (body) {
		//      body.collisionResponse = true;
		//      body.mass = 1;
		//      world.remove(body);
		//    });
		//    bodiesToRemove = [];
		//  }

		world.step(1.0 / 60.0);

		for (const i in dice) {
			dice[i].updateMeshFromBody();
		}
		checkStillRolling();
		scaleFallingDice();
		highlightSpecialDice();

		const numDiceStillInPlay: number = diceRemainingInPlay();
		const stillScaling: boolean = scalingDice !== null && scalingDice.length > 0;
		const stillHaveSpecialDice: boolean = specialDice !== null && specialDice.length > 0;
		//console.log(`numDiceStillInPlay = ${numDiceStillInPlay}, animationsShouldBeDone = ${animationsShouldBeDone}, allDiceHaveStoppedRolling = ${allDiceHaveStoppedRolling}, stillScaling = ${stillScaling}`);

		if (!animationsShouldBeDone && numDiceStillInPlay === 0 && allDiceHaveStoppedRolling &&
			!stillScaling && !stillHaveSpecialDice) {
			animationsShouldBeDone = true;
			//console.log('animationsShouldBeDone = true;');
			diceRollData = null;
			dice = [];
			setTimeout(allDiceShouldBeDestroyedByNow, 3000);
		}
	}

	function update() {
		controls.update();
		if (stats) {
			stats.update();
		}
	}

	function movePointAtAngle(point: Vector, angleInDegrees: number, distance): Vector {
		const angleInRadians: number = angleInDegrees * Math.PI / 180;
		return new Vector(point.x + (Math.sin(angleInRadians) * distance), point.y - (Math.cos(angleInRadians) * distance));
	}

	function getDieSpeed(die: IDie): number {
		const dieObject = die.getObject();
		const velocity = dieObject.body.velocity;
		return Math.sqrt(velocity.x * velocity.x + velocity.y * velocity.y + velocity.z * velocity.z);
	}

	const highestDieSpeedWeHaveSeen = 128;

	// Returns a number between 0 and 1, where 1 is max speed and 0 is no movement.
	function getNormalizedDieSpeed(die: IDie): number {
		const dieSpeed: number = getDieSpeed(die);
		//trackDieVelocities(dieSpeed);
		return MathEx.clamp(dieSpeed, 0, highestDieSpeedWeHaveSeen) / highestDieSpeedWeHaveSeen;
	}

	function positionTrailingSprite(die: IDie, trailingEffect: TrailingEffect, index = 0): SpriteProxy {
		if (die.rollType === DieCountsAs.totalScore || die.rollType == DieCountsAs.inspiration || die.rollType == DieCountsAs.bentLuck) {
			const pos: Vector = getScreenCoordinates(die.getObject());
			if (!pos)
				return null;

			const dieNormalizedSpeed: number = getNormalizedDieSpeed(die);


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
				const printPos: Vector = movePointAtAngle(pos, angle + angleToMovePawPrint, trailingEffect.LeftRightDistanceBetweenPrints * scaleFactor);

				const spriteProxy: SpriteProxy = diceLayer.AddTrailingEffect(trailingEffect, printPos.x, printPos.y, angle, spriteScale);
				die.lastPos[index] = pos;
				return spriteProxy;
			}
		}
		return null;
	}

	function addTrailingEffects(die: IDie, trailingEffects: Array<TrailingEffect>) {
		for (let j = 0; j < trailingEffects.length; j++) {
			const trailingEffect: TrailingEffect = trailingEffects[j];

			if (positionTrailingSprite(die, trailingEffect, j)) {
				if (trailingEffect.OnPrintPlaySound) {
					if (trailingEffect.intervalBetweenSounds == 0)
						trailingEffect.intervalBetweenSounds = trailingEffect.MedianSoundInterval + Random.plusMinus(trailingEffect.PlusMinusSoundInterval);
					if (diceSounds.safePlayMp3(trailingEffect.OnPrintPlaySound, trailingEffect.intervalBetweenSounds)) {
						trailingEffect.intervalBetweenSounds = trailingEffect.MedianSoundInterval + Random.plusMinus(trailingEffect.PlusMinusSoundInterval);
					}
				}
			}

			//let addSpriteFunc: any;
			//if (trailingEffect.Type === SpriteType.PawPrint)
			//	addSpriteFunc = diceLayer.addPawPrint.bind(diceLayer);
			//else if (trailingEffect.Type === SpriteType.Raven)
			//	addSpriteFunc = diceLayer.addRaven.bind(diceLayer);
			//else if (trailingEffect.Type === SpriteType.Smoke)
			//	addSpriteFunc = diceLayer.addSmoke.bind(diceLayer);
			//else if (trailingEffect.Type === SpriteType.SparkTrail)
			//	addSpriteFunc = diceLayer.addSparkTrail.bind(diceLayer);
			//else if (trailingEffect.Type === SpriteType.SmallSparks)
			//	addSpriteFunc = diceLayer.smallSpark.bind(diceLayer);
			//else if (trailingEffect.Type === SpriteType.Fangs)
			//	addSpriteFunc = diceLayer.addFangs.bind(diceLayer);
			//else if (trailingEffect.Type === SpriteType.Spiral)
			//	addSpriteFunc = diceLayer.addSpiral.bind(diceLayer);
			//else
			//	continue;

			//if (positionTrailingSprite(die, addSpriteFunc, trailingEffect.MinForwardDistanceBetweenPrints,
			//	trailingEffect.LeftRightDistanceBetweenPrints, j)) {
			//	if (trailingEffect.OnPrintPlaySound) {
			//		if (trailingEffect.intervalBetweenSounds == 0)
			//			trailingEffect.intervalBetweenSounds = trailingEffect.MedianSoundInterval + Random.plusMinus(trailingEffect.PlusMinusSoundInterval);
			//		if (diceSounds.playRandom(trailingEffect.OnPrintPlaySound, trailingEffect.NumRandomSounds, trailingEffect.intervalBetweenSounds)) {
			//			trailingEffect.intervalBetweenSounds = trailingEffect.MedianSoundInterval + Random.plusMinus(trailingEffect.PlusMinusSoundInterval);
			//		}
			//	}
			//}
		}
	}

	// TODO: For goodness sakes, Mark, do something with this.
	function old_positionTrailingSprite(die: IDie, addPrintFunc: (x: number, y: number, angle: number) => SpriteProxy, minForwardDistanceBetweenPrints: number, leftRightDistanceBetweenPrints: number = 0, index: number = 0): SpriteProxy {
		if (die.rollType === DieCountsAs.totalScore || die.rollType === DieCountsAs.inspiration || die.rollType === DieCountsAs.bentLuck) {
			const pos: Vector = getScreenCoordinates(die.getObject());
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
				const printPos: Vector = movePointAtAngle(pos, angle + angleToMovePawPrint, leftRightDistanceBetweenPrints);
				const spriteProxy: SpriteProxy = addPrintFunc(printPos.x, printPos.y, angle);
				//diceLayer.addPawPrint(pawPrintPos.x, pawPrintPos.y, angle);
				die.lastPos[index] = pos;
				return spriteProxy;
			}
		}
		return null;
	}

	function updateDieRollSpecialEffects() {
		for (let i = 0; i < dice.length; i++) {
			const die: IDie = dice[i];

			if (die.rollType === DieCountsAs.bentLuck)
				addTrailingEffects(die, diceRollData.secondRollData.trailingEffects);
			else
				addTrailingEffects(die, diceRollData.trailingEffects);

			if (die.rollType === DieCountsAs.inspiration) {
				const distanceBetweenRipples = 80;
				const ripple: ColorShiftingSpriteProxy = old_positionTrailingSprite(die, diceLayer.addRipple.bind(diceLayer), distanceBetweenRipples, 0, diceRollData.trailingEffects.length) as ColorShiftingSpriteProxy;

				if (ripple) {
					ripple.opacity = 0.5;
					ripple.hueShift = diceLayer.getHueShift(die.playerID) + Random.plusMinus(30);
				}
			}

			const screenPos: Vector = getScreenCoordinates(die.getObject());

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
				die.sparks.forEach(function (spark: SpriteProxy) {
					const newLocationWeight = 0.1;
					spark.x = spark.x * (1 - newLocationWeight) + newX * newLocationWeight;
					spark.y = spark.y * (1 - newLocationWeight) + newY * newLocationWeight;
				});
			}
		}
	}

	function animateDiceRollerFps(nowMs: DOMHighResTimeStamp) {
		try {
			const now: number = Date.now();
			const elapsed: number = now - lastDrawTimeDiceRoller;

			const okayToDrawImages: boolean = elapsed > fpsIntervalDiceRoller;
			if (!okayToDrawImages)
				return;

			const startUpdate: number = performance.now();
			if (!testingDiceRoller) {
				updatePhysics();
				renderer.render(scene, camera);
			}

			lastDrawTimeDiceRoller = now - (elapsed % fpsIntervalDiceRoller);
			updateDieRollSpecialEffects();
			diceLayer.renderCanvas();

			update();

			if (diceRollerShowFpsWindow) {
				if (!diceRollerFpsWindow) {
					diceRollerFpsWindow = new FpsWindow('Dice', 1);
				}
				diceRollerFpsWindow.showAllFramerates(diceRollerTimeBetweenFramesQueue, diceRollerDrawTimeForEachFrameQueue, diceLayer.diceFrontContext, now);
			}


			const endUpdate: number = performance.now();
			calculateFramerate(startUpdate, endUpdate);
		}
		finally {
			requestAnimationFrame(animateDiceRollerFps);
		}
	}

	function startAnimatingDiceRoller(fps: number): void {
		changeFramerateDiceRoller(fps);
		requestAnimationFrame(animateDiceRollerFps);
	}

	function randomDiceThrow() {
		clearBeforeRoll();

		const dieValues = 10;
		for (let i = 0; i < diceToRoll; i++) {
			// @ts-ignore - DiceD20
			const die = new DiceD10x01({ size: dieScale, backColor: '#D0D0ff' });
			scene.add(die.getObject());
			dice.push(die);
		}
		needToHookEvents = true;

		for (let i = 0; i < dice.length; i++) {
			const yRand = Math.random() * 20
			const dieObject = dice[i].getObject();
			dieObject.position.x = -15 - (i % 3) * dieScale;
			dieObject.position.y = 4 + Math.floor(i / 3) * dieScale;
			dieObject.position.z = -13 + (i % 3) * dieScale;
			dieObject.quaternion.x = (Math.random() * 90 - 45) * Math.PI / 180;
			dieObject.quaternion.z = (Math.random() * 90 - 45) * Math.PI / 180;
			dice[i].updateBodyFromMesh();
			const rand = Math.random() * 5;
			dieObject.body.velocity.set(35 + rand, 10 + yRand, 25 + rand);
			dieObject.body.angularVelocity.set(20 * Math.random() - 10, 20 * Math.random() - 10, 20 * Math.random() - 10);

			diceValues.push({ dice: dice[i], value: Math.floor(Math.random() * dieValues + 1) });
			dieObject.body.name = 'die';
		}

		allDiceHaveStoppedRolling = false;

		// @ts-ignore - DiceManager
		DiceManager.prepareValues(diceValues);

		if (needToHookEvents) {
			// Test to see if this is related to the memory leak:

			needToHookEvents = false;
			for (let i = 0; i < dice.length; i++) {
				const die = dice[i].getObject();

				die.body.addEventListener("collide", handleDieCollision);
			}
		}
	}

	if (repeatRandomThrow) {
		randomDiceThrowIntervalId = setInterval(randomDiceThrow, secondsBetweenRolls * 1000);
		randomDiceThrow();
	}
	startAnimatingDiceRoller(30);
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
function handleFpsChangeDiceRoller(frameRateChangeData: FrameRateChangeData) {
	changeFramerateDiceRoller(frameRateChangeData.FrameRate);
}

//! Keep for diagnostics
function getDiceValue(die: IDie) {
	for (let i = 0; i < diceValues.length; i++) {
		const thisDiceValueEntry = diceValues[i];
		if (thisDiceValueEntry.dice === die) {
			if (thisDiceValueEntry.value !== die.getUpsideValue())
				console.error(`thisDiceValueEntry.value (${thisDiceValueEntry.value}) != dice.getUpsideValue() (${die.getUpsideValue()})`);
			return thisDiceValueEntry.value;
		}
	}
	return 0;
}

function queueRoll(diceRollData: DiceRollData) {
	console.log('queueRoll - TODO');
	// TODO: queue this roll for later when the current roll has stopped.
}

function addD100(diceRollData: DiceRollData, backgroundColor: string, textColor: string, playerID: number, throwPower = 1, xPositionModifier = 0) {
	const magicRingHueShift: number = Math.floor(Math.random() * 360);
	// @ts-ignore - DiceD10x10
	const die = new DiceD10x10({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
	die.playerID = playerID;
	prepareD10x10Die(die, throwPower, xPositionModifier);
	die.rollType = DieCountsAs.totalScore;
	if (diceRollData.isMagic) {
		die.attachedSprites.push(diceLayer.addMagicRing(960, 540, magicRingHueShift));
		die.origins.push(new Vector(diceLayer.magicRingRed.originX, diceLayer.magicRingRed.originY));
	}

	// @ts-ignore - DiceD10x01
	die = new DiceD10x01({ size: dieScale, backColor: backgroundColor, fontColor: textColor });
	die.playerID = playerID;
	prepareD10x01Die(die, throwPower, xPositionModifier);
	die.rollType = DieCountsAs.totalScore;
	if (diceRollData.isMagic) {
		die.attachedSprites.push(diceLayer.addMagicRing(960, 540, magicRingHueShift + Random.plusMinusBetween(10, 25)));
		die.origins.push(new Vector(diceLayer.magicRingRed.originX, diceLayer.magicRingRed.originY));
	}
}

//function render() {
//	renderer.render(scene, camera);

//	updateDieRollSpecialEffects();

//	diceLayer.renderCanvas();
//}

init();

let rollingOnlyAddOnDice: boolean;
let secondRollTryCount = 0;

function isLuckBent(localDiceRollData: DiceRollData) {
	return localDiceRollData.type === DiceRollType.BendLuckAdd || localDiceRollData.type === DiceRollType.BendLuckSubtract;
}

function addD20(diceRollData: DiceRollData, d20BackColor: string, d20FontColor: string, xPositionModifier: number, playerID = -1): IDie {
	// @ts-ignore - DiceD20
	const die: IDie = new DiceD20({ size: dieScale, backColor: d20BackColor, fontColor: d20FontColor });
	die.isD20 = true;
	prepareDie(die, diceRollData.throwPower, xPositionModifier);
	die.rollType = DieCountsAs.totalScore;
	die.playerID = playerID;
	return die;
}

function addBadLuckEffects(die: IDie) {
	die.attachedSprites.push(diceLayer.addBadLuckRing(960, 540));
	die.origins.push(new Vector(diceLayer.badLuckRing.originX, diceLayer.badLuckRing.originY));
}

function addGoodLuckEffects(die: IDie) {
	die.attachedSprites.push(diceLayer.addLuckyRing(960, 540));
	die.origins.push(new Vector(diceLayer.cloverRing.originX, diceLayer.cloverRing.originY));
}

function rollAddOnDice() {
	if (!diceRollData)
		return;

	allDiceHaveStoppedRolling = false;

	let xPositionModifier = 0;

	if (Math.random() * 100 < 50)
		xPositionModifier = 26;  // Throw from the right to the left.

	const localDiceRollData: DiceRollData = getMostRecentDiceRollData();

	let die: IDie;
	const throwPower: number = diceRollData.throwPower * 1.2;

	let dieBack: string;
	let dieFont: string;

	if (diceRollData.bentLuckMultiplier < 0) {
		// Bad luck
		dieBack = DiceLayer.badLuckDieColor;
		dieFont = DiceLayer.badLuckFontColor;
	}
	else {
		dieBack = DiceLayer.goodLuckDieColor;
		dieFont = DiceLayer.goodLuckFontColor;
	}

	if (isLuckBent(localDiceRollData)) {
		die = addDie('d4', DamageType.None, DieCountsAs.bentLuck, dieBack, dieFont, throwPower, xPositionModifier, false);
		die.isLucky = true;
	}
	else {
		if (localDiceRollData.itsAD20Roll) {   // TODO: Send itsAD20Roll from DM app.
			die = addD20(localDiceRollData, dieBack, dieFont, xPositionModifier);
			die.isLucky = true;  // TODO: Send IsLucky from DM app.
		}
		else {
			//let saveDamageModifier: number = damageModifierThisRoll;
			addDieFromStr(localDiceRollData.playerRollOptions[0].PlayerID, localDiceRollData.damageHealthExtraDice, DieCountsAs.totalScore, throwPower);
			//damageModifierThisRoll += saveDamageModifier;
		}
	}

	const isGoodLuck: boolean = isLuckBent(localDiceRollData) && diceRollData.bentLuckMultiplier > 0 || localDiceRollData.type === DiceRollType.LuckRollHigh;
	const isBadLuck: boolean = isLuckBent(localDiceRollData) && diceRollData.bentLuckMultiplier < 0 || localDiceRollData.type === DiceRollType.LuckRollLow;

	if (isGoodLuck) {
		//console.log('addGoodLuckEffects...');
		addGoodLuckEffects(die);
	}
	else if (isBadLuck) {
		//console.log('addBadLuckEffects...');
		addBadLuckEffects(die);
	}
}

function throwAdditionalDice() {
	secondRollTryCount++;

	if (!rollingOnlyAddOnDice && !allDiceHaveStoppedRolling && secondRollTryCount < 30) {
		setTimeout(throwAdditionalDice, 300);
		return;
	}

	freezeExistingDice();
	diceLayer.clearTextEffects();

	const localDiceRollData: DiceRollData = getMostRecentDiceRollData();

	if (isLuckBent(localDiceRollData))
		diceLayer.reportAddOnRoll(diceRollData.secondRollTitle, diceRollData.bentLuckMultiplier);
	else {
		if (!diceRollData.damageHealthExtraDice)
			localDiceRollData.itsAD20Roll = true;
		else if (localDiceRollData.type === DiceRollType.LuckRollHigh || localDiceRollData.type === DiceRollType.LuckRollLow)
			localDiceRollData.itsAD20Roll = true;

		diceLayer.reportAddOnRoll(diceRollData.secondRollTitle, diceRollData.bentLuckMultiplier);
	}

	diceSounds.safePlayMp3('PaladinThunder');
	if (rollingOnlyAddOnDice)
		rollAddOnDice();
	else
		setTimeout(rollAddOnDice, 2500);
}

function needToRollAddOnDice(diceRollDto: DiceRollData, bentLuckMultiplier = 0) {
	if (diceRollData && diceRollData.secondRollData)
		return;

	if (diceRollData === null) {
		diceRollData = diceRollDto;
		rollingOnlyAddOnDice = true;
	}

	diceRollData.bentLuckMultiplier = bentLuckMultiplier;
	diceRollData.secondRollData = diceRollDto;
	diceRollData.secondRollData.vantageKind = diceRollData.vantageKind;

	secondRollTryCount = 0;
	throwAdditionalDice();
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

function getDiceInPlay(): number {
	let diceInPlay = 0;
	for (let i = 0; i < dice.length; i++) {
		const die: IDie = dice[i];
		if (die.inPlay) {
			diceInPlay++;
		}
	}

	return diceInPlay;
}

function testD20Removal(roll1: number, roll2: number, roll3: number, vantage: VantageKind, type: DiceRollType): MockDie {
	dice = [];
	diceRollData.vantageKind = vantage;
	diceRollData.type = type;
	diceRollData.itsAD20Roll = true;
	dice.push(new MockDie('A', roll1));
	dice.push(new MockDie('B', roll2));

	let diceInPlay: number = getDiceInPlay();

	if (diceInPlay !== 2)
		console.error('a) diceInPlay: ' + diceInPlay);
	removeD20s();
	diceInPlay = getDiceInPlay();
	if (diceInPlay !== 1)
		console.error('b) diceInPlay: ' + diceInPlay);

	dice.push(new MockDie('C', roll3));
	removeD20s();
	diceInPlay = 0;
	let returnDie: MockDie;
	for (let i = 0; i < dice.length; i++) {
		const die: IDie = dice[i];
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
function d20RollIncludes(topNumber: number): boolean {
	for (let i = 0; i < dice.length; i++) {
		const die: IDie = dice[i];

		if (die.isD20 && topNumber === die.getTopNumber())
			return true;
	}
	return false;
}

function addD20sForPlayer(playerID: number, xPositionModifier: number, kind: VantageKind, inspiration = '', numD20s = 1, dieLabelOverride: string = null) {
	const d20BackColor: string = diceLayer.getDieColor(playerID);
	const d20FontColor: string = diceLayer.getDieFontColor(playerID);
	if (kind !== VantageKind.Normal)
		numD20s = 2;

	const magicRingHueShift: number = Math.floor(Math.random() * 360);

	for (let i = 0; i < numD20s; i++) {
		const die = addD20(diceRollData, d20BackColor, d20FontColor, xPositionModifier);
		if (dieLabelOverride)
			die.dieType = dieLabelOverride;
		else
			die.dieType = DiceRollType[diceRollData.type];
		attachLabel(die, d20FontColor, d20BackColor);
		die.playerID = playerID;
		die.playerName = diceLayer.getPlayerName(playerID);
		console.log('die.playerName: ' + die.playerName);
		die.kind = kind;
		if (diceRollData.isMagic) {
			die.attachedSprites.push(diceLayer.addMagicRing(960, 540, magicRingHueShift + Random.plusMinusBetween(10, 25)));
			die.origins.push(new Vector(diceLayer.magicRingRed.originX, diceLayer.magicRingRed.originY));
		}
		if (diceRollData.numHalos > 0) {
			const angleDelta: number = 360 / diceRollData.numHalos;
			let angle: number = Math.random() * 360;
			for (let j = 0; j < diceRollData.numHalos; j++) {
				die.attachedSprites.push(diceLayer.addHaloSpin(960, 540, diceLayer.activePlayerHueShift + Random.plusMinus(30), angle));
				die.origins.push(diceLayer.haloSpinRed.getOrigin());
				angle += angleDelta;
			}
		}
	}

	if (inspiration) {
		addDieFromStr(playerID, inspiration, DieCountsAs.inspiration, diceRollData.throwPower, xPositionModifier, d20BackColor, d20FontColor, diceRollData.isMagic);
	}
}

function addDiceFromDto(diceDto: DiceDto, xPositionModifier: number) {
	// TODO: Check DieCountsAs.totalScore - do we want to set that from C# side of things?
	const die: IDie = createDie(diceDto.Quantity, diceDto.Sides, diceDto.DamageType, DieCountsAs.totalScore, diceDto.BackColor, diceDto.FontColor, diceRollData.throwPower, xPositionModifier, diceDto.IsMagic, diceDto.CreatureId);
	die.playerName = diceDto.PlayerName;
	die.dataStr = diceDto.Data;
	die.dieType = DiceRollType[DiceRollType.None];
	if (diceDto.Label)
		diceLayer.attachLabel(die, diceDto.Label, diceDto.FontColor, diceDto.BackColor); // So the text matches the die color.
	die.kind = diceDto.Vantage;
	if (diceDto.IsMagic) {
		die.attachedSprites.push(diceLayer.addMagicRing(960, 540, Random.max(360)));
		die.origins.push(new Vector(diceLayer.magicRingRed.originX, diceLayer.magicRingRed.originY));
	}
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

function prepareDiceDtoRoll(diceRollDto: DiceRollData, xPositionModifier: number) {
	for (let i = 0; i < diceRollDto.diceDtos.length; i++) {
		const diceDto: DiceDto = diceRollDto.diceDtos[i];
		addDiceFromDto(diceDto, xPositionModifier);
	}

	diceRollData.hasMultiPlayerDice = diceRollDto.diceDtos.length > 0;  // Any DiceDtos (even one) will go in a multiplayerSummary!
}

function prepareLegacyRoll(xPositionModifier: number) {
	let playerID = -1;
	if (diceRollData.playerRollOptions.length === 1)
		playerID = diceRollData.playerRollOptions[0].PlayerID;
	if (diceRollData.type === DiceRollType.WildMagic) {
		diceRollData.modifier = 0;
		diceRollData.itsAD20Roll = false;
		addD100(diceRollData, diceLayer.activePlayerDieColor, diceLayer.activePlayerDieFontColor, playerID, diceRollData.throwPower, xPositionModifier);
	}
	else if (diceRollData.type === DiceRollType.PercentageRoll) {
		diceRollData.modifier = 0;
		diceRollData.itsAD20Roll = false;
		if (diceRollData.rollScope === RollScope.ActivePlayer) {
			addD100(diceRollData, diceLayer.activePlayerDieColor, diceLayer.activePlayerDieFontColor, playerID, diceRollData.throwPower, xPositionModifier);
			diceRollData.hasSingleIndividual = true;
		}
		else if (diceRollData.rollScope === RollScope.Individuals) {
			diceRollData.playerRollOptions.forEach(function (playerRollOption: PlayerRollOptions) {
				addD100(diceRollData, diceLayer.activePlayerDieColor, diceLayer.activePlayerDieFontColor, playerRollOption.PlayerID, diceRollData.throwPower, xPositionModifier);
			});
			diceRollData.hasMultiPlayerDice = diceRollData.playerRollOptions.length > 1;
			diceRollData.hasSingleIndividual = diceRollData.playerRollOptions.length === 1;
		}
	}
	else if (diceRollData.type === DiceRollType.Initiative || diceRollData.type === DiceRollType.NonCombatInitiative) {
		diceRollData.modifier = 0;
		diceRollData.maxInspirationDiceAllowed = 4;
		diceRollData.itsAD20Roll = true;
		for (let i = 0; i < diceLayer.players.length; i++) {
			const player: Character = diceLayer.players[i];
			if (player.Hidden)
				continue;
			let initiativeBonus: number;
			if (diceRollData.type === DiceRollType.NonCombatInitiative)
				initiativeBonus = 0;
			else
				initiativeBonus = player.rollInitiative;
			addD20sForPlayer(player.playerID, xPositionModifier, initiativeBonus, player.inspiration);
		}
		diceRollData.hasMultiPlayerDice = true;
	}
	else if (diceRollData.type === DiceRollType.DamageOnly) {
		diceRollData.modifier = 0;
		diceRollData.itsAD20Roll = false;
		addDieFromStr(playerID, diceRollData.damageHealthExtraDice, DieCountsAs.damage, diceRollData.throwPower, xPositionModifier, undefined, undefined, diceRollData.isMagic);
	}
	else if (diceRollData.type === DiceRollType.HealthOnly) {
		diceRollData.modifier = 0;
		diceRollData.itsAD20Roll = false;
		addDieFromStr(playerID, diceRollData.damageHealthExtraDice, DieCountsAs.health, diceRollData.throwPower, xPositionModifier, DiceLayer.healthDieBackgroundColor, DiceLayer.healthDieFontColor, diceRollData.isMagic);
	}
	else if (diceRollData.type === DiceRollType.ExtraOnly) {
		diceRollData.modifier = 0;
		diceRollData.itsAD20Roll = false;
		addDieFromStr(playerID, diceRollData.damageHealthExtraDice, DieCountsAs.extra, diceRollData.throwPower, xPositionModifier, DiceLayer.extraDieBackgroundColor, DiceLayer.extraDieFontColor, diceRollData.isMagic);
	}
	else if (diceRollData.type === DiceRollType.InspirationOnly) {
		diceRollData.modifier = 0;
		diceRollData.itsAD20Roll = false;
		for (let i = 0; i < diceRollData.playerRollOptions.length; i++) {
			const playerRollOptions: PlayerRollOptions = diceRollData.playerRollOptions[i];
			addD20sForPlayer(playerRollOptions.PlayerID, xPositionModifier, playerRollOptions.VantageKind, playerRollOptions.Inspiration, 0);
		}
	}
	else if (diceRollData.damageHealthExtraDice.indexOf('d20') >= 0) {
		let dieStr: string = diceRollData.damageHealthExtraDice;
		if (dieStr === '1d20("Wild Magic Check")') {
			const numD20s: number = diceRollData.modifier;
			diceRollData.modifier = 0;
			dieStr = numD20s.toString() + 'd20("Wild Magic Check")';
		}
		const d20BackColor: string = diceLayer.getDieColor(playerID);
		const d20FontColor: string = diceLayer.getDieFontColor(playerID);
		//addDieFromStr(playerID, dieStr, DieCountsAs.totalScore, diceRollData.throwPower, xPositionModifier, diceLayer.activePlayerDieColor, diceLayer.activePlayerDieFontColor);
		addDieFromStr(playerID, dieStr, DieCountsAs.totalScore, diceRollData.throwPower, xPositionModifier, d20BackColor, d20FontColor);
	}
	else {
		diceRollData.itsAD20Roll = true;
		if (diceRollData.rollScope === RollScope.ActivePlayer) {
			const activePlayerRollOptions: PlayerRollOptions = diceRollData.playerRollOptions[0];
			// TODO: I think there's a bug here active player's inspiration needs to be used.
			let vantageKind: VantageKind = diceRollData.vantageKind;
			if (diceRollData.playerRollOptions.length === 1) {
				vantageKind = activePlayerRollOptions.VantageKind;
			}
			let numD20s = 1;
			let dieLabel = '';
			if (diceRollData.type === DiceRollType.WildMagicD20Check) {
				numD20s = diceRollData.modifier;
				diceRollData.modifier = 0;
				dieLabel = '"Wild Magic Check"';
			}
			let playerId: number = diceLayer.playerID;
			if (activePlayerRollOptions)
				playerId = activePlayerRollOptions.PlayerID;
			addD20sForPlayer(playerId, xPositionModifier, vantageKind, diceRollData.groupInspiration, numD20s, dieLabel);
			diceRollData.hasSingleIndividual = true;
		}
		else if (diceRollData.rollScope === RollScope.Individuals) {
			diceRollData.playerRollOptions.forEach(function (playerRollOption: PlayerRollOptions) {
				addD20sForPlayer(playerRollOption.PlayerID, xPositionModifier, playerRollOption.VantageKind, playerRollOption.Inspiration);
			});
			diceRollData.hasMultiPlayerDice = diceRollData.playerRollOptions.length > 1;
			diceRollData.hasSingleIndividual = diceRollData.playerRollOptions.length === 1;
		}
		if (isAttack(diceRollData)) {
			addDieFromStr(playerID, diceRollData.damageHealthExtraDice, DieCountsAs.damage, diceRollData.throwPower, xPositionModifier);
		}
	}
	return playerID;
}

//! Called from Connection.ts
function pleaseRollDice(diceRollDto: DiceRollData) {

	DiceLayer.numFiltersOnDieCleanup = 0;
	DiceLayer.numFiltersOnRoll = 0;
	//testing = true;
	//diceRollData = diceRollDto;

	//if (testD20Removal(3, 2, 1, DiceRollKind.Disadvantage, DiceRollType.LuckRollHigh).name != 'A') debugger;
	//if (testD20Removal(4, 5, 7, DiceRollKind.Disadvantage, DiceRollType.LuckRollLow).name != 'B') debugger;
	//return;

	animationsShouldBeDone = false;
	rollingOnlyAddOnDice = false;


	if (diceRollDto.type === DiceRollType.BendLuckAdd || diceRollDto.type === DiceRollType.LuckRollHigh) {
		needToRollAddOnDice(diceRollDto, +1);
		return;
	}
	else if (diceRollDto.type === DiceRollType.BendLuckSubtract || diceRollDto.type === DiceRollType.LuckRollLow) {
		needToRollAddOnDice(diceRollDto, -1);
		return;
	}
	else if (diceRollDto.type === DiceRollType.AddOnDice) {
		needToRollAddOnDice(diceRollDto, 0);
		return;
	}

	diceRollData = diceRollDto;
	diceRollData.timeLastRolledMs = performance.now();
	attemptedRollWasSuccessful = false;
	wasCriticalHit = false;
	attemptedRollWasNarrowlySuccessful = false;

	if (randomDiceThrowIntervalId !== 0) {
		clearInterval(randomDiceThrowIntervalId);
		randomDiceThrowIntervalId = 0;
	}

	// @ts-ignore - DiceManager
	if (DiceManager.throwRunning) {
		queueRoll(diceRollData);
		return;
	}

	clearBeforeRoll();

	let xPositionModifier = 0;

	if (Math.random() * 100 < 50)
		xPositionModifier = 26;  // Throw from the right to the left.

	if (diceRollDto.diceDtos && diceRollDto.diceDtos.length > 0) {
		//console.log('prepareDiceDtoRoll...');
		prepareDiceDtoRoll(diceRollDto, xPositionModifier);
		console.log(dice);
	}


	if (!diceRollDto.suppressLegacyRoll) {
		//console.log('prepareLegacyRoll...');
		prepareLegacyRoll(xPositionModifier);
	}
	//console.log(dice);

	try {
		// @ts-ignore - DiceManager
		DiceManager.prepareValues(diceValues);
	}
	catch (ex) {
		console.log('exception on call to DiceManager.prepareValues: ' + ex);
	}

	if (diceRollData.onThrowSound) {
		diceSounds.safePlayMp3(diceRollData.onThrowSound);
	}
	//startedRoll = true;
}