Imports System.Collections.Generic
Imports Autodesk.Revit
Imports Autodesk.Revit.DB
Imports Autodesk.Revit.UI
Imports ADSK.MepVerticalMark.My.Resources

<Autodesk.Revit.Attributes.Transaction(Attributes.TransactionMode.Manual)>
Public Class CmdVerticalDuctMark
    Implements Autodesk.Revit.UI.IExternalCommand

    Dim fsReibai As Object


    Public Function Execute(ByVal commandData As Autodesk.Revit.UI.ExternalCommandData, ByRef message As String, ByVal elements As Autodesk.Revit.DB.ElementSet) As Autodesk.Revit.UI.Result Implements Autodesk.Revit.UI.IExternalCommand.Execute

        Dim uiDoc As UI.UIDocument = commandData.Application.ActiveUIDocument
        Dim dbDoc As DB.Document = uiDoc.Document
        Dim uiRet As UI.Result = UI.Result.Cancelled
        Dim curView As DB.View = dbDoc.ActiveView
        
        Using trgp As New TransactionGroup(dbDoc, CMD_VERTICAL_DUCT)
            If trgp.Start = TransactionStatus.Started Then
                Try

                    If curView.ViewType <> ViewType.FloorPlan Then
                        Throw New Exception(IDS_ERR_NOTVIEWPLAN)
                    End If

                    Dim viewPlan As ViewPlan = TryCast(dbDoc.ActiveView, ViewPlan)
                    Dim viewRange As PlanViewRange = viewPlan.GetViewRange()
                    Dim dTopOffset = viewRange.GetOffset(PlanViewPlane.TopClipPlane)    'ftで入っている
                    Dim dBottomOffset = viewRange.GetOffset(PlanViewPlane.BottomClipPlane)
                    Dim topOffsetMm = UnitUtils.ConvertFromInternalUnits(dTopOffset, DB.UnitTypeId.Millimeters)
                    Dim bottomOffsetMm = UnitUtils.ConvertFromInternalUnits(dBottomOffset, DB.UnitTypeId.Millimeters)
                    Dim bottomClipPlaneLevel As Level = dbDoc.GetElement(viewRange.GetLevelId(PlanViewPlane.BottomClipPlane)) 
                    Dim topClipPlaneLevel As Level = dbDoc.GetElement(viewRange.GetLevelId(PlanViewPlane.TopClipPlane)) 
                    
                    Dim skpPlane As DB.SketchPlane = curView.SketchPlane
                    Dim zVal As Double = skpPlane.GetPlane.Origin.Z
                    Dim thisLevel As DB.Level = curView.GenLevel

                    '上のレベル
                    Dim upLevel As DB.Level = MdlUtils.UpperLevel(dbDoc, thisLevel)
                    If IsNothing(upLevel) = True Then
                        Throw New Exception(IDS_ERR_NOUPPERLEVEL)
                    End If
                    
                    '************************************************
                    '
                    '  必要なタグを取得
                    '
                    '************************************************
                    '尺度
                    Dim curScale As Integer = curView.Scale

                    '必要なファミリシンボル
                    Dim fsYabane0 As DB.FamilySymbol
                    Dim fsYabaneU As DB.FamilySymbol
                    Dim fsYabaneS As DB.FamilySymbol
                    Dim fsSizeComma As DB.FamilySymbol
                    Dim fsSize As DB.FamilySymbol

                    'エラーメッセージ
                    Dim errMsg As String = String.Empty

                    '尺度によって使用するファミリ名は異なる
                    If curScale <= 100 Then
                        '1/100以下
                        '矢羽
                        fsYabane0 = MdlUtils.FinsFamilySymbolByFamilyAndType(dbDoc, TAG_DUCT_YABANE_100, TYPE_NASHI, errMsg)
                        If IsNothing(fsYabane0) = True Then
                            Throw New Exception(errMsg)
                        End If
                        fsYabaneU = MdlUtils.FinsFamilySymbolByFamilyAndType(dbDoc, TAG_DUCT_YABANE_100, TYPE_UE, errMsg)
                        If IsNothing(fsYabaneU) = True Then
                            Throw New Exception(errMsg)
                        End If
                        fsYabaneS = MdlUtils.FinsFamilySymbolByFamilyAndType(dbDoc, TAG_DUCT_YABANE_100, TYPE_SHITA, errMsg)
                        If IsNothing(fsYabaneS) = True Then
                            Throw New Exception(errMsg)
                        End If
                        'ダクトサイズ
                        fsSizeComma = MdlUtils.FinsFamilySymbolByFamilyAndType(dbDoc, TAG_DUCT_SIZE_100, TYPE_COMMA, errMsg)
                        If IsNothing(fsSizeComma) = True Then
                            Throw New Exception(errMsg)
                        End If
                        fsSize = MdlUtils.FinsFamilySymbolByFamilyAndType(dbDoc, TAG_DUCT_SIZE_100, TYPE_STANDARD, errMsg)
                        If IsNothing(fsSize) = True Then
                            Throw New Exception(errMsg)
                        End If

                    Else
                        '1/200以上
                        '矢羽
                        fsYabane0 = MdlUtils.FinsFamilySymbolByFamilyAndType(dbDoc, TAG_DUCT_YABANE_200, TYPE_NASHI, errMsg)
                        If IsNothing(fsYabane0) = True Then
                            Throw New Exception(errMsg)
                        End If
                        fsYabaneU = MdlUtils.FinsFamilySymbolByFamilyAndType(dbDoc, TAG_DUCT_YABANE_200, TYPE_UE, errMsg)
                        If IsNothing(fsYabaneU) = True Then
                            Throw New Exception(errMsg)
                        End If
                        fsYabaneS = MdlUtils.FinsFamilySymbolByFamilyAndType(dbDoc, TAG_DUCT_YABANE_200, TYPE_SHITA, errMsg)
                        If IsNothing(fsYabaneS) = True Then
                            Throw New Exception(errMsg)
                        End If
                        'ダクトサイズ
                        fsSizeComma = MdlUtils.FinsFamilySymbolByFamilyAndType(dbDoc, TAG_DUCT_SIZE_200, TYPE_COMMA, errMsg)
                        If IsNothing(fsSizeComma) = True Then
                            Throw New Exception(errMsg)
                        End If
                        fsSize = MdlUtils.FinsFamilySymbolByFamilyAndType(dbDoc, TAG_DUCT_SIZE_200, TYPE_STANDARD, errMsg)
                        If IsNothing(fsSize) = True Then
                            Throw New Exception(errMsg)
                        End If
                    End If

                    '線種
                    Dim catLeader As Category = GetLinestyleByName(dbDoc, LINESTYLE_LEADER)
                    If IsNothing(catLeader) Then
                        Throw New Exception(String.Format(IDS_ERR_NOLEADERLINESTYLE, LINESTYLE_LEADER))
                    End If


                    '************************************************
                    ' 矢印タイプ
                    '************************************************
                    Dim dlg1 As New DlgMarkType
                    dlg1.Text = CMD_VERTICAL_DUCT

                    If dlg1.ShowDialog <> System.Windows.Forms.DialogResult.OK Then
                        Return Result.Cancelled
                    End If

                    '************************************************
                    ' 竪ダクトを選択
                    '************************************************

                    Dim lstDuct As New List(Of DB.Mechanical.Duct)
                    Try
                        Dim refs As List(Of DB.Reference) = uiDoc.Selection.PickObjects(UI.Selection.ObjectType.Element, New SelFiltVerticalDuct, IDS_STATUS_SELECTDUCT)
                        For Each ref1 As DB.Reference In refs
                            lstDuct.Add(dbDoc.GetElement(ref1))
                        Next
                    Catch ex As Exception
                        If TypeOf (ex) Is Exceptions.OperationCanceledException Then
                            trgp.RollBack()
                            TaskDialog.Show(IDS_TXT_INFO, IDS_INFO_OPERATIONCANCEL)
                            Return Result.Cancelled
                        Else
                            Throw New Exception(ex.Message)
                        End If
                    End Try

                    If lstDuct.Count = 0 Then
                        Throw New Exception(IDS_ERR_SELECTDUCT)
                    End If

                    '始点
                    Dim startDuct As DB.Mechanical.Duct = Nothing
                    If lstDuct.Count > 1 Then
                        '選択したダクトのうちで始点に指定したいダクトを選択
                        Dim lstDuctIds As New List(Of DB.ElementId)
                        For Each d1 As DB.Mechanical.Duct In lstDuct
                            lstDuctIds.Add(d1.Id)
                        Next
                        Dim selFiltN As New SelFiltNominated(lstDuctIds)
                        Try
                            Dim ref As DB.Reference = uiDoc.Selection.PickObject(UI.Selection.ObjectType.Element, selFiltN, IDS_STATUS_SELECTSTARTDUCT)
                            startDuct = dbDoc.GetElement(ref)
                        Catch ex As Exception
                            If TypeOf (ex) Is Exceptions.OperationCanceledException Then
                                trgp.RollBack()
                                TaskDialog.Show(IDS_TXT_INFO, IDS_INFO_OPERATIONCANCEL)
                                Return Result.Cancelled
                            Else
                                Throw New Exception(ex.Message)
                            End If
                        End Try
                    Else
                        startDuct = lstDuct.First
                    End If


                    Dim startLoc As DB.LocationCurve = startDuct.Location
                    Dim startPoint As DB.XYZ = startLoc.Curve.GetEndPoint(0)
                    startPoint = New DB.XYZ(startPoint.X, startPoint.Y, zVal)

                    'タグを挿入する点を指定する
                    Dim selPoint As DB.XYZ = Nothing
                    Try
                        selPoint = uiDoc.Selection.PickPoint(IDS_STATUS_SELECTPLACETAGPOINT)
                    Catch ex As Exception
                        If TypeOf (ex) Is Exceptions.OperationCanceledException Then
                            trgp.RollBack()
                            TaskDialog.Show(IDS_TXT_INFO, IDS_INFO_OPERATIONCANCEL)
                            Return Result.Cancelled
                        Else
                            Throw New Exception(ex.Message)
                        End If
                    End Try

                    Dim elbowPoint As DB.XYZ = selPoint

                    '上か下かの判断をする(Yの値で判断)
                    Dim refZ As Double = 0
                    If startPoint.Y < selPoint.Y Then
                        
                        If IsNothing(topClipPlaneLevel) Then
                                                    
                            '上向き----無制限
                            refZ = bottomClipPlaneLevel.Elevation + dBottomOffset
                            lstDuct = MdlUtils.FilterDuctByZval(lstDuct, refZ, True)
                        Else 
                                                        
                            '上向き----Z=FL+上オフセット
                            refZ = topClipPlaneLevel.Elevation + dTopOffset
                            lstDuct = MdlUtils.FilterDuctByZval(lstDuct, refZ, False)
                        End If

                        If lstDuct.Count = 0 Then
                            Throw New Exception(String.Format(IDS_ERR_NO_SPECIFIED_DUCT, topClipPlaneLevel.Name, topOffsetMm.ToString  ("+#;-#;+0;")))
                        End If
                    Else
                        '下向き----Z=FL+下オフセット
                        refZ = bottomClipPlaneLevel.Elevation + dBottomOffset
                        lstDuct = MdlUtils.FilterDuctByZval(lstDuct, refZ, False)
                        If lstDuct.Count = 0 Then
                            Throw New Exception(String.Format(IDS_ERR_NO_SPECIFIED_DUCT, bottomClipPlaneLevel.Name, bottomOffsetMm.ToString  ("+#;-#;+0;")))
                        End If
                    End If

                    '***********************************************
                    '
                    '   順番の決定
                    '
                    '***********************************************

                    '----------------------------
                    'パイプの存在範囲のXが大きければXが若い順、Yが大きければYの大きい順で並べる。
                    Dim lstXval As New List(Of Double)
                    Dim lstYval As New List(Of Double)
                    For Each selDuct As DB.Mechanical.Duct In lstDuct
                        Dim selLocCv As DB.LocationCurve = selDuct.Location
                        Dim selCv As DB.Curve = selLocCv.Curve
                        Dim mitPt As DB.XYZ = selCv.Evaluate(0.5, True)
                        lstXval.Add(mitPt.X)
                        lstYval.Add(mitPt.Y)
                    Next
                    lstXval.Sort()
                    lstYval.Sort()
                    Dim deltaX As Double = lstXval.Last - lstXval.First
                    Dim deltaY As Double = lstYval.Last - lstYval.First

                    '大きい方で並べ替え
                    If deltaX >= deltaY Then
                        lstDuct.Sort(New CmpDuctByCoordX)
                    Else
                        lstDuct.Sort(New CmpDuctByCoordY)
                    End If
                    '----------------------------


                    'startDuct = lstDuct.Item(0)
                    'startLoc = startDuct.Location
                    'startPoint = startLoc.Curve.GetEndPoint(0)
                    'startPoint = New DB.XYZ(startPoint.X, startPoint.Y, zVal)

                    '線を作成する（60度限定)
                    Dim sp1 As New DB.XYZ(startPoint.X, startPoint.Y, zVal)
                    Dim ep1 As New DB.XYZ(elbowPoint.X, elbowPoint.Y, zVal)

                    'ep1から水平線を作成
                    Dim line1 As DB.Line = DB.Line.CreateUnbound(ep1, New DB.XYZ(1, 0, 0))

                    'sp1から60度の線を作成
                    Dim line2 As DB.Line = DB.Line.CreateUnbound(sp1, New DB.XYZ(1, Math.Sqrt(3.0), 0))

                    '交点を求める
                    Dim mp1 As DB.XYZ = Nothing
                    Using intersectResult As DB.CurveIntersectResult = line1.Intersect(line2, DB.CurveIntersectResultOption.Detailed)
                        If intersectResult.Result <> DB.SetComparisonResult.Overlap Then
                            Throw New Exception(IDS_ERR_INVALIDPOINT)
                        End If
                        Dim overlaps As IList(Of DB.CurveOverlapPoint) = intersectResult.GetOverlaps()
                        If overlaps Is Nothing OrElse overlaps.Count = 0 Then
                            Throw New Exception(IDS_ERR_INVALIDPOINT)
                        End If
                        Using cop As DB.CurveOverlapPoint = overlaps(0)
                            mp1 = cop.Point
                        End Using
                    End Using
                    If IsNothing(mp1) = True Then
                        Throw New Exception(IDS_ERR_INVALIDPOINT)
                    End If
                    ' RevitのCurveの始点終点の間隔の最小値　1/32''
                    If (mp1-sp1).GetLength() < 1.0/32000 Then
                        Throw New Exception(IDS_ERR_INVALIDPOINT)
                    End If
                    
                    '作成する線分のリスト
                    Dim lstGlines As New List(Of DB.Line)

                    'mpとepの距離が10mm以上だと曲がる
                    'If Math.Abs(mp1.X - ep1.X) > DB.UnitUtils.ConvertToInternalUnits(10, DB.DisplayUnitType.DUT_MILLIMETERS) * curView.Scale Then
                    If dlg1.MarkType = 1 Then

                        Dim mp2 As DB.XYZ = 0.5 * (sp1 + mp1)
                        Dim mp3 As DB.XYZ = mp2 + DB.XYZ.BasisX * (ep1.X - mp1.X)
                        Dim lin1 As DB.Line = DB.Line.CreateBound(sp1, mp2)
                        Dim lin2 As DB.Line = DB.Line.CreateBound(mp2, mp3)
                        Dim lin3 As DB.Line = DB.Line.CreateBound(mp3, ep1)
                        mp1 = ep1

                        lstGlines.Add(lin1)
                        lstGlines.Add(lin2)
                        lstGlines.Add(lin3)

                    Else
                        'Dim gLine1 As DB.Line = DB.Line.CreateBound(sp1, mp1)
                        lstGlines.Add(DB.Line.CreateBound(sp1, mp1))
                    End If

                    '作成したタグ
                    Dim lstTags As New List(Of IndependentTag)
                    Dim arrowFamilySymbol As FamilySymbol = Nothing
                    Dim arrowLineParam As Parameter = Nothing
                    
                    Using tr1 As New Transaction(dbDoc, TRANS_CREATEMARK)
                        If tr1.Start = TransactionStatus.Started Then
                            Try
                                '詳細線分の作成
                                Using sb1 As New DB.SubTransaction(dbDoc)
                                    If sb1.Start = DB.TransactionStatus.Started Then
                                        Try
                                            For Each gline1 As DB.Line In lstGlines
                                                if(lstGlines.Last = gLine1) Then Continue For
                                                Dim dLine1 As DB.DetailLine = dbDoc.Create.NewDetailCurve(curView, gline1)
                                                ChangeLineSyle(dLine1, catLeader)
                                            Next
                                            sb1.Commit()
                                        Catch ex As Exception
                                            sb1.RollBack()
                                            Throw New Exception(IDS_ERR_CREATEDETAILLINEERR)
                                        End Try
                                    End If
                                End Using

                                '5mmピッチ矢羽を並べる
                                Dim mm5 As Double = 5
                                mm5 = DB.UnitUtils.ConvertToInternalUnits(mm5, DB.UnitTypeId.Millimeters)
                                mm5 = mm5 * curScale

                                Dim insPnt As DB.XYZ = mp1
                                '移動トランスフォーム
                                Dim mTrans As DB.Transform = DB.Transform.CreateTranslation(New DB.XYZ(mm5, 0, 0))

                                For j As Integer = 0 To lstDuct.Count - 1
                                    '挿入位置
                                    'insPnt = New DB.XYZ(mp1.X + j * mm5, mp1.Y, mp1.Z)
                                    If j <> 0 Then
                                        insPnt = mTrans.OfPoint(insPnt)
                                    End If
                                    'タグの作成
                                    Dim Duct1 As DB.Mechanical.Duct = lstDuct(j)
                                    'Dim ipdTag As DB.IndependentTag = dbDoc.Create.NewTag(curView, Duct1, False, DB.TagMode.TM_ADDBY_CATEGORY, DB.TagOrientation.Horizontal, insPnt)
                                    Dim refDutc1 As New DB.Reference(Duct1)
                                    Dim ipdTag As DB.IndependentTag = DB.IndependentTag.Create(dbDoc, curView.Id, refDutc1, False, DB.TagMode.TM_ADDBY_CATEGORY, DB.TagOrientation.Horizontal, insPnt)

                                    Dim prmMark As DB.Parameter = Duct1.Parameter(DB.BuiltInParameter.RBS_DUCT_PIPE_SYSTEM_ABBREVIATION_PARAM)
                                    ipdTag.ChangeTypeId(fsYabaneS.Id)
                                    arrowFamilySymbol = fsYabaneS
                                    arrowLineParam = arrowFamilySymbol.LookupParameter("矢羽線部")

                                    Dim strSysTyp = String.Empty
                                    Dim prmSysTyp As DB.Parameter = Duct1.Parameter(DB.BuiltInParameter.RBS_DUCT_SYSTEM_TYPE_PARAM)
                                    If IsNothing(prmSysTyp) = False Then
                                        If prmSysTyp.HasValue = True Then
                                            Dim eidSysTyp As DB.ElementId = prmSysTyp.AsElementId
                                            Dim elmSysTyp As DB.Element = dbDoc.GetElement(eidSysTyp)
                                            strSysTyp = elmSysTyp.Name
                                        End If
                                    End If

                                Next

                                '**************************************
                                '
                                '  配管サイズタグ/冷媒符号タグ
                                '
                                '**************************************
                                '9mmピッチ並べる
                                Dim mm9 As Double = 9
                                mm9 = DB.UnitUtils.ConvertToInternalUnits(mm9, DB.UnitTypeId.Millimeters)
                                mm9 = mm9 * curView.Scale
                                '移動トランスフォーム
                                Dim mTran4 As DB.Transform = DB.Transform.CreateTranslation(New DB.XYZ(mm9, 0, 0))
                                insPnt = mTran4.OfPoint(insPnt)

                                Dim DuctCount As Integer = 0
                                'サイズタグが見つかった場合は作成
                                For k As Integer = 0 To lstDuct.Count - 1
                                    'タグの作成
                                    Dim Duct1 As DB.Mechanical.Duct = lstDuct(k)

                                    'Dim ipdTag As DB.IndependentTag = dbDoc.Create.NewTag(curView, Duct1, False, DB.TagMode.TM_ADDBY_CATEGORY, DB.TagOrientation.Horizontal, insPnt)
                                    Dim refToTag As DB.Reference = New DB.Reference(Duct1)
                                    Dim ipdTag As DB.IndependentTag = DB.IndependentTag.Create(dbDoc, curView.Id, refToTag, False, DB.TagMode.TM_ADDBY_CATEGORY, DB.TagOrientation.Horizontal, insPnt)
                                    lstTags.Add(ipdTag)

                                    If DuctCount = 0 Then
                                        arrowFamilySymbol = fsSize
                                        DuctCount += 1
                                    Else
                                        arrowFamilySymbol = fsSizeComma
                                    End If
                                    ipdTag.ChangeTypeId(arrowFamilySymbol.Id)
                                Next

                                insPnt = mTrans.OfPoint(insPnt)

                                tr1.Commit()
                            Catch ex As Exception
                                tr1.RollBack()
                                Throw New Exception(tr1.GetName + IDS_TXT_COLON + ex.Message)
                            End Try
                        End If
                    End Using

#Region "タグ移動"
                    If lstTags.Count > 1 Then
                        Using tr1 As New Transaction(dbDoc, TRANS_MOVETAG)
                            If tr1.Start = TransactionStatus.Started Then
                                Try
                                    'それぞれの移動するべき距離
                                    Dim lstTagMoves As New List(Of Double)
                                    For i As Integer = 1 To lstTags.Count - 1
                                        Dim bdTag0 As BoundingBoxXYZ = lstTags(i - 1).BoundingBox(curView)
                                        Dim bdTag1 As BoundingBoxXYZ = lstTags(i).BoundingBox(curView)
                                        lstTagMoves.Add(bdTag0.Max.X - bdTag1.Min.X)
                                    Next
                                    '移動は累積で
                                    Dim totalMove As Double = 0
                                    For i As Integer = 0 To lstTags.Count - 2
                                        totalMove += lstTagMoves(i)
                                        ElementTransformUtils.MoveElement(dbDoc, lstTags(i + 1).Id, XYZ.BasisX * totalMove)
                                    Next
                                    tr1.Commit()
                                Catch ex As Exception
                                    tr1.RollBack()
                                    Throw New Exception(tr1.GetName + IDS_TXT_COLON + ex.Message)
                                End Try
                            End If
                        End Using
                    End If
#End Region
#Region "下線の延長"
                    If lstTags.Count > 0 Then
                        Using tr1 As New Transaction(dbDoc, TRANS_CREATEUNDERLINE)
                            If tr1.Start = TransactionStatus.Started Then
                                Try
                                    Dim bdLastTag As BoundingBoxXYZ = lstTags.Last.BoundingBox(curView)
                                    Dim ptLine As XYZ = New XYZ(bdLastTag.Max.X, mp1.Y, mp1.Z)
                                    Dim gLineB As DB.Line = DB.Line.CreateBound(mp1, ptLine)
                                    Dim dLineB As DB.DetailLine = dbDoc.Create.NewDetailCurve(curView, gLineB)
                                    ChangeLineSyle(dLineB, catLeader)
                                    tr1.Commit()
                                Catch ex As Exception
                                    tr1.RollBack()
                                    Throw New Exception(tr1.GetName + IDS_TXT_COLON + ex.Message)
                                End Try
                            End If
                        End Using
                    End If
#End Region

#Region "Draw lines aligned to arrow marks"
                    Using tr1 As New Transaction(dbDoc, "Draw lines aligned to arrow marks")
                        If tr1.Start = TransactionStatus.Started Then
                            Try
                                If Not IsNothing(arrowLineParam) Then
                                    Dim arrowLength As Double = arrowLineParam.AsDouble()
                                    Dim arrowLengthU = DB.UnitUtils.ConvertToInternalUnits(arrowLength, DB.UnitTypeId.Meters)*curView.Scale
                                    Dim line As DB.Line = lstGlines.Last()

                                    If startPoint.Y <= selPoint.Y Then
                                        arrowLengthU = 0
                                    End If

                                    Dim newLine As DB.Line = DB.Line.CreateBound(line.Origin, line.Origin + line.Direction*(line.Length - arrowLengthU))
                                    Dim dLine1 As DB.DetailLine = dbDoc.Create.NewDetailCurve(curView, newLine)
                                    ChangeLineSyle(dLine1, catLeader)
                                End If
                                tr1.Commit()
                            Catch ex As Exception
                                tr1.RollBack()
                                Throw New Exception(tr1.GetName + IDS_TXT_COLON + ex.Message)
                            End Try
                        End If
                    End Using
#End Region
                    
                    trgp.Assimilate()
                    uiRet = Result.Succeeded
                Catch ex As Exception
                    trgp.RollBack()
                    TaskDialog.Show(IDS_TXT_ERR, ex.Message)
                    uiRet = Result.Failed
                End Try
            End If
        End Using

        Return uiRet
    End Function
End Class
