var connection;

function connectToSignalR(signalR) {
  connection = new signalR.HubConnectionBuilder().withUrl("/CodeRushedHub").configureLogging(signalR.LogLevel.Information).build();
  window.onload = function () {
    connection.start().catch(err => console.error(err.toString()));
    connection.on("ExecuteCommand", executeCommand);
    connection.on("UserHasCoins", userHasCoins);
  };
}

function userHasCoins(userId: string, amount: number) {
  if (activeGame instanceof DroneGame) {
    let userDrone: Drone = <Drone>activeGame.allDrones.find(userId);
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