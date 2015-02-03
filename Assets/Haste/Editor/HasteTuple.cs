namespace Haste {

  public class HasteTuple<T1, T2> {

    public T1 First { get; private set; }
    public T2 Second { get; private set; }

    internal HasteTuple(T1 first, T2 second) {
      First = first;
      Second = second;
    }
  }

  public static class HasteTuple {

    public static HasteTuple<T1, T2> Create<T1, T2>(T1 first, T2 second) {
      return new HasteTuple<T1, T2>(first, second);
    }
  }
}
