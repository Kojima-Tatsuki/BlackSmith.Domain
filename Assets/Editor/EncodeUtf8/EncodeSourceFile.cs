/*
 * This file is a part of the project "EncodeUtf8".
 * https://github.com/catsnipe/EncodeUtf8
 */

using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

#nullable enable

public class EncodeSourceFile : Editor
{
    private const string ToolName = "Tools/Encode to Utf8";

    [MenuItem(ToolName)]
    private static void MenuExec()
    {
        var files = Directory.GetFiles("Assets/", "*.cs", SearchOption.AllDirectories);

        foreach (var file in files)
        {
#if UNITY_STANDALONE_WIN
            var path = $"{Application.dataPath}{file.Remove(0, 6)}".Replace("/", "\\");
#else
            var path = $"{Application.dataPath}{file.Remove(0, 6)}".Replace("\\", "/");
#endif

            // エンコードを UTF-8 に変換
            var enc = GetEncode(File.ReadAllBytes(path));
            if (enc != Encoding.UTF8)
            {
                var text = File.ReadAllText(path, Encoding.GetEncoding("shift-jis"));
                File.WriteAllText(path, text, Encoding.UTF8);

                Debug.Log($"{Path.GetFileName(path)}: Convert to UTF-8 from {enc}.");
            }

            // 改行コードを LF に統一
            var crlf_text = File.ReadAllText(path);
            if (crlf_text.IndexOf("\r\n") >= 0)
            {
                crlf_text = crlf_text.Replace("\r\n", "\n");
                File.WriteAllText(path, crlf_text, Encoding.UTF8);

                Debug.Log($"{Path.GetFileName(path)}: CRLF -> LF");
            }
        }
    }

    /// <summary>
    /// BOMを調べてエンコードを判定する
    /// DOBON.Net https://dobon.net/vb/dotnet/string/detectcode.html
    /// </summary>
    public static Encoding? GetEncode(byte[] bytes)
    {
        const byte bEscape = 0x1B; 
        const byte bAt = 0x40; // @
        const byte bDollar = 0x24; // $
        const byte bAnd = 0x26; // &
        const byte bOpen = 0x28; // (
        const byte bB = 0x42;
        const byte bD = 0x44;
        const byte bJ = 0x4A;
        const byte bI = 0x49;

        int length = bytes.Length;

        // Encode::is_utf8 は無視

        bool isBinary = false;
        for (int i = 0; i < length; i++)
        {
            var b1 = bytes[i];
            if (b1 <= 0x06 || b1 == 0x7F || b1 == 0xFF)
            {
                isBinary = true; // 'binary'
                if (b1 == 0x00 && i < length - 1 && bytes[i + 1] <= 0x7F)
                    return Encoding.Unicode; // smells like raw unicode
            }
        }

        if (isBinary)
            return null;

        // not Japanese
        bool notJapanese = true;
        for (int i = 0; i < length; i++)
        {
            var b1 = bytes[i];

            if (b1 == bEscape || 0x80 <= b1)
            {
                notJapanese = false;
                break;
            }
        }

        if (notJapanese)
            return Encoding.ASCII;

        for (int i = 0; i < length - 2; i++)
        {
            var b1 = bytes[i];
            var b2 = bytes[i + 1];
            var b3 = bytes[i + 2];

            if (b1 == bEscape)
            {
                if (b2 == bDollar && b3 == bAt)
                    return Encoding.GetEncoding(50220); // JIS_0208 1978
                else if (b2 == bDollar && b3 == bB)
                    return Encoding.GetEncoding(50220); // JIS_0208 1983
                else if (b2 == bOpen && (b3 == bB || b3 == bJ))
                    return Encoding.GetEncoding(50220); // JIS_ASC
                else if (b2 == bOpen && b3 == bI)
                    return Encoding.GetEncoding(50220); // JIS_KANA

                if (i < length - 3)
                {
                    var b4 = bytes[i + 3];

                    if (b2 == bDollar && b3 == bOpen && b4 == bD)
                        return Encoding.GetEncoding(50220); //JIS_0212
                    if (i < length - 5 && b2 == bAnd && b3 == bAt && b4 == bEscape && bytes[i + 4] == bDollar && bytes[i + 5] == bB)
                        return Encoding.GetEncoding(50220); //JIS_0208 1990
                }
            }
        }

        // should be euc|sjis|utf8
        int sjis = 0;
        int euc = 0;
        int utf8 = 0;

        for (int i = 0; i < length - 1; i++)
        {
            var b1 = bytes[i];
            var b2 = bytes[i + 1];

            if (((0x81 <= b1 && b1 <= 0x9F) || (0xE0 <= b1 && b1 <= 0xFC)) && ((0x40 <= b2 && b2 <= 0x7E) || (0x80 <= b2 && b2 <= 0xFC)))
            {
                // SJIS_C
                sjis += 2;
                i++;
            }
        }

        for (int i = 0; i < length - 1; i++)
        {
            var b1 = bytes[i];
            var b2 = bytes[i + 1];

            if (((0xA1 <= b1 && b1 <= 0xFE) && (0xA1 <= b2 && b2 <= 0xFE)) || (b1 == 0x8E && (0xA1 <= b2 && b2 <= 0xDF)))
            {
                // EUC_C
                // EUC_KANA
                euc += 2;
                i++;
            }
            else if (i < length - 2)
            {
                var b3 = bytes[i + 2];

                if (b1 == 0x8F && (0xA1 <= b2 && b2 <= 0xFE) && (0xA1 <= b3 && b3 <= 0xFE))
                {
                    // EUC_0212
                    euc += 3;
                    i += 2;
                }
            }
        }

        for (int i = 0; i < length - 1; i++)
        {
            var b1 = bytes[i];
            var b2 = bytes[i + 1];

            if ((0xC0 <= b1 && b1 <= 0xDF) && (0x80 <= b2 && b2 <= 0xBF))
            {
                // UTF8
                utf8 += 2;
                i++;
            }
            else if (i < length - 2)
            {
                var b3 = bytes[i + 2];
                if ((0xE0 <= b1 && b1 <= 0xEF) && (0x80 <= b2 && b2 <= 0xBF) && (0x80 <= b3 && b3 <= 0xBF))
                {
                    // UTF8
                    utf8 += 3;
                    i += 2;
                }
            }
        }

        if (euc > sjis && euc > utf8)
            return Encoding.GetEncoding(51932); // EUC
        else if (sjis > euc && sjis > utf8)
            return Encoding.GetEncoding(932); // SJIS
        else if (utf8 > euc && utf8 > sjis)
            return Encoding.UTF8; // UTF8

        return null;
    }
}
