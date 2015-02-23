using UnityEngine;
using UnityEditor;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using Microsoft.CSharp;

namespace Haste {

  public static class HasteBuild {

    public static readonly string SOURCE_PATH = "Assets/Haste/Editor/";
    public static readonly string INTERNAL_RESOURCES_PATH = "Assets/Haste/Editor/InternalResources/";

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

    public static string[] GetSource(string buildPath) {
      var source = new List<string>();

      foreach (var file in Directory.GetFiles(buildPath, "*.cs")) {
        source.Add(File.ReadAllText(file));
      }

      foreach (var dir in Directory.GetDirectories(buildPath)) {
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

    [MenuItem("Window/Build Haste")]
    public static void BuildHaste() {
      var source = GetSource(SOURCE_PATH);

      var dest = EditorUtility.SaveFilePanel("Build Haste Free", "", "HasteFree.dll", "dll");
      if (dest.Length != 0) {
        BuildAssembly(source, dest, "").LogErrors();
      }

      dest = EditorUtility.SaveFilePanel("Build Haste Pro", "", "HastePro.dll", "dll");
      if (dest.Length != 0) {
        BuildAssembly(source, dest, "/optimize /define:IS_HASTE_PRO").LogErrors();
      }
    }
  }
}
