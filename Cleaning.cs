using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using System.Xml.Linq;
using UnhollowerRuntimeLib;
using UnityEngine;
using Sirenix.Serialization;
using System.Collections;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using BepInEx.IL2CPP.Utils.Collections;
using System.Collections.Generic;
using System.Drawing;
using HappyTall;
using static Il2CppSystem.Globalization.TimeSpanFormat;
using System.Security.AccessControl;
using System;
using Il2CppSystem.Reflection;
using Sirenix.Serialization.Utilities;
using static ServerMail;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using Il2CppSystem;
using UnhollowerBaseLib.Runtime.VersionSpecific.AssemblyName;
using ParadoxNotion;
using Il2CppSystem.Linq;
using ParadoxNotion.Serialization.FullSerializer;
using UnhollowerBaseLib;
using Il2CppSystem.Linq.Expressions;
using TranslationENMOD;
using static MapIndex;
using static PlayerData;

namespace TranslationENMOD
{
    internal class Cleaning
    {
        public static void Init()
        {
            var a = Path.Combine(BepInEx.Paths.GameRootPath, "DecryptedBundles");
            var b = Path.Combine(BepInEx.Paths.GameRootPath, "TranslatedBundles");
            var c = Path.Combine(BepInEx.Paths.GameRootPath, "FinalBundles");
            if (!Directory.Exists(a))
            {
                Directory.CreateDirectory(a);
            }
            if (!Directory.Exists(b))
            {
                Directory.CreateDirectory(b);
            }
            if (!Directory.Exists(c))
            {
                Directory.CreateDirectory(c);
            }
            var dir = new DirectoryInfo(Path.Combine(BepInEx.Paths.GameRootPath, "DecryptedBundles")).GetFiles();
            var dir2 = new DirectoryInfo(Path.Combine(BepInEx.Paths.GameRootPath, "TranslatedBundles")).GetFiles();
            var dir3 = new DirectoryInfo(Path.Combine(BepInEx.Paths.GameRootPath, "FinalBundles")).GetFiles();
            var datadir = new DirectoryInfo(Application.dataPath).GetFiles();
            foreach (var file in dir)
            {
                File.Delete(file.FullName);
            }
            foreach (var file in dir2)
            {
                File.Delete(file.FullName);
            }
            foreach (var file in dir3)
            {
                File.Delete(file.FullName);
            }
            var path = Path.Combine(BepInEx.Paths.PluginPath, "Dump", "result.txt");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var path2 = Path.Combine(BepInEx.Paths.PluginPath, "Dump", "result.txt");
            if (File.Exists(path2))
            {
                File.Delete(path2);
            }
            var path3 = Path.Combine(BepInEx.Paths.PluginPath, "Dump", "etcUN.txt");
            if (File.Exists(path2))
            {
                File.Delete(path2);
            }
            string newdialogsdictfile = Path.Combine(BepInEx.Paths.PluginPath, "Translations", "NewKV - DONOTUSEWITHALREADYMODDEDASSETS", "dialogs.txt");

            string newresultDictfile = Path.Combine(BepInEx.Paths.PluginPath, "Translations", "NewKV - DONOTUSEWITHALREADYMODDEDASSETS", "result.txt");
            if (File.Exists(newresultDictfile))
            {
                File.Delete(newresultDictfile);
            }
            if (File.Exists(newdialogsdictfile))
            {
                File.Delete(newdialogsdictfile);
            }
        }
        public static void CleanAfterDone()
        {
            var a = new DirectoryInfo(Path.Combine(BepInEx.Paths.GameRootPath, "DecryptedBundles"));
            var b = new DirectoryInfo(Path.Combine(BepInEx.Paths.GameRootPath, "TranslatedBundles"));
            foreach(var file in a.GetFiles())
            {
                file.Delete();
            }
            foreach (var file in b.GetFiles())
            {
                file.Delete();
            }
            a.Delete();
            b.Delete();

        }
    }
}
