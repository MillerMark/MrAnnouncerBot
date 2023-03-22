enum AnimationStyle {
  Static = 1,     // Show a single frame and never change it when advancing the frame.
  Random,         // Select a random frame and show it each time we advance the frame.
  Sequential,     // Animate once through the cycle and then clean up the animation.
  SequentialStop, // Animate once through the cycle and then show the last frame.
  Loop,
  CenterLoop      // Contains a lead-in animation, a center loop, and an outro animation.
                  // smoke.returnFrameIndex = 30;
                  // smoke.segmentSize = 47;
                  // smoke.resumeFrameIndex = 77;
}
// New comment 