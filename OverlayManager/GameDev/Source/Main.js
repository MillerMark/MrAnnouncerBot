function updateScreen() {
    myContext.clearRect(0, 0, 1820, 980);
    var now = new Date();
    myRocket.updatePosition(now);
    myRocket.bounce(0, 0, 1820, 980, now);
    if (coins.collect(myRocket.x, myRocket.y, 310, 70) > 0)
        new Audio('Assets/Sound Effects/CollectCoin.wav').play();
    redMeteors.bounce(0, 0, 1820, 980, now);
    blueMeteors.bounce(0, 0, 1820, 980, now);
    purpleMeteors.bounce(0, 0, 1820, 980, now);
    backgroundBanner.draw(myContext, 0, 0);
    coins.draw(myContext, now);
    redMeteors.draw(myContext, now);
    blueMeteors.draw(myContext, now);
    purpleMeteors.draw(myContext, now);
    myRocket.draw(myContext, now);
    redExplosions.draw(myContext, now);
}
function handleKeyDown(evt) {
    var Key_C = 67;
    var Key_D = 68;
    var Key_M = 77;
    var Key_Up = 38;
    var Key_Right = 39;
    var Key_Left = 37;
    var Key_Down = 40;
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
    else if (evt.keyCode == Key_M) {
        myRocket.dropMeteor(now);
    }
    else if (evt.keyCode == Key_C) {
        if (myRocket.chuteDeployed)
            myRocket.retractChutes(now);
        else
            myRocket.deployChute(now);
        return false;
    }
}
function buildCenterRect(sprites) {
    sprites.fillRect(100, 100, myCanvas.clientWidth - 100, myCanvas.clientHeight - 100, 12);
}
function buildInnerRect(sprites) {
    sprites.fillRect(400, 200, myCanvas.clientWidth - 400, myCanvas.clientHeight - 200, 12);
}
function fillScreenRect(sprites) {
    sprites.fillRect(0, 0, myCanvas.clientWidth, myCanvas.clientHeight, 12);
}
function fillScreenRectMinusTwitchBanner(sprites) {
    sprites.fillRect(0, 0, myCanvas.clientWidth, myCanvas.clientHeight, 12);
    sprites.collect(100, 260, 1650, 240);
}
function outlineScreenRect(sprites) {
    sprites.outlineRect(0, 0, myCanvas.clientWidth, myCanvas.clientHeight, 11, rectangleDrawingSegment.bottom, 4);
}
function outlineMargin(sprites, margin) {
    sprites.outlineRect(2 * margin, margin, myCanvas.clientWidth - 2 * margin, myCanvas.clientHeight - margin, 12);
}
function outlineBigRect(sprites) {
    outlineMargin(sprites, 100);
}
function outlineMediumRect(sprites) {
    outlineMargin(sprites, 200);
}
function outlineSmallRect(sprites) {
    outlineMargin(sprites, 300);
}
function addExplosion(x) {
    redExplosions.sprites.push(new SpriteProxy(0, x - redExplosions.spriteWidth / 2 + 50, 0));
}
document.onkeydown = handleKeyDown;
var myCanvas = document.getElementById("myCanvas");
var coins = new Sprites("Spinning Coin/SpinningCoin", 165, 5, AnimationStyle.Loop, fillScreenRectMinusTwitchBanner);
var redMeteors = new Sprites("Spinning Rock/Red/Meteor", 63, 50, AnimationStyle.Loop);
redMeteors.moves = true;
var blueMeteors = new Sprites("Spinning Rock/Blue/Meteor", 63, 50, AnimationStyle.Loop);
blueMeteors.moves = true;
var purpleMeteors = new Sprites("Spinning Rock/Purple/Meteor", 63, 50, AnimationStyle.Loop);
purpleMeteors.moves = true;
var redExplosions = new Sprites("Explosion/Red/Explosion", 179, 5, AnimationStyle.Sequential);
var backgroundBanner = new Part("CodeRushedBanner", 1, AnimationStyle.Static, 200, 300);
var myContext = myCanvas.getContext("2d");
var myRocket = new Rocket(0, 0);
var started = false;
myRocket.x = 0;
myRocket.y = 0;
setInterval(updateScreen, 10);
//# sourceMappingURL=Main.js.map