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

    public static CompilerResults BuildAssembly(string[] sources, string dest, string compilerOptions) {
      var compileParams = new CompilerParameters();
      compileParams.OutputAssembly = dest;
      compileParams.CompilerOptions = compilerOptions;
      compileParams.ReferencedAssemblies.Add(UnityEnginePath);
      compileParams.ReferencedAssemblies.Add(UnityEditorPath);

      var codeProvider = new CSharpCodeProvider(new Dictionary<string, string>{ { "CompilerVersion", "v3.0" } });
      return codeProvider.CompileAssemblyFromSource(compileParams, sources);
    }

    public static string CreateFolder(string parent, string path) {
      return Directory.CreateDirectory(Path.Combine(parent, path)).FullName;
    }

    public static void ExportPackage(string exportPath, string[] source, string name, string compilerOptions) {
      // Create folders
      var editorPath = CreateFolder(CreateFolder(exportPath, "Haste"), "Editor");

      // Build dll
      BuildAssembly(source, Path.Combine(editorPath, "Haste.dll"), compilerOptions).LogErrors();

      // Copy internal resources folder
      var internalResourcesPath = Path.Combine(editorPath, "InternalResources");
      FileUtil.CopyFileOrDirectory(INTERNAL_RESOURCES_PATH, internalResourcesPath);
    }

    [MenuItem("Window/Export Haste")]
    public static void ExportHaste() {
      var rootPath = EditorUtility.SaveFolderPanel("Export Haste", "", "");
      if (string.IsNullOrEmpty(rootPath)) {
        Debug.LogError("Export path required");
        return;
      }

      var source = GetSource(SOURCE_PATH);
      ExportPackage(Path.Combine(rootPath, "HasteFree"), source, "HasteFree", "");
      ExportPackage(Path.Combine(rootPath, "HastePro"), source, "HastePro", "/optimize /define:IS_HASTE_PRO");
      AssetDatabase.ExportPackage("Assets/Haste",
        Path.Combine(rootPath, String.Format("HasteSource.unitypackage")),
        ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
    }
  }
}
