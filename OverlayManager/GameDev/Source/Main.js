
function updateScreen() {
  myContext.clearRect(0, 0, 1820, 980);
  myRocket.updatePosition();
  myRocket.bounce(0, 0, 1820, 980);
  coins.collect(myRocket.x, myRocket.y, 310, 70);
  coins.draw(myContext);
  myRocket.draw(myContext);
}

function handleKeyDown(evt) {
  const Key_C = 67;
  const Key_D = 68;
  const Key_Up = 38;
  const Key_Right = 39;
  const Key_Left = 37;
  const Key_Down = 40;

  evt = evt || window.event;
  if (evt.keyCode == 13) {
    if (!started || myRocket.isDocked) {
      started = true;
      myRocket.launch();
    }
    else if (myRocket.enginesRetracted)
      myRocket.extendEngines();
    else
      myRocket.retractEngines();
    return false;
  }
  else if (evt.keyCode == Key_Up) {
    myRocket.fireMainThrusters();
    return false;
  }
  else if (evt.keyCode == Key_Down) {
    myRocket.killHoverThrusters();
    return false;
  }
  else if (evt.keyCode == Key_Right) {
    myRocket.fireLeftThruster();
    return false;
  }
  else if (evt.keyCode == Key_Left) {
    myRocket.fireRightThruster();
    return false;
  }
  else if (evt.keyCode == Key_D) {
    myRocket.dock();
  }
  else if (evt.keyCode == Key_C) {
    if (myRocket.chuteDeployed)
      myRocket.retractChutes();
    else 
      myRocket.deployChute();
    return false;
  }
}

document.onkeydown = handleKeyDown;
var myCanvas = document.getElementById("myCanvas");
var coins = new Coins(400, 200, myCanvas.clientWidth - 400, myCanvas.clientHeight - 200);
var myContext = myCanvas.getContext("2d");
var myRocket = new Rocket(0, 0);
var started = false;
myRocket.x = 0;
myRocket.y = 0;
setInterval(updateScreen, 10);
