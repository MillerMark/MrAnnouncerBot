abstract class DragonGame extends GamePlusQuiz {
	abstract layerSuffix: string;
	spellGhost: Sprites;
	spellFairy: Sprites;
	spellPlasma: Sprites;
	spellSmoke: Sprites;

  spellEffects: SpriteCollection;

	loadSpell(spellName: string): Sprites {
		let spell: Sprites = new Sprites(`PlayerEffects/Spells/${spellName}/${spellName}${this.layerSuffix}`, 60, fps30, AnimationStyle.Loop, true);
		spell.name = spellName;
		spell.originX = 439;
		spell.originY = 232;
		this.spellEffects.add(spell);
		return spell;
	}

	constructor(context: CanvasRenderingContext2D) {
		super(context);
		this.spellEffects = new SpriteCollection();
	}

	clearWindup(windupName: string): void {
		this.spellEffects.allSprites.forEach(function (sprites: Sprites) { sprites.sprites = [] });
	}

	addWindup(windupData: string): void {
		let sprites: Sprites = this.spellEffects.getSpritesByName(windupData);
		let startingAngle: number = 0;
		let startingFrameIndex: number = startingAngle / 6 % 360;
		let hueShift: number = Random.max(360);
		if (sprites) {
			let sprite: SpriteProxy = sprites.addShifted(1125, 934, startingFrameIndex, hueShift, 100, 100);
			sprite.fadeInTime = 400;
		}
	}

	updateScreen(context: CanvasRenderingContext2D, now: number) {
		this.spellEffects.draw(context, now);
	}

	initialize() {
		let saveAssets: string = Folders.assets;
		super.initialize();
		Folders.assets = 'GameDev/Assets/DragonH/';
		this.spellGhost = this.loadSpell('Ghost');
		this.spellFairy = this.loadSpell('Fairy');
		this.spellPlasma = this.loadSpell('Plasma');
		this.spellSmoke = this.loadSpell('Smoke');
		this.spellSmoke = this.loadSpell('Fire');
		this.spellSmoke = this.loadSpell('LiquidSparks');
		this.spellSmoke = this.loadSpell('Narrow');
		this.spellSmoke = this.loadSpell('Orb');
		this.spellSmoke = this.loadSpell('Trails');
		this.spellSmoke = this.loadSpell('Wide');
		Folders.assets = saveAssets;
	}
}
