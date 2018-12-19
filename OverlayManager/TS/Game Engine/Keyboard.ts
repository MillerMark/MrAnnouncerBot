function handleKeyDown(evt) {
  const Key_B = 66;
  const Key_C = 67;
  const Key_D = 68;
  //const Key_G = 71;
  const Key_M = 77;
  const Key_O = 79;
  const Key_P = 80;
  const Key_S = 83;
  const Key_Up = 38;
  const Key_Right = 39;
  const Key_Left = 37;
  const Key_Down = 40;

  var now = performance.now();
  evt = evt || window.event;
  if (evt.keyCode == 13) {
    if (!myRocket.started || myRocket.isDocked) {
      myRocket.started = true;
      myRocket.launch(now);
      gravityGames.newGame();
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
  //else if (evt.keyCode == Key_G) {
  //  myRocket.dropSeed(now, 'grass');
  //}
  else if (evt.keyCode == Key_M) {
    myRocket.dropMeteor(now);
  }
  else if (evt.keyCode == Key_O) {
    myRocket.releaseDrone(now, '', '', '');
  }
  else if (evt.keyCode == Key_S) {
    myRocket.dropSeed(now);
  }
  else if (evt.keyCode == Key_P) {
    gravityGames.cyclePlanet();
  }
  else if (evt.keyCode == Key_B) {
    myRocket.releaseBee(now, '', '', '', '');
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

