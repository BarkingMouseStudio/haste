using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class HasteSearch {

    HasteResultComparer comparer = new HasteResultComparer();
    HasteIndex index;

    public HasteSearch(HasteIndex index) {
      this.index = index;
    }

    // Perform fast subsequence filtering
    IEnumerator Filter(string queryLower, int queryLen, IPromise<List<IHasteItem>> promise) {
      var matches = new List<IHasteItem>();

      if (queryLen == 0) {
        promise.Resolve(matches);
        yield break;
      }

      // Lookup bucket by first char
      HashSet<IHasteItem> bucket;
      if (!index.TryGetValue(queryLower[0], out bucket)) {
        promise.Resolve(matches);
        yield break;
      }

      int queryBits = HasteStringUtils.LetterBitsetFromString(queryLower);
      char q0 = queryLower[0];

      foreach (var m in bucket) {
        if (m.PathLower.Length < queryLen) {
          continue;
        }

        bool firstCharName = m.NameLower.Length > 0 && m.NameLower[0] == q0;
        bool firstCharPath = m.PathLower.Length > 0 && m.PathLower[0] == q0;
        bool firstCharExtension = m.ExtensionLower.Length > 0 && m.ExtensionLower[0] == q0;
        if (q0 != '.' && !firstCharName && !firstCharPath && !firstCharExtension) {
          // TODO: Move to bucketing instead of boundaries
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

      promise.Resolve(matches);
    }

    IEnumerator Map(List<IHasteItem> matches, string queryLower, int queryLen, IPromise<List<IHasteResult>> promise) {
      var results = new List<IHasteResult>(matches.Count);
      foreach (var m in matches) {
        results.Add(m.GetResult(queryLower, queryLen));
        yield return null;
      }
      promise.Resolve(results);
    }

    IEnumerator Merge(IEnumerable<IHasteResult> left, IEnumerable<IHasteResult> right, IPromise<IEnumerable<IHasteResult>> promise) {
      var leftCount = left.Count();
      var rightCount = right.Count();

      var result = new List<IHasteResult>(leftCount + rightCount);

      IHasteResult left0, right0;
      while (leftCount > 0 && rightCount > 0) {
        left0 = left.First();
        right0 = right.First();

        if (comparer.Compare(left0, right0) <= 0) {
          result.Add(left0);
          left = left.Skip(1);
        } else {
          result.Add(right0);
          right = right.Skip(1);
        }

        yield return null;
      }

      // Append any remaining elements.
      while (leftCount > 0) {
        result.Add(left.First());
        left = left.Skip(1);
        yield return null;
      }

      while (rightCount > 0) {
        result.Add(right.First());
        right = right.Skip(1);
        yield return null;
      }

      promise.Resolve(result);
    }

    IEnumerator Sort(IEnumerable<IHasteResult> m, IPromise<IEnumerable<IHasteResult>> promise) {
      var mCount = m.Count();

      // Base case: a list of zero or one elements is sorted, by definition.
      if (mCount <= 1) {
        promise.Resolve(m);
        yield break;
      }

      // Recursive case: 1st, divide the list into equal-sized sublists.
      var middle = mCount / 2;

      var left = m.Take(middle);
      var right = m.Skip(middle);

      HasteDebug.Assert(left.Count() + right.Count() == mCount,
        "Halves should equal whole.");

      // Recursively sort both sublists
      var leftPromise = new Promise<IEnumerable<IHasteResult>>();
      yield return Haste.Scheduler.Start(Sort(left, leftPromise));

      var rightPromise = new Promise<IEnumerable<IHasteResult>>();
      yield return Haste.Scheduler.Start(Sort(right, rightPromise));

      // Then merge the now-sorted sublists
      yield return Haste.Scheduler.Start(Merge(leftPromise.Value, rightPromise.Value, promise));
    }

    public IEnumerator Search(string query, int count, IPromise<IEnumerable<IHasteResult>> promise) {
      int queryLen = query.Length;
      string queryLower = query.ToLowerInvariant();

      // Grab a filtered subset from the index
      var filter = new Promise<List<IHasteItem>>();
      yield return Haste.Scheduler.Start(Filter(queryLower, queryLen, filter)); // Wait on filter
      Debug.Log(filter.GetHashCode() + " " + filter.IsComplete + " " + filter.Value);

      // Convert items to results with scores
      var map = new Promise<List<IHasteResult>>();
      yield return Haste.Scheduler.Start(Map(filter.Value, queryLower, queryLen, map)); // Wait on map
      Debug.Log(map.GetHashCode() + " " + map.IsComplete + " " + map.Value);

      // Sort the results based on those scores
      var sort = new Promise<IEnumerable<IHasteResult>>();
      yield return Haste.Scheduler.Start(Sort(map.Value, sort)); // Wait on sort
      Debug.Log(sort.GetHashCode() + " " + sort.IsComplete + " " + sort.Value);

      // Take desired count
      var results = sort.Value.Take(count);
      promise.Resolve(results);
    }
  }
}
