using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using PipeSizing.Components;

namespace PipeSizing.Components
{
  public sealed class Elements
  {
    private readonly IList<Pipe> _allPipes;

    public Elements(UIDocument rvtUIDoc)
    {
      RvtUIDoc = rvtUIDoc;
      _allPipes = PipeAry();
    }

    public UIDocument RvtUIDoc { get; }

    public Document RvtDBDoc => RvtUIDoc.Document;

    public IList<Pipe> PipeAry()
    {
      IList<Pipe> ret = new List<Pipe>();
      var fec = new FilteredElementCollector(RvtDBDoc);
      fec.OfCategory(BuiltInCategory.OST_PipeCurves);
      fec.WhereElementIsNotElementType();

      foreach (Pipe pipe in fec)
      {
        ret.Add(pipe);
      }

      return ret;
    }

    public IList<Pipe> GetSameConnectorPipe(Element connector, IList<Pipe> pipeAry)
    {
      IList<Pipe> ret = new List<Pipe>();
      IList<string> ids = new List<string>();

      IList<Element> cnctElemes = GetConnectorConnectElems(connector);

      foreach (Element elem in cnctElemes)
      {
        if (elem.Category.Id.ToString().Equals(((int)BuiltInCategory.OST_PipeCurves).ToString()))
        {
          Pipe pipe = (Pipe)elem;

          if (!ids.Contains(pipe.Id.ToString()))
          {
            ret.Add(pipe);
            ids.Add(pipe.Id.ToString());
          }
        }
        else
        {
          IList<Element> cncts = GetConnectorConnectElems(elem);

          foreach (Element el in cncts)
          {
            if (el.Category.Id.ToString().Equals(((int)BuiltInCategory.OST_PipeCurves).ToString()))
            {
              Pipe pipe = (Pipe)el;

              if (!ids.Contains(pipe.Id.ToString()))
              {
                ret.Add(pipe);
                ids.Add(pipe.Id.ToString());
              }
            }
            else
            {
              IList<Element> cs = GetConnectorConnectElems(el);

              foreach (Element e in cs)
              {
                if (e.Category.Id.ToString().Equals(((int)BuiltInCategory.OST_PipeCurves).ToString()))
                {
                  Pipe pipe = (Pipe)e;

                  if (!ids.Contains(pipe.Id.ToString()))
                  {
                    ret.Add(pipe);
                    ids.Add(pipe.Id.ToString());
                  }
                }
              }
            }
          }
        }
      }

      return ret;
    }

    IList<Element> GetConnectorConnectElems(Element connector)
    {
      IList<Element> ret = new List<Element>();

      FamilyInstance famIns = connector as FamilyInstance;

      if (famIns == null)
      {
        return ret;
      }

      MEPModel mepMdl = famIns.MEPModel;

      ConnectorManager cnctMgr = mepMdl.ConnectorManager;

      ConnectorSet cnctSet = cnctMgr.Connectors;

      foreach (Connector cnct in cnctSet)
      {
        ConnectorSet cs = cnct.AllRefs;

        foreach (Connector c in cs)
        {
          ret.Add(c.Owner);
        }
      }

      return ret;
    }

    public IList<Pipe> AllPipeAry => _allPipes;

    public IList<Element> SelectPipeAry
    {
      get
      {
        IList<Element> ret = new List<Element>();

        foreach (ElementId eId in RvtUIDoc.Selection.GetElementIds())
        {
          Element elem = RvtDBDoc.GetElement(eId);

          if (elem.Category != null)
          {
            if (elem.Category.Id.ToString().Equals(((int)BuiltInCategory.OST_PipeCurves).ToString()) ||
                elem.Category.Id.ToString().Equals(((int)BuiltInCategory.OST_FlexPipeCurves).ToString()))
            {
              ret.Add(elem);
            }
          }
        }

        return ret;
      }
    }

    public IList<FamilyInstance> SelectPipeFittingValveAry
    {
      get
      {
        IList<FamilyInstance> ret = new List<FamilyInstance>();

        foreach (ElementId eId in RvtUIDoc.Selection.GetElementIds())
        {
          Element elem = RvtDBDoc.GetElement(eId);

          if (elem.Category != null)
          {
            if (elem.Category.Id.ToString().Equals(((int)BuiltInCategory.OST_PipeFitting).ToString()))
            {
              FamilyInstance famIns = elem as FamilyInstance;
              if (famIns != null)
              {
                ret.Add(famIns);
              }
            }
            else if (elem.Category.Id.ToString().Equals(((int)BuiltInCategory.OST_PipeAccessory).ToString()))
            {
              FamilyInstance famIns = elem as FamilyInstance;
              if (famIns != null)
              {
                ret.Add(famIns);
              }
            }
          }
        }

        return ret;
      }
    }
  }
}
