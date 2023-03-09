using AssetsTools.NET.Extra;
using AssetsTools.NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AssetsTools.NET.Cpp2IL;
using Il2CppSystem.Linq.Expressions;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using SevenZip.Compression.LZMA;
using HarmonyLib;
using UnityEngine;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace TranslationENMOD
{
    public class Dump
    {
        private static string[] TEXTS = new string[] { "Text", "TextMeshProUGUI", "Menu", "Say", "UILabel" };


        static public void TranslateResources(string file, string gameDataDir)

        {



            List<string> result = new List<string>();
            var am = new AssetsManager();
            FindCpp2IlFilesResult il2cppFiles = FindCpp2IlFiles.Find(gameDataDir);
            if (il2cppFiles.success)
            {
                am.MonoTempGenerator = new Cpp2IlTempGenerator(il2cppFiles.metaPath, il2cppFiles.asmPath);
            }

            am.LoadClassPackage("classdata.tpk");

            var afileInst = am.LoadAssetsFile(file, true);
            var afile = afileInst.file;

            am.LoadClassDatabaseFromPackage(afile.Metadata.UnityVersion);
            //var bundlereplacers = new List<BundleReplacer>();
            var replacers = new List<AssetsReplacer>();


            foreach (var inf in afile.GetAssetsOfType(AssetClassID.MonoBehaviour))
            {
                try
                {
                    System.Collections.Generic.List<string> strings = new System.Collections.Generic.List<string>();
                    var baseField = am.GetBaseField(afileInst, inf);

                    string name = baseField["m_Name"].AsString;
                    //Plugin.log.LogInfo("object name = " + name);
                    foreach (var x in baseField)
                    {

                        //Plugin.log.LogInfo("        Existing Fields = " + x.FieldName);
                        //Plugin.log.LogInfo("        Existing TypeName = " + x.TypeName);

                        if (x.TypeName == "string" || x.TypeName == "String")
                        {

                            //Plugin.log.LogInfo("Field Name : " + x.FieldName);
                            if (x.FieldName == "m_Text")
                            {
                                Plugin.log.LogInfo("Found a string in ... " + file.ToString());
                                x.AsString = Helpers.AddItemToListUI(x.AsString, "UITextKV");
                            }

                        }
                    }
                }
                catch { }
            }
        }
        static public void TranslateBundles(string file, string gameDataDir)

        {
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
                var baseField = am.GetBaseField(inst, inf);

            }

            foreach (var inf in afile.GetAssetsOfType(AssetClassID.MonoBehaviour))
            {
                try
                {
                    System.Collections.Generic.List<string> strings = new System.Collections.Generic.List<string>();
                    var baseField = am.GetBaseField(inst, inf);

                    string name = baseField["m_Name"].AsString;
                    //Plugin.log.LogInfo("object name = " + name);
                    foreach (var x in baseField)
                    {

                        //Plugin.log.LogInfo("        Existing Fields = " + x.FieldName);
                        //Plugin.log.LogInfo("        Existing TypeName = " + x.TypeName);

                        if (x.TypeName == "string" || x.TypeName == "String")
                        {

                            //Plugin.log.LogInfo("Field Name : " + x.FieldName);
                            if (x.FieldName == "m_Text")
                            {
                                Plugin.log.LogInfo("Found a string in bundle ... " + file.ToString());
                            }

                        }
                    }
                }
                catch { }
            }
        }


        }
    }


