using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Haste {

  public class HasteBenchmarks : EditorWindow {

    static void Benchmark(string name, int iter, Action action) {
      long totalTicks = 0;

      Stopwatch timer = new Stopwatch();
      for (int i = 0; i < iter; i++) {
        timer.Reset();
        timer.Start();

        action();

        timer.Stop();
        totalTicks += timer.ElapsedTicks;
      }

      var avgTicks = (float)totalTicks / iter;
      HasteDebug.Info("{0} ({1}) - Avg. Ticks: {2}", name, iter, avgTicks);
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
      if (GUILayout.Button(String.Format("Bench {0}", "LongestCommonSubsequenceLength"))) {
        BenchLongestCommonSubsequenceLength();
      }
      if (GUILayout.Button(String.Format("Bench {0}", "BoldLabel"))) {
        BenchBoldLabel();
      }
    }

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

    public void BenchHasteIndexFilter() {
      var index = new HasteIndex();
      for (int i = 0; i < 10000; i++) {
        index.Add(new HasteItem(HastePerf.GetRandomPath(), 0, HasteHierarchySource.NAME));
      }

      Benchmark("HasteIndex#Filter", 10, () => {
        index.Filter("s", 25);
      });
    }

    public void BenchHasteResult() {
      var item = new HasteItem("Apples/Bananas/Carrots", 0, "TEST");
      var query = "abc";
      Benchmark("HasteResult", 10000, () => {
        new HasteHierarchyResult(item, query);
      });
    }

    public void BenchBoldLabel() {
      string str = "Apples/Bananas/Carrots";
      int[] indices = new int[]{0, 7, 15};
      Benchmark("BoldLabel", 10000, () => {
        HasteStringUtils.BoldLabel(str, indices);
      });
    }

    public void BenchHasteItem() {
      Benchmark("HasteItem", 10000, () => {
        new HasteItem("Apples/Bananas/Carrots", 0, "TEST");
      });
    }

    public void BenchLongestCommonSubsequenceLength() {
      var testItem = new HasteItem("Apples/Bananas/Carrots", 0, "TEST");
      var testQuery = "abc";

      Benchmark("LongestCommonSubsequenceLength", 10000, () => {
        HasteStringUtils.LongestCommonSubsequenceLength(testQuery, testItem.BoundariesLower);
      });
    }
  }
}
