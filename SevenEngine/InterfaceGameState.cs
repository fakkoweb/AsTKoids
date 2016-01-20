// Author(s):
// - Zachary Aaron Patten (aka Seven) seven@sevenengine.com
// Last Edited: 08-09-14

using Seven.Structures;

namespace SevenEngine
{
  /// <summary>INHERIT ME!</summary>
  public interface InterfaceGameState
  {
    string Id { get; set; }
    bool IsReady { get; }
    void Load();
    string Update(float elapsedTime);
    void Render();
  }
}