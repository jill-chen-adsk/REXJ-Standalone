using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace MepManholeTool.Utils
{
    public static class GeometryUtl
    {
        /// <summary>三角形と点の内外判定</summary>
        /// <param name="pos"       >ポイント</param>
        /// <param name="triMeshes" >三角メッシュ</param>
        /// <param name="zValue"    >内側にあった場合のZ値</param>
        /// <returns><p>結果</p>
        ///             <p>True  = 内側</p>
        ///             <p>False = 外側</p></returns>
        public static bool CheckTrianglePoint(XYZ pos,
                                IList<MeshTriangle> triMeshes,
                                ref double zValue)
        {
            bool ret = false;
            try
            {
                // 三角メッシュの内外判定で取得
                foreach (MeshTriangle triMesh in triMeshes)
                {
                    // 2Dでポイントが含まれる三角形
                    double z = 0;
                    int retFunc = CheckTrianglePoint2D(pos, triMesh);
                    
                    if (retFunc == 3)
                    {
                        // ３角メッシュの内点のZ値取得
                        if (GetZValuePointInTriMesh(pos, triMesh, ref z))
                        {
                            ret = true;
                            if( z > zValue)
                                zValue = z;
                        }
                    }
                    else if (retFunc is >= 0 and <= 2)
                    {
                        ret = true;
                        zValue = triMesh.get_Vertex(retFunc).Z;
                        break;
                    }
                }
            }
            catch
            {
                ret = false;
            }
            return ret;
        }
        
        /// <summary>三角形と点の内外判定</summary>
        /// <param name="pos"     >ポイント</param>
        /// <param name="triMesh" >三角メッシュ</param>
        /// <returns><p>結果</p>
        ///             <p>-1  = 外側</p>
        ///             <p>0-2 = 頂点上(頂点番号)</p>
        ///             <p>3   = 内側</p></returns>
        private static int CheckTrianglePoint2D(XYZ pos, MeshTriangle triMesh)
        {
            int ret = -1;
            IList<XYZ> poss = new List<XYZ>();
            poss.Add(triMesh.get_Vertex(0));
            poss.Add(triMesh.get_Vertex(1));
            poss.Add(triMesh.get_Vertex(2));

            for (int i = 0; i < 3; ++i)
            {
                if (Distance2D(pos, poss[i]) < 0.1)
                {
                    ret = i;
                    break;
                }
            }

            if (ret == -1)
            {
                bool retFunc = CheckTrianglePoint2D(pos, poss[0], poss[1], poss[2]);
                if (retFunc)
                {
                    ret = 3;
                }
            }
            return ret;
        }
        
        /// <summary>三角形と点の内外判定</summary>
        /// <param name="pos" >ポイント</param>
        /// <param name="pos1">１つ目のポイント</param>
        /// <param name="pos2">２つ目のポイント</param>
        /// <param name="pos3">３つ目のポイント</param>
        /// <returns><p>結果</p>
        ///             <p>True  = 内側</p>
        ///             <p>False = 外側</p></returns>
        public static bool CheckTrianglePoint2D(XYZ pos,
                                  XYZ pos1,
                                  XYZ pos2,
                                  XYZ pos3)
        {
            bool ret = false;

            XYZ posB = new XYZ(pos.X, pos.Y, 0.0);
            IList<XYZ> poss = new List<XYZ>();
            poss.Add(new XYZ(pos1.X, pos1.Y, 0.0));
            poss.Add(new XYZ(pos2.X, pos2.Y, 0.0));
            poss.Add(new XYZ(pos3.X, pos3.Y, 0.0));

            try
            {
                // 三角形の重心
                XYZ? gravity = TriangleGravity2D(poss[0], poss[1], poss[2]);
                if (gravity == null)
                {
                    return ret;
                }

                // 三角形重心とポイントの線分
                Line line1 = Line.CreateBound(posB, gravity);

                // 線分と三角形の交点
                bool flag = false;
                for (int i = 0; i < 3; ++i)
                {
                    int j = i + 1;
                    if (j > 2)
                    {
                        j = 0;
                    }
                    Line line2 = Line.CreateBound(poss[i], poss[j]);
                    if (line2 != null)
                    {
                        using (CurveIntersectResult curveIntersect = line1.Intersect(line2, CurveIntersectResultOption.Simple))
                        {
                            if (curveIntersect.Result == SetComparisonResult.Overlap)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        flag = true;
                        break;
                    }
                }

                // 交点判定
                if (flag == false)
                {
                    ret = true;
                }
            }
            catch
            {
                ret = false;
            }

            return ret;
        }
        
        public static double Distance2D(XYZ pos1, XYZ pos2)
        {
            return Distance(pos1.X, pos1.Y, 0.0, pos2.X, pos2.Y, 0.0);
        }
        
        /// <summary>
        /// p点が上から見たときにtriの中にあるかどうか判別
        /// </summary>
        /// <param name="tri"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsPointInside( this MeshTriangle tri, XYZ p )
        {
            var t0 = tri.get_Vertex( 0 ) ;
            var t1 = tri.get_Vertex( 1 ) ;
            var t2 = tri.get_Vertex( 2 ) ;

            var denominator = ( ( t1.Y - t2.Y ) * ( t0.X - t2.X ) + ( t2.X - t1.X ) * ( t0.Y - t2.Y ) ) ;
            var a = ( ( t1.Y - t2.Y ) * ( p.X - t2.X ) + ( t2.X - t1.X ) * ( p.Y - t2.Y ) ) / denominator ;
            var b = ( ( t2.Y - t0.Y ) * ( p.X - t2.X ) + ( t0.X - t2.X ) * ( p.Y - t2.Y ) ) / denominator ;
            var c = 1 - a - b ;
            return a >= 0 && a <= 1 && b >= 0 && b <= 1 && c >= 0 && c <= 1 ;
        }
        
        /// <summary>３角メッシュの内点のZ値取得</summary>
        /// <param name="pos"     >ポイント</param>
        /// <param name="triMesh" >三角メッシュ</param>
        /// <param name="zValue"  >交点</param>
        /// <returns><p>結果</p>
        ///             <p>True  = ポイントが三角メッシュ内</p>
        ///             <p>False = ポイントが三角メッシュ外</p></returns>
        public static bool GetZValuePointInTriMesh(XYZ? pos,
            MeshTriangle? triMesh,
            ref double zValue)
        {
            bool ret = false;

            if ((pos == null) || (triMesh == null))
            {
                return ret;
            }

            // 三角メッシュの法線
            XYZ normal = CrossProduct(triMesh.get_Vertex(0), triMesh.get_Vertex(1), triMesh.get_Vertex(2));

            // ベクトル
            double vecX = pos.X - triMesh.get_Vertex(0).X;
            double vecY = pos.Y - triMesh.get_Vertex(0).Y;
            double vecZ = (-(vecX * normal.X) - (vecY * normal.Y)) / normal.Z;

            // Z値
            zValue = vecZ + triMesh.get_Vertex(0).Z;

            ret = true;
            return ret;
        }
        
        public static XYZ CrossProduct(XYZ pos0, XYZ pos1, XYZ pos2)
        {
            IList<double> ret = new List<double>();

            double p0X = pos0.X;
            double p0Y = pos0.Y;
            double p0Z = pos0.Z;
            double p1X = pos1.X;
            double p1Y = pos1.Y;
            double p1Z = pos1.Z;
            double p2X = pos2.X;
            double p2Y = pos2.Y;
            double p2Z = pos2.Z;
            
            double vecAx = p1X - p0X;
            double vecAy = p1Y - p0Y;
            double vecAz = p1Z - p0Z;

            double vecBx = p2X - p0X;
            double vecBy = p2Y - p0Y;
            double vecBz = p2Z - p0Z;

            ret.Add((vecAy * vecBz) - (vecAz * vecBy));
            ret.Add((vecAz * vecBx) - (vecAx * vecBz));
            ret.Add((vecAx * vecBy) - (vecAy * vecBx));

            return new XYZ(ret[0], ret[1], ret[2]);
        }
        
        public static double Distance(double p1X, double p1Y, double p1Z,
            double p2X, double p2Y, double p2Z)
        {
            return Math.Sqrt(Math.Pow(p2X - p1X, 2) +
                             Math.Pow(p2Y - p1Y, 2) +
                             Math.Pow(p2Z - p1Z, 2));
        }
        
        /// --------------------------------------------------------------------------------
        /// <summary>取得三角形重心</summary>
        /// --------------------------------------------------------------------------------
        public static XYZ? TriangleGravity2D(XYZ position1, XYZ position2, XYZ position3)
        {
            // 初期化
            XYZ? result = null;
            
            // Z座標
            double zCoordinate = position1.Z;

            // 線分1
            XYZ startPosition = new XYZ(position1.X, position1.Y, zCoordinate);
            XYZ endPosition = new XYZ(position2.X, position2.Y, zCoordinate);

            Curve curve1;
            if (Distance2D(startPosition, endPosition) > 0.1)
            {
                curve1 = Line.CreateBound(startPosition, endPosition);
            }
            else
            {
                return null;
            }

            // 線分2
            startPosition = endPosition;
            endPosition = new XYZ(position3.X, position3.Y, zCoordinate);

            Curve curve2;
            if (Distance2D(startPosition, endPosition) > 0.1)
            {
                curve2 = Line.CreateBound(startPosition, endPosition);
            }
            else
            {
                return null;
            }

            // 線分3
            startPosition = endPosition;
            endPosition = new XYZ(position1.X, position1.Y, zCoordinate);

            Curve curve3;
            if (Distance2D(startPosition, endPosition) > 0.1)
            {
                curve3 = Line.CreateBound(startPosition, endPosition);
            }
            else
            {
                return null;
            }

            IList<Curve> curves = new List<Curve>();
            if ((curve1 != null) && (curve2 != null) && (curve3 != null))
            {
                curves.Add(curve1);
                curves.Add(curve2);
                curves.Add(curve3);

                result = PolygonGravity2D(curves);
            }

            return result;
        }

        public static XYZ? PolygonGravity2D(IList<Curve>? curves)
        {
            if (curves == null || curves.Count == 0 || curves.Count < 3)
            {
                return null;
            }
            
            // 頂点
            List<double> px = new List<double>();
            List<double> py = new List<double>();
            List<double> pz = new List<double>();
            
            for (int i = curves.Count - 1; i >= 0; --i)
            {
                px.Add(curves[i].GetEndPoint(0).X);
                py.Add(curves[i].GetEndPoint(0).Y);
                pz.Add(curves[i].GetEndPoint(0).Z);
            }
            
            var ret = PolygonGravity2D(px, py, pz);
            return ret;
        }
        
        
        /// <summary>
        /// 多角形重心取得
        /// </summary>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <param name="pz"></param>
        /// <returns></returns>
        public static XYZ? PolygonGravity2D(List<double>? px, List<double>? py, List<double>? pz)
        {
            List<double> ret = new List<double>();

            // 頂点数
            if (px == null || py == null || pz == null || px.Count != py.Count || px.Count != pz.Count)
            {
                return null;
            }

            int numVer = px.Count;

            double gx = 0.0;
            double gy = 0.0;
            double gz = pz[0];

            // 面積の和
            double area = PolygonArea(px, py, pz);

            // 重心の和
            for (int i = 0; i < numVer; ++i)
            {
                int j = i + 1;
                if (j >= numVer)
                {
                    j = 0;
                }

                double commonFactor = (px[i] * py[j]) - (px[j] * py[i]);
                gx += (px[i] + px[j]) * commonFactor;
                gy += (py[i] + py[j]) * commonFactor;
            }

            // 重心の和
            gx = (1 / (6 * area)) * gx;
            gy = (1 / (6 * area)) * gy;

            // 重心
            ret.Add(gx);
            ret.Add(gy);
            ret.Add(gz);

            return new XYZ(ret[0], ret[1], ret[2]);
        }
        
        /// <summary>
        /// 多角形面積取得
        /// </summary>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <param name="pz"></param>
        /// <returns></returns>
        public static double PolygonArea(List<double>? px, List<double>? py, List<double>? pz)
        {
            double ret = 0.0;

            // 頂点
            if (px == null || py == null || pz == null || px.Count != py.Count || px.Count != pz.Count)
            {
                return ret;
            }

            int numVer = px.Count;

            // 面積の和
            double xiyi = 0.0;
            for (int i = 0; i < numVer; ++i)
            {
                int j = i + 1;
                if (j >= numVer)
                {
                    j = 0;
                }
                xiyi = xiyi + ((px[i] * py[j]) - (px[j] * py[i]));
            }

            ret = xiyi * 0.5;
            return ret;
        }
    }
}