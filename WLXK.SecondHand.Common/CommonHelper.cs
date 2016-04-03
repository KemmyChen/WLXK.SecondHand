using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WLXK.SecondHand.Common
{
    public static class CommonHelper
    {
        private static string WebSite { get; set; }
        private static string AdminEmail { get; set; }
        private static string AdminQQNum { get; set; }

        public static string GetStringMD5(string str)
        {
            str = str + "qZ2huo@";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            using (MD5 md5 = MD5.Create())
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
                byte[] strs = md5.ComputeHash(buffer);
                for (int i = 0; i < strs.Length; i++)
                {
                    sb.Append(strs[i].ToString("x2"));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 得到当前网站地址
        /// </summary>
        /// <returns></returns>
        public static string GetWebSite()
        {
            if (WebSite == null)
            {
                WebSite = ConfigurationManager.AppSettings["website"].ToString();
            }

            return WebSite;
        }


        public static string ConvertToPinYin(string hans)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in hans)
            {
                ChineseChar chin = new ChineseChar(item);

                foreach (var pinyin in chin.Pinyins)
                {
                    sb.Append(pinyin);
                }
            }

            return sb.ToString();
        }

        public static string ConvertToHans(string pinyin)
        {
            //StringBuilder sb = new StringBuilder();
            char[] str = ChineseChar.GetChars(pinyin);
            return str.ToString();
        }

        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static String GetStreamMD5(Stream stream)
        {
            string strResult = "";
            string strHashData = "";
            byte[] arrbytHashValue;
            System.Security.Cryptography.MD5CryptoServiceProvider oMD5Hasher =
                new System.Security.Cryptography.MD5CryptoServiceProvider();
            arrbytHashValue = oMD5Hasher.ComputeHash(stream); //计算指定Stream 对象的哈希值
            //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”
            strHashData = System.BitConverter.ToString(arrbytHashValue);
            //替换-
            strHashData = strHashData.Replace("-", "");
            strResult = strHashData;
            return strResult;
        }


        //得到系统管理员邮箱
        public static string GetAdminEmail()
        {
            if (AdminEmail == null)
            {
                AdminEmail = ConfigurationManager.AppSettings["adminEmail"].ToString();
            }

            return AdminEmail;
        }

        //得到系统管理员QQ
        public static string GetAdminQQ()
        {
            if (AdminQQNum == null)
            {
                AdminQQNum = ConfigurationManager.AppSettings["adminQQ"].ToString();
            }

            return AdminQQNum;
        }
    }
}
