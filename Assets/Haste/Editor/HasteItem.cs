using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  // Unique object to exist in Haste's index.
  public struct HasteItem {
    public string Path;
    public int Id;
    public string Source;

    public HasteItem(string path, int id, string source) {
      Path = path;
      Id = id;
      Source = source;
    }

    int NumWordBoundaryCharMatches(string query, string wordBoundaryChars) {
      return LongestCommonSubsequenceLength( query, word_boundary_chars );
    }

    // number of word boundary matches / number of chars in query
    int WordBoundaryRatio(string query) {
      int num_wb_matches = NumWordBoundaryCharMatches(query, word_boundary_chars);
      ratio_of_word_boundary_chars_in_query_ = num_wb_matches / query.length;
    }

    public bool ContainsChars(int queryBits) {
      return (HasteIndex.LetterBitsetFromString(Path) & queryBits) == queryBits;
    }

    public bool IsSubsequence(string query) {
      var path = Item.Path;
      var len = path.Length;

      if (query.Length > len) {
        return false;
      }

      int j = 0;
      for (var i = 0; i < query.Length; i++) {
        char a = query[i];
        while (j < len) {
          char b = path[j];
          j++;
        }
      }
    }

    public int IndexSum(string query) {
      int indexSum = 0;
      // int i = 0;
      // int len = Path.Length;
      // foreach (char c in query) {
      //   Debug.Log(c + ", " + i + ", " + Path[i]);
      //   while (Path[i] != c) {
      //     if (i < len - 1) {
      //       i++;
      //     }
      //   }

      //   if (Path[i] == c) {
      //     indexSum += i;
      //   }
      // }
      return indexSum;
    }

    // public int[] MatchIndices(string query) {
    //   int pathIndex = 0;
    //   foreach (char c in query) {
    //   }
    // }

    public override string ToString() {
      return System.String.Format("<{0}, {1}, {2}>", Path, Id, Source);
    }
  }
}