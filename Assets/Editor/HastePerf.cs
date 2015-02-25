using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Haste {

  internal static class HastePerf {

    public static readonly string[] WORDS = new string[]{"Unicorn", "Ant", "Ape", "Bat", "Bear", "Bee", "Bison", "Boar", "Camel", "Cat", "Clam", "Cobra", "Cod", "Crab", "Crane", "Crow", "Deer", "Dog", "Dove", "Duck", "Eagle", "Eel", "Eland", "Elk", "Emu", "Finch", "Fish", "Fly", "Fox", "Frog", "Gaur", "Gnat", "Gnu", "Goat", "Goose", "Gull", "Hare", "Hawk", "Heron", "Horse", "Human", "Hyena", "Ibex", "Ibis", "Jay", "Koala", "Kudu", "Lark", "Lemur", "Lion", "Llama", "Loris", "Louse", "Mink", "Mole", "Moose", "Mouse", "Mule", "Newt", "Okapi", "Oryx", "Otter", "Owl", "Pig", "Pony", "Quail", "Rail", "Ram", "Rat", "Raven", "Rook", "Seal", "Shark", "Sheep", "Shrew", "Skunk", "Snail", "Snake", "Squid", "Stork", "Swan", "Tapir", "Tiger", "Toad", "Trout", "Viper", "Wasp", "Whale", "Wolf", "Worm", "Wren", "Yak", "Zebra"};

    public static string GetRandomName() {
      StringBuilder nameBuilder = new StringBuilder();
      string word;
      for (int i = 0; i < Random.Range(1, 3); i++) {
        word = WORDS[Random.Range(0, WORDS.Length - 1)];
        nameBuilder.Append(word);
      }
      return nameBuilder.ToString();
    }

    public static string GetRandomPath() {
      StringBuilder pathBuilder = new StringBuilder();
      for (int i = 0; i < Random.Range(0, 2); i++) {
        pathBuilder.Append(GetRandomName()).Append(Path.DirectorySeparatorChar);
      }
      pathBuilder.Append(GetRandomName());
      return pathBuilder.ToString();
    }

    [MenuItem("Window/Populate GameObjects")]
    public static void PopulateGameObjects() {
      Stack<GameObject> parents = new Stack<GameObject>();
      parents.Push(new GameObject(GetRandomName()));

      float parentProbability = 0.4f;
      int count = 10000;

      GameObject go;
      for (var i = 0; i < count; i++) {
        go = new GameObject(GetRandomName());
        go.transform.parent = parents.Peek().transform;

        if (Random.value < parentProbability) {
          parents.Push(go);
        } else if (parents.Count > 1) {
          parents.Pop();
        }
      }
    }

    public static string CreateFolder(string name, string parent = "Assets") {
      return AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(parent, name));
    }

    [MenuItem("Window/Populate Assets")]
    public static void PopulateAssets() {
      Stack<string> folders = new Stack<string>();
      folders.Push(CreateFolder(GetRandomName(), "Assets"));

      float folderProbability = 0.4f;
      int count = 100;

      string folder;
      for (var i = 0; i < count; i++) {
        folder = CreateFolder(GetRandomName(), folders.Peek());

        if (Random.value < folderProbability) {
          folders.Push(folder);
        } else if (folders.Count > 1) {
          folders.Pop();
        }
      }
    }
  }
}
