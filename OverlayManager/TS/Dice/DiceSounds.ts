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
			audio.addEventListener('error', () => {
				console.log(`Sound file not found: ${fileName}`);
				resolve();
			});
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

	async playAttackCommentaryAsync(d20RollValue: number, d20Modifier: number, totalDamage: number, maxDamage: number, minCrit: number, attemptedRollWasSuccessful: boolean): Promise<void> {
		await this.playD20PlusModifierAsync(d20RollValue, d20Modifier);

		if (attemptedRollWasSuccessful) {
			await this.playSuccessfulAttackCommentaryAsync(d20RollValue, maxDamage, totalDamage, minCrit);
		}
		else {
			await this.playFailedAttackCommentaryAsync(d20RollValue, maxDamage, totalDamage);
		}
	}

	private async playD20PlusModifierAsync(d20RollValue: number, d20Modifier: number) {
		await this.playNumberAsync(d20RollValue);

		if (d20Modifier !== 0) {
			if (d20Modifier > 0)
				await this.playSoundFileAsync('Announcer/Numbers/Plus[3]');
			else
				await this.playSoundFileAsync('Announcer/Numbers/Minus[3]');

			await this.playNumberAsync(Math.abs(d20Modifier));
		}
	}

	private async playFailedAttackCommentaryAsync(d20RollValue: number, maxDamage: number, totalDamage: number): Promise<void> {
		if (d20RollValue === 1) {
			await this.playSoundFileAsync('Announcer/SpectacularMiss[9]');
		}
		else {
			if (maxDamage > 0)
				if (totalDamage === maxDamage)
					await this.playSoundFileAsync('Announcer/MaxDamageMiss[0]');
				else if (totalDamage / maxDamage > 0.7)
					await this.playSoundFileAsync('Announcer/LotsOfDamageMiss[0]');
				else
					await this.playSoundFileAsync('Announcer/Miss[0]');
			else {
				// TODO: add gender misses.
				await this.playSoundFileAsync('Announcer/Miss[0]');
			}
		}
	}

	private async playSuccessfulAttackCommentaryAsync(d20RollValue: number, maxDamage: number, totalDamage: number, minCrit: number): Promise<void> {
		if (d20RollValue >= minCrit) {
			await this.playSoundFileAsync('Announcer/CriticalHit[21]');
		}
		else {
			if (maxDamage > 0) {
				await this.playAttackCommentaryWithMaxDamageAsync(totalDamage, maxDamage);
			}
			else {
				// TODO: add gender hits.
				await this.playSoundFileAsync('Announcer/Hit[25]');
			}
		}
	}

	private async playAttackCommentaryWithMaxDamageAsync(totalDamage: number, maxDamage: number) {
		// TODO: Move likelyMorningCodeRushedShow to a global function.
		const time: Date = new Date();
		const likelyMorningCodeRushedShow: boolean = time.getHours() < 16;

		if (likelyMorningCodeRushedShow)
			await this.playSoundFileAsync('Announcer/Hit[25]');
		else if (totalDamage === maxDamage)
			await this.playSoundFileAsync('Announcer/MaxDamage[12]');
		else if (totalDamage / maxDamage > 0.7 && totalDamage > 7)
			await this.playSoundFileAsync('Announcer/LotsOfDamage[24]');
		else if (totalDamage / maxDamage < 0.3)
			await this.playSoundFileAsync('Announcer/NotMuchDamage[22]');
		else
			await this.playSoundFileAsync('Announcer/Hit[25]');
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
		this.playNumberAsync(d20RollValue);
	}

	async playWildMagicD20CheckCommentary(d20RollValue: number): Promise<void> {
		if (d20RollValue !== 1)
			return;
		await this.playSoundFileAsync('Announcer/Reactions/OhNo[5]');
		await this.playSoundFileAsync(this.getNumberFile(1));
	}

	playExtraCommentary(type: DiceRollType, d20RollValue: number): void {
	}

	async playHealthCommentary(type: DiceRollType, totalHealthPlusModifier: number): Promise<void> {
		await this.playNumberAsync(totalHealthPlusModifier);
		await this.playSoundFileAsync('Announcer/Damage/Health[3]');
	}

	async playAttackPlusDamageCommentaryAsync(d20RollValue: number, d20Modifier: number, totalDamage: number, maxDamage: number, damageType: DamageType, damageSummary: Map<DamageType, number>, minCrit: number, attemptedRollWasSuccessful: boolean): Promise<void> {
		await this.playAttackCommentaryAsync(d20RollValue, d20Modifier, totalDamage, maxDamage, minCrit, attemptedRollWasSuccessful);
		if (attemptedRollWasSuccessful && ((damageSummary && damageSummary.size > 0) || totalDamage > 0))
		{
			await this.playSoundFileAsync('Announcer/Numbers/With[4]');
			await this.playDamageCommentaryAsync(totalDamage, damageType, damageSummary);
		}
	}

	async playDamageCommentaryAsync(totalDamage: number, damageType: DamageType, damageSummary: Map<DamageType, number> = null): Promise<void> {
		if (damageSummary) {
			const numDamages: number = damageSummary.size;
			let damageCount = 0;
			for (const key of damageSummary.keys()) {
				damageCount++;
				await this.playIndividualDamageCommentaryAsync(damageSummary.get(key), key);
				if (damageCount < numDamages)  // Separate different damage reports 
					await this.playSoundFileAsync('Announcer/Numbers/Plus[3]');
			}
		}
		else {
			await diceSounds.playIndividualDamageCommentaryAsync(totalDamage, damageType);
		}
	}

	async delay(delayMs: number): Promise<void> {
		return new Promise(function (resolve) {
			setTimeout(resolve, delayMs);
		});
	}

	async playIndividualDamageCommentaryAsync(totalDamage: number, damageType: DamageType): Promise<void> {
		await this.playNumberAsync(totalDamage);
		await this.playSoundFileAsync(this.getDamageSoundFileName(damageType));
	}

	static readonly soundFileTailTrimMs: number = -600;

	async playSoundFileAsync(suffixSoundFile: string) {
		const asyncAudioPlayer: AsyncAudioPlayer = new AsyncAudioPlayer();
		await asyncAudioPlayer.play(this, suffixSoundFile, DiceSounds.soundFileTailTrimMs);
	}

	private async playNumberAsync(value: number) {
		if (isNaN(value) || value === undefined) {
			console.error(`value is NaN.`);
			return;
		}
		const asyncAudioPlayer: AsyncAudioPlayer = new AsyncAudioPlayer();

		if (value > 100) {
			const numHundreds: number = Math.floor(value / 100);
			await asyncAudioPlayer.play(this, this.getNumberFile(numHundreds * 100), DiceSounds.soundFileTailTrimMs);
			const tens: number = value - numHundreds * 100;
			if (tens !== 0)
				await asyncAudioPlayer.play(this, this.getNumberFile(tens), DiceSounds.soundFileTailTrimMs);
		}
		else {
			await asyncAudioPlayer.play(this, this.getNumberFile(value), DiceSounds.soundFileTailTrimMs);
		}
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

	async playSavingThrowCommentary(d20RollValue: number, d20Modifier: number, savingThrow: Ability, attemptedRollWasSuccessful: boolean): Promise<void> {

		await this.playD20PlusModifierAsync(d20RollValue, d20Modifier);
		if (attemptedRollWasSuccessful)
			await this.playSoundFileAsync('Announcer/Reactions/Success[4]');
		else
			await this.playSoundFileAsync('Announcer/Reactions/Failure[3]');
	}

	async playSkillCheckCommentary(d20RollValue: number, d20Modifier: number, skillCheck: Skills, attemptedRollWasSuccessful: boolean): Promise<void> {
		await this.playD20PlusModifierAsync(d20RollValue, d20Modifier);
		if (attemptedRollWasSuccessful)
			await this.playSoundFileAsync('Announcer/Reactions/Success[4]');
		else 
			await this.playSoundFileAsync('Announcer/Reactions/Failure[3]');
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