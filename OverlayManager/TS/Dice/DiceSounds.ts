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

	playAttackCommentary(d20RollValue: number, totalDamage: number, maxDamage: number): any {
		if (attemptedRollWasSuccessful) {
			if (d20RollValue >= diceRollData.minCrit) {
				diceSounds.playRandom('Announcer/CriticalHit', 21);
			}
			else {
				if (maxDamage > 0)
					if (totalDamage == maxDamage)
						diceSounds.playRandom('Announcer/MaxDamage', 12);
					else if (totalDamage / maxDamage > 0.7 && totalDamage > 7)
						diceSounds.playRandom('Announcer/LotsOfDamage', 24);
					else if (totalDamage / maxDamage < 0.3)
						diceSounds.playRandom('Announcer/NotMuchDamage', 22);
					else
						diceSounds.playRandom('Announcer/Hit', 25);
				else {
					// TODO: add gender hits.
					diceSounds.playRandom('Announcer/Hit', 25);
				}
			}
		}
		else if (d20RollValue == 1) {
			diceSounds.playRandom('Announcer/SpectacularMiss', 9);
		}
		else {
			if (maxDamage > 0)
				if (totalDamage == maxDamage)
					diceSounds.playRandom('Announcer/MaxDamageMiss', 0);
				else if (totalDamage / maxDamage > 0.7)
					diceSounds.playRandom('Announcer/LotsOfDamageMiss', 0);
				else
					diceSounds.playRandom('Announcer/Miss', 0);
			else {
				// TODO: add gender misses.
				diceSounds.playRandom('Announcer/Miss', 0);
			}
		}
	}

	playFlatD20Commentary(d20RollValue: number): void {
	}
	playWildMagicD20CheckCommentary(d20RollValue: number): void {
	}
	playExtraCommentary(type: DiceRollType, d20RollValue: number): void {
	}
	playHealthCommentary(type: DiceRollType, d20RollValue: number): void {
	}
	playDamageCommentary(type: DiceRollType, d20RollValue: number, totalDamage: number, maxDamage: number): void {
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
		const numDieBurstSounds: number = 2;
		var index: number = Math.floor(Math.random() * numDieBurstSounds) + 1;
		this.safePlayMp3(`DieBurst${index}`);
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