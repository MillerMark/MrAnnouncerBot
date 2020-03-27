var connection;

function connectToSignalR(signalR) {
	console.log('connecting to signalR...');
	connection = new signalR.HubConnectionBuilder().withUrl("/CodeRushedHub").configureLogging(signalR.LogLevel.Information).build();
	connection.serverTimeoutInMilliseconds = 1000000; // 1000 second
	window.onload = function () {
		console.log('signalR loaded...');
		connection.start().catch(err => console.error(err.toString()));
    connection.on("ExecuteCommand", executeCommand);
		connection.on("ChangePlayerHealth", changePlayerHealth);
		connection.on("changePlayerWealth", changePlayerWealth);
    connection.on("UserHasCoins", userHasCoins);
		connection.on("SuppressVolume", suppressVolume);
    connection.on("FocusItem", focusItem);
    connection.on("UnfocusItem", unfocusItem);
    connection.on("AddWindup", addWindup);
    connection.on("CastSpell", castSpell);
    connection.on("ClearWindup", clearWindup);
		connection.on("MoveFred", moveFred);
		connection.on("TriggerEffect", triggerEffect);
		connection.on("AnimateSprinkles", animateSprinkles);
    connection.on("UpdateClock", updateClock);
    connection.on("RollDice", rollDice);
    connection.on("ClearDice", clearDice);
		connection.on("SendScrollLayerCommand", sendScrollLayerCommand);
		connection.on("PlayerDataChanged", playerDataChanged);
		connection.on("MapDataChanged", mapDataChanged);
		connection.on("SetPlayerData", initializePlayerData);
		console.log('PartBackgroundLoader.initialize();');
		PartBackgroundLoader.okayToStartLoading = true;
		PartBackgroundLoader.initialize();
  };
}

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
	if (activeFrontGame instanceof DragonGame) {
		activeFrontGame.clearWindup(windupName);
	}
	if (activeBackGame instanceof DragonGame) {
		activeBackGame.clearWindup(windupName);
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

function initializePlayerData(playerData: string) {
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.initializePlayerData(playerData);
	}
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.initializePlayerData(playerData);
	}
	if (diceLayer) {
		diceLayer.initializePlayerData(playerData);
	}
}

function sendScrollLayerCommand(commandData: string) {
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.characterStatsScroll.sendScrollLayerCommand(commandData);
	}
}

function clearDice() {
  if (diceLayer) {
    diceLayer.clearDice();
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

function executeCommand(command: string, params: string, userInfo: UserInfo) {
  console.log('executeCommand from Connection.ts');
  if (activeBackGame) {
		activeBackGame.executeCommand(command, params, userInfo, activeBackGame.nowMs);
  }
  if (activeFrontGame) {
		activeFrontGame.executeCommand(command, params, userInfo, activeFrontGame.nowMs);
  }
  if (activeDroneGame) {
		activeDroneGame.executeCommand(command, params, userInfo, activeDroneGame.nowMs);
  }
}
function changePlayerHealth(playerHealth: string) {
  console.log('changePlayerHealth from Connection.ts');
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.changePlayerHealth(playerHealth);
  }
	
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.changePlayerHealth(playerHealth);
  }
}

function changePlayerWealth(playerWealth: string) {
  console.log('changePlayerWealth from Connection.ts');
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.changePlayerWealth(playerWealth);
  }
	
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.changePlayerWealth(playerWealth);
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

function mapDataChanged(mapData: string) {
	
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
    let userDrone: Drone = <Drone>activeDroneGame.allDrones.find(userId);
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
  if (connection.connectionState == 1)
    connection.invoke("DiceHaveStoppedRolling", diceData);
}

function allDiceHaveBeenDestroyed(diceData: string) {
  if (connection.connectionState == 1)
    connection.invoke("AllDiceHaveBeenDestroyed", diceData);
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