using ADSK.JExtRAC.AutoLayoutTag.Entities;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text ;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AutoLayoutTag;

namespace ADSK.JExtRAC.AutoLayoutTag.Components
{
    /// ================================================================================
    /// <summary>Service</summary>
    /// ================================================================================
    public class Service
    {
        // Member variable

        #region Member Variables

        /// <summary>Attribute</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>Elements</summary>
        private RvtExtApp.Components.Elements _CmpElements;

        /// <summary>Geometry</summary>
        private RvtExtApp.Components.Geometry _CmpGeometry;

        /// <summary>Parameters</summary>
        private RvtExtApp.Components.Parameters _CmpParameters;

        /// <summary>Settings</summary>
        private RvtExtApp.Components.Settings _CmpSettings;

        /// <summary>DtItems</summary>
        private RvtExtApp.Entities.DtItems _EntDtItems;

        /// <summary>Const Tolerance</summary>
        private const double Tolerance = 1e-6;

        /// <summary> position tag</summary>
        private enum POST_TAG
        { CS_NULL, CS_LEFT, CS_TOP, CS_RIGHT, CS_BOT }

        /// <summary> const value</summary>
        private double distance = 10000;

        #endregion Member Variables

        // Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <param name="cmpAttribute"  >Attribute</param>
        /// <param name="cmpElements"   >Elements</param>
        /// <param name="cmpGeometry"   >Geometry</param>
        /// <param name="cmpParameters" >Parameters</param>
        /// <param name="cmpSettings"   >Settings</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public Service(RvtExtApp.Components.Attribute cmpAttribute,
                     RvtExtApp.Components.Elements cmpElements,
                     RvtExtApp.Components.Geometry cmpGeometry,
                     RvtExtApp.Components.Parameters cmpParameters,
                     RvtExtApp.Components.Settings cmpSettings)
        {
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;
            _EntDtItems = new RvtExtApp.Entities.DtItems(_CmpAttribute);
        }

        #endregion Constructor

        // Member functions

        #region Member Functions

        /// ================================================================================
        /// <summary>Create tag</summary>
        ///
        /// <param name="doc">document</param>
        /// <param name="dicCategory">dictionary value</param>
        /// <param name="outline">outline</param>
        /// <param name="leftRight">value check box left right</param>
        /// <param name="topBottom">value check box top bottom</param>
        /// <param name="optareaPremieses">option area premises</param>
        /// <param name="handlePresetTag">option handle preset tag</param>
        /// <returns></returns>
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        public bool CreateIndependentTag(Document doc, Collections.Generic.Dictionary<BuiltInCategory, FamilySymbol> dicCategory,
            Collections.Generic.List<Element> listElement, Outline outline, bool leftRight,
            bool topBottom, int tagLeader, int optgetObject, int optareaPremieses, int handlePresetTag)
        {
            bool ret = true;
            // Get value distance A and B from file settings

            GetSettingValue(_EntDtItems.FilePath, out double distA, out double distB);
            distA /= _CmpGeometry.UnitCoe;
            distB /= _CmpGeometry.UnitCoe;

            // Distance offset a and b
            double viewScale = doc.ActiveView.Scale * 1.0;
            distA *= viewScale;
            distB *= viewScale;

            // List element user pick by handle
            List<Element> listElementSetTag = listElement;

            // List element user choose by category
            if (optgetObject == 1)
                listElementSetTag = GetAllElement(doc, dicCategory);

            if (listElementSetTag == null || listElementSetTag.Count == 0)
                return false;

            if (optareaPremieses == 0)
            {
                // Get list element data
                List<ElementData> listEleDataAutomatic = GetListElementData(doc, dicCategory, listElementSetTag, distA);

                // Set automatic
                ret = CreateIndependentTagsAutomatic(doc, listEleDataAutomatic, leftRight, topBottom, tagLeader, handlePresetTag, distA, distB);
            }
            else
            {
                //If user choose set by hand get all element inside bounding box
                var listElementId = listElementSetTag.Select(x => x.Id).ToList();
                Collections.Generic.List<Element> listElementInsideBox = GetElementInsideOrOutsideBoundingBox(doc, outline, listElementId);

                // get list element data
                List<ElementData> listEleByHand = GetListElementData(doc, dicCategory, listElementInsideBox, distA);

                //set by hand
                ret = CreateIndependentTagsByHand(doc, listEleByHand, outline, leftRight, topBottom, tagLeader, handlePresetTag, distA);
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>Create tag automatic</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="listEleData">dictionary category value input</param>
        /// <param name="leftRight">value check box left right</param>
        /// <param name="topBottom">value check box top bottom</param>
        /// <param name="handlePresetTag">option handle preset tag</param>
        /// <param name="distA">value distance offset a</param>
        /// <param name="distB">value distance offset b</param>
        /// <returns></returns
        /// <returns></returns>
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        public bool CreateIndependentTagsAutomatic(Document doc, Collections.Generic.List<ElementData> listEleData,
            bool leftRight, bool topBottom, int tagLeader, int handlePresetTag, double distA, double distB)

        {
            // Return value
            bool ret = true;

            // List tag need delete if user choose option handle preset tag = 1
            Collections.Generic.List<ElementId> listTagExits = null;

            // Delete tag exits of element
            if (handlePresetTag == 1)
                DeleteAllTag(doc, listEleData);

            if (listEleData == null || listEleData.Count == 0)
                return false;
            foreach (var eleData in listEleData)
            {
                if (eleData == null)
                    continue;
                Element ele = eleData.ElementOrigin as Element;
                if (ele == null)
                    continue;
                FamilySymbol tagType = eleData.TagSymbol as FamilySymbol;
                if (!tagType.IsActive)
                    tagType.Activate();
                // Get Reference
                Reference reference = new Reference(ele);
                if (reference == null)
                    continue;

                // Preset tag processing
                listTagExits = _CmpElements.GetAllTagOfElement(doc, ele);

                //Set tag for element without tag
                if (handlePresetTag == 0 && listTagExits.Count > 0)
                    continue;

                //Get position tag
                XYZ pos = GetCenterElement(doc, ele);

                // position tag
                POST_TAG posittionTag = GetStatusLocationTag(doc, ele, tagType, distA, distB, leftRight, topBottom, out XYZ tagHead);
                if (tagHead == null)
                    continue;

                IndependentTag tag;
                if (tagLeader == 0)
                {
                    tag = IndependentTag.Create(doc, tagType.Id, doc.ActiveView.Id,
                                                           reference, true, TagOrientation.Horizontal, pos);
                    if (tag != null)
                    {
                        tag.LeaderEndCondition = LeaderEndCondition.Free;
                        tag.SetLeaderEnd(reference, pos);
                        tag.TagHeadPosition = tagHead;
                        tag.SetLeaderElbow(reference, pos);
                        tag.LeaderEndCondition = LeaderEndCondition.Attached;
                    }
                }
                else
                {
                    tag = IndependentTag.Create(doc, tagType.Id, doc.ActiveView.Id,
                                                            reference, false, TagOrientation.Horizontal, tagHead);
                }

                doc.Regenerate();
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>Get type of tag</summary>
        ///
        /// <param name="dicCategory">dictionary input</param>
        /// <param name="ele">element</param>
        /// <returns></returns>
        /// <returns></returns>
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private FamilySymbol GetFamilySymbolUser(Collections.Generic.Dictionary<BuiltInCategory, FamilySymbol> dicCategory, Element ele)
        {
            FamilySymbol retVal = null;
            if (dicCategory == null || dicCategory.Count == 0 || ele == null || ele.Category == null)
                return null;

            foreach (var pair in dicCategory)
            {
                if (pair.Value == null)
                    continue;
                BuiltInCategory builtIn = pair.Key;

                if (ele.Category.Id.ToString() == ((int)builtIn).ToString())
                {
                    retVal = pair.Value as FamilySymbol;
                    return retVal;
                }
            }
            return retVal;
        }

        /// ================================================================================
        /// <summary>Get status location tag</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="element">Element</param>
        /// <param name="tagType">FamilyType off tag</param>
        /// <param name="aOffset">Distance offset</param>
        /// <param name="bOffset">Distance b offset</param>
        /// <param name="isLeftRight">is Left right</param>
        /// <param name="isTopDown">bool is top bottom</param>
        /// <param name="positionTag"> position tag</param>
        /// <returns></returns>
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private POST_TAG GetStatusLocationTag(Document doc, Element element, FamilySymbol tagType, double aOffset, double bOffset,
            bool isLeftRight, bool isTopDown, out XYZ positionTag)
        {
            positionTag = positionTag = GetCenterElement(doc, element);
            if (element == null)
                return POST_TAG.CS_NULL;

            if (tagType == null)
                return POST_TAG.CS_NULL;

            if (GetBounbdingboxOfTag(doc, element, tagType.Id, aOffset, out BoundingBoxXYZ boundingBoxTagOffset, out BoundingBoxXYZ boundingBoxOrigin) == false)
                return POST_TAG.CS_NULL;

            if (GetBoundingboxWithNewOffset(doc, element, boundingBoxTagOffset, aOffset, bOffset, out List<BoundingBoxXYZ> lstBboxOffset) == false)
                return POST_TAG.CS_NULL;

            if (GetWidthAndHeightBoundingBox(boundingBoxOrigin, out double widthTag, out double heightTag) == false)
                return POST_TAG.CS_NULL;

            var bbBoxElementCurrent = element.get_BoundingBox(doc.ActiveView);
            if (bbBoxElementCurrent == null)
                return POST_TAG.CS_NULL;

            // Get width and height bounding box of element current
            if (GetWidthAndHeightBoundingBox(bbBoxElementCurrent, out double widthElement, out double heightElement) == false)
                return POST_TAG.CS_NULL;
            // Check left
            bool isLeft = CheckIntersectionBoundingBox(doc, lstBboxOffset[0], element.Id);

            //Check top
            bool isTop = CheckIntersectionBoundingBox(doc, lstBboxOffset[1], element.Id);

            //check right
            bool isRight = CheckIntersectionBoundingBox(doc, lstBboxOffset[2], element.Id);

            // check bottom
            bool isBottom = CheckIntersectionBoundingBox(doc, lstBboxOffset[3], element.Id);

            // point center element
            XYZ pos = GetCenterElement(doc, element);

            if (isLeftRight && isTopDown)
            {
                if (isLeft == false)
                {
                    // get position set tag
                    positionTag = GetPointOnVector(pos, doc.ActiveView.RightDirection.Negate(), aOffset + bOffset + widthTag / 2 + widthElement / 2);
                    return POST_TAG.CS_LEFT;
                }
                else if (isTop == false)
                {
                    // get position set tag
                    positionTag = GetPointOnVector(pos, doc.ActiveView.UpDirection, aOffset + bOffset + heightTag / 2 + heightElement / 2);
                    return POST_TAG.CS_TOP;
                }
                else if (isRight == false)
                {
                    // get position set tag
                    positionTag = GetPointOnVector(pos, doc.ActiveView.RightDirection, aOffset + bOffset + widthTag / 2 + widthElement / 2);
                    return POST_TAG.CS_RIGHT;
                }
                else if (isBottom == false)
                {
                    // get position set tag
                    positionTag = GetPointOnVector(pos, doc.ActiveView.UpDirection.Negate(), aOffset + bOffset + heightTag / 2 + heightElement / 2);
                    return POST_TAG.CS_BOT;
                }
                else
                {
                    // get position set tag
                    positionTag = GetPointOnVector(pos, doc.ActiveView.RightDirection.Negate(), aOffset + bOffset + widthTag / 2 + widthElement / 2);
                    return POST_TAG.CS_LEFT;
                }
            }
            else if (isLeftRight && isTopDown == false)
            {
                if (isLeft == false)
                {
                    // get position set tag
                    positionTag = GetPointOnVector(pos, doc.ActiveView.RightDirection.Negate(), aOffset + bOffset + widthTag / 2 + widthElement / 2);
                    return POST_TAG.CS_LEFT;
                }
                else if (isRight == false)
                {
                    // get position set tag
                    positionTag = GetPointOnVector(pos, doc.ActiveView.RightDirection, aOffset + bOffset + widthTag / 2 + widthElement / 2);
                    return POST_TAG.CS_RIGHT;
                }
                else
                {
                    // get position set tag
                    positionTag = GetPointOnVector(pos, doc.ActiveView.RightDirection.Negate(), aOffset + bOffset + widthTag / 2 + widthElement / 2);
                    return POST_TAG.CS_LEFT;
                }
            }
            else if (isLeftRight == false && isTopDown)
            {
                if (isTop == false)
                {
                    // get position set tag
                    positionTag = GetPointOnVector(pos, doc.ActiveView.UpDirection, aOffset + bOffset + heightTag / 2 + heightElement / 2);
                    return POST_TAG.CS_TOP;
                }
                else if (isBottom == false)
                {
                    // get position set tag
                    positionTag = GetPointOnVector(pos, doc.ActiveView.UpDirection.Negate(), aOffset + bOffset + heightTag / 2 + heightElement / 2);
                    return POST_TAG.CS_BOT;
                }
                else
                {
                    // get position set tag
                    positionTag = GetPointOnVector(pos, doc.ActiveView.UpDirection, aOffset + bOffset + heightTag / 2 + heightElement / 2);
                    return POST_TAG.CS_TOP;
                }
            }
            else
            {
                // get position set tag
                positionTag = GetPointOnVector(pos, doc.ActiveView.RightDirection.Negate(), aOffset + bOffset + widthTag / 2 + widthElement / 2);
                return POST_TAG.CS_NULL;
            }
        }

        /// ================================================================================
        /// <summary>Get element data</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="dicCateType">Dictionary input</param>
        /// <param name="listElement">List element</param>
        /// <returns></returns>
        ///
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private List<RvtExtApp.Entities.ElementData> GetListElementData(Document doc, Dictionary<BuiltInCategory, FamilySymbol> dicCateType,
            Collections.Generic.IList<Element> listElement, double distA)

        {
            List<RvtExtApp.Entities.ElementData> retVal = new List<RvtExtApp.Entities.ElementData>();
            foreach (var ele in listElement)
            {
                if (ele == null)
                    continue;

                // Element
                RvtExtApp.Entities.ElementData elementDistance = new RvtExtApp.Entities.ElementData();

                FamilySymbol tagSymbol = GetFamilySymbolUser(dicCateType, ele);

                //element set tag
                elementDistance.ElementOrigin = ele;

                //check
                elementDistance.IsCheck = false;

                //family type of tag
                elementDistance.TagSymbol = tagSymbol;
                if (elementDistance.TagSymbol == null)
                    continue;

                GetBounbdingboxOfTag(doc, ele, tagSymbol.Id, distA, out BoundingBoxXYZ boundingboxOff, out BoundingBoxXYZ boundingBoxOri);
                // center point
                if (boundingboxOff == null)
                    continue;
                elementDistance.CenterPoint = (boundingboxOff.Min + boundingboxOff.Max) / 2;

                //width
                elementDistance.Width = (boundingboxOff.Max.X - boundingboxOff.Min.X);

                //height
                elementDistance.Height = (boundingboxOff.Max.Y - boundingboxOff.Min.Y);

                retVal.Add(elementDistance);
            }
            return retVal;
        }

        /// ================================================================================
        /// <summary>Get the index of the region to which the element's position belongs</summary>
        ///
        /// <param name="listElementData"></param>
        /// <param name="outLine"></param>
        /// <param name="leftRight"></param>
        /// <param name="topBottom"></param>
        ///
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private void GetPostionELement(
            Collections.Generic.List<RvtExtApp.Entities.ElementData> listElementData, Outline outLine, bool leftRight, bool topBottom)
        {
            //Collections.Generic.List<RvtExtApp.Entities.ElementData> retVal = new List<ElementData>();

            if (outLine == null)
                return;

            XYZ maxPointR = new XYZ(outLine.MaximumPoint.X, outLine.MaximumPoint.Y, 0);
            XYZ minPointL = new XYZ(outLine.MinimumPoint.X, outLine.MinimumPoint.Y, 0);
            XYZ maxPointL = new XYZ(minPointL.X, maxPointR.Y, 0);
            XYZ minPointR = new XYZ(maxPointR.X, minPointL.Y, 0);

            double h = maxPointR.DistanceTo(minPointR);
            double b = maxPointL.DistanceTo(maxPointR);

            XYZ vtMoveX = (maxPointR - maxPointL).Normalize();
            XYZ vtMoveY = (maxPointR - minPointR).Normalize();

            XYZ midPoint1 = GetPointOnVector(maxPointL, vtMoveX, b / 2);
            XYZ midPoint2 = GetPointOnVector(minPointR, vtMoveY, h / 2);
            XYZ midPoint3 = GetPointOnVector(minPointL, vtMoveX, b / 2);
            XYZ midPoint4 = GetPointOnVector(minPointL, vtMoveY, h / 2);

            XYZ centerPoint = GetPointOnVector(midPoint4, vtMoveX, b / 2);

            if (leftRight && !topBottom)
            {
                foreach (var eleData in listElementData)
                {
                    XYZ centerELe = eleData.CenterPoint;

                    // check position on region
                    if (CheckPosition(maxPointL, midPoint1, centerPoint, midPoint4, centerELe, true))
                    {
                        eleData.PostElement = POST_ELEMENT.CS_REGION1;
                    }
                    else if (CheckPosition(midPoint4, centerPoint, midPoint3, minPointL, centerELe, true))
                    {
                        eleData.PostElement = POST_ELEMENT.CS_REGION2;
                    }
                    else if (CheckPosition(midPoint1, maxPointR, midPoint2, centerPoint, centerELe, true))
                    {
                        eleData.PostElement = POST_ELEMENT.CS_REGION3;
                    }
                    else
                    {
                        eleData.PostElement = POST_ELEMENT.CS_REGION4;
                    }
                }
            }
            else if (!leftRight && topBottom)
            {
                foreach (var eleData in listElementData)
                {
                    XYZ centerELe = eleData.CenterPoint;

                    // check position on region
                    if (CheckPosition(maxPointL, midPoint1, centerPoint, midPoint4, centerELe, true))
                    {
                        eleData.PostElement = POST_ELEMENT.CS_REGION1;
                    }
                    else if (CheckPosition(midPoint1, maxPointR, midPoint2, centerPoint, centerELe, true))
                    {
                        eleData.PostElement = POST_ELEMENT.CS_REGION2;
                    }
                    else if (CheckPosition(midPoint4, centerPoint, midPoint3, minPointL, centerELe, true))
                    {
                        eleData.PostElement = POST_ELEMENT.CS_REGION3;
                    }
                    else
                    {
                        eleData.PostElement = POST_ELEMENT.CS_REGION4;
                    }
                }
            }
            else
            {
                foreach (var eleData in listElementData)
                {
                    XYZ centerELe = eleData.CenterPoint;

                    // check position on region
                    if (CheckPosition(maxPointL, centerPoint, midPoint4, null, centerELe, false))
                    {
                        eleData.PostElement = POST_ELEMENT.CS_REGION1;
                    }
                    else if (CheckPosition(maxPointL, midPoint1, centerPoint, null, centerELe, false))
                    {
                        eleData.PostElement = POST_ELEMENT.CS_REGION2;
                    }
                    else if (CheckPosition(midPoint1, maxPointR, centerPoint, null, centerELe, false))
                    {
                        eleData.PostElement = POST_ELEMENT.CS_REGION3;
                    }
                    else if (CheckPosition(maxPointR, midPoint2, centerPoint, null, centerELe, false))
                    {
                        eleData.PostElement = POST_ELEMENT.CS_REGION4;
                    }
                    else if (CheckPosition(centerPoint, midPoint2, minPointR, null, centerELe, false))
                    {
                        eleData.PostElement = POST_ELEMENT.CS_REGION5;
                    }
                    else if (CheckPosition(centerPoint, minPointR, midPoint3, null, centerELe, false))
                    {
                        eleData.PostElement = POST_ELEMENT.CS_REGION6;
                    }
                    else if (CheckPosition(centerPoint, midPoint3, minPointL, null, centerELe, false))
                    {
                        eleData.PostElement = POST_ELEMENT.CS_REGION7;
                    }
                    else
                    {
                        eleData.PostElement = POST_ELEMENT.CS_REGION8;
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>Check position of element</summary>
        ///
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="pointNeedCheck"></param>
        /// <param name="isRectang"></param>
        /// <returns></returns>
        ///
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private static bool CheckPosition(XYZ p1, XYZ p2, XYZ p3, XYZ p4, XYZ pointNeedCheck, bool isRectang)
        {
            double s1, s2, s3, s4;

            double sumArea = 0.0;

            double vArea = 0.0;

            if (isRectang)
            {
                vArea = CalAreaRec(p1, p2, p3, p4);

                s1 = CalAreaTri(p1, p2, pointNeedCheck);

                s2 = CalAreaTri(p2, p3, pointNeedCheck);

                s3 = CalAreaTri(pointNeedCheck, p3, p4);

                s4 = CalAreaTri(pointNeedCheck, p4, p1);

                sumArea = s1 + s2 + s3 + s4;

                if (Math.Round(s1, 5) == 0 && Math.Round(sumArea, 5) == Math.Round(vArea, 5)
                    || Math.Round(s2, 5) == 0 && Math.Round(sumArea, 5) == Math.Round(vArea, 5)
                    || Math.Round(s3, 5) == 0 && Math.Round(sumArea, 5) == Math.Round(vArea, 5)
                    || Math.Round(s4, 5) == 0 && Math.Round(sumArea, 5) == Math.Round(vArea, 5))
                {
                    //System.Windows.Forms.MessageBox.Show("Điểm nằm trên cạnh của đa giác", "Show");
                    return true;
                }
                else if (Math.Round(sumArea, 5) == Math.Round(vArea, 5))
                {
                    //System.Windows.Forms.MessageBox.Show("Điểm nằm trong đa giác", "Show");
                    return true;
                }
                else if (Math.Round(sumArea, 5) > Math.Round(vArea, 5))
                {
                    //System.Windows.Forms.MessageBox.Show("Điểm nằm ngoài đa giác", "Show");
                    return false;
                }
            }
            else
            {
                vArea = CalAreaTri(p1, p2, p3);

                s1 = CalAreaTri(p1, p2, pointNeedCheck);

                s2 = CalAreaTri(p2, p3, pointNeedCheck);

                s3 = CalAreaTri(pointNeedCheck, p3, p1);

                sumArea = s1 + s2 + s3;

                if (Math.Round(s1, 5) == 0 && Math.Round(sumArea, 5) == Math.Round(vArea, 5)
                    || Math.Round(s2, 5) == 0 && Math.Round(sumArea, 5) == Math.Round(vArea, 5)
                    || Math.Round(s3, 5) == 0 && Math.Round(sumArea, 5) == Math.Round(vArea, 5))
                {
                    //System.Windows.Forms.MessageBox.Show("Điểm nằm trên cạnh của đa giác", "Show");
                    return true;
                }
                else if (Math.Round(sumArea, 5) == Math.Round(vArea, 5))
                {
                    //System.Windows.Forms.MessageBox.Show("Điểm nằm trong đa giác", "Show");
                    return true;
                }
                else if (Math.Round(sumArea, 5) > Math.Round(vArea, 5))
                {
                    //System.Windows.Forms.MessageBox.Show("Điểm nằm ngoài đa giác", "Show");
                    return false;
                }
            }

            return false;
        }

        /// ================================================================================
        /// <summary>Calculator area triangle</summary>
        ///
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns>value area</returns>
        ///
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private static double CalAreaTri(XYZ p1, XYZ p2, XYZ p3)
        {
            double area = 0.0;

            area = 0.5 * (p1.X * (p2.Y - p3.Y) + p2.X * (p3.Y - p1.Y) + p3.X * (p1.Y - p2.Y));

            return Math.Abs(area);
        }

        /// ================================================================================
        /// <summary>Calculator area rectangle</summary>
        ///
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns>value area</returns>
        ///
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private static double CalAreaRec(XYZ p1, XYZ p2, XYZ p3, XYZ p4)
        {
            double area = 0.0;

            double b = p1.DistanceTo(p2);

            double h = p2.DistanceTo(p3);

            area = b * h;

            return Math.Abs(area);
        }

        /// ================================================================================
        /// <summary>Create tag by hand</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="listEleData">Dictionary input</param>
        /// <param name="outline">Outline</param>
        /// <param name="leftRight">Value check box left right</param>
        /// <param name="topBottom">Value check box top bottom</param>
        /// <param name="handlePresetTag">Option draw tag</param>
        /// <param name="distA">Distance a offset</param>
        /// <returns></returns>
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private bool CreateIndependentTagsByHand(Document doc, Collections.Generic.List<ElementData> listEleData, Outline outline,
            bool leftRight, bool topBottom, int tagLeader, int handlePresetTag, double distA)
        {
            bool ret = true;

            // List tag to need delete
            if (listEleData == null || listEleData.Count == 0)
                return false;

            // List tag need delete if user choose option handle preset tag = 1
            Collections.Generic.List<ElementId> listTagExits = null;

            // Delete tag exits of element
            if (handlePresetTag == 1)
                DeleteAllTag(doc, listEleData);

            // Get position tag element
            GetPosTag(doc, listEleData, outline, leftRight, topBottom, distA);

            foreach (var eledata in listEleData)
            {
                if (eledata == null)
                    continue;
                // Get element
                Element ele = eledata.ElementOrigin;
                if (ele == null)
                    continue;

                //Get symbol type of tag and then active it
                FamilySymbol tagType = eledata.TagSymbol;
                if (tagType == null)
                    continue;
                if (!tagType.IsActive)
                    tagType.Activate();

                // Preset tag processing
                listTagExits = _CmpElements.GetAllTagOfElement(doc, ele);

                //Set tag for element without tag
                if (handlePresetTag == 0 && listTagExits.Count > 0)
                    continue;

                // Get Reference
                Reference reference = new Reference(ele);
                if (reference == null)
                    continue;

                //Get position tag
                XYZ pos = GetCenterElement(doc, ele);

                // TagHeadPosition
                XYZ tagHead = eledata.CenterPoint;
                if (tagHead == null)
                    continue;

                // Create tag
                IndependentTag tag;
                if (tagLeader == 0)
                {
                    tag = IndependentTag.Create(doc, tagType.Id, doc.ActiveView.Id,
                                                           reference, true, TagOrientation.Horizontal, tagHead);
                    if (tag != null)
                    {
                        tag.LeaderEndCondition = LeaderEndCondition.Free;
                        tag.SetLeaderEnd(reference, pos);
                        tag.TagHeadPosition = tagHead;

                        bool hasElbow = tag.HasLeaderElbow(reference);
                        if (hasElbow)
                            tag.SetLeaderElbow(reference, pos);

                        tag.LeaderEndCondition = LeaderEndCondition.Attached;
                    }
                }
                else
                {
                    tag = IndependentTag.Create(doc, tagType.Id, doc.ActiveView.Id,
                                                            reference, false, TagOrientation.Horizontal, tagHead);
                }
                doc.Regenerate();
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>Delete tag </summary>
        ///
        /// <param name="doc"></param>
        /// <param name="listEleData"></param>
        ///
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private void DeleteAllTag(Document doc, Collections.Generic.List<ElementData> listEleData)
        {
            // List tag need delete if user choose option handle preset tag = 1
            Collections.Generic.List<ElementId> listTagDelete = new List<ElementId>();
            foreach (var eledata in listEleData)
            {
                if (eledata == null)
                    continue;
                // Get element
                Element ele = eledata.ElementOrigin;
                if (ele == null)
                    continue;

                // Preset tag processing
                var listelementIds = _CmpElements.GetAllTagOfElement(doc, ele);
                foreach (var tagid in listelementIds)
                {
                    listTagDelete.Add(tagid);
                }
            }
            if (listTagDelete != null && listTagDelete.Count > 0)
            {
                doc.Delete(listTagDelete);
                doc.Regenerate();
            }
        }

        /// ================================================================================
        /// <summary>Get position set tag</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="listElementDis">List element distance</param>
        /// <param name="outline">Outline</param>
        /// <param name="distA">distance A offset</param>
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private void GetPosTag(Document doc, List<ElementData> listElementDis, Outline outline, bool leftRight, bool topBottom, double distA)
        {
            if (outline == null || listElementDis == null || listElementDis.Count == 0)
                return;

            XYZ maxPointR = new XYZ(outline.MaximumPoint.X, outline.MaximumPoint.Y, 0);
            XYZ minPointL = new XYZ(outline.MinimumPoint.X, outline.MinimumPoint.Y, 0);
            XYZ maxPointL = new XYZ(minPointL.X, maxPointR.Y, 0);
            XYZ minPointR = new XYZ(maxPointR.X, minPointL.Y, 0);

            double h = maxPointR.DistanceTo(minPointR);
            double b = maxPointL.DistanceTo(maxPointR);

            XYZ vtMoveX = (maxPointR - maxPointL).Normalize();
            XYZ vtMoveY = (maxPointR - minPointR).Normalize();

            XYZ midPoint1 = GetPointOnVector(maxPointL, vtMoveX, b / 2);
            XYZ midPoint2 = GetPointOnVector(minPointR, vtMoveY, h / 2);
            XYZ midPoint3 = GetPointOnVector(minPointL, vtMoveX, b / 2);
            XYZ midPoint4 = GetPointOnVector(minPointL, vtMoveY, h / 2);

            XYZ pointMove = new XYZ();

            GetPostionELement(listElementDis, outline, leftRight, topBottom);

            if (leftRight && !topBottom)
            {
                // Line Left

                //region1
                GetPosTagByDirection(doc, listElementDis, distA, midPoint4, maxPointL, POST_ELEMENT.CS_REGION1, true, false, true);

                pointMove = GetPointOnVector(maxPointL, vtMoveY, 1);
                GetPosTagByDirection(doc, listElementDis, distA, maxPointL, pointMove, POST_ELEMENT.CS_REGION1, true, false, false);

                //region2
                GetPosTagByDirection(doc, listElementDis, distA, midPoint4, minPointL, POST_ELEMENT.CS_REGION2, true, true, true);

                pointMove = GetPointOnVector(minPointL, vtMoveY.Negate(), 1);
                GetPosTagByDirection(doc, listElementDis, distA, minPointL, pointMove, POST_ELEMENT.CS_REGION2, true, true, false);

                // Line Right
                //region3
                GetPosTagByDirection(doc, listElementDis, distA, midPoint2, maxPointR, POST_ELEMENT.CS_REGION3, true, true, true);

                pointMove = GetPointOnVector(maxPointR, vtMoveY, 1);
                GetPosTagByDirection(doc, listElementDis, distA, maxPointR, pointMove, POST_ELEMENT.CS_REGION3, true, true, false);

                //region4
                GetPosTagByDirection(doc, listElementDis, distA, midPoint2, minPointR, POST_ELEMENT.CS_REGION4, true, false, true);

                pointMove = GetPointOnVector(minPointR, vtMoveY.Negate(), 1);
                GetPosTagByDirection(doc, listElementDis, distA, minPointR, pointMove, POST_ELEMENT.CS_REGION4, true, false, false);
            }
            else if (topBottom && !leftRight)
            {
                // Line top

                //region1
                GetPosTagByDirection(doc, listElementDis, distA, midPoint1, maxPointL, POST_ELEMENT.CS_REGION1, false, true, true);

                pointMove = GetPointOnVector(maxPointL, vtMoveX.Negate(), 1);
                GetPosTagByDirection(doc, listElementDis, distA, maxPointL, pointMove, POST_ELEMENT.CS_REGION1, false, true, false);

                //region2
                GetPosTagByDirection(doc, listElementDis, distA, midPoint1, maxPointR, POST_ELEMENT.CS_REGION2, false, false, true);

                pointMove = GetPointOnVector(maxPointR, vtMoveX, 1);
                GetPosTagByDirection(doc, listElementDis, distA, maxPointR, pointMove, POST_ELEMENT.CS_REGION2, false, false, false);

                // Line bottom
                //region3
                GetPosTagByDirection(doc, listElementDis, distA, midPoint3, minPointL, POST_ELEMENT.CS_REGION3, false, false, true);

                pointMove = GetPointOnVector(minPointL, vtMoveX.Negate(), 1);
                GetPosTagByDirection(doc, listElementDis, distA, minPointL, pointMove, POST_ELEMENT.CS_REGION3, false, false, false);

                //region4
                GetPosTagByDirection(doc, listElementDis, distA, midPoint3, minPointR, POST_ELEMENT.CS_REGION4, false, true, true);

                pointMove = GetPointOnVector(minPointR, vtMoveX, 1);
                GetPosTagByDirection(doc, listElementDis, distA, minPointR, pointMove, POST_ELEMENT.CS_REGION4, false, true, false);
            }
            else
            {
                List<ElementData> listElment = new List<ElementData>();

                //region1
                listElment = listElementDis.Where(x => x.PostElement == POST_ELEMENT.CS_REGION1).ToList();
                GetPosTagByDirection(doc, listElment, distA, midPoint4, maxPointL, POST_ELEMENT.CS_REGION1, true, false, true);

                pointMove = GetPointOnVector(maxPointL, vtMoveY, 1);
                GetPosTagByDirection(doc, listElment, distA, maxPointL, pointMove, POST_ELEMENT.CS_REGION1, true, false, false);

                //region2
                listElment = listElementDis.Where(x => x.PostElement == POST_ELEMENT.CS_REGION2).ToList();
                GetPosTagByDirection(doc, listElment, distA, midPoint1, maxPointL, POST_ELEMENT.CS_REGION2, false, true, true);

                pointMove = GetPointOnVector(maxPointL, vtMoveX.Negate(), 1);
                GetPosTagByDirection(doc, listElment, distA, maxPointL, pointMove, POST_ELEMENT.CS_REGION2, false, true, false);

                //region3
                listElment = listElementDis.Where(x => x.PostElement == POST_ELEMENT.CS_REGION3).ToList();
                GetPosTagByDirection(doc, listElment, distA, midPoint1, maxPointR, POST_ELEMENT.CS_REGION3, false, false, true);

                pointMove = GetPointOnVector(maxPointR, vtMoveX, 1);
                GetPosTagByDirection(doc, listElment, distA, maxPointR, pointMove, POST_ELEMENT.CS_REGION3, false, false, false);

                //region4
                listElment = listElementDis.Where(x => x.PostElement == POST_ELEMENT.CS_REGION4).ToList();
                GetPosTagByDirection(doc, listElment, distA, midPoint2, maxPointR, POST_ELEMENT.CS_REGION4, true, true, true);

                pointMove = GetPointOnVector(maxPointR, vtMoveY, 1);
                GetPosTagByDirection(doc, listElment, distA, maxPointR, pointMove, POST_ELEMENT.CS_REGION4, true, true, false);

                //region5
                listElment = listElementDis.Where(x => x.PostElement == POST_ELEMENT.CS_REGION5).ToList();
                GetPosTagByDirection(doc, listElment, distA, midPoint2, minPointR, POST_ELEMENT.CS_REGION5, true, false, true);

                pointMove = GetPointOnVector(minPointR, vtMoveY.Negate(), 1);
                GetPosTagByDirection(doc, listElment, distA, minPointR, pointMove, POST_ELEMENT.CS_REGION5, true, false, false);

                //region6
                listElment = listElementDis.Where(x => x.PostElement == POST_ELEMENT.CS_REGION6).ToList();
                GetPosTagByDirection(doc, listElment, distA, midPoint3, minPointR, POST_ELEMENT.CS_REGION6, false, true, true);

                pointMove = GetPointOnVector(minPointR, vtMoveX, 1);
                GetPosTagByDirection(doc, listElment, distA, minPointR, pointMove, POST_ELEMENT.CS_REGION6, false, true, false);

                //region7
                listElment = listElementDis.Where(x => x.PostElement == POST_ELEMENT.CS_REGION7).ToList();
                GetPosTagByDirection(doc, listElment, distA, midPoint3, minPointL, POST_ELEMENT.CS_REGION7, false, false, true);

                pointMove = GetPointOnVector(minPointL, vtMoveX.Negate(), 1);
                GetPosTagByDirection(doc, listElment, distA, minPointL, pointMove, POST_ELEMENT.CS_REGION7, false, false, false);

                //region8
                listElment = listElementDis.Where(x => x.PostElement == POST_ELEMENT.CS_REGION8).ToList();
                GetPosTagByDirection(doc, listElment, distA, midPoint4, minPointL, POST_ELEMENT.CS_REGION8, true, true, true);

                pointMove = GetPointOnVector(minPointL, vtMoveY.Negate(), 1);
                GetPosTagByDirection(doc, listElment, distA, minPointL, pointMove, POST_ELEMENT.CS_REGION8, true, true, false);
            }
        }

        /// ================================================================================
        /// <summary>Get distance of element to line </summary>
        ///
        /// <param name="ele">Element</param>
        /// <param name="end">End line</param>
        /// <returns></returns>
        ///
        /// <history>2021/12/11 Created Applied Technology</history>
        /// ================================================================================

        private double GetDistance(ElementData ele, XYZ end)
        {
            if (end == null)
                return 0;

            XYZ pos = (ele.CenterPoint);

            return pos.DistanceTo(end);
        }

        /// ================================================================================
        /// <summary>Get all element of multi category</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="dicCategory">Dictionary category input</param>
        /// <returns></returns>
        /// <history>2021/12/11 Created Applied Technology</history>
        /// ================================================================================
        private List<Element> GetAllElement(Document doc, Dictionary<BuiltInCategory, FamilySymbol> dicCategory)
        {
            List<Element> retVal = new List<Element>();

            if (dicCategory == null || dicCategory.Count == 0)
                return retVal;

            // list BuiltInCategory
            List<BuiltInCategory> listBuilt = new List<BuiltInCategory>();
            foreach (var pair in dicCategory)
            {
                BuiltInCategory built = pair.Key;
                listBuilt.Add(built);
            }

            ElementMulticategoryFilter ruleCate = new ElementMulticategoryFilter(listBuilt);

            //get list element of category
            retVal = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .WherePasses(ruleCate)
                .ToElements().ToList();

            return retVal;
        }

        /// ================================================================================
        /// <summary>Check tag outside crop view</summary>
        ///
        /// <param name="doc">document</param>
        /// <returns></returns>
        ///
        /// <history>2021/12/11 Created Applied Technology</history>
        /// ================================================================================
        public bool CheckHasElementOutsideCropView(Document doc, out System.Text.StringBuilder strID)
        {
            // Get bounding box from crop view
            BoundingBoxXYZ boundingboxcropView = doc.ActiveView.CropBox;

            strID = new System.Text.StringBuilder();

            List<IndependentTag> independentTags = new FilteredElementCollector(doc, doc.ActiveView.Id)
               .OfClass(typeof(IndependentTag))
               .WhereElementIsNotElementType()
               .Cast<IndependentTag>().ToList();

            if (independentTags.Count == 0)
                return false;

            if (boundingboxcropView != null)
            {
                // Get max , min of bounding box
                XYZ min = boundingboxcropView.Transform.OfPoint(boundingboxcropView.Min);
                XYZ max = boundingboxcropView.Transform.OfPoint(boundingboxcropView.Max);

                double Ymax = max.Y;
                double Ymin = min.Y;
                double Xmax = max.X;
                double Xmin = min.X;

                List<IndependentTag> ListTagOutsideCropView = independentTags.FindAll(x => x.TagHeadPosition.X > Xmax || x.TagHeadPosition.X < Xmin || x.TagHeadPosition.Y > Ymax || x.TagHeadPosition.Y < Ymin);
                if (ListTagOutsideCropView != null && ListTagOutsideCropView.Count > 0)
                {
                    string listID = string.Empty;
                    foreach (var tag in ListTagOutsideCropView)
                    {
                        if (tag == null)
                            continue;
                        listID += ";" + tag.Id.ToString();
                    }
                    if (!string.IsNullOrEmpty(listID))
                    {
                        listID = listID.Remove(0, 1);
                        strID.AppendLine(_CmpAttribute.ResourceText("IDS_TXT_OUTSIDE_CROP_VIEW"));
                        strID.AppendLine(listID);
                    }
                    return true;
                }
            }

            return false;
        }

        /// ================================================================================
        /// <summary>Get point by line direction</summary>
        ///
        /// <param name="doc"></param>
        /// <param name="listElementDis"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        ///  <param name="posELement"></param>
        /// <param name="isLineLeftOrRight"></param>
        /// <param name="isNegate"></param>
        ///
        /// <history>2021/12/11 Created Applied Technology</history>
        /// ================================================================================
        private void GetPosTagByDirection(Document doc, List<ElementData> listElementDis, double distA,
            XYZ start, XYZ end, POST_ELEMENT posELement,
            bool isLineLeftOrRight, bool isNegate, bool isCheckIntersec)
        {
            // Max length
            double MaxLength = start.DistanceTo(end);

            // Width or height of the previous element
            double whtagStatus = 0;

            //Cumulative distance to check with the maximum length
            double distance = 0;

            // Width or height
            double spanOffsetMove = 0;
            double spanOffsetMoveDistanceLine = 0;

            // Vector move
            XYZ vectorMove = (end - start).Normalize();
            XYZ vectorOffset = vectorMove.CrossProduct(XYZ.BasisZ).Negate();

            if (isNegate)
                vectorOffset = vectorMove.CrossProduct(XYZ.BasisZ);

            if (isCheckIntersec)
            {
                // get distance of element to line
                foreach (var ele in listElementDis)
                {
                    if (ele.IsCheck || ele.PostElement != posELement)
                        continue;

                    // Get distance
                    double dist = GetDistance(ele, end);
                    ele.Distance = dist;
                }
            }

            // Sort list by distance
            listElementDis = listElementDis.OrderByDescending(x => x.Distance).ToList();

            // Point to offset and point origin
            XYZ poinBase = new XYZ();
            XYZ pointOffset = new XYZ();

            // Get element first
            ElementData eleFist = listElementDis.FirstOrDefault(x => x.IsCheck == false && x.PostElement == posELement) as ElementData;
            if (eleFist == null)
                return;

            // Get span offset and span move
            if (isLineLeftOrRight)
            {
                spanOffsetMove = eleFist.Height;
                spanOffsetMoveDistanceLine = eleFist.Width;
            }
            else
            {
                spanOffsetMove = eleFist.Width;
                spanOffsetMoveDistanceLine = eleFist.Height;
            }

            // Get center point set tag
            poinBase = GetPointOnVector(start, vectorOffset, spanOffsetMoveDistanceLine / 2 + distA);

            if (isCheckIntersec)
            {
                while (true)
                {
                    poinBase = GetPointOnVector(poinBase, vectorMove, spanOffsetMove / 2);

                    eleFist.CenterPoint = poinBase;

                    // Get bounding box of tag
                    BoundingBoxXYZ boundingBox = GetBoundingBox(poinBase, eleFist.Width, eleFist.Height);
                    eleFist.BoundingBoxEle = boundingBox;

                    // Check intersection
                    bool checkintersec = CheckIntersectionBoundingBox(doc, boundingBox, eleFist.ElementOrigin.Id);
                    eleFist.IsCheck = !checkintersec;

                    distance += spanOffsetMove;
                    whtagStatus = spanOffsetMove / 2;
                    if (eleFist.IsCheck)
                    {
                        break;
                    }

                    // Check length
                    if (distance > MaxLength)
                    {
                        eleFist.IsCheck = false;
                        break;
                    }
                }
            }
            else
            {
                poinBase = GetPointOnVector(poinBase, vectorMove, spanOffsetMove / 2 + distA);

                eleFist.CenterPoint = poinBase;

                // Get bounding box of tag
                BoundingBoxXYZ boundingBox = GetBoundingBox(poinBase, eleFist.Width, eleFist.Height);
                eleFist.BoundingBoxEle = boundingBox;

                // Check intersection
                bool checkintersec = CheckIntersectionBoundingBox(doc, boundingBox, eleFist.ElementOrigin.Id);
                eleFist.IsCheck = !checkintersec;

                whtagStatus = spanOffsetMove / 2;
            }

            if (isCheckIntersec)
            {
                if (!eleFist.IsCheck)
                    return;
            }

            for (int i = 1; i < listElementDis.Count; i++)
            {
                // get element
                ElementData eleDis = listElementDis[i] as ElementData;
                if (eleDis.IsCheck)
                    continue;

                if (eleDis.PostElement != posELement)
                    continue;

                // Get span offset and span move
                if (isLineLeftOrRight)
                {
                    spanOffsetMove = eleDis.Height;
                    spanOffsetMoveDistanceLine = eleDis.Width;
                }
                else
                {
                    spanOffsetMove = eleDis.Width;
                    spanOffsetMoveDistanceLine = eleDis.Height;
                }

                while (true)
                {
                    // Get point
                    pointOffset = GetPointOnVector(poinBase, vectorMove, whtagStatus + spanOffsetMove / 2);
                    eleDis.CenterPoint = pointOffset;

                    // Get bounding box of element
                    BoundingBoxXYZ boundingBox1 = GetBoundingBox(pointOffset, eleDis.Width, eleDis.Height);
                    eleDis.BoundingBoxEle = boundingBox1;

                    // check intersection of element
                    bool checkintersec1 = CheckIntersectionBoundingBox(doc, boundingBox1, eleDis.ElementOrigin.Id);
                    eleDis.IsCheck = !checkintersec1;
                    distance += spanOffsetMove;
                    whtagStatus = spanOffsetMove / 2;
                    poinBase = pointOffset;

                    if (eleDis.IsCheck)
                        break;
                    if (distance >= MaxLength)
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary>Get bounding box from point center</summary>
        ///
        /// <param name="pointCenter">Point center</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        /// <returns></returns>
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private BoundingBoxXYZ GetBoundingBox(XYZ pointCenter, double width, double height)
        {
            BoundingBoxXYZ retVal = new BoundingBoxXYZ();

            XYZ p1 = GetPointOnVector(pointCenter, _CmpElements.RvtDBDoc.ActiveView.UpDirection, height / 2);
            XYZ p2 = GetPointOnVector(pointCenter, _CmpElements.RvtDBDoc.ActiveView.UpDirection.Negate(), height / 2);

            XYZ maxBox = GetPointOnVector(p1, _CmpElements.RvtDBDoc.ActiveView.RightDirection, width / 2);
            XYZ minBox = GetPointOnVector(p2, _CmpElements.RvtDBDoc.ActiveView.RightDirection.Negate(), width / 2);

            retVal.Max = maxBox;
            retVal.Min = minBox;

            return retVal;
        }

        /// ================================================================================
        /// <summary>Get point center of element</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="ele">Element</param>
        /// <returns></returns>
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private XYZ GetCenterElement(Document doc, Element ele)
        {
            // Get bounding box
            var bb = ele.get_BoundingBox(doc.ActiveView);
            if (bb == null)
            {
                // Get location
                Location lc = ele.Location;
                if (lc == null)
                    return null;

                if (lc is LocationPoint)
                {
                    // Get location point
                    LocationPoint lcP = lc as LocationPoint;
                    if (lc == null)
                        return null;

                    // Point center
                    var centerP = lcP.Point;
                    return new XYZ(centerP.X, centerP.Y, 0);
                }
                else if (lc is LocationCurve)
                {
                    // Get location curve
                    LocationCurve lcCurve = lc as LocationCurve;
                    if (lcCurve == null)
                        return null;

                    // Point center
                    var centerP = (lcCurve.Curve.GetEndPoint(1) + lcCurve.Curve.GetEndPoint(0)) / 2;
                    return new XYZ(centerP.X, centerP.Y, 0);
                }
            }
            else
            {
                // Point center
                var centerP = (bb.Max + bb.Min) / 2;
                return new XYZ(centerP.X, centerP.Y, 0);
            }

            return null;
        }

        /// ================================================================================
        /// <summary>Get element inside of bounding box</summary>
        ///
        /// <param name="document">Document</param>
        /// <param name="myOutLn">OutLine</param>
        /// <param name="listEleId">List ELementId</param>
        /// <returns></returns>
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private static Collections.Generic.List<Element> GetElementInsideOrOutsideBoundingBox(Document document, Outline myOutLn, List<ElementId> listEleId)
        {
            List<Element> retVal = new List<Element>();

            // Use BoundingBoxIsInside filter to find elements with a bounding box that is contained(inside completely)
            // by the given Outline in the document.
            // Create a BoundingBoxIsInside filter for Outline
            BoundingBoxIsInsideFilter filter = new BoundingBoxIsInsideFilter(myOutLn);

            // Apply the filter to the elements in the active document
            // This filter excludes all objects derived from View and objects derived from ElementType
            FilteredElementCollector collector = new FilteredElementCollector(document, listEleId);
            retVal = collector.WherePasses(filter).ToElements().ToList();

            return retVal;
        }

        /// ================================================================================
        /// <summary>Get bounding box of tag</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="element">Element</param>
        /// <param name="tagSymbol">Family type of tag</param>
        /// <param name="aOffset">Distance offset</param>
        /// <param name="boundingboxTagOffset">Bounding box offset</param>
        /// <param name="boundingboxTag">Bounding box status</param>
        /// <returns></returns>
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private bool GetBounbdingboxOfTag(Document doc, Element element, ElementId tagSymbol, double aOffset, out BoundingBoxXYZ boundingboxTagOffset, out BoundingBoxXYZ boundingboxTag)
        {
            boundingboxTagOffset = new BoundingBoxXYZ();
            boundingboxTag = new BoundingBoxXYZ();

            if (element == null)
                return false;

            SubTransaction trans = new SubTransaction(doc);
            trans.Start();

            // Get reference
            Reference reference = new Reference(element);
            if (reference == null)
            {
                trans.RollBack();
                return false;
            }
            XYZ pos = GetCenterElement(doc, element);

            IndependentTag tag = IndependentTag.Create(doc, tagSymbol, doc.ActiveView.Id, reference, false, TagOrientation.Horizontal, pos);
            if (tag == null)
            {
                trans.RollBack();
                return false;
            }

            boundingboxTag = tag.get_BoundingBox(doc.ActiveView);
            if (boundingboxTag == null)
            {
                trans.RollBack();
                return false;
            }

            trans.RollBack();

            // get new point

            XYZ newMax = GetPointOnVector(boundingboxTag.Max, doc.ActiveView.UpDirection, aOffset);
            newMax = GetPointOnVector(newMax, doc.ActiveView.RightDirection, aOffset);

            XYZ newMin = GetPointOnVector(boundingboxTag.Min, doc.ActiveView.UpDirection.Negate(), aOffset);
            newMin = GetPointOnVector(newMin, doc.ActiveView.RightDirection.Negate(), aOffset);

            // Bounding box offset
            boundingboxTagOffset.Max = newMax;
            boundingboxTagOffset.Min = newMin;

            return true;
        }

        /// ================================================================================
        /// <summary>Check element hidden in view or not</summary>
        ///
        /// <param name="e">Element</param>
        /// <param name="v">View</param>
        /// <returns></returns>
        ///
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private bool IsElementHiddenInView(
           Element e,
           View v)
        {
            if (v.CropBoxActive)
            {
                BoundingBoxXYZ viewBox = v.CropBox;
                BoundingBoxXYZ elBox = e.get_BoundingBox(v);
                if (elBox == null)
                    return true;

                Transform transInv = v.CropBox.Transform.Inverse;

                elBox.Max = transInv.OfPoint(elBox.Max);
                elBox.Min = transInv.OfPoint(elBox.Min);

                // The transform above might switch
                // max and min values.

                if (elBox.Min.X > elBox.Max.X)
                {
                    XYZ tmpP = elBox.Min;
                    elBox.Min = new XYZ(elBox.Max.X, elBox.Min.Y, 0);
                    elBox.Max = new XYZ(tmpP.X, elBox.Max.Y, 0);
                }

                if (elBox.Min.Y > elBox.Max.Y)
                {
                    XYZ tmpP = elBox.Min;
                    elBox.Min = new XYZ(elBox.Min.X, elBox.Max.Y, 0);
                    elBox.Max = new XYZ(tmpP.X, elBox.Min.Y, 0);
                }

                if (elBox.Min.X > viewBox.Max.X
                  || elBox.Max.X < viewBox.Min.X
                  || elBox.Min.Y > viewBox.Max.Y
                  || elBox.Max.Y < viewBox.Min.Y)
                {
                    return true;
                }
                else
                {
                    BoundingBoxXYZ inside = new BoundingBoxXYZ();

                    double x, y;

                    x = elBox.Max.X;

                    if (elBox.Max.X > viewBox.Max.X)
                        x = viewBox.Max.X;

                    y = elBox.Max.Y;

                    if (elBox.Max.Y > viewBox.Max.Y)
                        y = viewBox.Max.Y;

                    inside.Max = new XYZ(x, y, 0);

                    x = elBox.Min.X;

                    if (elBox.Min.X < viewBox.Min.X)
                        x = viewBox.Min.X;

                    y = elBox.Min.Y;

                    if (elBox.Min.Y < viewBox.Min.Y)
                        y = viewBox.Min.Y;

                    inside.Min = new XYZ(x, y, 0);

                    double eBBArea = (elBox.Max.X - elBox.Min.X)
                      * (elBox.Max.Y - elBox.Min.Y);

                    double einsideArea =
                      (inside.Max.X - inside.Min.X)
                      * (inside.Max.Y - inside.Min.Y);

                    double factor = einsideArea / eBBArea;

                    if (factor < 0.25)
                        return true;
                }
            }

            bool hidden = e.IsHidden(v);

            if (!hidden)
            {
                Category cat = e.Category;

                while (null != cat && !hidden)
                {
                    hidden = !cat.get_Visible(v);
                    cat = cat.Parent;
                }
            }
            return hidden;
        }

        /// ================================================================================
        /// <summary>Check intersection of 2 bounding box</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="boundingBoxCheck">Bounding box need check</param>
        /// <param name="elesetTag">Element need set tag</param>
        /// <returns></returns>
        ///
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private bool CheckIntersectionBoundingBox(Document doc, BoundingBoxXYZ boundingBoxCheck, ElementId elesetTag)
        {
            // List of category exclude
            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_Grids);
            builtInCats.Add(BuiltInCategory.OST_Rooms);
            builtInCats.Add(BuiltInCategory.OST_Areas);
            builtInCats.Add(BuiltInCategory.OST_MEPSpaces);

            // Create outline
            Outline outline = new Outline(new XYZ(boundingBoxCheck.Min.X, boundingBoxCheck.Min.Y, -distance), new XYZ(boundingBoxCheck.Max.X, boundingBoxCheck.Max.Y, distance));

            //Get element intersection
            var elements = GetElementInside(doc, outline);

            // Check category
            foreach (var element in elements)
            {
                if (element == null)
                    continue;

                if (!IsElementVisibleInView(doc.ActiveView, element))
                    continue;

                if (IsElementHiddenInView(element, doc.ActiveView) == true)
                    continue;

                if (element.Id.ToString() == elesetTag.ToString())
                    continue;

                if (element.Category == null)
                    return true;

                if (builtInCats.Any(x => ((int)x).ToString() == element.Category.Id.ToString()) == false)
                    return true;
            }

            return false;
        }

        /// ================================================================================
        /// <summary>GetElementInside</summary>
        ///
        /// <param name="document">Document</param>
        /// <param name="outline">ouline input</param>
        /// <returns>List Element</returns>
        ///
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private List<Element> GetElementInside(Document document, Outline outline)
        {
            List<Element> lstElementInside = new List<Element>();

            // Find all element inside outline
            var lstAllElement = GetAllElementInsideOrCutOutLine(document, outline, document.ActiveView);
            lstElementInside.AddRange(lstAllElement);

            // Find all tag
            var lstTag = CheckBoundingboxInsideOrCutTag(document, outline, document.ActiveView);
            lstElementInside.AddRange(lstTag);

            // Remove duplicate
            lstElementInside = lstElementInside.GroupBy(x => x.Id).Select(y => y.FirstOrDefault()).ToList();

            return lstElementInside;
        }

        /// ================================================================================
        /// <summary>Get All Element Inside Or Cut OutLine</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="outline">Outline input</param>
        /// <param name="activeView">Active view</param>
        /// <returns></returns>
        ///
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private List<Element> GetAllElementInsideOrCutOutLine(Document doc, Outline outline, View activeView)
        {
            List<Element> retVal = new List<Element>();

            // Filter inside bounding box
            BoundingBoxIsInsideFilter boundingBoxInsideFilter = new BoundingBoxIsInsideFilter(outline);
            FilteredElementCollector filterInside = new FilteredElementCollector(doc, activeView.Id).WhereElementIsNotElementType();
            retVal.AddRange(filterInside.WherePasses(boundingBoxInsideFilter).ToElements());

            // Filter cut bounding box
            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(outline);
            filterInside = new FilteredElementCollector(doc, activeView.Id).WhereElementIsNotElementType();
            retVal.AddRange(filterInside.WherePasses(boundingBoxIntersectsFilter).ToElements());

            return retVal;
        }

        /// ================================================================================
        /// <summary>CheckBoundingboxInsideOrCutTag</summary>
        ///
        /// <param name="doc"></param>
        /// <param name="outline"></param>
        /// <param name="activeView"></param>
        /// <returns></returns>
        ///
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private List<Element> CheckBoundingboxInsideOrCutTag(Document doc, Outline outline, View activeView)
        {
            List<Element> retVal = new List<Element>();

            // Get all tag
            var lstAllTag = new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType().OfClass(typeof(IndependentTag)).ToList();

            foreach (var ele in lstAllTag)
            {
                IndependentTag tag = ele as IndependentTag;

                var bbBox = tag.get_BoundingBox(activeView);
                if (bbBox == null)
                    continue;

                var insideMin = outline.Contains(bbBox.Min, Tolerance);
                var insideMax = outline.Contains(bbBox.Max, Tolerance);

                if (insideMin && insideMax)
                    retVal.Add(tag);
                else
                {
                    Outline outlineTemp = new Outline(bbBox.Min, bbBox.Max);
                    if (outlineTemp == null)
                        continue;

                    if (outline.Intersects(outlineTemp, Tolerance))
                        retVal.Add(tag);
                }
            }
            return retVal;
        }

        /// ================================================================================
        /// <summary>Check element visble in view or not</summary>
        ///
        /// <param name="view"></param>
        /// <param name="el"></param>
        /// <returns></returns>
        /// <returns></returns>
        ///
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        public static bool IsElementVisibleInView(View view, Element el)
        {
            if (view == null || el == null)
                return false;

            // Obtain the element's document
            Document doc = el.Document;

            ElementId elId = el.Id;

            // Create a FilterRule that searches for an element matching the given Id
            FilterRule idRule = ParameterFilterRuleFactory.CreateEqualsRule(new ElementId(BuiltInParameter.ID_PARAM), elId);
            var idFilter = new ElementParameterFilter(idRule);

            // Use an ElementCategoryFilter to speed up the search, as ElementParameterFilter is a slow filter
            Category cat = el.Category;
            var catFilter = new ElementCategoryFilter(cat.Id);

            // Use the constructor of FilteredElementCollector that accepts a view id as a parameter to only search that view
            // Also use the WhereElementIsNotElementType filter to eliminate element types
            FilteredElementCollector collector =
                new FilteredElementCollector(doc, view.Id).WhereElementIsNotElementType().WherePasses(catFilter).WherePasses(idFilter);

            // If the collector contains any items, then we know that the element is visible in the given view
            return collector.Any();
        }

        /// ================================================================================
        /// <summary>Get bounding box new offset</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="element">element</param>
        /// <param name="boundingboxTag">bounding box tag</param>
        /// <param name="aOffset">a offset</param>
        /// <param name="bOffset">b offset</param>
        /// <param name="lstBboxOffset"> list bounding box offset</param>
        /// <returns></returns>
        ///
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private bool GetBoundingboxWithNewOffset(Document doc, Element element, BoundingBoxXYZ boundingboxTag,
            double aOffset, double bOffset, out List<BoundingBoxXYZ> lstBboxOffset)
        {
            lstBboxOffset = new List<BoundingBoxXYZ>();

            if (GetWidthAndHeightBoundingBox(boundingboxTag, out double widthTag, out double heightTag) == false)
                return false;

            var bbBoxElementCurrent = element.get_BoundingBox(doc.ActiveView);
            if (bbBoxElementCurrent == null)
                return false;

            // Get width and height bounding box of element current
            if (GetWidthAndHeightBoundingBox(bbBoxElementCurrent, out double widthElement, out double heightElement) == false)
                return false;

            XYZ rightDirOfView = doc.ActiveView.RightDirection;

            // Get centerP of bounding box
            var centerP = (boundingboxTag.Max + boundingboxTag.Min) / 2;

            // Left
            var offsetVal = (widthTag / 2) + (widthElement / 2) + bOffset;
            var newBBoxLeft = MoveBoundingBox(boundingboxTag, offsetVal, rightDirOfView.Negate());

            // Top
            offsetVal = (heightTag / 2) + (heightElement / 2) + bOffset;
            var newBBoxTop = MoveBoundingBox(boundingboxTag, offsetVal, rightDirOfView.CrossProduct(XYZ.BasisZ).Negate());

            // right
            offsetVal = (widthTag / 2) + (widthElement / 2) + bOffset;
            var newBBoxRight = MoveBoundingBox(boundingboxTag, offsetVal, rightDirOfView);

            // Down
            offsetVal = (heightTag / 2) + (heightElement / 2) + bOffset;
            var newBBoxDown = MoveBoundingBox(boundingboxTag, offsetVal, rightDirOfView.CrossProduct(XYZ.BasisZ));

            lstBboxOffset.Add(newBBoxLeft);
            lstBboxOffset.Add(newBBoxTop);
            lstBboxOffset.Add(newBBoxRight);
            lstBboxOffset.Add(newBBoxDown);

            return true;
        }

        /// ================================================================================
        /// <summary>Move bounding box</summary>
        ///
        /// <param name="boundingboxCurent">boundingboxCurent</param>
        /// <param name="offset"> distance offset</param>
        /// <param name="vecMove"> vector move</param>
        /// <returns></returns>
        ///
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private BoundingBoxXYZ MoveBoundingBox(BoundingBoxXYZ boundingboxCurent, double offset, XYZ vecMove)
        {
            if (boundingboxCurent == null)
                return null;

            var newMaxBbox = GetPointOnVector(boundingboxCurent.Max, vecMove, offset);
            var newMinBbox = GetPointOnVector(boundingboxCurent.Min, vecMove, offset);

            // Bounding box offset
            BoundingBoxXYZ newBoundingBox = new BoundingBoxXYZ();
            newBoundingBox.Min = newMinBbox;
            newBoundingBox.Max = newMaxBbox;

            return newBoundingBox;
        }

        /// ================================================================================
        /// <summary>Get point on vector</summary>
        ///
        /// <param name="pointInsert">Point origin</param>
        /// <param name="vectorDir">direction</param>
        /// <param name="dDistance">distance</param>
        /// <returns></returns>
        ///
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private XYZ GetPointOnVector(XYZ pointInsert, XYZ vectorDir, double dDistance)
        {
            return (pointInsert + (vectorDir.Normalize()) * dDistance);
        }

        /// ================================================================================
        /// <summary>Get width or height of bounding box</summary>
        ///
        /// <param name="bbox">bounding box need</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <returns></returns>
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        private bool GetWidthAndHeightBoundingBox(BoundingBoxXYZ bbox, out double width, out double height)
        {
            width = 0;
            height = 0;
            if (bbox == null)
                return false;
            // take 4 points of bounding box
            var minP = new XYZ(bbox.Min.X, bbox.Min.Y, 0);
            var maxP = new XYZ(bbox.Max.X, bbox.Max.Y, 0);
            var leftP = new XYZ(minP.X, maxP.Y, 0);
            var rightP = new XYZ(maxP.X, minP.Y, 0);

            // get width and height of bounding box
            width = minP.DistanceTo(rightP);
            height = minP.DistanceTo(leftP);

            return true;
        }

        /// ================================================================================
        /// <summary>Get value setting A and B</summary>
        ///
        /// <param name="filePath">file path settings.txt</param>
        /// <param name="distA">distance a</param>
        /// <param name="distB">distance b</param>
        ///
        /// <history><p>2021/12/10 Created  Applied Technology </p></history>
        /// ================================================================================
        public void GetSettingValue(string filePath, out double distA, out double distB)
        {
            // default a and b
            distA = 5;
            distB = 30;

            // get file path
            if (!System.IO.File.Exists(filePath))
            {
                filePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                    + "\\" + _CmpAttribute.ResourceText("IDS_FILE_ITEMS");
            }

            // check empty file path
            if (string.IsNullOrEmpty(filePath))
                return;

            System.Text.Encoding.RegisterProvider( CodePagesEncodingProvider.Instance );
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");

            string[] lstValue = new[] { "5", "30", "10", "10" } ;   //読み込み失敗時の設定初期値
            // read value from file settings
            try {
                lstValue = System.IO.File.ReadAllLines(filePath, enc);
            }
            catch ( Exception e ) {
                //Console.WriteLine( e ) ;
            }

            
            
            if (lstValue.Length == 1)
            {
                // get distance a
                if (double.TryParse(lstValue[0], out double parsedA))
                    distA = System.Math.Abs(parsedA);
            }
            else if (lstValue.Length > 1)
            {
                // get distance a
                if (double.TryParse(lstValue[0], out double parsedA))
                    distA = System.Math.Abs(parsedA);

                // get distance b
                if (double.TryParse(lstValue[1], out double parsedB))
                    distB = System.Math.Abs(parsedB);
            }
        }

        #endregion Member Functions
    }
}