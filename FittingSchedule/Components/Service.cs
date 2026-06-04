using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FittingSchedule;
using ADSK.JExtRAC.FittingSchedule.Components;

namespace ADSK.JExtRAC.FittingSchedule.Components
{
    /// ================================================================================
    /// <summary>サービス</summary>
    /// ================================================================================
    public class Service
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>要素</summary>
        private RvtExtApp.Components.Elements _CmpElements;

        /// <summary>図形</summary>
        private RvtExtApp.Components.Geometry _CmpGeometry;

        /// <summary>パラメータ</summary>
        private RvtExtApp.Components.Parameters _CmpParameters;

        /// <summary>設定</summary>
        private RvtExtApp.Components.Settings _CmpSettings;

        /// <summary>共有パラメータ - 建具タイプ</summary>
        private RvtExtApp.Entities.SpWinDoorType _EntSpWinDoorType;

        /// <summary>共有パラメータ - ビュー</summary>
        private RvtExtApp.Entities.SpView _EntSpView;

        #endregion Memeber Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"  >属性</param>
        /// <param name="cmpElements"   >要素</param>
        /// <param name="cmpGeometry"   >図形</param>
        /// <param name="cmpParameters" >パラメータ</param>
        /// <param name="cmpSettings"   >設定</param>
        /// <param name="entSpArea"     >共有パラメータ - 建具タイプ</param>
        /// <param name="entSpRoom"     >共有パラメータ - ビュー</param>
        ///
        /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Service(RvtExtApp.Components.Attribute cmpAttribute,
                       RvtExtApp.Components.Elements cmpElements,
                       RvtExtApp.Components.Geometry cmpGeometry,
                       RvtExtApp.Components.Parameters cmpParameters,
                       RvtExtApp.Components.Settings cmpSettings,
                       RvtExtApp.Entities.SpWinDoorType entSpWinDoorType,
                       RvtExtApp.Entities.SpView entSpView)
        {
            // 初期化
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;
            _EntSpWinDoorType = entSpWinDoorType;
            _EntSpView = entSpView;
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>建具姿図のビュー設定</summary>
        ///
        /// <param name="familyInstances"   >ファミリインスタンス</param>
        /// <param name="idTagSymbolDoor"   >ドアタグシンボルのID</param>
        /// <param name="idTagSymbolWindow" >窓タグシンボル</param>
        /// <param name="viewScale"         >ビュー縮尺</param>
        /// <param name="duplicateViewOpt"  ><p>ビューが重複している時のオプション</p>
        ///                                     <p>0=古いビューを削除</p>
        ///                                     <p>1=更新しない</p>
        ///                                     <p>2=古いビューをリネームする</p></param>
        /// <param name="progressBarThread" >プログレスバースレッド</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history><p>2011/07/28 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p>
        ///          <p>2017/10/10 Modified CST,Co.Ltd. Ryo Kuroda</p>
        ///          <p>2021/10/13 Modified Applied Technology</p></history>
        /// ================================================================================
        public
        bool SetPartsView(Collections.Generic.IList<Revit.DB.FamilyInstance> familyInstances,
                          int idTagSymbolDoor,
                          int idTagSymbolWindow,
                          int viewScale,
                          int detailLevel,
                          int duplicateViewOpt,
                          ref ProgressBarThread progressBarThread, ref string error)
        {
            bool ret = true;
            error = string.Empty;
            try
            {
                if (familyInstances == null)
                {
                    ret = false;
                    return ret;
                }

                // タグシンボル設定
                Revit.DB.ElementType tagSymbolDoor = _CmpElements.GetTagType(idTagSymbolDoor);
                if (tagSymbolDoor == null)
                {
                    ret = false;
                    return ret;
                }

                Revit.DB.ElementType tagSymbolWindow = _CmpElements.GetTagType(idTagSymbolWindow);
                if (tagSymbolWindow == null)
                {
                    ret = false;
                    return ret;
                }

                // ビュー範囲の余白値を設定
                double viewBBoxBlank = double.Parse(_CmpAttribute.ResourceText("IDS_SET_VIEWBBOXBLANK")) / _CmpGeometry.UnitCoe;

                int cntProgress = 0;
                progressBarThread.SetData(familyInstances.Count, cntProgress);

                Revit.DB.ViewFamilyType viewFamilyType = null;

                //断面図ビューファミリリストを取得
                Collections.Generic.IList<Revit.DB.ViewFamilyType> viewSecList = _CmpElements.GetViewFamilyTypes(Revit.DB.ViewFamily.Section);
                //新しいビューに適用されるテンプレートが<なし>のものを探す
                foreach (Revit.DB.ViewFamilyType viewFamType in viewSecList) {
                    if (Int32.Parse(viewFamType.DefaultTemplateId.ToString()) == -1) {
                        viewFamilyType = viewFamType;
                        break;
                    }
                }
                if(viewFamilyType == null) {  
                    error = "VIEWSEC_FAILD";
                    ret = false;
                    return ret; 
                }

                foreach (Revit.DB.FamilyInstance familyInstance in familyInstances)
                {
                    // 姿図名
                    string partsDrawName = " ";

                    // カテゴリ
                    Revit.DB.ElementType tagSymbol = null;

                    Revit.DB.BuiltInCategory categoryType = _CmpSettings.GetPartsSymbolType(familyInstance.Symbol);
                    if (categoryType == Revit.DB.BuiltInCategory.OST_Doors)
                    {
                        tagSymbol = tagSymbolDoor;
                        partsDrawName = _CmpAttribute.ResourceText("IDS_LST_SECVIEW_DOOR");
                    }
                    else if (categoryType == Revit.DB.BuiltInCategory.OST_Windows)
                    {
                        tagSymbol = tagSymbolWindow;
                        partsDrawName = _CmpAttribute.ResourceText("IDS_LST_SECVIEW_WINDOW");
                    }

                    // 建具名
                    string partsName = GetPartsName(familyInstance.Symbol);

                    // ビュー名
                    string viewName = SetPartsViewName(familyInstance.Symbol, partsName, partsDrawName);
                    if (CheckDuplicateView(ref viewName, duplicateViewOpt) == false)
                    {
                        continue;
                    }

                    // 描画範囲
                    Revit.DB.BoundingBoxXYZ partsBoundingBox = new Revit.DB.BoundingBoxXYZ();

                    if (GetPartsBoundingBox(familyInstance, detailLevel, viewBBoxBlank, ref partsBoundingBox) == false)
                    {
                        ret = false;
                        break;
                    }
                    
                    // ビュー作成
                    Revit.DB.ViewSection viewSec = Revit.DB.ViewSection.CreateSection(_CmpElements.RvtDBDoc, viewFamilyType.Id, partsBoundingBox);
                    viewSec.Name = viewName;

                    if (viewSec == null) {
                        ret = false;
                        break;
                    }

                    // 2017/10/10
                    // 以前はライブラリ内で処理
                    viewSec.CropBox = partsBoundingBox;

                    // タグ作成
                    Revit.DB.IndependentTag tag = _CmpElements.CreateTag(viewSec,
                                                                         familyInstance,
                                                                         tagSymbol);

                    // ビュー縮尺
                    viewSec.Scale = viewScale;
                    viewSec.DetailLevel = (Revit.DB.ViewDetailLevel)detailLevel;

                    _EntSpView.CurrentElem = viewSec;

                    // 記号非表示設定
                    _EntSpView.SignNotDisp = 1;

                    // 表示設定
                    Collections.Generic.IList<Revit.DB.Element> visElems = new Collections.Generic.List<Revit.DB.Element>();
                    visElems.Add(familyInstance);
                    visElems.Add(tag);
                    SetViewVisible(viewSec, visElems);

                    // シートタイトル登録

                    _EntSpView.TitleOnSheet = partsName;

                    progressBarThread.SetData(++cntProgress);
                }
                return ret;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        void
        GetFamilyInstanceBound(Revit.DB.FamilyInstance familyInstance, int viewDetailLevel, ref Revit.DB.BoundingBoxXYZ boundBox)
        {
            // 初期化
            Revit.DB.GeometryElement geomElem = familyInstance.get_Geometry(new Revit.DB.Options() { DetailLevel = (Revit.DB.ViewDetailLevel)viewDetailLevel, });
            Collections.Generic.IList<Revit.DB.XYZ> points = new Collections.Generic.List<Revit.DB.XYZ>();
            _CmpGeometry.GetPoints(geomElem, ref points);

            double minX = 0.0;
            double minY = 0.0;
            double minZ = 0.0;
            double maxX = 0.0;
            double maxY = 0.0;
            double maxZ = 0.0;

            // 最大値と最小値
            if (points.Count > 0)
            {
                Revit.DB.XYZ point = points[0];
                minX = point.X;
                minY = point.Y;
                minZ = point.Z;
                maxX = point.X;
                maxY = point.Y;
                maxZ = point.Z;
                if (points.Count > 1)
                {
                    for (int i = 1; i < points.Count; ++i)
                    {
                        point = points[i];
                        if (point.X < minX)
                        {
                            minX = point.X;
                        }
                        else if (point.X > maxX)
                        {
                            maxX = point.X;
                        }

                        if (point.Y < minY)
                        {
                            minY = point.Y;
                        }
                        else if (point.Y > maxY)
                        {
                            maxY = point.Y;
                        }

                        if (point.Z < minZ)
                        {
                            minZ = point.Z;
                        }
                        else if (point.Z > maxZ)
                        {
                            maxZ = point.Z;
                        }
                    }
                }
            }
            boundBox.Min = new Revit.DB.XYZ(minX, minY, minZ);
            boundBox.Max = new Revit.DB.XYZ(maxX, maxY, maxZ);
        }

        /// ================================================================================
        /// <summary>建具の領域を取得</summary>
        ///
        /// <param name="familyInstance">ファミリインスタンス</param>
        /// <param name="viewBBoxBlank" >領域の余白</param>
        /// <param name="boundingBox"   >建具の領域</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history><p>2011/07/28 Created GSA,Inc. Shinichi Ishii</p>
        ///          <p>2017/10/11 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        bool GetPartsBoundingBox(Revit.DB.FamilyInstance familyInstance,
                                     int viewDetailLevel,
                                     double viewBBoxBlank,
                                     ref Revit.DB.BoundingBoxXYZ boundingBox)
        {
            bool ret = true;

            if (boundingBox == null)
            {
                ret = false;
                return ret;
            }

            // ファミリシンボル
            Revit.DB.FamilySymbol familySymbol = familyInstance.Symbol;
            if (familySymbol == null)
            {
                ret = false;
                return ret;
            }

            // ファミリシンボル形状範囲
            Revit.DB.BoundingBoxXYZ symbolBBox = new Revit.DB.BoundingBoxXYZ();
            // 2022/02/02 詳細レベルが指定できるように修正
            GetFamilyInstanceBound(familyInstance, viewDetailLevel, ref symbolBBox);

            //余白
            symbolBBox.Min = new Revit.DB.XYZ(symbolBBox.Min.X - viewBBoxBlank,
                                              symbolBBox.Min.Y - viewBBoxBlank,
                                              symbolBBox.Min.Z - viewBBoxBlank);
            symbolBBox.Max = new Revit.DB.XYZ(symbolBBox.Max.X + viewBBoxBlank,
                                              symbolBBox.Max.Y + viewBBoxBlank,
                                              symbolBBox.Max.Z + viewBBoxBlank);

            // ビュー範囲
            boundingBox.Enabled = true;
            double widthHalf = System.Math.Abs(symbolBBox.Max.X - symbolBBox.Min.X) * 0.5;
            double heightHalf = System.Math.Abs(symbolBBox.Max.Z - symbolBBox.Min.Z) * 0.5;
            double depth = System.Math.Abs(symbolBBox.Max.Y - symbolBBox.Min.Y);

            boundingBox.Min = new Revit.DB.XYZ(-widthHalf, -heightHalf, -depth);
            boundingBox.Max = new Revit.DB.XYZ(widthHalf, heightHalf, depth);

            // フリップ対応
            Revit.DB.Transform instanceTransform = _CmpGeometry.GetElemTransform(familyInstance);
            double normalFlag = 1.0;
            if (_CmpGeometry.Distance2D(instanceTransform.BasisY, familyInstance.FacingOrientation) > _CmpGeometry.Approx0Len)
            {
                normalFlag = -1.0;
            }

            // 座標変換
            Revit.DB.XYZ basisX = new Revit.DB.XYZ(instanceTransform.BasisX.X, instanceTransform.BasisX.Y, instanceTransform.BasisX.Z);
            Revit.DB.XYZ basisY = new Revit.DB.XYZ(0, 0, 1);
            Revit.DB.XYZ basisZ = new Revit.DB.XYZ(basisX.Y, -basisX.X, basisX.Z);

            Revit.DB.Transform transform = Revit.DB.Transform.Identity;
            transform.set_Basis(0, basisX);
            transform.set_Basis(1, basisY);
            transform.set_Basis(2, basisZ);

            // 位置
            double posX = symbolBBox.Min.X;
            double posY = (System.Math.Abs(symbolBBox.Min.Y) < System.Math.Abs(symbolBBox.Max.Y) ? symbolBBox.Max.Y : System.Math.Abs(symbolBBox.Min.Y)) * normalFlag;
            double posZ = symbolBBox.Min.Z + (heightHalf * 2.0);

            basisX = new Revit.DB.XYZ(posX * instanceTransform.BasisX.X, posX * instanceTransform.BasisX.Y, posX * instanceTransform.BasisX.Z);
            basisY = new Revit.DB.XYZ(posY * instanceTransform.BasisY.X, posY * instanceTransform.BasisY.Y, posY * instanceTransform.BasisY.Z);
            basisZ = new Revit.DB.XYZ(posZ * instanceTransform.BasisZ.X, posZ * instanceTransform.BasisZ.Y, posZ * instanceTransform.BasisZ.Z);
            posX = basisX.X + basisY.X + basisZ.X;
            posY = basisX.Y + basisY.Y + basisZ.Y;
            posZ = basisX.Z + basisY.Z + basisZ.Z;

            transform.Origin = new Revit.DB.XYZ(instanceTransform.Origin.X + posX,
                                                instanceTransform.Origin.Y + posY,
                                                instanceTransform.Origin.Z + posZ);

            if (normalFlag == 1.0)
            {
                Revit.DB.Transform transformRotate = Revit.DB.Transform.CreateRotationAtPoint(new Revit.DB.XYZ(0.0, 1.0, 0.0),
                                                                                              System.Math.PI,
                                                                                              new Revit.DB.XYZ(widthHalf, 0.0, 0.0));

                transform = transform.Multiply(transformRotate);
            }

            boundingBox.Transform = transform;

            return ret;
        }

        /// ================================================================================
        /// <summary>建具名を取得</summary>
        ///
        /// <param name="familySymbol">ファミリシンボル</param>
        ///
        /// <returns>建具名</returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string GetPartsName(Revit.DB.FamilySymbol familySymbol)
        {
            _EntSpWinDoorType.CurrentElem = familySymbol;

            // 建具記号
            string partsMark = _EntSpWinDoorType.Mark;
            if (partsMark == null)
            {
                partsMark = " ";
            }
            // 建具番号
            string partsNumber = _EntSpWinDoorType.No;
            if (partsNumber == null)
            {
                partsNumber = " ";
            }
            return partsMark + partsNumber;
        }

        /// ================================================================================
        /// <summary>建具のビュー名を設定</summary>
        ///
        /// <param name="familySymbol"  >ファミリシンボル</param>
        /// <param name="partsName"     >建具名</param>
        /// <param name="partsDrawName" >建具姿図の接頭語</param>
        ///
        /// <returns>建具のビュー名</returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string SetPartsViewName(Revit.DB.FamilySymbol familySymbol, string partsName, string partsDrawName)
        {
            // 姿図
            string viewName = partsDrawName;

            // 建具記号・番号
            viewName += "_" + partsName;

            // ファミリ名
            string familyName = familySymbol.Family.Name;
            viewName += "_" + familyName;

            // タイプ名
            string typeName = familySymbol.Name;
            viewName += "_" + typeName;

            return viewName;
        }

        /// ================================================================================
        /// <summary>ビューの可視を設定</summary>
        ///
        /// <param name="view"    >ビュー</param>
        /// <param name="visElems">可視の要素</param>
        ///
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        void SetViewVisible(Revit.DB.View view, Collections.Generic.IList<Revit.DB.Element> visElems)
        {
            if (view == null)
            {
                return;
            }

            _EntSpView.CurrentElem = view;

            // CropBox設定
            view.CropBoxActive = true;
            view.CropBoxVisible = false;

            // Far Clipping設定
            _EntSpView.FarClipping = 2;

            if (visElems == null)
            {
                return;
            }

            // 対象要素を表示
            Collections.Generic.IList<int> visElemsID = new Collections.Generic.List<int>();
            Collections.Generic.IList<Revit.DB.ElementId> unHideElemIds = new Collections.Generic.List<Revit.DB.ElementId>();
            foreach (Revit.DB.Element elem in visElems)
            {
                visElemsID.Add(Int32.Parse(elem.Id.ToString()));

                if (elem.IsHidden(view) == true)
                {
                    unHideElemIds.Add(elem.Id);
                }
            }
            if (unHideElemIds.Count > 0)
            {
                view.UnhideElements(unHideElemIds);
            }

            // ビューの要素
            Collections.Generic.IList<Revit.DB.Element> viewElems = _CmpElements.GetViewElements(view);

            // 非表示要素
            Collections.Generic.IList<Revit.DB.ElementId> hidenElemIds = new Collections.Generic.List<Revit.DB.ElementId>();
            foreach (Revit.DB.Element elem in viewElems)
            {
                if (elem.IsHidden(view) == false)
                {
                    if (elem.CanBeHidden(view) == true)
                    {
                        if (visElemsID.Contains(Int32.Parse(elem.Id.ToString())) == false)
                        {
                            hidenElemIds.Add(elem.Id);
                        }
                    }
                }
            }

            if (hidenElemIds.Count > 0)
            {
                view.HideElements(hidenElemIds);
            }
        }

        /// ================================================================================
        /// <summary>ビューの重複をチェック</summary>
        ///
        /// <param name="viewName"        >ビュー名</param>
        /// <param name="duplicateViewOpt"><p>ビューが重複している時のオプション</p>
        ///                                   <p>0=古いビューを削除</p>
        ///                                   <p>1=更新しない</p>
        ///                                   <p>2=古いビューをリネームする</p></param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = ビューの処理をする</p>
        ///             <p>False = ビューの処理をしない</p></returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        bool CheckDuplicateView(ref string viewName, int duplicateViewOpt)
        {
            bool ret = true;

            // 建具のビュー
            Collections.Generic.IList<Revit.DB.Element> elems = _CmpElements.ElementsViewSectionParts;
            if (elems != null)
            {
                Revit.DB.Element duplicateElem = null;

                // 要素
                foreach (Revit.DB.Element elem in elems)
                {
                    // ビュー名の重複チェック
                    if (elem.Name == viewName)
                    {
                        duplicateElem = elem;
                        break;
                    }
                }

                // ビューが重複している時の処理
                if (duplicateElem != null)
                {
                    switch (duplicateViewOpt)
                    {
                        case 0:
                            _CmpElements.RvtDBDoc.Delete(duplicateElem.Id);
                            break;

                        case 1:
                            ret = false;
                            break;

                        case 2:
                            int index = CheckDuplicateView(viewName, elems);
                            duplicateElem.Name += "_" + index.ToString();
                            break;
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>ビューの重複をチェック(オーバーロード)</summary>
        ///
        /// <param name="viewName">ビュー名</param>
        /// <param name="elems"   >要素</param>
        ///
        /// <returns>重複ビューのインクリメント</returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int CheckDuplicateView(string viewName, Collections.Generic.IList<Revit.DB.Element> elems)
        {
            int retVal = 0;
            int maxVal = 0;

            // 要素
            foreach (Revit.DB.Element elem in elems)
            {
                // 要素名とビュー名を比較
                if (elem.Name.Length > (viewName.Length + 1))
                {
                    if (elem.Name.Substring(0, viewName.Length) == viewName)
                    {
                        string sign = elem.Name.Substring(viewName.Length, 1);
                        string valStr = elem.Name.Substring(viewName.Length + 1);
                        if (sign == "_")
                        {
                            if (UtilValue.IsInteger(valStr) == true)
                            {
                                int val = int.Parse(valStr);
                                if (val > maxVal)
                                {
                                    maxVal = val;
                                }
                            }
                        }
                    }
                }
            }
            retVal = maxVal + 1;
            return retVal;
        }

        /// ================================================================================
        /// <summary>断面図ビューを取得</summary>
        ///
        /// <param name="table"             >テーブルデータ</param>
        /// <param name="viewSections"      >断面図ビュー</param>
        /// <param name="viewsLayoutStatus" >ビューのレイアウト状態</param>
        /// <param name="progressBarThread" >プログレスバースレッド</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/10/13 Modified  Applied Technology</history>
        /// ================================================================================
        public
        bool GetViewSection(System.Data.DataTable table,
                            ref Collections.Generic.IList<Revit.DB.ViewSection> viewSections,
                            ref Collections.Generic.IList<int> viewsLayoutStatus,
                            ref ProgressBarThread progressBarThread, ref string errMsg)
        {
            bool ret = true;
            try
            {
                if (table == null)
                {
                    errMsg = "GetViewSection: DataTable is null.";
                    ret = false;
                    return ret;
                }

                if (table.Rows.Count == 0)
                {
                    errMsg = "No elevation views are available for layout.\n\n"
                           + "Please create door/window elevation views first using 'Create Window/Door View', "
                           + "then return to this command to place them on the sheet.";
                    ret = false;
                    return ret;
                }

                int cntProgress = 0;
                progressBarThread.SetData(table.Rows.Count, cntProgress);
                var errors = new System.Text.StringBuilder();

                for (int i = 0; i < table.Rows.Count; ++i)
                {
                    System.Data.DataRow row = table.Rows[i];
                    string idStr = row[0].ToString();
                    string nameStr = row.ItemArray.Length > 1 ? row[1].ToString() : "";
                    int id = -1;
                    if (UtilValue.IsInteger(idStr))
                    {
                        id = int.Parse(idStr);
                    }

                    if (id != 0)
                    {
                        Revit.DB.ViewSection viewSection = _CmpElements.GetViewSection(id);
                        if (viewSection != null)
                        {
                            viewSections.Add(viewSection);
                            viewsLayoutStatus.Add(1);
                        }
                        else
                        {
                            errors.AppendLine("View not found: '" + nameStr + "' [ID:" + id + "]");
                        }
                    }
                    else
                    {
                        viewsLayoutStatus.Add(0);
                    }
                    progressBarThread.SetData(++cntProgress);
                }

                if (viewSections.Count == 0)
                {
                    errMsg = "GetViewSection: No valid ViewSection elements found.\n" + errors.ToString();
                    ret = false;
                }
                else if (errors.Length > 0)
                {
                    errMsg = errors.ToString();
                }

                return ret;
            }
            catch (Exception ex)
            {
                errMsg = "GetViewSection exception: " + ex.Message;
                return false;
            }
        }

        /// ================================================================================
        /// <summary>建具姿図ビューのレイアウトを設定</summary>
        ///
        /// <param name="viewSheet"         >シートビュー</param>
        /// <param name="blankTop"          >上の余白</param>
        /// <param name="blankBottom"       >下の余白</param>
        /// <param name="blankLeft"         >左の余白</param>
        /// <param name="blankRight"        >右の余白</param>
        /// <param name="viewSections"      >断面図ビューリスト</param>
        /// <param name="viewsLayoutStatus" >ビューのレイアウト状態</param>
        /// <param name="progressBarThread" >プログレスバースレッド</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p>
        ///          <p>2021/10/13 Modified Applied Technology</p></history>
        /// ================================================================================
        public
        bool SetLayoutPartsView(Revit.DB.ViewSheet viewSheet,
                                int blankTop, int blankBottom, int blankLeft, int blankRight,
                                ref Collections.Generic.IList<Revit.DB.ViewSection> viewSections,
                                ref Collections.Generic.IList<int> viewsLayoutStatus,
                                ref ProgressBarThread progressBarThread, ref string erro)
        {
            bool ret = true;
            erro = string.Empty;
            try
            {
                if (viewSheet == null)
                {
                    erro = "ViewSheet is null.";
                    ret = false;
                    return ret;
                }

                if (viewSections == null || viewSections.Count == 0)
                {
                    erro = "No section views to layout. viewSections count: "
                         + (viewSections == null ? "null" : viewSections.Count.ToString());
                    ret = false;
                    return ret;
                }

                // シート描画範囲
                double leftPos = 0.0;
                double bottomPos = 0.0;
                double rangeX = 0.0;
                double rangeY = 0.0;
                GetViewSheetRange(viewSheet,
                                  blankTop, blankBottom, blankLeft, blankRight,
                                  ref leftPos, ref bottomPos, ref rangeX, ref rangeY);

                // ビュー配置
                int cntProgress = 0;
                progressBarThread.SetData(viewsLayoutStatus.Count, cntProgress);
                int viewCount = -1;
                double posX = leftPos;
                double posY = bottomPos;
                double bottomPosDist = 0.0;
                int placedCount = 0;
                var errors = new System.Text.StringBuilder();

                for (int i = 0; i < viewsLayoutStatus.Count; ++i)
                {
                    // 改行
                    if (viewsLayoutStatus[i] == 0)
                    {
                        bottomPos -= bottomPosDist;
                        posX = leftPos;
                        posY = bottomPos;
                        bottomPosDist = 0.0;
                    }
                    else
                    {
                        // ビュー範囲
                        viewCount++;
                        if (viewCount >= viewSections.Count)
                        {
                            errors.AppendLine("viewCount (" + viewCount + ") exceeded viewSections.Count (" + viewSections.Count + ")");
                            break;
                        }

                        var currentView = viewSections[viewCount];
                        double viewWidth = 0.0;
                        double viewHeight = 0.0;
                        double viewCenterX = 0.0;
                        double viewCenterY = 0.0;
                        GetViewRange(currentView, ref viewWidth, ref viewHeight, ref viewCenterX, ref viewCenterY);
                        double viewWidthHalfLeft = (viewWidth * 0.5) - viewCenterX;
                        double viewHeightHalfTop = (viewHeight * 0.5) + viewCenterY;
                        double viewWidthHalfRight = (viewWidth * 0.5) + viewCenterX;
                        double viewHeightHalfBottom = (viewHeight * 0.5) - viewCenterY;

                        posX += viewWidthHalfLeft;
                        posY = bottomPos - viewHeightHalfTop;

                        // 自動改行
                        if (posX + viewWidthHalfRight > rangeX)
                        {
                            bottomPos -= bottomPosDist;
                            posX = leftPos + viewWidthHalfLeft;
                            posY = bottomPos - viewHeightHalfTop;
                            bottomPosDist = 0.0;
                        }

                        if (posY - viewHeightHalfBottom < rangeY)
                        {
                            errors.AppendLine("View '" + currentView.Name + "' exceeds sheet bounds, stopping layout.");
                            break;
                        }

                        // ビュー配置位置
                        double viewLocX = posX + viewCenterX;
                        double viewLocY = posY + viewCenterY;

                        // ビューポート作成
                        try
                        {
                            Revit.DB.XYZ location = new Revit.DB.XYZ(viewLocX, viewLocY, 0.0);
                            Revit.DB.Viewport.Create(_CmpElements.RvtDBDoc, viewSheet.Id, currentView.Id, location);
                            placedCount++;
                        }
                        catch (Exception vpEx)
                        {
                            errors.AppendLine("Failed to place '" + currentView.Name + "' [ID:" + currentView.Id + "] on sheet: " + vpEx.Message);
                        }

                        posX += viewWidthHalfRight;

                        // 上下間隔距離
                        if ((posY - viewHeightHalfBottom) < (bottomPos - bottomPosDist))
                        {
                            bottomPosDist = bottomPos - (posY - viewHeightHalfBottom);
                        }
                    }

                    progressBarThread.SetData(++cntProgress);
                }

                if (errors.Length > 0)
                {
                    erro = errors.ToString();
                    if (placedCount == 0)
                        ret = false;
                }

                return ret;
            }
            catch (Exception ex)
            {
                erro = "SetLayoutPartsView exception: " + ex.Message;
                return false;
            }
        }

        /// ================================================================================
        /// <summary>ビューポートを設定</summary>
        ///
        /// <param name="viewSheet"         >シートビュー</param>
        /// <param name="viewSections"      >断面図ビュー</param>
        /// <param name="progressBarThread" >プログレスバースレッド</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/10/13 Modified Applied Technology</p></history>
        /// ================================================================================
        public
        bool SetViewPort(Revit.DB.ViewSheet viewSheet,
                         ref Collections.Generic.IList<Revit.DB.ViewSection> viewSections,
                         ref ProgressBarThread progressBarThread, ref string erro)
        {
            bool ret = true;
            erro = string.Empty;
            try
            {
                if (viewSheet == null)
                {
                    erro = "SetViewPort: ViewSheet is null.";
                    ret = false;
                    return ret;
                }

                if (viewSections == null)
                {
                    erro = "SetViewPort: viewSections is null.";
                    ret = false;
                    return ret;
                }

                // ビューポートシンボル
                Revit.DB.ElementType viewPortSymbol = _CmpElements.ViewPortSymbolNoTitle;

                // ビューポート
                Collections.Generic.IList<Revit.DB.Element> viewPorts = _CmpElements.GetViewPorts(viewSheet.Name, viewSections);

                if (viewPorts == null || viewPorts.Count == 0)
                {
                    erro = "SetViewPort: No viewports found on sheet '" + viewSheet.Name
                         + "' for the placed views. viewPorts count: "
                         + (viewPorts == null ? "null" : viewPorts.Count.ToString());
                    return ret;
                }

                if (viewPortSymbol == null)
                {
                    // Collect diagnostic info about available viewport types
                    var diagInfo = new System.Text.StringBuilder();
                    diagInfo.AppendLine("SetViewPort: 'No Title' viewport symbol not found. Viewports placed but title not removed.");
                    diagInfo.AppendLine("Available viewport types in project:");
                    try
                    {
                        var allVpTypes = new Revit.DB.FilteredElementCollector(_CmpElements.RvtDBDoc)
                            .OfClass(typeof(Revit.DB.ElementType));
                        foreach (var e in allVpTypes)
                        {
                            var t = e as Revit.DB.ElementType;
                            if (t != null && t.FamilyName != null &&
                                t.FamilyName.ToLowerInvariant().Contains("viewport"))
                            {
                                diagInfo.AppendLine("  FamilyName=\"" + t.FamilyName + "\" Name=\"" + t.Name + "\" Id=" + t.Id);
                            }
                        }
                    }
                    catch { }
                    erro = diagInfo.ToString();
                    return ret;
                }

                // ビューポートシンボル変更
                int cntProgress = 0;
                progressBarThread.SetData(viewPorts.Count, cntProgress);
                var errors = new System.Text.StringBuilder();

                foreach (Revit.DB.Element elem in viewPorts)
                {
                    try
                    {
                        elem.ChangeTypeId(viewPortSymbol.Id);
                    }
                    catch (Exception vpEx)
                    {
                        errors.AppendLine("Failed to change viewport type for [ID:" + elem.Id + "]: " + vpEx.Message);
                    }
                    progressBarThread.SetData(++cntProgress);
                }

                if (errors.Length > 0)
                    erro = errors.ToString();

                return ret;
            }
            catch (Exception ex)
            {
                erro = "SetViewPort exception: " + ex.Message;
                return false;
            }
        }

        /// ================================================================================
        /// <summary>シートビューの範囲を取得</summary>
        ///
        /// <param name="viewSheet"   >シートビュー</param>
        /// <param name="blankTop"    >上の余白</param>
        /// <param name="blankBottom" >下の余白</param>
        /// <param name="blankLeft"   >左の余白</param>
        /// <param name="blankRight"  >右の余白</param>
        /// <param name="leftPos"     >ビューの配置の左位置</param>
        /// <param name="bottomPos"   >ビューの配置の下位置</param>
        /// <param name="rangeX"      >シートビューの範囲 X</param>
        /// <param name="rangeY"      >シートビューの範囲 Y</param>]
        ///
        /// <history>2011/07/29 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetViewSheetRange(Revit.DB.ViewSheet viewSheet,
                               int blankTop, int blankBottom, int blankLeft, int blankRight,
                               ref double leftPos,
                               ref double bottomPos,
                               ref double rangeX,
                               ref double rangeY)
        {
            double coeBlankTop = (double)blankTop / _CmpGeometry.UnitCoe;
            double coeBlankBottom = (double)blankBottom / _CmpGeometry.UnitCoe;
            double coeBlankLeft = (double)blankLeft / _CmpGeometry.UnitCoe;
            double coeBlankRight = (double)blankRight / _CmpGeometry.UnitCoe;

            Revit.DB.BoundingBoxUV boundingBoxUV = viewSheet.Outline;
            double width = System.Math.Abs(boundingBoxUV.Max.U - boundingBoxUV.Min.U) - (coeBlankLeft + coeBlankRight);
            double height = System.Math.Abs(boundingBoxUV.Max.V - boundingBoxUV.Min.V) - (coeBlankTop + coeBlankBottom);

            leftPos = boundingBoxUV.Min.U + coeBlankLeft;
            bottomPos = boundingBoxUV.Max.V - coeBlankTop;

            rangeX = boundingBoxUV.Max.U - coeBlankRight;
            rangeY = boundingBoxUV.Min.V + coeBlankBottom;
        }

        /// ================================================================================
        /// <summary>ビューの範囲を取得</summary>
        ///
        /// <param name="view"    >ビュー</param>
        /// <param name="width"   >ビューの幅</param>
        /// <param name="height"  >ビューの高さ</param>
        /// <param name="centerX" >ビューの中心X</param>
        /// <param name="centerY" >ビューの中心Y</param>
        ///
        /// <history>2011/07/29 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetViewRange(Revit.DB.View view,
                          ref double width, ref double height,
                          ref double centerX, ref double centerY)
        {
            if (view != null)
            {
                Revit.DB.BoundingBoxUV boundingBox = view.Outline;
                width = System.Math.Abs(boundingBox.Max.U - boundingBox.Min.U);
                height = System.Math.Abs(boundingBox.Max.V - boundingBox.Min.V);

                centerX = boundingBox.Max.U - (width * 0.5);
                centerY = boundingBox.Max.V - (height * 0.5);
            }
        }

        #endregion Member Functions

        // プロパティ
    }
}