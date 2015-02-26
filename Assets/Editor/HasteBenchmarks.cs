using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Haste {

  internal class HasteBenchmarks : EditorWindow {

    static void Benchmark(string name, int iter, Action action) {
      long totalTicks = 0;
      double totalMS = 0.0;

      Stopwatch timer = new Stopwatch();
      for (int i = 0; i < iter; i++) {
        timer.Reset();
        timer.Start();

        action();

        timer.Stop();
        totalTicks += timer.ElapsedTicks;
        totalMS += timer.Elapsed.TotalMilliseconds;
      }

      var avgTicks = (float)totalTicks / iter;
      var avgTime = totalMS / iter;
      HasteDebug.Info("{0} ({1}) - Avg. Ticks: {2}, Avg. Time: {3}ms", name, iter, avgTicks, avgTime);
    }

    [MenuItem("Window/Benchmarks")]
    public static void Open() {
      var window = EditorWindow.GetWindow<HasteBenchmarks>();
      window.title = "Haste Benchmarks";
    }

    void OnGUI() {
      if (GUILayout.Button(String.Format("Bench {0}", "HasteIndex#Add"))) {
        BenchHasteIndexAdd();
      }
      if (GUILayout.Button(String.Format("Bench {0}", "HasteIndex#Filter"))) {
        BenchHasteIndexFilter();
      }
      if (GUILayout.Button(String.Format("Bench {0}", "HasteResult"))) {
        BenchHasteResult();
      }
      if (GUILayout.Button(String.Format("Bench {0}", "HasteItem"))) {
        BenchHasteItem();
      }
      if (GUILayout.Button(String.Format("Bench {0}", "HasteResultComparer"))) {
        BenchHasteResultComparer();
      }
      if (GUILayout.Button(String.Format("Bench {0}", "GetFileNameWithoutExtension"))) {
        BenchGetFileNameWithoutExtension();
      }
      if (GUILayout.Button(String.Format("Bench {0}", "LongestCommonSubsequenceLength"))) {
        BenchLongestCommonSubsequenceLength();
      }
      if (GUILayout.Button(String.Format("Bench {0}", "Approximately"))) {
        BenchApproximately();
      }
      if (GUILayout.Button(String.Format("Bench {0}", "GetBoundaries"))) {
        BenchGetBoundaries();
      }
      if (GUILayout.Button(String.Format("Bench {0}", "BoldLabel"))) {
        BenchBoldLabel();
      }
      if (GUILayout.Button(String.Format("Bench {0}", "LetterBitsetFromString"))) {
        BenchLetterBitsetFromString();
      }
    }

    // ~ 20
    public void BenchHasteIndexAdd() {
      List<HasteItem> items = new List<HasteItem>();
      int i = 0;
      for (; i < 10000; i++) {
        items.Add(new HasteItem(HastePerf.GetRandomPath(), 0, HasteHierarchySource.NAME));
      }

      var index = new HasteIndex();
      i = 0;
      Benchmark("HasteIndex#Add", 10000, () => {
        index.Add(items[i]);
        i++;
      });
    }

    // ~ 384ms
    public void BenchHasteResultComparer() {
      IList<HasteItem> items = new List<HasteItem>();
      for (int i = 0; i < 20000; i++) {
        items.Add(new HasteItem(HastePerf.GetRandomPath(), 0, HasteHierarchySource.NAME));
      }

      string query = "abc";
      int queryLen = query.Length;

      IEnumerable<HasteHierarchyResult> results = items.Select(m => {
        return new HasteHierarchyResult(m, query, queryLen);
      });

      var comparer = new HasteResultComparer();

      Benchmark("HasteResultComparer", 10, () => {
        results.OrderBy(r => r, comparer).ToArray();
      });
    }

    // ~ 248ms
    public void BenchHasteIndexFilter() {
      var index = new HasteIndex();
      for (int i = 0; i < 100000; i++) {
        index.Add(new HasteItem(HastePerf.GetRandomPath(), 0, HasteHierarchySource.NAME));
      }
      Benchmark("HasteIndex#Filter", 10, () => {
        index.Filter("s", 100);
      });
    }

    // ~ 1.75
    public void BenchLetterBitsetFromString() {
      Benchmark("LetterBitsetFromString", 100000, () => {
        HasteStringUtils.LetterBitsetFromString("this is a test");
      });
    }

    // ~ 25
    public void BenchHasteResult() {
      var item = new HasteItem("Apples/Bananas/Carrots", 0, "TEST");
      var query = "abc";
      var queryLen = query.Length;
      Benchmark("HasteResult", 10000, () => {
        new HasteHierarchyResult(item, query, queryLen);
      });
    }

    // ~ 16
    public void BenchBoldLabel() {
      string str = "Apples/Bananas/Carrots";
      int[] indices = new int[]{0, 7, 15};
      Benchmark("BoldLabel", 10000, () => {
        HasteStringUtils.BoldLabel(str, indices);
      });
    }

    // ~ 20
    public void BenchHasteItem() {
      Benchmark("HasteItem", 10000, () => {
        new HasteItem("Apples/Bananas/Carrots", 0, "TEST");
      });
    }

    // ~ 2.75
    public void BenchApproximately() {
      Benchmark("Approximately", 100000, () => {
        HasteResultComparer.Approximately(1.0f, 0.0f);
        HasteResultComparer.Approximately(0.0f, 1.0f);
        HasteResultComparer.Approximately(1.0f, 1.0f);
      });
    }

    // ~ 13
    public void BenchGetBoundaries() {
      var str = "Apples/Bananas/Carrots";
      Benchmark("GetBoundaries", 10000, () => {
        HasteStringUtils.GetBoundaries(str);
      });
    }

    // ~ 4
    public void BenchGetFileNameWithoutExtension() {
      Benchmark("GetFileNameWithoutExtension", 100000, () => {
        HasteStringUtils.GetFileNameWithoutExtension("Apples/Bananas/Carrots.cs");
      });
    }

    // ~ 5-8.5
    public void BenchLongestCommonSubsequenceLength() {
      var testItem = new HasteItem("Apples/Bananas/Carrots", 0, "TEST");
      var testQuery = "abc";

      Benchmark("LongestCommonSubsequenceLength", 100000, () => {
        HasteStringUtils.LongestCommonSubsequenceLength(testQuery, testItem.BoundariesLower);
      });
    }
  }
}
