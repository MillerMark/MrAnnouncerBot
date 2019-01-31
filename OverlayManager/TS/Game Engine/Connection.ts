var connection;

function connectToSignalR(signalR) {
  connection = new signalR.HubConnectionBuilder().withUrl("/CodeRushedHub").configureLogging(signalR.LogLevel.Information).build();
  window.onload = function () {
    connection.start().catch(err => console.error(err.toString()));
    connection.on("ExecuteCommand", executeCommand);
    connection.on("UserHasCoins", userHasCoins);
    connection.on("PlayerPageChanged", playerPageChanged);
    connection.on("FocusItem", focusItem);
    connection.on("UnfocusItem", unfocusItem);
    connection.on("TriggerEffect", triggerEffect);
  };
}

function triggerEffect(effectData: string) {
  console.log(effectData);
}

function executeCommand(command: string, params: string, userId: string, userName: string, displayName: string, color: string) {
  console.log('executeCommand from Connection.ts');
  if (activeBackGame) {
    activeBackGame.executeCommand(command, params, userId, userName, displayName, color, activeBackGame.nowMs);
  }
  if (activeFrontGame) {
    activeFrontGame.executeCommand(command, params, userId, userName, displayName, color, activeFrontGame.nowMs);
  }
}

function focusItem(playerID: number, pageID: number, itemID: string) {
  if (activeBackGame instanceof DragonGame) {
    activeBackGame.characterStatsScroll.focusItem(playerID, pageID, itemID);
  }
}

function unfocusItem(playerID: number, pageID: number, itemID: string) {
  if (activeBackGame instanceof DragonGame) {
    activeBackGame.characterStatsScroll.unfocusItem(playerID, pageID, itemID);
  }
}

function playerPageChanged(playerID: number, pageID: number, playerData: string) {
  if (activeBackGame instanceof DragonGame) {
    activeBackGame.characterStatsScroll.playerPageChanged(playerID, pageID, playerData);
  }
}

function userHasCoins(userId: string, amount: number) {
  if (activeBackGame instanceof DroneGame) {
    let userDrone: Drone = <Drone>activeBackGame.allDrones.find(userId);
    if (userDrone)
      userDrone.coinCount += amount;
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