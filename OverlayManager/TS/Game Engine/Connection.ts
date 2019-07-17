var connection;

function connectToSignalR(signalR) {
	connection = new signalR.HubConnectionBuilder().withUrl("/CodeRushedHub").configureLogging(signalR.LogLevel.Information).build();
	connection.serverTimeoutInMilliseconds = 1000000; // 1000 second
  window.onload = function () {
    connection.start().catch(err => console.error(err.toString()));
    connection.on("ExecuteCommand", executeCommand);
    connection.on("UserHasCoins", userHasCoins);
		connection.on("SuppressVolume", suppressVolume);
    connection.on("PlayerDataChanged", playerDataChanged);
    connection.on("FocusItem", focusItem);
    connection.on("UnfocusItem", unfocusItem);
    connection.on("TriggerEffect", triggerEffect);
    connection.on("UpdateClock", updateClock);
    connection.on("RollDice", rollDice);
    connection.on("ClearDice", clearDice);
		connection.on("SetPlayerData", setPlayerData);
		connection.on("SendScrollLayerCommand", sendScrollLayerCommand);
  };
}

function triggerEffect(effectData: string) {
  if (activeFrontGame instanceof DragonFrontGame) {
    activeFrontGame.triggerEffect(effectData);
  }
}
function updateClock(clockData: string) {
  if (activeFrontGame instanceof DragonFrontGame) {
    activeFrontGame.updateClock(clockData);
  }
}

function setPlayerData(playerData: string) {
	if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.setPlayerData(playerData);
	}
	if (activeBackGame instanceof DragonBackGame) {
		activeBackGame.characterStatsScroll.setPlayerData(playerData);
	}
	if (diceLayer) {
		diceLayer.setPlayerData(playerData);
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

function executeCommand(command: string, params: string, userId: string, userName: string, displayName: string, color: string) {
  console.log('executeCommand from Connection.ts');
  if (activeBackGame) {
    activeBackGame.executeCommand(command, params, userId, userName, displayName, color, activeBackGame.nowMs);
  }
  if (activeFrontGame) {
    activeFrontGame.executeCommand(command, params, userId, userName, displayName, color, activeFrontGame.nowMs);
  }
  if (activeDroneGame) {
    activeDroneGame.executeCommand(command, params, userId, userName, displayName, color, activeDroneGame.nowMs);
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

function playerDataChanged(playerID: number, pageID: number, playerData: string) {
	if (activeBackGame instanceof DragonBackGame) {
    activeBackGame.playerDataChanged(playerID, pageID, playerData);
  }
  if (activeFrontGame instanceof DragonFrontGame) {
		activeFrontGame.playerChanged(playerID, playerData);
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

function arm(userId: string) {
  connection.invoke("Arm", userId);
}

function disarm(userId: string) {
  connection.invoke("Disarm", userId);
}

function fire(userId: string) {
  connection.invoke("Fire", userId);
}