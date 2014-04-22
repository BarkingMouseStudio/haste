using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

namespace Haste {

  public class ProjectIndex : AbstractIndex {

    private FileSystemWatcher watcher;

    private object queueLock = new object();
    private Queue<string> assetsAdded;
    private Queue<string> assetsRemoved;

    public ProjectIndex() {
      assetsAdded = new Queue<string>();
      assetsRemoved = new Queue<string>();

      EditorApplication.update += Update;

      watcher = new FileSystemWatcher();
      watcher.Path = Application.dataPath;
      watcher.NotifyFilter = NotifyFilters.LastAccess |
        NotifyFilters.LastWrite |
        NotifyFilters.FileName |
        NotifyFilters.DirectoryName;
      watcher.Filter = "";
      watcher.Created += new FileSystemEventHandler(OnAssetCreated);
      watcher.Deleted += new FileSystemEventHandler(OnAssetDeleted);
      watcher.Renamed += new RenamedEventHandler(OnAssetRenamed);
      watcher.Error += new ErrorEventHandler(OnError);
      watcher.IncludeSubdirectories = true;
      watcher.EnableRaisingEvents = true;
    }

    void OnError(object source, ErrorEventArgs e) {
      Debug.LogException(e.GetException());
    }

    void OnAssetCreated(object source, FileSystemEventArgs e) {
      lock (queueLock) {
        assetsAdded.Enqueue(e.FullPath);
      }
    }

    void OnAssetDeleted(object source, FileSystemEventArgs e) {
      lock (queueLock) {
        assetsRemoved.Enqueue(e.FullPath);
      }
    }

    void OnAssetRenamed(object source, RenamedEventArgs e) {
      lock (queueLock) {
        assetsRemoved.Enqueue(e.OldFullPath);
        assetsAdded.Enqueue(e.FullPath);
      }
    }

    int clock = 0;

    void Update() {
      if (clock % 100 == 0) {
        lock (queueLock) {
          if (assetsRemoved.Count > 0) {
            Logger.Info("Removing", assetsRemoved.Count);
            foreach (string assetPath in assetsRemoved) {
              RemoveAsset(assetPath);
            }
            assetsRemoved.Clear();
          }

          if (assetsAdded.Count > 0) {
            Logger.Info("Adding", assetsRemoved.Count);
            foreach (string assetPath in assetsAdded) {
              AddAsset(assetPath);
            }
            assetsAdded.Clear();
          }
        }
      }

      clock++;
    }

    void AddAsset(string assetPath) {
      if (Path.GetExtension(assetPath) == ".meta") {
        return; // Ignore meta files
      }

      string assetName = Path.GetFileName(assetPath);

      if (assetName.StartsWith(".")) {
        return; // Ignore hidden files
      }

      string relativePath = Utils.GetRelativeAssetPath(assetPath);

      AddItem(new Item(assetName, relativePath, Source.Project, AssetDatabase.GetCachedIcon(relativePath)));
    }

    void RemoveAsset(string assetPath) {
      if (Path.GetExtension(assetPath) == ".meta") {
        return; // Ignore meta files
      }

      string assetName = Path.GetFileName(assetPath);

      if (assetName.StartsWith(".")) {
        return; // Ignore hidden files
      }

      RemoveItem(Utils.GetRelativeAssetPath(assetPath));
    }

    public void TraverseFiles(string path) {
      try {
        foreach (string assetPath in Directory.GetFiles(path)) {
          AddAsset(assetPath);
        }
      } catch (Exception e) {
        Debug.LogException(e);
      }
    }

    public void TraverseDirectories(string path) {
      try {
        foreach (string assetPath in Directory.GetDirectories(path)) {
          AddAsset(assetPath);
          TraversePath(assetPath);
        }
      } catch (Exception e) {
        Debug.LogException(e);
      }
    }

    public void TraversePath(string path) {
      TraverseFiles(path);
      TraverseDirectories(path);
    }

    public override void Rebuild() {
      index.Clear();
      TraversePath(Application.dataPath);
    }
  }
}
