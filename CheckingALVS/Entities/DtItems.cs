
using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.CheckingALVS;

namespace ADSK.JExtRAC.CheckingALVS.Entities
{
    /// ================================================================================
    /// <summary>データテーブル - 項目</summary>
    /// ================================================================================
    public class DtItems
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>ファイル - 係数</summary>
        private string _FileCoeff;

        /// <summary>用途地域</summary>
        private System.Data.DataTable _UseDistrict;

        /// <summary>部屋種類</summary>
        private System.Data.DataTable _RoomKind;

        /// <summary>排煙必要係数</summary>
        private System.Data.DataTable _SmokeNesCoeff;

        /// <summary>換気必要係数</summary>
        private System.Data.DataTable _VentilationNesCoeff;

        /// <summary>デフォルトの防煙壁長さ</summary>
        private System.Data.DataTable _DefaultSmokeWallLength;

        #endregion Memeber Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"  >属性</param>
        ///
        /// <history>2011/07/29 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public DtItems(RvtExtApp.Components.Attribute cmpAttribute)
        {
            // 初期化
            _CmpAttribute = cmpAttribute;

            string itemsFoldr = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // ファイル - 係数
            _FileCoeff = itemsFoldr + "\\" + _CmpAttribute.ResourceText("IDS_FILE_COEFFICIENT");
            if (System.IO.File.Exists(_FileCoeff) == false)
            {
                _FileCoeff = null;
            }
        }

        #endregion Constructor

        // メンバ関数

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>用途地域</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Data.DataTable UseDistrict
        {
            get
            {
                if (_UseDistrict == null)
                {
                    if (_FileCoeff != null)
                    {
                        _UseDistrict = new System.Data.DataTable();
                        string className = "UseDistrictList";
                        string subName = "Item";
                        Collections.Generic.IList<string> itemNames = new Collections.Generic.List<string>();
                        Collections.Generic.IList<System.Type> itemTypes = new Collections.Generic.List<System.Type>();

                        itemNames.Add("Name");
                        itemTypes.Add(typeof(string));
                        itemNames.Add("a");
                        itemTypes.Add(typeof(double));
                        itemNames.Add("b");
                        itemTypes.Add(typeof(double));
                        itemNames.Add("d");
                        itemTypes.Add(typeof(double));

                        _UseDistrict = UtilXml.GetXMLFile(_FileCoeff, className, subName, itemNames, itemTypes);
                    }
                }
                return _UseDistrict;
            }
        }

        /// ================================================================================
        /// <summary>部屋種類</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Data.DataTable RoomKind
        {
            get
            {
                if (_RoomKind == null)
                {
                    if (_FileCoeff != null)
                    {
                        _RoomKind = new System.Data.DataTable();
                        string className = "RoomKindList";
                        string subName = "Item";
                        Collections.Generic.IList<string> itemNames = new Collections.Generic.List<string>();
                        Collections.Generic.IList<System.Type> itemTypes = new Collections.Generic.List<System.Type>();

                        itemNames.Add("Name");
                        itemTypes.Add(typeof(string));
                        itemNames.Add("Value");
                        itemTypes.Add(typeof(double));

                        _RoomKind = UtilXml.GetXMLFile(_FileCoeff, className, subName, itemNames, itemTypes);
                    }
                }
                return _RoomKind;
            }
        }

        /// ================================================================================
        /// <summary>排煙必要係数</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Data.DataTable SmokeNesCoeff
        {
            get
            {
                if (_SmokeNesCoeff == null)
                {
                    if (_FileCoeff != null)
                    {
                        _SmokeNesCoeff = new System.Data.DataTable();
                        string className = "SmokeNecessaryCoefficient";
                        string subName = "Item";
                        Collections.Generic.IList<string> itemNames = new Collections.Generic.List<string>();
                        Collections.Generic.IList<System.Type> itemTypes = new Collections.Generic.List<System.Type>();

                        itemNames.Add("Name");
                        itemTypes.Add(typeof(string));
                        itemNames.Add("Value");
                        itemTypes.Add(typeof(double));

                        _SmokeNesCoeff = UtilXml.GetXMLFile(_FileCoeff, className, subName, itemNames, itemTypes);
                    }
                }
                return _SmokeNesCoeff;
            }
        }

        /// ================================================================================
        /// <summary>換気必要係数</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Data.DataTable VentilationNesCoeff
        {
            get
            {
                if (_VentilationNesCoeff == null)
                {
                    if (_FileCoeff != null)
                    {
                        _VentilationNesCoeff = new System.Data.DataTable();
                        string className = "VentilationNecessaryCoefficient";
                        string subName = "Item";
                        Collections.Generic.IList<string> itemNames = new Collections.Generic.List<string>();
                        Collections.Generic.IList<System.Type> itemTypes = new Collections.Generic.List<System.Type>();

                        itemNames.Add("Name");
                        itemTypes.Add(typeof(string));
                        itemNames.Add("Value");
                        itemTypes.Add(typeof(double));

                        _VentilationNesCoeff = UtilXml.GetXMLFile(_FileCoeff, className, subName, itemNames, itemTypes);
                    }
                }
                return _VentilationNesCoeff;
            }
        }

        /// ================================================================================
        /// <summary>デフォルトの防煙壁長さ</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Data.DataTable DefaultSmokeWallLength
        {
            get
            {
                if (_DefaultSmokeWallLength == null)
                {
                    if (_FileCoeff != null)
                    {
                        _DefaultSmokeWallLength = new System.Data.DataTable();
                        string className = "SmokeWallLengthDefault";
                        string subName = "Item";
                        Collections.Generic.IList<string> itemNames = new Collections.Generic.List<string>();
                        Collections.Generic.IList<System.Type> itemTypes = new Collections.Generic.List<System.Type>();

                        itemNames.Add("Name");
                        itemTypes.Add(typeof(string));
                        itemNames.Add("Value");
                        itemTypes.Add(typeof(double));

                        _DefaultSmokeWallLength = UtilXml.GetXMLFile(_FileCoeff, className, subName, itemNames, itemTypes);
                    }
                }
                return _DefaultSmokeWallLength;
            }
        }

        #endregion Properties
    }
}