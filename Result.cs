using UnityEngine;

namespace Haste {

  public struct Result {
    public Item Item;
    public int Score;
    public int[] Matches;

    public Result(Item item, int score) {
      this.Item = item;
      this.Score = score;

      // TODO: Can eventually hold match indices within string
      this.Matches = new int[0];
    }
  }
}