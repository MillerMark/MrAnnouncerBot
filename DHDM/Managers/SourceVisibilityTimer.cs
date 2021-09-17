//#define profiling
using System;
using System.Linq;
using DndCore;

namespace DHDM
{
  public class SourceVisibilityTimer : System.Timers.Timer
  {

    public SourceVisibilityTimer()
    {

    }
    public SetObsSourceVisibilityEventArgs ea { get; set; }
  }
}
