var connection;

function addWindup(windupData: string): void {
	if (activeFrontGame instanceof DragonGame) {
		activeFrontGame.addWindupFromStr(windupData);
	}
	if (activeBackGame instanceof DragonGame) {
		activeBackGame.addWindupFromStr(windupData);
	}
}

function castSpell(spellData: string): void {
	if (activeFrontGame instanceof DragonGame) {
		activeFrontGame.castSpell(spellData);
	}
	if (activeBackGame instanceof DragonGame) {
		activeBackGame.castSpell(spellData);
	}
}

function clearWindup(windupName: string): void {
	let fadeOut = true;
	if (windupName.endsWith('!')) {
		fadeOut = false;
		windupName = windupName.substr(0, windupName.length - 1);
	}

	if (activeFrontGame instanceof DragonGame) {
		activeFrontGame.clearWindup(windupName, fadeOut);
	}
	if (activeBackGame instanceof DragonGame) {
		activeBackGame.clearWindup(windupName, fadeOut);
	}
}

function moveFred(movement: string): void {
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.moveFred(movement);
	}
}

function animateSprinkles(commandData: string) {
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.animateSprinkles(commandData);
	}
}

function updateInGameCreatures(commandData: string) {
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.updateInGameCreatures(commandData);
	}
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.updateInGameCreatures(commandData);
	}
}

function triggerEffect(effectData: string) {
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.triggerEffect(effectData);
	}
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.triggerEffect(effectData);
	}
}

function updateClock(clockData: string) {
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.updateClock(clockData);
	}
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.updateClock(clockData);
	}
}

function floatPlayerText(playerId: number, message: string, fillColor: string, outlineColor: string) {
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.floatPlayerText(playerId, message, fillColor, outlineColor);
	}
}

function playSound(soundFileName: string) {
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.playSound(soundFileName);
	}
}

let bigPlayerData = "";

const bigDataPortionToken = "portion: ";

function showImageFront(fileName: string) {
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.showImage(fileName);
	}
}

function showImageBack(fileName: string) {
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.showImage(fileName);
	}
}

function preloadImageFront(baseFileName: string, startIndex: number, stopIndex: number, digitCount: number) {
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.preloadImage(baseFileName, startIndex, stopIndex, digitCount);
	}
}

function preloadImageBack(baseFileName: string, startIndex: number, stopIndex: number, digitCount: number) {
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.preloadImage(baseFileName, startIndex, stopIndex, digitCount);
	}
}

function initializePlayerData(playerData: string) {
	if (playerData.startsWith(bigDataPortionToken)) {
		bigPlayerData += playerData.substr(bigDataPortionToken.length);
		return;
	}
	const rebuiltPlayerData: string = bigPlayerData + playerData;
	bigPlayerData = "";
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.initializePlayerData(rebuiltPlayerData);
	}
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.initializePlayerData(rebuiltPlayerData);
	}
	if (diceLayer) {
		diceLayer.initializePlayerData(rebuiltPlayerData);
	}
}

function sendScrollLayerCommand(commandData: string) {
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.characterStatsScroll.sendScrollLayerCommand(commandData);
	}
}

function executeSoundCommand(commandData: string) {
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.executeSoundCommand(commandData);
	}
}

function showValidationIssue(commandData: string) {
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.showValidationIssue(commandData);
	}
}

function clearDice(diceGroup: string) {
	if (diceLayer) {
		diceLayer.clearDice(diceGroup);
	}
}

function rollDice(diceRollData: string) {
	if (diceLayer) {
		diceLayer.rollDice(diceRollData);
	}
}

class UserInfo {
	constructor(public userId: string, public userName: string, public displayName: string, public color: string, public showsWatched: number) {

	}
}
function controlSpaceship(command: string, data: string, userId: string, userName: string, displayName: string, color: string, showsWatched: number) {
	let userInfo: UserInfo = new UserInfo(userId, userName, displayName, color, showsWatched);
	if (activeMarkFliesGame) {
		activeMarkFliesGame.controlSpaceship(command, data, userInfo, activeMarkFliesGame.nowMs);
	}
}

function executeCommand(command: string, params: string, userId: string, userName: string, displayName: string, color: string, showsWatched: number) {
	let userInfo: UserInfo = new UserInfo(userId, userName, displayName, color, showsWatched);
	if (activeBackGame) {
		activeBackGame.executeCommand(command, params, userInfo, activeBackGame.nowMs);
	}
	if (activeMarkFliesGame) {
		activeMarkFliesGame.executeCommand(command, params, userInfo, activeMarkFliesGame.nowMs);
	}
	if (activeFrontGame) {
		activeFrontGame.executeCommand(command, params, userInfo, activeFrontGame.nowMs);
	}
	if (activeDroneGame) {
		activeDroneGame.executeCommand(command, params, userInfo, activeDroneGame.nowMs);
	}
}
function changePlayerHealth(playerHealth: string) {
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.changePlayerHealth(playerHealth);
	}

	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.changePlayerHealth(playerHealth);
	}
}
function changePlayerStats(playerStats: string) {
	//console.log('changePlayerStats from Connection.ts');
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.changePlayerStats(playerStats);
	}
}

function changePlayerWealth(playerWealth: string) {
	//console.log('changePlayerWealth from Connection.ts');
	//if (activeBackGame instanceof DragonBackGame) {
	//	activeBackGame.changePlayerWealth(playerWealth);
	// }

	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.changePlayerWealth(playerWealth);
	}
}

function inGameUICommand(commandData: string) {
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.inGameUICommand(commandData);
	}
}

function changeFrameRate(frameRateData: string) {
	let frameRateChangeData: FrameRateChangeData = JSON.parse(frameRateData);
	//frameRateData

	ColorShiftingSpriteProxy.globalAllowColorShifting = frameRateChangeData.AllowColorShifting;
	DiceLayer.maxFiltersOnDieCleanup = frameRateChangeData.MaxFiltersOnDieCleanup;
	DiceLayer.maxFiltersOnRoll = frameRateChangeData.MaxFiltersOnRoll;
	DragonGame.maxFiltersPerWindup = frameRateChangeData.MaxFiltersPerWindup;
	//ColorShiftingSpriteProxy.globalAllowCanvasFilterCaching = frameRateChangeData.AllowCanvasFilterCaching;

	//let message: string = `${frameRateChangeData.OverlayName} overlay: ${frameRateChangeData.FrameRate} fps `;
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.showFpsWindow = frameRateChangeData.ShowFpsWindow;
		if (frameRateChangeData.OverlayName === 'Back') {
			activeBackGame.handleFpsChange(frameRateChangeData);
		}
	}

	if (diceLayer) {
		diceRollerShowFpsWindow = frameRateChangeData.ShowFpsWindow;
		if (frameRateChangeData.OverlayName === 'Dice') {
			if (diceRollerPlayers)
				diceRollerPlayers.handleFpsChangeDiceRoller(frameRateChangeData);
			if (diceRollerViewers)
				diceRollerViewers.handleFpsChangeDiceRoller(frameRateChangeData);
		}
	}

	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.showFpsWindow = frameRateChangeData.ShowFpsWindow;
		if (frameRateChangeData.OverlayName === 'Front') {
			activeFrontGame.handleFpsChange(frameRateChangeData);
		}
		//activeFrontGame.showFpsMessage(message);
	}
}

function focusItem(playerID: number, pageID: number, itemID: string) {
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.characterStatsScroll.focusItem(playerID, pageID, itemID);
	}
}

function unfocusItem(playerID: number, pageID: number, itemID: string) {
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.characterStatsScroll.unfocusItem(playerID, pageID, itemID);
	}
}

function dmDataChanged(dmData: string) {
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.dmDataChanged(dmData);
	}
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.dmDataChanged(dmData);
	}
}

function calibrateLeapMotion(calibrationData: string) {
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.calibrateLeapMotion(calibrationData);
	}
}

function updateSkeletalData(skeletalData: string) {
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.updateSkeletalData(skeletalData);
	}
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.updateSkeletalData(skeletalData);
	}
}

function speechBubble(speechStr: string) {
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.speechBubble(speechStr);
	}
}function showBook(title: string) {
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.showBook(title);
	}
}

function cardCommand(cardStr: string) {
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.cardCommand(cardStr);
	}
}

function contestCommand(contestStr: string) {
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.contestCommand(contestStr);
	}
}


function playerDataChanged(playerID: number, pageID: number, playerData: string) {
	if (activeBackGame instanceof DragonGame) {
		activeBackGame.playerChanged(playerID, pageID, playerData);
	}
	if (activeFrontGame instanceof DragonGame) {
		activeFrontGame.playerChanged(playerID, pageID, playerData);
	}
	if (diceLayer) {
		diceLayer.playerChanged(playerID);
	}
}

function userHasCoins(userId: string, amount: number) {
	if (activeDroneGame instanceof DroneGame) {
		const userDrone: Drone = activeDroneGame.allDrones.find(userId) as Drone;
		if (userDrone)
			userDrone.coinCount += amount;
	}
}
function suppressVolume(seconds: number) {
	if (activeDroneGame instanceof DroneGame) {
		Boombox.suppressVolume(seconds, performance.now());
	}
}

function chat(message: string) {
	connection.invoke("Chat", message);
}

function whisper(userName: string, message: string) {
	connection.invoke("Whisper", userName, message);
}

function needToGetCoins(userId: string) {
	connection.invoke("NeedToGetCoins", userId);
}

function diceHaveStoppedRolling(diceData: string) {
	if (connection.connectionState === 1) {
		//console.error('connection.invoke("DiceHaveStoppedRolling"...');
		connection.invoke("DiceHaveStoppedRolling", diceData);
	}
}

function updateVideoFeed(videoFeedData: string) {
	if (connection.connectionState === 1) {
		//console.log(videoFeedData);
		connection.invoke("UpdateVideoFeed", videoFeedData);
	}
}

function allDiceHaveBeenDestroyed(diceData: string) {
	if (connection.connectionState === 1)
		connection.invoke("AllDiceHaveBeenDestroyed", diceData);
}

function inGameUIResponse(response: string) {
	if (connection.connectionState === 1)
		connection.invoke("InGameUIResponse", response);
}

function arm(userId: string) {
	connection.invoke("Arm", userId);
}

function disarm(userId: string) {
	connection.invoke("Disarm", userId);
}

function fire(userId: string) {
	connection.invoke("Fire", userId);
}

function tellDM(message: string) {
	connection.invoke("TellDM", message);
}

function connectToSignalR(signalR) {
	console.log('connecting to signalR...');
	connection = new signalR.HubConnectionBuilder().withUrl("/CodeRushedHub").configureLogging(signalR.LogLevel.Information).build();
	connection.serverTimeoutInMilliseconds = 1000000; // 1000 second
	window.onload = function () {
		console.log('signalR loaded...');
		connection.start().catch(err => console.error(err.toString()));
		connection.on("ExecuteCommand", executeCommand);
		connection.on("ControlSpaceship", controlSpaceship);
		connection.on("ChangePlayerHealth", changePlayerHealth);
		connection.on("ChangePlayerStats", changePlayerStats);
		connection.on("changePlayerWealth", changePlayerWealth);
		connection.on("ChangeFrameRate", changeFrameRate);
		connection.on("InGameUICommand", inGameUICommand);
		connection.on("UserHasCoins", userHasCoins);
		connection.on("SuppressVolume", suppressVolume);
		connection.on("FocusItem", focusItem);
		connection.on("UnfocusItem", unfocusItem);
		connection.on("AddWindup", addWindup);
		connection.on("CastSpell", castSpell);
		connection.on("ClearWindup", clearWindup);
		connection.on("MoveFred", moveFred);
		connection.on("TriggerEffect", triggerEffect);
		connection.on("PlaySound", playSound);
		connection.on("AnimateSprinkles", animateSprinkles);
		connection.on("UpdateInGameCreatures", updateInGameCreatures);
		connection.on("UpdateClock", updateClock);
		connection.on("FloatPlayerText", floatPlayerText);
		connection.on("RollDice", rollDice);
		connection.on("ClearDice", clearDice);
		connection.on("SendScrollLayerCommand", sendScrollLayerCommand);
		connection.on("ExecuteSoundCommand", executeSoundCommand);
		connection.on("ShowValidationIssue", showValidationIssue);
		connection.on("PlayerDataChanged", playerDataChanged);
		connection.on("DmDataChanged", dmDataChanged);
		connection.on("CalibrateLeapMotion", calibrateLeapMotion);
		connection.on("UpdateSkeletalData", updateSkeletalData);
		connection.on("SpeechBubble", speechBubble);
		connection.on("ShowBook", showBook);
		connection.on("CardCommand", cardCommand);
		connection.on("ContestCommand", contestCommand);
		connection.on("SetPlayerData", initializePlayerData);
		connection.on("ShowImageFront", showImageFront);
		connection.on("ShowImageBack", showImageBack);
		connection.on("PreloadImageFront", preloadImageFront);
		connection.on("PreloadImageBack", preloadImageBack);
		console.log('PartBackgroundLoader.initialize();');
		PartBackgroundLoader.okayToStartLoading = true;
		PartBackgroundLoader.initialize();
	};
}