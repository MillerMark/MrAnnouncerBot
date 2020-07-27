class InGameCreature {
	static readonly ImageWidth = 104;
	static readonly ImageHeight = 90;
	Name: string;
	Alignment: string;
	ImageURL: string;
	Kind: string;
	Index: number;
	CropX: number;
	CropY: number;
	CropWidth: number;
	Health: number;
	IsTargeted: boolean;
	PercentDamageJustInflicted: number;
	PercentHealthJustGiven: number;
	IsEnemy: boolean;
	imageLoaded: boolean;
	image: HTMLImageElement;
	removing = false;
	justAdded: boolean;

	setImageUrl(imageURL: string) {
		this.imageLoaded = false;
		this.ImageURL = imageURL;
		this.image = new Image();
		this.image.src = this.ImageURL;
		this.image.onload = function () {
			this.imageLoaded = true;
		}.bind(this);
	}

	constructor(inGameCreature: InGameCreature) {
		this.Name = inGameCreature.Name;
		this.Alignment = inGameCreature.Alignment;
		this.Kind = inGameCreature.Kind;
		this.Index = inGameCreature.Index;
		this.CropX = inGameCreature.CropX;
		this.CropY = inGameCreature.CropY;
		this.CropWidth = inGameCreature.CropWidth;
		this.Health = inGameCreature.Health;
		this.IsTargeted = inGameCreature.IsTargeted;
		this.IsEnemy = inGameCreature.IsEnemy;
		this.PercentDamageJustInflicted = inGameCreature.PercentDamageJustInflicted;
		this.PercentHealthJustGiven = inGameCreature.PercentHealthJustGiven;
		this.setImageUrl(inGameCreature.ImageURL);
	}
}

