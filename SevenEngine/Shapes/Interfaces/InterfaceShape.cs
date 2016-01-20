// Author(s):
// - Zachary Aaron Patten (aka Seven) seven@sevenengine.com
// Last Edited: 11-28-13

using System;

namespace SevenEngine.Physics.Primitives
{
  public interface InterfaceShape
  {
    float SurfaceArea { get; }
    float Volume { get; }

    float MinimumX { get; }
    float MinimumY { get; }
    float MinimumZ { get; }
    float MaximumX { get; }
    float MaximumY { get; }
    float MaximumZ { get; }
  }
}