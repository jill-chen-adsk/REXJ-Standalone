using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace SectionListSteel.Utils {
    internal static class LoadRSTXml
    {
        private static readonly string FILENAME = "REXJ-RST.xml";

        private static readonly string ELEM_ROOT = "REXJ-RST";
        private static readonly string ELEM_VERSION = "Version";

        public static string GetVersion()
        {
            // ファイルオープン
            if (!OpenFile(FileName(), ELEM_ROOT, out XmlDocument xmlDoc, out XmlElement root))
                return string.Empty;

            string version = GetElementAsString(root, ELEM_VERSION);

            return version;
        }

        private static string FileName()
        {
            return Directory.GetParent(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName) + @"\" + FILENAME;
        }

        /// <summary>
        /// 要素取得(string)
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="strElem"></param>
        /// <returns></returns>
        private static string GetElementAsString(XmlNode parent, string strElem)
        {
            string str = "";
            XmlNode node = parent.SelectSingleNode(strElem);
            if (node == null)
                return str;

            str = node.InnerText;
            return str;
        }

        /// <summary>
        /// XMLファイルを開く
        /// </summary>
        private static bool OpenFile(string fileFullPath, string elemRoot, out XmlDocument xmlDoc, out XmlElement root)
        {
            xmlDoc = new XmlDocument();
            root = null;

            if (!File.Exists(fileFullPath))
                return false;
            try
            {
                xmlDoc.Load(fileFullPath);
            }
            catch (Exception)
            {
                return false;
            }

            root = xmlDoc.DocumentElement;
            if (root.Name != elemRoot)
                return false;

            return true;
        }
    }
}