using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Il2CppInterop;
using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using LibCpp2IL;
using System.IO;
using UnityEngine;
using LibCpp2IL.Metadata;
using BepInEx;
using UnityEngine.Profiling.Memory.Experimental;
using static Il2CppSystem.Globalization.CultureInfo;

namespace TranslationENMOD
{



    internal static class MetadataProcessing
    {
        public static Dictionary<string, string> processbuffer = new Dictionary<string, string>();
        static List<string> untranslated = new List<string>();
        public static string path = Path.Combine(Application.dataPath, "il2cpp_data", "Metadata", "global-metadata.dat");
        static public void ProcessMetadata()
        {
            var unityversion = new AssetRipper.VersionUtilities.UnityVersion(2020, 3, 12);

            var pepath = Path.Combine(BepInEx.Paths.GameRootPath, "GameAssembly.dll");
            var bytearray = File.ReadAllBytes(path);
            //Il2CppMetadata metadata = LibCpp2IL.Metadata.Il2CppMetadata.ReadFrom(bytearray, unityversion);


            LibCpp2IlMain.LoadFromFile(pepath, path, unityversion);
            var metadata = LibCpp2IlMain.TheMetadata;
            var header = metadata.metadataHeader;
            Plugin.log.LogInfo("Header String Count : " + header.stringLiteralCount);

            for (int i = 0; i < header.stringLiteralCount; i++)
            {
                                try
                {
                    var str = LibCpp2IlMain.TheMetadata.GetStringLiteralFromIndex((uint)i);




                    if (Helpers.IsChinese(str))
                    {
                        var finalstring = str.Replace("\n", "<lf>");

                        if (HasNoInvalidChars(finalstring))
                        {
                            //Plugin.log.LogInfo("Metadata string ... : " + finalstring);

                            MetaDataReplace(finalstring, LibCpp2IlMain.TheMetadata.GetStringLiteralFromIndex((uint)i));
                            foreach(var kvp in processbuffer)
                            {
                                //Plugin.log.LogInfo("Key : " + kvp.Key + " // Value : " + kvp.Value);
                            }

                            
                        }

                    }
                }
                catch
                {

                }
            }
            if (processbuffer.Count > 0)
            {
                Plugin.log.LogInfo("ProcessBuffer Count : " + processbuffer.Count());

                WriteToMetadata(header, metadata);
            }
            var pathMUN = Path.Combine(BepInEx.Paths.PluginPath, "Dump", "MetadataUN.txt");
            if (File.Exists(pathMUN))
            {
                File.Delete(pathMUN);
            }
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(pathMUN, append: true))
            {
                foreach (var str in untranslated)
                {
                    sw.Write(str + System.Environment.NewLine);
                }
            }
        }
        static public bool HasNoInvalidChars(string str)
        {
            var b = true;
            foreach (var ch in str)
            {
                var pattern = @"\p{IsCJKUnifiedIdeographs}";
                var pattern2 = @"\p{IsBasicLatin}";
                var pattern3 = @"\p{P}";
                //Plugin.log.LogInfo("Check character : " + ch + "// CJKCheck = " + Regex.IsMatch(ch.ToString(), pattern) + " // Latin Check " + Regex.IsMatch(ch.ToString(), pattern2) + " // Punct Check : " + Regex.IsMatch(ch.ToString(), pattern3));
                if (str.Contains("UI/") || !Helpers.IsChinese(str) || !Regex.IsMatch(ch.ToString(), pattern) && !Regex.IsMatch(ch.ToString(), pattern2) && !Regex.IsMatch(ch.ToString(), pattern3))
                {
                    b = false;
                }
            }
            return b;
        }

        static string MetaDataReplace(string str, string original)
        {
            if (!Plugin.forbiddenMetadataList.Contains(str))
            {
                if (Plugin.MetaDataDict.ContainsKey(str))
                {
                    var dest = Plugin.MetaDataDict[str].Replace("<lf>","\n");                  
                    processbuffer.Add(original, dest);

                }
                else
                {
                    untranslated.Add(str);
                }
            }
            else
            {
                Plugin.log.LogInfo("Not touching this with a 15ft pole : " + str);
            }
            return str;
        }
        public static void WriteToMetadata(Il2CppGlobalMetadataHeader header, Il2CppMetadata metadata)
        {
            if(File.Exists(path + ".bak"))
            {
                File.Delete(path + ".bak");
            }
            Plugin.log.LogInfo("Creating a backup of your metadata in the relevant folder (adding a .bak extension)");
            File.Copy(path, path + ".bak");

            var sl = typeof(Il2CppMetadata).GetField("stringLiterals", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).GetValue(metadata);
            var stringLiterals = (Il2CppStringLiteral[])sl;
           

            var destStream = System.IO.File.Create(path + ".new");
            using (BinaryReader stream = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                for (int i = 1; i < header.stringLiteralCount; i++)
                {
                    byte[] verifyArray = new byte[stringLiterals[i].length];

                    var bytes = Encoding.UTF8.GetBytes(LibCpp2IlMain.TheMetadata.GetStringLiteralFromIndex((uint)i));
                    //Plugin.log.LogInfo("Byte Translation : " + bytes.ToStringEnumerable());
                    try
                    {
                        stream.BaseStream.Seek((long)header.stringLiteralDataOffset + (long)stringLiterals[i].dataIndex, SeekOrigin.Begin);

                        var pos = stream.ReadBytes((int)stringLiterals[i].length);
                        //Plugin.log.LogInfo("Current Position = " + (int)stream.BaseStream.Position);
                        //Plugin.log.LogInfo("Theoretical Position = " + (int)header.stringLiteralOffset + stringLiterals[i].dataIndex);
                        if (processbuffer.ContainsKey(Encoding.UTF8.GetString(bytes)) && Encoding.UTF8.GetString(bytes) == Encoding.UTF8.GetString(pos))
                        {
                            Plugin.log.LogInfo("uhoh : " + Encoding.UTF8.GetString(bytes) + " // " + Encoding.UTF8.GetString(pos));
                            

                            Plugin.log.LogInfo("Found");
                            byte[] expectedArray = Encoding.UTF8.GetBytes(processbuffer[Encoding.UTF8.GetString(bytes)]);

                            var pos2 = expectedArray;
                            Plugin.log.LogInfo("pos.length : " + pos.Length);
                            Plugin.log.LogInfo("pos2.length : " + pos2.Length);
                            int lengthDifference = pos2.Length - pos.Length;
                            var oldByteLength = pos.Length;
                            var newByteLength = pos2.Length;
                            var startPosition = (long)header.stringLiteralDataOffset + (long)stringLiterals[i].dataIndex;
                            stream.BaseStream.Seek((long)header.stringLiteralDataOffset + (long)stringLiterals[i].dataIndex, SeekOrigin.Begin);
                            //stream.BaseStream.Write(pos2, 0, expectedArray.Length);
                            stream.BaseStream.Seek(startPosition, SeekOrigin.Begin);
                            stream.BaseStream.Write(pos2, 0, newByteLength);


                            //

                            if (lengthDifference < 0)
                            {
                                stream.BaseStream.Seek(startPosition + newByteLength, SeekOrigin.Begin);
                                byte[] buffer = new byte[-lengthDifference];
                                stream.Read(buffer, 0, -lengthDifference);
                                stream.BaseStream.Seek(startPosition + oldByteLength, SeekOrigin.Begin);
                                stream.BaseStream.Write(buffer, 0, -lengthDifference);
                            }
                            // If the new bytes are longer than the original bytes, move the remaining bytes backward
                            else if (lengthDifference > 0)
                            {
                                stream.BaseStream.Seek(startPosition + oldByteLength, SeekOrigin.Begin);
                                byte[] buffer = new byte[lengthDifference];
                                stream.Read(buffer, 0, lengthDifference);
                                stream.BaseStream.Seek(startPosition + newByteLength, SeekOrigin.Begin);
                                stream.BaseStream.Write(buffer, 0, lengthDifference);
                            }

                            //

                            Plugin.log.LogInfo("updated ? : " + Encoding.UTF8.GetString(bytes) + " // " + Encoding.UTF8.GetString(pos2));

                        }


                    }
                    catch (Exception ex)
                    {
                        Plugin.log.LogInfo(ex.ToString());
                    }


                }
                stream.Close();
            }
        }
    }
}


