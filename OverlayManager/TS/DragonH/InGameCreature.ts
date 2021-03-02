class InGameCreature {
	static readonly ImageWidth = 104;
	static readonly ImageHeight = 90;
	Name: string;
	Alignment: string;
	ImageURL: string;
	ForegroundHex: string;
	BackgroundHex: string;
	Kind: string;
	Index: number;
	CropX: number;
	CropY: number;
	NumNames: number;
	NumAhems: number;
	CropWidth: number;
	Health: number;
	Conditions: Conditions;
	IsTargeted: boolean;
	TurnIsActive: boolean;
	PercentDamageJustInflicted: number;
	PercentHealthJustGiven: number;
	IsEnemy: boolean;
	IsSelected: boolean;
	IsAlly: boolean;
	FriendFoeStatusUnknown: boolean;
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
		this.BackgroundHex = inGameCreature.BackgroundHex;
		this.ForegroundHex = inGameCreature.ForegroundHex;
		this.Index = inGameCreature.Index;
		this.CropX = inGameCreature.CropX;
		this.CropY = inGameCreature.CropY;
		this.NumAhems = inGameCreature.NumAhems;
		this.NumNames = inGameCreature.NumNames;
		this.CropWidth = inGameCreature.CropWidth;
		this.Health = inGameCreature.Health;
		this.Conditions = inGameCreature.Conditions;
		this.IsTargeted = inGameCreature.IsTargeted;
		this.TurnIsActive = inGameCreature.TurnIsActive;
		this.IsEnemy = inGameCreature.IsEnemy;
		this.IsAlly = inGameCreature.IsAlly;
		this.FriendFoeStatusUnknown = inGameCreature.FriendFoeStatusUnknown;
		this.PercentDamageJustInflicted = inGameCreature.PercentDamageJustInflicted;
		this.PercentHealthJustGiven = inGameCreature.PercentHealthJustGiven;
		this.setImageUrl(inGameCreature.ImageURL);
	}
}

