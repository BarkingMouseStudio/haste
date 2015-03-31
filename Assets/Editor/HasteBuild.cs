using UnityEngine;
using UnityEditor;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CSharp;

namespace Haste {

  internal static class HasteBuild {

    public static readonly string SOURCE_PATH = "Assets/Haste/Editor/";
    public static readonly string INTERNAL_RESOURCES_PATH = "Assets/Haste/Editor/InternalResources";
    public static readonly string ANCILLARY_PATH = "Assets/Ancillary";

    public static string UnityEnginePath {
      get {
        return Path.Combine(EditorApplication.applicationContentsPath, "Frameworks/Managed/UnityEngine.dll");
      }
    }

    public static string UnityEditorPath {
      get {
        return Path.Combine(EditorApplication.applicationContentsPath, "Frameworks/Managed/UnityEditor.dll");
      }
    }

    public static void LogErrors(this CompilerResults result) {
      if (result.Errors.Count > 0) {
        foreach (var error in result.Errors) {
          Debug.LogError(error.ToString());
        }
      }
    }

    public static string[] GetSource(string path) {
      var source = new List<string>();

      foreach (var file in Directory.GetFiles(path, "*.cs")) {
        source.Add(File.ReadAllText(file));
      }

      foreach (var dir in Directory.GetDirectories(path)) {
        if (!dir.StartsWith(INTERNAL_RESOURCES_PATH)) {
          source.AddRange(GetSource(dir));
        }
      }

      return source.ToArray();
    }

    public static CompilerResults BuildAssembly(string[] sources, string dest, string compilerOptions = "") {
      var compileParams = new CompilerParameters();
      compileParams.OutputAssembly = dest;
      compileParams.CompilerOptions = compilerOptions;
      compileParams.ReferencedAssemblies.Add(UnityEnginePath);
      compileParams.ReferencedAssemblies.Add(UnityEditorPath);

      var codeProvider = new CSharpCodeProvider(new Dictionary<string, string>{ { "CompilerVersion", "v3.0" } });
      return codeProvider.CompileAssemblyFromSource(compileParams, sources);
    }

    public static string CreateDirectory(string parent, string path) {
      return Directory.CreateDirectory(Path.Combine(parent, path)).FullName;
    }

    public static void CopyContentsRecursively(string src, string dest) {
      DirectoryInfo dir = new DirectoryInfo(src);

      if (!Directory.Exists(dest)) {
        Directory.CreateDirectory(dest);
      }

      foreach (FileInfo file in dir.GetFiles()) {
        if (file.Extension == ".meta") {
          continue; // Ignore meta files
        }

        if (file.Name.StartsWith(".")) {
          continue; // Ignore hidden files
        }

        file.CopyTo(Path.Combine(dest, file.Name));
      }

      foreach (DirectoryInfo subdir in dir.GetDirectories()) {
        if (subdir.Name.StartsWith(".")) {
          continue; // Ignore hidden files
        }

        CopyContentsRecursively(subdir.FullName, Path.Combine(dest, subdir.Name));
      }
    }

    public static void ExportHasteFree(string rootPath, string[] source) {
      var exportPath = Path.Combine(rootPath, "HasteFree");
      FileUtil.DeleteFileOrDirectory(exportPath);

      // Create folders
      var topPath = CreateDirectory(exportPath, "Haste");
      var editorPath = CreateDirectory(topPath, "Editor");

      // Build dll
      BuildAssembly(source, Path.Combine(editorPath, "Haste.dll")).LogErrors();

      // Copy ancillary folder
      CopyContentsRecursively(ANCILLARY_PATH, topPath);

      // Copy internal resources folder
      var internalResourcesPath = Path.Combine(editorPath, "InternalResources");
      FileUtil.ReplaceDirectory(INTERNAL_RESOURCES_PATH, internalResourcesPath);
    }

    public static void ExportHastePro(string rootPath, string[] source, string sourcePackagePath) {
      var exportPath = Path.Combine(rootPath, "HastePro");
      FileUtil.DeleteFileOrDirectory(exportPath);

      // Create folders
      var topPath = CreateDirectory(exportPath, "Haste");
      var editorPath = CreateDirectory(topPath, "Editor");

      // Build dll
      BuildAssembly(source, Path.Combine(editorPath, "Haste.dll"), "/optimize /define:IS_HASTE_PRO").LogErrors();

      // Copy ancillary folder
      CopyContentsRecursively(ANCILLARY_PATH, topPath);

      // Copy internal resources folder
      var internalResourcesPath = Path.Combine(editorPath, "InternalResources");
      FileUtil.ReplaceDirectory(INTERNAL_RESOURCES_PATH, internalResourcesPath);

      // Copy Haste Pro source
      var destSourcePackagePath = Path.Combine(topPath, "HasteProSource.unitypackage");
      FileUtil.ReplaceFile(sourcePackagePath, destSourcePackagePath);
    }

    public static string ExportHasteProSource(string rootPath) {
      var sourcePackagePath = Path.Combine(rootPath, "HasteProSource.unitypackage");
      FileUtil.DeleteFileOrDirectory(sourcePackagePath);

      AssetDatabase.ExportPackage("Assets/Haste", sourcePackagePath,
        ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);

      return sourcePackagePath;
    }

    [MenuItem("Window/Export Haste")]
    public static void ExportHaste() {
      var rootPath = EditorUtility.SaveFolderPanel("Export Haste", "", "");
      if (string.IsNullOrEmpty(rootPath)) {
        Debug.LogError("Export path required");
        return;
      }

      var sourcePackagePath = ExportHasteProSource(rootPath);
      var source = GetSource(SOURCE_PATH);
      ExportHasteFree(rootPath, source);
      ExportHastePro(rootPath, source, sourcePackagePath);

      Debug.Log("Done.");
    }
  }
}
