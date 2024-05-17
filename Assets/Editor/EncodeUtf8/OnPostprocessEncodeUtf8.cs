/*
 * This file is a part of the project "EncodeUtf8".
 * https://github.com/catsnipe/EncodeUtf8
 */

using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

#nullable enable

/// <summary>
/// 新規スクリプト生成時にテキストエンコードを UTF8 に矯正します
/// </summary>
public class OnPostprocessEncodeUtf8 : AssetPostprocessor
{
    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetsPath)
    {
        foreach(string asset in importedAssets)
        {
            if (!File.Exists(asset))
                continue;
            if (Path.GetExtension(asset) != ".cs") // C# ファイルのみ対象
                continue;
            if (asset.IndexOf("Assets/") != 0)
                continue;

            var bs  = File.ReadAllBytes(asset);
            if((bs[0] == 0xEF) && (bs[1] == 0xBB) && (bs[2] == 0xBF))
            {
                // UTF8 with BOM
            }
            else
            {
                var enc = EncodeSourceFile.GetEncode(bs);
                if(enc != Encoding.UTF8)
                {
                    var text = enc?.GetString(bs);
                    File.WriteAllText(asset, text, Encoding.UTF8);

                    Debug.Log("Convert to UTF-8: " + asset);
                }
            }

            var crlf_text = File.ReadAllText(asset);
            if (crlf_text.IndexOf("\r\n") >= 0)
            {
                crlf_text = crlf_text.Replace("\r\n", "\n");
                File.WriteAllText(asset, crlf_text, Encoding.UTF8);

                Debug.Log($"CRLF -> LF: {asset}");
            }
        }
    }
}
