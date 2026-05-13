using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
namespace SectionListRC.Entities
{
    /// ================================================================================
    /// <summary>共有パラメータ - コマンド</summary>
    /// ================================================================================
    public class SpCmd : SectionListRC.Entities.SpBase
    {
        // メンバ変数

        #region Memeber Variables

        //プロジェクト情報
        private Revit.DB.ProjectInfo _elemProjInfo;

        // パラメータ名
        private string _ParamNameCmd;

        // 項目数
        private int _ItemNum;

        #endregion Memeber Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"  >属性</param>
        /// <param name="cmpParameters" >パラメータ</param>
        /// <param name="cmpSettings"   >設定</param>
        /// <param name="projInfo"      >プロジェクト情報</param>
        /// <param name="defName"       >定義名</param>
        /// <param name="itemNum"       >項目数</param>
        ///
        /// <history><p>2013/04/03 Created GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public SpCmd(SectionListRC.Components.Attribute cmpAttribute,
                     SectionListRC.Components.Parameters cmpParameters,
                     SectionListRC.Components.Settings cmpSettings,
                     Revit.DB.ProjectInfo elemProjInfo,
                     string defName,
                     int itemNum) :
                base(cmpAttribute, cmpParameters, cmpSettings)
        {
            _elemProjInfo = elemProjInfo;
            _ParamNameCmd = defName;
            _ItemNum = itemNum;

            base.DefSuccess = SetDef();
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>定義設定</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2013/04/05 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        private
        bool SetDef()
        {
            bool ret = base.CmpParameters.SetDefinition(null,
                                                    base.CmpSettings.CategoryProjInfo,
                                                    _ParamNameCmd,
                                                    Revit.DB.SpecTypeId.String.Text,
                                                    new Revit.DB.ForgeTypeId(string.Empty),
                                                    false,
                                                    0);
            return ret;
        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <returns>データ</returns>
        ///
        /// <history>2013/04/05 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> GetData()
        {
            string sValue = "";
            Collections.Generic.IList<string> valueSplit;

            // 戻り値
            Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();

            // データ分割
            base.CmpParameters.GetValueString(_elemProjInfo,
                                              _ParamNameCmd,
                                              Revit.DB.SpecTypeId.String.Text,
                                              new Revit.DB.ForgeTypeId(string.Empty),
                                              ref sValue);

            if (sValue == null)
            {
                foreach (Revit.DB.Parameter p in _elemProjInfo.Parameters)
                {
                    if (p.Definition.Name == _ParamNameCmd)
                    {
                        sValue = p.AsString();
                    }
                }
            }

            valueSplit = SectionListRC.JExtComCompat.UtilValue.SplitString(sValue, ",");

            bool flag = false;
            if (_ItemNum == valueSplit.Count)
            {
                flag = true;
            }

            // 値取得
            if (_ItemNum > 0)
            {
                for (int i = 0; i < _ItemNum; ++i)
                {
                    if (flag == true)
                    {
                        ret.Add(valueSplit[i]);
                    }
                    else
                    {
                        ret.Add("");
                    }
                }
            }
            else
            {
                if (valueSplit.Count > 0)
                {
                    for (int i = 0; i < valueSplit.Count; ++i)
                    {
                        ret.Add(valueSplit[i]);
                    }
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <returns>データ</returns>
        ///
        /// <history>2013/04/05 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool SetData(Collections.Generic.IList<string> value)
        {
            string valueStr = null;
            string separator = ",";

            // 戻り値
            bool ret = false;

            // 値設定
            if (value != null)
            {
                foreach (string str in value)
                {
                    valueStr += str + separator;
                }
            }

            if (valueStr != null)
            {
                valueStr = valueStr.Substring(0, valueStr.Length - 1);
            }

            // パラメータ値設定
            if (valueStr != null)
            {
                base.CmpParameters.SetValue(_elemProjInfo,
                                            _ParamNameCmd,
                                            Revit.DB.SpecTypeId.String.Text,
                                            new Revit.DB.ForgeTypeId(string.Empty),
                                            valueStr);
                ret = true;
            }
            return ret;
        }

        #endregion Member Functions
    }
}