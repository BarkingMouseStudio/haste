using UnityEngine;

namespace Haste {

  // TODO: Can eventually hold match index
  public struct Result {
    public Item Item;
    public int Score;

    public Result(Item item, int score) {
      this.Item = item;
      this.Score = score;
    }
  }
}