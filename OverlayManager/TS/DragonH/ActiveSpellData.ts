class ActiveSpellData {
	name: string;
	spellSlotLevel: number;
	spellLevel: number;
	playerLevel: number;
	description: string;
	castingTimeStr: string;
	rangeStr: string;
	componentsStr: string;
	durationStr: string;
	schoolOfMagic: SchoolOfMagic;
	requiresConcentration: boolean;
	morePowerfulAtHigherLevels: boolean;
	powerComesFromCasterLevel: boolean;

	constructor(fromDto: any) {
		if (fromDto) {
			this.name = fromDto.name;
			this.spellSlotLevel = fromDto.spellSlotLevel;
			this.spellLevel = fromDto.spellLevel;
			this.playerLevel = fromDto.playerLevel;
			this.description = fromDto.description;
			this.castingTimeStr = fromDto.castingTimeStr;
			this.rangeStr = fromDto.rangeStr;
			this.componentsStr = fromDto.componentsStr;
			this.durationStr = fromDto.durationStr;
			this.schoolOfMagic = fromDto.schoolOfMagic;
			this.requiresConcentration = fromDto.requiresConcentration;
			this.morePowerfulAtHigherLevels = fromDto.morePowerfulAtHigherLevels;
			this.powerComesFromCasterLevel = fromDto.powerComesFromCasterLevel; 
		}
	}
}