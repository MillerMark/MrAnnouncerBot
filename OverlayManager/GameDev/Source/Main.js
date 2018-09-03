
function updateScreen() {
  myContext.clearRect(0, 0, 1820, 980);
  var now = new Date();
  myRocket.updatePosition(now);
  myRocket.bounce(0, 0, 1820, 980, now);
  coins.collect(myRocket.x, myRocket.y, 310, 70);
  coins.draw(myContext, now);
  myRocket.draw(myContext, now);
}

function handleKeyDown(evt) {
  const Key_C = 67;
  const Key_D = 68;
  const Key_Up = 38;
  const Key_Right = 39;
  const Key_Left = 37;
  const Key_Down = 40;

  var now = new Date();
  evt = evt || window.event;
  if (evt.keyCode == 13) {
    if (!started || myRocket.isDocked) {
      started = true;
      myRocket.launch(now);
    }
    else if (myRocket.enginesRetracted)
      myRocket.extendEngines(now);
    else
      myRocket.retractEngines(now);
    return false;
  }
  else if (evt.keyCode == Key_Up) {
    myRocket.fireMainThrusters(now);
    return false;
  }
  else if (evt.keyCode == Key_Down) {
    myRocket.killHoverThrusters(now);
    return false;
  }
  else if (evt.keyCode == Key_Right) {
    myRocket.fireLeftThruster(now);
    return false;
  }
  else if (evt.keyCode == Key_Left) {
    myRocket.fireRightThruster(now);
    return false;
  }
  else if (evt.keyCode == Key_D) {
    myRocket.dock(now);
  }
  else if (evt.keyCode == Key_C) {
    if (myRocket.chuteDeployed)
      myRocket.retractChutes(now);
    else 
      myRocket.deployChute(now);
    return false;
  }
}

document.onkeydown = handleKeyDown;
var myCanvas = document.getElementById("myCanvas");
var coins = new Coins(100, 100, myCanvas.clientWidth - 100, myCanvas.clientHeight - 100);
var myContext = myCanvas.getContext("2d");
var myRocket = new Rocket(0, 0);
var started = false;
myRocket.x = 0;
myRocket.y = 0;
setInterval(updateScreen, 10);
