using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public interface IFuture<T> {
    T Value { get; }
    Exception Reason { get; }
    bool IsComplete { get; }
  }

  public interface IPromise<T> {
    void Resolve(T val);
    void Reject(Exception reason);
  }

  public class Promise<T> : IPromise<T>, IFuture<T> {

    public T Value { get; protected set; }
    public Exception Reason { get; protected set; }
    public bool IsComplete { get; protected set; }

    public void Resolve(T val) {
      if (IsComplete) {
        throw new Exception("Cannot write to a promise twice.");
      }

      Value = val;
      IsComplete = true;

      // Debug.Log(GetHashCode() + " " + IsComplete + " " + Value);
    }

    public void Reject(Exception reason) {
      if (IsComplete) {
        throw new Exception("Cannot write to a promise twice.");
      }

      Reason = reason;
      IsComplete = true;
    }
  }
}
