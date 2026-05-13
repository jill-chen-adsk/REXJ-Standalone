using System ;
using ADSK.JExtRAC.ValueCopy.Entities ;
using ADSK.JExtRAC.ValueCopy.Warning ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.UI.Selection ;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.Linq ;

namespace ADSK.JExtRAC.ValueCopy.Components
{
  /// ================================================================================
  /// <summary>Elements</summary>
  /// ================================================================================
  public class Elements
  {
    public UIDocument RvtUIDoc { get; }

    public Document RvtDBDoc => RvtUIDoc.Document ;

    #region Constructor

    /// ================================================================================
    /// <summary>Constructor</summary>
    ///
    /// <param name="rvtUIDoc">Revit UIドキュメント</param>
    ///
    /// <history>2021/11/29 Created Applied Technology</history>
    /// ================================================================================
    public Elements( UIDocument rvtUIDoc )
    {
      RvtUIDoc = rvtUIDoc ;
    }

    #endregion Constructor

    #region Member Functions

    /// ================================================================================
    /// <summary>Select element</summary>
    ///
    /// <param name="uiDoc">UIDocument</param>
    /// <param name="message">  string masage</param>
    /// <returns></returns>
    ///
    /// <history>2022/01/10 Created Applied Technology</history>
    /// ================================================================================
    public Element PickElement( UIDocument uiDoc, string message )
    {
      try {
        //PickObject
        Reference reference = uiDoc.Selection.PickObject( ObjectType.Element, new SelectionElementFilter(), message ) ;
        if ( reference == null )
          return null ;

        return uiDoc.Document.GetElement( reference ) ;
      }
      catch ( System.Exception ex ) {
        var mess = ex.Message ;
      }

      return null ;
    }

    /// ================================================================================
    /// <summary>Get Element Selected</summary>
    ///
    /// <param name="uiDoc">UIDocument</param>
    /// <returns></returns>
    ///
    /// <history>2022/01/10 Created Applied Technology</history>
    /// ================================================================================
    public List<Element> GetElementSelected( UIDocument uiDoc )
    {
      List<Element> retVal = new List<Element>() ;

      try {
        var selectedElement = uiDoc.Selection.GetElementIds() ;
        if ( selectedElement == null )
          return retVal ;

        foreach ( ElementId eleId in selectedElement ) {
          var ele = uiDoc.Document.GetElement( eleId ) ;
          if ( ele == null )
            continue ;

          var category = ele.Category ;
          if ( category == null )
            continue ;

          if ( SelectionElementFilter.ListCategoryDefault.Any( x => ( ((int)x).ToString() ) == category.Id.ToString() ) == false )
            continue ;

          retVal.Add( ele ) ;
        }
      }
      catch ( System.Exception ex ) {
        var mess = ex.Message ;
      }

      return retVal ;
    }

    /// ================================================================================
    /// <summary>Set parameter</summary>
    ///
    /// <param name="doc">Document</param>
    /// <param name="copySourceElement">ObjectElement</param>
    /// <param name="lstObjReportData">lstObjReportData</param>
    ///
    /// <history>2022/01/10 Created Applied Technology
    /// 2025/03/28 Arent. 高速化 
    /// </history>
    /// ================================================================================
    public void SetParmeterToElement( Document doc, ObjectElement copySourceElement, List<ObjectReportCopy> lstObjReportData )
    {
      if ( copySourceElement == null || copySourceElement.ElementCurrent == null )
        return ;

      // コピー対象のパラメータを事前に抽出
      var parametersToCopy = copySourceElement.ObjectParameterData.Where( objParameter => objParameter.IsCopy && objParameter.CurrentParameter != null ).ToList() ;

      if ( parametersToCopy.Count == 0 ) {
        return ;
      }

      // 単一のトランザクションを開始
      Transaction trans = new Transaction( doc, "Copy all parameters" ) ;
      trans.Start() ;

      try {
        // 警告抑制の設定
        FailureHandlingOptions options = trans.GetFailureHandlingOptions() ;
        SuppressWarning preprocessor = new SuppressWarning() ;
        options.SetFailuresPreprocessor( preprocessor ) ;
        options.SetClearAfterRollback( true ) ;
        trans.SetFailureHandlingOptions( options ) ;

        foreach ( ObjectParameter objParameter in parametersToCopy ) {
          var currentParameter = objParameter.CurrentParameter ;

          Stopwatch parameterStopwatch = Stopwatch.StartNew() ;

          foreach ( var objReportData in lstObjReportData ) {
            // パラメータを検索
            Stopwatch findParameterStopwatch = Stopwatch.StartNew() ;
            ObjectParameter findParameter = objReportData.ObjectParameterData.FirstOrDefault( param => param.NameParameter == objParameter.NameParameter && param.ElementIdGroup == objParameter.ElementIdGroup ) ;

            findParameterStopwatch.Stop() ;

            if ( findParameter == null ) {
              // 未発見のパラメータを新規作成して追加
              ObjectParameter objNewPr = new ObjectParameter() { IsCopy = true, NameParameter = objParameter.NameParameter, StatusCopyParameter = StatusCopy.CS_CanFindParameter } ;
              objReportData.ObjectParameterData.Add( objNewPr ) ;
              continue ;
            }

            // コピーフラグを設定
            findParameter.IsCopy = true ;

            // パラメータがnullの場合は処理をスキップ
            if ( findParameter.CurrentParameter == null ) {
              findParameter.StatusCopyParameter = StatusCopy.CS_CantCopy ;
              continue ;
            }

            // 読み取り専用パラメータはコピー不可
            if ( findParameter.CurrentParameter.IsReadOnly ) {
              findParameter.StatusCopyParameter = StatusCopy.CS_ReadOnlyOrRecipe ;
              continue ;
            }

            // StorageTypeが異なる場合
            if ( findParameter.CurrentParameter.StorageType != currentParameter.StorageType ) {
              findParameter.StatusCopyParameter = CopyParameterWithDiffrenceType( doc, currentParameter, findParameter.CurrentParameter ) ;
              continue ;
            }

            // 要素IDが範囲外でないかチェック
            if ( ValidateOutOfRangeElementId( doc, currentParameter.AsElementId(), findParameter.CurrentParameter.AsElementId() ) ) {
              findParameter.StatusCopyParameter = StatusCopy.CS_OutOfRange ;
              continue ;
            }

            // パラメータ設定
            bool result = false ;
            bool isString = false ;
            
            try {
              if ( currentParameter.StorageType == StorageType.Double )
                result = findParameter.CurrentParameter.Set( currentParameter.AsDouble() ) ;
              else if ( currentParameter.StorageType == StorageType.Integer )
                result = findParameter.CurrentParameter.Set( currentParameter.AsInteger() ) ;
              else if ( currentParameter.StorageType == StorageType.ElementId )
                result = findParameter.CurrentParameter.Set( currentParameter.AsElementId() ) ;
              else if ( currentParameter.StorageType == StorageType.String ) {
                result = findParameter.CurrentParameter.Set( currentParameter.AsString() ) ;
                isString = true ;
              }

              // 設定成功
              if ( result )
                findParameter.StatusCopyParameter = StatusCopy.CS_Success ;

              if ( SuppressWarning._IsHasError ) {
                findParameter.StatusCopyParameter = StatusCopy.CS_CantCopy ;
                SuppressWarning._IsHasError = false ;
              }
            }
            catch ( System.Exception ex ) {
              // コピー失敗
              findParameter.StatusCopyParameter = StatusCopy.CS_CantCopy ;

              // エラーメッセージ追加
              if ( ! isString )
                SuppressWarning.AppendError( doc, objReportData.ElementCurrent, ex.Message ) ;
            }
          }
        }

        // トランザクションをコミット
        trans.Commit() ;
      }
      catch ( Exception ex ) {
        // 全体的なエラーが発生した場合はロールバック
        trans.RollBack() ;
        Console.WriteLine( $"[Error] Transaction failed: {ex.Message}" ) ;
      }
    }

    /// ================================================================================
    /// <summary>Validate Out Of Range ElementId</summary>
    ///
    /// <param name="doc">Document</param>
    /// <param name="sourceId">ElementId</param>
    /// <param name="destId">ElementId</param>
    /// <returns></returns>
    ///
    /// <history>2022/01/10 Created Applied Technology</history>
    /// ================================================================================
    private bool ValidateOutOfRangeElementId( Document doc, ElementId sourceId, ElementId destId )
    {
      Element eleSource = doc.GetElement( sourceId ) ;
      if ( eleSource == null )
        return false ;

      Element eleDest = doc.GetElement( destId ) ;
      if ( eleDest == null )
        return false ;

      if ( eleSource.GetType().Equals( eleDest.GetType() ) == false )
        return true ;

      return false ;
    }

    /// ================================================================================
    /// <summary>Copy Parameter With DiffrenceT ype</summary>
    ///
    /// <param name="doc">Current document</param>
    /// <param name="prSource">Parameter</param>
    /// <param name="prCurrent">Parameter</param>
    /// <returns></returns>
    ///
    /// <history>2022/01/10 Created Applied Technology</history>
    /// ================================================================================
    private StatusCopy CopyParameterWithDiffrenceType( Document doc, Parameter prSource, Parameter prCurrent )
    {
      Transaction trans = new Transaction( doc, "Copy single parameter" ) ;
      trans.Start() ;

      try {
        switch ( prSource.StorageType ) {
          case StorageType.Integer :
          {
            // Value of parameter
            int valInt = prSource.AsInteger() ;

            switch ( prCurrent.StorageType ) {
              case StorageType.Double :
              {
                try {
                  // Convert to double
                  double valPr = System.Convert.ToDouble( valInt ) ;
                  valPr = UnitUtils.ConvertToInternalUnits( valPr, prCurrent.GetUnitTypeId() ) ;

                  // Set parameter
                  return SetParameterObject( prCurrent, valPr ) ;
                }
                catch ( System.Exception ex ) {
                  // Add error mess
                  SuppressWarning.AppendError( doc, prSource.Element, ex.Message ) ;

                  // Cant copy parameter
                  return StatusCopy.CS_CantCopy ;
                }
              }
              case StorageType.String :
              {
                try {
                  // Convert int to string
                  string valPr = prSource.AsValueString() ;

                  // Set parameter
                  return SetParameterObject( prCurrent, valPr ) ;
                }
                catch ( System.Exception ex ) {
                  // Add error mess
                  SuppressWarning.AppendError( doc, prSource.Element, ex.Message ) ;

                  // Cant copy parameter
                  return StatusCopy.CS_CantCopy ;
                }
              }

              case StorageType.ElementId :
              {
                // Cant copy parameter
                return StatusCopy.CS_CantCopy ;
              }
            }
          }
            break ;

          case StorageType.Double :
          {
            // Value of parameter
            string valDoubleStr = prSource.AsValueString() ;
            if ( double.TryParse( valDoubleStr, out double valDouble ) == false )
              return StatusCopy.CS_CantCopy ;

            switch ( prCurrent.StorageType ) {
              case StorageType.Integer :
              {
                try {
                  // Convert double to int
                  int valPr = (int)valDouble ;

                  if ( valDouble >= int.MinValue && valDouble <= int.MaxValue ) // Set parameter
                    return SetParameterObject( prCurrent, valPr ) ;
                  else
                    return StatusCopy.CS_OutOfRange ;
                }
                catch ( System.Exception ex ) {
                  // Add error mess
                  SuppressWarning.AppendError( doc, prSource.Element, ex.Message ) ;

                  // Cant copy parameter
                  return StatusCopy.CS_CantCopy ;
                }
              }

              case StorageType.String :
                try {
                  // Set parameter
                  return SetParameterObject( prCurrent, valDoubleStr ) ;
                }
                catch ( System.Exception ex ) {
                  // Add error mess
                  SuppressWarning.AppendError( doc, prSource.Element, ex.Message ) ;

                  // Cant copy parameter
                  return StatusCopy.CS_CantCopy ;
                }

              case StorageType.ElementId :
                // Cant copy parameter
                return StatusCopy.CS_CantCopy ;
            }
          }
            break ;

          case StorageType.String :
          {
            // Value of parameter
            string valStr = prSource.AsString() ;

            switch ( prCurrent.StorageType ) {
              case StorageType.Integer :
                try {
                  if ( double.TryParse( valStr, out double valPrDouble ) == false )
                    return StatusCopy.CS_CantCopy ;

                  if ( valPrDouble >= int.MinValue && valPrDouble <= int.MaxValue ) {
                    int valPr = (int)valPrDouble ;
                    // Set parameter
                    return SetParameterObject( prCurrent, valPr ) ;
                  }
                  else
                    return StatusCopy.CS_OutOfRange ;
                }
                catch ( System.Exception ex ) {
                  // Add error mess
                  SuppressWarning.AppendError( doc, prSource.Element, ex.Message ) ;

                  // Cant copy parameter
                  return StatusCopy.CS_CantCopy ;
                }

              case StorageType.Double :
                try {
                  if ( double.TryParse( valStr, out double valPr ) == false )
                    return StatusCopy.CS_CantCopy ;

                  // Set parameter
                  return SetParameterObject( prCurrent, valPr ) ;
                }
                catch ( System.Exception ex ) {
                  // Add error mess
                  SuppressWarning.AppendError( doc, prSource.Element, ex.Message ) ;

                  // Cant copy parameter
                  return StatusCopy.CS_CantCopy ;
                }

              case StorageType.ElementId :
                // Cant copy parameter
                return StatusCopy.CS_CantCopy ;
            }
          }
            break ;

          case StorageType.ElementId :
          {
            // Value of parameter
            ElementId valElementId = prSource.AsElementId() ;
            if ( valElementId == null )
              return StatusCopy.CS_CantCopy ;

            int intVal = Int32.Parse(valElementId.ToString()) ;

            switch ( prCurrent.StorageType ) {
              case StorageType.Integer :
                try {
                  // Set parameter
                  return SetParameterObject( prCurrent, intVal ) ;
                }
                catch ( System.Exception ex ) {
                  // Add error mess
                  SuppressWarning.AppendError( doc, prSource.Element, ex.Message ) ;

                  // Cant copy parameter
                  return StatusCopy.CS_CantCopy ;
                }

              case StorageType.Double :
              {
                try {
                  // Convert to double
                  double valPr = System.Convert.ToDouble( intVal ) ;

                  // Set parameter
                  return SetParameterObject( prCurrent, valPr ) ;
                }
                catch ( System.Exception ex ) {
                  // Add error mess
                  SuppressWarning.AppendError( doc, prSource.Element, ex.Message ) ;

                  // Cant copy parameter
                  return StatusCopy.CS_CantCopy ;
                }
              }

              case StorageType.String :
              {
                try {
                  // Convert to double
                  string valPr = System.Convert.ToString( intVal ) ;

                  // Set parameter
                  return SetParameterObject( prCurrent, valPr ) ;
                }
                catch ( System.Exception ex ) {
                  // Add error mess
                  SuppressWarning.AppendError( doc, prSource.Element, ex.Message ) ;

                  // Cant copy parameter
                  return StatusCopy.CS_CantCopy ;
                }
              }
              case StorageType.ElementId :
              {
                try {
                  if ( ValidateOutOfRangeElementId( doc, prSource.AsElementId(), prCurrent.AsElementId() ) )
                    return StatusCopy.CS_OutOfRange ;

                  // Set parameter
                  return SetParameterObject( prCurrent, valElementId ) ;
                }
                catch ( System.Exception ex ) {
                  // Add error mess
                  SuppressWarning.AppendError( doc, prSource.Element, ex.Message ) ;

                  // Cant copy parameter
                  return StatusCopy.CS_CantCopy ;
                }
              }
            }
          }
            break ;
        }
      }
      catch ( System.Exception ex ) {
        // Add error mess
        SuppressWarning.AppendError( doc, prSource.Element, ex.Message ) ;
      }
      finally {
        trans.Commit() ;
      }

      return StatusCopy.CS_Null ;
    }

    /// ================================================================================
    /// <summary>Set Parameter Object</summary>
    ///
    /// <param name="prCopy">Parameter</param>
    /// <param name="prVal">prVal</param>
    /// <returns></returns>
    ///
    /// <history>2022/01/10 Created Applied Technology</history>
    /// ================================================================================
    private StatusCopy SetParameterObject( Parameter prCopy, ElementId prVal )
    {
      var statusCopy = prCopy.Set( prVal ) ;
      if ( statusCopy )
        return StatusCopy.CS_Success ;
      else
        return StatusCopy.CS_CantCopy ;
    }

    /// ================================================================================
    /// <summary>Set Parameter Object</summary>
    ///
    /// <param name="prCopy">Parameter</param>
    /// <param name="prVal">prVal</param>
    /// <returns></returns>
    ///
    /// <history>2022/01/10 Created Applied Technology</history>
    /// ================================================================================
    private StatusCopy SetParameterObject( Parameter prCopy, int prVal )
    {
      var statusCopy = prCopy.Set( prVal ) ;
      if ( statusCopy )
        return StatusCopy.CS_Success ;
      else
        return StatusCopy.CS_CantCopy ;
    }

    /// ================================================================================
    /// <summary>Set Parameter Object</summary>
    ///
    /// <param name="prCopy">Parameter</param>
    /// <param name="prVal">prVal</param>
    /// <returns>Status of copy</returns>
    ///
    /// <history>2022/01/10 Created Applied Technology</history>
    /// ================================================================================
    private StatusCopy SetParameterObject( Parameter prCopy, double prVal )
    {
      var statusCopy = prCopy.Set( prVal ) ;
      if ( statusCopy )
        return StatusCopy.CS_Success ;
      else
        return StatusCopy.CS_CantCopy ;
    }

    /// ================================================================================
    /// <summary>Set Parameter Object</summary>
    ///
    /// <param name="prCopy">Parameter</param>
    /// <param name="prVal">Parameter value</param>
    /// <returns>Status of copy</returns>
    ///
    /// <history>2022/01/10 Created Applied Technology</history>
    /// ================================================================================
    private StatusCopy SetParameterObject( Parameter prCopy, string prVal )
    {
      var statusCopy = prCopy.Set( prVal ) ;
      if ( statusCopy )
        return StatusCopy.CS_Success ;
      else
        return StatusCopy.CS_CantCopy ;
    }

    /// ================================================================================
    /// <summary>Set Unit of current document to none</summary>
    ///
    /// <param name="curentDoc">Current document</param>
    ///
    /// <returns>True or false</returns>
    ///
    /// <history>2022/01/10 Created Applied Technology</history>
    /// ================================================================================
    public bool SetProjectUnitDisplayToNone( Document curentDoc )
    {
      Units units = curentDoc.GetUnits() ;
      if ( units == null )
        return false ;

      var lstAllUnit = UnitUtils.GetAllMeasurableSpecs() ;

      foreach ( var ut in lstAllUnit ) {
        if ( UnitUtils.IsMeasurableSpec( ut ) == false )
          continue ;

        if ( Units.IsModifiableSpec( ut ) == false )
          continue ;

        FormatOptions fmtOpts = units.GetFormatOptions( ut ) ;
        if ( fmtOpts == null )
          continue ;

        var symbolType = fmtOpts.GetSymbolTypeId() ;

        symbolType.Clear() ;
        fmtOpts.SetSymbolTypeId( symbolType ) ;

        units.SetFormatOptions( ut, fmtOpts ) ;
      }

      Transaction trans = new Transaction( curentDoc, "Set unit to none" ) ;
      trans.Start() ;

      // Start set unit
      curentDoc.SetUnits( units ) ;

      trans.Commit() ;

      return true ;
    }

    /// ================================================================================
    /// <summary>Set Unit of current document to Previous</summary>
    ///
    /// <param name="curentDoc">Current document</param>
    /// <param name="units">Current unit of document</param>
    ///
    /// <returns>True or false</returns>
    ///
    /// <history>2022/01/10 Created Applied Technology</history>
    /// ================================================================================
    public bool SetPreviousProjectUnitDisplay( Document curentDoc, Units units )
    {
      if ( units == null )
        return false ;

      Transaction trans = new Transaction( curentDoc, "Set unit to Previous" ) ;
      trans.Start() ;

      // Start set unit
      curentDoc.SetUnits( units ) ;

      trans.Commit() ;

      return true ;
    }

    #endregion Member Functions
  }
}
