using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using RvtExtApp   = ADSK.Ext.Fukashi;

namespace ADSK.Ext.Fukashi.Face.Components
{
    /// ================================================================================
    /// <summary>図形</summary>
    /// ================================================================================
    public partial class Geometry
    {
        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>面カーブ取征E/summary>
        /// 
        /// <param name="plnFace">平面</param>
        /// 
        /// <history>2016/12/07 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Revit.DB.Curve> GetFaceCurves(Revit.DB.PlanarFace plnFace)
        {
            // 戻り値
            Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();

            // ループ線�E
            Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = plnFace.GetEdgesAsCurveLoops();

            // 褁E��形状
            if (crvLoops.Count > 1)
            {
                return ret;
            }

            // 曲線を含むぁE
            bool isCyclic = false;

            Collections.Generic.IList<Revit.DB.Curve> curves = new Collections.Generic.List<Revit.DB.Curve>();
            foreach (Revit.DB.CurveLoop crvLoop in crvLoops)
            {
                foreach (Revit.DB.Curve curve in crvLoop)
                {
                    curves.Add(curve);

                    isCyclic = curve.IsCyclic;
                }
            }

            if (isCyclic)
            {
                return ret;
            }

            ret = GetCurves(curves);


            //// 調整
            //    curves = OptimizeLineVertexNoConvLine(curves);

            //Revit.DB.BoundingBoxUV bbUV = plnFace.GetBoundingBox();
            //Revit.DB.UV uv = bbUV.Min;
            //Revit.DB.XYZ loc = plnFace.Evaluate(uv);

            //Revit.DB.Curve leftbtm = null;
            //foreach (Revit.DB.Curve c in curves)
            //{
            //  if (Distance(loc, c.GetEndPoint(0)) < Approx0Len)
            //  {
            //    leftbtm = c;
            //  }
            //}

            //if (leftbtm == null)
            //{
            //  leftbtm = GetLeftBottomCurve(curves, 0);
            //}

            //// 連続するカーチE
            //GetContinuousCurves(curves, leftbtm, ref ret);

            return ret;
        }
        /// ================================================================================
        /// <summary>左下カーブ取征E/summary>
        /// 
        /// <param name="curves">カーチE/param>
        /// 
        /// <history>2017/02/06 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Revit.DB.Curve> GetCurves(Collections.Generic.IList<Revit.DB.Curve> curves)
        {
            // 戻り値
            Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();


            // 調整
            curves = OptimizeLineVertexNoConvLine(curves);

            Revit.DB.XYZ minP = GetMinPoint(curves);

            Revit.DB.Curve leftbtm = null;
            double dMinLen = double.MaxValue;

            for (int i = 0; i < curves.Count; ++i)
            {
                Revit.DB.XYZ p1 = curves[i].GetEndPoint(0);

                if (System.Math.Abs(minP.X - p1.X) < Approx0Len &&
                   System.Math.Abs(minP.Y - p1.Y) < Approx0Len &&
                   System.Math.Abs(minP.Z - p1.Z) < Approx0Len)
                {
                    //同一点あり　決宁E
                    leftbtm = curves[i];
                    break;
                }
                double len = GetPPLen(minP, p1);
                if (len < dMinLen)
                {
                    dMinLen = len;
                    //候裁E
                    leftbtm = curves[i];
                }
            }

            // 連続するカーチE
            GetContinuousCurves(curves, leftbtm, ref ret);

            return ret;
        }
        /// ================================================================================
        /// <summary>2点間距離</summary>
        /// 
        /// <param name="p1">点</param>
        /// <param name="p2">点</param>
        /// 
        /// <history>2017/02/06 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public double GetPPLen(Revit.DB.XYZ p1, Revit.DB.XYZ p2)
        {
            double x = p2.X - p1.X;
            double y = p2.Y - p1.Y;
            double z = p2.Z - p1.Z;
            return (System.Math.Sqrt(x * x + y * y + z * z));

        }

        /// ================================================================================
        /// <summary>面形状判宁E/summary>
        /// 
        /// <param name="plnFace" >平面</param>
        /// 
        /// <returns><p>1 = 三角形</p>
        ///           <p>2 = 台形</p>
        ///           <p>3 = 正方形</p>
        ///           <p>4 = 長方形</p>
        ///           <p>5 = ひし形</p>
        ///           <p>6 = 平行四辺形</p>
        ///           <p>7 = L字形</p>
        ///           <p>8 = T字形</p>
        ///           <p>9 = 凹字形</p>
        ///           <p>10= そ�E仁E/p></returns>
        /// 
        /// <history><p>2016/11/24 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2016/12/06 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        int FaceGeometryMode(Revit.DB.PlanarFace plnFace, Collections.Generic.IList<Revit.DB.Curve> curvesA)
        {
            // 戻り値
            int ret = 0;

            Collections.Generic.IList<Revit.DB.Curve> curves = new Collections.Generic.List<Revit.DB.Curve>();
            if (curvesA.Count == 0)
            {
                // ループ線�E
                Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = plnFace.GetEdgesAsCurveLoops();

                // 褁E��形状
                if (crvLoops.Count > 1)
                {
                    // そ�E仁E
                    ret = 10;
                    return ret;
                }

                // 曲線を含むぁE
                bool isCyclic = false;

                foreach (Revit.DB.CurveLoop crvLoop in crvLoops)
                {
                    foreach (Revit.DB.Curve curve in crvLoop)
                    {
                        curves.Add(curve);

                        isCyclic = curve.IsCyclic;
                    }
                }

                if (isCyclic)
                {
                    // そ�E仁E
                    ret = 10;
                    return ret;
                }
            }
            else
            {
                curves = curvesA;
            }

            // 調整
            curves = OptimizeLineVertexNoConvLine(curves);

            // 三角形
            if (curves.Count == 3)
            {
                ret = 1;
            }
            // 四角形
            else if (curves.Count == 4)
            {
                #region 四角形

                // 対角緁E
                Collections.Generic.IList<Revit.DB.Line> diagonals = GetDiagonal(curves);

                if (diagonals.Count == 2)
                {
                    Revit.DB.Line line1 = diagonals[0];
                    Revit.DB.Line line2 = diagonals[1];

                    // 対角線�E交点
                    Collections.Generic.IList<Revit.DB.XYZ> intersects = new Collections.Generic.List<Revit.DB.XYZ>();
                    IntersecCurve(line1, line2, ref intersects);

                    if (intersects.Count == 1)
                    {
                        Revit.DB.XYZ p0 = line1.GetEndPoint(0);
                        Revit.DB.XYZ p1 = line1.GetEndPoint(1);
                        Revit.DB.XYZ p2 = line2.GetEndPoint(0);
                        Revit.DB.XYZ p3 = line2.GetEndPoint(1);

                        Revit.DB.XYZ intersect = intersects[0];

                        // お互いに2等�E
                        if (System.Math.Abs(Distance(p0, intersect) - Distance(p1, intersect)) < Approx0Len &&
                            System.Math.Abs(Distance(p2, intersect) - Distance(p3, intersect)) < Approx0Len)
                        {
                            // 対角線同士の長さが等しぁE
                            if (System.Math.Abs(line1.Length - line2.Length) < Approx0Len)
                            {
                                // 交点からのそれぞれの対角線始点への単位�Eクトル
                                Revit.DB.XYZ uv1 = UnitVector(intersect, p0);
                                Revit.DB.XYZ uv2 = UnitVector(intersect, p2);

                                double cosSita = (uv1.X * uv2.X + uv1.Y * uv2.Y + uv1.Z * uv2.Z) / (System.Math.Sqrt(uv1.X * uv1.X + uv1.Y * uv1.Y + uv1.Z * uv1.Z) * System.Math.Sqrt(uv2.X * uv2.X + uv2.Y * uv2.Y + uv2.Z * uv2.Z));

                                // ラジアン角度
                                double rad = System.Math.Acos(cosSita);

                                // 直角に交差
                                if (System.Math.Abs(System.Math.Abs(rad) - System.Math.PI / 2) < Approx0Ang)
                                {
                                    // 正方形
                                    ret = 3;
                                }
                                // 直角に交差ではなぁE
                                else
                                {
                                    // 長方形
                                    ret = 4;
                                }
                            }
                            else
                            {
                                // 交点からのそれぞれの対角線始点への単位�Eクトル
                                Revit.DB.XYZ uv1 = UnitVector(intersect, p0);
                                Revit.DB.XYZ uv2 = UnitVector(intersect, p2);

                                double cosSita = (uv1.X * uv2.X + uv1.Y * uv2.Y + uv1.Z * uv2.Z) / (System.Math.Sqrt(uv1.X * uv1.X + uv1.Y * uv1.Y + uv1.Z * uv1.Z) * System.Math.Sqrt(uv2.X * uv2.X + uv2.Y * uv2.Y + uv2.Z * uv2.Z));

                                // ラジアン角度
                                double rad = System.Math.Acos(cosSita);

                                // 直角に交差
                                if (System.Math.Abs(System.Math.Abs(rad) - System.Math.PI / 2) < Approx0Ang)
                                {
                                    // ひし形
                                    ret = 5;
                                }
                                // 直角に交差ではなぁE
                                else
                                {
                                    // 平行四辺形
                                    ret = 6;
                                }
                            }
                        }
                        // 2等�EではなぁE
                        else
                        {
                            // どちらかの対辺が平衁E
                            Revit.DB.Line l0 = curves[0] as Revit.DB.Line;
                            Revit.DB.Line l1 = curves[1] as Revit.DB.Line;
                            Revit.DB.Line l2 = curves[2] as Revit.DB.Line;
                            Revit.DB.Line l3 = curves[3] as Revit.DB.Line;

                            if (IsParallelLine(l0, l2) ||
                                IsParallelLine(l1, l3))
                            {
                                // 台形
                                ret = 2;
                            }
                        }
                    }
                }

                #endregion
            }
            // 六角形
            else if (curves.Count == 6)
            {
                #region 六角形

                bool isRect = true;

                for (int i = 0; i < curves.Count; ++i)
                {
                    Revit.DB.Curve curve1 = curves[i];
                    Revit.DB.Curve curve2 = null;

                    if (i == curves.Count - 1)
                    {
                        curve2 = curves[0];
                    }
                    else
                    {
                        curve2 = curves[i + 1];
                    }

                    Revit.DB.XYZ p0 = curve1.GetEndPoint(0);
                    Revit.DB.XYZ p1 = curve1.GetEndPoint(1);
                    Revit.DB.XYZ p2 = curve2.GetEndPoint(0);
                    Revit.DB.XYZ p3 = curve2.GetEndPoint(1);

                    Revit.DB.XYZ uv1 = UnitVector(p0, p1);
                    Revit.DB.XYZ uv2 = UnitVector(p2, p3);

                    double cosSita = CosSita(uv1, uv2);

                    // ラジアン角度
                    double rad = System.Math.Acos(cosSita);

                    // 頂点が直见E
                    if (System.Math.Abs(System.Math.Abs(rad) - System.Math.PI / 2) < Approx0Ang)
                    {
                        isRect = true;
                    }
                    else
                    {
                        isRect = false;
                        break;
                    }
                }

                // すべての頂点が直见E
                if (isRect)
                {
                    // L字形
                    ret = 7;
                }

                #endregion
            }
            // 八角形
            else if (curves.Count == 8)
            {
                #region 八角形

                bool isRect = true;

                for (int i = 0; i < curves.Count; ++i)
                {
                    Revit.DB.Curve curve1 = curves[i];
                    Revit.DB.Curve curve2 = null;

                    if (i == curves.Count - 1)
                    {
                        curve2 = curves[0];
                    }
                    else
                    {
                        curve2 = curves[i + 1];
                    }

                    Revit.DB.XYZ p0 = curve1.GetEndPoint(0);
                    Revit.DB.XYZ p1 = curve1.GetEndPoint(1);
                    Revit.DB.XYZ p2 = curve2.GetEndPoint(0);
                    Revit.DB.XYZ p3 = curve2.GetEndPoint(1);

                    Revit.DB.XYZ uv1 = UnitVector(p0, p1);
                    Revit.DB.XYZ uv2 = UnitVector(p2, p3);

                    double cosSita = CosSita(uv1, uv2);

                    // ラジアン角度
                    double rad = System.Math.Acos(cosSita);

                    // 頂点が直见E
                    if (System.Math.Abs(System.Math.Abs(rad) - System.Math.PI / 2) < Approx0Ang)
                    {
                        isRect = true;
                    }
                    else
                    {
                        isRect = false;
                        break;
                    }
                }

                // すべての頂点が直见E
                if (isRect)
                {
                    // 凸凹並び頁E正負で判宁E
                    Collections.Generic.IList<Collections.Generic.IList<int>> unevenness = new Collections.Generic.List<Collections.Generic.IList<int>>();

                    for (int i = 0; i < curves.Count; ++i)
                    {
                        Revit.DB.Curve curve1 = curves[i];
                        Revit.DB.Curve curve2 = null;

                        if (i == curves.Count - 1)
                        {
                            curve2 = curves[0];
                        }
                        else
                        {
                            curve2 = curves[i + 1];
                        }

                        Revit.DB.XYZ p0 = curve1.GetEndPoint(0);
                        Revit.DB.XYZ p1 = curve1.GetEndPoint(1);
                        Revit.DB.XYZ p2 = curve2.GetEndPoint(0);
                        Revit.DB.XYZ p3 = curve2.GetEndPoint(1);

                        Revit.DB.XYZ uv1 = UnitVector(p0, p1);
                        Revit.DB.XYZ uv2 = UnitVector(p2, p3);

                        Revit.DB.XYZ cross = Gaiseki(uv1, uv2);

                        // 符号 正めE、負めE
                        Collections.Generic.IList<int> ary = new Collections.Generic.List<int>();
                        ary.Add(cross.X > 0 ? 1 : 0);
                        ary.Add(cross.Y > 0 ? 1 : 0);
                        ary.Add(cross.Z > 0 ? 1 : 0);

                        unevenness.Add(ary);
                    }

                    // 最初�E頂点
                    Collections.Generic.IList<int> ary0 = unevenness[0];

                    // 最初�E頂点と同一の凸凹ぁE
                    Collections.Generic.IList<int> sameAry = new Collections.Generic.List<int>();

                    int sameCount = 0;
                    int diffeCount = 0;

                    foreach (Collections.Generic.IList<int> ary1 in unevenness)
                    {
                        if (ary0[0] == ary1[0] &&
                            ary0[1] == ary1[1] &&
                            ary0[2] == ary1[2])
                        {
                            sameAry.Add(1);
                            sameCount += 1;
                        }
                        else
                        {
                            sameAry.Add(0);
                            diffeCount += 1;
                        }
                    }

                    string order = "";
                    foreach (int same in sameAry)
                    {
                        order += same.ToString();
                    }
                    foreach (int same in sameAry)
                    {
                        order += same.ToString();
                    }

                    // T孁E凸垁Eは凸の頂点ぁE個、�Eの頂点ぁE個で
                    // 頂点の並び頁E��凸凸凸凸凹凸凸凹
                    if (sameCount == 6 &&
                        diffeCount == 2)
                    {
                        // 多い方めE、少なぁE��めE
                        if (order.Contains("11110110"))
                        {
                            // T字形
                            ret = 8;
                        }

                        //凹垁E
                        if (order.Contains("11111100"))
                        {
                            // 凹字形
                            ret = 9;
                        }

                    }
                    else if (sameCount == 2 &&
                             diffeCount == 6)
                    {
                        // 多い方めE、少なぁE��めE
                        if (order.Contains("00001001"))
                        {
                            // T字形
                            ret = 8;
                        }

                        //凹垁E
                        if (order.Contains("00000011"))
                        {
                            // 凹字形
                            ret = 9;
                        }
                    }
                }

                #endregion
            }

            // 該当なぁE
            if (ret == 0)
            {
                // そ�E仁E
                ret = 10;
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>面形状判宁E/summary>
        /// 
        /// <param name="plnFace"   >平面</param>
        /// <param name="pickPos"   >選択点</param>
        /// <param name="polyCurves">篁E��形状</param>
        /// 
        /// <returns><p>1 = 三角形</p>
        ///           <p>2 = 台形</p>
        ///           <p>3 = 正方形</p>
        ///           <p>4 = 長方形</p>
        ///           <p>5 = ひし形</p>
        ///           <p>6 = 平行四辺形</p>
        ///           <p>7 = L字形</p>
        ///           <p>8 = T字形</p>
        ///           <p>9 = そ�E仁E/p></returns>
        /// 
        /// <history>2016/12/13 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        //public
        //int FaceGeometryMode(Revit.DB.PlanarFace plnFace,
        //                     Revit.DB.XYZ pickPos,
        //                     ref Collections.Generic.IList<Revit.DB.Curve> polyCurves)
        //{
        //  // 戻り値
        //  int ret = 0;

        //  // ループ線�E
        //  Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = plnFace.GetEdgesAsCurveLoops();

        //  // 曲線を含むぁE
        //  bool isCyclic = false;

        //  Revit.DB.XYZ normal = plnFace.FaceNormal;

        //  Collections.Generic.IList<Revit.DB.Curve> curves = new Collections.Generic.List<Revit.DB.Curve>();
        //  foreach (Revit.DB.CurveLoop crvLoop in crvLoops)
        //  {
        //    Collections.Generic.IList<Revit.DB.Curve> _curves = new Collections.Generic.List<Revit.DB.Curve>();
        //    foreach (Revit.DB.Curve curve in crvLoop)
        //    {
        //      _curves.Add(curve);
        //    }

        //    bool inCurves = IsPointInPolygon(_curves, _curves[0].GetEndPoint(0), pickPos, 1);

        //    if (inCurves)
        //    {
        //      foreach (Revit.DB.Curve curve in crvLoop)
        //      {
        //        curves.Add(curve);

        //        if (curve.IsCyclic)
        //        {
        //          isCyclic = curve.IsCyclic;
        //        }
        //      }

        //      break;
        //    }
        //  }

        //  if (curves.Count < 1)
        //  {
        //    return ret;
        //  }

        //  polyCurves = curves;

        //  if (isCyclic)
        //  {
        //    // そ�E仁E
        //    ret = 9;
        //    return ret;
        //  }

        //  // 調整
        //  curves = OptimizeLineVertexNoConvLine(curves);

        //  // 三角形
        //  if (curves.Count == 3)
        //  {
        //    ret = 1;
        //  }
        //  // 四角形
        //  else if (curves.Count == 4)
        //  {
        //    #region 四角形

        //    // 対角緁E
        //    Collections.Generic.IList<Revit.DB.Line> diagonals = GetDiagonal(curves);

        //    if (diagonals.Count == 2)
        //    {
        //      Revit.DB.Line line1 = diagonals[0];
        //      Revit.DB.Line line2 = diagonals[1];

        //      // 対角線�E交点
        //      Collections.Generic.IList<Revit.DB.XYZ> intersects = new Collections.Generic.List<Revit.DB.XYZ>();
        //      IntersecCurve(line1, line2, ref intersects);

        //      if (intersects.Count == 1)
        //      {
        //        Revit.DB.XYZ p0 = line1.GetEndPoint(0);
        //        Revit.DB.XYZ p1 = line1.GetEndPoint(1);
        //        Revit.DB.XYZ p2 = line2.GetEndPoint(0);
        //        Revit.DB.XYZ p3 = line2.GetEndPoint(1);

        //        Revit.DB.XYZ intersect = intersects[0];

        //        // お互いに2等�E
        //        if (System.Math.Abs(Distance(p0, intersect) - Distance(p1, intersect)) < Approx0Len &&
        //            System.Math.Abs(Distance(p2, intersect) - Distance(p3, intersect)) < Approx0Len)
        //        {
        //          // 対角線同士の長さが等しぁE
        //          if (System.Math.Abs(line1.Length - line2.Length) < Approx0Len)
        //          {
        //            // 交点からのそれぞれの対角線始点への単位�Eクトル
        //            Revit.DB.XYZ uv1 = UnitVector(intersect, p0);
        //            Revit.DB.XYZ uv2 = UnitVector(intersect, p2);

        //            double cosSita = (uv1.X * uv2.X + uv1.Y * uv2.Y + uv1.Z * uv2.Z) / (System.Math.Sqrt(uv1.X * uv1.X + uv1.Y * uv1.Y + uv1.Z * uv1.Z) * System.Math.Sqrt(uv2.X * uv2.X + uv2.Y * uv2.Y + uv2.Z * uv2.Z));

        //            // ラジアン角度
        //            double rad = System.Math.Acos(cosSita);

        //            // 直角に交差
        //            if (System.Math.Abs(System.Math.Abs(rad) - System.Math.PI / 2) < Approx0Ang)
        //            {
        //              // 正方形
        //              ret = 3;
        //            }
        //            // 直角に交差ではなぁE
        //            else
        //            {
        //              // 長方形
        //              ret = 4;
        //            }
        //          }
        //          else
        //          {
        //            // 交点からのそれぞれの対角線始点への単位�Eクトル
        //            Revit.DB.XYZ uv1 = UnitVector(intersect, p0);
        //            Revit.DB.XYZ uv2 = UnitVector(intersect, p2);

        //            double cosSita = (uv1.X * uv2.X + uv1.Y * uv2.Y + uv1.Z * uv2.Z) / (System.Math.Sqrt(uv1.X * uv1.X + uv1.Y * uv1.Y + uv1.Z * uv1.Z) * System.Math.Sqrt(uv2.X * uv2.X + uv2.Y * uv2.Y + uv2.Z * uv2.Z));

        //            // ラジアン角度
        //            double rad = System.Math.Acos(cosSita);

        //            // 直角に交差
        //            if (System.Math.Abs(System.Math.Abs(rad) - System.Math.PI / 2) < Approx0Ang)
        //            {
        //              // ひし形
        //              ret = 5;
        //            }
        //            // 直角に交差ではなぁE
        //            else
        //            {
        //              // 平行四辺形
        //              ret = 6;
        //            }
        //          }
        //        }
        //        // 2等�EではなぁE
        //        else
        //        {
        //          // どちらかの対辺が平衁E
        //          Revit.DB.Line l0 = curves[0] as Revit.DB.Line;
        //          Revit.DB.Line l1 = curves[1] as Revit.DB.Line;
        //          Revit.DB.Line l2 = curves[2] as Revit.DB.Line;
        //          Revit.DB.Line l3 = curves[3] as Revit.DB.Line;

        //          if (IsParallelLine(l0, l2) ||
        //              IsParallelLine(l1, l3))
        //          {
        //            // 台形
        //            ret = 2;
        //          }
        //        }
        //      }
        //    }

        //    #endregion
        //  }
        //  // 六角形
        //  else if (curves.Count == 6)
        //  {
        //    #region 六角形

        //    bool isRect = true;

        //    for (int i = 0; i < curves.Count; ++i)
        //    {
        //      Revit.DB.Curve curve1 = curves[i];
        //      Revit.DB.Curve curve2 = null;

        //      if (i == curves.Count - 1)
        //      {
        //        curve2 = curves[0];
        //      }
        //      else
        //      {
        //        curve2 = curves[i + 1];
        //      }

        //      Revit.DB.XYZ p0 = curve1.GetEndPoint(0);
        //      Revit.DB.XYZ p1 = curve1.GetEndPoint(1);
        //      Revit.DB.XYZ p2 = curve2.GetEndPoint(0);
        //      Revit.DB.XYZ p3 = curve2.GetEndPoint(1);

        //      Revit.DB.XYZ uv1 = UnitVector(p0, p1);
        //      Revit.DB.XYZ uv2 = UnitVector(p2, p3);

        //      double cosSita = CosSita(uv1, uv2);

        //      // ラジアン角度
        //      double rad = System.Math.Acos(cosSita);

        //      // 頂点が直见E
        //      if (System.Math.Abs(System.Math.Abs(rad) - System.Math.PI / 2) < Approx0Ang)
        //      {
        //        isRect = true;
        //      }
        //      else
        //      {
        //        isRect = false;
        //        break;
        //      }
        //    }

        //    // すべての頂点が直见E
        //    if (isRect)
        //    {
        //      // L字形
        //      ret = 7;
        //    }

        //    #endregion
        //  }
        //  // 八角形
        //  else if (curves.Count == 8)
        //  {
        //    #region 八角形

        //    bool isRect = true;

        //    for (int i = 0; i < curves.Count; ++i)
        //    {
        //      Revit.DB.Curve curve1 = curves[i];
        //      Revit.DB.Curve curve2 = null;

        //      if (i == curves.Count - 1)
        //      {
        //        curve2 = curves[0];
        //      }
        //      else
        //      {
        //        curve2 = curves[i + 1];
        //      }

        //      Revit.DB.XYZ p0 = curve1.GetEndPoint(0);
        //      Revit.DB.XYZ p1 = curve1.GetEndPoint(1);
        //      Revit.DB.XYZ p2 = curve2.GetEndPoint(0);
        //      Revit.DB.XYZ p3 = curve2.GetEndPoint(1);

        //      Revit.DB.XYZ uv1 = UnitVector(p0, p1);
        //      Revit.DB.XYZ uv2 = UnitVector(p2, p3);

        //      double cosSita = CosSita(uv1, uv2);

        //      // ラジアン角度
        //      double rad = System.Math.Acos(cosSita);

        //      // 頂点が直见E
        //      if (System.Math.Abs(System.Math.Abs(rad) - System.Math.PI / 2) < Approx0Ang)
        //      {
        //        isRect = true;
        //      }
        //      else
        //      {
        //        isRect = false;
        //        break;
        //      }
        //    }

        //    // すべての頂点が直见E
        //    if (isRect)
        //    {
        //      // 凸凹並び頁E正負で判宁E
        //      Collections.Generic.IList<Collections.Generic.IList<int>> unevenness = new Collections.Generic.List<Collections.Generic.IList<int>>();

        //      for (int i = 0; i < curves.Count; ++i)
        //      {
        //        Revit.DB.Curve curve1 = curves[i];
        //        Revit.DB.Curve curve2 = null;

        //        if (i == curves.Count - 1)
        //        {
        //          curve2 = curves[0];
        //        }
        //        else
        //        {
        //          curve2 = curves[i + 1];
        //        }

        //        Revit.DB.XYZ p0 = curve1.GetEndPoint(0);
        //        Revit.DB.XYZ p1 = curve1.GetEndPoint(1);
        //        Revit.DB.XYZ p2 = curve2.GetEndPoint(0);
        //        Revit.DB.XYZ p3 = curve2.GetEndPoint(1);

        //        Revit.DB.XYZ uv1 = UnitVector(p0, p1);
        //        Revit.DB.XYZ uv2 = UnitVector(p2, p3);

        //        Revit.DB.XYZ cross = Gaiseki(uv1, uv2);

        //        // 符号 正めE、負めE
        //        Collections.Generic.IList<int> ary = new Collections.Generic.List<int>();
        //        ary.Add(cross.X > 0 ? 1 : 0);
        //        ary.Add(cross.Y > 0 ? 1 : 0);
        //        ary.Add(cross.Z > 0 ? 1 : 0);

        //        unevenness.Add(ary);
        //      }

        //      // 最初�E頂点
        //      Collections.Generic.IList<int> ary0 = unevenness[0];

        //      // 最初�E頂点と同一の凸凹ぁE
        //      Collections.Generic.IList<int> sameAry = new Collections.Generic.List<int>();

        //      int sameCount = 0;
        //      int diffeCount = 0;

        //      foreach (Collections.Generic.IList<int> ary1 in unevenness)
        //      {
        //        if (ary0[0] == ary1[0] &&
        //            ary0[1] == ary1[1] &&
        //            ary0[2] == ary1[2])
        //        {
        //          sameAry.Add(1);
        //          sameCount += 1;
        //        }
        //        else
        //        {
        //          sameAry.Add(0);
        //          diffeCount += 1;
        //        }
        //      }

        //      string order = "";
        //      foreach (int same in sameAry)
        //      {
        //        order += same.ToString();
        //      }
        //      foreach (int same in sameAry)
        //      {
        //        order += same.ToString();
        //      }

        //      // T孁E凸垁Eは凸の頂点ぁE個、�Eの頂点ぁE個で
        //      // 頂点の並び頁E��凸凸凸凸凹凸凸凹
        //      if (sameCount == 6 &&
        //          diffeCount == 2)
        //      {
        //        // 多い方めE、少なぁE��めE
        //        if (order.Contains("11110110"))
        //        {
        //          // T字形
        //          ret = 8;
        //        }
        //      }
        //      else if (sameCount == 2 &&
        //               diffeCount == 6)
        //      {
        //        // 多い方めE、少なぁE��めE
        //        if (order.Contains("00001001"))
        //        {
        //          // T字形
        //          ret = 8;
        //        }
        //      }
        //    }

        //    #endregion
        //  }

        //  // 該当なぁE
        //  if (ret == 0)
        //  {
        //    // そ�E仁E
        //    ret = 9;
        //  }

        //  return ret;
        //}

        /// ================================================================================
        /// <summary>平行判宁E/summary>
        /// 
        /// <param name="line1">線�E1</param>
        /// <param name="line2">線�E2</param>
        /// 
        /// <history>2016/12/06 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool IsParallelLine(Revit.DB.Line line1,
                            Revit.DB.Line line2)
        {
            // 戻り値
            bool ret = false;

            Revit.DB.XYZ p0 = line1.GetEndPoint(0);
            Revit.DB.XYZ p1 = line1.GetEndPoint(1);
            Revit.DB.XYZ p2 = line2.GetEndPoint(0);
            Revit.DB.XYZ p3 = line2.GetEndPoint(1);

            Revit.DB.XYZ uv1 = UnitVector(p1, p0);
            Revit.DB.XYZ uv2 = UnitVector(p3, p2);

            double cosSita = (uv1.X * uv2.X + uv1.Y * uv2.Y + uv1.Z * uv2.Z) / (System.Math.Sqrt(uv1.X * uv1.X + uv1.Y * uv1.Y + uv1.Z * uv1.Z) * System.Math.Sqrt(uv2.X * uv2.X + uv2.Y * uv2.Y + uv2.Z * uv2.Z));

            // ラジアン角度
            double rad = System.Math.Acos(cosSita);

            // 0ぁE80度
            if (System.Math.Abs(rad) < Approx0Ang ||
                System.Math.Abs(System.Math.Abs(rad) - System.Math.PI) < Approx0Ang)
            {
                ret = true;
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>対角線取征E/summary>
        /// 
        /// <param name="polygonCurves">多角形線�E</param>
        /// 
        /// <history>2016/11/24 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Revit.DB.Line> GetDiagonal(Collections.Generic.IList<Revit.DB.Curve> polygonCurves)
        {
            // 戻り値
            Collections.Generic.IList<Revit.DB.Line> ret = new Collections.Generic.List<Revit.DB.Line>();

            Collections.Generic.IList<Revit.DB.XYZ> posAry = new Collections.Generic.List<Revit.DB.XYZ>();
            foreach (Revit.DB.Curve curve in polygonCurves)
            {
                Revit.DB.XYZ pos = curve.GetEndPoint(0);
                posAry.Add(pos);
            }

            for (int i = 0; i < posAry.Count; ++i)
            {
                // 最後�E点
                if (i == posAry.Count - 1)
                {
                    continue;
                }

                Revit.DB.XYZ p0 = posAry[i];

                for (int j = 0; j < posAry.Count; ++j)
                {
                    // 先�E点に限宁E
                    if (i + 1 >= j)
                    {
                        continue;
                    }
                    // 例夁E最初�E点
                    if (i == 0)
                    {
                        // 最後�E点が前の点
                        if (j == posAry.Count - 1)
                        {
                            continue;
                        }
                    }

                    Revit.DB.XYZ p1 = posAry[j];

                    if (Distance(p0, p1) > Approx0Len)
                    {
                        Revit.DB.Line line = Revit.DB.Line.CreateBound(p0, p1);
                        ret.Add(line);
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>グラフィチE��ススタイルID取征E/summary>
        /// 
        /// <param name="elem">要素</param>
        /// 
        /// <history>2016/11/28 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Revit.DB.ElementId GetGraphicsStyleId(Revit.DB.Element elem)
        {
            // 戻り値
            Revit.DB.ElementId ret = null;

            Revit.DB.Options opt = elem.Document.Application.Create.NewGeometryOptions();

            Revit.DB.GeometryElement geomElem = elem.get_Geometry(opt);

            Collections.Generic.IEnumerator<Revit.DB.GeometryObject> geoObjEnum = geomElem.GetEnumerator();
            geoObjEnum.Reset();

            while (geoObjEnum.MoveNext())
            {
                Revit.DB.GeometryObject geoObj = geoObjEnum.Current;
                Revit.DB.GeometryInstance geoIns = geoObj as Revit.DB.GeometryInstance;

                if (geoIns != null)
                {
                    Revit.DB.FamilyInstance famIns = elem as Revit.DB.FamilyInstance;
                    if (famIns == null) return null;

                    Revit.DB.GeometryElement ge = geoIns.GetSymbolGeometry(famIns.GetTransform());
                    Collections.Generic.IEnumerator<Revit.DB.GeometryObject> goEnum = ge.GetEnumerator();
                    goEnum.Reset();

                    while (goEnum.MoveNext())
                    {
                        Revit.DB.GeometryObject go = goEnum.Current;

                        if (go.GraphicsStyleId != null && Int32.Parse(go.GraphicsStyleId.ToString()) > 0)
                        {
                            ret = go.GraphicsStyleId;
                        }
                    }
                }
                else
                {
                    if (geoObj.GraphicsStyleId != null && Int32.Parse(geoObj.GraphicsStyleId.ToString()) > 0)
                    {
                        ret = geoObj.GraphicsStyleId;
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>持E��点が含まれる面</summary>
        /// 
        /// <param name="elem">要素</param>
        /// <param name="pos" >点</param>
        /// 
        /// <history>2016/12/07 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Revit.DB.PlanarFace GetPlnFace(Revit.DB.Element elem,
                                       Revit.DB.XYZ pos)
        {
            // 戻り値
            Revit.DB.PlanarFace ret = null;

            Revit.DB.Options opt = elem.Document.Application.Create.NewGeometryOptions();
            opt.ComputeReferences = true;

            Revit.DB.GeometryElement geomElem = elem.get_Geometry(opt);

            Collections.Generic.IEnumerator<Revit.DB.GeometryObject> geoObjEnum = geomElem.GetEnumerator();
            geoObjEnum.Reset();

            while (geoObjEnum.MoveNext())
            {
                Revit.DB.GeometryObject geoObj = geoObjEnum.Current;
                Revit.DB.GeometryInstance geoIns = geoObj as Revit.DB.GeometryInstance;

                if (geoIns != null)
                {
                    Revit.DB.FamilyInstance famIns = elem as Revit.DB.FamilyInstance;
                    if (famIns == null) return null;

                    Revit.DB.GeometryElement ge = geoIns.GetSymbolGeometry(famIns.GetTransform());
                    Collections.Generic.IEnumerator<Revit.DB.GeometryObject> goEnum = ge.GetEnumerator();
                    goEnum.Reset();

                    while (goEnum.MoveNext())
                    {
                        Revit.DB.GeometryObject go = goEnum.Current;

                        Revit.DB.Solid solid = go as Revit.DB.Solid;

                        if (solid != null)
                        {
                            foreach (Revit.DB.Face face in solid.Faces)
                            {
                                Revit.DB.PlanarFace plnFace = face as Revit.DB.PlanarFace;

                                if (plnFace != null)
                                {
                                    Revit.DB.Mesh mesh = face.Triangulate();
                                    Collections.Generic.IList<Revit.DB.XYZ> posAry = mesh.Vertices;

                                    if (posAry.Count > 2)
                                    {
                                        Revit.DB.XYZ pA = posAry[0];
                                        Revit.DB.XYZ pB = posAry[1];
                                        Revit.DB.XYZ pC = posAry[2];

                                        // 平面の法緁E
                                        Revit.DB.XYZ normal = plnFace.FaceNormal;

                                        // 面上点から選択点へのベクトル
                                        Revit.DB.XYZ vec = new Revit.DB.XYZ(pos.X - pA.X,
                                                                            pos.Y - pA.Y,
                                                                            pos.Z - pA.Z);

                                        // 法線との冁E��E
                                        // 0は面丁E同じ方向への働きなぁE、正は同じ向き、負は送E��ぁE
                                        double dotN = Naiseki(vec, normal);

                                        // 誤差吸収（�E琁E��る桁�E適宜調整�E�E
                                        if (System.Math.Abs(dotN) < 0.0001)
                                        {
                                            dotN = 0.0;
                                        }

                                        if (dotN == 0.0)
                                        {
                                            ret = plnFace;
                                            return ret;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Revit.DB.Solid solid = geoObj as Revit.DB.Solid;

                    if (solid != null)
                    {
                        foreach (Revit.DB.Face face in solid.Faces)
                        {
                            Revit.DB.PlanarFace plnFace = face as Revit.DB.PlanarFace;

                            if (plnFace != null)
                            {
                                Revit.DB.Mesh mesh = face.Triangulate();
                                Collections.Generic.IList<Revit.DB.XYZ> posAry = mesh.Vertices;

                                if (posAry.Count > 2)
                                {
                                    Revit.DB.XYZ pA = posAry[0];
                                    Revit.DB.XYZ pB = posAry[1];
                                    Revit.DB.XYZ pC = posAry[2];

                                    // 平面の法緁E
                                    Revit.DB.XYZ normal = plnFace.FaceNormal;

                                    // 面上点から選択点へのベクトル
                                    Revit.DB.XYZ vec = new Revit.DB.XYZ(pos.X - pA.X,
                                                                        pos.Y - pA.Y,
                                                                        pos.Z - pA.Z);

                                    // 法線との冁E��E
                                    // 0は面丁E同じ方向への働きなぁE、正は同じ向き、負は送E��ぁE
                                    double dotN = Naiseki(vec, normal);

                                    // 誤差吸収（�E琁E��る桁�E適宜調整�E�E
                                    if (System.Math.Abs(dotN) < 0.0001)
                                    {
                                        dotN = 0.0;
                                    }

                                    if (dotN == 0.0)
                                    {
                                        ret = plnFace;
                                        return ret;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>エチE��が含まれる面</summary>
        /// 
        /// <param name="elem">要素</param>
        /// <param name="edge">エチE��</param>
        /// 
        /// <history>2016/12/07 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Revit.DB.PlanarFace> GetPlnFace(Revit.DB.Element elem,
                                                                  Revit.DB.Edge edge)
        {
            // 戻り値
            Collections.Generic.IList<Revit.DB.PlanarFace> ret = new Collections.Generic.List<Revit.DB.PlanarFace>();

            Revit.DB.Curve curve = edge.AsCurve();

            Revit.DB.Options opt = elem.Document.Application.Create.NewGeometryOptions();
            opt.ComputeReferences = true;

            Revit.DB.GeometryElement geomElem = elem.get_Geometry(opt);

            Collections.Generic.IEnumerator<Revit.DB.GeometryObject> geoObjEnum = geomElem.GetEnumerator();
            geoObjEnum.Reset();

            while (geoObjEnum.MoveNext())
            {
                Revit.DB.GeometryObject geoObj = geoObjEnum.Current;
                Revit.DB.GeometryInstance geoIns = geoObj as Revit.DB.GeometryInstance;

                if (geoIns != null)
                {
                    Revit.DB.FamilyInstance famIns = elem as Revit.DB.FamilyInstance;
                    if (famIns == null) return null;

                    Revit.DB.GeometryElement ge = geoIns.GetSymbolGeometry(famIns.GetTransform());
                    Collections.Generic.IEnumerator<Revit.DB.GeometryObject> goEnum = ge.GetEnumerator();
                    goEnum.Reset();

                    while (goEnum.MoveNext())
                    {
                        Revit.DB.GeometryObject go = goEnum.Current;

                        Revit.DB.Solid solid = go as Revit.DB.Solid;

                        if (solid != null)
                        {
                            foreach (Revit.DB.Face face in solid.Faces)
                            {
                                Revit.DB.PlanarFace plnFace = face as Revit.DB.PlanarFace;

                                if (plnFace != null)
                                {
                                    bool set = false;

                                    foreach (Revit.DB.EdgeArray edgeAry in plnFace.EdgeLoops)
                                    {
                                        foreach (Revit.DB.Edge e in edgeAry)
                                        {
                                            Revit.DB.Curve c = e.AsCurve();

                                            if (EqualCurve(curve, c))
                                            {
                                                ret.Add(plnFace);
                                                set = true;
                                                break;
                                            }
                                        }

                                        if (set)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Revit.DB.Solid solid = geoObj as Revit.DB.Solid;

                    if (solid != null)
                    {
                        foreach (Revit.DB.Face face in solid.Faces)
                        {
                            Revit.DB.PlanarFace plnFace = face as Revit.DB.PlanarFace;

                            if (plnFace != null)
                            {
                                bool set = false;

                                foreach (Revit.DB.EdgeArray edgeAry in plnFace.EdgeLoops)
                                {
                                    foreach (Revit.DB.Edge e in edgeAry)
                                    {
                                        Revit.DB.Curve c = e.AsCurve();

                                        if (EqualCurve(curve, c))
                                        {
                                            ret.Add(plnFace);
                                            set = true;
                                            break;
                                        }
                                    }

                                    if (set)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>傾き�E近い平面取征E/summary>
        /// 
        /// <param name="plnFace" >基準面</param>
        /// <param name="plnFaces">対象面</param>
        /// 
        /// <history>2016/12/07 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Revit.DB.PlanarFace GetSimilarPlnFace(Revit.DB.PlanarFace plnFace,
                                              Collections.Generic.IList<Revit.DB.PlanarFace> plnFaces)
        {
            // 戻り値
            Revit.DB.PlanarFace ret = null;

            Revit.DB.XYZ normal = plnFace.FaceNormal;

            double rad = 0;

            foreach (Revit.DB.PlanarFace pf in plnFaces)
            {
                Revit.DB.XYZ norm = pf.FaceNormal;

                double dot = Naiseki(normal, norm);
                Revit.DB.XYZ cross = Gaiseki(normal, norm);

                double r = System.Math.Atan2(cross.Z, dot);

                if (System.Math.Abs(dot) < 0.00001)
                {
                    dot = 0.0;
                }

                if (dot == 0)
                {
                    r = System.Math.PI / 2;
                }

                if (r > System.Math.PI / 2)
                {
                    r -= System.Math.PI;
                }
                else if (r < -System.Math.PI / 2)
                {
                    r += System.Math.PI;
                }

                r = System.Math.Abs(r);

                if (ret == null)
                {
                    ret = pf;
                    rad = r;
                }
                else
                {
                    if (rad > r)
                    {
                        ret = pf;
                        rad = r;
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>基準面刁E��エチE��の条件確誁E/summary>
        /// 
        /// <param name="edge1"   >エチE��1</param>
        /// <param name="edge2"   >エチE��2</param>
        /// <param name="plnFace" >基準面</param>
        /// <param name="divide"  >刁E��緁E/param>
        /// 
        /// <history>2016/12/08 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool FulFillEdges(Revit.DB.Edge edge1,
                          Revit.DB.Edge edge2,
                          Revit.DB.PlanarFace plnFace,
                          ref Revit.DB.Line divide)
        {
            // 戻り値
            bool ret = false;

            Revit.DB.Curve curve1 = edge1.AsCurve();
            Revit.DB.Curve curve2 = edge2.AsCurve();

            if (curve1.IsCyclic || curve2.IsCyclic)
            {
                return ret;
            }

            Revit.DB.XYZ pA = plnFace.Origin;
            Revit.DB.XYZ pB = pA + plnFace.XVector;
            Revit.DB.XYZ pC = pA + plnFace.YVector;
            Revit.DB.XYZ normal = plnFace.FaceNormal;

            // 端点
            Revit.DB.XYZ p0 = curve1.GetEndPoint(0);
            Revit.DB.XYZ p1 = curve1.GetEndPoint(1);
            Revit.DB.XYZ p2 = curve2.GetEndPoint(0);
            Revit.DB.XYZ p3 = curve2.GetEndPoint(1);

            // 投影した点
            Revit.DB.XYZ pShadow0 = GetCrossPoint(Revit.DB.Line.CreateBound(p0, p0 + normal), pA, pB, pC, 1);
            Revit.DB.XYZ pShadow1 = GetCrossPoint(Revit.DB.Line.CreateBound(p1, p1 + normal), pA, pB, pC, 1);
            Revit.DB.XYZ pShadow2 = GetCrossPoint(Revit.DB.Line.CreateBound(p2, p2 + normal), pA, pB, pC, 1);
            Revit.DB.XYZ pShadow3 = GetCrossPoint(Revit.DB.Line.CreateBound(p3, p3 + normal), pA, pB, pC, 1);

            Revit.DB.XYZ uv1 = UnitVector(pShadow0, pShadow1);
            Revit.DB.XYZ uv2 = UnitVector(pShadow2, pShadow3);

            double cosSita = CosSita(uv1, uv2);

            // ラジアン角度
            double rad = System.Math.Acos(cosSita);

            if (rad - System.Math.PI / 2 > Approx0Ang)
            {
                rad -= System.Math.PI;
            }
            else if (rad + System.Math.PI / 2 < Approx0Ang)
            {
                rad += System.Math.PI;
            }

            // 2つのエチE��の傾きが異なめE
            if (System.Math.Abs(rad) > Approx0Ang)
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_EDGEANGLE"),
                                                     _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));

                return ret;
            }

            bool isSameAng = false;

            Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = plnFace.GetEdgesAsCurveLoops();

            Collections.Generic.IList<Revit.DB.Curve> curves = new Collections.Generic.List<Revit.DB.Curve>();
            foreach (Revit.DB.CurveLoop crvLoop in crvLoops)
            {
                foreach (Revit.DB.Curve curve in crvLoop)
                {
                    Revit.DB.XYZ ep0 = curve.GetEndPoint(0);
                    Revit.DB.XYZ ep1 = curve.GetEndPoint(1);

                    Revit.DB.XYZ uv = UnitVector(ep0, ep1);

                    cosSita = CosSita(uv, uv1);

                    rad = System.Math.Acos(cosSita);

                    if (System.Math.Abs(rad) < Approx0Ang)
                    {
                        isSameAng = true;
                    }
                }
            }

            // 基準面の辺と同じ傾きなぁE
            if (isSameAng == false)
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_EDGEANGLEBASE"),
                                                     _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));

                return ret;
            }

            ret = true;

            Revit.DB.XYZ norm = uv1.CrossProduct(normal);

            // 基準面から遠ぁE��
            double dis0 = Distance(p0, pShadow0);
            double dis1 = Distance(p1, pShadow1);
            double dis2 = Distance(p2, pShadow2);
            double dis3 = Distance(p3, pShadow3);

            if ((dis0 > dis2 && dis0 > dis3) || (dis1 > dis2 && dis1 > dis3))
            {
                // 近い端点
                if (Distance(pShadow0, pShadow2) <= Distance(pShadow1, pShadow2))
                {
                    divide = Revit.DB.Line.CreateBound(pShadow0, pShadow0 + norm);
                }
                else
                {
                    divide = Revit.DB.Line.CreateBound(pShadow1, pShadow1 + norm);
                }
            }
            else
            {
                // 近い端点
                if (Distance(pShadow2, pShadow0) <= Distance(pShadow3, pShadow0))
                {
                    divide = Revit.DB.Line.CreateBound(pShadow2, pShadow2 + norm);
                }
                else
                {
                    divide = Revit.DB.Line.CreateBound(pShadow3, pShadow3 + norm);
                }
            }

            Revit.DB.XYZ lp0 = divide.GetEndPoint(0);
            Revit.DB.XYZ lp1 = divide.GetEndPoint(1);
            Revit.DB.XYZ vec = new Revit.DB.XYZ(lp1.X - lp0.X,
                                                lp1.Y - lp0.Y,
                                                lp1.Z - lp0.Z);
            lp0 = lp0 + vec * 1000;
            lp1 = lp1 + -vec * 1000;
            divide = Revit.DB.Line.CreateBound(lp0, lp1);

            return ret;
        }

        /// ================================================================================
        /// <summary>基準面形状の刁E��</summary>
        /// 
        /// <param name="curves">カーチE/param>
        /// <param name="divide">刁E��緁E/param>
        /// 
        /// <history>2016/12/08 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> DividePolCurves(Collections.Generic.IList<Revit.DB.Curve> curves,
                                                                                             Revit.DB.Line divide)
        {
            // 戻り値
            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> ret = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.Curve>>();

            int curveNum = 0;
            int count = 0;

            int countIntersect = 0;
            // 交差回数 �E�E と 2�E��E1つ目のカーチE
            // 1�E�Eは2つ目のカーチE

            Collections.Generic.IList<Revit.DB.XYZ> posAry1 = new Collections.Generic.List<Revit.DB.XYZ>();
            Collections.Generic.IList<Revit.DB.XYZ> posAry2 = new Collections.Generic.List<Revit.DB.XYZ>();

            foreach (Revit.DB.Curve curve in curves)
            {
                count += 1;

                Revit.DB.XYZ p0 = curve.GetEndPoint(0);

                Collections.Generic.IList<Revit.DB.XYZ> intersects = new Collections.Generic.List<Revit.DB.XYZ>();
                IntersecCurve(curve, divide, ref intersects);

                if (intersects.Count == 0)
                {
                    if (countIntersect != 1)
                    {
                        posAry1.Add(p0);
                    }
                    else if (countIntersect == 1)
                    {
                        posAry2.Add(p0);
                    }
                }
                else if (intersects.Count == 1)
                {
                    if (countIntersect != 1)
                    {
                        posAry1.Add(p0);
                        posAry1.Add(intersects[0]);

                        posAry2.Add(intersects[0]);
                    }
                    else if (countIntersect == 1)
                    {
                        posAry2.Add(p0);
                        posAry2.Add(intersects[0]);

                        posAry1.Add(intersects[0]);
                    }

                    countIntersect += 1;

                    if (curveNum == 0)
                    {
                        curveNum = count;
                    }
                }
            }

            Collections.Generic.IList<Revit.DB.Curve> curves1 = new Collections.Generic.List<Revit.DB.Curve>();
            for (int i = 0; i < posAry1.Count; ++i)
            {
                if (i < posAry1.Count - 1)
                {
                    Revit.DB.Line line = Revit.DB.Line.CreateBound(posAry1[i], posAry1[i + 1]);
                    curves1.Add(line);
                }
                else
                {
                    Revit.DB.Line line = Revit.DB.Line.CreateBound(posAry1[i], posAry1[0]);
                    curves1.Add(line);
                }
            }
            ret.Add(curves1);


            Collections.Generic.IList<Revit.DB.Curve> curves2 = new Collections.Generic.List<Revit.DB.Curve>();

            // 最初�E交差が�E数回目
            if (curveNum % 2 == 0)
            {
                for (int i = 0; i < posAry2.Count; ++i)
                {
                    if (i == 0)
                    {
                        Revit.DB.Line line = Revit.DB.Line.CreateBound(posAry2[posAry2.Count - 1], posAry2[i]);
                        curves2.Add(line);
                    }
                    else
                    {
                        Revit.DB.Line line = Revit.DB.Line.CreateBound(posAry2[i - 1], posAry2[i]);
                        curves2.Add(line);
                    }
                }
            }
            else
            {
                for (int i = 0; i < posAry2.Count; ++i)
                {
                    if (i < posAry2.Count - 1)
                    {
                        Revit.DB.Line line = Revit.DB.Line.CreateBound(posAry2[i], posAry2[i + 1]);
                        curves2.Add(line);
                    }
                    else
                    {
                        Revit.DB.Line line = Revit.DB.Line.CreateBound(posAry2[i], posAry2[0]);
                        curves2.Add(line);
                    }
                }
            }

            ret.Add(curves2);

            return ret;
        }

        /// ================================================================================
        /// <summary>平坁E��</summary>
        /// 
        /// <param name="curves">カーチE/param>
        /// 
        /// <history>2016/12/08 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Revit.DB.XYZ AveragePos(Collections.Generic.IList<Revit.DB.Curve> curves)
        {
            Revit.DB.XYZ ret = new Revit.DB.XYZ();

            foreach (Revit.DB.Curve curve in curves)
            {
                ret += curve.GetEndPoint(0);
            }

            ret /= curves.Count;

            return ret;
        }

        /// ================================================================================
        /// <summary>基準面からの距離</summary>
        /// 
        /// <param name="plnFace"       >基準面</param>
        /// <param name="heightPlnFace" >高さ用面</param>
        /// <param name="edgesFaces"    >エチE��を含む面</param>
        /// 
        /// <history>2016/12/15 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        double GetPlaneDistance(Revit.DB.PlanarFace plnFace,
                                Revit.DB.PlanarFace heightPlnFace,
                                Collections.Generic.IList<Collections.Generic.IList<Revit.DB.PlanarFace>> edgesFaces)
        {
            // 戻り値
            double ret = 0;

            Revit.DB.XYZ normal = plnFace.FaceNormal;

            Revit.DB.XYZ p0 = plnFace.Origin;
            Revit.DB.XYZ p1 = p0 + normal;
            Revit.DB.Line normLine = Revit.DB.Line.CreateBound(p0, p1);

            if (heightPlnFace != null)
            {
                //両面が平行か�E�E
                double h1 = Math.Abs(Naiseki(plnFace.FaceNormal, heightPlnFace.FaceNormal));
                if (h1 < 1.0 - Approx0Len)
                {
                    return -1.0;
                }

                Revit.DB.XYZ pA = heightPlnFace.Origin;
                Revit.DB.XYZ pB = pA + heightPlnFace.XVector;
                Revit.DB.XYZ pC = pA + heightPlnFace.YVector;

                Revit.DB.XYZ intersectPos = GetCrossPoint(normLine, pA, pB, pC, 1);
                if (intersectPos == null)
                {
                    return -1.0;
                }

                ret = Distance(p0, intersectPos);
            }
            else
            {
                foreach (Collections.Generic.IList<Revit.DB.PlanarFace> edgeFaces in edgesFaces)
                {
                    Revit.DB.PlanarFace similarPlnFace = GetSimilarPlnFace(plnFace, edgeFaces);

                    Revit.DB.XYZ pA = similarPlnFace.Origin;
                    Revit.DB.XYZ pB = pA + similarPlnFace.XVector;
                    Revit.DB.XYZ pC = pA + similarPlnFace.YVector;

                    Revit.DB.XYZ intersectPos = GetCrossPoint(normLine, pA, pB, pC, 1);

                    if (intersectPos == null)
                    {
                        return -1.0;
                    }
                    ret = Distance(p0, intersectPos);
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>同一面上で線�Eと同じ側の判宁E/summary>
        /// 
        /// <param name="edge"    >エチE��</param>
        /// <param name="plnFace" >基準面</param>
        /// <param name="curves"  >カーチE/param>
        /// <param name="divide"  >墁E��緁E/param>
        /// 
        /// <history>2016/12/08 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool SameSideCurves(Revit.DB.Edge edge,
                            Revit.DB.PlanarFace plnFace,
                            Collections.Generic.IList<Revit.DB.Curve> curves,
                            Revit.DB.Line divide)
        {
            // 戻り値
            bool ret = false;

            Revit.DB.Curve edgeCrv = edge.AsCurve();
            Revit.DB.XYZ ep0 = edgeCrv.GetEndPoint(0);
            Revit.DB.XYZ ep1 = edgeCrv.GetEndPoint(1);

            Revit.DB.XYZ normal = plnFace.FaceNormal;
            Revit.DB.XYZ pA = plnFace.Origin;
            Revit.DB.XYZ pB = pA + plnFace.XVector;
            Revit.DB.XYZ pC = pA + plnFace.YVector;

            Revit.DB.Line lShadow0 = Revit.DB.Line.CreateBound(ep0, ep0 + normal);
            Revit.DB.Line lShadow1 = Revit.DB.Line.CreateBound(ep1, ep1 + normal);

            // 投影点
            Revit.DB.XYZ pShadow0 = GetCrossPoint(lShadow0, pA, pB, pC, 1);
            Revit.DB.XYZ pShadow1 = GetCrossPoint(lShadow1, pA, pB, pC, 1);

            // 刁E��線丁E
            bool onLine0 = IsOnLine(divide, pShadow0, 1);
            bool onLine1 = IsOnLine(divide, pShadow1, 1);

            foreach (Revit.DB.Curve curve in curves)
            {
                Revit.DB.XYZ cp0 = curve.GetEndPoint(0);
                Revit.DB.XYZ cp1 = curve.GetEndPoint(1);

                Collections.Generic.IList<Revit.DB.XYZ> intersects = new Collections.Generic.List<Revit.DB.XYZ>();

                if (onLine0 == false)
                {
                    if (Distance(pShadow0, cp0) > Approx0Len)
                    {
                        Revit.DB.Line line0 = Revit.DB.Line.CreateBound(pShadow0, cp0);
                        IntersecCurve(divide, line0, ref intersects);
                        if (intersects.Count < 1)
                        {
                            ret = true;
                        }
                        intersects.Clear();
                    }

                    if (Distance(pShadow0, cp1) > Approx0Len)
                    {
                        Revit.DB.Line line1 = Revit.DB.Line.CreateBound(pShadow0, cp1);
                        IntersecCurve(divide, line1, ref intersects);
                        if (intersects.Count < 1)
                        {
                            ret = true;
                        }
                        intersects.Clear();
                    }
                }

                if (onLine1 == false)
                {
                    if (Distance(pShadow1, cp0) > Approx0Len)
                    {
                        Revit.DB.Line line2 = Revit.DB.Line.CreateBound(pShadow1, cp0);
                        IntersecCurve(divide, line2, ref intersects);
                        if (intersects.Count < 1)
                        {
                            ret = true;
                        }
                        intersects.Clear();
                    }

                    if (Distance(pShadow1, cp1) > Approx0Len)
                    {
                        Revit.DB.Line line3 = Revit.DB.Line.CreateBound(pShadow1, cp1);
                        IntersecCurve(divide, line3, ref intersects);
                        if (intersects.Count < 1)
                        {
                            ret = true;
                        }
                        intersects.Clear();
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>直線上判宁E/summary>
        /// 
        /// <param name="line">直緁E/param>
        /// <param name="p"   >点</param>
        /// <param name="mode">端点上含む = 1、含まなぁE= 0</param>
        /// 
        /// <history>2016/12/09 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool IsOnLine(Revit.DB.Line line, Revit.DB.XYZ p, int mode)
        {
            // 戻り値
            bool ret = false;

            Revit.DB.XYZ p0 = line.GetEndPoint(0);
            Revit.DB.XYZ p1 = line.GetEndPoint(1);

            if (mode == 1)
            {
                // 端点丁E
                if (Distance(p0, p) < Approx0Len)
                {
                    ret = true;
                    return ret;
                }
                if (Distance(p1, p) < Approx0Len)
                {
                    ret = true;
                    return ret;
                }
            }

            // 始点から点へ
            Revit.DB.XYZ uv0 = UnitVector(p0, p);
            // 点から終点へ
            Revit.DB.XYZ uv1 = UnitVector(p, p1);

            if (System.Math.Abs(uv0.X - uv1.X) > Approx0Len ||
                System.Math.Abs(uv0.Y - uv1.Y) > Approx0Len ||
                System.Math.Abs(uv0.Z - uv1.Z) > Approx0Len)
            {
                return ret;
            }

            // 終点から点へ
            uv0 = UnitVector(p1, p);
            // 点から始点へ
            uv1 = UnitVector(p, p0);

            if (System.Math.Abs(uv0.X - uv1.X) > Approx0Len ||
                System.Math.Abs(uv0.Y - uv1.Y) > Approx0Len ||
                System.Math.Abs(uv0.Z - uv1.Z) > Approx0Len)
            {
                return ret;
            }

            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>関連性判宁E- エチE��と面</summary>
        /// 
        /// <param name="edge"    >エチE��</param>
        /// <param name="plnFace" >平面</param>
        /// 
        /// <returns>true = 関連あり(面冁E��外周丁E</returns>
        /// 
        /// <history><p>2016/12/09 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2016/12/14 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        bool RelevantEdgeOnCurves(Revit.DB.Edge edge,
                                  Collections.Generic.IList<Revit.DB.Curve> curvesA,
                                  Revit.DB.PlanarFace plnFace)
        {
            bool ret = false;

            Revit.DB.Curve edgeCrv = edge.AsCurve();
            Revit.DB.XYZ p0 = edgeCrv.GetEndPoint(0);
            Revit.DB.XYZ p1 = edgeCrv.GetEndPoint(1);

            Collections.Generic.IList<Revit.DB.Curve> curves = GetFaceCurves(plnFace);

            if (curvesA.Count != 0)
            {
                curves = curvesA;
            }

            Revit.DB.XYZ pA = plnFace.Origin;
            Revit.DB.XYZ pB = pA + plnFace.XVector;
            Revit.DB.XYZ pC = pA + plnFace.YVector;
            Revit.DB.XYZ normal = plnFace.FaceNormal;

            Revit.DB.Line lShadow0 = Revit.DB.Line.CreateBound(p0, p0 + normal);
            Revit.DB.Line lShadow1 = Revit.DB.Line.CreateBound(p1, p1 + normal);

            // 投影点
            Revit.DB.XYZ pShadow0 = GetCrossPoint(lShadow0, pA, pB, pC, 1);
            Revit.DB.XYZ pShadow1 = GetCrossPoint(lShadow1, pA, pB, pC, 1);

            if (Distance(pShadow0, pShadow1) < Approx0Len)
            {
                return ret;
            }

            Revit.DB.Line lShadow = Revit.DB.Line.CreateBound(pShadow0, pShadow1);

            foreach (Revit.DB.Curve curve in curves)
            {
                if (curve.IsCyclic == false)
                {
                    Revit.DB.Line line = curve as Revit.DB.Line;

                    if (IsOnLine(line, pShadow0, 1))
                    {
                        ret = true;
                        break;
                    }
                    else if (IsOnLine(line, pShadow1, 1))
                    {
                        ret = true;
                        break;
                    }
                    else
                    {
                        Revit.DB.XYZ lp0 = line.GetEndPoint(0);
                        Revit.DB.XYZ lp1 = line.GetEndPoint(1);

                        if (IsOnLine(lShadow, lp0, 1))
                        {
                            ret = true;
                            break;
                        }
                        else if (IsOnLine(lShadow, lp1, 1))
                        {
                            ret = true;
                            break;
                        }
                    }
                }
            }

            if (ret == false)
            {
                // 面外周上になぁE
                // 面冁E��あるぁE一部また�E全佁E

                // 交差判宁E
                foreach (Revit.DB.Curve curve in curves)
                {
                    Collections.Generic.IList<Revit.DB.XYZ> intersects = new Collections.Generic.List<Revit.DB.XYZ>();

                    IntersecCurve(lShadow, curve, ref intersects);

                    if (intersects.Count > 0)
                    {
                        ret = true;
                        break;
                    }
                }
                // 完�E冁E��
                if (ret == false)
                {
                    Revit.DB.XYZ p = curves[0].GetEndPoint(0);

                    if (IsPointInPolygon(curves, p, pShadow0, 1) &&
                        IsPointInPolygon(curves, p, pShadow1, 1))
                    {
                        ret = true;
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>関連性判宁E- エチE��とエチE��</summary>
        /// 
        /// <param name="edge1"   >エチE��1</param>
        /// <param name="edge2"   >エチE��2</param>
        /// <param name="plnFace" >平面</param>
        /// 
        /// <history>2016/12/12 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool RelevantEdges(Revit.DB.Edge edge1,
                           Revit.DB.Edge edge2,
                           Revit.DB.PlanarFace plnFace)
        {
            // 戻り値
            bool ret = false;

            Revit.DB.Curve curve1 = edge1.AsCurve();
            Revit.DB.Curve curve2 = edge2.AsCurve();

            Revit.DB.XYZ p0 = curve1.GetEndPoint(0);
            Revit.DB.XYZ p1 = curve1.GetEndPoint(1);
            Revit.DB.XYZ p2 = curve2.GetEndPoint(0);
            Revit.DB.XYZ p3 = curve2.GetEndPoint(1);

            Revit.DB.XYZ pA = plnFace.Origin;
            Revit.DB.XYZ pB = pA + plnFace.XVector;
            Revit.DB.XYZ pC = pA + plnFace.YVector;
            Revit.DB.XYZ normal = plnFace.FaceNormal;

            Revit.DB.Line lShadow0 = Revit.DB.Line.CreateBound(p0, p0 + normal);
            Revit.DB.Line lShadow1 = Revit.DB.Line.CreateBound(p1, p1 + normal);
            Revit.DB.Line lShadow2 = Revit.DB.Line.CreateBound(p2, p2 + normal);
            Revit.DB.Line lShadow3 = Revit.DB.Line.CreateBound(p3, p3 + normal);

            // 投影点
            Revit.DB.XYZ pShadow0 = GetCrossPoint(lShadow0, pA, pB, pC, 1);
            Revit.DB.XYZ pShadow1 = GetCrossPoint(lShadow1, pA, pB, pC, 1);
            Revit.DB.XYZ pShadow2 = GetCrossPoint(lShadow2, pA, pB, pC, 1);
            Revit.DB.XYZ pShadow3 = GetCrossPoint(lShadow3, pA, pB, pC, 1);

            if (Distance(pShadow0, pShadow1) < Approx0Len ||
                Distance(pShadow2, pShadow3) < Approx0Len)
            {
                return ret;
            }

            // 面上�E線�E
            Revit.DB.Line line0 = Revit.DB.Line.CreateBound(pShadow0, pShadow1);
            Revit.DB.Line line1 = Revit.DB.Line.CreateBound(pShadow2, pShadow3);

            // すべての端点が離れてぁE��
            if ((Distance(pShadow0, pShadow2) > Approx0Len && Distance(pShadow0, pShadow3) > Approx0Len) &&
                (Distance(pShadow1, pShadow2) > Approx0Len && Distance(pShadow1, pShadow3) > Approx0Len))
            {
                return ret;
            }
            // 両端が同ぁE
            else if ((Distance(pShadow0, pShadow2) < Approx0Len && Distance(pShadow1, pShadow3) > Approx0Len) ||
                     (Distance(pShadow0, pShadow3) < Approx0Len && Distance(pShadow1, pShadow2) < Approx0Len))
            {
                return ret;
            }
            // 重なってぁE��
            else if (IsOnLine(line0, pShadow2, 0) ||
                     IsOnLine(line0, pShadow3, 0) ||
                     IsOnLine(line1, pShadow0, 0) ||
                     IsOnLine(line1, pShadow1, 0))
            {
                return ret;
            }

            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>同一カーブ判宁E/summary>
        /// 
        /// <param name="curve1">カーチE</param>
        /// <param name="curve2">カーチE</param>
        /// 
        /// <history>2016/12/07 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool EqualCurve(Revit.DB.Curve curve1,
                        Revit.DB.Curve curve2)
        {
            // 戻り値
            bool ret = false;

            // 始点終点
            if (IsEqualCurve(curve1, curve2))
            {
                double param0 = curve1.GetEndParameter(0);
                double param1 = curve1.GetEndParameter(1);
                double param2 = curve1.GetEndParameter(0);
                double param3 = curve1.GetEndParameter(1);

                if (System.Math.Abs(param0 - param2) < Approx0Ang &&
                    System.Math.Abs(param1 - param3) < Approx0Ang)
                {
                    if (System.Math.Abs(curve1.Length - curve2.Length) < Approx0Len)
                    {
                        if (curve1.Tessellate().Count == curve2.Tessellate().Count)
                        {
                            ret = true;
                        }
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>Solid取征E/summary>
        /// 
        /// <param name="elem">要素</param>
        /// 
        /// <history>2016/11/21 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Revit.DB.Solid> GetSolids(Revit.DB.Element elem)
        {
            // 戻り値
            Collections.Generic.IList<Revit.DB.Solid> ret = new Collections.Generic.List<Revit.DB.Solid>();

            Revit.DB.Options opt = elem.Document.Application.Create.NewGeometryOptions();

            Revit.DB.GeometryElement geomElem = elem.get_Geometry(opt);

            Collections.Generic.IEnumerator<Revit.DB.GeometryObject> geoObjEnum = geomElem.GetEnumerator();
            geoObjEnum.Reset();

            while (geoObjEnum.MoveNext())
            {
                Revit.DB.GeometryObject geoObj = geoObjEnum.Current;
                Revit.DB.GeometryInstance geoIns = geoObj as Revit.DB.GeometryInstance;

                if (geoIns != null)
                {
                    Revit.DB.FamilyInstance famIns = elem as Revit.DB.FamilyInstance;
                    if (famIns == null) return null;

                    Revit.DB.GeometryElement ge = geoIns.GetSymbolGeometry(famIns.GetTransform());
                    Collections.Generic.IEnumerator<Revit.DB.GeometryObject> goEnum = ge.GetEnumerator();
                    goEnum.Reset();

                    while (goEnum.MoveNext())
                    {
                        Revit.DB.GeometryObject go = goEnum.Current;

                        Revit.DB.Solid solid = go as Revit.DB.Solid;

                        if (solid != null)
                        {
                            ret.Add(solid);
                        }
                    }
                }
                else
                {
                    Revit.DB.Solid solid = geoObj as Revit.DB.Solid;

                    if (solid != null)
                    {
                        ret.Add(solid);
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>3点のなす面と線�Eの交点</summary>
        /// 
        /// <param name="line">線�E</param>
        /// <param name="pA"  >点A</param>
        /// <param name="pB"  >点B</param>
        /// <param name="pC"  >点C</param>
        /// <param name="modeExtend">線�E延長</param>
        /// 
        /// <history>2016/12/07 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Revit.DB.XYZ GetCrossPoint(Revit.DB.Line line,
                                   Revit.DB.XYZ pA,
                                   Revit.DB.XYZ pB,
                                   Revit.DB.XYZ pC,
                                   int modeExtend)
        {
            // 戻り値
            Revit.DB.XYZ ret = null;

            // 平面の定数
            double a = 0;
            double b = 0;
            double c = 0;
            double d = 0;

            // 平面の方程弁E
            GetPlaneEquation(pA,
                             pB,
                             pC,
                             ref a,
                             ref b,
                             ref c,
                             ref d);

            // 平面の法緁E
            Revit.DB.XYZ normal = new Revit.DB.XYZ(a, b, c);

            // 中忁E��(重忁E
            Revit.DB.XYZ pO = new Revit.DB.XYZ((pA.X + pB.X + pC.X) / 3,
                                               (pA.Y + pB.Y + pC.Y) / 3,
                                               (pA.Z + pB.Z + pC.Z) / 3);

            // 始点
            Revit.DB.XYZ p0 = line.GetEndPoint(0);
            // 終点
            Revit.DB.XYZ p1 = line.GetEndPoint(1);

            if (modeExtend == 1)
            {
                Revit.DB.XYZ vec = new Revit.DB.XYZ(p1.X - p0.X,
                                                    p1.Y - p0.Y,
                                                    p1.Z - p0.Z);

                p0 = p0 + vec * 1000000;
                p1 = p1 + -vec * 1000000;
            }

            // 中忁E��から始点へのベクトル
            Revit.DB.XYZ vecO0 = new Revit.DB.XYZ(p0.X - pO.X,
                                                  p0.Y - pO.Y,
                                                  p0.Z - pO.Z);
            // 中忁E��から終点へのベクトル
            Revit.DB.XYZ vecO1 = new Revit.DB.XYZ(p1.X - pO.X,
                                                  p1.Y - pO.Y,
                                                  p1.Z - pO.Z);

            // 法線との冁E��E
            // 0は面丁E同じ方向への働きなぁE、正は同じ向き、負は送E��ぁE
            double dotN0 = Naiseki(vecO0, normal);
            double dotN1 = Naiseki(vecO1, normal);

            // 誤差吸収（�E琁E��る桁�E適宜調整�E�E
            if (System.Math.Abs(dotN0) < 0.000001)
            {
                dotN0 = 0.0;
            }
            if (System.Math.Abs(dotN1) < 0.000001)
            {
                dotN1 = 0.0;
            }

            // 両端が平面丁E
            if (dotN0 == 0.0 && dotN1 == 0.0)
            {
                return ret;
            }
            else
            {
                // 端点が平面の牁E�Eずつ
                if ((dotN0 >= 0.0 && dotN1 <= 0.0) || (dotN0 <= 0.0 && dotN1 >= 0.0))
                {
                    // 始点から終点へ
                    Revit.DB.XYZ vec01 = new Revit.DB.XYZ(p1.X - p0.X,
                                                          p1.Y - p0.Y,
                                                          p1.Z - p0.Z);

                    // 冁E���E毁E= 距離の毁E
                    double dotRate = System.Math.Abs(dotN0) / (System.Math.Abs(dotN0) + System.Math.Abs(dotN1));

                    // 平面との交点
                    Revit.DB.XYZ pCross = new Revit.DB.XYZ(p0.X + vec01.X * dotRate,
                                                           p0.Y + vec01.Y * dotRate,
                                                           p0.Z + vec01.Z * dotRate);

                    ret = pCross;
                }
                // 両点が平面の同じ側
                else
                {
                    return ret;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>平面の方程弁E/summary>
        /// 
        /// <param name="pA">点A</param>
        /// <param name="pB">点B</param>
        /// <param name="pC">点C</param>
        /// <param name="a" >定数a</param>
        /// <param name="b" >定数b</param>
        /// <param name="c" >定数c</param>
        /// <param name="d" >定数d</param>
        /// 
        /// <history>2016/12/07 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetPlaneEquation(Revit.DB.XYZ pA,
                              Revit.DB.XYZ pB,
                              Revit.DB.XYZ pC,
                              ref double a,
                              ref double b,
                              ref double c,
                              ref double d)
        {
            // ax + by + cz + d = 0

            a = (pB.Y - pA.Y) * (pC.Z - pA.Z) - (pC.Y - pA.Y) * (pB.Z - pA.Z);
            b = (pB.Z - pA.Z) * (pC.X - pA.X) - (pC.Z - pA.Z) * (pB.X - pA.X);
            c = (pB.X - pA.X) * (pC.Y - pA.Y) - (pC.X - pA.X) * (pB.Y - pA.Y);
            d = -(a * pA.X + b * pA.Y + c * pA.Z);
        }

        /// ================================================================================
        /// <summary>Cosシータ</summary>
        /// 
        /// <param name="vec1">ベクトル1</param>
        /// <param name="vec2">ベクトル2</param>
        /// 
        /// <history>2016/11/28 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        double CosSita(Revit.DB.XYZ vec1, Revit.DB.XYZ vec2)
        {
            // 戻り値
            double ret = 0;

            // 2ベクトルの冁E��E
            double naiseki = Naiseki(vec1, vec2);

            // ベクトルの長ぁE
            double length1 = System.Math.Sqrt(vec1.X * vec1.X + vec1.Y * vec1.Y + vec1.Z * vec1.Z);
            double length2 = System.Math.Sqrt(vec2.X * vec2.X + vec2.Y * vec2.Y + vec2.Z * vec2.Z);

            ret = naiseki / (length1 * length2);

            return ret;
        }

        /// ================================================================================
        /// <summary>冁E��E/summary>
        /// 
        /// <param name="vec1">ベクトル1</param>
        /// <param name="vec2">ベクトル2</param>
        /// 
        /// <history>2016/11/18 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        double Naiseki(Revit.DB.XYZ vec1, Revit.DB.XYZ vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z * vec2.Z;
        }

        /// ================================================================================
        /// <summary>外穁E/summary>
        /// 
        /// <param name="vec1">ベクトル1</param>
        /// <param name="vec2">ベクトル2</param>
        /// 
        /// <history>2016/11/18 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Revit.DB.XYZ Gaiseki(Revit.DB.XYZ vec1, Revit.DB.XYZ vec2)
        {
            return new Revit.DB.XYZ(vec1.Y * vec2.Z - vec1.Z * vec2.Y,
                                    vec1.Z * vec2.X - vec1.X * vec2.Z,
                                    vec1.X * vec2.Y - vec1.Y * vec2.X);
        }
        /// ================================================================================
        /// <summary>面カーブ取征E/summary>
        /// 
        /// <param name="plnFace">平面</param>
        /// 
        /// <history>2016/12/21 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Revit.DB.Curve> GetFaceCurvesBase(Revit.DB.PlanarFace plnFace)
        {
            // 戻り値
            Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();

            // ループ線�E
            Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = plnFace.GetEdgesAsCurveLoops();

            // 褁E��形状
            if (crvLoops.Count > 1)
            {
                return ret;
            }

            // 曲線を含むぁE
            bool isCyclic = false;

            Collections.Generic.IList<Revit.DB.Curve> curves = new Collections.Generic.List<Revit.DB.Curve>();
            foreach (Revit.DB.CurveLoop crvLoop in crvLoops)
            {
                foreach (Revit.DB.Curve curve in crvLoop)
                {
                    curves.Add(curve);

                    isCyclic = curve.IsCyclic;
                }
            }

            if (isCyclic)
            {
                return ret;
            }

            ret = GetCurvesBase(curves);



            return ret;
        }

        /// ================================================================================
        /// <summary>カーブ取征E/summary>
        /// 
        /// <param name="curves">カーチE/param>
        /// 
        /// <history>2016/12/21 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Revit.DB.Curve> GetCurvesBase(Collections.Generic.IList<Revit.DB.Curve> curves)
        {
            // 戻り値
            Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();

            //長ぁE��を検索
            Revit.DB.Curve leftbtm = null;

            //平行辺の長ぁE��ぁE��検索
            for (int i = 0; i < 2; ++i)
            {
                int j = i + 2;

                Revit.DB.Curve curve1 = curves[i];
                Revit.DB.Curve curve2 = curves[j];

                Revit.DB.XYZ p0 = curve1.GetEndPoint(0);
                Revit.DB.XYZ p1 = curve1.GetEndPoint(1);
                Revit.DB.XYZ pp1 = new Revit.DB.XYZ(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
                double len1 = System.Math.Sqrt(pp1.X * pp1.X + pp1.Y * pp1.Y + pp1.Z * pp1.Z);

                Revit.DB.XYZ p2 = curve2.GetEndPoint(0);
                Revit.DB.XYZ p3 = curve2.GetEndPoint(1);
                Revit.DB.XYZ pp2 = new Revit.DB.XYZ(p3.X - p2.X, p3.Y - p2.Y, p3.Z - p2.Z);
                double len2 = System.Math.Sqrt(pp2.X * pp2.X + pp2.Y * pp2.Y + pp2.Z * pp2.Z);

                Revit.DB.XYZ uv1 = UnitVector(p0, p1);
                Revit.DB.XYZ uv2 = UnitVector(p2, p3);

                Revit.DB.XYZ crsP = Gaiseki(uv1, uv2);
                double crs = Math.Sqrt(crsP.X * crsP.X + crsP.Y * crsP.Y + crsP.Z * crsP.Z);
                if (System.Math.Abs(crs) < Approx0Len)
                {
                    leftbtm = (len1 > len2) ? curve1 : curve2;
                    break;
                }

            }

            if (leftbtm == null)
            {
                leftbtm = GetLeftBottomCurve(curves, 0);
            }

            // 連続するカーチE
            GetContinuousCurves(curves, leftbtm, ref ret);

            return ret;
        }

        /// ================================================================================
        /// <summary>L型　面カーブ取征E/summary>
        /// 
        /// <param name="plnFace">平面</param>
        /// 
        /// <history>2016/12/21 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Revit.DB.Curve> GetFaceCurvesLType(Revit.DB.PlanarFace plnFace)
        {
            // 戻り値
            Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();

            // ループ線�E
            Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = plnFace.GetEdgesAsCurveLoops();

            // 褁E��形状
            if (crvLoops.Count > 1)
            {
                return ret;
            }

            // 曲線を含むぁE
            bool isCyclic = false;

            Collections.Generic.IList<Revit.DB.Curve> curves = new Collections.Generic.List<Revit.DB.Curve>();
            foreach (Revit.DB.CurveLoop crvLoop in crvLoops)
            {
                foreach (Revit.DB.Curve curve in crvLoop)
                {
                    curves.Add(curve);

                    isCyclic = curve.IsCyclic;
                }
            }

            if (isCyclic)
            {
                return ret;
            }

            ret = GetCurvesLType(curves);


            return ret;
        }
        /// ================================================================================
        /// <summary>L型　カーブ取征E/summary>
        /// 
        /// <param name="curves">カーチE/param>
        /// 
        /// <history>2016/12/21 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Revit.DB.Curve> GetCurvesLType(Collections.Generic.IList<Revit.DB.Curve> curves)
        {
            // 戻り値
            Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();

            //調整
            curves = OptimizeLineVertexNoConvLine(curves);



            // _____
            // |    |
            // |    |
            // |    |____
            // |         |
            // |_________|
            //
            //    ↑この線�Eを検索する


            double maxLen = -double.MaxValue;
            int indx = -1;
            for (int i = 0; i < curves.Count; ++i)
            {
                Revit.DB.Curve curve1 = curves[i];
                Revit.DB.Curve curve2 = null;

                if (i == curves.Count - 1)
                {
                    curve2 = curves[0];
                }
                else
                {
                    curve2 = curves[i + 1];
                }

                Revit.DB.XYZ p0 = curve1.GetEndPoint(0);
                Revit.DB.XYZ p1 = curve1.GetEndPoint(1);
                Revit.DB.XYZ pp1 = new Revit.DB.XYZ(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
                double len1 = System.Math.Sqrt(pp1.X * pp1.X + pp1.Y * pp1.Y + pp1.Z * pp1.Z);

                Revit.DB.XYZ p2 = curve2.GetEndPoint(0);
                Revit.DB.XYZ p3 = curve2.GetEndPoint(1);
                Revit.DB.XYZ pp2 = new Revit.DB.XYZ(p3.X - p2.X, p3.Y - p2.Y, p3.Z - p2.Z);
                double len2 = System.Math.Sqrt(pp2.X * pp2.X + pp2.Y * pp2.Y + pp2.Z * pp2.Z);

                double len = len1 + len2;
                if (len > maxLen)
                {
                    maxLen = len;
                    indx = i;
                }

                Revit.DB.XYZ uv1 = UnitVector(p0, p1);
                Revit.DB.XYZ uv2 = UnitVector(p2, p3);

                Revit.DB.XYZ crs = Gaiseki(uv1, uv2);
            }

            Revit.DB.Curve leftbtm = null;
            if (indx > -1)
            {
                leftbtm = curves[(indx + 1) % curves.Count];
            }
            else
            {
                return ret;
            }


            // 連続するカーチE
            GetContinuousCurves(curves, leftbtm, ref ret);

            return ret;
        }

        /// ================================================================================
        /// <summary>T型　面カーブ取征E/summary>
        /// 
        /// <param name="plnFace">平面</param>
        /// 
        /// <history>2016/12/21 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Revit.DB.Curve> GetFaceCurvesTType(Revit.DB.PlanarFace plnFace)
        {
            // 戻り値
            Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();

            // ループ線�E
            Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = plnFace.GetEdgesAsCurveLoops();

            // 褁E��形状
            if (crvLoops.Count > 1)
            {
                return ret;
            }

            // 曲線を含むぁE
            bool isCyclic = false;

            Collections.Generic.IList<Revit.DB.Curve> curves = new Collections.Generic.List<Revit.DB.Curve>();
            foreach (Revit.DB.CurveLoop crvLoop in crvLoops)
            {
                foreach (Revit.DB.Curve curve in crvLoop)
                {
                    curves.Add(curve);

                    isCyclic = curve.IsCyclic;
                }
            }

            if (isCyclic)
            {
                return ret;
            }

            ret = GetCurvesTType(curves);


            return ret;
        }

        /// ================================================================================
        /// <summary>T型　カーブ取征E/summary>
        /// 
        /// <param name="curves">カーチE/param>
        /// 
        /// <history>2016/12/21 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Revit.DB.Curve> GetCurvesTType(Collections.Generic.IList<Revit.DB.Curve> curves)
        {
            // 戻り値
            Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();


            //調整
            curves = OptimizeLineVertexNoConvLine(curves);

            //      _____
            //      |    |
            //      |    |
            // _____|    |____
            // |              |
            // |______________|
            //
            //        ↑この線�Eを検索する


            int indx = -1;
            for (int i = 0; i < curves.Count; ++i)
            {
                bool bVec = false;
                Revit.DB.XYZ p0 = curves[i].GetEndPoint(0);
                Revit.DB.XYZ p1 = curves[i].GetEndPoint(1);
                Revit.DB.XYZ uv1 = UnitVector(p0, p1);
                for (int j = 0; j < curves.Count; ++j)
                {
                    if (i == j) continue;

                    Revit.DB.XYZ p2 = curves[j].GetEndPoint(0);
                    Revit.DB.XYZ p3 = curves[j].GetEndPoint(1);
                    Revit.DB.XYZ uv2 = UnitVector(p2, p3);

                    //同じ方向�Eクトルがあるか調査
                    //�E�この線�Eを検索する�E��Eベクトルは「対」がなぁE
                    Revit.DB.XYZ crsP = Gaiseki(uv1, uv2);
                    double crs = Math.Sqrt(crsP.X * crsP.X + crsP.Y * crsP.Y + crsP.Z * crsP.Z);
                    if (System.Math.Abs(crs) < Approx0Len)
                    {
                        double dot = Naiseki(uv1, uv2);
                        if (dot > 0.0)
                        {
                            bVec = true;
                            break;
                        }
                    }
                }
                if (!bVec)
                {
                    indx = i;
                    break;
                }
            }

            Revit.DB.Curve leftbtm = null;
            if (indx > -1)
            {
                leftbtm = curves[indx];
            }
            else
            {
                return ret;
            }

            // 連続するカーチE
            GetContinuousCurves(curves, leftbtm, ref ret);

            return ret;
        }


        /// ================================================================================
        /// <summary>同一面検索</summary>
        /// 
        /// <param name="elem">要素</param>
        /// <param name="plane">面</param>
        /// 
        /// <history>2016/12/07 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Revit.DB.PlanarFace GetSamePlnFace(Revit.DB.Element elem, Revit.DB.PlanarFace plane)
        {
            // 戻り値
            Revit.DB.XYZ vn = plane.FaceNormal;
            double area = plane.Area;

            Revit.DB.Options opt = elem.Document.Application.Create.NewGeometryOptions();
            opt.ComputeReferences = true;

            Revit.DB.GeometryElement geomElem = elem.get_Geometry(opt);

            Collections.Generic.IEnumerator<Revit.DB.GeometryObject> geoObjEnum = geomElem.GetEnumerator();
            geoObjEnum.Reset();

            while (geoObjEnum.MoveNext())
            {
                Revit.DB.GeometryObject geoObj = geoObjEnum.Current;
                Revit.DB.GeometryInstance geoIns = geoObj as Revit.DB.GeometryInstance;

                if (geoIns != null)
                {
                    Revit.DB.FamilyInstance famIns = elem as Revit.DB.FamilyInstance;
                    if (famIns == null) return null;

                    Revit.DB.GeometryElement ge = geoIns.GetSymbolGeometry(famIns.GetTransform());
                    Collections.Generic.IEnumerator<Revit.DB.GeometryObject> goEnum = ge.GetEnumerator();
                    goEnum.Reset();

                    while (goEnum.MoveNext())
                    {
                        Revit.DB.GeometryObject go = goEnum.Current;

                        Revit.DB.Solid solid = go as Revit.DB.Solid;

                        if (solid != null)
                        {
                            foreach (Revit.DB.Face face in solid.Faces)
                            {
                                Revit.DB.PlanarFace plnFace = face as Revit.DB.PlanarFace;

                                if (plnFace != null)
                                {
                                    if (System.Math.Abs(area - plnFace.Area) < Approx0Len)
                                    {
                                        double dot = Naiseki(vn, plnFace.FaceNormal);
                                        if (dot > 1.0 - Approx0Len)
                                        {
                                            return plnFace;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Revit.DB.Solid solid = geoObj as Revit.DB.Solid;

                    if (solid != null)
                    {
                        foreach (Revit.DB.Face face in solid.Faces)
                        {
                            Revit.DB.PlanarFace plnFace = face as Revit.DB.PlanarFace;

                            if (plnFace != null)
                            {
                                if (System.Math.Abs(area - plnFace.Area) < Approx0Len)
                                {
                                    double dot = Naiseki(vn, plnFace.FaceNormal);
                                    if (dot > 1.0 - Approx0Len)
                                    {
                                        return plnFace;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// ================================================================================
        /// <summary>エチE��が含まれる面</summary>
        /// 
        /// <param name="elem">要素</param>
        /// <param name="edge">エチE��</param>
        /// <param name="normal">面法緁E/param>
        /// 
        /// <history>2016/01/17 Created  GSA,Inc. Hideki Sudo</history>
        /// ================================================================================
        public
        Revit.DB.Edge GetSamePlnEdge(Revit.DB.Element elem, Revit.DB.Edge edge, Revit.DB.XYZ normal)
        {
            Revit.DB.Curve curve = edge.AsCurve();
            double len = curve.Length;
            Revit.DB.XYZ uv1 = UnitVector(curve.GetEndPoint(0), curve.GetEndPoint(1));


            Revit.DB.Options opt = elem.Document.Application.Create.NewGeometryOptions();
            opt.ComputeReferences = true;

            Revit.DB.GeometryElement geomElem = elem.get_Geometry(opt);

            Collections.Generic.IEnumerator<Revit.DB.GeometryObject> geoObjEnum = geomElem.GetEnumerator();
            geoObjEnum.Reset();

            while (geoObjEnum.MoveNext())
            {
                Revit.DB.GeometryObject geoObj = geoObjEnum.Current;
                Revit.DB.GeometryInstance geoIns = geoObj as Revit.DB.GeometryInstance;

                if (geoIns != null)
                {
                    Revit.DB.FamilyInstance famIns = elem as Revit.DB.FamilyInstance;
                    if (famIns == null) return null;

                    Revit.DB.GeometryElement ge = geoIns.GetSymbolGeometry(famIns.GetTransform());
                    Collections.Generic.IEnumerator<Revit.DB.GeometryObject> goEnum = ge.GetEnumerator();
                    goEnum.Reset();

                    while (goEnum.MoveNext())
                    {
                        Revit.DB.GeometryObject go = goEnum.Current;

                        Revit.DB.Solid solid = go as Revit.DB.Solid;

                        if (solid != null)
                        {
                            foreach (Revit.DB.Face face in solid.Faces)
                            {
                                Revit.DB.PlanarFace plnFace = face as Revit.DB.PlanarFace;

                                if (plnFace != null)
                                {
                                    double dotP = Naiseki(normal, plnFace.FaceNormal);
                                    if (!(dotP > 1.0 - Approx0Len))
                                    {
                                        continue;
                                    }

                                    foreach (Revit.DB.EdgeArray edgeAry in plnFace.EdgeLoops)
                                    {
                                        foreach (Revit.DB.Edge e in edgeAry)
                                        {
                                            Revit.DB.Curve c = e.AsCurve();
                                            if (System.Math.Abs(len - c.Length) < Approx0Len)
                                            {
                                                Revit.DB.XYZ uv2 = UnitVector(c.GetEndPoint(0), c.GetEndPoint(1));
                                                double dot = Naiseki(uv1, uv2);
                                                if (dot > 1.0 - Approx0Len)
                                                {
                                                    return e;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Revit.DB.Solid solid = geoObj as Revit.DB.Solid;

                    if (solid != null)
                    {
                        foreach (Revit.DB.Face face in solid.Faces)
                        {
                            Revit.DB.PlanarFace plnFace = face as Revit.DB.PlanarFace;

                            if (plnFace != null)
                            {
                                double dotP = Naiseki(normal, plnFace.FaceNormal);
                                if (!(dotP > 1.0 - Approx0Len))
                                {
                                    continue;
                                }

                                foreach (Revit.DB.EdgeArray edgeAry in plnFace.EdgeLoops)
                                {
                                    foreach (Revit.DB.Edge e in edgeAry)
                                    {
                                        Revit.DB.Curve c = e.AsCurve();
                                        if (System.Math.Abs(len - c.Length) < Approx0Len)
                                        {
                                            Revit.DB.XYZ uv2 = UnitVector(c.GetEndPoint(0), c.GetEndPoint(1));
                                            double dot = Naiseki(uv1, uv2);
                                            if (dot > 1.0 - Approx0Len)
                                            {
                                                return e;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return GetSamePlnEdge(elem, edge);
        }

        /// ================================================================================
        /// <summary>エチE��が含まれる面</summary>
        /// 
        /// <param name="elem">要素</param>
        /// <param name="edge">エチE��</param>
        /// 
        /// <history>2016/01/17 Created  GSA,Inc. Hideki Sudo</history>
        /// ================================================================================
        public
        Revit.DB.Edge GetSamePlnEdge(Revit.DB.Element elem, Revit.DB.Edge edge)
        {
            Revit.DB.Curve curve = edge.AsCurve();
            double len = curve.Length;
            Revit.DB.XYZ uv1 = UnitVector(curve.GetEndPoint(0), curve.GetEndPoint(1));


            Revit.DB.Options opt = elem.Document.Application.Create.NewGeometryOptions();
            opt.ComputeReferences = true;

            Revit.DB.GeometryElement geomElem = elem.get_Geometry(opt);

            Collections.Generic.IEnumerator<Revit.DB.GeometryObject> geoObjEnum = geomElem.GetEnumerator();
            geoObjEnum.Reset();

            while (geoObjEnum.MoveNext())
            {
                Revit.DB.GeometryObject geoObj = geoObjEnum.Current;
                Revit.DB.GeometryInstance geoIns = geoObj as Revit.DB.GeometryInstance;

                if (geoIns != null)
                {
                    Revit.DB.FamilyInstance famIns = elem as Revit.DB.FamilyInstance;
                    if (famIns == null) return null;
                    Revit.DB.GeometryElement ge = geoIns.GetSymbolGeometry(famIns.GetTransform());
                    Collections.Generic.IEnumerator<Revit.DB.GeometryObject> goEnum = ge.GetEnumerator();
                    goEnum.Reset();

                    while (goEnum.MoveNext())
                    {
                        Revit.DB.GeometryObject go = goEnum.Current;

                        Revit.DB.Solid solid = go as Revit.DB.Solid;

                        if (solid != null)
                        {
                            foreach (Revit.DB.Face face in solid.Faces)
                            {
                                Revit.DB.PlanarFace plnFace = face as Revit.DB.PlanarFace;

                                if (plnFace != null)
                                {
                                    foreach (Revit.DB.EdgeArray edgeAry in plnFace.EdgeLoops)
                                    {
                                        foreach (Revit.DB.Edge e in edgeAry)
                                        {
                                            Revit.DB.Curve c = e.AsCurve();
                                            if (System.Math.Abs(len - c.Length) < Approx0Len)
                                            {
                                                Revit.DB.XYZ uv2 = UnitVector(c.GetEndPoint(0), c.GetEndPoint(1));
                                                double dot = Naiseki(uv1, uv2);
                                                if (dot > 1.0 - Approx0Len)
                                                {
                                                    return e;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Revit.DB.Solid solid = geoObj as Revit.DB.Solid;

                    if (solid != null)
                    {
                        foreach (Revit.DB.Face face in solid.Faces)
                        {
                            Revit.DB.PlanarFace plnFace = face as Revit.DB.PlanarFace;

                            if (plnFace != null)
                            {
                                foreach (Revit.DB.EdgeArray edgeAry in plnFace.EdgeLoops)
                                {
                                    foreach (Revit.DB.Edge e in edgeAry)
                                    {
                                        Revit.DB.Curve c = e.AsCurve();
                                        if (System.Math.Abs(len - c.Length) < Approx0Len)
                                        {
                                            Revit.DB.XYZ uv2 = UnitVector(c.GetEndPoint(0), c.GetEndPoint(1));
                                            double dot = Naiseki(uv1, uv2);
                                            if (dot > 1.0 - Approx0Len)
                                            {
                                                return e;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }


        /// ================================================================================
        /// <summary>面と面の関俁E/summary>
        /// 
        /// <param name="planeB">基準面</param>
        /// <param name="planeH">対象面</param>
        /// 
        /// <history>2016/01/17 Created  GSA,Inc. Hideki Sudo</history>
        /// ================================================================================
        public
        bool PlaneFaceRel(Revit.DB.PlanarFace planeB, Revit.DB.PlanarFace planeH)
        {
            Revit.DB.XYZ vv = UnitVector(planeB.Origin, planeH.Origin);

            double dot = Naiseki(planeB.FaceNormal, vv);
            if (dot < 0.0)
            {
                //ふかすと埋まってしまぁE
                return false;
            }

            return true;

        }



        /// ================================================================================
        /// <summary>ソリチE��リスト取征E/summary>
        /// 
        /// <param name="elem">要素</param>
        /// <param name="solidList">ソリチE��リスチE/param>
        /// 
        /// <history>2017/2/03 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public
        void GetElemSolid(Revit.DB.Element elem, ref Collections.Generic.IList<Revit.DB.Solid> solidList)
        {
            Revit.DB.Options opt = elem.Document.Application.Create.NewGeometryOptions();
            opt.ComputeReferences = true;

            Revit.DB.GeometryElement geomElem = elem.get_Geometry(opt);

            Collections.Generic.IEnumerator<Revit.DB.GeometryObject> geoObjEnum = geomElem.GetEnumerator();
            geoObjEnum.Reset();

            while (geoObjEnum.MoveNext())
            {
                Revit.DB.GeometryObject geoObj = geoObjEnum.Current;

                Revit.DB.Solid solid = geoObj as Revit.DB.Solid;
                if (solid != null)
                {
                    if (solid.Volume > 0.0)
                    {
                        solidList.Add(solid);
                    }
                }
                else
                {
                    Revit.DB.FamilyInstance famIns = elem as Revit.DB.FamilyInstance;
                    if (famIns != null)
                    {
                        Revit.DB.GeometryInstance geoIns = geoObj as Revit.DB.GeometryInstance;
                        if (geoIns != null)
                        {
                            Revit.DB.GeometryElement ge = geoIns.GetSymbolGeometry(famIns.GetTransform());
                            Collections.Generic.IEnumerator<Revit.DB.GeometryObject> goEnum = ge.GetEnumerator();
                            goEnum.Reset();
                            while (goEnum.MoveNext())
                            {
                                Revit.DB.GeometryObject go = goEnum.Current;

                                Revit.DB.Solid solid2 = go as Revit.DB.Solid;

                                if (solid2 != null && solid2.Volume > 0.0)
                                {
                                    solidList.Add(solid2);
                                }
                            }
                        }
                    }
                }
            }
        }
        /// ================================================================================
        /// <summary>合�E後�E面検索</summary>
        /// 
        /// <param name="elem1">要素</param>
        /// <param name="elem2">要素</param>
        /// <param name="pickPos">ピック点</param>
        /// 
        /// <history>2017/2/03 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public
        Revit.DB.PlanarFace GetSolidFace(Revit.DB.Element elem1, Collections.Generic.IList<Revit.DB.Element> elemList, Revit.DB.XYZ pickPos)
        {
            Revit.DB.PlanarFace retPFace = null;
            Collections.Generic.IList<Revit.DB.Solid> solidList2 = new Collections.Generic.List<Revit.DB.Solid>();
            GetElemSolid(elem1, ref solidList2);

            foreach (Revit.DB.Element elem2 in elemList)
            {
                GetElemSolid(elem2, ref solidList2);
            }

            Revit.DB.Solid baseSolid = (Revit.DB.BooleanOperationsUtils.ExecuteBooleanOperation(solidList2[0], solidList2[1], Revit.DB.BooleanOperationsType.Union));
            for (int ii = 2; ii < solidList2.Count; ii++)
            {
                baseSolid = (Revit.DB.BooleanOperationsUtils.ExecuteBooleanOperation(baseSolid, solidList2[ii], Revit.DB.BooleanOperationsType.Union));
            }


            // 結果面
            foreach (Revit.DB.Face geomFace in baseSolid.Faces)
            {
                Revit.DB.PlanarFace geomPFace = geomFace as Revit.DB.PlanarFace;
                if (geomPFace == null)
                {
                    continue;
                }
                Revit.DB.IntersectionResult interRet = geomPFace.Project(pickPos);
                if (interRet == null)
                    continue;

                if (interRet.Distance < Approx0Len)
                {
                    retPFace = geomPFace;
                    break;
                }
            }
            return retPFace;
        }

        /// ================================================================================
        /// <summary>ソリチE��合�E</summary>
        /// 
        /// <param name="refList">要素ref</param>
        /// 
        /// <history>2017/2/03 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public
        Revit.DB.Solid GetUnionSolid(Collections.Generic.ICollection<Revit.DB.Reference> refList)
        {

            Collections.Generic.IList<Revit.DB.Solid> solidList2 = new Collections.Generic.List<Revit.DB.Solid>();
            foreach (Revit.DB.Reference refe in refList)
            {
                Revit.DB.Element elem2 = RvtDBDoc.GetElement(refe);

                GetElemSolid(elem2, ref solidList2);
            }
            if (solidList2.Count == 1)
            {
                return solidList2[0];
            }

            Revit.DB.Solid baseSolid = null;
            try
            {
                baseSolid = (Revit.DB.BooleanOperationsUtils.ExecuteBooleanOperation(solidList2[0], solidList2[1], Revit.DB.BooleanOperationsType.Union));
                for (int ii = 2; ii < solidList2.Count; ii++)
                {
                    baseSolid = (Revit.DB.BooleanOperationsUtils.ExecuteBooleanOperation(baseSolid, solidList2[ii], Revit.DB.BooleanOperationsType.Union));
                }
            }
            catch
            {
                baseSolid = null;
            }
            return baseSolid;
        }
        /// ================================================================================
        /// <summary>ソリチE��合�E</summary>
        /// 
        /// <param name="baseSolid">ソリチE��</param>
        /// <param name="baseSolid">検索点</param>
        /// 
        /// <history>2017/2/03 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public
        Revit.DB.PlanarFace SeachPickFace(Revit.DB.Solid baseSolid, Revit.DB.XYZ pickPos)
        {

            Revit.DB.PlanarFace retPFace = null;
            foreach (Revit.DB.Face geomFace in baseSolid.Faces)
            {
                Revit.DB.PlanarFace geomPFace = geomFace as Revit.DB.PlanarFace;
                if (geomPFace == null)
                {
                    continue;
                }
                Revit.DB.IntersectionResult interRet = geomPFace.Project(pickPos);
                if (interRet == null)
                    continue;

                if (interRet.Distance < Approx0Len)
                {
                    retPFace = geomPFace;
                    break;
                }
            }
            return retPFace;

        }

        /// ================================================================================
        /// <summary>ソリチE��交差3</summary>
        /// 
        /// <returns>結果</returns>
        /// 
        /// <history>2017/02/06 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public
        bool SolidIntersect3(Revit.DB.Solid unionSolid, Revit.DB.Reference subjRef, Revit.DB.XYZ subjPos,
            ref Collections.Generic.IList<Revit.DB.Curve> retCv,
            ref Revit.DB.PlanarFace retPln)
        {
            // 戻り値
            bool ret = false;

            Revit.DB.Element subjElem = RvtDBDoc.GetElement(subjRef);
            Revit.DB.PlanarFace subjPFace = subjElem.GetGeometryObjectFromReference(subjRef) as Revit.DB.PlanarFace;
            Revit.DB.XYZ subjNormal = subjPFace.FaceNormal;

            // 相手面
            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> oopoCrvAryAry = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.Curve>>();
            Collections.Generic.IList<Revit.DB.Curve> oopoCrvAry = new Collections.Generic.List<Revit.DB.Curve>();
            Collections.Generic.IList<Revit.DB.PlanarFace> oopoPFaceAry = new Collections.Generic.List<Revit.DB.PlanarFace>();
            foreach (Revit.DB.Face geomFace in unionSolid.Faces)
            {
                Revit.DB.PlanarFace geomPFace = geomFace as Revit.DB.PlanarFace;
                if (geomPFace != null)
                {
                    Collections.Generic.IList<Revit.DB.CurveLoop> geomCrvLoopAry = geomPFace.GetEdgesAsCurveLoops();
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
                if (Distance(subjNormal, normPFace) < Approx0Len)
                {
                    Revit.DB.IntersectionResult interRet = subjPFace.Project(orgPFace);
                    if (interRet == null)
                        continue;
                    if (interRet.Distance < Approx0Len)
                    {
                        srcCrvAryAry.Add(srcCrvAry);
                        srcPfaceAry.Add(pFace);
                    }
                }
            }
            if (srcCrvAryAry.Count == 0)
            {
                return ret;
            }

            // 相手カーブ決宁E
            Collections.Generic.IList<Revit.DB.Curve> retCurveAry = null;
            Revit.DB.PlanarFace retPface = null;
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
                        dist = Distance(subjPos, srcCrvAry[j].GetEndPoint(0));
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
                retCurveAry = srcCrvAryAry[idx];
                retPface = srcPfaceAry[idx];

            }
            else
            {
                retCurveAry = srcCrvAryAry[0];
                retPface = srcPfaceAry[0];
            }

            //DEBUG
            //Revit.DB.WireframeBuilder wb = new Revit.DB.WireframeBuilder();
            //foreach (Revit.DB.Curve cv in retCurveAry)
            //{
            //    wb.AddCurve(cv);
            //}
            //using (Revit.DB.Transaction t = new Revit.DB.Transaction(RvtDBDoc, "Create tessellated direct shape"))
            //{

            //    t.Start();
            //    Revit.DB.DirectShape ds1 = Revit.DB.DirectShape.CreateElement(RvtDBDoc, new Revit.DB.ElementId(Revit.DB.BuiltInCategory.OST_GenericModel),
            //                                "Application id",
            //                                "Geometry object id");
            //    ds1.SetShape(wb);

            //    Revit.DB.DirectShapeOptions dsOptions = ds1.GetOptions();
            //    dsOptions.ReferencingOption = Revit.DB.DirectShapeReferencingOption.Referenceable;
            //    ds1.SetOptions(dsOptions);

            //    t.Commit();
            //}
            //DEBUG

            retCv = retCurveAry;
            retPln = retPface;
            return ret;
        }

        /// ================================================================================
        /// <summary>Min点取征E/summary>
        /// 
        /// <param name="cvList">カーチE/param>
        /// 
        /// <returns>結果</returns>
        /// 
        /// <history>2017/02/06 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public Revit.DB.XYZ GetMinPoint(Collections.Generic.IList<Revit.DB.Curve> cvList)
        {
            double mx = Double.MaxValue;
            double my = Double.MaxValue;
            double mz = Double.MaxValue;
            foreach (Revit.DB.Curve cv in cvList)
            {
                Revit.DB.XYZ p = cv.GetEndPoint(0);
                if (p.X < mx) mx = p.X;
                if (p.Y < my) my = p.Y;
                if (p.Z < mz) mz = p.Z;
            }

            return new Revit.DB.XYZ(mx, my, mz);

        }
        /// ================================================================================
        /// <summary>中央点取征E/summary>
        /// 
        /// <param name="cvList">カーチE/param>
        /// 
        /// <returns>結果</returns>
        /// 
        /// <history>2017/02/06 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public Revit.DB.XYZ GetCenterPoint(Collections.Generic.IList<Revit.DB.Curve> cvList)
        {
            Revit.DB.XYZ ret = new Revit.DB.XYZ(0, 0, 0);
            if (cvList.Count != 4)
                return null;

            return (new Revit.DB.XYZ((cvList[0].GetEndPoint(0).X + cvList[2].GetEndPoint(0).X) * 0.5,
                                     (cvList[0].GetEndPoint(0).Y + cvList[2].GetEndPoint(0).Y) * 0.5,
                                     (cvList[0].GetEndPoint(0).Z + cvList[2].GetEndPoint(0).Z) * 0.5));
        }
        /// ================================================================================
        /// <summary>カーブ並べ替ぁE/summary>
        /// 
        /// <param name="curves">カーチE/param>
        /// 
        /// <returns>結果</returns>
        /// 
        /// <history>2017/02/06 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public void ContinuousCurves(ref Collections.Generic.IList<Revit.DB.Curve> curves)
        {

            Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();
            Revit.DB.Curve leftbtm = GetLeftBottomCurve(curves, 0);

            // 連続するカーチE
            GetContinuousCurves(curves, leftbtm, ref ret);
            curves = ret;
        }
        /// ================================================================================
        /// <summary>面カーブ並べ替ぁE/summary>
        /// 
        /// <param name="plnFace">面</param>
        /// <param name="curves">カーチE/param>
        /// 
        /// <returns>結果</returns>
        /// 
        /// <history>2017/02/06 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public Collections.Generic.IList<Revit.DB.Curve> ContinuousCurves(Revit.DB.PlanarFace plnFace, Collections.Generic.IList<Revit.DB.Curve> curves)
        {
            Collections.Generic.IList<Revit.DB.Curve> ret = new Collections.Generic.List<Revit.DB.Curve>();
            // 調整
            curves = OptimizeLineVertexNoConvLine(curves);

            Revit.DB.BoundingBoxUV bbUV = plnFace.GetBoundingBox();
            Revit.DB.UV uv = bbUV.Min;
            Revit.DB.XYZ loc = plnFace.Evaluate(uv);

            Revit.DB.Curve leftbtm = null;
            foreach (Revit.DB.Curve c in curves)
            {
                if (Distance(loc, c.GetEndPoint(0)) < Approx0Len)
                {
                    leftbtm = c;
                }
            }

            if (leftbtm == null)
            {
                leftbtm = GetLeftBottomCurve(curves, 0);
            }

            // 連続するカーチE
            GetContinuousCurves(curves, leftbtm, ref ret);

            return ret;
        }

        /// ================================================================================
        /// <summary>角度取征E/summary>
        /// 
        /// <param name="pos0">基準点</param>
        /// <param name="pos1">他点�E�E/param>
        /// <param name="pos2">他点�E�E/param>
        /// 
        /// <returns>結果</returns>
        /// 
        /// <history>2017/02/06 Created CST,Co.Ltd. Hideki Sudo</history>
        /// ================================================================================
        public double Angle3DA(Revit.DB.XYZ pos0, Revit.DB.XYZ pos1, Revit.DB.XYZ pos2)
        {
            double angle = 0;

            // 冁E��E
            double dotVal = DotProduct(pos0, pos1, pos2);

            // 外穁E
            Revit.DB.XYZ crossVal = CrossProduct(pos0, pos1, pos2);

            double axis = crossVal.Z;
            if (System.Math.Abs(pos0.X - pos1.X) < Approx0Len && System.Math.Abs(pos0.X - pos2.X) < Approx0Len)
            {
                axis = crossVal.X;
            }
            else if (System.Math.Abs(pos0.Y - pos1.Y) < Approx0Len && System.Math.Abs(pos0.Y - pos2.Y) < Approx0Len)
            {
                axis = crossVal.Y;
            }
            // 角度
            angle = System.Math.Atan2(axis, dotVal);


            return angle;
        }

        #endregion

        // プロパティ
        #region Properties

        #endregion
    }
}
