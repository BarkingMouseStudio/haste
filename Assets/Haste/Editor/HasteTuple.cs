namespace Haste {

  public class HasteTuple<T1, T2> {

    public T1 First { get; private set; }
    public T2 Second { get; private set; }

    public HasteTuple(T1 first, T2 second) {
      First = first;
      Second = second;
    }
  }

  public class HasteTuple<T1, T2, T3> {

    public T1 First { get; private set; }
    public T2 Second { get; private set; }
    public T3 Third { get; private set; }

    public HasteTuple(T1 first, T2 second, T3 third) {
      First = first;
      Second = second;
      Third = third;
    }
  }

  public static class HasteTuple {

    public static HasteTuple<T1, T2> Create<T1, T2>(T1 first, T2 second) {
      return new HasteTuple<T1, T2>(first, second);
    }

    public static HasteTuple<T1, T2, T3> Create<T1, T2, T3>(T1 first, T2 second, T3 third) {
      return new HasteTuple<T1, T2, T3>(first, second, third);
    }
  }
}
