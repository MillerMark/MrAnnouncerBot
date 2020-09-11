class AsyncAudioPlayer {
	constructor() {

	}
	play(soundManager: SoundManager, fileName: string, delayMs = 0): Promise<void> {
		//console.log(`!!! - Playing ${fileName}...`);
		const promise: Promise<void> = new Promise<void>((resolve) => {
			const audio: HTMLAudioElement = soundManager.safePlayMp3ReturnAudio(fileName);
			audio.addEventListener('loadedmetadata', () => {
				const delayToSuccessMs: number = audio.duration * 1000 + delayMs;
				setTimeout(resolve, delayToSuccessMs);
			}, false);
		}
		);
		return promise;
	}
}

class DiceSounds extends SoundManager {
	hitDice: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
	hitFloor: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
	hitWall: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();
	settles: Array<HTMLAudioElement> = new Array<HTMLAudioElement>();

	lastSettleTime: Date;

	constructor(soundPath: string) {
		super(soundPath);
		this.loadPercentageSoundEffects(this.hitDice, "Dice/Die", 6);
		this.loadPercentageSoundEffects(this.hitFloor, "Dice/Floor", 4);
		this.loadPercentageSoundEffects(this.hitWall, "Dice/Wall", 5);
		this.loadPercentageSoundEffects(this.settles, "Dice/Settles", 7);
	}

	playAttackCommentary(d20RollValue: number, totalDamage: number, maxDamage: number): void {
		const audio: HTMLAudioElement = diceSounds.playNumber(d20RollValue);
		audio.addEventListener('loadedmetadata', () => {
			this.playAttackResultCommentary(audio, d20RollValue, maxDamage, totalDamage);
		}, false);
	}

	private playAttackResultCommentary(audio: HTMLAudioElement, d20RollValue: number, maxDamage: number, totalDamage: number) {
		const durationMS: number = audio.duration * 1000 - 400;
		console.log('durationMS: ' + durationMS);
		if (attemptedRollWasSuccessful) {
			this.playSuccessfulAttackCommentary(d20RollValue, durationMS, maxDamage, totalDamage);
		}
		else {
			this.playFailedAttackCommentary(d20RollValue, durationMS, maxDamage, totalDamage);
		}
	}

	private playFailedAttackCommentary(d20RollValue: number, delayPlayMs: number, maxDamage: number, totalDamage: number) {
		if (d20RollValue === 1) {
			diceSounds.playMp3In(delayPlayMs, 'Announcer/SpectacularMiss[9]');
		}
		else {
			if (maxDamage > 0)
				if (totalDamage === maxDamage)
					diceSounds.playMp3In(delayPlayMs, 'Announcer/MaxDamageMiss[0]');
				else if (totalDamage / maxDamage > 0.7)
					diceSounds.playMp3In(delayPlayMs, 'Announcer/LotsOfDamageMiss[0]');
				else
					diceSounds.playMp3In(delayPlayMs, 'Announcer/Miss[0]');
			else {
				// TODO: add gender misses.
				diceSounds.playMp3In(delayPlayMs, 'Announcer/Miss[0]');
			}
		}
	}

	private playSuccessfulAttackCommentary(d20RollValue: number, delayMs: number, maxDamage: number, totalDamage: number) {
		if (d20RollValue >= diceRollData.minCrit) {
			diceSounds.playMp3In(delayMs, 'Announcer/CriticalHit[21]');
		}
		else {
			if (maxDamage > 0) {
				this.playAttackCommentaryWithMaxDamage(delayMs, totalDamage, maxDamage);
			}
			else {
				// TODO: add gender hits.
				diceSounds.playMp3In(delayMs, 'Announcer/Hit[25]');
			}
		}
	}

	private playAttackCommentaryWithMaxDamage(delayMs: number, totalDamage: number, maxDamage: number) {
		// TODO: Move likelyMorningCodeRushedShow to a global function.
		const time: Date = new Date();
		const likelyMorningCodeRushedShow: boolean = time.getHours() < 16;

		if (likelyMorningCodeRushedShow)
			diceSounds.playMp3In(delayMs, 'Announcer/Hit[25]');
		else if (totalDamage === maxDamage)
			diceSounds.playMp3In(delayMs, 'Announcer/MaxDamage[12]');
		else if (totalDamage / maxDamage > 0.7 && totalDamage > 7)
			diceSounds.playMp3In(delayMs, 'Announcer/LotsOfDamage[24]');
		else if (totalDamage / maxDamage < 0.3)
			diceSounds.playMp3In(delayMs, 'Announcer/NotMuchDamage[22]');
		else
			diceSounds.playMp3In(delayMs, 'Announcer/Hit[25]');
	}

	playNumber(value: number): HTMLAudioElement {
		const fileToPlay = this.getNumberFile(value);
		console.log(`fileToPlay: ${fileToPlay}`);
		return this.safePlayMp3ReturnAudio(fileToPlay);
	}

	private getNumberFile(value: number) {
		let numMp3Files = 3;
		if (value > 30)
			numMp3Files = 1;

		let suffix: string;
		const suffixMap = 'abc';

		if (numMp3Files === 1)
			suffix = '';
		else
			suffix = suffixMap[Random.intMax(numMp3Files)];

		const fileToPlay = `Announcer/Numbers/${value}${suffix}`;
		return fileToPlay;
	}

	playFlatD20Commentary(d20RollValue: number): void {
	}
	playWildMagicD20CheckCommentary(d20RollValue: number): void {
	}
	playExtraCommentary(type: DiceRollType, d20RollValue: number): void {
	}

	playHealthCommentary(type: DiceRollType, totalHealthPlusModifier: number): void {
		const audio: HTMLAudioElement = diceSounds.playNumber(totalHealthPlusModifier);
		// TODO: Play "Health"
		//audio.addEventListener('loadedmetadata', () => {
		//	diceSounds.playDamage(damageType, audio.duration * 1000);
		//}, false);
	}

	playDamageCommentary(totalDamage: number, damageType: DamageType): void {
		this.playNumberThen(totalDamage, this.getDamageSoundFileName(damageType));
	}

	private playNumberThen(value: number, suffixSoundFile: string) {
		const soundFileTailTrimMs = -600;
		const asyncAudioPlayer: AsyncAudioPlayer = new AsyncAudioPlayer();

		if (value > 100) {
			const numHundreds: number = Math.floor(value / 100);
			asyncAudioPlayer.play(this, this.getNumberFile(numHundreds * 100), soundFileTailTrimMs)
				.then(() => asyncAudioPlayer.play(this, this.getNumberFile(value - numHundreds * 100), soundFileTailTrimMs)
					.then(() => asyncAudioPlayer.play(this, suffixSoundFile, soundFileTailTrimMs))
				);
		}
		else {
			asyncAudioPlayer.play(this, this.getNumberFile(value), soundFileTailTrimMs)
				.then(() => asyncAudioPlayer.play(this, suffixSoundFile, soundFileTailTrimMs));
		}
	}

	playDamage(damageType: DamageType, delayMs: number) {
		//console.log('delayMs: ' + delayMs);
		const fileName = this.getDamageSoundFileName(damageType);
		//console.log('fileName: ' + fileName);
		this.playMp3In(delayMs, fileName);
	}

	private getDamageSoundFileName(damageType: DamageType) {
		return `Announcer/Damage/${DamageType[damageType]}[3]`;
	}

	playPercentageRollCommentary(type: DiceRollType, d20RollValue: number): void {
	}
	playWildMagicCommentary(type: DiceRollType, d20RollValue: number): void {
	}
	playMultiplayerCommentary(type: DiceRollType, d20RollValue: number): void {
	}
	playChaosBoltCommentary(d20RollValue: number, savingThrow: Ability): void {
	}
	playSavingThrowCommentary(d20RollValue: number, savingThrow: Ability): void {
	}
	playSkillCheckCommentary(d20RollValue: number, skillCheck: Skills): void {
	}

	playOpenDiePortal() {
		this.safePlayMp3('DiePortal');
	}

	playSteampunkTunnel() {
		this.safePlayMp3('SteampunkTunnel');
	}

	playFireball() {
		this.safePlayMp3('Fireball2');
	}

	playHandGrab(): any {
		this.safePlayMp3('HandGrabsDice');
	}
	playDiceBlow() {
		this.safePlayMp3('DiceBlow');
	}

	playWildMagic(fileName: string, count: number = 0): void {
		if (count == 0)
			this.safePlayMp3('WildMagic/' + fileName);
		else
			this.playRandom('WildMagic/' + fileName, count);
	}

	playDieBomb() {
		const numDieBombSounds: number = 2;
		var index: number = Math.floor(Math.random() * numDieBombSounds) + 1;
		this.safePlayMp3(`DieBomb${index}`);
	}


	playDieBurst() {
		const numDieBurstSounds: number = 5;
		var index: number = Math.floor(Math.random() * numDieBurstSounds) + 1;
		this.safePlayMp3(`DieBurst${index}`);
	}

	playSmokyPortal() {
		this.safePlayMp3('Dice/SmokeyPortal/ElectricPortal[10]', 200);
		this.safePlayMp3('Dice/SmokeyPortal/Intro[3]', 200);
		this.safePlayMp3('Dice/SmokeyPortal/Outro[3]', 200);
	}

	playWallHit(percentage: number): void {
		this.play(this.getPercentageAudio(this.hitWall, percentage));
	}

	playFloorHit(percentage: number): void {
		this.play(this.getPercentageAudio(this.hitFloor, percentage));
	}

	playSettle(): any {
		if (this.lastSettleTime) {
			var now: Date = new Date();
			if (now.valueOf() - this.lastSettleTime.valueOf() < 500)
				return;
		}

		this.play(this.getPercentageAudio(this.settles, Math.random()));
		this.lastSettleTime = new Date();
	}

	playDiceHit(percentage: number): any {
		this.play(this.getPercentageAudio(this.hitDice, percentage));
	}
}