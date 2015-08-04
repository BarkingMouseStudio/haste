using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class HasteSearch {

    const int MIN_SORT_LEN = 1000;

    readonly HasteItem[] emptyMatches = new HasteItem[0];

    HasteIndex index;

    public HasteSearch(HasteIndex index) {
      this.index = index;
    }

    // Perform fast subsequence filtering
    IEnumerator Filter(string queryLower, int queryLen, IPromise<HasteItem[]> promise) {
      if (queryLen == 0) {
        promise.Resolve(emptyMatches);
        yield break;
      }

      // Lookup bucket by first char
      HashSet<HasteItem> bucket;
      if (!index.TryGetValue(queryLower[0], out bucket)) {
        promise.Resolve(emptyMatches);
        yield break;
      }

      int queryBits = HasteStringUtils.LetterBitsetFromString(queryLower);

      // We need to copy the hashset in case the indexer adds an item while we iterate
      var bucketArr = new HasteItem[bucket.Count];
      bucket.CopyTo(bucketArr);

      double startTime = EditorApplication.timeSinceStartup;

      var matches = new List<HasteItem>();
      HasteItem m;
      for (var i = 0; i < bucketArr.Length; i++) {
        m = bucketArr[i];

        if (m.pathLower.Length < queryLen) {
          continue;
        }

        var contains = HasteStringUtils.ContainsChars(m.bitset, queryBits);
        if (!contains) {
          continue;
        }

        var subsequence = HasteStringUtils.ContainsSubsequence(m.pathLower, queryLower, m.pathLower.Length, queryLen);
        if (!subsequence) {
          continue;
        }

        matches.Add(m);

        if (EditorApplication.timeSinceStartup - startTime >= Haste.MAX_ITER_TIME) {
          startTime = EditorApplication.timeSinceStartup;
          yield return null;
        }
      }

      promise.Resolve(matches.ToArray());
    }

    IEnumerator Map(HasteItem[] matches, string queryLower, int queryLen, IPromise<IHasteResult[]> promise) {
      double startTime = EditorApplication.timeSinceStartup;

      IHasteResult[] results = new IHasteResult[matches.Length];
      HasteItem m;
      for (var i = 0; i < matches.Length; i++) {
        m = matches[i];
        results[i] = m.GetResult(HasteScoring.Score(m, queryLower, queryLen), queryLower);

        if (EditorApplication.timeSinceStartup - startTime >= Haste.MAX_ITER_TIME) {
          startTime = EditorApplication.timeSinceStartup;
          yield return null;
        }
      }

      promise.Resolve(results);
    }

    void Swap(IHasteResult[] A, int i, int j) {
      var tmp = A[i];
      A[i] = A[j];
      A[j] = tmp;
    }

    int Partition(IHasteResult[] A, int lo, int hi) {
      var pivotIndex = hi;
      var pivotValue = A[pivotIndex];

      // Put the chosen pivot at A[hi]
      Swap(A, pivotIndex, hi);

      // Compare remaining array elements against pivotValue = A[hi]
      var storeIndex = lo;
      for (var i = lo; i <= hi; i++) {
        if (A[i].CompareTo(pivotValue) == -1) {
          Swap(A, i, storeIndex);
          storeIndex++;
        }
      }

      Swap(A, storeIndex, hi); // Move pivot to its final place

      return storeIndex;
    }

    // In-place async quicksort
    IEnumerator Sort(IHasteResult[] A, int lo, int hi) {
      var len = (hi - lo) + 1;

      if (len < MIN_SORT_LEN) {
        Array.Sort(A, lo, len);
        yield break;
      }

      if (lo < hi) {
        var p = Partition(A, lo, hi);
        yield return Haste.Scheduler.Start(Sort(A, lo, p - 1));
        yield return Haste.Scheduler.Start(Sort(A, p + 1, hi));
      }
    }

    public IEnumerator Search(string query, int count, IPromise<IHasteResult[]> searchResult) {
      int queryLen = query.Length;
      string queryLower = query.ToLowerInvariant();

      // Grab a filtered subset from the index
      var filterResult = new Promise<HasteItem[]>();
      // using (new HasteStopwatch("Filter")) {
      yield return Haste.Scheduler.Start(Filter(queryLower, queryLen, filterResult)); // Wait on filter
      // }

      if (filterResult.Reason != null) {
        searchResult.Reject(filterResult.Reason);
        yield break;
      } else if (filterResult.Value == null) {
        searchResult.Reject(new ArgumentNullException("filterResult"));
        yield break;
      }

      // Convert items to results with scores
      var mapResult = new Promise<IHasteResult[]>();
      // using (new HasteStopwatch("Map")) {
      yield return Haste.Scheduler.Start(Map(filterResult.Value, queryLower, queryLen, mapResult)); // Wait on map
      // }

      if (mapResult.Reason != null) {
        searchResult.Reject(mapResult.Reason);
        yield break;
      } else if (mapResult.Value == null) {
        searchResult.Reject(new ArgumentNullException("mapResult"));
        yield break;
      }

      // Sort the results based on those scores
      var sorted = mapResult.Value;
      // using (new HasteStopwatch("Sort")) {
      yield return Haste.Scheduler.Start(Sort(sorted, 0, sorted.Length - 1)); // Wait on sort
      // }

      // Take desired count
      searchResult.Resolve(sorted.Take(count).ToArray());
    }
  }
}
