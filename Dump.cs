using AssetsTools.NET.Extra;
using AssetsTools.NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UVExtractor.Il2Cpp;
using System.IO;
using AssetsTools.NET.Cpp2IL;
using Il2CppSystem.Linq.Expressions;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using static ServerTower.TowerData;
using SevenZip.Compression.LZMA;
using HarmonyLib;
using UnityEngine;

namespace TranslationENMOD
{
    public class Dump
    {
        private static string[] TEXTS = new string[] { "Text", "TextMeshProUGUI", "Menu", "Say", "UILabel" };


        static public void TranslateBundle(string file, string gameDataDir)

        {


            List<string> dialogs = new List<string>();
            List<string> result = new List<string>();
            var am = new AssetsManager();
            FindCpp2IlFilesResult il2cppFiles = FindCpp2IlFiles.Find(gameDataDir);
            if (il2cppFiles.success)
            {
                am.MonoTempGenerator = new Cpp2IlTempGenerator(il2cppFiles.metaPath, il2cppFiles.asmPath);
            }

            am.LoadClassPackage("classdata.tpk");

            var bunInst = am.LoadBundleFile(file, true);
            var bun = bunInst.file;
            AssetsFileInstance inst = null;
            try
            {

                inst = am.LoadAssetsFileFromBundle(bunInst, 0, true);
            }
            catch (Exception ex)
            {
                return;
            }

            var afile = inst.file;
            am.LoadClassDatabaseFromPackage(afile.Metadata.UnityVersion);
            var bundlereplacers = new List<BundleReplacer>();
            var replacers = new List<AssetsReplacer>();
            foreach (var inf in afile.GetAssetsOfType(AssetClassID.GameObject))
            {
                try
                { 
                var baseField = am.GetBaseField(inst, inf);
                string name = baseField["m_Name"].AsString;
                if (Helpers.IsChinese(name))
                {
                    Plugin.log.LogInfo("2// " + name);
                }
                var components = baseField["m_Component.Array"];
                foreach (var data in components)
                {
                    var componentPointer = data["Component"];
                    var componentExtInfo = am.GetExtAsset(inst, componentPointer);
                    var componentType = (AssetClassID)componentExtInfo.info.TypeId;

                    Plugin.log.LogInfo($"  {componentType}");
                }
                }
                catch
                {

                }
            }

            foreach (var inf in afile.GetAssetsOfType(AssetClassID.MonoBehaviour))
            {

                System.Collections.Generic.List<string> strings = new System.Collections.Generic.List<string>();
                var baseField = am.GetBaseField(inst, inf);

                string name = baseField["m_Name"].AsString;
                Plugin.log.LogInfo("object name = " + name);
                foreach (var x in baseField)
                {

                    //Plugin.log.LogInfo("        Existing Fields = " + x.FieldName);
                    //Plugin.log.LogInfo("        Existing TypeName = " + x.TypeName);

                    if (x.TypeName == "string" || x.TypeName == "String")
                    {


                        if (x.FieldName.Contains("serializedGraph"))
                        {
                            var s = Helpers.CustomEscape(Regex.Unescape(x.AsString));
                            for (int i = 0; i < x.Children.Count; i++)
                            {
                                Deserialization(x.Children[i].AsString, dialogs, x.Children[i]);
                                for (int j = 0; j < x.Children.Count; j++)
                                {
                                    Deserialization(x.Children[i].Children[j].AsString, dialogs, x.Children[i]);
                                }
                            }
                            //Plugin.log.LogInfo("duserializedgraph = " + s);
                            Deserialization(s, dialogs, x);
                        }

                        else
                        {
                            //Plugin.log.LogInfo("Hey ! " + x.AsString);
                            if (Helpers.IsChinese(x.AsString))
                            {
                                var s = x.AsString;
                                //Plugin.log.LogInfo("Found string in result = " + s);
                                var key = Helpers.CustomEscape(x.AsString);
                                if (Plugin.translationDict.ContainsKey(key))
                                {
                                    if (!Plugin.newresultDict.ContainsKey(key))
                                    {
                                        Plugin.newresultDict.Add(key, Plugin.translationDict[key]);
                                    }
                                    //Plugin.log.LogInfo("Found match in dict for result = " + Plugin.translationDict[Helpers.CustomEscape(x.AsString)]);
                                    x.AsString = Plugin.translationDict[key];

                                    for (int i = 0; i < x.Children.Count; i++)
                                    {

                                        if (x.Children[i].TypeName == "string")
                                        {
                                            Plugin.log.LogInfo("Found a Child ! " + x.Children[i].ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    result.Add(Helpers.CustomEscape(x.AsString));
                                }


                            }


                        }
                    }
                    else
                    {
                        //Plugin.log.LogInfo("x: name " + x.FieldName + " ; x: name " + x.TypeName);
                        foreach (var s in x)
                        {
                            if (s.TypeName != "float" && s.TypeName != "int" && s.TypeName != "float" && s.TypeName != "UInt8" && s.TypeName != "double" && s.TypeName != "SInt64" && !s.TypeName.Contains("animation"))
                            {
                                Plugin.log.LogInfo("    x => s : TypeName " + s.TypeName + " ; s : Name : " + s.FieldName);
                                IterateThroughArray(s, result);
                            }
                        }




                    }
                    foreach (var child in x.Children)
                    {
                        if (child.TypeName == "string" && child.AsString != null || child.TypeName == "String" && child.AsString != null)
                        {

                            if (Helpers.IsChinese(child.AsString))
                            {
                                
                                var key = child.AsString;
                                if (Plugin.translationDict.ContainsKey(key))
                                {
                                    if (!Plugin.newresultDict.ContainsKey(key))
                                    {
                                        Plugin.log.LogInfo("Found Child Translation : " + key);
                                        Plugin.newresultDict.Add(key, Plugin.translationDict[key]);
                                    }
                                    child.AsString = Plugin.translationDict[key];

                                }
                                else
                                {
                                    Plugin.log.LogInfo("Found Child Un : " + key);
                                    result.Add(key);
                                }
                            }

                            else
                            {
                                foreach (var child2 in child)
                                {

                                    if (child2.TypeName == "string" && child.AsString != null || child2.TypeName == "String" && child.AsString != null)
                                    {
                                        //Plugin.log.LogInfo("Found Child2");
                                        if (Helpers.IsChinese(child2.AsString))
                                        {
                                            var key = child2.AsString;
                                            if (Plugin.translationDict.ContainsKey(key))
                                            {
                                                if (!Plugin.newresultDict.ContainsKey(key))
                                                {
                                                    Plugin.log.LogInfo("Found Child2 Translation : " + key);
                                                    Plugin.newresultDict.Add(key, Plugin.translationDict[key]);
                                                }
                                                child2.AsString = Plugin.translationDict[key];

                                            }
                                            else
                                            {
                                                Plugin.log.LogInfo("Found Child2 Un : " + key);
                                                result.Add(key);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }

                }










                replacers.Add(new AssetsReplacerFromMemory(afile, inf, baseField));
            }


                bundlereplacers.Add(new BundleReplacerFromAssets(inst.name, null, afile, replacers));
                using (AssetsFileWriter writer = new AssetsFileWriter(Path.Combine(BepInEx.Paths.GameRootPath, "TranslatedBundles", am.LoadBundleFile(file, true).name)))
                {
                    bun.Write(writer, bundlereplacers);
                }

                am.UnloadAllAssetsFiles();
                if (result.Count > 0)
                {
                    var path = Path.Combine(BepInEx.Paths.PluginPath, "Dump", "resultUN.txt");
                    if (File.Exists(path))
                    {
                        Plugin.contents = File.ReadAllLines(path).ToList();
                    }
                    else
                    {
                        Plugin.contents = new System.Collections.Generic.List<string>();
                    }
                    using (StreamWriter sw = new StreamWriter(path, append: true))
                    {
                        Plugin.log.LogInfo("Now writing untranslated lines in : " + path);


                        foreach (string s in result.Distinct())
                        {
                            if (!Plugin.contents.Contains(s))
                            {
                                sw.WriteLine(s);
                            }
                        }

                    }
                }
                if (dialogs.Count > 0)
                {
                    var path = Path.Combine(BepInEx.Paths.PluginPath, "Dump", "dialogsUN.txt");
                    if (File.Exists(path))
                    {
                        Plugin.contents = File.ReadAllLines(path).ToList();
                    }
                    else
                    {
                        Plugin.contents = new System.Collections.Generic.List<string>();
                    }
                    using (StreamWriter sw = new StreamWriter(path, append: true))
                    {
                        Plugin.log.LogInfo("Now writing untranslated lines in : " + path);


                        foreach (string s in dialogs.Distinct())
                        {
                            if (!Plugin.contents.Contains(s))
                            {
                                sw.WriteLine(s);
                            }
                        }

                    }

                }

        }



        private static bool IsTextComponents(string className)
        {
            if (className.Contains("Text")) return true;
            foreach (var text in TEXTS)
            {
                if (text.Equals(className)) return true;
            }
            return false;
        }
        public static string GetUnicodeString(string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                sb.Append("\\u");
                sb.Append(String.Format("{0:x4}", (int)c));
            }
            return sb.ToString();
        }
        public static string Deserialization(string s, List<string> dialogs, AssetTypeValueField x)
        {
            //Plugin.log.LogInfo("Deserialization() in progress");
            //Plugin.log.LogInfo("String I'm working on " + s);
            if (Helpers.IsChinese(s))
            {
                //Plugin.log.LogInfo("duserializedgraph = " + s);
                string pattern = "\"([^\"]*)\"";
                System.Text.RegularExpressions.MatchCollection matchCollection = System.Text.RegularExpressions.Regex.Matches(s, pattern);
                //Plugin.log.LogInfo("duserializedgraph = " + Regex.Unescape(du._serializedGraph.ToString()));
                if (matchCollection.Count > 0)
                {
                    string str1 = "";
                    foreach (var match in matchCollection)
                    {
                        if (match != null && Helpers.IsChinese(match.ToString()))
                        {
                            var key = match.ToString().Replace("\"", "");
                            if (Plugin.translationDict.ContainsKey(key))
                            {
                                //Plugin.log.LogInfo("Deserialization Match found = " + str);
                                //Plugin.log.LogInfo("Match with double quotes = " + match.ToString());
                                //Plugin.log.LogInfo("Replacement value = " + Plugin.translationDict[str]);
                                if (!Plugin.newdialogsDict.ContainsKey(key))
                                {
                                    Plugin.newdialogsDict.Add(key, Plugin.translationDict[key]);
                                }
                                s = s.Replace(match.ToString(), "\"" + GetUnicodeString(Plugin.translationDict[key]) + "\"");
                                x.AsString = s;
                            }
                            else
                            {
                                dialogs.Add(match.ToString().Replace("\"", ""));
                            }

                            //Plugin.log.LogInfo("Final Value = " + s);
                        }
                    }
                    //Plugin.log.LogInfo("Deserialization Result = " + Helpers.CustomUnescape(s));
                    return Helpers.CustomUnescape(s);

                }
                else
                {
                    return null;
                }


            }
            else
            {
                return null;
            }
        }

        public static void IterateThroughArray(AssetTypeValueField y, List<string> stringreturns)
        {
            switch (y.TypeName)
            {
                default:
                    try
                    {
                        PopulateArray(y, stringreturns);
                    }
                    catch
                    {

                    }
                    break;
                case "string":

                    if (Helpers.IsChinese(y.AsString))
                    {
                        if (y.AsString != null)
                        {
                            //Plugin.log.LogInfo("Original String ! :" + y.AsString);
                            string key = Helpers.CustomEscape(y.AsString);
                            if (Plugin.translationDict.ContainsKey(key))
                            {
                                Plugin.log.LogInfo("        Original String    ! :" + y.AsString);
                                string s = Plugin.translationDict[key];
                                if (!Plugin.newresultDict.ContainsKey(key))
                                {
                                    Plugin.newresultDict.Add(key, s);
                                    Plugin.log.LogInfo("        Match String    ! :" + s);
                                }


                                y.AsString = Helpers.CustomUnescape(s);

                            }
                            else
                            {
                                Plugin.log.LogInfo("        Added String to Untranslated ! :" + Helpers.CustomEscape(y.AsString));
                                //Plugin.log.LogInfo("IterateThroughArray : " + y.AsString);
                                stringreturns.Add(Helpers.CustomEscape(y.AsString));
                            }
                        }
                    }
                    if (y.AsString.Contains("\\u"))
                    {
                        Plugin.log.LogInfo($"Warning, Encoded string in {y.FieldName}, type {y.TypeName} : " + y.AsString);
                        Plugin.log.LogInfo("Decoded string for debugging purposes : " + Regex.Unescape(y.AsString));
                    }
                    break;
                case "Array":
                    Plugin.log.LogInfo("        Array found ! ");
                    PopulateArray(y, stringreturns);
                    break;
                case "List":
                    PopulateArray(y, stringreturns);
                    break;


            }
        }
        public static void PopulateArray(AssetTypeValueField y, List<string> stringreturns)
        {
            try
            {
                foreach (var subvalue in y)
                {
                    if(subvalue.TypeName != "float" && subvalue.TypeName != "int" && subvalue.TypeName != "float" && subvalue.TypeName != "UInt8" && subvalue.TypeName != "double" && subvalue.TypeName != "SInt64" && !subvalue.TypeName.Contains("animation"))
                    { 
                    Plugin.log.LogInfo("        y => z : TypeName " + subvalue.TypeName + " ; s : Name : " + subvalue.FieldName);
                    IterateThroughArray(subvalue, stringreturns);
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.log.LogError(ex);
            }
        }



    }
}

