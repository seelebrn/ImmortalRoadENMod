using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using System.Xml.Linq;
using UnhollowerRuntimeLib;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using BepInEx.IL2CPP.Utils.Collections;
using System.Collections.Generic;
using System.Drawing;
using static Il2CppSystem.Globalization.TimeSpanFormat;
using System.Security.AccessControl;
using System;
using Il2CppSystem.Reflection;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using Il2CppSystem;
using UnhollowerBaseLib.Runtime.VersionSpecific.AssemblyName;
using Il2CppSystem.Linq;
using UnhollowerBaseLib;
using Il2CppSystem.Linq.Expressions;
using TranslationENMOD;

namespace TranslationENMOD
{
    internal class Cleaning
    {
        public static void Init()
        {
           

           
            var path = Path.Combine(BepInEx.Paths.PluginPath, "Dump", "UITextKV.txt");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var path2 = Path.Combine(BepInEx.Paths.PluginPath, "Dump", "TALV.txt");
            if (File.Exists(path2))
            {
                File.Delete(path2);
            }
           
        }
    }
}
