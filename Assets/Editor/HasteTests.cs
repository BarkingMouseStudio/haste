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

      queryLower = "acl";
      IHasteResult g = new HasteResult(new HasteItem("Assets/Create/Lens Flare", 0, ""), queryLower, queryLower.Length);
      IHasteResult h = new HasteResult(new HasteItem("GameObject/Align With View", 0, ""), queryLower, queryLower.Length);
      Assert.That(comparer.Compare(g, h), Is.EqualTo(-1));
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
    public void TestGetFileNameWithoutExtension() {
      Assert.That(HasteStringUtils.GetFileNameWithoutExtension("/"), Is.EqualTo(""));
      Assert.That(HasteStringUtils.GetFileNameWithoutExtension("/."), Is.EqualTo(""));
      Assert.That(HasteStringUtils.GetFileNameWithoutExtension("this/is/a/test/"), Is.EqualTo("test"));
      Assert.That(HasteStringUtils.GetFileNameWithoutExtension("this/is/a/test/."), Is.EqualTo(""));
      Assert.That(HasteStringUtils.GetFileNameWithoutExtension("test"), Is.EqualTo("test"));
      Assert.That(HasteStringUtils.GetFileNameWithoutExtension("test."), Is.EqualTo("test"));
      Assert.That(HasteStringUtils.GetFileNameWithoutExtension("test.cs"), Is.EqualTo("test"));
      Assert.That(HasteStringUtils.GetFileNameWithoutExtension("my/file/is/a/test"), Is.EqualTo("test"));
      Assert.That(HasteStringUtils.GetFileNameWithoutExtension("my/file/is/a/test.cs"), Is.EqualTo("test"));
      Assert.That(HasteStringUtils.GetFileNameWithoutExtension("te.mp/test"), Is.EqualTo("test"));
      Assert.That(HasteStringUtils.GetFileNameWithoutExtension("temp./test"), Is.EqualTo("test"));
      Assert.That(HasteStringUtils.GetFileNameWithoutExtension("te.mp/test.cs"), Is.EqualTo("test"));
      Assert.That(HasteStringUtils.GetFileNameWithoutExtension("temp./test.cs"), Is.EqualTo("test"));
    }

    [Test]
    public void TestGetExtension() {
      Assert.That(HasteStringUtils.GetExtension("/"), Is.EqualTo(""));
      Assert.That(HasteStringUtils.GetExtension("/."), Is.EqualTo(""));
      Assert.That(HasteStringUtils.GetExtension("this/is/a/test/"), Is.EqualTo(""));
      Assert.That(HasteStringUtils.GetExtension("this/is/a/test/."), Is.EqualTo(""));
      Assert.That(HasteStringUtils.GetExtension("test"), Is.EqualTo(""));
      Assert.That(HasteStringUtils.GetExtension("test."), Is.EqualTo(""));
      Assert.That(HasteStringUtils.GetExtension("test.cs"), Is.EqualTo("cs"));
      Assert.That(HasteStringUtils.GetExtension("my/file/is/a/test"), Is.EqualTo(""));
      Assert.That(HasteStringUtils.GetExtension("my/file/is/a/test.cs"), Is.EqualTo("cs"));
      Assert.That(HasteStringUtils.GetExtension("te.mp/test"), Is.EqualTo(""));
      Assert.That(HasteStringUtils.GetExtension("temp./test"), Is.EqualTo(""));
      Assert.That(HasteStringUtils.GetExtension("te.mp/test.cs"), Is.EqualTo("cs"));
      Assert.That(HasteStringUtils.GetExtension("temp./test.cs"), Is.EqualTo("cs"));
    }

    [Test]
    public void TestGetFileName() {
      Assert.That(HasteStringUtils.GetFileName("/"), Is.EqualTo(""));
      Assert.That(HasteStringUtils.GetFileName("this/is/a/test/"), Is.EqualTo("test"));
      Assert.That(HasteStringUtils.GetFileName("test"), Is.EqualTo("test"));
      Assert.That(HasteStringUtils.GetFileName("test.cs"), Is.EqualTo("test.cs"));
      Assert.That(HasteStringUtils.GetFileName("my/file/is/a/test"), Is.EqualTo("test"));
      Assert.That(HasteStringUtils.GetFileName("my/file/is/a/test.cs"), Is.EqualTo("test.cs"));
      Assert.That(HasteStringUtils.GetFileName("te.mp/test"), Is.EqualTo("test"));
      Assert.That(HasteStringUtils.GetFileName("temp./test"), Is.EqualTo("test"));
      Assert.That(HasteStringUtils.GetFileName("te.mp/test.cs"), Is.EqualTo("test.cs"));
      Assert.That(HasteStringUtils.GetFileName("temp./test.cs"), Is.EqualTo("test.cs"));
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

    void TestBoldLabel(string path, string query, string expected) {
      string queryLower = query.ToLowerInvariant();

      HasteItem item = new HasteItem(path, 0, "");
      int[] boundaryIndices = HasteStringUtils.GetBoundaryIndices(item.Path);
      int[] indices = HasteStringUtils.GetWeightedSubsequence(item.PathLower, queryLower, boundaryIndices);

      string bolded = HasteStringUtils.BoldLabel(item.Path, indices, "[", "]");
      Assert.That(bolded, Is.EqualTo(expected));
    }

    [Test]
    [Category("BoldLabel")]
    public void TestBoldLabel1() {
      TestBoldLabel("Apples/Bananas/Carrots", "albncr",
        "[A]pp[l]es/[B]a[n]anas/[C]a[r]rots");
    }

    [Test]
    [Category("BoldLabel")]
    public void TestBoldLabel2() {
      TestBoldLabel("Unity Test Tools/Platform Runner/Run on platform", "upr",
        "[U]nity Test Tools/[P]latform [R]unner/Run on platform");
    }

    [Test]
    [Category("BoldLabel")]
    public void TestBoldLabel3() {
      TestBoldLabel("Component/Physics/Mesh Collider", "mc",
        "Component/Physics/[M]esh [C]ollider");
    }

    [Test]
    [Category("BoldLabel")]
    public void TestBoldLabel4() {
      TestBoldLabel("Apples/Bananas/Carrots", "albncr",
        "[A]pp[l]es/[B]a[n]anas/[C]a[r]rots");
    }

    [Test]
    [Category("BoldLabel")]
    public void TestBoldLabel5() {
      TestBoldLabel("Abples/Bananas/Cherribs", "abc",
        "[A]bples/[B]ananas/[C]herribs");
    }

    [Test]
    [Category("BoldLabel")]
    public void TestBoldLabel6() {
      TestBoldLabel("Abples/Bananas/Cherribs", "abbc",
        "[A][b]ples/[B]ananas/[C]herribs");
    }

    [Test]
    [Category("BoldLabel")]
    public void TestBoldLabel7() {
      TestBoldLabel("Abples/Bananas/Cherribs", "abac",
        "[A]bples/[B][a]nanas/[C]herribs");
    }

    [Test]
    [Category("BoldLabel")]
    public void TestBoldLabel8() {
      TestBoldLabel("Abples/Bananas/Cherribs", "bp",
        "A[b][p]les/Bananas/Cherribs");
    }

    [Test]
    [Category("BoldLabel")]
    public void TestBoldLabel9() {
      TestBoldLabel("Abcles/Abales/Cherries", "abac",
        "[A][b]cles/[A]bales/[C]herries");
    }

    [Test]
    [Category("BoldLabel")]
    public void TestBoldLabel10() {
      TestBoldLabel("Assets/UnityTestTools/Common/Editor/icons/rerun-lighttheme.png", "rop",
        "Assets/UnityTestTools/Common/Edito[r]/ic[o]ns/rerun-lighttheme.[p]ng");
    }

    [Test]
    [Category("BoldLabel")]
    public void TestBoldLabel11() {
      TestBoldLabel("Assets/Haste/Editor/Extensions/ArrayExtensions.cs", "A/E.cs",
        "[A]ssets[/]Haste/[E]ditor/Extensions/ArrayExtensions[.][c][s]");
    }
  }
}
