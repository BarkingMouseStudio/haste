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

    [MenuItem("Window/Haste/Benchmarks")]
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
      if (GUILayout.Button(String.Format("Bench {0}", "HasteResult#CompareTo"))) {
        BenchHasteResultCompareTo();
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
      if (GUILayout.Button(String.Format("Bench {0}", "StartsWith"))) {
        BenchStartsWith();
      }
      if (GUILayout.Button(String.Format("Bench {0}", "IndexOfZero"))) {
        BenchIndexOfZero();
      }
      if (GUILayout.Button(String.Format("Bench {0}", "GetWeightedSubsequence"))) {
        BenchGetWeightedSubsequence();
      }
    }

    // ~ 13 => 22
    public void BenchHasteIndexAdd() {
      List<IHasteItem> items = new List<IHasteItem>();
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

    // ~ 5
    public void BenchHasteResultCompareTo() {
      string query = "abc";
      int queryLen = query.Length;

      var a = new HasteItem(HastePerf.GetRandomPath(), 0, HasteHierarchySource.NAME);
      var b = new HasteItem(HastePerf.GetRandomPath(), 0, HasteHierarchySource.NAME);

      var aR = new HasteResult(a, HasteScoring.Score(a, query, queryLen), query);
      var bR = new HasteResult(b, HasteScoring.Score(b, query, queryLen), query);

      Benchmark("HasteResult#CompareTo", 10, () => {
        aR.CompareTo(bR);
      });
    }

    // ~ 126ms
    public void BenchHasteIndexFilter() {
      var index = new HasteIndex();
      var search = new HasteSearch(index);
      for (int i = 0; i < 100000; i++) {
        index.Add(new HasteItem(HastePerf.GetRandomPath(), 0, HasteHierarchySource.NAME));
      }
      Benchmark("HasteIndex#Filter", 10, () => {
        var promise = new Promise<IHasteResult[]>();
        HasteScheduler.Sync(search.Search("s", 100, promise));
      });
    }

    // ~ 2.5
    public void BenchLetterBitsetFromString() {
      Benchmark("LetterBitsetFromString", 100000, () => {
        HasteStringUtils.LetterBitsetFromString("this is a test");
      });
    }

    // ~ 24 - 26
    public void BenchHasteResult() {
      var item = new HasteItem("Apples/Bananas/Carrots", 0, "");
      var query = "abc";
      var queryLen = query.Length;
      Benchmark("HasteResult", 10000, () => {
        new HasteResult(item, HasteScoring.Score(item, query, queryLen), query);
      });
    }

    // ~ 26
    public void BenchBoldLabel() {
      string str = "Apples/Bananas/Carrots";
      int[] indices = new int[]{0, 7, 15};
      Benchmark("BoldLabel", 10000, () => {
        HasteStringUtils.BoldLabel(str, indices);
      });
    }

    // ~ 40
    public void BenchHasteItem() {
      Benchmark("HasteItem", 10000, () => {
        new HasteItem("Apples/Bananas/Carrots", 0, "TEST");
      });
    }

    // ~ 2.5
    public void BenchApproximately() {
      Benchmark("Approximately", 100000, () => {
        HasteMathUtils.Approximately(1.0f, 0.0f);
        HasteMathUtils.Approximately(0.0f, 1.0f);
        HasteMathUtils.Approximately(1.0f, 1.0f);
      });
    }

    // ~ 17
    public void BenchGetBoundaries() {
      var str = "Apples/Bananas/Carrots";
      Benchmark("GetBoundaries", 10000, () => {
        HasteStringUtils.GetBoundaries(str);
      });
    }

    // ~ 155
    public void BenchGetWeightedSubsequence() {
      string query = "ropr";
      string path = "Unity Test Tools/Platform Runner/Run on platform";

      string queryLower = query.ToLowerInvariant();
      string pathLower = path.ToLowerInvariant();
      int[] boundaryIndices = HasteStringUtils.GetBoundaryIndices(path);

      Benchmark("GetWeightedSubsequence", 10000, () => {
        HasteStringUtils.GetWeightedSubsequence(pathLower, queryLower, boundaryIndices);
      });
    }

    // ~ 5
    public void BenchGetFileNameWithoutExtension() {
      Benchmark("GetFileNameWithoutExtension", 100000, () => {
        HasteStringUtils.GetFileNameWithoutExtension("Apples/Bananas/Carrots.cs");
      });
    }

    // ~ 18
    public void BenchStartsWith() {
      Benchmark("StartsWith", 100000, () => {
        ("Apples/Bananas/Carrots.cs").StartsWith("Apples");
      });
    }

    // ~ 2.7
    public void BenchIndexOfZero() {
      Benchmark("IndexOfZero", 100000, () => {
        ("Apples/Bananas/Carrots.cs").IndexOf("Apples");
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
