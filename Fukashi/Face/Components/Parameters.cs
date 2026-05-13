using System;
using Collections = System.Collections;
using System.IO;
using Revit       = Autodesk.Revit;
using RvtExtApp   = ADSK.Ext.Fukashi;

namespace ADSK.Ext.Fukashi.Face.Components
{
  /// ================================================================================
  /// <summary>パラメータ</summary>
  /// ================================================================================
  public class Parameters
  {
    #region Member Variables

    /// <summary>属性</summary>
    private readonly RvtExtApp.Face.Components.Attribute _CmpAttribute;

    private readonly Revit.UI.UIDocument _rvtUiDoc;

    /// <summary>標準共有パラメータファイル名</summary>
    private readonly string _ShParamDefaultFileName;

    /// <summary>共有パラメータフォルダ名</summary>
    private readonly string _ShParamFolderName;

    /// <summary>共有パラメータファイル名</summary>
    private readonly string _ShParamFileName;

    /// <summary>共有パラメータグループ名</summary>
    private readonly string _ShParamGroupName;

    /// <summary>マテリアル</summary>
    private string _Material;

    #endregion

    #region Constructor

    public Parameters(Revit.UI.UIDocument rvtUiDoc, RvtExtApp.Face.Components.Attribute cmpAttribute)
    {
      _rvtUiDoc = rvtUiDoc ?? throw new ArgumentNullException(nameof(rvtUiDoc));
      _CmpAttribute = cmpAttribute ?? throw new ArgumentNullException(nameof(cmpAttribute));

      _ShParamDefaultFileName = null;
      Revit.DB.DefinitionFile defFile = GetSharedParameterFile();
      if (defFile != null)
      {
        _ShParamDefaultFileName = defFile.Filename;
      }

      _ShParamFolderName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
      _ShParamFileName = _CmpAttribute.ResourceText("IDS_SHPARAM_FILE");
      _ShParamGroupName = _CmpAttribute.ResourceText("IDS_SHPARAM_GROUP");

      if (_ShParamDefaultFileName == null)
      {
        _ShParamDefaultFileName = Path.Combine(_ShParamFolderName, _ShParamFileName);
      }
    }

    #endregion

    #region Shared parameter file

    public Revit.DB.DefinitionFile GetSharedParameterFile()
    {
      try
      {
        return _rvtUiDoc.Application.Application.OpenSharedParameterFile();
      }
      catch
      {
        return null;
      }
    }

    public Revit.DB.DefinitionFile SetSharedParameterFile(object unused, string filePath)
    {
      try
      {
        var app = _rvtUiDoc.Application.Application;
        app.SharedParametersFilename = filePath;
        return app.OpenSharedParameterFile();
      }
      catch
      {
        return null;
      }
    }

    /// <summary>フォルダ名とファイル名で共有パラメータを開く。</summary>
    public Revit.DB.DefinitionFile SetSharedParameterFile(string folderName, string fileName)
    {
      return SetSharedParameterFile(null, Path.Combine(folderName ?? string.Empty, fileName ?? string.Empty));
    }

    public bool SetSharedParamDefault()
    {
      Revit.DB.DefinitionFile defFile = SetSharedParameterFile(null, _ShParamDefaultFileName);
      return defFile != null;
    }

    #endregion

    #region Definitions

    public
    bool SetDefinition(Revit.DB.Element elem,
                       Collections.Generic.IList<Revit.DB.Category> categories,
                       string defName,
                       Revit.DB.ForgeTypeId paramType,
                       Revit.DB.ForgeTypeId bltParamGroup,
                       bool visible,
                       int bindingMode)
    {
      bool ret = SetDefinition(elem,
                               _ShParamFolderName,
                               _ShParamFileName,
                               _ShParamGroupName,
                               categories,
                               defName,
                               paramType,
                               bltParamGroup,
                               visible,
                               bindingMode);
      return ret;
    }

    public
    bool SetDefinition(Revit.DB.Element elem,
                       Revit.DB.Category category,
                       string defName,
                       Revit.DB.ForgeTypeId paramType,
                       Revit.DB.ForgeTypeId bltParamGroup,
                       bool visible,
                       int bindingMode)
    {
      Collections.Generic.IList<Revit.DB.Category> categories = new Collections.Generic.List<Revit.DB.Category>();
      categories.Add(category);
      return SetDefinition(elem,
                           categories,
                           defName,
                           paramType,
                           bltParamGroup,
                           visible,
                           bindingMode);
    }

    public
    bool SetDefinition(Revit.DB.Element elem,
                       string folderName,
                       string fileName,
                       string groupName,
                       Collections.Generic.IList<Revit.DB.Category> categories,
                       string defName,
                       Revit.DB.ForgeTypeId paramType,
                       Revit.DB.ForgeTypeId bltParamGroup,
                       bool visible,
                       int bindingMode)
    {
      try
      {
        var doc = _rvtUiDoc.Document;
        var app = _rvtUiDoc.Application.Application;

        string filePath = Path.Combine(folderName, fileName);
        if (!File.Exists(filePath))
        {
          using (File.Create(filePath)) { }
        }

        string origFile = app.SharedParametersFilename;
        app.SharedParametersFilename = filePath;
        Revit.DB.DefinitionFile defFile = app.OpenSharedParameterFile();
        if (defFile == null)
          return false;

        Revit.DB.DefinitionGroup group = defFile.Groups.get_Item(groupName);
        if (group == null)
        {
          group = defFile.Groups.Create(groupName);
        }

        Revit.DB.Definition def = group.Definitions.get_Item(defName);
        if (def == null)
        {
          Revit.DB.ExternalDefinitionCreationOptions opts =
            new Revit.DB.ExternalDefinitionCreationOptions(defName, paramType);
          opts.Visible = visible;
          def = group.Definitions.Create(opts);
        }

        if (def != null)
        {
          Revit.DB.CategorySet catSet = new Revit.DB.CategorySet();
          foreach (Revit.DB.Category cat in categories ?? Array.Empty<Revit.DB.Category>())
          {
            if (cat != null)
              catSet.Insert(cat);
          }

          Revit.DB.BindingMap bindingMap = doc.ParameterBindings;

          bool bltLooksValid =
            bltParamGroup != null
            && !string.IsNullOrEmpty(bltParamGroup.TypeId);

          if (!bindingMap.Contains(def))
          {
            Revit.DB.Binding binding;
            if (bindingMode == 1)
            {
              binding = app.Create.NewTypeBinding(catSet);
            }
            else
            {
              binding = app.Create.NewInstanceBinding(catSet);
            }

            bindingMap.Insert(def, binding,
              bltLooksValid ? bltParamGroup : Revit.DB.GroupTypeId.General);
          }
        }

        try
        {
          app.SharedParametersFilename = origFile;
        }
        catch
        {
        }

        return true;
      }
      catch
      {
        return false;
      }
    }

    #endregion

    #region Values

    public void GetStrVal(string material)
    {
      _Material = material;
    }

    public void GetStrVal(ref string material)
    {
      material = _Material;
    }

    public void GetValueString(
      Revit.DB.Element elem,
      string defName,
      Revit.DB.ForgeTypeId paramType,
      Revit.DB.ForgeTypeId paramGroup,
      ref string sValue)
    {
      try
      {
        if (elem != null && !string.IsNullOrEmpty(defName))
        {
          Revit.DB.Parameter p = elem.LookupParameter(defName);
          if (p != null)
          {
            sValue = !string.IsNullOrEmpty(p.AsString()) ? p.AsString() : p.AsValueString();
          }
          else
          {
            foreach (Revit.DB.Parameter qp in elem.Parameters)
            {
              if (qp?.Definition?.Name == defName)
              {
                sValue = !string.IsNullOrEmpty(qp.AsString()) ? qp.AsString() : qp.AsValueString();
                return;
              }
            }
          }
        }
      }
      catch { }
    }

    public bool SetValue(
      Revit.DB.Element elem,
      string paramName,
      Revit.DB.ForgeTypeId paramType,
      Revit.DB.ForgeTypeId paramGroup,
      string value)
    {
      try
      {
        Revit.DB.Parameter param = elem?.LookupParameter(paramName);
        if (param != null && !param.IsReadOnly)
        {
          param.Set(value);
          return true;
        }
      }
      catch { }
      return false;
    }

    #endregion
  }
}
