
function updateScreen() {
  myContext.clearRect(0, 0, 1920, 1080);
  myRocket.updatePosition();
  myRocket.bounce(0, -10000, 1920, 1080);
  myRocket.draw(myContext);
}

function handleKeyDown(evt) {
  const Key_C = 67;
  const Key_Up = 38;
  const Key_Right = 39;
  const Key_Left = 37;
  evt = evt || window.event;
  if (evt.keyCode == 13) {
    myRocket.move(1, -3);
    return false;
  }
  else if (evt.keyCode == Key_Up) {
    myRocket.fireMainThrusters();
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
var myContext = myCanvas.getContext("2d");
var myRocket = new Rocket(200, 100);
setInterval(updateScreen, 5);