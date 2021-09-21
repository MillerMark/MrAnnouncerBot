using System;

namespace DHDM
{
  public class BaseLiveFeedAnimator
  {
    public double VideoAnchorHorizontal { get; private set; }
    public double VideoAnchorVertical { get; private set; }
    public double VideoWidth { get; set; }
    public double VideoHeight { get; set; }

    /// <summary>
    /// The left point on the screen around which the video will rotate, scale, etc.
    /// </summary>
    public double ScreenAnchorLeft { get; set; }

    /// <summary>
    /// The top point on the screen around which the video will rotate, scale, etc.
    /// </summary>
    public double ScreenAnchorTop { get; set; }

    public string SceneName { get; set; }

    public BaseLiveFeedAnimator(double videoAnchorHorizontal,
    double videoAnchorVertical,
    double videoWidth,
    double videoHeight, string sceneName, string itemName)
    {
      SceneName = sceneName;
      ItemName = itemName;
      VideoHeight = videoHeight;
      VideoWidth = videoWidth;
      VideoAnchorHorizontal = videoAnchorHorizontal;
      VideoAnchorVertical = videoAnchorVertical;
    }

    public string ItemName { get; set; }
  }
}
