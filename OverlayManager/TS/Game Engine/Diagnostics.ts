var screenCenterX: number = screenWidth / 2;
var screenCenterY: number = screenHeight / 2;

function drawCrossHairs(context: CanvasRenderingContext2D, x: number, y: number) {
  const crossHalfSize: number = 8;
  context.beginPath();
  context.strokeStyle = '#f00';
  context.moveTo(x - crossHalfSize, y);
  context.lineTo(x + crossHalfSize, y);
  context.moveTo(x, y - crossHalfSize);
  context.lineTo(x, y + crossHalfSize);
  context.stroke();
}