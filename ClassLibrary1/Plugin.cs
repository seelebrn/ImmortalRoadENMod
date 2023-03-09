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
using BepInEx.Unity.IL2CPP.Utils.Collections;
using UnhollowerBaseLib.Runtime;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
//using static MapIndex;
using Microsoft.CSharp;
//using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using System.Linq;
using KEngine;
using DataLib;
using KEngine.UI;
using static KEngine.AssetFileLoader;
using Il2CppSystem.Linq;
using System.Xml;
using SimpleJSON;
using static Octree.OctreeElement;
using Cpp2IlInjected;
using static Il2CppSystem.Net.WebCompletionSource;
using UnityEngine.UI;
using Il2CppSystem.Resources;

namespace TranslationENMOD
{

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource log = new ManualLogSource("EN"); // The source name is shown in BepInEx log
        public static System.Collections.Generic.List<string> contents = new System.Collections.Generic.List<string>();
        public static System.Collections.Generic.List<string> processed = new System.Collections.Generic.List<string>();
        public static string sourceDir = BepInEx.Paths.PluginPath;
        public static string fileDir = Application.dataPath;
        public static List<string> TAUn = new List<string>();
        public static Dictionary<string, string> TADict = new Dictionary<string, string>();
        public static List<string> UITextUn = new List<string>();
        public static Dictionary<string, string> UITextDict = new Dictionary<string, string>();


        public static bool clean = false;
        public static Il2CppSystem.Collections.Generic.List<Il2CppSystem.Reflection.FieldInfo> fields = new Il2CppSystem.Collections.Generic.List<Il2CppSystem.Reflection.FieldInfo>();

        public static Dictionary<string, string> FileToDictionary(string dir)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            IEnumerable<string> lines = File.ReadLines(Path.Combine(sourceDir, "Translations", dir));

            foreach (string line in lines)
            {

                var arr = line.Split('¤');

                var pair = new KeyValuePair<string, string>(Regex.Replace(arr[0], @"\t|\n|\r", ""), arr[1]);

                if (!dict.ContainsKey(pair.Key))
                    dict.Add(pair.Key, pair.Value);
                else
                    Debug.Log($"Found a duplicated line while parsing {dir}: {pair.Key}");

            }

            return dict;

        }
        public static void LoadPersonUI()
        {




        }
        public override void Load()
        {

            if (File.Exists(Path.Combine(BepInEx.Paths.PluginPath, "Dump", "TAKV.txt")))
            {
                File.Delete(Path.Combine(BepInEx.Paths.PluginPath, "Dump", "TAKV.txt"));
            }
            if (File.Exists(Path.Combine(BepInEx.Paths.PluginPath, "Dump", "UITextKV.txt")))
            {
                File.Delete(Path.Combine(BepInEx.Paths.PluginPath, "Dump", "UITextKV.txt"));
            }
            BepInEx.Logging.Logger.Sources.Add(log);
            TADict = FileToDictionary("TAKV.txt");
            UITextDict = FileToDictionary("UITextKV.txt");
            AddComponent<mbmb>();
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

    }
    class mbmb : MonoBehaviour
    {
        public static List<string> processed = new List<string>();
        public mbmb(System.IntPtr handle) : base(handle) { }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                var dir = new DirectoryInfo(Path.Combine(Application.dataPath)).GetFiles();
                foreach (var x in dir)
                {
                    if (x.FullName.EndsWith("level0") || x.FullName.EndsWith("level2") || x.FullName.EndsWith("sharedassets2.assets"))
                    {
                        Plugin.log.LogInfo("Now dumping strings from  : " + x.FullName);
                        Dump.TranslateResources(x.FullName, Application.dataPath);
                        Plugin.log.LogInfo("Done dumping strings from  : " + x.FullName + "... Phew !");
                    }


                    if (!x.FullName.Contains("sharedassets") && !x.FullName.Contains("\\level") && !x.FullName.Contains("resS") && !x.FullName.Contains("manifest") && !x.FullName.Contains("globalgamemanagers") && !x.FullName.Contains(".info") && !x.FullName.Contains(".json") && !x.FullName.Contains(".config"))
                    {
                        if (!x.FullName.Contains(".resource"))
                        {
                            Plugin.log.LogInfo("Now dumping strings from  : " + x.FullName);
                            Dump.TranslateResources(x.FullName, Application.dataPath);
                            Plugin.log.LogInfo("Done dumping strings from  : " + x.FullName + "... Phew !");
                        }
                    }
                }

                var dirbundles = Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "Bundles", "Windows"), "*", SearchOption.AllDirectories);
                foreach (var x in dirbundles)
                {
                    if (!x.Contains("manifest") && !x.Contains("ui\\atlas\\icons\\"))
                    {
                        Plugin.log.LogInfo("Now dumping strings from AssetBundle : " + x);
                        Dump.TranslateBundles(x, Application.dataPath);
                        Plugin.log.LogInfo("Done dumping strings from  AssetBundle : " + x + "... Phew !");

                    }
                }
            }
        }
    }

    /*[HarmonyPatch(typeof(AssetBundleLoader), "OnFinish")]
    static class Patch_AssetBundleLoader_OnFinish
    {
        static void Postfix(AssetBundleLoader __instance,ref Il2CppSystem.Object resultObj)
        {
            Plugin.log.LogInfo("Patch_AssetBundleLoader_OnFinish : " + resultObj.GetIl2CppType().Name);
            foreach (var x in __instance.loadedObjs)
            {
                Plugin.log.LogInfo("Patch_AssetBundleLoader_OnFinish__instance.loadedObjs : " + x.Value.assetBundle);
            }
        }
    }
    [HarmonyPatch(typeof(AssetBundleLoader), "PushLoadedAsset")]
    static class Patch_AssetBundleLoader_PushLoadedAsset
    {
        static void Postfix(ref UnityEngine.Object getAsset)
        {
            Plugin.log.LogInfo("Patch_AssetBundleLoader_PushLoadedAsset : " + getAsset.name);
        }
    }


    [HarmonyPatch(typeof(KResourceModule), "LoadAssetsSync")]
    class Patch_KResourceModule_LoadAssetsSync
    {
        static void Postfix(KResourceModule __instance, ref string path)
        {
            Plugin.log.LogInfo("Patch_KResourceModule_LoadAssetsSync : " + path);
        }
    }
    [HarmonyPatch]
    class Patch_RSELoader
    {
        static System.Collections.Generic.IEnumerable<System.Reflection.MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(RseLoader), "LoadAsset", new System.Type[] { typeof(AbstractResourceLoader), typeof(string) }).MakeGenericMethod(typeof(UnityEngine.Object));
            yield return AccessTools.Method(typeof(RseLoader), "LoadAsset", new System.Type[] { typeof(AssetBundle), typeof(string) }).MakeGenericMethod(typeof(UnityEngine.Object));

        }
        static void Postfix(RseLoader __instance)
        {
            Plugin.log.LogInfo("Berk ! " + __instance);
        }
    }*/
    /*[HarmonyPatch]
    class Patch_AssetFileBridgeDelegate
    {
        static System.Collections.Generic.IEnumerable<System.Reflection.MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(AssetFileBridgeDelegate), "Invoke");
            yield return AccessTools.Method(typeof(AssetFileBridgeDelegate), "BeginInvoke");

        }
        static void Prefix(AssetFileBridgeDelegate __instance, ref UnityEngine.Object resultObj)
        {

   
        }*/


    [HarmonyPatch(typeof(ResourceReader), "LoadString")]
    class Patch_ResourceReader
    {
        static void Postfix(ResourceReader __instance, ref string __result)
        {

            Plugin.log.LogInfo(__result);

        }
    }
    [HarmonyPatch(typeof(UnityEngine.UI.Text), "OnEnable")]
    static class Patch_UnityEngineUIText
    {
        static void Postfix(UnityEngine.UI.Text __instance)
        {
            if (__instance.m_Text != null)
            {
                if (Helpers.IsChinese(__instance.m_Text))
                {
                    __instance.m_Text = Helpers.AddItemToListUI(__instance.m_Text, "UITextKV");

                }
            }
        }
    }

    [HarmonyPatch(typeof(SimpleJSON.JSONNode), "Parse")]

    static class JsonPatching
    {
        static bool ArrayIterate(JSONNode obj, bool check)
        {
            try
            {

                if (obj != null)
                {
                    if (Helpers.IsChinese(obj.ToString()))
                    {
                        //Plugin.log.LogInfo("original.ToString() : " + obj.ToString());
                        //Plugin.log.LogInfo("obj.Count : " + obj.Count);
                        if (obj.Count > 0)
                        {
                            for (int i = 0; i < obj.Count; i++)
                            {
                                if (obj.Childs.Count() > 0)
                                {
                                    //Plugin.log.LogInfo("    Child.ToString() : " + obj[i].ToString());
                                    //Plugin.log.LogInfo("    Child.Count : " + obj[i].Count);
                                    ArrayIterate(obj[i], check);
                                }
                                if (obj[i].Count == 0)
                                {
                                    if (obj[i].ToString() != null)
                                    {
                                        if (Helpers.IsChinese(obj[i].ToString()) || obj[i].ToString().Contains("\\u"))
                                        {
                                            var value = obj[i].ToString().Replace("\"", "");
                                            if (value == "八卦丹丹方")
                                            {
                                                //Plugin.log.LogInfo("Found !");
                                                check = true;

                                            }
                                            //Plugin.log.LogInfo("Value = " + value);
                                            if (Plugin.TADict.ContainsKey(value))
                                            {
                                                obj[i] = Plugin.TADict[value];
                                                //Plugin.log.LogInfo("Replacement Found ! " + Plugin.TADict[value]);
                                            }
                                            else
                                            {
                                                if (!value.Contains("土之窍穴-") && !value.Contains("金之窍穴-") && !value.Contains("木之窍穴-") && !value.Contains("火之窍穴-") && !value.Contains("水之窍穴-"))
                                                {
                                                    //Plugin.log.LogInfo("Writing line to TAUn : " + value);
                                                    Helpers.AddItemToList(value, "TAKV");
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Plugin.log.LogInfo("Exception :" + e);
                Plugin.log.LogInfo("Exception Json :" + obj.ToString());
            }
            return check;
        }
        static bool check = false;
        static void Postfix(JSONNode __instance, ref JSONNode __result, ref string aJSON)
        {

            if (aJSON != null)
            {
                if (Helpers.IsChinese(__result.ToString()))
                {
                    try
                    {
                        if (JsonPatching.check == true)
                        {
                            Plugin.log.LogInfo("Check : " + __result.ToString());
                        }
                        ArrayIterate(__result, JsonPatching.check);

                    }
                    catch (System.Exception e)
                    {
                        Plugin.log.LogInfo("Exception :" + e);
                        Plugin.log.LogInfo("Exception Json :" + __result.ToString());
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(mTextPro), "SetText")]
    static class Patch_mTextPro
    {
        static void Postfix(mTextPro __instance, ref string messageStr, ref string eventStr)
        {
            Plugin.log.LogInfo(__instance.text);
            Plugin.log.LogInfo(messageStr);
            Plugin.log.LogInfo(eventStr);
        }
    }
    public static class Helpers
    {
        public static string AddItemToList(string str, string file)
        {

            var list = new System.Collections.Generic.List<string>();
            var path = Path.Combine(BepInEx.Paths.PluginPath, "Dump", file + ".txt");
            if (File.Exists(path))
            {
                list = File.ReadAllLines(path).ToList();
            }
            else
            {

                list = new System.Collections.Generic.List<string>();
            }
            if (Plugin.TADict.ContainsKey(Helpers.CustomEscape(str)))
            {
                //Plugin.log.LogInfo("Found Match : " + Helpers.CustomUnescape(Plugin.etcDict[Helpers.CustomEscape(str)]));
                return Helpers.CustomUnescape(Plugin.TADict[Helpers.CustomEscape(str)]);
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
        public static string AddItemToListUI(string str, string file)
        {

            var list = new System.Collections.Generic.List<string>();
            var path = Path.Combine(BepInEx.Paths.PluginPath, "Dump", file + ".txt");
            if (File.Exists(path))
            {
                list = File.ReadAllLines(path).ToList();
            }
            else
            {

                list = new System.Collections.Generic.List<string>();
            }
            //Plugin.log.LogInfo("Original String : " + Helpers.CustomEscape(str));

            if (Plugin.UITextDict.ContainsKey(Helpers.CustomEscape(str)))
            {
                //Plugin.log.LogInfo("Found Match : " + Helpers.CustomUnescape(Plugin.UITextDict[Helpers.CustomEscape(str)]));
                return Helpers.CustomUnescape(Plugin.UITextDict[Helpers.CustomEscape(str)]);
            }
            else
            {

                using (StreamWriter sw = new StreamWriter(path, append: true))
                {
                    //Plugin.log.LogInfo("Wriring Untranslated String ... : " + Helpers.CustomEscape(str));
                    if (!list.Contains(Helpers.CustomEscape(str)) && str != null)
                    {
                        if (Helpers.IsChinese(Helpers.CustomEscape(str)))
                        {

                            sw.Write(Helpers.CustomEscape(str) + System.Environment.NewLine);
                            list.Add(Helpers.CustomEscape(str));

                        }
                    }

                }
                return str;
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


