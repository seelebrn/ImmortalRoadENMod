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

using System.Security.AccessControl;
using System;
using Il2CppSystem.Reflection;
using Sirenix.Serialization.Utilities;

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
using BepInEx.Unity.IL2CPP.Utils.Collections;
using UnhollowerBaseLib.Runtime;
using UnityEngine.SceneManagement;
using NodeCanvas.Tasks.Actions;
using Sirenix.OdinInspector;
using System.Runtime.InteropServices;
//using static MapIndex;
using Microsoft.CSharp;
//using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using System.Linq;
using System.Dynamic;
using Il2CppSystem.Xml.Schema;
using static MapIndex;

namespace UVExtractor.Il2Cpp
{

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource log = new ManualLogSource("EN"); // The source name is shown in BepInEx log
        public static System.Collections.Generic.List<string> contents = new System.Collections.Generic.List<string>();
        public static System.Collections.Generic.List<string> processed = new System.Collections.Generic.List<string>();
        public static string UVPTranslatorPath = Path.Combine(BepInEx.Paths.PluginPath, "Dump");
        public static string sourceDir = BepInEx.Paths.PluginPath;
        public static string fileDir = Application.dataPath;
        public static List<string> meshes = new List<string>();
        public static System.Collections.Generic.List<string> texts = new List<string>();
        public static System.Collections.Generic.List<string> textcontent = new System.Collections.Generic.List<string>();
        public static System.Collections.Generic.List<string> meshescontent = new System.Collections.Generic.List<string>();
        public static System.Collections.Generic.List<string> items = new System.Collections.Generic.List<string>();

        public static Dictionary<string, string> resultDict = new Dictionary<string, string>();
        public static Dictionary<string, string> dialogsDict = new Dictionary<string, string>();
        public static Dictionary<string, string> etcDict = new Dictionary<string, string>();
        public static Dictionary<string, string> translationDict = new Dictionary<string, string>();
        public static Dictionary<string, string> newresultDict = new Dictionary<string, string>();
        public static Dictionary<string, string> newdialogsDict = new Dictionary<string, string>();
        public static string[] forbidden = new string[]
        {
        "animations",
        "audios",
        "font1",
        "font2",
        "fonts"
        };
        public static string[] assets = new string[]
{
        "resources.assets",
        "level0",
        "level1",
        "sharedassets0.assets",
        "sharedassets1.assets",
        "globalgamemanagers.assets"

};
        public static bool clean = false;
        public static Il2CppSystem.Collections.Generic.List<Il2CppSystem.Reflection.FieldInfo> fields = new Il2CppSystem.Collections.Generic.List<Il2CppSystem.Reflection.FieldInfo>();

        public static Dictionary<string, string> FileToDictionary(string dir)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            IEnumerable<string> lines = File.ReadLines(Path.Combine(sourceDir, "Translations", dir));

            foreach (string line in lines)
            {

                var arr = line.Split('¤');
                if (arr[0] != arr[1])
                {
                    var pair = new KeyValuePair<string, string>(Regex.Replace(arr[0], @"\t|\n|\r", ""), arr[1]);

                    if (!dict.ContainsKey(pair.Key))
                        dict.Add(pair.Key, pair.Value);
                    else
                        Debug.Log($"Found a duplicated line while parsing {dir}: {pair.Key}");
                }
            }

            return dict;

        }
        public static void LoadPersonUI()
        {




        }
        public override void Load()
        {

            BepInEx.Logging.Logger.Sources.Add(log);
            var etcUn = Path.Combine(BepInEx.Paths.PluginPath, "Dump", "etcUN.txt");
            if (File.Exists(etcUn))
            {
                File.Delete(etcUn);
            }
            var Config = File.ReadAllLines(Path.Combine(BepInEx.Paths.PluginPath, "config.txt"));

            if (Config.Contains("Clean=true")) { clean = true; }
            if (Config.Contains("Dump=false"))
            {
                log.LogInfo("Switch Dump set to false, not dumping anything");
            }
            if (Config.Contains("Dump=true"))
            {
                log.LogInfo("Initializing Dumping process...");
                Cleaning.Init();



                resultDict = FileToDictionary("result.txt");
                dialogsDict = FileToDictionary("dialogs.txt");
                etcDict = FileToDictionary("etc.txt");
                //translationDict = resultDict.Concat(dialogsDict.Where(x => !resultDict.Keys.Contains(x.Key))).ToDictionary(k => k.Key, v => v.Value);
                translationDict = resultDict.MergeLeft(dialogsDict);
                BepInEx.Logging.Logger.Sources.Add(log);
                Write();
                var dir = new DirectoryInfo(Path.Combine(BepInEx.Paths.GameRootPath, "DecryptedBundles")).GetFiles();
                var dir2 = new DirectoryInfo(Path.Combine(BepInEx.Paths.GameRootPath, "TranslatedBundles")).GetFiles();
                var dir3 = new DirectoryInfo(Path.Combine(BepInEx.Paths.GameRootPath, "FinalBundles")).GetFiles();
                var datadir = new DirectoryInfo(Application.dataPath).GetFiles();

                var path = Path.Combine(BepInEx.Paths.PluginPath, "Dump", "result.txt");

                var path2 = Path.Combine(BepInEx.Paths.PluginPath, "Dump", "result.txt");
                foreach (var x in dir)
                {
                    Plugin.log.LogInfo("Now dumping strings from AssetBundle : " + x.FullName);
                    if (!x.FullName.Contains("resS") && !x.FullName.Contains("manifest") && !x.FullName.EndsWith("\\pc"))
                    {

                        Dump.TranslateBundle(x.FullName, Application.dataPath);

                    }
                    Plugin.log.LogInfo("Done dumping strings from AssetBundle : " + x.FullName + "... Phew !");
                }



                Unwrite();
                string newdialogsdictfile = Path.Combine(BepInEx.Paths.PluginPath, "Translations", "NewKV - DONOTUSEWITHALREADYMODDEDASSETS", "dialogs.txt");
                string newresultDictfile = Path.Combine(BepInEx.Paths.PluginPath, "Translations", "NewKV - DONOTUSEWITHALREADYMODDEDASSETS", "result.txt");
                using (StreamWriter sw = new StreamWriter(newresultDictfile, append: true))
                {
                    Plugin.log.LogInfo("Now writing new results KV in : " + newresultDictfile);


                    foreach (var kvp in newresultDict)
                    {
                        sw.Write(kvp.Key + "¤" + kvp.Value + System.Environment.NewLine);

                    }

                }

                using (StreamWriter sw = new StreamWriter(newdialogsdictfile, append: true))
                {
                    Plugin.log.LogInfo("Now writing new dialogs KV in : " + newdialogsdictfile);


                    foreach (var kvp in newdialogsDict)
                    {
                        sw.Write(kvp.Key + "¤" + kvp.Value + System.Environment.NewLine);
                    }

                }
                if (clean == true)
                {
                    Cleaning.CleanAfterDone();
                }
            }

            AddComponent<mbmb>();
            resultDict = FileToDictionary("result.txt");
            dialogsDict = FileToDictionary("dialogs.txt");
            etcDict = FileToDictionary("etc.txt");
            //translationDict = resultDict.Concat(dialogsDict.Where(x => !resultDict.Keys.Contains(x.Key))).ToDictionary(k => k.Key, v => v.Value);
            translationDict = resultDict.MergeLeft(dialogsDict);
            Plugin.log.LogInfo("Running Harmony Patches...");
            var harmony = new Harmony("Cadenza.GAME.ENMOD");
            Harmony.CreateAndPatchAll(System.Reflection.Assembly.GetExecutingAssembly(), null);






        }


        internal static class MyPluginInfo
        {
            public const string PLUGIN_GUID = "Example.Plugin";
            public const string PLUGIN_NAME = "My first plugin";
            public const string PLUGIN_VERSION = "1.0.0";
        }



        public static void Write()
        {
            string assetPath0 = $"{Paths.GameRootPath}\\灵墟_Data";
            string assetPath = $"{Paths.GameRootPath}\\灵墟_Data\\StreamingAssets";
            DirectoryInfo x = new DirectoryInfo((Path.Combine(Paths.GameRootPath, "DecryptedBundles")));
            foreach (var file in x.GetFiles())
            {
                File.Delete(x.FullName);
            }
            foreach (string assetFile in Directory.GetFiles(assetPath))
            {
                if (!assetFile.Contains("manifest"))
                {
                    byte key = 0x40;
                    byte[] byteArr = File.ReadAllBytes(assetFile);
                    for (int i = 0; i < byteArr.Length; i++)
                    {
                        byteArr[i] = (byte)(byteArr[i] ^ key);
                    }

                    File.WriteAllBytes(Path.Combine(Paths.GameRootPath, "DecryptedBundles", Path.GetFileName(assetFile)), byteArr);
                }
            }

        }
        public static void Unwrite()
        {
            string assetPath0 = $"{Paths.GameRootPath}\\灵墟_Data";
            string assetPath = $"{Paths.GameRootPath}\\TranslatedBundles";
            DirectoryInfo x = new DirectoryInfo((Path.Combine(Paths.GameRootPath, "FinalBundles")));
            foreach (var file in x.GetFiles())
            {
                File.Delete(x.FullName);
            }
            foreach (string assetFile in Directory.GetFiles(assetPath))
            {
                if (!assetFile.Contains("manifest"))
                {
                    byte key = 0x40;
                    byte[] byteArr = File.ReadAllBytes(assetFile);
                    for (int i = 0; i < byteArr.Length; i++)
                    {
                        byteArr[i] = (byte)(key ^ byteArr[i]);
                    }

                    File.WriteAllBytes(Path.Combine(Paths.GameRootPath, "FinalBundles", Path.GetFileName(assetFile)), byteArr);
                }
            }

        }

    }
    class mbmb : MonoBehaviour
    {
        public static List<string> processed = new List<string>();
        public mbmb(System.IntPtr handle) : base(handle) { }

        private void Update()
        {

        }
    }



    public static class Helpers
    {
        public static string AddItemToList(string str)
        {

            var list = new System.Collections.Generic.List<string>();
            var path = Path.Combine(BepInEx.Paths.PluginPath, "Dump", "etcUN.txt");
            if (File.Exists(path))
            {
                list = File.ReadAllLines(path).ToList();
            }
            else
            {

                list = new System.Collections.Generic.List<string>();
            }
            if (Plugin.etcDict.ContainsKey(Helpers.CustomEscape(str)))
            {
                //Plugin.log.LogInfo("Found Match : " + Helpers.CustomUnescape(Plugin.etcDict[Helpers.CustomEscape(str)]));
                return Helpers.CustomUnescape(Plugin.etcDict[Helpers.CustomEscape(str)]);
            }
            else
            {

                using (StreamWriter sw = new StreamWriter(path, append: true))
                {

                    if (!list.Contains(Helpers.CustomEscape(str)) && str != null)
                    {
                        if (Helpers.IsChinese(str))
                        {

                            sw.Write(Helpers.CustomEscape(str) + System.Environment.NewLine);
                            list.Add(Helpers.CustomEscape(str));

                        }
                    }

                }
                return null;
            }
        }
        public static readonly Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
        public static bool IsChinese(string s)
        {
            if (s != null && s != "")
            {
                return cjkCharRegex.IsMatch(s);
            }
            else return false;
        }

        public static Dictionary<string, string> LoadFileToDictionary(string dir)
        {

            Dictionary<string, string> dict = new Dictionary<string, string>();

            IEnumerable<string> lines = File.ReadLines(Path.Combine(Plugin.sourceDir, "Translations", dir));

            foreach (string line in lines)
            {

                var arr = line.Split('¤');
                if (arr[0] != arr[1])
                {
                    var pair = new KeyValuePair<string, string>(Regex.Replace(arr[0], @"\t|\n|\r", ""), arr[1]);

                    if (!dict.ContainsKey(pair.Key))
                        dict.Add(pair.Key, pair.Value);
                    else
                        Debug.Log($"Found a duplicated line while parsing {dir}: {pair.Key}");


                }
            }

            return dict;

            //return File.ReadLines(Path.Combine(BepInEx.Paths.PluginPath, "Translations", dir))
            //    .Select(line =>
            //    {
            //        var arr = line.Split('¤');
            //        return new KeyValuePair<string, string>(Regex.Replace(arr[0], @"\t|\n|\r", ""), arr[1]);
            //    })
            //    .GroupBy(kvp => kvp.Key)
            //    .Select(x => x.First())
            //    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, comparer);
        }

        public static string CustomEscape(string s)
        {
            return s.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
        }
        public static string CustomUnescape(string s)
        {
            return s.Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t");
        }
    }
    public class utils
    {
        public static List<string> PopulateTexts(JObject jObject)
        {
            List<string> list = new List<string>();
            try
            {
                foreach (JProperty jProperty in ((JToken)jObject))
                {
                    JToken jTokenValue = jProperty.Value;
                    ProcessToken(jTokenValue, list);
                }
                return list;
            }
            catch (System.Exception ex)
            {
                Plugin.log.LogError(ex);
                return null;
            }
        }

        public static void ProcessToken(JToken jTokenValue, List<string> list)
        {
            switch (jTokenValue.Type)
            {
                case JTokenType.String:
                    JValue value = (JValue)jTokenValue;
                    if (value.Value is string text && Helpers.IsChinese(text))
                    {
                        list.Add(text);
                    }
                    break;
                case JTokenType.Object:
                    PopulateTexts((JObject)jTokenValue);
                    break;
                case JTokenType.Array:
                    foreach (JToken arrayItem in ((JArray)jTokenValue))
                    {
                        ProcessToken(arrayItem, list);
                    }
                    break;
            }
        }
        public static Il2CppSystem.Type GetILType<T>()
        {
            return UnhollowerRuntimeLib.Il2CppType.Of<T>();
        }
    }
    [HarmonyPatch]

    class Patch_Util
    {

        static IEnumerable<System.Reflection.MethodBase> TargetMethods()
        {
            var methods = new List<string>();
            IList<string> names = new List<string>();


            yield return AccessTools.Method(typeof(Util), "ToChinese");
            yield return AccessTools.Method(typeof(Util), "StringToUnicode");
            yield return AccessTools.Method(typeof(Util), "UnicodeToString");
            //yield return AccessTools.Method(typeof(Util), "ToString");
            yield return AccessTools.Method(typeof(Util), "GetWWWStreamFile");
            yield return AccessTools.Method(typeof(Util), "ConvertSidText");
            yield return AccessTools.Method(typeof(Util), "MakeConditionStrColor");
            yield return AccessTools.Method(typeof(Util), "ConvertDataLength");
            yield return AccessTools.Method(typeof(Util), "ConvertSidText");
            yield return AccessTools.Method(typeof(Util), "HttpSyncGet");

        }
        static void Postfix(Util __instance, ref string __result)
        {

            Plugin.log.LogInfo("Patch_Util : " + Regex.Unescape(__result));


        }
    }
    /*[HarmonyPatch]
    class Patch_Util3
    {

        static IEnumerable<System.Reflection.MethodBase> TargetMethods()
        {
            var methods = new List<string>();
            IList<string> names = new List<string>();

            foreach(var x in typeof(Config).GetNestedTypes())
            {
                foreach(var n in typeof(Config).GetMethods().Where(y => y.ReturnType == typeof(string)).Where(z => !z.Name.Contains("get_") && !z.Name.Contains("ToString")))
                {
                    yield return n;
                }
                foreach(var m in x.GetMethods().Where(y => y.ReturnType == typeof(string)).Where(z => !z.Name.Contains("get_") && !z.Name.Contains("ToString")))
                {
                    Plugin.log.LogInfo("Count = " + x.GetMethods().Where(y => y.ReturnType == typeof(string)).Where(z => !z.Name.Contains("get_") && !z.Name.Contains("ToString")).Count());
                    yield return m;
                }
            }
           
        }

        static void Postfix(Config __instance, ref string __result)
        {
            if(Helpers.IsChinese(__result))
            { 
            Plugin.log.LogInfo("Patch_Config3 : " + __result);
            }



        }
    }*/
    /*[HarmonyPatch]
    class Patch_Util4
    {

        static IEnumerable<System.Reflection.MethodBase> TargetMethods()
        {
            var methods = new List<string>();
            IList<string> names = new List<string>();


            yield return AccessTools.Method(typeof(Util), "GetResourceInfo");
        }

        static void Postfix(Util __instance, ref ResourcesIndex.ReUnit __result)
        {

            Plugin.log.LogInfo("Patch_Util4 : " + __result.name);
            Plugin.log.LogInfo("Patch_Util4 : " + __result.ext);
            Plugin.log.LogInfo("Patch_Util4 : " + __result.nm);




        }
    }*/
    //Quests names, desc and a few other things.
    [HarmonyPatch]
    class Patch_Util5
    {
        static IEnumerable<System.Reflection.MethodBase> TargetMethods()
        {
            var methods = new List<string>();
            IList<string> names = new List<string>();


            yield return AccessTools.Method(typeof(ResourceControl.__c), "_LoadBundleAndAssetsAsync_b__23_19");
        }


        static void Postfix(ResourceControl.__c __instance, ref TaskUnit __result)
        {
            try
            {
                foreach (var x in __result.Steps)
                {
                    //Plugin.log.LogInfo(x.Dec);
                    //Plugin.log.LogInfo(x.DecFull);
                    //Plugin.log.LogInfo(x.StepName);

                    if (Helpers.AddItemToList(x.Dec) != null)
                    {
                        x.Dec = Helpers.AddItemToList(x.Dec);
                    }
                    if (Helpers.AddItemToList(x.DecFull) != null)
                    {
                        x.DecFull = Helpers.AddItemToList(x.DecFull);
                    }
                    if (Helpers.AddItemToList(x.StepName) != null)
                    {
                        x.StepName = Helpers.AddItemToList(x.StepName);
                    }

                }
            }
            catch (System.Exception e)
            {
                //Plugin.log.LogInfo(e);
            }


        }

    }
    //Tiles names and a few other things.
    [HarmonyPatch]
    class Patch_Util6
    {
        static IEnumerable<System.Reflection.MethodBase> TargetMethods()
        {
            var methods = new List<string>();
            IList<string> names = new List<string>();


            yield return AccessTools.Method(typeof(MapControl), "AddPlaceNode");
        }


        static void Prefix(MapControl __instance, ref PlaceUnit pu)
        {
            try
            {

                if (Plugin.etcDict.ContainsKey(pu.place_name))
                {
                    pu.place_name = Plugin.etcDict[pu.place_name];
                }
                else
                {
                    Helpers.AddItemToList(pu.place_name);
                }
                if (Plugin.etcDict.ContainsKey(pu.Ftext_in))
                {
                    pu.Ftext_in = Plugin.etcDict[pu.Ftext_in];
                }
                else
                {
                    Helpers.AddItemToList(pu.Ftext_in);
                }
                if (Plugin.etcDict.ContainsKey(pu.Ftext_out))
                {
                    pu.Ftext_out = Plugin.etcDict[pu.Ftext_out];
                }
                else
                {
                    Helpers.AddItemToList(pu.Ftext_out);
                }


            }
            catch
            {

            }


        }

    }

    [HarmonyPatch(typeof(Config), "GetRanDomMapData")]
    static class Patchxx
    {
        static void Postfix(Config __instance)
        {
            foreach (var s in __instance.Assos)
            {
                if (Helpers.AddItemToList(s.name) != null)
                {
                    s.name = Helpers.AddItemToList(s.name);
                }
                if (Helpers.AddItemToList(s.dec) != null)
                {
                    s.name = Helpers.AddItemToList(s.dec);
                }

            }
            foreach (var s in __instance.CzDataList)
            {
                if (Helpers.AddItemToList(s.strDec) != null)
                {
                    s.strDec = Helpers.AddItemToList(s.strDec);
                }
                if (Helpers.AddItemToList(s.strName) != null)
                {
                    s.strName = Helpers.AddItemToList(s.strName);
                }
                if (Helpers.AddItemToList(s.strPir) != null)
                {
                    s.strPir = Helpers.AddItemToList(s.strPir);
                }

            }
            for (int i = 0; i < __instance.SEDecText.Count; i++)
            {
                if (Helpers.AddItemToList(__instance.SEDecText[i]) != null)
                {
                    __instance.SEDecText[i] = Helpers.AddItemToList(__instance.SEDecText[i]);
                }


            }
            for (int i = 0; i < __instance.SEDecText.Count; i++)
            {
                if (Helpers.AddItemToList(__instance.SEDecText[i]) != null)
                {
                    __instance.SEDecText[i] = Helpers.AddItemToList(__instance.SEDecText[i]);
                }

            }
        }
    }
    [HarmonyPatch]
    class Patch_Util8
    {

        static string StringTest(string str)
        {
            try

            {
                if (str != null && str != "")
                {
                    if (Helpers.IsChinese(str))
                    {
                        Plugin.log.LogInfo(str);
                        if (Plugin.etcDict.ContainsKey(str))
                        {
                            str = Plugin.etcDict[str];
                            Plugin.log.LogInfo("Match Found : " + str);

                        }
                        else
                        {
                            Helpers.AddItemToList(str);
                            Plugin.log.LogInfo("Adding to Untranslated...: " + str);

                        }

                    }
                    else
                    {

                    }
                }
                return str;
            }
            catch
            {
                return str;
            }

        }
        static IEnumerable<System.Reflection.MethodBase> TargetMethods()
        {
            var methods = new List<string>();
            IList<string> names = new List<string>();


            yield return AccessTools.Method(typeof(ResourceControl.__c), "_LoadBundleAndAssetsAsync_b__23_20");
        }


        static void Postfix(ResourceControl.__c __instance, ref Role __result, ref UnityEngine.Object input)
        {
            Plugin.log.LogInfo("aaa : " + input.GetIl2CppType().FullName + " // " + input.name);

            var b = __result.KungFuTuiJian;
            foreach (var a in b)
            {

                var a2 = a.KungFuList;
                try
                {
                    foreach (var b2 in a2)
                    {
                        try
                        {
                            foreach (var b3 in b2.acSkill)
                            {
                                try { b3.skillName = StringTest(b3.skillName); Plugin.log.LogInfo("b2.acSkillb3.skillName" + b3.skillName); } catch { }
                                try
                                {
                                    foreach (var b4 in b3.Lv_effectList)
                                    {
                                        try { b4.Dec = StringTest(b4.Dec); Plugin.log.LogInfo("b2.acSkillb4.Dec" + b4.Dec); } catch { }

                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        catch
                        {

                        }
                        try
                        {
                            foreach (var b3 in b2.ptSkill)
                            {
                                try { b3.skillName = StringTest(b3.skillName); Plugin.log.LogInfo("b2.ptSkillb3.skillName" + b3.skillName); } catch { }
                                try
                                {
                                    foreach (var b4 in b3.LearnAwards.List)
                                    {
                                        //Plugin.log.LogInfo("bb4.AwardName : " + b4.AwardName);
                                        try { b4.AwardTypeName = StringTest(b4.AwardTypeName); Plugin.log.LogInfo("b2.ptSkillb4.AwardTypeName" + b4.AwardTypeName); } catch { }
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                        catch
                        {

                        }


                    }
                }
                catch
                {

                }
            }





            var d = __result.MesData;
            try
            {
                foreach (var a1 in d)
                {
                    //Plugin.log.LogInfo("MesData => a1.Dialog : " + a1.Dialog);
                }
            }
            catch
            {

            }
            var e = __result.PlotList;
            try
            {
                foreach (var a1 in e)
                {
                    try { a1.Dec = StringTest(a1.Dec); Plugin.log.LogInfo("ea1.Dec" + a1.Dec); } catch { }
                    try { a1.TitleS = StringTest(a1.TitleS); Plugin.log.LogInfo("ea1.TitleS" + a1.TitleS); } catch { }
                    try { a1.aimplace._Map = StringTest(a1.aimplace._Map); Plugin.log.LogInfo("ea1.aimplace._Map" + a1.aimplace._Map); } catch { }
                }
            }
            catch
            {

            }
            var f = __result.Reladata;
            try
            {
                foreach (var a1 in f)
                {
                    try { a1.NID = StringTest(a1.NID); Plugin.log.LogInfo("fa1.NID" + a1.NID); } catch { }
                    for (int i = 0; i < a1.NID.Count(); i++)
                    {
                        try { a1.Fulls[i] = StringTest(a1.Fulls[i]); Plugin.log.LogInfo("fa1.Fulls[i]" + a1.Fulls[i]); } catch { }
                    }

                }
            }
            catch
            {

            }
            var g = __result.KungConfigFus;

            try
            {
                foreach (var a1 in g)
                {
                    //Plugin.log.LogInfo("KungConnfigFus => a1.KungFu.name : " + a1.KungFu.name);
                    //Plugin.log.LogInfo("KungConnfigFus => a1.KungFu.Dec : " + a1.KungFu.Dec);
                    //Plugin.log.LogInfo("KungConnfigFus => a1.KungFu.speedDec : " + a1.KungFu.speedDec);
                    //Plugin.log.LogInfo("KungConnfigFus => a1.KungFu.Tip : " + a1.KungFu.Tip);
                    try { a1.KungFu.name = StringTest(a1.KungFu.name); Plugin.log.LogInfo("a1.KungFu.namea2.skillName" + a1.KungFu.name); } catch { }

                    try { a1.KungFu.Dec = StringTest(a1.KungFu.Dec); Plugin.log.LogInfo("a1.KungFu.Deca2.skillName" + a1.KungFu.Dec); } catch { }

                    try { a1.KungFu.speedDec = StringTest(a1.KungFu.speedDec); Plugin.log.LogInfo("a1.KungFu.speedDeca2.skillName" + a1.KungFu.speedDec); } catch { }

                    try { a1.KungFu.Tip = StringTest(a1.KungFu.Tip); Plugin.log.LogInfo("a1.KungFu.Tipa2.skillName" + a1.KungFu.Tip); } catch { }

                    try { a1.KungFu.serializationData.SerializedBytesString = StringTest(a1.KungFu.serializationData.SerializedBytesString); Plugin.log.LogInfo("a1.KungFu.serializationData.SerializedBytesStringa2.skillName" + a1.KungFu.serializationData.SerializedBytesString); } catch { }

                    try { __result.RoleSortDec = StringTest(__result.RoleSortDec); Plugin.log.LogInfo("g__result.RoleSortDec" + __result.RoleSortDec); } catch { }
                    try
                    {
                        foreach (var a2 in a1.KungFu.LvUpFightData.buffList)
                        {
                            try { a2.dec = StringTest(a2.dec); Plugin.log.LogInfo("g__result.RoleSortDec" + a2.dec); } catch { }
                            try
                            {
                                a2.name = StringTest(a2.name); Plugin.log.LogInfo("g__result.RoleSortDec" + a2.name);
                                try { a2.groupName = StringTest(a2.groupName); Plugin.log.LogInfo("g__result.RoleSortDec" + a2.groupName); } catch { }
                                try { a2.groupName = StringTest(a2.groupName); Plugin.log.LogInfo("g__result.RoleSortDec" + a2.groupName); } catch { }
                                try
                                {
                                    try { a2.fightBuffInfo.fightbuff.name = StringTest(a2.fightBuffInfo.fightbuff.name); Plugin.log.LogInfo("a2.fightBuffInfo.fightbuff.name__result.RoleSortDec" + a2.fightBuffInfo.fightbuff.name); } catch { }
                                    try { a2.fightBuffInfo.name = StringTest(a2.fightBuffInfo.name); Plugin.log.LogInfo("a2.fightBuffInfo.name__result.RoleSortDec" + a2.fightBuffInfo.name); } catch { }
                                    try { a2.fightBuffInfo.fightbuff.dec = StringTest(a2.fightBuffInfo.fightbuff.dec); Plugin.log.LogInfo("a2.fightBuffInfo.fightbuff.dec__result.RoleSortDec" + a2.fightBuffInfo.fightbuff.dec); } catch { }
                                }
                                catch
                                {

                                }
                            }
                            catch { }
                        }
                    }

                    catch { }

                    try
                    {
                        foreach (var a2 in a1.KungFu.LvUpFightData.buffList2)
                        {
                            a2.dec = StringTest(a2.dec); Plugin.log.LogInfo("g__result.RoleSortDec" + a2.dec);
                            a2.name = StringTest(a2.name); Plugin.log.LogInfo("g__result.RoleSortDec" + a2.name);
                            a2.groupName = StringTest(a2.groupName); Plugin.log.LogInfo("g__result.RoleSortDec" + a2.groupName);

                        }
                    }
                    catch { }

                    try
                    {
                        foreach (var a2 in a1.KungFu.ptSkill)
                        {
                            try { a2.skillName = StringTest(a2.skillName); Plugin.log.LogInfo("a1.KungFu.ptSkilla2.skillName" + a2.skillName); } catch { }
                            foreach (var x in a2.LearnAwards.List)
                            {
                                try { x.AwardName = StringTest(x.AwardName); Plugin.log.LogInfo("x.AwardNamea2.skillName" + x.AwardName); } catch { }
                                try { x.AwardTypeName = StringTest(x.AwardTypeName); Plugin.log.LogInfo("x.AwardTypeNamea2.skillName" + x.AwardTypeName); } catch { }
                                try { x.CurrAwardName = StringTest(x.CurrAwardName); Plugin.log.LogInfo("x.CurrAwardNamea2.skillName" + x.CurrAwardName); } catch { }
                            }
                        }
                        try
                        {
                            foreach (var a3 in a1.KungFu.baseFightData.buffList)
                            {
                                try { a3.name = StringTest(a3.name); Plugin.log.LogInfo("a3.namea2.skillName" + a3.name); } catch { }
                                try { a3.name = StringTest(a3.dec); Plugin.log.LogInfo("a3.deca2.skillName" + a3.dec); } catch { }
                                try { a3.name = StringTest(a3.serializationData.SerializedBytesString); Plugin.log.LogInfo("serializationData.SerializedBytesStringa2.skillName" + a3.dec); } catch { }
                                try { a3.name = StringTest(a3.fightBuffInfo.name); Plugin.log.LogInfo("a3.fightBuffInfo.namea2.skillName" + a3.fightBuffInfo.name); } catch { }
                                try { a3.name = StringTest(a3.fightBuffInfo.fightbuff.name); Plugin.log.LogInfo("a3.fightBuffInfo.fightbuff.namea2.skillName" + a3.name); } catch { }
                                try { a3.name = StringTest(a3.fightBuffInfo.fightbuff.dec); Plugin.log.LogInfo("a3.fightBuffInfo.fightbuff.deca2.skillName" + a3.name); } catch { }
                            }
                        }
                        catch
                        {

                        }

                    }
                    catch
                    {

                    }
                    try
                    {
                        foreach (var a2 in a1.KungFu.acSkill)
                        {
                            try { a2.skillName = StringTest(a2.skillName); Plugin.log.LogInfo("a1.KungFu.acSkilla2.skillName" + a2.skillName); } catch { }
                            try
                            {
                                foreach (var a2bis in a2.Lv_effectList)
                                {
                                    try { a2bis.Dec = StringTest(a2bis.Dec); Plugin.log.LogInfo("a1.KungFu.acSkilla2bis.Dec" + a2bis.Dec); } catch { }


                                }
                                foreach (var a3bis in a2.condition.Conditions)
                                {
                                    foreach (var a4bis in a3bis.List)
                                    {
                                        try { a4bis.AwardName = StringTest(a4bis.AwardName); Plugin.log.LogInfo("a4bis.AwardNamea2bis.Dec" + a4bis.AwardName); } catch { }
                                        try { a4bis.AwardTypeName = StringTest(a4bis.AwardTypeName); Plugin.log.LogInfo("a4bis.AwardTypeNamea2bis.Dec" + a4bis.AwardTypeName); } catch { }
                                        try { a4bis.CurrAwardName = StringTest(a4bis.CurrAwardName); Plugin.log.LogInfo("a4bis.CurrAwardNamea2bis.Dec" + a4bis.CurrAwardName); } catch { }

                                    }
                                }
                            }
                            catch
                            {

                            }
                            try
                            {
                                foreach (var a3 in a2.LearnAwards.List)
                                {
                                    //Plugin.log.LogInfo("bb4.AwardName : " + b4.AwardName);
                                    try { a3.AwardName = StringTest(a3.AwardName); Plugin.log.LogInfo("a2.LearnAwards.Lista3.AwardName" + a3.AwardName); } catch { }
                                    try { a3.AwardTypeName = StringTest(a3.AwardTypeName); Plugin.log.LogInfo("a2.LearnAwards.Lista3.AwardTypeName" + a3.AwardTypeName); } catch { }

                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                    catch
                    {

                    }
                    try
                    {
                        foreach (var b2 in a1.KungFu.TuPoCostData)
                        {
                            try
                            {
                                foreach (var b3 in b2.LevelUpAwards.List)
                                {
                                    //Plugin.log.LogInfo("KungConnfigFus => b2.LevelUpAwards.List.AwardName : " + b3.AwardName);
                                    try { b3.AwardTypeName = StringTest(b3.AwardTypeName); Plugin.log.LogInfo("b2.LevelUpAwards.Listb3.AwardTypeName" + b3.AwardTypeName); } catch { }
                                    //Plugin.log.LogInfo("KungConnfigFus => b2.LevelUpAwards.List.CurrAwardName : " + b3.CurrAwardName);
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                    catch
                    {

                    }

                }
            }
            catch
            {

            }




            var h = __result.ResHunyin;
            try
            {
                foreach (var a1 in h.List)
                {

                    try { a1.AwardName = StringTest(a1.AwardName); Plugin.log.LogInfo("h.Lista1.AwardName" + a1.AwardName); } catch { }
                    try { a1.AwardTypeName = StringTest(a1.AwardTypeName); Plugin.log.LogInfo("h.Lista1.AwardTypeName" + a1.AwardTypeName); } catch { }
                    try { a1.CurrAwardName = StringTest(a1.CurrAwardName); Plugin.log.LogInfo("h.Lista1.CurrAwardName" + a1.CurrAwardName); } catch { }


                }
            }
            catch
            {

            }




            var j = __result.SoulList;
            try
            {
                foreach (var a1 in j)
                {
                    try { a1.Sname = StringTest(a1.Sname); Plugin.log.LogInfo("ja1.Sname" + a1.Sname); } catch { }
                    foreach (var a2 in a1.su)
                    {
                        try { a2.Name = StringTest(a2.Name); Plugin.log.LogInfo("a1.sua2.Name" + a2.Name); } catch { }
                        try { a2.Dec = StringTest(a2.Dec); Plugin.log.LogInfo("a1.sua2.Dec" + a2.Dec); } catch { }
                        try { a2.Pdata.skillName = StringTest(a2.Pdata.skillName); Plugin.log.LogInfo("a2.Pdata.skillName" + a2.Pdata.skillName); } catch { }
                        try { a2.Pdata.Dec = StringTest(a2.Pdata.Dec); Plugin.log.LogInfo("a2.Pdata.Dec" + a2.Pdata.Dec); } catch { }

                        try
                        {
                            foreach (var a3 in a2.Bless.List)
                            {
                                try { a3.AwardName = StringTest(a3.AwardName); Plugin.log.LogInfo("a2.Bless.Lista3.AwardName" + a3.AwardName); } catch { }
                                try { a3.AwardTypeName = StringTest(a3.AwardTypeName); Plugin.log.LogInfo("a2.Bless.Lista3.AwardTypeName" + a3.AwardTypeName); } catch { }
                                try { a3.CurrAwardName = StringTest(a3.CurrAwardName); Plugin.log.LogInfo("a2.Bless.Lista3.CurrAwardName" + a3.CurrAwardName); } catch { }

                            }
                        }
                        catch { }
                        try
                        {
                            foreach (var a4 in a2.Sdata.buffList)
                            {
                                try { a4.name = StringTest(a4.name); Plugin.log.LogInfo("a2.Sdata.buffLista4.name" + a4.name); } catch { }
                                try { a4.dec = StringTest(a4.dec); Plugin.log.LogInfo("a2.Sdata.buffLista4.dec" + a4.dec); } catch { }
                                try { a4.fightBuffInfo.name = StringTest(a4.fightBuffInfo.name); Plugin.log.LogInfo("a2.Sdata.buffLista4.fightBuffInfo.name" + a4.fightBuffInfo.name); } catch { }
                                try { a4.fightBuffInfo.fightbuff.Name = StringTest(a4.fightBuffInfo.fightbuff.Name); Plugin.log.LogInfo("a2.Sdata.buffLista4.fightBuffInfo.fightbuff.Name" + a4.fightBuffInfo.fightbuff.Name); } catch { }
                                try { a4.fightBuffInfo.fightbuff.name = StringTest(a4.fightBuffInfo.fightbuff.name); Plugin.log.LogInfo("a2.Sdata.buffLista4.fightBuffInfo.fightbuff.name" + a4.fightBuffInfo.fightbuff.name); } catch { }
                                try { a4.fightBuffInfo.fightbuff.dec = StringTest(a4.fightBuffInfo.fightbuff.dec); Plugin.log.LogInfo("a2.Sdata.buffLista4.fightBuffInfo.fightbuff.dec" + a4.fightBuffInfo.fightbuff.dec); } catch { }
                                try { a4.fightBuffInfo.fightbuff.groupName = StringTest(a4.fightBuffInfo.fightbuff.groupName); Plugin.log.LogInfo("a2.Sdata.buffLista4.fightBuffInfo.fightbuff.groupName" + a4.fightBuffInfo.fightbuff.groupName); } catch { }
                            }
                        }
                        catch
                        {

                        }
                        try
                        {
                            foreach (var a5 in a2.Sdata.buffList2)
                            {
                                try { a5.name = StringTest(a5.name); Plugin.log.LogInfo("a2.Sdata.buffList2a5.name" + a5.name); } catch { }
                                try { a5.dec = StringTest(a5.dec); Plugin.log.LogInfo("a2.Sdata.buffList2a5.dec" + a5.dec); } catch { }
                                try { a5.fightBuffInfo.name = StringTest(a5.fightBuffInfo.name); Plugin.log.LogInfo("a2.Sdata.buffList2a5.fightBuffInfo.name" + a5.fightBuffInfo.name); } catch { }
                                try { a5.fightBuffInfo.fightbuff.Name = StringTest(a5.fightBuffInfo.fightbuff.Name); Plugin.log.LogInfo("a2.Sdata.buffList2a5.fightBuffInfo.fightbuff.Name" + a5.fightBuffInfo.fightbuff.Name); } catch { }
                                try { a5.fightBuffInfo.fightbuff.name = StringTest(a5.fightBuffInfo.fightbuff.name); Plugin.log.LogInfo("a2.Sdata.buffList2a5.fightBuffInfo.fightbuff.name" + a5.fightBuffInfo.fightbuff.name); } catch { }
                                try { a5.fightBuffInfo.fightbuff.dec = StringTest(a5.fightBuffInfo.fightbuff.dec); Plugin.log.LogInfo("a2.Sdata.buffList2a5.fightBuffInfo.fightbuff.dec" + a5.fightBuffInfo.fightbuff.dec); } catch { }
                                try { a5.fightBuffInfo.fightbuff.groupName = StringTest(a5.fightBuffInfo.fightbuff.groupName); Plugin.log.LogInfo("a2.Sdata.buffList2a5.fightBuffInfo.fightbuff.groupName" + a5.fightBuffInfo.fightbuff.groupName); } catch { }
                            }
                        }
                        catch
                        {

                        }
                        try
                        {
                            foreach (var a6 in a2.Sdata.passiveList)
                            {
                                try { a6.psd.skillName = StringTest(a6.psd.skillName); Plugin.log.LogInfo("a2.Sdata.passiveLista6.psd.skillName" + a6.psd.skillName); } catch { }
                                try { a6.psd.Dec = StringTest(a6.psd.Dec); Plugin.log.LogInfo("a2.Sdata.passiveLista6.psd.Dec" + a6.psd.Dec); } catch { }
                            }
                        }
                        catch { }






                    }


                }
            }
            catch
            {

            }


            foreach (var a in __result.ngConditionList)
            {
                foreach (var a2 in a.List)
                {
                    try { a2.AwardName = StringTest(a2.AwardName); Plugin.log.LogInfo("ngConditionLista2.AwardName" + a2.AwardName); } catch { }
                    try { a2.AwardTypeName = StringTest(a2.AwardTypeName); Plugin.log.LogInfo("a2.AwardTypeName" + a2.AwardTypeName); } catch { }
                    try { a2.CurrAwardName = StringTest(a2.CurrAwardName); Plugin.log.LogInfo("a2.CurrAwardName" + a2.CurrAwardName); } catch { }
                }
            }
            foreach (var a in __result.pgConditionList)
            {
                foreach (var a2 in a.List)
                {
                    try { a2.AwardName = StringTest(a2.AwardName); Plugin.log.LogInfo("pgConditionLista2.AwardName" + a2.AwardName); } catch { }
                    try { a2.AwardTypeName = StringTest(a2.AwardTypeName); Plugin.log.LogInfo("pgConditionLista2.AwardTypeName" + a2.AwardTypeName); } catch { }
                    try { a2.CurrAwardName = StringTest(a2.CurrAwardName); Plugin.log.LogInfo("pgConditionLista2.CurrAwardName" + a2.CurrAwardName); } catch { }
                }
            }
            foreach (var a in __result.wgConditionList)
            {
                foreach (var a2 in a.List)
                {
                    try { a2.AwardName = StringTest(a2.AwardName); Plugin.log.LogInfo("wgConditionLista2.AwardName" + a2.AwardName); } catch { }
                    try { a2.AwardTypeName = StringTest(a2.AwardTypeName); Plugin.log.LogInfo("wgConditionLista2.AwardTypeName" + a2.AwardTypeName); } catch { }
                    try { a2.CurrAwardName = StringTest(a2.CurrAwardName); Plugin.log.LogInfo("wgConditionLista2.CurrAwardName" + a2.CurrAwardName); } catch { }
                }
            }

            foreach (var a in __result.serializationData.SerializationNodes)
            {
                try { a.Data = StringTest(a.Data.ToString()); Plugin.log.LogInfo("a.Data" + a.Data); } catch { }
            }
            /*
            var list = __result._transform.GetComponents<UnityEngine.UI.Text>().ToList();
            list.AddRange(__result._transform.GetComponentsInChildren<UnityEngine.UI.Text>(true));*/


        }

    }

    [HarmonyPatch]
    static class PatchResourcex
    {
        static string StringTest(string str)
        {
            try

            {
                if (str != null && str != "")
                {
                    if (Helpers.IsChinese(str))
                    {
                        Plugin.log.LogInfo(str);
                        if (Plugin.etcDict.ContainsKey(str))
                        {
                            str = Plugin.etcDict[str];
                            Plugin.log.LogInfo("Match Found : " + str);

                        }
                        else
                        {
                            Helpers.AddItemToList(str);
                            Plugin.log.LogInfo("Adding to Untranslated...: " + str);

                        }

                    }
                    else
                    {

                    }
                }
                return str;
            }
            catch
            {
                return str;
            }

        }
        static IEnumerable<System.Reflection.MethodBase> TargetMethods()
        {
            var methods = new List<string>();
            IList<string> names = new List<string>();

            yield return AccessTools.Method(typeof(GameElement), "getPSByIndex");
        }
        static void Postfix(GameElement __instance, ref passiveSkillData __result)
        {
            Plugin.log.LogInfo("Patchx : " + __result.Dec);
        }
    }
    public static class DictionaryExtensions
    {
        // Works in C#3/VS2008:
        // Returns a new dictionary of this ... others merged leftward.
        // Keeps the type of 'this', which must be default-instantiable.
        // Example: 
        //   result = map.MergeLeft(other1, other2, ...)
        public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others)
            where T : IDictionary<K, V>, new()
        {
            T newMap = new T();
            foreach (IDictionary<K, V> src in
                (new List<IDictionary<K, V>> { me }).Concat(others))
            {
                // ^-- echk. Not quite there type-system.
                foreach (KeyValuePair<K, V> p in src)
                {

                    newMap[p.Key] = p.Value;

                }
            }
            return newMap;
        }

        public static Dictionary<TKey, TValue>
        Merge<TKey, TValue>(IEnumerable<Dictionary<TKey, TValue>> dictionaries)
        {
            var result = new Dictionary<TKey, TValue>(dictionaries.First().Comparer);
            foreach (var dict in dictionaries)
                foreach (var x in dict)
                    result[x.Key] = x.Value;
            return result;
        }


    }
}

