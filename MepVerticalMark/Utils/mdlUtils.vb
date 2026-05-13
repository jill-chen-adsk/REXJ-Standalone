Imports Autodesk
Imports Autodesk.Revit
Imports ADSK.MepVerticalMark.My.Resources
Imports Autodesk.Revit.DB
Imports Autodesk.Revit.DB.Plumbing

Module MdlUtils


    Public Function UpperLevel(ByVal dbDoc As DB.Document, ByVal ThisLevel As DB.Level) As DB.Level

        Dim lvList As List(Of DB.Level) = LevelSort(dbDoc)
        Dim uLevel As DB.Level = Nothing
        For i As Integer = 0 To lvList.Count - 2
            Dim lvTemp As DB.Level = lvList.Item(i)
            If lvTemp.Id.Equals(ThisLevel.Id) = True Then
                uLevel = lvList.Item(i + 1)
                Exit For
            End If
        Next

        Return uLevel


    End Function


    ''' <summary>
    ''' レベルの順番
    ''' </summary>
    ''' <param name="dbDoc"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LevelSort(ByVal dbDoc As DB.Document) As List(Of DB.Level)

        Dim lvCollector As New DB.FilteredElementCollector(dbDoc)
        lvCollector.OfClass(GetType(DB.Level))
        Dim lvList As New List(Of DB.Level)
        For Each lvElm As DB.Element In lvCollector.ToElements
            Dim lv As DB.Level = TryCast(lvElm, DB.Level)
            lvList.Add(lv)
        Next
        Dim srtLevel As New CmpLevel()
        lvList.Sort(srtLevel)

        Return lvList

    End Function



    Public Function FinsFamilySymbolByFamilyAndType(ByVal dbDoc As DB.Document,
                                                    ByVal FamilyName As String,
                                                    ByVal TypeName As String,
                                                    ByRef message As String) As DB.FamilySymbol

        Dim collector As New DB.FilteredElementCollector(dbDoc)
        collector.OfClass(GetType(DB.FamilySymbol))

        'タイプ名が""の場合は先頭のファミリシンボルを返す
        Dim query As IEnumerable(Of DB.FamilySymbol)
        If TypeName = String.Empty Then
            query = From elm As DB.FamilySymbol In collector Where elm.Family.Name.ToUpper.Contains(FamilyName.ToUpper) Select elm

        Else
            query = From elm As DB.FamilySymbol In collector Where elm.Family.Name.ToUpper.Contains(FamilyName.ToUpper) And elm.Name.ToUpper = TypeName.ToUpper Select elm

        End If

        Dim lstFamilySymbol As List(Of DB.FamilySymbol) = query.ToList

        If lstFamilySymbol.Count >= 1 Then
            message = String.Empty
            Return lstFamilySymbol.Item(0)
        Else
            message = String.Format(IDS_ERR_FAMILYNOTLOAD, FamilyName + IDS_TXT_COLON + TypeName)
            Return Nothing
        End If

    End Function


    ''' <summary>
    ''' パイプが竪パイプと仮定して、始点と終点のZ値の範囲に与えられたZが入っているものだけ選ぶ
    ''' </summary>
    ''' <param name="pipeList"></param>
    ''' <param name="zVal"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FilterPipeByZval(pipeList As List(Of Pipe), zVal As Double, isUnlimited As Boolean) As List(Of Pipe)
        Dim lstNewPipes As New List(Of Pipe)
        For Each p1 As Pipe In pipeList
            Dim loc1 As DB.LocationCurve = p1.Location
            Dim cv1 As DB.Curve = loc1.Curve
            Dim z0 As Double = cv1.GetEndPoint(0).Z
            Dim z1 As Double = cv1.GetEndPoint(1).Z

            If isUnlimited Then
                Dim z As Double = Math.Min(z0, z1)
                If z > zVal Then
                    lstNewPipes.Add(p1)
                End If
            Else
                If z0 > z1 Then
                    Dim z00 As Double = z0
                    z0 = z1
                    z1 = z00
                End If
                If z0 <= zVal And zVal <= z1 Then
                    lstNewPipes.Add(p1)
                End If
            End If
        Next
        Return lstNewPipes
    End Function
    

    Public Function FilterDuctByZval(DuctList As List(Of Mechanical.Duct), Zval As Double, isUnlimited As Boolean) As List(Of Mechanical.Duct)
        Dim lstNewDucts As New List(Of Mechanical.Duct)

        For Each p1 As Mechanical.Duct In DuctList
            Dim loc1 As LocationCurve = p1.Location
            Dim cv1 As Curve = loc1.Curve
            Dim z0 As Double = cv1.GetEndPoint(0).Z
            Dim z1 As Double = cv1.GetEndPoint(1).Z
            
            If isUnlimited Then
                Dim z As Double = Math.Min(z0, z1)
                If z > zVal Then
                    lstNewDucts.Add(p1)
                End If
            Else
                If z0 > z1 Then
                    Dim z00 As Double = z0
                    z0 = z1
                    z1 = z00
                End If
                If z0 <= zVal And zVal <= z1 Then
                    lstNewDucts.Add(p1)
                End If
            End If
        Next
        Return lstNewDucts
    End Function
    
    Public Function NearestPipe(ByVal PipeList As List(Of DB.Plumbing.Pipe), ByVal selPt As DB.XYZ) As DB.Plumbing.Pipe

        Dim pipe0 As DB.Plumbing.Pipe = Nothing
        Dim dist0 As Double = -100

        For Each pipe1 As DB.Plumbing.Pipe In PipeList
            Dim loc As DB.LocationCurve = pipe1.Location
            Dim cv1 As DB.Curve = loc.Curve
            Dim cv11 As DB.Curve = cv1.Clone
            cv11.MakeUnbound()
            Dim dist1 As Double = cv11.Distance(selPt)
            If dist0 < 0 Then
                dist0 = dist1
                pipe0 = pipe1
            Else
                If dist0 > dist1 Then
                    dist0 = dist1
                    pipe0 = pipe1
                End If
            End If
        Next
        Return pipe0

    End Function


    Public Function NearestDuct(ByVal DuctList As List(Of DB.Mechanical.Duct), ByVal selPt As DB.XYZ) As DB.Mechanical.Duct
        Dim Duct0 As DB.Mechanical.Duct = Nothing
        Dim dist0 As Double = -100

        For Each Duct1 As DB.Mechanical.Duct In DuctList
            Dim loc As DB.LocationCurve = Duct1.Location
            Dim cv1 As DB.Curve = loc.Curve
            Dim cv11 As DB.Curve = cv1.Clone
            cv11.MakeUnbound()
            Dim dist1 As Double = cv11.Distance(selPt)
            If dist0 < 0 Then
                dist0 = dist1
                Duct0 = Duct1
            Else
                If dist0 > dist1 Then
                    dist0 = dist1
                    Duct0 = Duct1
                End If
            End If
        Next
        Return Duct0
    End Function


    Public Function SelectDuct(ByVal uiDoc As UI.UIDocument) As List(Of DB.Mechanical.Duct)

        Dim dbDoc As DB.Document = uiDoc.Document

        Dim lstDucts As New List(Of DB.Mechanical.Duct)

        Dim prePicks As List(Of DB.ElementId) = uiDoc.Selection.GetElementIds
        If prePicks.Count > 0 Then
            Dim Collector1 As New DB.FilteredElementCollector(dbDoc, prePicks)
            Dim lstElms As List(Of DB.Element) = Collector1.OfClass(GetType(DB.Mechanical.Duct)).ToElements
            For Each elm As DB.Element In lstElms
                Dim pp As DB.Mechanical.Duct = TryCast(elm, DB.Mechanical.Duct)
                If IsNothing(pp) = False Then
                    lstDucts.Add(pp)
                End If
            Next
        End If
        Return lstDucts

    End Function

    Public Function GetLinestyleByName(ByVal dbDoc As DB.Document, ByVal LineStyleName As String) As DB.Category

        Dim ret As DB.Category = Nothing

        '線種は線のサブカテゴリである
        Dim lineCat As DB.Category = dbDoc.Settings.Categories.Item(DB.BuiltInCategory.OST_Lines)
        Dim lineSbCats As DB.CategoryNameMap = lineCat.SubCategories

        'このサブカテゴリの中にあるか？
        Dim lineSbCatIter As DB.CategoryNameMapIterator = lineSbCats.ForwardIterator
        Do While lineSbCatIter.MoveNext
            Dim cat As DB.Category = lineSbCatIter.Current
            If cat.Name.Equals(LineStyleName, StringComparison.CurrentCultureIgnoreCase) = True Then
                ret = cat
                Exit Do
            End If
        Loop
        If IsNothing(ret) And lineCat.CanAddSubcategory = True Then
            '作成する
            Using sb1 As New DB.Transaction(dbDoc, TRANS_CREATELINESTYLE)
                If sb1.Start = DB.TransactionStatus.Started Then

                    Try
                        ret = dbDoc.Settings.Categories.NewSubcategory(lineCat, LineStyleName)
                        sb1.Commit()

                    Catch ex As Exception
                        sb1.RollBack()
                    End Try
                End If
            End Using
        End If

        Return ret

    End Function

    Public Sub ChangeLineSyle(ByVal Curve0 As DB.CurveElement, Lineategory As DB.Category)

        Curve0.LineStyle = Lineategory.GetGraphicsStyle(DB.GraphicsStyleType.Projection)

    End Sub

End Module
