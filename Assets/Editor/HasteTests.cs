using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace Haste {

  [TestFixture]
  internal class HasteTests {

    [Test]
    public void Comparer() {
      HasteResultComparer comparer = new HasteResultComparer();

      string queryLower = "rop";
      IHasteResult a = new HasteResult(new HasteItem("Unity Test Tools/Platform Runner/Run on platform", 0, ""), queryLower, queryLower.Length);
      IHasteResult b = new HasteResult(new HasteItem("Assets/UnityTestTools/Common/Editor/icons/rerun-darktheme.png", 0, ""), queryLower, queryLower.Length);
      Assert.That(comparer.Compare(a, b), Is.EqualTo(-1));

      queryLower = "ca";
      IHasteResult c = new HasteResult(new HasteItem("Component/Add...", 0, ""), queryLower, queryLower.Length);
      IHasteResult d = new HasteResult(new HasteItem("Component/Layout/Canvas", 0, ""), queryLower, queryLower.Length);
      Assert.That(comparer.Compare(c, d), Is.EqualTo(-1));

      queryLower = "cec";
      IHasteResult e = new HasteResult(new HasteItem("GameObject/Create Empty Child", 0, ""), queryLower, queryLower.Length);
      IHasteResult f = new HasteResult(new HasteItem("Component/Physics/Cloth Renderer", 0, ""), queryLower, queryLower.Length);
      Assert.That(comparer.Compare(e, f), Is.EqualTo(-1));
    }

    [Test]
    public void Filter() {
      var index = new HasteIndex();
      index.Add(new HasteItem("Path/MyFileWithExtension.cs", 0, ""));

      // Test extensions
      var results = index.Filter(".cs", 1);
      Assert.That(results.Length, Is.EqualTo(1));

      // Test boundaries
      results = index.Filter("pm", 1);
      Assert.That(results.Length, Is.EqualTo(1));

      // Test name
      results = index.Filter("m", 1);
      Assert.That(results.Length, Is.EqualTo(1));
    }

    [Test]
    public void Approximately() {
      Assert.That(HasteResultComparer.Approximately(0.0f, 0.0f), Is.True);
      Assert.That(HasteResultComparer.Approximately(1.0f, 1.0f), Is.True);
      Assert.That(HasteResultComparer.Approximately(1.0f, 0.0f), Is.False);
      Assert.That(HasteResultComparer.Approximately(0.0f, 1.0f), Is.False);
    }

    [Test]
    public void GetBoundaries() {
      Assert.That(HasteStringUtils.GetBoundaries("Yak"), Is.EqualTo("y"));
      Assert.That(HasteStringUtils.GetBoundaries("LlamaCrab"), Is.EqualTo("lc"));
      Assert.That(HasteStringUtils.GetBoundaries("ShrewRail/Wren"), Is.EqualTo("srw"));
      Assert.That(HasteStringUtils.GetBoundaries("Unity Test Tools/Platform Runner/Run on platform"), Is.EqualTo("uttprrop"));
    }

    [Test]
    public void BoldLabel() {
      string path = "Apples/Bananas/Carrots";
      string query = "albncr";

      int[] boundaryIndices = HasteStringUtils.GetBoundaryIndices(path);
      int[] indices;
      HasteStringUtils.GetMatchIndices(path.ToLowerInvariant(), query.ToLowerInvariant(), 0, boundaryIndices, out indices);

      Assert.That(HasteStringUtils.BoldLabel(path, indices, "[", "]"),
        Is.EqualTo("[A]pp[l]es/[B]a[n]anas/[C]a[r]rots"));
    }
  }
}
