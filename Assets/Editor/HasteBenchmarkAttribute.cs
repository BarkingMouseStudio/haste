using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class HasteBenchmarkAttribute : Attribute {

  public string Name { get; private set; }

  public HasteBenchmarkAttribute(string name) {
    Name = name;
  }
}
