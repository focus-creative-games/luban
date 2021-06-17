using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Types;
using System;
using System.IO;

namespace Luban.Job.Cfg.Utils
{
    static class DataUtil
    {
        public static string[] SplitVectorString(string x)
        {
            return x.Split(',', '_', ';');
        }

        public static string[] SplitStringByAnySepChar(string x, string sep)
        {
            return x.Split(sep.ToCharArray());
        }

        public static DType CreateVector(TVector2 type, string x)
        {
            var values = DataUtil.SplitVectorString(x);

            return new DVector2(new System.Numerics.Vector2(float.Parse(values[0]), float.Parse(values[1])));

        }

        public static DType CreateVector(TVector3 type, string x)
        {
            var values = DataUtil.SplitVectorString(x);

            return new DVector3(new System.Numerics.Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2])));

        }

        public static DType CreateVector(TVector4 type, string x)
        {
            var values = DataUtil.SplitVectorString(x);
            return new DVector4(new System.Numerics.Vector4(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3])));
        }

        public static DDateTime CreateDateTime(string x, TimeZoneInfo timeZoneInfo)
        {

            DateTime dateTime = DateTime.ParseExact(x,
                new string[] {
                    "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH", "yyyy-MM-dd",
                    //"yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd HH:mm", "yyyy/MM/dd HH", "yyyy/MM/dd",
                },
                System.Globalization.CultureInfo.InvariantCulture);
            return new DDateTime(TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZoneInfo));
        }

        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetSourceFile(DType data)
        {
            return (string)data.Source;
        }

        public static string UnEscapeString(string s)
        {
            if (s == "null" || s == "\"\"")
            {
                return "";
            }
            return s;
        }

        public static (string Key, string Text) ExtractText(string rawKeyAndText)
        {
            string[] keyAndText = rawKeyAndText.Split('|');
            if (keyAndText.Length != 2)
            {
                throw new Exception("text data should like <key>|<text>");
            }
            return (keyAndText[0], keyAndText[1]);
        }

        //public static string Data2String(DType data)
        //{
        //    var s = new StringBuilder();
        //    data.Apply(VisitorToString.Ins, s);
        //    return s.ToString();
        //}
    }
}
