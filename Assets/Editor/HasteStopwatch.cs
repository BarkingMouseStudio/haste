using System;
using System.Diagnostics;

namespace Haste {

  public class HasteStopwatch : IDisposable {

    Stopwatch timer;
    string name;

    public HasteStopwatch(string name) {
      this.name = name;
      this.timer = new Stopwatch();
      this.timer.Start();
    }

    public void Dispose() {
      timer.Stop();
      HasteDebug.Info("{0}: {1} ({2}ms)", name, timer.ElapsedTicks, timer.ElapsedMilliseconds);
    }
  }
}