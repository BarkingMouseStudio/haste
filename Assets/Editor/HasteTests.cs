using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using System.Linq;

namespace Haste {

  [TestFixture]
  internal class HasteTests {

    [Test]
    public void TestResultComparer() {
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
    public void TestFilter() {
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
    public void TestApproximately() {
      Assert.That(HasteResultComparer.Approximately(0.0f, 0.0f), Is.True);
      Assert.That(HasteResultComparer.Approximately(1.0f, 1.0f), Is.True);
      Assert.That(HasteResultComparer.Approximately(1.0f, 0.0f), Is.False);
      Assert.That(HasteResultComparer.Approximately(0.0f, 1.0f), Is.False);
    }

    [Test]
    public void TestGetBoundaryIndices() {
      HasteItem item = new HasteItem("Unity Test Tools/Platform Runner/Run on platform.cs", 0, "");
      int[] boundaryIndices = HasteStringUtils.GetBoundaryIndices(item.Path);
      string bolded = HasteStringUtils.BoldLabel(item.Path, boundaryIndices, "[", "]");
      string expected = "[U]nity [T]est [T]ools/[P]latform [R]unner/[R]un [o]n [p]latform[.][c]s";
      Assert.That(bolded, Is.EqualTo(expected));
    }

    [Test]
    public void TestGetBoundaries() {
      Assert.That(HasteStringUtils.GetBoundaries("Yak"), Is.EqualTo("y"));
      Assert.That(HasteStringUtils.GetBoundaries("LlamaCrab"), Is.EqualTo("lc"));
      Assert.That(HasteStringUtils.GetBoundaries("ShrewRail/Wren"), Is.EqualTo("srw"));
      Assert.That(HasteStringUtils.GetBoundaries("Unity Test Tools/Platform Runner/Run on platform"), Is.EqualTo("uttprrop"));
    }

    [Test]
    [Category("BoldLabel")]
    public void TestBoldLabel1() {
      string queryLower = "albncr";

      HasteItem item = new HasteItem("Apples/Bananas/Carrots", 0, "");
      int[] boundaryIndices = HasteStringUtils.GetBoundaryIndices(item.Path);
      int[] indices;
      HasteStringUtils.GetMatchIndices(item.PathLower, queryLower, boundaryIndices, out indices);

      string bolded = HasteStringUtils.BoldLabel(item.Path, indices, "[", "]");
      string expected = "[A]pp[l]es/[B]a[n]anas/[C]a[r]rots";
      Assert.That(bolded, Is.EqualTo(expected));
    }

    [Test]
    [Category("BoldLabel")]
    public void TestBoldLabel2() {
      string queryLower = "upr";

      HasteItem item = new HasteItem("Unity Test Tools/Platform Runner/Run on platform", 0, "");
      int[] boundaryIndices = HasteStringUtils.GetBoundaryIndices(item.Path);
      int[] indices;
      HasteStringUtils.GetMatchIndices(item.PathLower, queryLower, boundaryIndices, out indices);

      string bolded = HasteStringUtils.BoldLabel(item.Path, indices, "[", "]");
      string expected = "[U]nity Test Tools/[P]latform [R]unner/Run on platform";
      Assert.That(bolded, Is.EqualTo(expected));
    }

    [Test]
    [Category("BoldLabel")]
    public void TestBoldLabel3() {
      string queryLower = "mc";

      HasteItem item = new HasteItem("Component/Physics/Mesh Collider", 0, "");
      int[] boundaryIndices = HasteStringUtils.GetBoundaryIndices(item.Path);
      int[] indices;
      HasteStringUtils.GetMatchIndices(item.PathLower, queryLower, boundaryIndices, out indices);

      string bolded = HasteStringUtils.BoldLabel(item.Path, indices, "[", "]");
      string expected = "Component/Physics/[M]esh [C]ollider";
      Assert.That(bolded, Is.EqualTo(expected));
    }
  }
}
