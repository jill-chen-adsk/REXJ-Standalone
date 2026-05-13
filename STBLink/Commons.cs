using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;


namespace STBLink
{
    class Commons
    {
        internal const string SystemName = "ST-Bridge Link";


        internal static string DLLFilePath = "";

        internal static Document doc;

        internal static int GridMode = 0;


        /// <summary>
        /// Converts feet to metric units.
        /// </summary>
        /// <param name="ft">Feet.</param>
        /// <param name="unit">Unit: =0:mm, =1:cm, =3:m</param>
        /// <returns></returns>
        internal static double ft2mm(double ft, int unit = 0, int round = 1)
        {
            int pow = (int)Math.Pow(10, unit);
            double mm = ft * 304.8 * Math.Pow(10, round);

            mm = Math.Round(mm, MidpointRounding.AwayFromZero) / Math.Pow(10, round) / pow;

            return mm;
        }
        internal static XYZ ft2mm(XYZ ft, int unit = 0)
        {
            XYZ mm = new XYZ(ft2mm(ft.X, unit),
                             ft2mm(ft.Y, unit),
                             ft2mm(ft.Z, unit));

            return mm;
        }
        /// <summary>
        /// Converts metric units to feet.
        /// </summary>
        /// <param name="mm">Dimension value.</param>
        /// <param name="unit">Unit: =0:mm, =1:cm, =3:m</param>
        /// <returns></returns>
        internal static double mm2ft(double mm, int unit = 0)
        {
            int pow = (int)Math.Pow(10, unit);
            double ft = mm / 304.8 * pow;

            return ft;
        }
        internal static XYZ mm2ft(XYZ mm, int unit = 0)
        {
            XYZ ft = new XYZ(mm2ft(mm.X, unit),
                             mm2ft(mm.Y, unit),
                             mm2ft(mm.Z, unit));

            return ft;
        }

        /// <summary>[2D] Returns distance from line segment to point as positive on the left, negative on the right.
        /// </summary>
        /// <param name="Lx1">Segment start X.</param>
        /// <param name="Ly1">Segment start Y.</param>
        /// <param name="Lx2">Segment end X.</param>
        /// <param name="Ly2">Segment end Y.</param>
        /// <param name="Px">Point X.</param>
        /// <param name="Py">Point Y.</param>
        /// <returns>Signed distance: positive if left of the segment (from start), negative if right.</returns>
        internal static double LinePointDist(double Lx1, double Ly1, double Lx2, double Ly2, double Px, double Py)
        {
            //-----------------------------------------------------------------------
            //              - (Lx2,Ly2)
            //          +  / |
            //            / \
            //          / - \ Dist
            //        .        .
            //     (Lx1,Ly1)  (Px,Py)
            //-----------------------------------------------------------------------
            double EPS = 0.00001;
            double Dist = 0;
            double cVtx = 0;
            double cVty = 0;

            cVtx = -(Ly2 - Ly1);
            cVty = Lx2 - Lx1;

            if (Math.Abs(cVtx) <= EPS && Math.Abs(cVty) <= EPS)
            {
                Dist = Math.Sqrt((Px - Lx1) * (Px - Lx1) + (Py - Ly1) * (Py - Ly1));
            }
            else
            {
                Dist = (cVtx * Px + cVty * Py + (Lx1 * (Ly2 - Ly1) - Ly1 * (Lx2 - Lx1))) / Math.Sqrt(cVtx * cVtx + cVty * cVty);
            }

            Dist = Math.Round(Dist, 3, MidpointRounding.AwayFromZero);

            return Dist;
        }

        /// <summary>[2D] Computes intersection of segment 1 and segment 2. Return: intersection flag [=1: outside segments, =0: on both segments, =-1: no intersection, =2: on segment 1, =3: on segment 2]
        /// </summary>
        /// <param name="Lx11">Segment 1 start X.</param>
        /// <param name="Ly11">Segment 1 start Y.</param>
        /// <param name="Lx12">Segment 1 end X.</param>
        /// <param name="Ly12">Segment 1 end Y.</param>
        /// <param name="Lx21">Segment 2 start X.</param>
        /// <param name="Ly21">Segment 2 start Y.</param>
        /// <param name="Lx22">Segment 2 end X.</param>
        /// <param name="Ly22">Segment 2 end Y.</param>
        /// <param name="Xx">(Output) Intersection X.</param>
        /// <param name="Yy">(Output) Intersection Y.</param>
        /// <returns>Intersection flag [=1: outside segments, =0: on both segments, =-1: no intersection, =2: on segment 1, =3: on segment 2]</returns>
        internal static int CalcCross(double Lx11, double Ly11, double Lx12, double Ly12,
                                      double Lx21, double Ly21, double Lx22, double Ly22,
                                      out double Xx, out double Yy)
        {
            double Zgosa = 0.000001;

            Xx = 0;
            Yy = 0;

            int RetCode = 0;

            double clx11 = Lx11;
            double cly11 = Ly11;
            double clx12 = Lx12;
            double cly12 = Ly12;
            double clx21 = Lx21;
            double cly21 = Ly21;
            double clx22 = Lx22;
            double cly22 = Ly22;
            double gXx = 0;
            double gYy = 0;

            if (clx11 > clx12)
            {
                gXx = clx11; clx11 = clx12; clx12 = gXx;
                gYy = cly11; cly11 = cly12; cly12 = gYy;
            }
            if (clx21 > clx22)
            {
                gXx = clx21; clx21 = clx22; clx22 = gXx;
                gYy = cly21; cly21 = cly22; cly22 = gYy;
            }

            double cA1 = cly12 - cly11;
            double cB1 = -(clx12 - clx11);
            double cC1 = cly11 * (clx12 - clx11) - clx11 * (cly12 - cly11);
            double cA2 = cly22 - cly21;
            double cB2 = -(clx22 - clx21);
            double cC2 = cly21 * (clx22 - clx21) - clx21 * (cly22 - cly21);

            double Det = cA1 * cB2 - cA2 * cB1;

            if (Math.Abs(Det) <= Zgosa)
            {
                RetCode = -1;
                gXx = clx11;
                gYy = cly11;
            }
            else
            {
                short Flg1 = 0;
                short Flg2 = 0;

                gXx = (cB1 * cC2 - cB2 * cC1) / Det;
                gYy = (cA2 * cC1 - cA1 * cC2) / Det;

                cA1 = Math.Sqrt((clx11 - gXx) * (clx11 - gXx) + (cly11 - gYy) * (cly11 - gYy));
                cB1 = Math.Sqrt((clx12 - gXx) * (clx12 - gXx) + (cly12 - gYy) * (cly12 - gYy));
                cC1 = Math.Sqrt((clx12 - clx11) * (clx12 - clx11) + (cly12 - cly11) * (cly12 - cly11));

                if (Math.Abs(cC1 - (cA1 + cB1)) <= Zgosa) Flg1 = 1;

                cA2 = Math.Sqrt((clx21 - gXx) * (clx21 - gXx) + (cly21 - gYy) * (cly21 - gYy));
                cB2 = Math.Sqrt((clx22 - gXx) * (clx22 - gXx) + (cly22 - gYy) * (cly22 - gYy));
                cC2 = Math.Sqrt((clx22 - clx21) * (clx22 - clx21) + (cly22 - cly21) * (cly22 - cly21));

                if (Math.Abs(cC2 - (cA2 + cB2)) <= Zgosa) Flg2 = 1;

                if (Flg1 == 1 && Flg2 == 1)
                {
                    RetCode = 0;
                }
                else if (Flg1 == 1)
                {
                    RetCode = 2;
                }
                else if (Flg2 == 1)
                {
                    RetCode = 3;
                }
                else
                {
                    RetCode = 1;
                }
            }

            Xx = gXx;
            Yy = gYy;

            return RetCode;
        }
        /// <summary>[2D] Computes intersection of segment 1 and segment 2 (lines via X,Y). Return: intersection flag [=1: outside segments, =0: on both segments, =-1: no intersection, =2: on segment 1, =3: on segment 2]
        /// </summary>
        /// <param name="L11"></param>
        /// <param name="L12"></param>
        /// <param name="L21"></param>
        /// <param name="L22"></param>
        /// <param name="Cc"></param>
        /// <returns></returns>
        internal static int CalcCross(XYZ L11, XYZ L12, XYZ L21, XYZ L22, out XYZ Cc)
        {
            int RetCode = CalcCross(L11.X, L11.Y, L12.X, L12.Y, L21.X, L21.Y, L22.X, L22.Y, out double Xx, out double Yy);

            Cc = new XYZ(Xx, Yy, L11.Z);

            return RetCode;
        }

        /// <summary>Transforms to the vector coordinates specified by vec.
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        internal static double Get_Point_Vec(XYZ vec, XYZ p)
        {
            double ret = 0;
            ret = vec.X * p.X + vec.Y * p.Y + vec.Z * p.Z;
            return ret;
        }

        internal static void AxisRotate(double px, double py, double pz, double lx1, double ly1, double lz1, double lx2, double ly2, double lz2, double deg, ref double xx, ref double yy, ref double zz)
        {
            /*
            '-----------------------------------------------------------------------
            ' /// Rotate point around an arbitrary axis ///
            '-----------------------------------------------------------------------
            '  Input  ... (Px,Py,Pz)                  : Point to rotate
            '		     (Lx1,Ly1,Lz1)-(Lx2,Ly2,Lz2) : Axis of rotation
            '		     Deg                         : Rotation angle (degrees)
            '  Output   (Xx,Yy,Zz)                   : Result coordinates
            '-----------------------------------------------------------------------
            */
            double D2R = 0.0;
            double s, c, x, y, z, nx, ny, nz, dd;

            xx = px; yy = py; zz = pz;

            if (D2R == 0) D2R = Math.Atan((double)1.0) * 4 / 180;
            s = Math.Sin(deg * D2R); c = Math.Cos(deg * D2R);
            x = px - lx1; y = py - ly1; z = pz - lz1;
            nx = lx2 - lx1; ny = ly2 - ly1; nz = lz2 - lz1;
            dd = Math.Sqrt(nx * nx + ny * ny + nz * nz);
            nx /= dd; ny /= dd; nz /= dd;
            xx = x * (nx * nx * (1 - c) + c) + y * (ny * nx * (1 - c) - nz * s) + z * (nz * nx * (1 - c) + ny * s);
            yy = x * (nx * ny * (1 - c) + nz * s) + y * (ny * ny * (1 - c) + c) + z * (nz * ny * (1 - c) - nx * s);
            zz = x * (nx * nz * (1 - c) - ny * s) + y * (ny * nz * (1 - c) + nx * s) + z * (nz * nz * (1 - c) + c);
            xx += lx1; yy += ly1; zz += lz1;

        }
        internal static void AxisRotate(XYZ P1, XYZ L1, XYZ L2, double Deg, ref XYZ P2)
        {
            double xx = 0, yy = 0, zz = 0;

            AxisRotate(P1.X, P1.Y, P1.Z,
                   L1.X, L1.Y, L1.Z,
                   L2.X, L2.Y, L2.Z,
                   Deg,
                   ref xx, ref yy, ref zz);

            P2 = new XYZ(xx, yy, zz);
        }
        /// <summary>[3D] Returns distance between point O and point P.</summary>
        /// <param name="Ox">Point O X.</param>
        /// <param name="Oy">Point O Y.</param>
        /// <param name="Oz">Point O Z.</param>
        /// <param name="Px">Point P X.</param>
        /// <param name="Py">Point P Y.</param>
        /// <param name="Pz">Point P Z.</param>
        /// <returns>Distance.</returns>
        internal static double PointPointDist3D(double Ox, double Oy, double Oz, double Px, double Py, double Pz)
        {
            //----------------------------------------------------------------------
            //                . (Px,Py,Pz)
            //              /
            //            / Dist
            //          /
            //        .
            //     (Ox,Oy,Oz)
            //-----------------------------------------------------------------------
            double Dist;
            Dist = Math.Sqrt((Px - Ox) * (Px - Ox) + (Py - Oy) * (Py - Oy) + (Pz - Oz) * (Pz - Oz));
            return Dist;
        }
        internal static double PointPointDist3D(XYZ O, XYZ P)
        {
            double Dist;
            Dist = PointPointDist3D(O.X, O.Y, O.Z, P.X, P.Y, P.Z);
            return Dist;
        }

        /// <summary>[3D] Distance from point to line.
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <returns></returns>
        internal static double LinePointDist(XYZ vec1, XYZ vec2)
        {
            double dist = 0;
            XYZ vec = vec1.CrossProduct(vec2);
            double S = vec.GetLength();
            dist = S / vec1.GetLength();
            return dist;
        }



        /// <summary>
        /// Checks whether a point lies on the line segment (collinearity and betweenness).
        /// </summary>
        /// <param name="p0">Start point.</param>
        /// <param name="p1">End point.</param>
        /// <param name="chk">Point to check.</param>
        /// <returns>=0 between start and end, =1 collinear but outside segment, =2 coincident with start or end, =-1 not collinear</returns>
        /// <remarks></remarks>
        internal static int CheckOnLine(XYZ p0, XYZ p1, XYZ chk)
        {
            XYZ v1 = (chk - p0).Normalize();
            XYZ v2 = (p1 - p0).Normalize();
            XYZ v3 = (chk - p1).Normalize();

            if ((chk.DistanceTo(p0) < 0.0001) || (chk.DistanceTo(p1) < 0.0001))
            {
                // Coincides with start or end
                return 2;
            }

            if (v1.CrossProduct(v2).GetLength() < 0.0001)
            {
                // Cross product magnitude zero => vectors parallel => on the line

                if (v1.DistanceTo(-v3) < 0.0001)
                {
                    // V1 and V3 are opposite direction => between P0 and P1
                    return 0;
                }

                // Outside segment beyond P0 or P1
                return 1;
            }

            // Off the line
            return -1;
        }

        /// <summary>
        /// Area by the shoelace / coordinate method (signed for winding: counterclockwise &gt; 0, clockwise &lt; 0).
        /// </summary>
        /// <param name="Pxyz">Point coordinates.</param>
        /// <returns>Signed area.</returns>
        /// <remarks></remarks>
        internal static double CalcMenseki(List<XYZ> Pxyz)
        {
            // Area by coordinate method
            double menseki = 0;
            for (int i = 0; i < Pxyz.Count; ++i)
            {
                int j = i + 1;
                if (j >= Pxyz.Count) { j = 0; }

                menseki += Pxyz[i].X * Pxyz[j].Y - Pxyz[j].X * Pxyz[i].Y;
            }
            menseki = menseki / 2;

            return menseki;
        }


        /// <summary>
        /// Inside/outside test (-1: outside, 0: on edge, 1: inside).
        /// </summary>
        /// <param name="PtOrg">Polygon vertices.</param>
        /// <param name="chkP">Point to test.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static int IntoRegion(List<XYZ> PtOrg, XYZ chkP)
        {
            // Prerequisites:
            // - All polygon vertices and chkP lie on the same plane.

            if (PtOrg.Count <= 2) { return -1; }

            List<XYZ> Pt = new List<XYZ>();
            Pt.AddRange(PtOrg);

            // Remove collinear vertices
            for (int p = Pt.Count - 2; p >= 1; --p)
            {
                if (CheckOnLine(Pt[p - 1], Pt[p + 1], Pt[p]) == 0)
                {
                    Pt.RemoveAt(p);
                }
            }
            if (Pt.Count <= 2) { return -1; }
            if (CheckOnLine(Pt[0], Pt[Pt.Count - 2], Pt[Pt.Count - 1]) == 0)
            {
                // Last vertex collinear with previous and start => drop last vertex
                Pt.RemoveAt(Pt.Count - 1);
            }
            if (Pt.Count <= 2) { return -1; }


            // On-edge check
            for (int p1 = 0; p1 <= Pt.Count - 1; ++p1)
            {
                int p2 = p1 + 1;
                if (p2 >= Pt.Count) { p2 = 0; }

                int ret = CheckOnLine(Pt[p1], Pt[p2], chkP);
                if (ret == 0)
                {
                    // On perimeter edge
                    return 0;
                }
            }

            // Concave polygon handling
            List<XYZ> Puvw = TransformCoord(Pt);
            double m = CalcMenseki(Puvw);

            List<int> dentIndex = new List<int>();

            for (int P0 = 0; P0 < Puvw.Count; ++P0)
            {
                // Next vertex
                int P1 = P0 + 1;
                if (P1 >= Puvw.Count) { P1 = 0; }

                // Previous vertex
                int P2 = P0 - 1;
                if (P2 < 0) { P2 = Puvw.Count - 1; }

                XYZ v1 = (Puvw[P1] - Puvw[P0]).Normalize();
                XYZ v2 = (Puvw[P0] - Puvw[P2]).Normalize();

                if (v1.DistanceTo(v2) < 0.0001)
                {
                    // Collinear
                    continue;
                }

                XYZ Vn = (v2.CrossProduct(v1)).Normalize();

                if (m > 0)
                {
                    // Counterclockwise
                }
                else
                {
                    // Clockwise (flip vector)
                    Vn = -Vn;
                }

                if (Vn.Z < 0)
                {
                    // Concave vertex
                    dentIndex.Add(P0);
                }
            }

            if (dentIndex.Count > 0)
            {
                // Exclude concave vertices
                List<XYZ> Pt2 = new List<XYZ>();
                for (int p1 = 0; p1 <= Pt.Count - 1; ++p1)
                {
                    if (dentIndex.Contains(p1) == false)
                    {
                        Pt2.Add(Pt[p1]);
                    }
                }

                // Inside/outside with concave vertices removed
                int iretA = IntoRegion(Pt2, chkP);
                if (iretA == 1)
                {
                    // Inside -> recheck regions that include concave pockets
                    for (int i = dentIndex.Count - 1; i >= 0; --i)
                    {
                        int tempIdx = dentIndex[i];
                        List<int> idx = new List<int>()
                        {
                            tempIdx,
                        };
                        dentIndex.RemoveAt(i);

                        int tempIdx2 = tempIdx;
                        while (true)
                        {
                            if (dentIndex.Count == 0) { break; }

                            tempIdx2 = tempIdx2 - 1;

                            // Indices checked in descending order; no wrap to Count-1 when hitting -1

                            if (dentIndex.Contains(tempIdx2) == true)
                            {
                                // Adjacent prev index also concave -> merge region
                                idx.Insert(0, tempIdx2);
                            }
                            else
                            {
                                break;
                            }
                        }

                        tempIdx2 = tempIdx;
                        while (true)
                        {
                            if (dentIndex.Count == 0) { break; }

                            tempIdx2 = tempIdx2 + 1;
                            if (tempIdx2 >= Pt.Count) { tempIdx2 = 0; }

                            if (dentIndex.Contains(tempIdx2) == true)
                            {
                                // Adjacent next index also concave -> merge region
                                idx.Add(tempIdx2);
                            }
                            else
                            {
                                break;
                            }
                        }

                        // Add neighbors before/after the concave run
                        tempIdx2 = idx[0] - 1;
                        if (tempIdx2 < 0) { tempIdx2 = Pt.Count - 1; }
                        idx.Insert(0, tempIdx2);

                        tempIdx2 = idx[idx.Count - 1] + 1;
                        if (tempIdx2 >= Pt.Count) { tempIdx2 = 0; }
                        idx.Add(tempIdx2);

                        List<XYZ> Pt3 = new List<XYZ>();
                        for (int j = 0; j <= idx.Count - 1; ++j)
                        {
                            Pt3.Add(Pt[idx[j]]);
                        }

                        int iretB = IntoRegion(Pt3, chkP);
                        if (iretB >= 0)
                        {
                            // In concave-pocket region or on its edge -> outside original polygon
                            return -1;
                        }

                        if (dentIndex.Count == 0) { break; }
                    }
                }
                else if (iretA == 0)
                {
                    // On edge
                    // -> If truly on original boundary, already handled above.
                    //    Edge coincident only after removing concaves implies outside.
                    iretA = -1;
                }

                return iretA;

            }
            else
            {

                XYZ normal = new XYZ();
                XYZ Vec1 = (Pt[1] - Pt[0]).Normalize();
                for (int p = 2; p <= Pt.Count - 1; ++p)
                {
                    XYZ Vec2 = (Pt[p] - Pt[0]).Normalize();
                    normal = (Vec1.CrossProduct(Vec2)).Normalize();
                    if (normal.GetLength() > 0.01)
                    {
                        break;
                    }
                }


                // Inside/outside test
                int result = 0;
                for (int p1 = 0; p1 <= Pt.Count - 1; ++p1)
                {
                    int p2 = p1 + 1;
                    if (p2 >= Pt.Count) { p2 = 0; }

                    XYZ v1 = (Pt[p2] - Pt[p1]).Normalize();
                    v1 = v1.CrossProduct(normal);

                    XYZ v2 = (chkP - Pt[p1]).Normalize();

                    double dot = v1.DotProduct(v2);
                    if (result == 0)
                    {
                        if (dot < 0)
                        {
                            result = -1;
                        }
                        else if (dot > 0)
                        {
                            result = 1;
                        }
                    }
                    else
                    {
                        // Sign mismatch -> outside
                        if (result < 0 && dot > 0) { return -1; }
                        if (result > 0 && dot < 0) { return -1; }
                    }
                }

                // Inside
                return 1;
            }

        }



        /// <summary>
        /// Transforms points to UVW local coordinates with the plane normal as the W axis.
        /// </summary>
        /// <param name="Pxyz">World XYZ coordinates.</param>
        /// <returns>Local UVW coordinates.</returns>
        /// <remarks></remarks>
        private static List<XYZ> TransformCoord(List<XYZ> Pxyz)
        {
            XYZ v1 = (Pxyz[1] - Pxyz[0]).Normalize();
            XYZ v2 = (Pxyz[2] - Pxyz[0]).Normalize();
            XYZ normal = (v1.CrossProduct(v2)).Normalize();

            XYZ vecU = v1;
            XYZ vecV = (normal.CrossProduct(v1)).Normalize();
            XYZ vecW = normal;

            List<XYZ> Puvw = new List<XYZ>();
            for (int i = 0; i <= Pxyz.Count - 1; ++i)
            {
                //|u| |Ux Uy Uz||x|
                //|v|=|Vx Vy Wz||y|
                //|w| |Wx Wy Vz||z|
                double u = vecU.X * Pxyz[i].X + vecU.Y * Pxyz[i].Y + vecU.Z * Pxyz[i].Z;
                double v = vecV.X * Pxyz[i].X + vecV.Y * Pxyz[i].Y + vecV.Z * Pxyz[i].Z;
                double w = vecW.X * Pxyz[i].X + vecW.Y * Pxyz[i].Y + vecW.Z * Pxyz[i].Z;

                Puvw.Add(new XYZ(u, v, w));
            }

            return Puvw;
        }


        /// <summary>
        /// Tests whether all points lie on the same plane.
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="Point0"></param>
        /// <returns></returns>
        internal static bool CalcPlane(XYZ normal, List<XYZ> Point0)
        {
            bool ret = true;
            for (int i = 1; i < Point0.Count(); i++)
            {
                XYZ v = (Point0[i] - Point0[0]).Normalize();
                if (Math.Abs(v.DotProduct(normal)) > 0.001) // Dot with normal nonzero => not on the plane
                {
                    ret = false;
                    break;
                }
            }

            return ret;
        }



        /// <summary>
        /// Configures shared parameters file path behavior.
        /// </summary>
        internal static void SetSharedParametersFile()
        {
            string filename = System.IO.Path.GetFileName(Commons.doc.Application.SharedParametersFilename);
            if (filename == RevitLNK.REXStructual &&
                System.IO.File.Exists(Commons.doc.Application.SharedParametersFilename))
            {
                // Some firms lock down the Documents folder;
                // if the shared parameters file name matches, do not overwrite.
                // Same file name anywhere is acceptable.

                // Lookup table paths: use same folder as the shared parameters file.
                string folderpath = System.IO.Path.GetDirectoryName(Commons.doc.Application.SharedParametersFilename);
                if (System.IO.File.Exists(folderpath + "\\" + RevitLNK.ConvRFA_tbl)) RevitLNK.familyTableFile = folderpath + "\\" + RevitLNK.ConvRFA_tbl;
                if (System.IO.File.Exists(folderpath + "\\" + RevitLNK.ConvBase_tbl)) RevitLNK.BaseTableFile = folderpath + "\\" + RevitLNK.ConvBase_tbl;
            }
            else
            {
                string mydocu = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Autodesk REXJ\\" + RevitLNK.RevitVersion;
                RevitLNK.familyTableFile = mydocu + "\\" + RevitLNK.ConvRFA_tbl;
                RevitLNK.BaseTableFile = mydocu + "\\" + RevitLNK.ConvBase_tbl;
                Commons.doc.Application.SharedParametersFilename = RevitLNK.sharedParamsFile;
            }
        }



        /// <summary>
        /// Reads the version written in the XML file.
        /// </summary>
        /// <returns></returns>
        internal static string GetVersion()
        {
            string version = null;

            // \Contents\ST-Bridge\ -> \Contents\ST-Bridge
            string xmlpath = System.IO.Path.GetDirectoryName(Commons.DLLFilePath);
            // \Contents\ST-Bridge -> \Contents
            xmlpath = System.IO.Path.GetDirectoryName(xmlpath);
            // \Contents -> \Contents\REXJ-RST.xml
            xmlpath += @"\REXJ-RST.xml";
            if (System.IO.File.Exists(xmlpath))
            {
                System.Xml.Linq.XDocument xdoc = System.Xml.Linq.XDocument.Load(xmlpath);
                version = xdoc?.Root?.Element("Version")?.Value;
            }

            if (version == null)
            {
                // If file missing or unreadable, return the DLL version
                version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }

            version = $"[Ver.{version}]";

            return version;
        }

    }



    class FamilyOption : IFamilyLoadOptions
    {
        public bool OnFamilyFound(
          bool familyInUse,
          out bool overwriteParameterValues)
        {
            overwriteParameterValues = true;
            return true;
        }

        public bool OnSharedFamilyFound(
          Family sharedFamily,
          bool familyInUse,
          out FamilySource source,
          out bool overwriteParameterValues)
        {
            source = FamilySource.Family;
            overwriteParameterValues = true;
            return true;
        }
    }
}
