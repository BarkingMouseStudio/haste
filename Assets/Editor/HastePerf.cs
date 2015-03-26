using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Haste {

  internal static class HastePerf {

    public static readonly string[] WORDS = new string[]{"Adjule", "Agogwe", "Ahool", "Akkorokamui", "Almas", "Altamaha-ha", "Ameranthropoides loysi", "Amomongo", "Andean Wolf", "Appalachian Black Panther", "Aswang", "Atti", "Ayia Napa sea monster", "Barmanou", "Batutut", "Beaman", "Bear Lake Monster", "Beast of Bladenboro", "Beast of Bodmin", "Beast of Bray Road", "Beast of Busco", "Beast of Dartmoor", "Beast of Dean", "Beast of Exmoor", "Beast of Gévaudan", "Bergman's bear", "Bessie", "Bigfoot", "Black Shuck", "Bownessie", "British big cats", "Brosno dragon", "Bukit Timah Monkey Man", "Bunyip", "Burmese gray wild dog", "Buru", "Cadborosaurus willsi", "Canvey Island Monster", "Cardiff Giant", "Champ", "Cherufe", "Chessie (sea monster)", "Chickcharney", "Chuchunya", "Chupacabra", "Con Rit", "Dingonek", "Devil Bird", "Dobhar-chú", "Dover Demon", "Eastern Cougar", "Ebu Gogo", "Elasmotherium", "Elmendorf Beast", "Elwedritsche", "Emela-ntouka", "Enfield Monster", "Ennedi tiger", "Fear liath", "Fiskerton Phantom", "Flatwoods monster", "Flying rod", "Fouke Monster", "Fur-bearing trout", "Garou", "Gazeka", "Gambo", "Ghost deer", "Giant anaconda", "Giglioli's Whale", "Globster", "Gloucester Sea serpent", "Gnome of Gerona", "Goatman", "Grassman", "Gunni", "Grootslang", "Hakawai", "Hellhound", "Hibagon", "High-finned sperm whale", "Hodag", "Hokkaidō Wolf", "Homo gardarensis", "Honey Island Swamp monster", "Honshū wolf", "Hoop snake", "Huay Chivo", "Hyote", "Igopogo", "Iliamna Lake Monster", "Inkanyamba", "Isshii", "Ivory-billed woodpecker", "J'ba fofi", "Jackalope", "Jersey Devil", "Kaijin", "Kappa", "Kawekaweau", "Kelpie", "Kikiyaon ", "Kingstie", "Kongamato", "Koolakamba", "Kraken", "Kting Voar", "Kumi Lizard/Ngarara", "Kusshii", "Lagarfljót Worm", "Lake Tianchi Monster", "Lake Van Monster", "Lake Worth monster", "Lariosauro", "Lizard Man of Scape Ore Swamp", "Loch Ness Monster", "Loveland Frog", "Lusca", "MacFarlane's Bear", "Maero", "Mahamba", "Maltese Tiger", "Mamlambo", "Manananggal", "Manatee of Helena", "Mande Barung", "Man-eating tree", "Manipogo", "Mapinguari", "Maricoxi", "Marozi", "Mbielu-Mbielu-Mbielu", "Megalania prisca", "Megalodon", "Melon heads", "Memphre", "Menehune", "Mermaid", "Merman", "Michigan dogman", "Minhocão", "Minnesota Iceman", "Mitla", "Mngwa", "Moa", "Moehau", "Mogollon Monster", "Mokele-Mbembe", "Momo the Monster", "Mongolian Death Worm", "Monkey-man of Delhi", "Mono Grande", "Montauk Monster", "Morag", "Mothman", "Mountain Fennec", "Muckie", "Muc-sheilch", "Muhuru", "Mussie", "Monster of Monterey", "Nahuelito", "Nandi Bear", "Ndendeki", "Ngoima ", "Ngoubou", "Nguma-monene", "Ogopogo", "Old Yellow Top", "Olitiau", "Onza", "Orang-Bati", "Orang Mawas", "Orang Pendek", "Owlman", "Ozark Howler", "\"Panthera tigris sudanensis\"", "Peluda", "Phantom cat", "Phantom kangaroo", "Phaya Naga", "Pogeyan", "Popobawa", "Pope Lick Monster", "Poukai", "Pukwudgie", "Pygmy Gorilla", "Pygmy Elephant", "Qilin", "Queensland Tiger", "Rake", "Reptilians", "Ropen", "Row", "Salawa", "Sea monk", "Sea monsters", "Sea serpent", "Selma", "Sewer alligator", "Sharlie", "Shōjō", "Shug Monkey", "Shunka Warakin", "Sigbin", "Sirrush", "Skunk Ape", "Spring-heeled Jack", "Steller's Sea Ape", "Storsjöodjuret", "Stronsay Beast", "Sucuriju Gigante", "Tahoe Tessie", "Takitaro", "Tapire-iauara", "Tatzelwurm", "Thetis Lake monster", "Thunderbird", "Thylacine", "Tikbalang", "Trinity Alps giant salamander", "Trunko", "Tsuchinoko", "Tsul 'Kalu", "Turtle Lake Monster", "Umdhlebi", "Urayuli", "Veo", "Waheela", "Waitoreke", "Wampus Cat", "Wendigo", "Wild Man of the Navidad", "Wog", "Wolpertinger", "Wucharia", "Ya-te-veo", "Yeren", "Yeti", "Yowie", "Zanzibar Leopard", "Zuiyō-maru creature"};

    public static string GetRandomName() {
      return WORDS[Random.Range(0, WORDS.Length - 1)];
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
