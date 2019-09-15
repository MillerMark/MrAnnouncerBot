using System;

public enum FanfareDuration
{
    fullLength,
    clipped
}


public class FanfareDto
{

    public string DisplayName { get; set; }
    public double SecondsLong { get; set; }
    public int Index { get; set; }
    public FanfareDuration Duration { get; set; }
    public DateTime LastPlayed { get; set; }

    public FanfareDto()
    {

    }
}