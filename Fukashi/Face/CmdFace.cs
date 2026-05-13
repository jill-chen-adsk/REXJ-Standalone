using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using RvtExtApp   = ADSK.Ext.Fukashi;

namespace ADSK.Ext.Fukashi.Face
{
  /// ================================================================================
  /// <summary>コマンド 面フカシ</summary>
  /// ================================================================================
  [Revit.Attributes.Transaction(Revit.Attributes.TransactionMode.Manual)]
  [Revit.Attributes.Regeneration(Revit.Attributes.RegenerationOption.Manual)]
  public class CmdFace : Revit.UI.IExternalCommand
  {
    // メンバ関数
    #region Member Functions
    /// ================================================================================
    /// <summary>コマンド実行処理</summary>
    /// 
    /// <param name="commandData" >Revit コマンドデータ</param>
    /// <param name="message"     >エラーメッセージ</param>
    /// <param name="elements"    >エラー要素</param>
    /// 
    /// <returns>実行結果</returns>
    /// 
    /// <history><p>2016/11/17 Created CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2017/01/24 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Revit.UI.Result Execute(Revit.UI.ExternalCommandData commandData,
                            ref string message,
                            Revit.DB.ElementSet elements)
    {
        // 初期化
        Revit.UI.UIApplication                rvtUIApp  = commandData.Application;
        Revit.UI.UIDocument                   rvtUIDoc  = rvtUIApp.ActiveUIDocument;
        Revit.DB.Document                     rvtDBDoc  = rvtUIDoc.Document;
        Revit.ApplicationServices.Application rvtSvcApp = rvtDBDoc.Application;

        RvtExtApp.Face.Components.Attribute  cmpAttribute  = new RvtExtApp.Face.Components.Attribute();
        RvtExtApp.Face.Components.Elements   cmpElements   = new RvtExtApp.Face.Components.Elements(rvtUIDoc, cmpAttribute);
        RvtExtApp.Face.Components.Geometry   cmpGeometry   = new RvtExtApp.Face.Components.Geometry(rvtUIDoc, cmpAttribute);
        RvtExtApp.Face.Components.Parameters cmpParameters = new RvtExtApp.Face.Components.Parameters(rvtUIDoc, cmpAttribute);
        RvtExtApp.Face.Components.Settings   cmpSettings   = new RvtExtApp.Face.Components.Settings(rvtUIDoc);
        RvtExtApp.Face.Components.Service    cmpService    = new RvtExtApp.Face.Components.Service(cmpAttribute,
                                                                                        cmpElements,
                                                                                        cmpGeometry,
                                                                                        cmpParameters,
                                                                                        cmpSettings);
        RvtExtApp.Face.Components.UI cmpUI = new RvtExtApp.Face.Components.UI(cmpAttribute,
                                                                    rvtUIApp);

        
        // 戻り値
        Revit.UI.Result retCmd = Revit.UI.Result.Cancelled;
//SolidIntersect4(rvtDBDoc, rvtUIDoc, cmpElements, cmpGeometry);
//return retCmd;

        // トランザクショングループ
        Revit.DB.TransactionGroup transGrp = new Revit.DB.TransactionGroup(rvtDBDoc);
        transGrp.Start(cmpAttribute.ResourceText("IDS_TXT_FUKASHI_FACEPICK"));
        // トランザクション
        Revit.DB.Transaction trans = new Revit.DB.Transaction(rvtDBDoc);
            
        // ワークフロー
        trans.Start(cmpAttribute.ResourceText("IDS_TXT_FLOW"));
        string retMsg = cmpService.WorkFlow();
        trans.Commit();

        if (retMsg != null)
        {
            System.Windows.Forms.MessageBox.Show(retMsg);
            retCmd = Revit.UI.Result.Failed;
        }

        // マテリアル
        string cmbBoxVal = cmpUI.GetCurrentMaterialCmbBoxValue();
        int id = 0;
        if (int.TryParse(cmbBoxVal, out id) == false)
        {
            System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_GETMATERIAL"),
                                                    cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));

            transGrp.RollBack();
            return retCmd;
        }

        // リボン無効化
        trans.Start("リボン無効化");
        cmpUI.SetRibbonEnable(false);
        trans.Commit();

        // フカシファミリ取得
        trans.Start("フカシファミリ取得");
        cmpElements.GetFukashiFamily();
        trans.Commit();


        if (id > 0)
        {
            Revit.DB.ElementId materialId = new Revit.DB.ElementId(id);
            cmpElements.MaterialId = materialId;
        }
        else
        {
            Revit.DB.ElementId materialId = Revit.DB.ElementId.InvalidElementId;
            cmpElements.MaterialId = materialId;
        }

        string appId      = System.Reflection.Assembly.GetExecutingAssembly().GetType().GUID.ToString();
        string appDataId  = System.Guid.NewGuid().ToString();

        Revit.DB.Reference pickObj = null;
        Collections.Generic.ICollection<Revit.DB.Reference> pickObjs = new Collections.Generic.List<Revit.DB.Reference>();

        //範囲指定で要素選択
        while (true)
        {
            try
            {
                String msg = cmpAttribute.ResourceText("IDS_TXT_PICK_JOINSOLID");
                pickObjs = rvtUIDoc.Selection.PickObjects(Revit.UI.Selection.ObjectType.Element, msg);

            }
            catch (Revit.Exceptions.OperationCanceledException)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_FINISHCMD"),
                                                        cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));

                // リボン有効化
                trans.Start("リボン有効化");
                cmpUI.SetRibbonEnable(true);
                trans.Commit();
                transGrp.RollBack();
                return retCmd;
            }
            if (pickObjs.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_PICK_YOUSO"),
                                                    cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));


            }
            else
                break;
        }

        // 基準面選択
        while(true)
        { 
            try
            {
                // 基準面選択
                String msg = cmpAttribute.ResourceText("IDS_TXT_PICK_KIJUNFACE");
                pickObj = rvtUIDoc.Selection.PickObject(Revit.UI.Selection.ObjectType.Face, msg);

            }
            catch (Revit.Exceptions.OperationCanceledException)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_FINISHCMD"),
                                                    cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));

                break;
            }

            // 選択点
            Revit.DB.XYZ pickPos = pickObj.GlobalPoint;
            cmpService.pcpos = pickPos;

            // 選択要素
            Revit.DB.Element elem = rvtDBDoc.GetElement(pickObj);
            // 選択カテゴリ
            Revit.DB.Category category = elem.Category;
            cmpElements.ElemCategory = category;

            //選択対象カテゴリチェック
            if (!cmpElements.CheckTargetCategory())
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SELECT_TARGET"),
                                                    cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
                continue;
            }
            if (elem.Name.Contains("フカシファミリ"))
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SELECT_TARGET"),
                                                    cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
                continue;
            }

            // グラフィックススタイルID
            Revit.DB.ElementId graphicsStyleId = cmpGeometry.GetGraphicsStyleId(elem);
            cmpElements.GraphicsStyleId = graphicsStyleId;

            // 選択面
            Revit.DB.PlanarFace plnFace = cmpGeometry.GetPlnFace(elem, pickPos);
            Revit.DB.PlanarFace plnFaceBase = plnFace;
            Collections.Generic.IList<Revit.DB.Curve> curvesA = new Collections.Generic.List<Revit.DB.Curve>();


            //範囲指定した要素(pickObjs)と、面指定した要素を合成
            Collections.Generic.ICollection<Revit.DB.Reference> pickObjsAdd = new Collections.Generic.List<Revit.DB.Reference>();
            pickObjsAdd = pickObjs;
            //基準面選択部材は追加しないpickObjsAdd.Add(pickObj);

            Revit.DB.Solid baseSolid = cmpGeometry.GetUnionSolid(pickObjsAdd);
            if(baseSolid == null)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_ERRSOLID_FINISHCMD"),
                                                        cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
                break;
            }

            //ピック点位ら、合成したSOLIDのFACEを検索
            Revit.DB.PlanarFace retPFace = cmpGeometry.SeachPickFace(baseSolid, pickPos);

            //ピック点位ら、合成したSOLID-FACEのエッジアレイを検索
            Revit.DB.PlanarFace retPln = null;
            cmpGeometry.SolidIntersect3(baseSolid, pickObj, pickPos, ref curvesA, ref retPln);
            curvesA = cmpGeometry.OptimizeLineVertexNoConvLine(curvesA);


            //DEBUG/////
            //using (Revit.DB.Transaction t = new Revit.DB.Transaction(rvtDBDoc, "Create tessellated direct shape"))
            //{
            //    //DirectShapeで形状を作成
            //    t.Start();
            //    Revit.DB.DirectShape ds = Revit.DB.DirectShape.CreateElement(rvtDBDoc, new Revit.DB.ElementId(Revit.DB.BuiltInCategory.OST_GenericModel),
            //                                                "Application id",
            //                                                "Geometry object id");
            //    ds.SetShape(new Revit.DB.GeometryObject[] { baseSolid });
            //    t.Commit();
            //}
            ///////
            // マテリアル
            cmbBoxVal = cmpUI.GetCurrentMaterialCmbBoxValue();
            id = 0;
            if (int.TryParse(cmbBoxVal, out id) == false)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_GETMATERIAL"),
                                                     cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));

                transGrp.RollBack();
                return retCmd;
            }


            if (id > 0)
            {
                Revit.DB.ElementId materialId = new Revit.DB.ElementId(id);
                cmpElements.MaterialId = materialId;
            }
            else
            {
                Revit.DB.ElementId materialId = Revit.DB.ElementId.InvalidElementId;
                cmpElements.MaterialId = materialId;
            }


            if (retPFace != null)
            {
                plnFace = retPFace;
            }
            if (plnFace != null)
            {
                // 基準面形状判定
                int mode = cmpGeometry.FaceGeometryMode(plnFace, curvesA);

                // 高さ用の面
                Revit.DB.PlanarFace heightFace = null;
                // 高さ用の辺
                Collections.Generic.IList<Revit.DB.Edge> heightEdges = new Collections.Generic.List<Revit.DB.Edge>();
                // 辺を含む面
                Collections.Generic.IList<Collections.Generic.IList<Revit.DB.PlanarFace>> edgesFaces = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.PlanarFace>>();

                #region 面またはエッジの指定

                Revit.UI.TaskDialog taskDlg = new Revit.UI.TaskDialog(cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
                taskDlg.MainInstruction = cmpAttribute.ResourceText("IDS_TXT_FACEOREDGE_INSTRUCTION");
                if (mode == 3 || mode == 4)
                    taskDlg.MainContent = cmpAttribute.ResourceText("IDS_TXT_FACEOREDGE_CONTENT_RECT");
                else
                    taskDlg.MainContent = cmpAttribute.ResourceText("IDS_TXT_FACEOREDGE_CONTENT");
                taskDlg.AddCommandLink(Autodesk.Revit.UI.TaskDialogCommandLinkId.CommandLink1, cmpAttribute.ResourceText("IDS_TXT_PICK_FACE"));
                taskDlg.AddCommandLink(Autodesk.Revit.UI.TaskDialogCommandLinkId.CommandLink2, cmpAttribute.ResourceText("IDS_TXT_PICK_EDGE"));

                // 正方形、長方形の場合
                if (mode == 3 || mode == 4)
                {
                    taskDlg.AddCommandLink(Autodesk.Revit.UI.TaskDialogCommandLinkId.CommandLink3, cmpAttribute.ResourceText("IDS_TXT_PICK_EDGES"));
                }

                Revit.UI.TaskDialogResult taskDlgRlt = taskDlg.Show();

                // 面指定
                if (taskDlgRlt == Revit.UI.TaskDialogResult.CommandLink1)
                {
                    try
                    {
                        Revit.DB.Reference heightFaceObj = rvtUIDoc.Selection.PickObject(Revit.UI.Selection.ObjectType.Face);
                        Revit.DB.Element heightFaceElem = rvtDBDoc.GetElement(heightFaceObj);

                        heightFace = heightFaceElem.GetGeometryObjectFromReference(heightFaceObj) as Revit.DB.PlanarFace;

                        //暫定
                        Revit.DB.PlanarFace heightFace1 = cmpGeometry.GetSamePlnFace(heightFaceElem, heightFace);
                        //heightFace = heightFace1;
                        //暫定

                        if (heightFace == null)
                        {
                            System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_PICKPLANARFACE"),
                                                                    cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));

                            continue;
                        }
                    }
                    catch (Revit.Exceptions.OperationCanceledException)
                    {
                        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_RETURN_PICKFACE"),
                                                                cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));

                        continue;
                    }
                }
                // エッジを1つ指定
                else if (taskDlgRlt == Autodesk.Revit.UI.TaskDialogResult.CommandLink2)
                    {
                    //・「エッジを1つ選ぶ」場合は、離れていても作れるようにする
                    //ただし、長方形は離れていても、エッジから面を求める
                    //・「エッジを2つ選ぶ」場合は、面内または面上に制限する
                    try
                    {
                        Revit.DB.Reference heightEdgeObj = rvtUIDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Edge);

                        Revit.DB.Element heightEdgeElem = rvtDBDoc.GetElement(heightEdgeObj);

                        Revit.DB.Edge heightEdge = heightEdgeElem.GetGeometryObjectFromReference(heightEdgeObj) as Revit.DB.Edge;
                        Revit.DB.Curve curve1 = heightEdge.AsCurve();

                        //暫定
                        Revit.DB.Edge heightEdge1 = cmpGeometry.GetSamePlnEdge(heightEdgeElem, heightEdge);
                        //heightEdge = heightEdge1;
                        //Revit.DB.Curve curve2 = heightEdge1.AsCurve();
                        //暫定

                        if (heightEdge != null)
                        {
                            heightEdges.Add(heightEdge);

                            // 辺を含む面
                            Collections.Generic.IList<Revit.DB.PlanarFace> edgeFace = cmpGeometry.GetPlnFace(heightEdgeElem, heightEdge);


                            if (edgeFace == null || edgeFace.Count == 0)
                            {
                                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_RELEVANTEDGEFACE"),
                                                                        cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));

                                continue;
                            }

                            edgesFaces.Add(edgeFace);
                        }
                    }
                    catch (Revit.Exceptions.OperationCanceledException)
                    {
                        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_RETURN_PICKFACE"),
                                                                cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));

                        continue;
                    }
                }
                // エッジを2つ指定
                else if (taskDlgRlt == Revit.UI.TaskDialogResult.CommandLink3)
                {
                    try
                    {
                        if (mode == 3 || mode == 4)
                        {
                            //Collections.Generic.IList<Revit.DB.Reference> heightEdgeObjs = rvtUIDoc.Selection.PickObjects(Revit.UI.Selection.ObjectType.Edge);

                            Collections.Generic.IList<Revit.DB.Reference> heightEdgeObjs = new Collections.Generic.List<Revit.DB.Reference>();
                            // 辺を含む面
                            try
                            {
                                Revit.DB.Reference heightEdgeObj1 = rvtUIDoc.Selection.PickObject(Revit.UI.Selection.ObjectType.Edge);
                                heightEdgeObjs.Add(heightEdgeObj1);

                                Revit.DB.Reference heightEdgeObj2 = rvtUIDoc.Selection.PickObject(Revit.UI.Selection.ObjectType.Edge);
                                heightEdgeObjs.Add(heightEdgeObj2);
                            }
                            catch (Revit.Exceptions.OperationCanceledException)
                            {
                                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_RETURN_PICKFACE"),
                                                                        cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));

                                continue;
                            }



                            #region 選択数エラー
                            if (heightEdgeObjs.Count < 1)
                            {
                                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_PICKEDGE") + "\r\n" + cmpAttribute.ResourceText("IDS_TXT_RETURN_PICKFACE"),
                                                                        cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));

                                continue;
                            }
                            else if (heightEdgeObjs.Count > 2)
                            {
                                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_PICKEDEDGEOVER") + "\r\n" + cmpAttribute.ResourceText("IDS_TXT_RETURN_PICKFACE"),
                                                                        cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));

                                continue;
                            }
                            #endregion

                            foreach (Revit.DB.Reference heightEdgeObj in heightEdgeObjs)
                            {
                                Revit.DB.Element heightEdgeElem = rvtDBDoc.GetElement(heightEdgeObj);

                                Revit.DB.Edge heightEdge = heightEdgeElem.GetGeometryObjectFromReference(heightEdgeObj) as Revit.DB.Edge;

                                if (heightEdge != null)
                                {
                                    //暫定
                                    Revit.DB.Edge heightEdge1 = cmpGeometry.GetSamePlnEdge(heightEdgeElem, heightEdge);
                                    //heightEdge = heightEdge1;
                                    //暫定

                                    heightEdges.Add(heightEdge);

                                    // 辺を含む面
                                    Collections.Generic.IList<Revit.DB.PlanarFace> edgeFace = cmpGeometry.GetPlnFace(heightEdgeElem, heightEdge);
                                    edgesFaces.Add(edgeFace);
                                }
                            }
                        }
                    }
                    catch (Revit.Exceptions.OperationCanceledException)
                    {
                        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_RETURN_PICKFACE"),
                                                                cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));

                        continue;
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_RETURN_PICKFACE"),
                                                            cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));

                    continue;
                }

                #endregion

                // 三角形
                if (mode == 1)
                {
                    trans.Start("作成");

                    cmpService.CreateFukashi_Triangle(plnFace,
                                                        plnFaceBase,
                                                        curvesA,
                                                        heightFace,
                                                        heightEdges,
                                                        edgesFaces);

                    trans.Commit();
                }
                // 台形
                else if (mode == 2)
                {
                    trans.Start("作成");

                    cmpService.CreateFukashi_Trapezoid(plnFace,
                                                        plnFaceBase,
                                                        curvesA,
                                                        heightFace,
                                                        heightEdges,
                                                        edgesFaces);

                    trans.Commit();
                }
                // 正方形、長方形
                else if (mode == 3 || mode == 4)
                {
                    trans.Start("作成");

                    cmpService.CreateFukashi_Rectangle(plnFace,
                                                    plnFaceBase,
                                                    curvesA,
                                                    heightFace,
                                                    heightEdges,
                                                    edgesFaces,
                                                    trans);

                    if (trans.GetStatus() != Autodesk.Revit.DB.TransactionStatus.Committed)
                    {
                        trans.Commit();
                    }
                }
                // ひし形、平行四辺形
                else if (mode == 5 || mode == 6)
                {
                    trans.Start("作成");

                    cmpService.CreateFukashi_Trapezoid(plnFace,
                                                        plnFaceBase,
                                                        curvesA,
                                                        heightFace,
                                                        heightEdges,
                                                        edgesFaces);


                    trans.Commit();
                }
                // L字形
                else if (mode == 7)
                {
                    trans.Start("作成");

                    cmpService.CreateFukashi_LType(plnFace,
                                                    plnFaceBase,
                                                    curvesA,
                                                    heightFace,
                                                    heightEdges,
                                                    edgesFaces);


                    trans.Commit();
                }
                // T字形
                else if (mode == 8 || mode == 9)
                {
                    trans.Start("作成");

                    cmpService.CreateFukashi_TType(plnFace,
                                                    plnFaceBase,
                                                    curvesA,
                                                    heightFace,
                                                    heightEdges,
                                                    edgesFaces);


                    trans.Commit();
                }
                // その他
                else if (mode == 10)
                {
                    try
                    {
                        trans.Start("作成");

                        double thickness = 0;

                        //エッジから高さを求める
                        if (heightEdges.Count == 1)
                        {
                            Revit.DB.XYZ pb0 = plnFaceBase.Origin;
                            Revit.DB.XYZ pb1 = pb0 + plnFaceBase.XVector;
                            Revit.DB.XYZ pb2 = pb0 + plnFaceBase.YVector;

                            Revit.DB.XYZ pa1 = heightEdges[0].AsCurve().GetEndPoint(0) - pb0;
                            double h1 = cmpGeometry.Naiseki(plnFaceBase.FaceNormal, pa1);
                            if(h1 < 0)
                            {
                                //埋まる
                                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_RELEVANTFACE"),
                                cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
                                trans.RollBack();
                                continue;
                            }

                            Revit.DB.XYZ pa2 = heightEdges[0].AsCurve().GetEndPoint(1) - pb0;
                            double h2 = cmpGeometry.Naiseki(plnFaceBase.FaceNormal, pa2);
                            if(System.Math.Abs(h1- h2) > cmpGeometry.Approx0Len)
                            {
                                //傾いている
                                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_RELEVANTFACE"),
                                cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
                                trans.RollBack();
                                continue;
                            }

                            thickness = h1;
                        }
                        else
                        {
                            thickness = cmpGeometry.GetPlaneDistance(plnFace, heightFace, edgesFaces);
                        }
                        if (thickness < cmpGeometry.Approx0Len)
                        {
                            System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_RELEVANTFACE"),
                            cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
                            trans.RollBack();
                            continue;
                        }

                        Revit.DB.DirectShape ds = cmpService.CreateFukashi_DirectShape(category,
                                                                                        plnFace,
                                                                                        curvesA,
                                                                                        thickness,
                                                                                        appId,
                                                                                        appDataId);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.RollBack();

                        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_CREATE"),
                                                                cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
                    }
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_PICKPLANARFACE") + "\r\n" + cmpAttribute.ResourceText("IDS_TXT_RETURN_PICKFACE"),
                                                        cmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
            }
        }

      // リボン有効化
      trans.Start("リボン有効化");
      cmpUI.SetRibbonEnable(true);
      trans.Commit();

      // 選択したマテリアルの保存
      cmpParameters.GetStrVal(cmpElements.MaterialId.ToString());
      trans.Start("Write");
      cmpService.Set();
      trans.Commit();

      trans.Start("Set Default");
      cmpParameters.SetSharedParamDefault();
      trans.Commit();

      transGrp.Assimilate();

      retCmd = Autodesk.Revit.UI.Result.Succeeded;
      return retCmd;
    }
        private
bool SolidIntersect4(Revit.DB.Document _RvtDBDoc, Revit.UI.UIDocument _RvtUIDoc, RvtExtApp.Face.Components.Elements _CmpElements, RvtExtApp.Face.Components.Geometry _CmpGeometory)
        {
            // 戻り値
            bool ret = false;

            Revit.DB.Transaction tx = new Revit.DB.Transaction(_RvtDBDoc);

            // 対象要素
            Revit.DB.Reference subjRef = _RvtUIDoc.Selection.PickObject(Revit.UI.Selection.ObjectType.Face, "Select face");
            Revit.DB.Element subjElem = _RvtDBDoc.GetElement(subjRef);
            Revit.DB.PlanarFace subjPFace = subjElem.GetGeometryObjectFromReference(subjRef) as Revit.DB.PlanarFace;
            Revit.DB.XYZ subjNormal = subjPFace.FaceNormal;
            Revit.DB.XYZ subjPos = subjRef.GlobalPoint;
            Collections.Generic.IList<Revit.DB.Solid> subjSolidAry = new Collections.Generic.List<Revit.DB.Solid>();
            Revit.DB.Solid subjSolid = null;

            // 対象ソリッド
            Revit.DB.Options opt = _CmpElements.RvtDBDoc.Application.Create.NewGeometryOptions();
            opt.ComputeReferences = true;
            Revit.DB.GeometryElement geomElem = subjElem.get_Geometry(opt);
            Collections.Generic.IEnumerator<Revit.DB.GeometryObject> geomObjEnum = geomElem.GetEnumerator();
            while (geomObjEnum.MoveNext())
            {
                Revit.DB.GeometryObject geomObj = geomObjEnum.Current;
                Revit.DB.Solid geomSolid = geomObj as Revit.DB.Solid;
                if ((geomSolid != null) && (geomSolid.Volume > 0.0))
                {
                    subjSolid = geomSolid;
                    break;
                }
            }
            if (subjSolid == null)
            {
                return ret;
            }

            Revit.DB.PlanarFace subjSolidPFace = null;
            foreach (Revit.DB.Face face in subjSolid.Faces)
            {
                Revit.DB.PlanarFace pFace = face as Revit.DB.PlanarFace;
                if (pFace != null)
                {
                    if (pFace.Equals(subjPFace) == true)
                    {
                        subjSolidPFace = pFace;
                        break;
                    }
                }
            }
            if (subjSolidPFace == null)
            {
                return ret;
            }

            // 相手要素
            Revit.DB.Reference oopoRef = _RvtUIDoc.Selection.PickObject(Revit.UI.Selection.ObjectType.Element, "Select Element");
            Revit.DB.Element oopoElem = _RvtDBDoc.GetElement(oopoRef);
            Collections.Generic.IList<Revit.DB.Solid> oopoSolidAry = new Collections.Generic.List<Revit.DB.Solid>();
            _CmpGeometory.GetSolidElem(oopoElem, ref oopoSolidAry);
            Revit.DB.Solid oopoSolid = null;
            foreach (Revit.DB.Solid geomSolid in oopoSolidAry)
            {
                if ((geomSolid != null) && (geomSolid.Volume > 0.0))
                {
                    oopoSolid = geomSolid;
                    break;
                }
            }
            if (oopoSolid == null)
            {
                return ret;
            }

            // ソリッド結合
            Revit.DB.Solid unionSolid = Revit.DB.BooleanOperationsUtils.ExecuteBooleanOperation(subjSolid, oopoSolid, Revit.DB.BooleanOperationsType.Union);
            if ((unionSolid == null) || (unionSolid.Volume == 0.0))
            {
                return ret;
            }
            // 相手面
            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> oopoCrvAryAry = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.Curve>>();
            Collections.Generic.IList<Revit.DB.Curve> oopoCrvAry = new Collections.Generic.List<Revit.DB.Curve>();
            Collections.Generic.IList<Revit.DB.PlanarFace> oopoPFaceAry = new Collections.Generic.List<Revit.DB.PlanarFace>();
            Collections.Generic.IList<Revit.DB.CurveLoop> geomCrvLoopAry = null;
            foreach (Revit.DB.Face geomFace in unionSolid.Faces)
            {
                Revit.DB.PlanarFace geomPFace = geomFace as Revit.DB.PlanarFace;
                if (geomPFace != null)
                {
                    geomCrvLoopAry = geomPFace.GetEdgesAsCurveLoops();
                    foreach (Revit.DB.CurveLoop geomCurveLoop in geomCrvLoopAry)
                    {
                        oopoCrvAry = new Collections.Generic.List<Revit.DB.Curve>();
                        foreach (Revit.DB.Curve geomCurve in geomCurveLoop)
                        {
                            oopoCrvAry.Add(geomCurve);
                        }
                        oopoCrvAryAry.Add(oopoCrvAry);
                        oopoPFaceAry.Add(geomPFace);
                    }
                }
            }

            Revit.DB.IntersectionResult interRet;

            // 相手面検索
            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> srcCrvAryAry = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.Curve>>();
            Collections.Generic.IList<Revit.DB.Curve> srcCrvAry = new Collections.Generic.List<Revit.DB.Curve>();
            Collections.Generic.IList<Revit.DB.PlanarFace> srcPfaceAry = new Collections.Generic.List<Revit.DB.PlanarFace>();
            if (oopoCrvAryAry.Count == 0)
            {
                return ret;
            }
            for (int i = 0; i < oopoCrvAryAry.Count; ++i)
            {
                srcCrvAry = oopoCrvAryAry[i];
                Revit.DB.PlanarFace pFace = oopoPFaceAry[i];

                Revit.DB.XYZ normPFace = pFace.FaceNormal;
                Revit.DB.XYZ orgPFace = pFace.Origin;
                if (_CmpGeometory.Distance(subjNormal, normPFace) < _CmpGeometory.Approx0Len)
                {
                    interRet = subjPFace.Project(orgPFace);
                    if (interRet != null)
                    {
                        if (interRet.Distance < _CmpGeometory.Approx0Len)
                        {
                            srcCrvAryAry.Add(srcCrvAry);
                            srcPfaceAry.Add(pFace);
                        }
                    }
                    else
                    {
                        if (_CmpGeometory.Distance(subjPFace.Origin, orgPFace) < _CmpGeometory.Approx0Len)
                        {
                            srcCrvAryAry.Add(srcCrvAry);
                            srcPfaceAry.Add(pFace);
                        }
                    }
                }
            }
            if (srcCrvAryAry.Count == 0)
            {
                return ret;
            }

            // 相手カーブ決定
            Collections.Generic.IList<Revit.DB.Curve> retCurveAry = null;
            Revit.DB.PlanarFace retPface = null;
            int idxRet = GetNearCurve(srcCrvAryAry, subjPos, _CmpGeometory);
            retCurveAry = srcCrvAryAry[idxRet];
            retPface = srcPfaceAry[idxRet];

            // 相手カーブ左下
            int idxLocRetCrvs = GetIndexLBCurves(retCurveAry, _CmpGeometory);
            int idxLocRetCrvsB = idxLocRetCrvs - 1;
            if (idxLocRetCrvsB < 0)
            {
                idxLocRetCrvsB = retCurveAry.Count - 1;
            }
            double valD = retCurveAry[idxLocRetCrvsB].Length;
            double valW = retCurveAry[idxLocRetCrvs].Length;



            // 対象カーブ
            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> subjSolidPFaceCrvAryAry = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.Curve>>();
            Collections.Generic.IList<Revit.DB.Curve> subjSolidPFaceCrvAry = new Collections.Generic.List<Revit.DB.Curve>();
            geomCrvLoopAry = subjSolidPFace.GetEdgesAsCurveLoops();
            foreach (Revit.DB.CurveLoop geomCurveLoop in geomCrvLoopAry)
            {
                subjSolidPFaceCrvAry = new Collections.Generic.List<Revit.DB.Curve>();
                foreach (Revit.DB.Curve geomCurve in geomCurveLoop)
                {
                    subjSolidPFaceCrvAry.Add(geomCurve);
                }
                subjSolidPFaceCrvAryAry.Add(subjSolidPFaceCrvAry);
            }
            int idxSubjSolidPFace = GetNearCurve(subjSolidPFaceCrvAryAry, subjPos, _CmpGeometory);
            subjSolidPFaceCrvAry = subjSolidPFaceCrvAryAry[idxSubjSolidPFace];
            int idxSubjSolidPFaceCrvs = GetIndexLBCurves(subjSolidPFaceCrvAry, _CmpGeometory);

            // ファミリシンボル
            Revit.DB.FamilySymbol famSym = GetFamSym(Revit.DB.BuiltInCategory.OST_GenericModel,
                                                     "フカシファミリ_長方形_おかむら", _RvtDBDoc);

            if (famSym == null)
            {
                return ret;
            }
            if (famSym.IsActive == false)
            {
                tx.Start("sym");
                famSym.Activate();
                tx.Commit();
            }


            Revit.DB.BoundingBoxUV faceBboxUV = retPface.GetBoundingBox();
            Revit.DB.UV faceCenter = (faceBboxUV.Max + faceBboxUV.Min) / 2.0;

            Revit.DB.XYZ faceNormal = retPface.ComputeNormal(faceCenter);
            Revit.DB.XYZ faceRefDir = faceNormal.CrossProduct(faceNormal);

            Revit.DB.XYZ faceCorss = _CmpGeometory.CrossProduct(new Revit.DB.XYZ(0, 0, 0), retPface.XVector, retPface.YVector);
            Revit.DB.XYZ faceDir = retPface.XVector;
            if (_CmpGeometory.Distance(faceNormal, faceCorss) > _CmpGeometory.Approx0Len)
            {
                faceDir = retPface.YVector;
            }

            Revit.DB.XYZ locPosRetPface = retCurveAry[idxLocRetCrvs].GetEndPoint(0);
            Revit.DB.XYZ locPosSubjSolidPFace = subjSolidPFaceCrvAry[idxSubjSolidPFaceCrvs].GetEndPoint(0);
            Revit.DB.XYZ loacation = locPosRetPface;
            interRet = subjSolidPFace.Project(locPosRetPface);
            Revit.DB.XYZ distOrg = new Revit.DB.XYZ();
            if (interRet != null)
            {
                loacation = locPosRetPface;
            }
            else
            {
                if (_CmpGeometory.Distance(locPosRetPface, locPosSubjSolidPFace) < _CmpGeometory.Approx0Len)
                {
                    loacation = locPosRetPface;
                }
                else
                {
                    loacation = locPosSubjSolidPFace;
                    distOrg = new Revit.DB.XYZ(locPosRetPface.X - locPosSubjSolidPFace.X,
                                               locPosRetPface.Y - locPosSubjSolidPFace.Y,
                                               locPosRetPface.Z - locPosSubjSolidPFace.Z);
                }
            }
            Revit.DB.XYZ vecX = subjSolidPFace.XVector;
            Revit.DB.XYZ vecY = subjSolidPFace.YVector;

            double valX = ((distOrg.X * vecX.X) + (distOrg.Y * vecX.Y));
            double valY = ((distOrg.X * vecY.X) + (distOrg.Y * vecY.Y));

            tx.Start("fam");
            Revit.DB.FamilyInstance famInst = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(subjSolidPFace.Reference, loacation, faceDir, famSym);

            Revit.DB.Parameter param;
            param = famInst.LookupParameter("H");
            if (param != null)
            {
                param.Set(1.0);
            }
            param = famInst.LookupParameter("D");
            if (param != null)
            {
                param.Set(valD);
            }

            param = famInst.LookupParameter("W");
            if (param != null)
            {
                param.Set(valW);
            }

            param = famInst.LookupParameter("座標X");
            if (param != null)
            {
                param.Set(valX);
            }

            param = famInst.LookupParameter("座標Y");
            if (param != null)
            {
                param.Set(valY);
            }


            tx.Commit();




            return ret;

        }
        private
Revit.DB.FamilySymbol GetFamSym(Revit.DB.BuiltInCategory bltCat, string name, Revit.DB.Document _RvtDBDoc)
        {
            // 戻り値
            Revit.DB.FamilySymbol ret = null;

            Revit.DB.FilteredElementCollector fillElemCol = new Revit.DB.FilteredElementCollector(_RvtDBDoc);
            Revit.DB.FilteredElementIterator fillElemIter = fillElemCol.OfClass(typeof(Revit.DB.FamilySymbol))
                                                                         .OfCategory(bltCat)
                                                                         .GetElementIterator();



            while (fillElemIter.MoveNext())
            {
                Revit.DB.FamilySymbol famSym = fillElemIter.Current as Revit.DB.FamilySymbol;
                if (famSym != null)
                {
                    if (famSym.Name == name)
                    {
                        ret = famSym;
                        break;
                    }
                }

            }

            return ret;
        }

        /// ================================================================================
        /// <summary>カーブ左下インデックス</summary>
        /// 
        /// <returns>結果</returns>
        /// 
        /// <history>2013/06/11 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        int GetIndexLBCurves(Collections.Generic.IList<Revit.DB.Curve> curveAry, RvtExtApp.Face.Components.Geometry _CmpGeometory)
        {
            // 戻り値
            int ret = -1;

            for (int i = 0; i < curveAry.Count; ++i)
            {
                if (ret == -1)
                {
                    ret = i;
                    continue;
                }

                Revit.DB.XYZ idxLoxPos = curveAry[ret].GetEndPoint(0);
                Revit.DB.XYZ pos = curveAry[i].GetEndPoint(0);
                if (_CmpGeometory.Distance(idxLoxPos, pos) < _CmpGeometory.Approx0Len)
                {
                    continue;
                }

                double vX = idxLoxPos.X - pos.X;
                if (System.Math.Abs(vX) < _CmpGeometory.Approx0Len)
                {
                    vX = 0;
                }
                double vY = idxLoxPos.Y - pos.Y;
                if (System.Math.Abs(vY) < _CmpGeometory.Approx0Len)
                {
                    vY = 0;
                }
                double vZ = idxLoxPos.Z - pos.Z;
                if (System.Math.Abs(vZ) < _CmpGeometory.Approx0Len)
                {
                    vZ = 0;
                }

                if ((vX >= 0) && (vY >= 0) && (vZ >= 0))
                {
                    ret = i;
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>点に近いカーブ</summary>
        /// 
        /// <returns>結果</returns>
        /// 
        /// <history>2013/06/11 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        int GetNearCurve(Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> srcCrvAryAry,
                         Revit.DB.XYZ orgPos, RvtExtApp.Face.Components.Geometry _CmpGeometory)
        {
            // 戻り値
            int ret = 0;

            Collections.Generic.IList<Revit.DB.Curve> srcCrvAry = new Collections.Generic.List<Revit.DB.Curve>();

            if (srcCrvAryAry.Count > 1)
            {
                Collections.Generic.IList<double> srcDistAry = new Collections.Generic.List<double>();

                double min = 0.0;
                double dist = 0.0;
                for (int i = 0; i < srcCrvAryAry.Count; ++i)
                {
                    srcCrvAry = srcCrvAryAry[i];
                    min = 0;
                    for (int j = 0; j < srcCrvAry.Count; ++j)
                    {
                        dist = _CmpGeometory.Distance(orgPos, srcCrvAry[j].GetEndPoint(0));
                        if (j == 0)
                        {
                            min = dist;
                        }
                        else
                        {
                            if (min > dist)
                            {
                                min = dist;
                            }
                        }
                    }
                    srcDistAry.Add(min);
                }

                int idx = 0;
                min = srcDistAry[idx];
                for (int i = 1; i < srcDistAry.Count; ++i)
                {
                    if (min > srcDistAry[i])
                    {
                        idx = i;
                        min = srcDistAry[idx];
                    }
                }
                ret = idx;
            }

            return ret;
        }


        /// ================================================================================
        /// <summary>WarningMessageプリプロセッサセット</summary>
        /// 
        /// <param name="trans">トランザクション</param>
        /// 
        /// <history>2017/02/06 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        private void SetWarningMessagePrep(Revit.DB.Transaction trans)
    {
        Revit.DB.FailureHandlingOptions failOpt = trans.GetFailureHandlingOptions();
        failOpt.SetFailuresPreprocessor(new WarningMessage());
        trans.SetFailureHandlingOptions(failOpt);
    }

    #endregion
    }


    /// ================================================================================
    /// <summary>WarningMessageプリプロセッサ</summary>
    /// 
    /// <history>2017/02/06 Created CST,Co.Ltd. Hideki Sudo</history>
    /// ================================================================================
    public class WarningMessage : Revit.DB.IFailuresPreprocessor
    {
        public Revit.DB.FailureProcessingResult PreprocessFailures(Revit.DB.FailuresAccessor failuresAccessor)
        {
            Collections.Generic.IList<Revit.DB.FailureMessageAccessor> failList = new Collections.Generic.List<Revit.DB.FailureMessageAccessor>();
            failList = failuresAccessor.GetFailureMessages();

            foreach (Revit.DB.FailureMessageAccessor failure in failList)
            {
                String txt = failure.GetDescriptionText();
                if(txt.Contains("インスタンス基準点"))
                {
                    failuresAccessor.DeleteWarning(failure);
                }
            }

            return Revit.DB.FailureProcessingResult.Continue;
        }
    }
}
