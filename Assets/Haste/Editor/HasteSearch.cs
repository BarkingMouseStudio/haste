using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class HasteSearch {

    readonly HasteResultComparer comparer = new HasteResultComparer();
    readonly IHasteItem[] emptyMatches = new IHasteItem[0];

    HasteIndex index;

    public HasteSearch(HasteIndex index) {
      this.index = index;
    }

    // Perform fast subsequence filtering
    IEnumerator Filter(string queryLower, int queryLen, IPromise<IHasteItem[]> promise) {
      if (queryLen == 0) {
        promise.Resolve(emptyMatches);
        yield break;
      }

      // Lookup bucket by first char
      HashSet<IHasteItem> bucket;
      if (!index.TryGetValue(queryLower[0], out bucket)) {
        promise.Resolve(emptyMatches);
        yield break;
      }

      int queryBits = HasteStringUtils.LetterBitsetFromString(queryLower);
      char q0 = queryLower[0];

      // We need to copy the hashset in case the indexer adds an item while we iterate
      var bucketArr = new IHasteItem[bucket.Count];
      bucket.CopyTo(bucketArr);

      var matches = new List<IHasteItem>();

      foreach (var m in bucketArr) {
        if (m.PathLower.Length < queryLen) {
          continue;
        }

        bool firstCharName = m.NameLower.Length > 0 && m.NameLower[0] == q0;
        bool firstCharPath = m.PathLower.Length > 0 && m.PathLower[0] == q0;
        bool firstCharExtension = q0 == '.' || m.ExtensionLower.Length > 0 && m.ExtensionLower[0] == q0;
        if (!firstCharName && !firstCharPath && !firstCharExtension) {
          continue;
        }

        var contains = HasteStringUtils.ContainsChars(m.Bitset, queryBits);
        if (!contains) {
          continue;
        }

        var subsequence = HasteStringUtils.ContainsSubsequence(m.PathLower, queryLower, m.PathLower.Length, queryLen);
        if (!subsequence) {
          continue;
        }

        matches.Add(m);
        yield return null;
      }

      promise.Resolve(matches.ToArray());
    }

    IEnumerator Map(IHasteItem[] matches, string queryLower, int queryLen, IPromise<IHasteResult[]> promise) {
      IHasteResult[] results = new IHasteResult[matches.Length];
      IHasteItem m;
      for (var i = 0; i < matches.Length; i++) {
        m = matches[i];
        results[i] = m.GetResult(HasteScoring.Score(m, queryLower, queryLen), queryLower);
        yield return null;
      }
      promise.Resolve(results);
    }

    IEnumerator Merge(IHasteResult[] left, IHasteResult[] right, IPromise<IHasteResult[]> promise) {
      int leftCount = left.Length;
      int rightCount = right.Length;

      var result = new List<IHasteResult>(leftCount + rightCount);

      IHasteResult left0, right0;
      while (leftCount > 0 && rightCount > 0) {
        left0 = left.First();
        right0 = right.First();

        if (comparer.Compare(left0, right0) <= 0) {
          result.Add(left0);
          left = left.Skip(1).ToArray();
          leftCount--;
        } else {
          result.Add(right0);
          right = right.Skip(1).ToArray();
          rightCount--;
        }

        yield return null;
      }

      // Append any remaining elements.
      while (leftCount > 0) {
        result.Add(left.First());
        left = left.Skip(1).ToArray();
        leftCount--;
      }

      while (rightCount > 0) {
        result.Add(right.First());
        right = right.Skip(1).ToArray();
        rightCount--;
      }

      promise.Resolve(result.ToArray());
    }

    IEnumerator Sort(IHasteResult[] m, IPromise<IHasteResult[]> promise) {
      // Base case: a list of zero or one elements is sorted, by definition.
      if (m.Length <= 1) {
        promise.Resolve(m);
        yield break;
      }

      // Recursive case: 1st, divide the list into equal-sized sublists.
      var middle = m.Length / 2;


      var left = m.Take(middle).ToArray(); // new ArraySegment<IHasteResult>(m, 0, middle);
      var right = m.Skip(middle).ToArray(); // new ArraySegment<IHasteResult>(m, middle, mCount - middle);

      var leftCount = left.Length;
      var rightCount = right.Length;

      HasteDebug.Assert(leftCount + rightCount == m.Length,
        "Halves should equal whole.");

      // Recursively sort both sublists
      var leftPromise = new Promise<IHasteResult[]>();
      yield return Haste.Scheduler.Start(Sort(left, leftPromise));

      var rightPromise = new Promise<IHasteResult[]>();
      yield return Haste.Scheduler.Start(Sort(right, rightPromise));

      // Then merge the now-sorted sublists
      yield return Haste.Scheduler.Start(Merge(leftPromise.Value, rightPromise.Value, promise));
    }

    public IEnumerator Search(string query, int count, IPromise<IHasteResult[]> searchResult) {
      int queryLen = query.Length;
      string queryLower = query.ToLowerInvariant();

      // Grab a filtered subset from the index
      var filterResult = new Promise<IHasteItem[]>();
      yield return Haste.Scheduler.Start(Filter(queryLower, queryLen, filterResult)); // Wait on filter

      if (filterResult.Reason != null) {
        searchResult.Reject(filterResult.Reason);
        yield break;
      } else if (filterResult.Value == null) {
        searchResult.Reject(new ArgumentNullException("filterResult"));
        yield break;
      }

      // Convert items to results with scores
      var mapResult = new Promise<IHasteResult[]>();
      yield return Haste.Scheduler.Start(Map(filterResult.Value, queryLower, queryLen, mapResult)); // Wait on map

      if (mapResult.Reason != null) {
        searchResult.Reject(mapResult.Reason);
        yield break;
      } else if (mapResult.Value == null) {
        searchResult.Reject(new ArgumentNullException("mapResult"));
        yield break;
      }

      // Sort the results based on those scores
      var sortResult = new Promise<IHasteResult[]>();
      yield return Haste.Scheduler.Start(Sort(mapResult.Value, sortResult)); // Wait on sort

      if (sortResult.Reason != null) {
        searchResult.Reject(sortResult.Reason);
        yield break;
      } else if (sortResult.Value == null) {
        searchResult.Reject(new ArgumentNullException("sortResult"));
        yield break;
      }

      // Take desired count
      var results = sortResult.Value.Take(count).ToArray();
      searchResult.Resolve(results);
    }
  }
}
