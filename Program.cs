// Author(s):
// - Dario Facchini io.dariofacchini@gmail.com
// - Zachary Aaron Patten (aka Seven) seven@sevenengine.com -- for Seven library and importer
// Last Edited: 08-09-14

using System;

namespace AsTKoids
{
  static class Program
  {
    [STAThread]
    static void Main()
    {
      // you can select the starting window size here
      using (Game game = new Game(1280, 720)) { game.Run(); }
    }
  }
}