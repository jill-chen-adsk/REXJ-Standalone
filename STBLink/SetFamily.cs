using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Autodesk;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace STBLink
{
    class SetFamily
    {
        //柱
        internal static FamilyStructure.ClmFamilyName ClmFName = new FamilyStructure.ClmFamilyName(); //柱部材全てのファミリ名
        internal static FamilyStructure.ClmFamilyName PClmFName = new FamilyStructure.ClmFamilyName(); //柱部材全てのファミリ名
        internal static FamilyStructure.RC_Clm_Re RCClmRe = new FamilyStructure.RC_Clm_Re();
        internal static FamilyStructure.RC_Clm_Ro RCClmRo = new FamilyStructure.RC_Clm_Ro();
        internal static FamilyStructure.S_Clm_H SClmH = new FamilyStructure.S_Clm_H();
        internal static FamilyStructure.S_Clm_BH SClmBH = new FamilyStructure.S_Clm_BH();
        internal static FamilyStructure.S_Clm_Box SClmBox = new FamilyStructure.S_Clm_Box();
        internal static FamilyStructure.S_Clm_BBox SClmBBox = new FamilyStructure.S_Clm_BBox();
        internal static FamilyStructure.S_Clm_Pipe SClmPipe = new FamilyStructure.S_Clm_Pipe();
        internal static FamilyStructure.S_Clm_T SClmT = new FamilyStructure.S_Clm_T();
        internal static FamilyStructure.S_Clm_C SClmC = new FamilyStructure.S_Clm_C();
        internal static FamilyStructure.S_Clm_L SClmL = new FamilyStructure.S_Clm_L();
        internal static FamilyStructure.SRC_Clm_H SRCClmH = new FamilyStructure.SRC_Clm_H();
        internal static FamilyStructure.SRC_Clm_Cross SRCClmCross = new FamilyStructure.SRC_Clm_Cross();
        internal static FamilyStructure.SRC_Clm_T SRCClmT = new FamilyStructure.SRC_Clm_T();
        internal static FamilyStructure.SRC_Clm_H_Rou SRCClmH_Rou = new FamilyStructure.SRC_Clm_H_Rou();
        internal static FamilyStructure.SRC_Clm_Cross_Rou SRCClmCross_Rou = new FamilyStructure.SRC_Clm_Cross_Rou();
        internal static FamilyStructure.SRC_Clm_T_Rou SRCClmT_Rou = new FamilyStructure.SRC_Clm_T_Rou();
        internal static FamilyStructure.CFT_Clm_Box CFTClmBox = new FamilyStructure.CFT_Clm_Box();
        internal static FamilyStructure.CFT_Clm_Pipe CFTClmPipe = new FamilyStructure.CFT_Clm_Pipe();        

        //基礎梁
        internal static FamilyStructure.BClmFamilyName BClmFName = new FamilyStructure.BClmFamilyName();
        
        //梁
        internal static FamilyStructure.GirFamilyName GirFName = new FamilyStructure.GirFamilyName();
        internal static FamilyStructure.GirFamilyName BeamFName = new FamilyStructure.GirFamilyName();
        /// <summary>基礎梁
        /// </summary>
        internal static FamilyStructure.RC_Gir RCGir_F = new FamilyStructure.RC_Gir();
        /// <summary>ハンチ付基礎梁
        /// </summary>
        internal static FamilyStructure.RC_Gir RCGir_F_Haunch = new FamilyStructure.RC_Gir();
        /// <summary>基礎小梁
        /// </summary>
        internal static FamilyStructure.RC_Gir RCBeam_F = new FamilyStructure.RC_Gir();
        /// <summary>ハンチ付基礎小梁
        /// </summary>
        internal static FamilyStructure.RC_Gir RCBeam_F_Haunch = new FamilyStructure.RC_Gir();
        /// <summary>RC大梁
        /// </summary>
        internal static FamilyStructure.RC_Gir RCGir = new FamilyStructure.RC_Gir();
        /// <summary>ハンチ付RC大梁
        /// </summary>
        internal static FamilyStructure.RC_Gir RCGir_Haunch = new FamilyStructure.RC_Gir();
        /// <summary>RC小梁
        /// </summary>
        internal static FamilyStructure.RC_Gir RCBeam = new FamilyStructure.RC_Gir();
        /// <summary>ハンチ付RC小梁
        /// </summary>
        internal static FamilyStructure.RC_Gir RCBeam_Haunch = new FamilyStructure.RC_Gir();
        internal static FamilyStructure.S_Gir_H SGirH = new FamilyStructure.S_Gir_H();
        internal static FamilyStructure.S_Gir_BH SGirBH = new FamilyStructure.S_Gir_BH();
        internal static FamilyStructure.S_Gir_C SGirC = new FamilyStructure.S_Gir_C();
        internal static FamilyStructure.S_Gir_L SGirL = new FamilyStructure.S_Gir_L();
        internal static FamilyStructure.S_Gir_LipC SGirLipC = new FamilyStructure.S_Gir_LipC();
        /// <summary>ハンチ付S大梁
        /// </summary>
        internal static FamilyStructure.S_Gir_H SGirH_Haunch = new FamilyStructure.S_Gir_H();
        internal static FamilyStructure.SRC_Gir SRCGirH = new FamilyStructure.SRC_Gir();        
        internal static FamilyStructure.S_Gir_H SBeamH = new FamilyStructure.S_Gir_H();
        internal static FamilyStructure.S_Gir_BH SBeamBH = new FamilyStructure.S_Gir_BH();
        internal static FamilyStructure.S_Gir_C SBeamC = new FamilyStructure.S_Gir_C();
        internal static FamilyStructure.S_Gir_L SBeamL = new FamilyStructure.S_Gir_L();
        internal static FamilyStructure.S_Gir_LipC SBeamLipC = new FamilyStructure.S_Gir_LipC();
        /// <summary>ハンチ付S小梁
        /// </summary>
        internal static FamilyStructure.S_Gir_H SBeamH_Haunch = new FamilyStructure.S_Gir_H();
        internal static FamilyStructure.SRC_Gir SRCBeamH = new FamilyStructure.SRC_Gir();

        //片持梁
        internal static FamilyStructure.CGirFamilyName CGirFName = new FamilyStructure.CGirFamilyName();
        internal static FamilyStructure.CGirFamilyName CBeamFName = new FamilyStructure.CGirFamilyName();
        /// <summary>片持基礎梁
        /// </summary>
        internal static FamilyStructure.RC_CGir RCCGir_F = new FamilyStructure.RC_CGir();
        /// <summary>片持基礎小梁
        /// </summary>
        internal static FamilyStructure.RC_CGir RCCBeam_F = new FamilyStructure.RC_CGir();
        internal static FamilyStructure.RC_CGir RCCGir = new FamilyStructure.RC_CGir();
        internal static FamilyStructure.S_CGir_H SCGirH = new FamilyStructure.S_CGir_H();
        internal static FamilyStructure.S_CGir_H SCGirBH = new FamilyStructure.S_CGir_H();
        internal static FamilyStructure.S_CGir_H SCBeamBH = new FamilyStructure.S_CGir_H();
        internal static FamilyStructure.S_Gir_C SCGirC = new FamilyStructure.S_Gir_C();
        internal static FamilyStructure.S_Gir_L SCGirL = new FamilyStructure.S_Gir_L();
        internal static FamilyStructure.S_Gir_LipC SCGirLipC = new FamilyStructure.S_Gir_LipC();
        internal static FamilyStructure.SRC_CGir SRCCGirH = new FamilyStructure.SRC_CGir();
        internal static FamilyStructure.RC_CGir RCCBeam = new FamilyStructure.RC_CGir();
        internal static FamilyStructure.S_CGir_H SCBeamH = new FamilyStructure.S_CGir_H();
        internal static FamilyStructure.S_Gir_C SCBeamC = new FamilyStructure.S_Gir_C();
        internal static FamilyStructure.S_Gir_L SCBeamL = new FamilyStructure.S_Gir_L();
        internal static FamilyStructure.S_Gir_LipC SCBeamLipC = new FamilyStructure.S_Gir_LipC();
        internal static FamilyStructure.SRC_CGir SRCCBeamH = new FamilyStructure.SRC_CGir();

        //Sブレース
        internal static FamilyStructure.BraFamilyName SBraFName = new FamilyStructure.BraFamilyName();
        internal static FamilyStructure.S_Bra_H SBraH = new FamilyStructure.S_Bra_H();
        internal static FamilyStructure.S_Bra_BH SBraBH = new FamilyStructure.S_Bra_BH();
        internal static FamilyStructure.S_Bra_Box SBraBox = new FamilyStructure.S_Bra_Box();
        internal static FamilyStructure.S_Bra_BBox SBraBBox = new FamilyStructure.S_Bra_BBox();
        internal static FamilyStructure.S_Bra_Pipe SBraPipe = new FamilyStructure.S_Bra_Pipe();
        internal static FamilyStructure.S_Bra_C SBraC = new FamilyStructure.S_Bra_C();
        internal static FamilyStructure.S_Bra_L SBraL = new FamilyStructure.S_Bra_L();
        internal static FamilyStructure.S_Bra_LipC SBraLipC = new FamilyStructure.S_Bra_LipC();
        internal static FamilyStructure.S_Bra_FB SBraFB = new FamilyStructure.S_Bra_FB();
        internal static FamilyStructure.S_Bra_RollBar SBraRollBar = new FamilyStructure.S_Bra_RollBar();

        internal static FamilyStructure.Slab Slab = new FamilyStructure.Slab();
        internal static FamilyStructure.Wall Wall = new FamilyStructure.Wall();

        //基礎
        internal static FamilyStructure.FoundationFamilyName FoFName = new FamilyStructure.FoundationFamilyName();
        internal static FamilyStructure.Foundation_Rect FRect = new FamilyStructure.Foundation_Rect();
        internal static FamilyStructure.Foundation_Tapered_Rect FTRect = new FamilyStructure.Foundation_Tapered_Rect();
        internal static FamilyStructure.Foundation_Triangle FTri = new FamilyStructure.Foundation_Triangle();
        internal static FamilyStructure.Foundation_Equi_Triangle FETriangle = new FamilyStructure.Foundation_Equi_Triangle();
        internal static FamilyStructure.Foundation_Octagon FOct = new FamilyStructure.Foundation_Octagon();
        internal static FamilyStructure.Foundation_Continuous FConti = new FamilyStructure.Foundation_Continuous();
        internal static FamilyStructure.Pile CastinPile = new FamilyStructure.Pile();
        internal static FamilyStructure.Pile_2 PrecastPile = new FamilyStructure.Pile_2();

        //STB2.0
        internal static FamilyStructure.Pile_S Pile_S = new FamilyStructure.Pile_S();
        internal static FamilyStructure.Pile_PHC Pile_PHC = new FamilyStructure.Pile_PHC();
        internal static FamilyStructure.Pile_ST Pile_ST = new FamilyStructure.Pile_ST();
        internal static FamilyStructure.Pile_SC Pile_SC = new FamilyStructure.Pile_SC();
        internal static FamilyStructure.Pile_PRC Pile_PRC = new FamilyStructure.Pile_PRC();
        internal static FamilyStructure.Pile_CPRC Pile_CPRC = new FamilyStructure.Pile_CPRC();


        //使用しない
        //internal static FamilyStructure.Pile_Straight PStraight = new FamilyStructure.Pile_Straight();
        //internal static FamilyStructure.Pile_Extended_Foot PFoot = new FamilyStructure.Pile_Extended_Foot();
        //internal static FamilyStructure.Pile_Extended_Top PTop = new FamilyStructure.Pile_Extended_Top();
        //internal static FamilyStructure.Pile_Extended_Top_Foot PTopFoot = new FamilyStructure.Pile_Extended_Top_Foot();

        internal static bool LoadTable()
        {
            //初期化
            ClmFName = new FamilyStructure.ClmFamilyName();
            PClmFName = new FamilyStructure.ClmFamilyName();
            BClmFName = new FamilyStructure.BClmFamilyName();
            GirFName = new FamilyStructure.GirFamilyName();
            BeamFName = new FamilyStructure.GirFamilyName();
            CGirFName = new FamilyStructure.CGirFamilyName();
            CBeamFName = new FamilyStructure.CGirFamilyName();
            SBraFName = new FamilyStructure.BraFamilyName();
            FoFName = new FamilyStructure.FoundationFamilyName();

            bool ret = false;
            string familyTableFile = RevitLNK.familyTableFile;
            try
            {
                ret = ReadTable(familyTableFile);
                if (ret == false)
                {
                    //バージョンが異なる
                    return false;
                }

                //CreateSLMTableFile();

                return ret;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// ファミリテーブルの読み込み
        /// </summary>
        /// <param name="familyTableFile"></param>
        /// <returns></returns>
        internal static bool ReadTable(string familyTableFile)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            StreamReader sr = new StreamReader(familyTableFile, Encoding.GetEncoding("Shift_JIS"));
            RCClmRe = new FamilyStructure.RC_Clm_Re();
            RCClmRo = new FamilyStructure.RC_Clm_Ro();
            SClmH = new FamilyStructure.S_Clm_H();
            SClmBH = new FamilyStructure.S_Clm_BH();
            SClmBox = new FamilyStructure.S_Clm_Box();
            SClmBBox = new FamilyStructure.S_Clm_BBox();
            SClmPipe = new FamilyStructure.S_Clm_Pipe();
            SClmT = new FamilyStructure.S_Clm_T();
            SClmC = new FamilyStructure.S_Clm_C();
            SClmL = new FamilyStructure.S_Clm_L(); ;
            SRCClmH = new FamilyStructure.SRC_Clm_H();
            SRCClmCross = new FamilyStructure.SRC_Clm_Cross();
            SRCClmT = new FamilyStructure.SRC_Clm_T();
            SRCClmH_Rou = new FamilyStructure.SRC_Clm_H_Rou();
            SRCClmCross_Rou = new FamilyStructure.SRC_Clm_Cross_Rou();
            SRCClmT_Rou = new FamilyStructure.SRC_Clm_T_Rou();
            CFTClmBox = new FamilyStructure.CFT_Clm_Box();
            CFTClmPipe = new FamilyStructure.CFT_Clm_Pipe();

            //梁
            RCGir_F = new FamilyStructure.RC_Gir
            {
                FamilyName = "基礎梁"
            };
            RCGir_F_Haunch = new FamilyStructure.RC_Gir
            {
                FamilyName = "ハンチ付き基礎梁"
            };
            RCBeam_F = new FamilyStructure.RC_Gir
            {
                FamilyName = "基礎小梁"
            };
            RCBeam_F_Haunch = new FamilyStructure.RC_Gir
            {
                FamilyName = "ハンチ付き基礎小梁"
            };
            RCGir = new FamilyStructure.RC_Gir
            {
                FamilyName = "RC大梁"
            };
            RCGir_Haunch = new FamilyStructure.RC_Gir
            {
                FamilyName = "ハンチ付きRC大梁"
            };
            RCBeam = new FamilyStructure.RC_Gir
            {
                FamilyName = "RC小梁"
            };
            RCBeam_Haunch = new FamilyStructure.RC_Gir
            {
                FamilyName = "ハンチ付きRC小梁"
            };
            SGirH = new FamilyStructure.S_Gir_H
            {
                FamilyName = "S大梁"
            };
            SGirBH = new FamilyStructure.S_Gir_BH
            {
                FamilyName = "S梁組立H形鋼"
            };
            SGirC = new FamilyStructure.S_Gir_C
            {
                FamilyName = "S梁溝形鋼"
            };
            SGirL = new FamilyStructure.S_Gir_L
            {
                FamilyName = "S梁山形鋼"
            };
            SGirLipC = new FamilyStructure.S_Gir_LipC
            {
                FamilyName = "S梁リップ溝形鋼"
            };
            SGirH_Haunch = new FamilyStructure.S_Gir_H
            {
                FamilyName = "ハンチ付きS大梁"
            };
            SBeamH = new FamilyStructure.S_Gir_H
            {
                FamilyName = "S小梁"
            };
            SBeamBH = new FamilyStructure.S_Gir_BH
            {
                FamilyName = "S小梁組立H形鋼"
            };
            SBeamC = new FamilyStructure.S_Gir_C
            {
                FamilyName = "S小梁溝形鋼"
            };
            SBeamL = new FamilyStructure.S_Gir_L
            {
                FamilyName = "S小梁山形鋼"
            };
            SBeamLipC = new FamilyStructure.S_Gir_LipC
            {
                FamilyName = "S小梁リップ溝形鋼"
            };
            SBeamH_Haunch = new FamilyStructure.S_Gir_H
            {
                FamilyName = "ハンチ付きS小梁"
            };
            SRCGirH = new FamilyStructure.SRC_Gir
            {
                FamilyName = "SRC大梁"
            };
            SRCBeamH = new FamilyStructure.SRC_Gir
            {
                FamilyName = "SRC小梁"
            };

            //片持梁
            RCCGir_F = new FamilyStructure.RC_CGir
            {
                FamilyName = "片持基礎梁"
            };
            RCCGir = new FamilyStructure.RC_CGir
            {
                FamilyName = "RC片持梁"
            };
            SCGirH = new FamilyStructure.S_CGir_H
            {
                FamilyName = "S片持梁"
            };
            SCGirBH = new FamilyStructure.S_CGir_H
            {
                FamilyName = "S片持梁組立H形鋼"
            };
            SCGirC = new FamilyStructure.S_Gir_C
            {
                FamilyName = "S片持梁溝形鋼"
            };
            SCGirL = new FamilyStructure.S_Gir_L
            {
                FamilyName = "S片持梁山形鋼"
            };
            SCGirLipC = new FamilyStructure.S_Gir_LipC
            {
                FamilyName = "S片持梁リップ溝形鋼"
            };
            SRCCGirH = new FamilyStructure.SRC_CGir
            {
                FamilyName = "SRC片持梁"
            };
            RCCBeam_F = new FamilyStructure.RC_CGir
            {
                FamilyName = "片持基礎小梁"
            };
            RCCBeam = new FamilyStructure.RC_CGir
            {
                FamilyName = "RC片持小梁"
            };
            SCBeamH = new FamilyStructure.S_CGir_H
            {
                FamilyName = "S片持小梁"
            };
            SCBeamBH = new FamilyStructure.S_CGir_H
            {
                FamilyName = "S片持小梁組立H形鋼"
            };
            SCBeamC = new FamilyStructure.S_Gir_C
            {
                FamilyName = "S片持小梁溝形鋼"
            };
            SCBeamL = new FamilyStructure.S_Gir_L
            {
                FamilyName = "S片持小梁山形鋼"
            };
            SCBeamLipC = new FamilyStructure.S_Gir_LipC
            {
                FamilyName = "S片持小梁リップ溝形鋼"
            };
            SRCCBeamH = new FamilyStructure.SRC_CGir
            {
                FamilyName = "SRC片持小梁"
            };

            //Sブレース
            SBraH = new FamilyStructure.S_Bra_H();
            SBraBH = new FamilyStructure.S_Bra_BH(); ;
            SBraBox = new FamilyStructure.S_Bra_Box();
            SBraBBox = new FamilyStructure.S_Bra_BBox();
            SBraPipe = new FamilyStructure.S_Bra_Pipe();
            SBraC = new FamilyStructure.S_Bra_C();
            SBraL = new FamilyStructure.S_Bra_L();
            SBraLipC = new FamilyStructure.S_Bra_LipC();
            SBraFB = new FamilyStructure.S_Bra_FB();
            SBraRollBar = new FamilyStructure.S_Bra_RollBar();

            //基礎
            FRect = new FamilyStructure.Foundation_Rect();
            FTRect = new FamilyStructure.Foundation_Tapered_Rect
            {
                FamilyName = "RC基礎矩形テーパー"
            };
            FTri = new FamilyStructure.Foundation_Triangle();
            FETriangle = new FamilyStructure.Foundation_Equi_Triangle();
            FOct = new FamilyStructure.Foundation_Octagon();
            FConti = new FamilyStructure.Foundation_Continuous();
            CastinPile = new FamilyStructure.Pile();
            PrecastPile = new FamilyStructure.Pile_2
            {
                FamilyName = "既製杭"
            };

            string str = "";
            string[] jouken = { " : " }; //文字を切り取る条件
            while (sr.Peek() >= 0)
            {
                do
                {
                    str = sr.ReadLine();

                    string[] split = str.Split(jouken, StringSplitOptions.None);
                    if (split.Length > 1)
                    {
                        if (split[0] == "Version")
                        {
                            bool ret = true;
                            ret &= double.TryParse(split[1], out double v1);
                            ret &= double.TryParse(RevitLNK.RFAtableVersion, out double v2);

                            if (ret && v1 >= v2)
                            {
                                //テーブルファイルのバージョンが、現在対応済みのバージョンより新しいものならばOK
                            }
                            else
                            {
                                return false;
                            }

                            continue;
                        }
                    }
                    if (split.Length < 2) { continue; }
                    string hed = split[0];
                    string dat = split[1];
                    string set = "";
                    if (split.Length < 3) //ファミリ名の設定
                    {
                        //柱
                        if (hed == RCClmRe.FamilyName) { RCClmRe.FamilyName = dat; }
                        if (hed == RCClmRo.FamilyName) { RCClmRo.FamilyName = dat; }
                        if (hed == SClmH.FamilyName) { SClmH.FamilyName = dat; }
                        if (hed == SClmBH.FamilyName) { SClmBH.FamilyName = dat; }
                        if (hed == SClmBox.FamilyName) { SClmBox.FamilyName = dat; }
                        if (hed == SClmBBox.FamilyName) { SClmBBox.FamilyName = dat; }
                        if (hed == SClmPipe.FamilyName) { SClmPipe.FamilyName = dat; }
                        if (hed == SClmT.FamilyName) { SClmT.FamilyName = dat; }
                        if (hed == SClmC.FamilyName) { SClmC.FamilyName = dat; }
                        if (hed == SClmL.FamilyName) { SClmL.FamilyName = dat; }
                        if (hed == SRCClmH.FamilyName) { SRCClmH.FamilyName = dat; }
                        if (hed == SRCClmCross.FamilyName) { SRCClmCross.FamilyName = dat; }
                        if (hed == SRCClmT.FamilyName) { SRCClmT.FamilyName = dat; }
                        if (hed == SRCClmH_Rou.FamilyName) { SRCClmH_Rou.FamilyName = dat; }
                        if (hed == SRCClmCross_Rou.FamilyName) { SRCClmCross_Rou.FamilyName = dat; }
                        if (hed == SRCClmT_Rou.FamilyName) { SRCClmT_Rou.FamilyName = dat; }
                        if (hed == CFTClmBox.FamilyName) { CFTClmBox.FamilyName = dat; }
                        if (hed == CFTClmPipe.FamilyName) { CFTClmPipe.FamilyName = dat; }

                        //梁
                        if (hed == RCGir_F.FamilyName) { RCGir_F.FamilyName = dat; }
                        if (hed == RCGir_F_Haunch.FamilyName) { RCGir_F_Haunch.FamilyName = dat; }
                        if (hed == RCBeam_F.FamilyName) { RCBeam_F.FamilyName = dat; }
                        if (hed == RCBeam_F_Haunch.FamilyName) { RCBeam_F_Haunch.FamilyName = dat; }
                        if (hed == RCGir.FamilyName) { RCGir.FamilyName = dat; }
                        if (hed == RCGir_Haunch.FamilyName) { RCGir_Haunch.FamilyName = dat; }
                        if (hed == RCBeam.FamilyName) { RCBeam.FamilyName = dat; }
                        if (hed == RCBeam_Haunch.FamilyName) { RCBeam_Haunch.FamilyName = dat; }
                        if (hed == SGirH.FamilyName) { SGirH.FamilyName = dat; }
                        if (hed == SGirBH.FamilyName) { SGirBH.FamilyName = dat; }
                        if (hed == SGirC.FamilyName) { SGirC.FamilyName = dat; }
                        if (hed == SGirL.FamilyName) { SGirL.FamilyName = dat; }
                        if (hed == SGirLipC.FamilyName) { SGirLipC.FamilyName = dat; }
                        if (hed == SGirH_Haunch.FamilyName) { SGirH_Haunch.FamilyName = dat; }
                        if (hed == SRCGirH.FamilyName) { SRCGirH.FamilyName = dat; }
                        if (hed == SBeamH.FamilyName) { SBeamH.FamilyName = dat; }
                        if (hed == SBeamBH.FamilyName) { SBeamBH.FamilyName = dat; }
                        if (hed == SBeamC.FamilyName) { SBeamC.FamilyName = dat; }
                        if (hed == SBeamL.FamilyName) { SBeamL.FamilyName = dat; }
                        if (hed == SBeamLipC.FamilyName) { SBeamLipC.FamilyName = dat; }
                        if (hed == SBeamH_Haunch.FamilyName) { SBeamH_Haunch.FamilyName = dat; }
                        if (hed == SRCBeamH.FamilyName) { SRCBeamH.FamilyName = dat; }

                        //片持梁
                        if (hed == RCCGir_F.FamilyName) { RCCGir_F.FamilyName = dat; }
                        if (hed == RCCBeam_F.FamilyName) { RCCBeam_F.FamilyName = dat; }
                        if (hed == RCCGir.FamilyName) { RCCGir.FamilyName = dat; }
                        if (hed == SCGirH.FamilyName) { SCGirH.FamilyName = dat; }
                        if (hed == SCGirBH.FamilyName) { SCGirBH.FamilyName = dat; }
                        if (hed == SCGirC.FamilyName) { SCGirC.FamilyName = dat; }
                        if (hed == SCGirL.FamilyName) { SCGirL.FamilyName = dat; }
                        if (hed == SCGirLipC.FamilyName) { SCGirLipC.FamilyName = dat; }
                        if (hed == SRCCGirH.FamilyName) { SRCCGirH.FamilyName = dat; }
                        if (hed == RCCBeam.FamilyName) { RCCBeam.FamilyName = dat; }
                        if (hed == SCBeamH.FamilyName) { SCBeamH.FamilyName = dat; }
                        if (hed == SCBeamBH.FamilyName) { SCBeamBH.FamilyName = dat; }
                        if (hed == SCBeamC.FamilyName) { SCBeamC.FamilyName = dat; }
                        if (hed == SCBeamL.FamilyName) { SCBeamL.FamilyName = dat; }
                        if (hed == SCBeamLipC.FamilyName) { SCBeamLipC.FamilyName = dat; }
                        if (hed == SRCCBeamH.FamilyName) { SRCCBeamH.FamilyName = dat; }

                        //Sブレース
                        if (hed == SBraH.FamilyName) { SBraH.FamilyName = dat; }
                        if (hed == SBraBH.FamilyName) { SBraBH.FamilyName = dat; }
                        if (hed == SBraBox.FamilyName) { SBraBox.FamilyName = dat; }
                        if (hed == SBraBBox.FamilyName) { SBraBBox.FamilyName = dat; }
                        if (hed == SBraPipe.FamilyName) { SBraPipe.FamilyName = dat; }
                        if (hed == SBraC.FamilyName) { SBraC.FamilyName = dat; }
                        if (hed == SBraL.FamilyName) { SBraL.FamilyName = dat; }
                        if (hed == SBraLipC.FamilyName) { SBraLipC.FamilyName = dat; }
                        if (hed == SBraFB.FamilyName) { SBraFB.FamilyName = dat; }
                        if (hed == SBraRollBar.FamilyName) { SBraRollBar.FamilyName = dat; }

                        //基礎
                        if (hed == FRect.FamilyName) { FRect.FamilyName = dat; }
                        if (hed == FTRect.FamilyName) { FTRect.FamilyName = dat; }
                        if (hed == FTri.FamilyName) { FTri.FamilyName = dat; }
                        if (hed == FETriangle.FamilyName) { FETriangle.FamilyName = dat; }
                        if (hed == FOct.FamilyName) { FOct.FamilyName = dat; }
                        if (hed == FConti.FamilyName) { FConti.FamilyName = dat; }
                        if (hed == CastinPile.FamilyName) { CastinPile.FamilyName = dat; }
                        if (hed == PrecastPile.FamilyName) { PrecastPile.FamilyName = dat; }

                        //STB2.0対応
                        if (hed == "RC杭") { CastinPile.FamilyName = dat; }
                        if (hed == "鋼管杭") { Pile_S.FamilyName = dat; }
                        if (hed == "既製杭 PHC") { Pile_PHC.FamilyName = dat; }
                        if (hed == "既製杭 ST") { Pile_ST.FamilyName = dat; }
                        if (hed == "既製杭 SC") { Pile_SC.FamilyName = dat; }
                        if (hed == "既製杭 PRC") { Pile_PRC.FamilyName = dat; }
                        if (hed == "既製杭 CPRC") { Pile_CPRC.FamilyName = dat; }

                    }
                    else //パラメータ名の設定
                    {

                        set = split[2];
                        switch (hed)
                        {
                            //柱
                            case "RC柱":
                                SetRCClmRecPara(RCClmRe, dat, set);
                                break;
                            case "RC円柱":
                                SetRCClmRouPara(RCClmRo, dat, set);
                                break;
                            case "S柱H形鋼":
                                SetSClmHPara(SClmH, dat, set);
                                break;
                            case "S柱組立H形鋼":
                                SetSClmBHPara(SClmBH, dat, set);
                                break;
                            case "S柱角形鋼管":
                                SetSClmBoxPara(SClmBox, dat, set);
                                break;
                            case "S柱組立角形鋼管":
                                SetSClmBBoxPara(SClmBBox, dat, set);
                                break;
                            case "S柱鋼管":
                                SetSClmPipePara(SClmPipe, dat, set);
                                break;
                            case "S柱T形鋼":
                                SetSClmTPara(SClmT, dat, set);
                                break;
                            case "S柱溝形鋼":
                                SetSClmCPara(SClmC, dat, set);
                                break;
                            case "S柱山形鋼":
                                SetSClmLPara(SClmL, dat, set);
                                break;
                            case "SRC柱H形":
                                SetSRCClmHPara(SRCClmH, dat, set);
                                break;
                            case "SRC柱＋形":
                                SetSRCClmCrossPara(SRCClmCross, dat, set);
                                break;
                            case "SRC柱T形":
                                SetSRCClmTPara(SRCClmT, dat, set);
                                break;
                            case "SRC柱H形(円形)":
                                SetSRCClmH_RouPara(SRCClmH_Rou, dat, set);
                                break;
                            case "SRC柱＋形(円形)":
                                SetSRCClmCross_RouPara(SRCClmCross_Rou, dat, set);
                                break;
                            case "SRC柱T形(円形)":
                                SetSRCClmT_RouPara(SRCClmT_Rou, dat, set);
                                break;
                            case "CFT柱角形鋼管":
                                SetCFTClmBoxPara(CFTClmBox, dat, set);
                                break;
                            case "CFT柱鋼管":
                                SetCFTClmPipePara(CFTClmPipe, dat, set);
                                break;
                            //梁・ブレース
                            case "RC梁":
                                SetRCGir(RCGir_F, dat, set);
                                SetRCGir(RCGir_F_Haunch, dat, set);
                                SetRCGir(RCBeam_F, dat, set);
                                SetRCGir(RCBeam_F_Haunch, dat, set);
                                SetRCGir(RCGir, dat, set);
                                SetRCGir(RCGir_Haunch, dat, set);
                                SetRCGir(RCBeam, dat, set);
                                SetRCGir(RCBeam_Haunch, dat, set);
                                break;
                            case "S梁":
                                SetSGir(SGirH, dat, set);
                                SetSGir(SGirH_Haunch, dat, set);
                                SetSGir(SBeamH, dat, set);
                                SetSGir(SBeamH_Haunch, dat, set);
                                SetSBra(SBraH, dat, set);
                                break;
                            case "S梁組立H形鋼":
                                SetSGir_BH(SGirBH, dat, set);
                                SetSGir_BH(SBeamBH, dat, set);                               
                                SetSBra_BH(SBraBH, dat, set);
                                break;
                            case "S梁溝形鋼":
                                SetSGir_C(SGirC, dat, set);
                                SetSGir_C(SBeamC, dat, set);
                                SetSGir_C(SCGirC, dat, set);
                                SetSGir_C(SCBeamC, dat, set);
                                SetSBra_C(SBraC, dat, set);
                                break;
                            case "S梁山形鋼":
                                SetSGir_L(SGirL, dat, set);
                                SetSGir_L(SBeamL, dat, set);
                                SetSGir_L(SCGirL, dat, set);
                                SetSGir_L(SCBeamL, dat, set);
                                SetSBra_L(SBraL, dat, set);
                                break;
                            case "S梁リップ溝形鋼":
                                SetSGir_LipC(SGirLipC, dat, set);
                                SetSGir_LipC(SBeamLipC, dat, set);
                                SetSGir_LipC(SCGirLipC, dat, set);
                                SetSGir_LipC(SCBeamLipC, dat, set);
                                SetSBra_LipC(SBraLipC, dat, set);
                                break;                                
                            case "SRC大梁":
                                SetSRCGir(SRCGirH, dat, set);
                                SetSRCGir(SRCBeamH, dat, set);
                                break;
                            case "RC片持梁":
                                SetRCCGir(RCCGir, dat, set);
                                SetRCCGir(RCCGir_F, dat, set);
                                SetRCCGir(RCCBeam, dat, set);
                                SetRCCGir(RCCBeam_F, dat, set);
                                break;
                            case "S片持梁":
                                SetSCGir(SCGirH, dat, set);
                                SetSCGir(SCBeamH, dat, set);
                                SetSCGir(SCGirBH, dat, set);
                                SetSCGir(SCBeamBH, dat, set);
                                break;
                            case "SRC片持梁":
                                SetSRCCGir(SRCCGirH, dat, set);
                                SetSRCCGir(SRCCBeamH, dat, set);
                                break;
                            //ブレース
                            case "ブレース角形鋼管":
                                SetSBra_Box(SBraBox, dat, set);
                                break;
                            case "ブレース組立角形鋼管":
                                SetSBra_BBox(SBraBBox, dat, set);
                                break;
                            case "ブレース円形鋼管":
                                SetSBra_Pipe(SBraPipe, dat, set);
                                break;
                            case "ブレースフラットバー":
                                SetSBra_FB(SBraFB, dat, set);
                                break;
                            case "ブレース丸鋼":
                                SetSBra_Bar(SBraRollBar, dat, set);
                                break;
                            case "床":
                                SetSlab(Slab, dat, set);
                                break;
                            case "壁":
                                SetWall(Wall, dat, set);
                                break;
                            //基礎
                            case "RC基礎矩形":
                                SetFRec(FRect, dat, set);
                                break;
                            case "RC基礎矩形テーパー":
                                SetFTRec(FTRect, dat, set);
                                break;
                            case "RC基礎三角":
                                SetFTri(FTri, dat, set);
                                break;
                            case "RC基礎正三角形":
                                SetFEqui_Tri(FETriangle, dat, set);
                                break;
                            case "RC基礎八角形":
                                SetFOcta(FOct, dat, set);
                                break;
                            case "布基礎":
                                SetFConti(FConti, dat, set);
                                break;
                            case "場所打ち杭":
                                SetCastinpile(CastinPile, dat, set);
                                break;
                            case "既製杭":
                                SetPrecastpile(PrecastPile, dat, set);
                                break;

                            //STB2.0
                            case "RC杭":
                                SetCastinpile(CastinPile, dat, set);
                                break;
                            case "鋼管杭":
                                SetPile(Pile_S, dat, set);
                                break;
                            case "既製杭 PHC":
                                SetPile(Pile_PHC, dat, set);
                                break;
                            case "既製杭 ST":
                                SetPile(Pile_ST, dat, set);
                                break;
                            case "既製杭 SC":
                                SetPile(Pile_SC, dat, set);
                                break;
                            case "既製杭 PRC":
                                SetPile(Pile_PRC, dat, set);
                                break;
                            case "既製杭 CPRC":
                                SetPile(Pile_CPRC, dat, set);
                                break;

                        }
                    }
                } while (false);
            }
            sr.Close();

            SetClmFamilyName();
            SetBClmFamilyName();
            SetGirFamilyName();
            SetBeamFamilyName();
            SetCGirFamilyName();
            SetCBeamFamilyName();
            SetBraFamilyName();
            SetFoundationFamilyName();
            return true;

        }

        internal static bool LoadBaseTable()
        {
            bool ret = false;
            string TableFile = RevitLNK.BaseTableFile;
            try
            {
                if (File.Exists(TableFile) == false)
                {
                    //ログに記録

                    return false;
                }
                else
                {
                    ret = ReadBaseTable(TableFile);
                    if (ret == false)
                    {
                        //バージョンが異なる
                        return false;
                    }
                }
                return ret;
            }
            catch (Exception)
            {
                return false;
            }
        }
        internal static bool ReadBaseTable(string tablefile)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            bool ret = true;
            RevitLNK.BClm = new List<RevitLNK.BaseColumn>();
            StreamReader sr = new StreamReader(tablefile, Encoding.GetEncoding("Shift_JIS"));

            string str = "";
            string[] jouken = { " : " }; //文字を切り取る条件
            string pass = "";
            while (sr.Peek() >= 0)
            {
                do
                {
                    str = sr.ReadLine();

                    string[] split = str.Split(jouken, StringSplitOptions.RemoveEmptyEntries);                   
                    if(split.Count() == 0) { continue; }
                    if(split[0] == "柱脚ファイルパス")
                    {
                        if (split.Length > 1)
                        {
                            pass = split[1];
                        }
                        continue;
                    }
                    if (split.Length < 3) { continue; }
                    RevitLNK.BaseColumn bclm = new RevitLNK.BaseColumn
                    {
                        pass = pass,
                        product_company = split[0],
                        product_code = split[1],
                        rfa_pass = split[2]
                    };
                    if (split.Length > 3)
                    { bclm.typename = split[3]; }
                    RevitLNK.BClm.Add(bclm);                      
                  
                } while (false);
            }
            sr.Close();
            



            return ret;
        }

        #region ファミリ名のセット
        /// <summary>柱ファミリ名のセット
        /// </summary>
        internal static void SetClmFamilyName()
        {
            for (int i = 0; i < LoadFamily.FamilyNameList.Count(); i++)
            {
                ClmFName.FamilyName[0][0] = RCClmRe.FamilyName;
                ClmFName.FamilyName[0][1] = RCClmRo.FamilyName;
                ClmFName.FamilyName[1][0] = SClmH.FamilyName;
                ClmFName.FamilyName[1][1] = SClmBH.FamilyName;
                ClmFName.FamilyName[1][2] = SClmBox.FamilyName;
                ClmFName.FamilyName[1][3] = SClmBBox.FamilyName;
                ClmFName.FamilyName[1][4] = SClmPipe.FamilyName;
                ClmFName.FamilyName[1][5] = SClmT.FamilyName;
                ClmFName.FamilyName[1][6] = SClmC.FamilyName;
                ClmFName.FamilyName[1][7] = SClmL.FamilyName;
                ClmFName.FamilyName[2][0] = SRCClmH.FamilyName;
                ClmFName.FamilyName[2][1] = SRCClmCross.FamilyName;
                ClmFName.FamilyName[2][2] = SRCClmT.FamilyName;
                ClmFName.FamilyName[2][3] = SRCClmH_Rou.FamilyName;
                ClmFName.FamilyName[2][4] = SRCClmCross_Rou.FamilyName;
                ClmFName.FamilyName[2][5] = SRCClmT_Rou.FamilyName;
                ClmFName.FamilyName[3][0] = CFTClmBox.FamilyName;
                ClmFName.FamilyName[3][1] = CFTClmPipe.FamilyName;
                PClmFName.FamilyName[0][0] = RCClmRe.FamilyName;
                PClmFName.FamilyName[0][1] = RCClmRo.FamilyName;
                PClmFName.FamilyName[1][0] = SClmH.FamilyName;
                PClmFName.FamilyName[1][1] = SClmBH.FamilyName;
                PClmFName.FamilyName[1][2] = SClmBox.FamilyName;
                PClmFName.FamilyName[1][3] = SClmBBox.FamilyName;
                PClmFName.FamilyName[1][4] = SClmPipe.FamilyName;
                PClmFName.FamilyName[1][5] = SClmT.FamilyName;
                PClmFName.FamilyName[1][6] = SClmC.FamilyName;
                PClmFName.FamilyName[1][7] = SClmL.FamilyName;
                PClmFName.FamilyName[2][0] = SRCClmH.FamilyName;
                PClmFName.FamilyName[2][1] = SRCClmCross.FamilyName;
                PClmFName.FamilyName[2][2] = SRCClmT.FamilyName;
                PClmFName.FamilyName[2][3] = SRCClmH_Rou.FamilyName;
                PClmFName.FamilyName[2][4] = SRCClmCross_Rou.FamilyName;
                PClmFName.FamilyName[2][5] = SRCClmT_Rou.FamilyName;
                PClmFName.FamilyName[3][0] = CFTClmBox.FamilyName;
                PClmFName.FamilyName[3][1] = CFTClmPipe.FamilyName;
                if (RCClmRe.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    RCClmRe.Loadflg = true;
                    ClmFName.TypeName[0][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[0][0] = true;
                    PClmFName.TypeName[0][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[0][0] = true;
                }
                if (RCClmRo.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    RCClmRo.Loadflg = true;
                    ClmFName.TypeName[0][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[0][1] = true;
                    PClmFName.TypeName[0][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[0][1] = true;
                }
                if (SClmH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SClmH.Loadflg = true;
                    ClmFName.TypeName[1][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[1][0] = true;
                    PClmFName.TypeName[1][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[1][0] = true;
                }
                if(SClmBH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SClmBH.Loadflg = true;
                    ClmFName.TypeName[1][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[1][1] = true;
                    PClmFName.TypeName[1][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[1][1] = true;
                }
                if (SClmBox.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SClmBox.Loadflg = true;
                    ClmFName.TypeName[1][2] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[1][2] = true;
                    PClmFName.TypeName[1][2] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[1][2] = true;
                }
                if (SClmBBox.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SClmBBox.Loadflg = true;
                    ClmFName.TypeName[1][3] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[1][3] = true;
                    PClmFName.TypeName[1][3] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[1][3] = true;
                }
                if (SClmPipe.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SClmPipe.Loadflg = true;
                    ClmFName.TypeName[1][4] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[1][4] = true;
                    PClmFName.TypeName[1][4] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[1][4] = true;
                }
                if(SClmT.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SClmT.Loadflg = true;
                    ClmFName.TypeName[1][5] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[1][5] = true;
                    PClmFName.TypeName[1][5] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[1][5] = true;
                }
                if (SClmC.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SClmC.Loadflg = true;
                    ClmFName.TypeName[1][6] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[1][6] = true;
                    PClmFName.TypeName[1][6] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[1][6] = true;
                }
                if (SClmL.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SClmL.Loadflg = true;
                    ClmFName.TypeName[1][7] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[1][7] = true;
                    PClmFName.TypeName[1][7] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[1][7] = true;
                }
                if (SRCClmH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SRCClmH.Loadflg = true;
                    ClmFName.TypeName[2][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[2][0] = true;
                    PClmFName.TypeName[2][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[2][0] = true;
                }
                if (SRCClmCross.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SRCClmCross.Loadflg = true;
                    ClmFName.TypeName[2][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[2][1] = true;
                    PClmFName.TypeName[2][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[2][1] = true;
                }
                if (SRCClmT.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SRCClmT.Loadflg = true;
                    ClmFName.TypeName[2][2] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[2][2] = true;
                    PClmFName.TypeName[2][2] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[2][2] = true;
                }
                if (SRCClmH_Rou.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SRCClmH_Rou.Loadflg = true;
                    ClmFName.TypeName[2][3] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[2][3] = true;
                    PClmFName.TypeName[2][3] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[2][3] = true;
                }
                if (SRCClmCross_Rou.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SRCClmCross_Rou.Loadflg = true;
                    ClmFName.TypeName[2][4] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[2][4] = true;
                    PClmFName.TypeName[2][4] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[2][4] = true;
                }
                if (SRCClmT_Rou.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SRCClmT_Rou.Loadflg = true;
                    ClmFName.TypeName[2][5] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[2][5] = true;
                    PClmFName.TypeName[2][5] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[2][5] = true;
                }
                if (CFTClmBox.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    CFTClmBox.Loadflg = true;
                    ClmFName.TypeName[3][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[3][0] = true;
                    PClmFName.TypeName[3][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[3][0] = true;
                }
                if (CFTClmPipe.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    CFTClmPipe.Loadflg = true;
                    ClmFName.TypeName[3][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    ClmFName.flg[3][1] = true;
                    PClmFName.TypeName[3][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    PClmFName.flg[3][1] = true;
                }
            }
        }
        /// <summary>基礎柱ファミリ名のセット
        /// </summary>
        internal static void SetBClmFamilyName()
        {
            for (int i = 0; i < LoadFamily.FamilyNameList.Count(); i++)
            {
                BClmFName.FamilyName[0][0] = RCClmRe.FamilyName;
                BClmFName.FamilyName[0][1] = RCClmRo.FamilyName;
                if (RCClmRe.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    RCClmRe.Loadflg = true;
                    BClmFName.FamilyName[0][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    BClmFName.flg[0][0] = true;
                }
                if (RCClmRo.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    RCClmRo.Loadflg = true;
                    BClmFName.FamilyName[0][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    BClmFName.flg[0][1] = true;
                }
                
            }
        }
        /// <summary>梁ファミリ名のセット
        /// </summary>
        internal static void SetGirFamilyName()
        {
            GirFName.FamilyName[0][0] = RCGir_F.FamilyName;
            GirFName.FamilyName[0][1] = RCGir_F_Haunch.FamilyName;
            GirFName.FamilyName[0][2] = RCGir.FamilyName;
            GirFName.FamilyName[0][3] = RCGir_Haunch.FamilyName;
            GirFName.FamilyName[1][0] = SGirH.FamilyName;
            GirFName.FamilyName[1][1] = SGirBH.FamilyName;
            GirFName.FamilyName[1][2] = SGirC.FamilyName;
            GirFName.FamilyName[1][3] = SGirL.FamilyName;
            GirFName.FamilyName[1][4] = SGirLipC.FamilyName;
            GirFName.FamilyName[1][5] = SGirH_Haunch.FamilyName;
            GirFName.FamilyName[2][0] = SRCGirH.FamilyName;
            BeamFName.FamilyName[0][0] = RCBeam_F.FamilyName;
            BeamFName.FamilyName[0][1] = RCBeam_F_Haunch.FamilyName;
            BeamFName.FamilyName[0][2] = RCBeam.FamilyName;
            BeamFName.FamilyName[0][3] = RCBeam_Haunch.FamilyName;
            BeamFName.FamilyName[1][0] = SBeamH.FamilyName;
            BeamFName.FamilyName[1][1] = SBeamBH.FamilyName;
            BeamFName.FamilyName[1][2] = SBeamC.FamilyName;
            BeamFName.FamilyName[1][3] = SBeamL.FamilyName;
            BeamFName.FamilyName[1][4] = SBeamLipC.FamilyName;
            BeamFName.FamilyName[1][5] = SBeamH_Haunch.FamilyName;
            BeamFName.FamilyName[2][0] = SRCBeamH.FamilyName;
            for (int i = 0; i < LoadFamily.FamilyNameList.Count(); i++)
            {
                if (RCGir_F.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    RCGir_F.Loadflg = true;
                    GirFName.TypeName[0][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    GirFName.flg[0][0] = true;
                }
                if (RCGir_F_Haunch.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    RCGir_F_Haunch.Loadflg = true;
                    GirFName.TypeName[0][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    GirFName.flg[0][1] = true;
                }
                if (RCGir.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    RCGir.Loadflg = true;
                    GirFName.TypeName[0][2] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    GirFName.flg[0][2] = true;
                }
                if (RCGir_Haunch.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    RCGir_Haunch.Loadflg = true;
                    GirFName.TypeName[0][3] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    GirFName.flg[0][3] = true;
                }
                if (SGirH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SGirH.Loadflg = true;
                    GirFName.TypeName[1][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    GirFName.flg[1][0] = true;
                }
                if (SGirBH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SGirH.Loadflg = true;
                    GirFName.TypeName[1][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    GirFName.flg[1][1] = true;
                }
                if (SGirC.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SGirC.Loadflg = true;
                    GirFName.TypeName[1][2] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    GirFName.flg[1][2] = true;
                }
                if (SGirL.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SGirC.Loadflg = true;
                    GirFName.TypeName[1][3] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    GirFName.flg[1][3] = true;
                  
                }
                if (SGirLipC.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SGirLipC.Loadflg = true;
                    GirFName.TypeName[1][4] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    GirFName.flg[1][4] = true;
                    
                }
                if (SGirH_Haunch.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SGirH_Haunch.Loadflg = true;
                    GirFName.TypeName[1][5] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    GirFName.flg[1][5] = true;
                }
                if (SRCGirH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SRCGirH.Loadflg = true;
                    GirFName.TypeName[2][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    GirFName.flg[2][0] = true;
                   
                }
            }
        }
        /// <summary>小梁ファミリ名のセット
        /// </summary>
        internal static void SetBeamFamilyName()
        {
            BeamFName.FamilyName[0][0] = RCBeam_F.FamilyName;
            BeamFName.FamilyName[0][1] = RCBeam_F_Haunch.FamilyName;
            BeamFName.FamilyName[0][2] = RCBeam.FamilyName;
            BeamFName.FamilyName[0][3] = RCBeam_Haunch.FamilyName;
            BeamFName.FamilyName[1][0] = SBeamH.FamilyName;
            BeamFName.FamilyName[1][1] = SBeamBH.FamilyName;
            BeamFName.FamilyName[1][2] = SBeamC.FamilyName;
            BeamFName.FamilyName[1][3] = SBeamL.FamilyName;
            BeamFName.FamilyName[1][4] = SBeamLipC.FamilyName;
            BeamFName.FamilyName[1][5] = SBeamH_Haunch.FamilyName;
            BeamFName.FamilyName[2][0] = SRCBeamH.FamilyName;
            for (int i = 0; i < LoadFamily.FamilyNameList.Count(); i++)
            {
                if(RCBeam_F.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    RCBeam_F.Loadflg = true;
                    BeamFName.TypeName[0][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    BeamFName.flg[0][0] = true;
                }
                if (RCBeam_F_Haunch.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    RCBeam_F_Haunch.Loadflg = true;
                    BeamFName.TypeName[0][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    BeamFName.flg[0][1] = true;
                }
                if (RCBeam.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    RCBeam.Loadflg = true;                   
                    BeamFName.TypeName[0][2] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    BeamFName.flg[0][2] = true;
                }
                if (RCBeam_Haunch.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    RCBeam_Haunch.Loadflg = true;
                    BeamFName.TypeName[0][3] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    BeamFName.flg[0][3] = true;
                }
                if (SBeamH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SBeamH.Loadflg = true;
                    BeamFName.TypeName[1][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    BeamFName.flg[1][0] = true;
                }
                if (SBeamBH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SBeamH.Loadflg = true;
                    BeamFName.TypeName[1][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    BeamFName.flg[1][1] = true;
                }
                if (SBeamC.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SBeamC.Loadflg = true;
                    BeamFName.TypeName[1][2] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    BeamFName.flg[1][2] = true;
                }
                if (SBeamL.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SBeamC.Loadflg = true;
                    BeamFName.TypeName[1][3] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    BeamFName.flg[1][3] = true;
                }
                if (SBeamLipC.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SBeamLipC.Loadflg = true;
                    BeamFName.TypeName[1][4] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    BeamFName.flg[1][4] = true;
                }
                if (SBeamH_Haunch.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SBeamH_Haunch.Loadflg = true;
                    BeamFName.TypeName[1][5] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    BeamFName.flg[1][5] = true;
                }
                if (SRCBeamH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SRCBeamH.Loadflg = true;
                    BeamFName.TypeName[2][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    BeamFName.flg[2][0] = true;
                }
            }
        }
        /// <summary>片持梁ファミリ名のセット
        /// </summary>
        internal static void SetCGirFamilyName()
        {
            for (int i = 0; i < LoadFamily.FamilyNameList.Count(); i++)
            {
                CGirFName.FamilyName[0][0] = RCCGir_F.FamilyName;
                CGirFName.FamilyName[0][1] = RCCGir.FamilyName;
                CGirFName.FamilyName[1][0] = SCGirH.FamilyName;
                CGirFName.FamilyName[1][1] = SCGirBH.FamilyName;
                CGirFName.FamilyName[1][2] = SCGirC.FamilyName;
                CGirFName.FamilyName[1][3] = SCGirL.FamilyName;
                CGirFName.FamilyName[1][4] = SCGirLipC.FamilyName;
                CGirFName.FamilyName[2][0] = SRCCGirH.FamilyName;
                if (RCCGir_F.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    RCCGir_F.Loadflg = true;
                    CGirFName.TypeName[0][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    CGirFName.flg[0][0] = true;
                }
                if (RCCGir.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    RCCGir.Loadflg = true;
                    CGirFName.TypeName[0][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    CGirFName.flg[0][1] = true;
                }
                if (SCGirH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SCGirH.Loadflg = true;
                    CGirFName.TypeName[1][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    CGirFName.flg[1][0] = true;
                }
                if (SCGirBH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SCGirBH.Loadflg = true;
                    CGirFName.TypeName[1][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    CGirFName.flg[1][1] = true;
                }
                if (SCGirC.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SCGirC.Loadflg = true;
                    CGirFName.TypeName[1][2] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    CGirFName.flg[1][2] = true;
                }
                if (SCGirL.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SCGirL.Loadflg = true;
                    CGirFName.TypeName[1][3] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    CGirFName.flg[1][3] = true;
                }
                if (SCGirLipC.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SCGirLipC.Loadflg = true;
                    CGirFName.TypeName[1][4] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    CGirFName.flg[1][4] = true;
                }
                if (SRCCGirH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SRCCGirH.Loadflg = true;
                    GirFName.TypeName[2][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    CGirFName.flg[2][0] = true;
                }
            }
        }
        /// <summary>片持小梁ファミリ名のセット
        /// </summary>
        internal static void SetCBeamFamilyName()
        {
            for (int i = 0; i < LoadFamily.FamilyNameList.Count(); i++)
            {
                CBeamFName.FamilyName[0][0] = RCCBeam_F.FamilyName;
                CBeamFName.FamilyName[0][1] = RCCBeam.FamilyName;
                CBeamFName.FamilyName[1][0] = SCBeamH.FamilyName;
                CBeamFName.FamilyName[1][1] = SCBeamBH.FamilyName;
                CBeamFName.FamilyName[1][2] = SCBeamC.FamilyName;
                CBeamFName.FamilyName[1][3] = SCBeamL.FamilyName;
                CBeamFName.FamilyName[1][4] = SCBeamLipC.FamilyName;
                CBeamFName.FamilyName[2][0] = SRCCBeamH.FamilyName;
                if (RCCBeam_F.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    RCCBeam_F.Loadflg = true;
                    CBeamFName.TypeName[0][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    CBeamFName.flg[0][0] = true;
                }
                if (RCCBeam.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    RCCBeam.Loadflg = true;
                    CBeamFName.TypeName[0][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    CBeamFName.flg[0][1] = true;
                }
                if (SCBeamH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SCBeamH.Loadflg = true;
                    CBeamFName.TypeName[1][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    CBeamFName.flg[1][0] = true;
                }
                if (SCBeamBH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SCBeamBH.Loadflg = true;
                    CBeamFName.TypeName[1][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    CBeamFName.flg[1][1] = true;
                }                
                if (SCBeamC.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SCBeamC.Loadflg = true;
                    CBeamFName.TypeName[1][2] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    CBeamFName.flg[1][2] = true;
                }
                if (SCBeamL.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SCBeamL.Loadflg = true;
                    CBeamFName.TypeName[1][3] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    CBeamFName.flg[1][3] = true;
                }
                if (SCBeamLipC.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SCBeamLipC.Loadflg = true;
                    CBeamFName.TypeName[1][4] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    CBeamFName.flg[1][4] = true;
                }
                if (SRCCBeamH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SRCCBeamH.Loadflg = true;
                    BeamFName.TypeName[2][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    CBeamFName.flg[2][0] = true;
                }
            }
        }
        /// <summary>Sブレースファミリ名のセット
        /// </summary>
        internal static void SetBraFamilyName()
        {
            for (int i = 0; i < LoadFamily.FamilyNameList.Count(); i++)
            {
                SBraFName.FamilyName[0][0] = SBraH.FamilyName;
                SBraFName.FamilyName[0][1] = SBraBH.FamilyName;
                SBraFName.FamilyName[0][2] = SBraBox.FamilyName;
                SBraFName.FamilyName[0][3] = SBraBBox.FamilyName;
                SBraFName.FamilyName[0][4] = SBraPipe.FamilyName;
                SBraFName.FamilyName[1][0] = SBraC.FamilyName;
                SBraFName.FamilyName[1][1] = SBraL.FamilyName;
                SBraFName.FamilyName[1][2] = SBraLipC.FamilyName;
                SBraFName.FamilyName[1][3] = SBraFB.FamilyName;
                SBraFName.FamilyName[1][4] = SBraRollBar.FamilyName;
                if (SBraH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SBraH.Loadflg = true;
                    SBraFName.TypeName[0][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    SBraFName.flg[0][0] = true;
                }
                if (SBraBH.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SBraBH.Loadflg = true;
                    SBraFName.TypeName[0][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    SBraFName.flg[0][1] = true;
                }
                if (SBraBox.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SBraBox.Loadflg = true;
                    SBraFName.TypeName[0][2] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    SBraFName.flg[0][2] = true;
                }
                if (SBraBBox.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SBraBBox.Loadflg = true;
                    SBraFName.TypeName[0][3] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    SBraFName.flg[0][3] = true;
                }
                if (SBraPipe.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SBraPipe.Loadflg = true;
                    SBraFName.TypeName[0][4] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    SBraFName.flg[0][4] = true;
                }
                if (SBraC.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SBraC.Loadflg = true;
                    SBraFName.TypeName[1][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    SBraFName.flg[1][0] = true;
                }
                if (SBraL.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SBraL.Loadflg = true;
                    SBraFName.TypeName[1][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    SBraFName.flg[1][1] = true;
                }
                if (SBraLipC.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SBraLipC.Loadflg = true;
                    SBraFName.TypeName[1][2] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    SBraFName.flg[1][2] = true;
                }
                if (SBraFB.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SBraFB.Loadflg = true;
                    SBraFName.TypeName[1][3] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    SBraFName.flg[1][3] = true;
                }
                if (SBraRollBar.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    SBraRollBar.Loadflg = true;
                    SBraFName.TypeName[1][4] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    SBraFName.flg[1][4] = true;
                }
            }
        }
        internal static void SetFoundationFamilyName()
        {
            FoFName.FamilyName[0][0] = FRect.FamilyName;
            FoFName.FamilyName[0][1] = FTRect.FamilyName;
            FoFName.FamilyName[0][2] = FTri.FamilyName;
            FoFName.FamilyName[0][3] = FETriangle.FamilyName;
            FoFName.FamilyName[0][4] = FOct.FamilyName;
            FoFName.FamilyName[1][0] = FConti.FamilyName;
            FoFName.FamilyName[2][0] = CastinPile.FamilyName;
            FoFName.FamilyName[2][1] = PrecastPile.FamilyName;

            FoFName.FamilyName[2][2] = Pile_S.FamilyName;
            FoFName.FamilyName[2][3] = Pile_PHC.FamilyName;
            FoFName.FamilyName[2][4] = Pile_ST.FamilyName;
            FoFName.FamilyName[2][5] = Pile_SC.FamilyName;
            FoFName.FamilyName[2][6] = Pile_PRC.FamilyName;
            FoFName.FamilyName[2][7] = Pile_CPRC.FamilyName;

            for (int i = 0; i < LoadFamily.FamilyNameList.Count; i++)
            {               
                if (FRect.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    FRect.Loadflg = true;
                    FoFName.TypeName[0][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    FoFName.flg[0][0] = true;
                }
                if (FTRect.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    FTRect.Loadflg = true;
                    FoFName.TypeName[0][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    FoFName.flg[0][1] = true;
                }
                if (FTri.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    FTri.Loadflg = true;
                    FoFName.TypeName[0][2] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    FoFName.flg[0][2] = true;
                }
                if (FETriangle.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    FETriangle.Loadflg = true;
                    FoFName.TypeName[0][3] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    FoFName.flg[0][3] = true;

                }
                if (FOct.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    FOct.Loadflg = true;
                    FoFName.TypeName[0][4] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    FoFName.flg[0][4] = true;
                }
                if (FConti.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    FConti.Loadflg = true;
                    FoFName.TypeName[1][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    FoFName.flg[1][0] = true;
                }
                if (CastinPile.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    CastinPile.Loadflg = true;
                    FoFName.TypeName[2][0] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    FoFName.flg[2][0] = true;
                }
                if (PrecastPile.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    PrecastPile.Loadflg = true;
                    FoFName.TypeName[2][1] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    FoFName.flg[2][1] = true;
                }


                if (Pile_S.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    Pile_S.Loadflg = true;
                    FoFName.TypeName[2][2] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    FoFName.flg[2][2] = true;
                }
                if (Pile_PHC.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    Pile_PHC.Loadflg = true;
                    FoFName.TypeName[2][3] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    FoFName.flg[2][3] = true;
                }
                if (Pile_ST.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    Pile_ST.Loadflg = true;
                    FoFName.TypeName[2][4] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    FoFName.flg[2][4] = true;
                }
                if (Pile_SC.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    Pile_SC.Loadflg = true;
                    FoFName.TypeName[2][5] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    FoFName.flg[2][5] = true;
                }
                if (Pile_PRC.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    Pile_PRC.Loadflg = true;
                    FoFName.TypeName[2][6] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    FoFName.flg[2][6] = true;
                }
                if (Pile_CPRC.FamilyName == LoadFamily.FamilyNameList[i].FamilyName)
                {
                    Pile_CPRC.Loadflg = true;
                    FoFName.TypeName[2][7] = LoadFamily.FamilyNameList[i].FamilyTypeName;
                    FoFName.flg[2][7] = true;
                }

            }
        }
        #endregion

        #region パラメータセット
        #region 柱
        //RC角柱
        private static bool SetRCClmRecPara(FamilyStructure.RC_Clm_Re  clm, string dat, string set)
        {
            bool ret = true;
            try
            {   //タイプパラメータ                                
                if (dat == clm.strength_concrete) { clm.strength_concrete = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.DX) { clm.DX = set; }
                if (dat == clm.DY) { clm.DY = set; }
                if (dat == clm.name) { clm.name = set; }
                StringSet(clm.D_reinforcement_main, dat, set);
                StringSet(clm.D_reinforcement_2nd_main, dat, set);
                if (dat == clm.D_reinforcement_axial) { clm.D_reinforcement_axial = set; }
                StringSet(clm.D_reinforcement_band, dat, set);
                if (dat == clm.D_bar_spacing) { clm.D_bar_spacing = set; }
                if (dat == clm.strength_reinforcement_main) { clm.strength_reinforcement_main = set; }
                if (dat == clm.strength_reinforcement_2nd_main) { clm.strength_reinforcement_2nd_main = set; }
                if (dat == clm.strength_reinforcement_axial) { clm.strength_reinforcement_axial = set; }
                StringSet(clm.depth_cover_X, dat, set);
                StringSet(clm.depth_cover_Y, dat, set);
                StringSet(clm.count_main_X_1st, dat, set);
                StringSet(clm.count_2nd_main_X_1st, dat, set);
                StringSet(clm.count_main_X_2nd, dat, set);
                StringSet(clm.count_2nd_main_X_2nd, dat, set);
                StringSet(clm.count_main_Y_1st, dat, set);
                StringSet(clm.count_2nd_main_Y_1st, dat, set);
                StringSet(clm.count_main_Y_2nd, dat, set);
                StringSet(clm.count_2nd_main_Y_2nd, dat, set);
                StringSet(clm.count_band_dir_X, dat, set);
                StringSet(clm.count_band_dir_Y, dat, set);
                StringSet(clm.pitch_band, dat, set);
                StringSet(clm.count_bar_spacing_X, dat, set);
                StringSet(clm.count_bar_spacing_Y, dat, set);
                StringSet(clm.pitch_bar_spacing, dat, set);
                StringSet(clm.count_axial, dat, set);
                if (dat == clm.center_reinforcement_start_X) { clm.center_reinforcement_start_X = set; }
                if (dat == clm.center_reinforcement_end_X) { clm.center_reinforcement_end_X = set; }
                if (dat == clm.center_reinforcement_start_Y) { clm.center_reinforcement_start_Y = set; }
                if (dat == clm.center_reinforcement_end_Y) { clm.center_reinforcement_end_Y = set; }
                StringSet(clm.kind_reinforcement_corner, dat, set);
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.strength_reinforcement_band) { clm.strength_reinforcement_band = set; }
                if (dat == clm.strength_bar_spacing) { clm.strength_bar_spacing = set; }
                if (dat == clm.interval_reinforcement) { clm.interval_reinforcement = set; }
                if (dat == clm.count_main_total) { clm.count_main_total = set; }
                if (dat == clm.count_main_X) { clm.count_main_X = set; }
                if (dat == clm.count_main_Y) { clm.count_main_Y = set; }
                if (dat == clm.count_main_total_X) { clm.count_main_total_X = set; }
                if (dat == clm.pitch_bar_spacing_list) { clm.pitch_bar_spacing_list = set; }
                if (dat == clm.count_axial_list) { clm.count_axial_list = set; }
                if (dat == clm.center_reinforcement_X) { clm.center_reinforcement_X = set; }
                if (dat == clm.center_reinforcement_Y) { clm.center_reinforcement_Y = set; }
                if (dat == clm.concrete_reductionrate) { clm.concrete_reductionrate = set; }
                if (dat == clm.strength_reinforcement_main_Y) { clm.strength_reinforcement_main_Y = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.thickness_ex_start_X) { clm.thickness_ex_start_X = set; }
                if (dat == clm.thickness_ex_end_X) { clm.thickness_ex_end_X = set; }
                if (dat == clm.thickness_ex_start_Y) { clm.thickness_ex_start_Y = set; }
                if (dat == clm.thickness_ex_end_Y) { clm.thickness_ex_end_Y = set; }

            }
            catch
            { ret = false; }
          
            return ret;
        }
        //RC円柱
        private static bool SetRCClmRouPara(FamilyStructure.RC_Clm_Ro clm, string dat, string set)
        {
            bool ret = true;
            try
            {   //タイプパラメータ                                
                if (dat == clm.strength_concrete) { clm.strength_concrete = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.D) { clm.D = set; }
                if (dat == clm.name) { clm.name = set; }
                StringSet(clm.D_reinforcement_main, dat, set);
                StringSet(clm.count_main, dat, set);
                StringSet(clm.D_reinforcement_band, dat, set);
                StringSet(clm.count_band, dat, set);
                StringSet(clm.pitch_band, dat, set);
                if (dat == clm.D_bar_spacing) { clm.D_bar_spacing = set; }
                StringSet(clm.count_bar_spacing_X, dat, set);
                StringSet(clm.count_bar_spacing_Y, dat, set);
                StringSet(clm.pitch_bar_spacing, dat, set);
                if (dat == clm.depth_cover_X) { clm.depth_cover_X = set; }
                if (dat == clm.D_reinforcement_axial) { clm.D_reinforcement_axial = set; }
                StringSet(clm.count_axial, dat, set);
                if (dat == clm.strength_reinforcement_main) { clm.strength_reinforcement_main = set; }
                if (dat == clm.strength_reinforcement_axial) { clm.strength_reinforcement_axial = set; }
                if (dat == clm.center_reinforcement_start_X) { clm.center_reinforcement_start_X = set; }
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.strength_reinforcement_band) { clm.strength_reinforcement_band = set; }
                if (dat == clm.strength_bar_spacing) { clm.strength_bar_spacing = set; }
                if (dat == clm.pitch_bar_spacing_list) { clm.pitch_bar_spacing_list = set; }
                if (dat == clm.count_axial_list) { clm.count_axial_list = set; }
                if (dat == clm.center_reinforcement) { clm.center_reinforcement = set; }
                if (dat == clm.concrete_reductionrate) { clm.concrete_reductionrate = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.thickness_ex_start_X) { clm.thickness_ex_start_X = set; }
            }
            catch
            { ret = false; }

            return ret;
        }
        //S柱H形
        private static bool SetSClmHPara(FamilyStructure.S_Clm_H clm, string dat, string set)
        {
            bool ret = true;
            try
            {
                //タイプパラメータ                                
                if (dat == clm.strength_web) { clm.strength_web = set; }
                if (dat == clm.strength_main) { clm.strength_main = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.B) { clm.B = set; }
                if (dat == clm.A) { clm.A = set; }
                if (dat == clm.t1) { clm.t1 = set; }
                if (dat == clm.t2) { clm.t2 = set; }
                if (dat == clm.r) { clm.r = set; }
                if (dat == clm.name) { clm.name = set; }
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.type_name) { clm.type_name = set; }
                if (dat == clm.type) { clm.type = set; }
                if (dat == clm.base_type) { clm.base_type = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.condition_bottom) { clm.condition_bottom = set; }
                if (dat == clm.condition_top) { clm.condition_top = set; }
                if (dat == clm.joint_top) { clm.joint_top = set; }
                if (dat == clm.joint_bottom) { clm.joint_bottom = set; }
                if (dat == clm.kind_joint_top) { clm.kind_joint_top = set; }
                if (dat == clm.kind_joint_bottom) { clm.kind_joint_bottom = set; }
            }
            catch
            { ret = false; }

            return ret;
        }
        //S柱組立H形
        private static bool SetSClmBHPara(FamilyStructure.S_Clm_BH clm, string dat, string set)
        {
            bool ret = true;
            try
            {
                //タイプパラメータ                                
                if (dat == clm.strength_web) { clm.strength_web = set; }
                if (dat == clm.strength_main) { clm.strength_main = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.B) { clm.B = set; }
                if (dat == clm.A) { clm.A = set; }
                if (dat == clm.t1) { clm.t1 = set; }
                if (dat == clm.t2) { clm.t2 = set; }
                if (dat == clm.name) { clm.name = set; }
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.type_name) { clm.type_name = set; }
                if (dat == clm.base_type) { clm.base_type = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.condition_bottom) { clm.condition_bottom = set; }
                if (dat == clm.condition_top) { clm.condition_top = set; }
                if (dat == clm.joint_top) { clm.joint_top = set; }
                if (dat == clm.joint_bottom) { clm.joint_bottom = set; }
                if (dat == clm.kind_joint_top) { clm.kind_joint_top = set; }
                if (dat == clm.kind_joint_bottom) { clm.kind_joint_bottom = set; }
            }
            catch
            { ret = false; }

            return ret;
        }
        //S柱角形
        private static bool SetSClmBoxPara(FamilyStructure.S_Clm_Box clm, string dat, string set)
        {
            bool ret = true;
            try
            {
                //タイプパラメータ                                
                if (dat == clm.strength_main) { clm.strength_main = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.B) { clm.B = set; }
                if (dat == clm.A) { clm.A = set; }
                if (dat == clm.t1) { clm.t1 = set; }
                if (dat == clm.r) { clm.r = set; }
                if (dat == clm.name) { clm.name = set; }
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.type_name) { clm.type_name = set; }
                if (dat == clm.type) { clm.type = set; }
                if (dat == clm.base_type) { clm.base_type = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.condition_bottom) { clm.condition_bottom = set; }
                if (dat == clm.condition_top) { clm.condition_top = set; }
                if (dat == clm.joint_top) { clm.joint_top = set; }
                if (dat == clm.joint_bottom) { clm.joint_bottom = set; }
                if (dat == clm.kind_joint_top) { clm.kind_joint_top = set; }
                if (dat == clm.kind_joint_bottom) { clm.kind_joint_bottom = set; }
            }
            catch
            { ret = false; }

            return ret;
        }
        //S柱組立角形
        private static bool SetSClmBBoxPara(FamilyStructure.S_Clm_BBox clm, string dat, string set)
        {
            bool ret = true;
            try
            {
                //タイプパラメータ
                if (dat == clm.strength_main) { clm.strength_main = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.B) { clm.B = set; }
                if (dat == clm.A) { clm.A = set; }
                if (dat == clm.size_imput) { clm.size_imput = set; } //マッピングテーブルなし
                if (dat == clm.t1) { clm.t1 = set; }
                if (dat == clm.t2) { clm.t2 = set; }
                if (dat == clm.r) { clm.r = set; }
                if (dat == clm.name) { clm.name = set; }
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.type_name) { clm.type_name = set; }
                if (dat == clm.base_type) { clm.base_type = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.condition_bottom) { clm.condition_bottom = set; }
                if (dat == clm.condition_top) { clm.condition_top = set; }
                if (dat == clm.joint_top) { clm.joint_top = set; }
                if (dat == clm.joint_bottom) { clm.joint_bottom = set; }
                if (dat == clm.kind_joint_top) { clm.kind_joint_top = set; }
                if (dat == clm.kind_joint_bottom) { clm.kind_joint_bottom = set; }
            }
            catch
            { ret = false; }

            return ret;
        }
        //S柱円形
        private static bool SetSClmPipePara(FamilyStructure.S_Clm_Pipe clm, string dat, string set)
        {
            bool ret = true;
            try
            {
                //タイプパラメータ                                
                if (dat == clm.strength_main) { clm.strength_main = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.D) { clm.D = set; }
                if (dat == clm.t) { clm.t = set; }
                if (dat == clm.name) { clm.name = set; }
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.type_name) { clm.type_name = set; }
                if (dat == clm.base_type) { clm.base_type = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.condition_bottom) { clm.condition_bottom = set; }
                if (dat == clm.condition_top) { clm.condition_top = set; }
                if (dat == clm.joint_top) { clm.joint_top = set; }
                if (dat == clm.joint_bottom) { clm.joint_bottom = set; }
                if (dat == clm.kind_joint_top) { clm.kind_joint_top = set; }
                if (dat == clm.kind_joint_bottom) { clm.kind_joint_bottom = set; }
            }
            catch
            { ret = false; }

            return ret;
        }
        //S柱T形
        private static bool SetSClmTPara(FamilyStructure.S_Clm_T clm, string dat, string set)
        {
            bool ret = true;
            try
            {
                //タイプパラメータ                                
                if (dat == clm.strength_web) { clm.strength_web = set; }
                if (dat == clm.strength_main) { clm.strength_main = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.B) { clm.B = set; }
                if (dat == clm.A) { clm.A = set; }
                if (dat == clm.t1) { clm.t1 = set; }
                if (dat == clm.t2) { clm.t2 = set; }
                if (dat == clm.r) { clm.r = set; }
                if (dat == clm.name) { clm.name = set; }
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.type_name) { clm.type_name = set; }
                if (dat == clm.type) { clm.type = set; }
                if (dat == clm.base_type) { clm.base_type = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.condition_bottom) { clm.condition_bottom = set; }
                if (dat == clm.condition_top) { clm.condition_top = set; }
                if (dat == clm.joint_top) { clm.joint_top = set; }
                if (dat == clm.joint_bottom) { clm.joint_bottom = set; }
                if (dat == clm.kind_joint_top) { clm.kind_joint_top = set; }
                if (dat == clm.kind_joint_bottom) { clm.kind_joint_bottom = set; }
            }
            catch
            { ret = false; }

            return ret;
        }
        //S柱溝形
        private static bool SetSClmCPara(FamilyStructure.S_Clm_C clm, string dat, string set)
        {
            bool ret = true;
            try
            {
                //タイプパラメータ
                if (dat == clm.strength_main) { clm.strength_main = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.B) { clm.B = set; }
                if (dat == clm.A) { clm.A = set; }
                if (dat == clm.t1) { clm.t1 = set; }
                if (dat == clm.t2) { clm.t2 = set; }
                if (dat == clm.r1) { clm.r1 = set; }
                if (dat == clm.r2) { clm.r2 = set; }
                if (dat == clm.name) { clm.name = set; }
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.side) { clm.side = set; }
                if (dat == clm.type_name) { clm.type_name = set; }
                if (dat == clm.type) { clm.type = set; }
                if (dat == clm.base_type) { clm.base_type = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.condition_bottom) { clm.condition_bottom = set; }
                if (dat == clm.condition_top) { clm.condition_top = set; }
                if (dat == clm.joint_top) { clm.joint_top = set; }
                if (dat == clm.joint_bottom) { clm.joint_bottom = set; }
                if (dat == clm.kind_joint_top) { clm.kind_joint_top = set; }
                if (dat == clm.kind_joint_bottom) { clm.kind_joint_bottom = set; }
            }
            catch
            { ret = false; }

            return ret;
        }
        //S柱山形
        private static bool SetSClmLPara(FamilyStructure.S_Clm_L clm, string dat, string set)
        {
            bool ret = true;
            try
            {
                //タイプパラメータ                                
                if (dat == clm.strength_main) { clm.strength_main = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.B) { clm.B = set; }
                if (dat == clm.A) { clm.A = set; }
                if (dat == clm.t1) { clm.t1 = set; }
                if (dat == clm.t2) { clm.t2 = set; }
                if (dat == clm.r1) { clm.r1 = set; }
                if (dat == clm.r2) { clm.r2 = set; }
                if (dat == clm.name) { clm.name = set; }
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.side) { clm.side = set; }
                if (dat == clm.type_name) { clm.type_name = set; }
                if (dat == clm.type) { clm.type = set; }
                if (dat == clm.base_type) { clm.base_type = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.condition_bottom) { clm.condition_bottom = set; }
                if (dat == clm.condition_top) { clm.condition_top = set; }
                if (dat == clm.joint_top) { clm.joint_top = set; }
                if (dat == clm.joint_bottom) { clm.joint_bottom = set; }
                if (dat == clm.kind_joint_top) { clm.kind_joint_top = set; }
                if (dat == clm.kind_joint_bottom) { clm.kind_joint_bottom = set; }
            }
            catch
            { ret = false; }

            return ret;
        }
        //SRC柱H形
        private static bool SetSRCClmHPara(FamilyStructure.SRC_Clm_H clm, string dat, string set)
        {
            bool ret = true;
            try
            {
                //タイプパラメータ                                
                if (dat == clm.strength_concrete) { clm.strength_concrete = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.DX) { clm.DX = set; }
                if (dat == clm.DY) { clm.DY = set; }
                if (dat == clm.name) { clm.name = set; }
                StringSet(clm.D_reinforcement_main, dat, set);
                StringSet(clm.D_reinforcement_2nd_main, dat, set);
                StringSet(clm.D_reinforcement_band, dat, set);
                if (dat == clm.D_bar_spacing) { clm.D_bar_spacing = set; }
                if (dat == clm.strength_reinforcement_main) { clm.strength_reinforcement_main = set; }
                if (dat == clm.strength_reinforcement_2nd_main) { clm.strength_reinforcement_2nd_main = set; }
                StringSet(clm.depth_cover_X, dat, set);
                StringSet(clm.depth_cover_Y, dat, set);
                StringSet(clm.count_main_X_1st, dat, set);
                StringSet(clm.count_2nd_main_X_1st, dat, set);
                StringSet(clm.count_main_X_2nd, dat, set);
                StringSet(clm.count_2nd_main_X_2nd, dat, set);
                StringSet(clm.count_main_Y_1st, dat, set);
                StringSet(clm.count_2nd_main_Y_1st, dat, set);
                StringSet(clm.count_main_Y_2nd, dat, set);
                StringSet(clm.count_2nd_main_Y_2nd, dat, set);
                StringSet(clm.count_band_dir_X, dat, set);
                StringSet(clm.count_band_dir_Y, dat, set);
                StringSet(clm.pitch_band, dat, set);
                StringSet(clm.count_bar_spacing_X, dat, set);
                StringSet(clm.count_bar_spacing_Y, dat, set);
                StringSet(clm.pitch_bar_spacing, dat, set);
                StringSet(clm.kind_reinforcement_corner, dat, set);
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.strength_reinforcement_band) { clm.strength_reinforcement_band = set; }
                if (dat == clm.strength_bar_spacing) { clm.strength_bar_spacing = set; }
                if (dat == clm.interval_reinforcement) { clm.interval_reinforcement = set; }
                if (dat == clm.count_main_total) { clm.count_main_total = set; }
                if (dat == clm.count_main_X) { clm.count_main_X = set; }
                if (dat == clm.count_main_Y) { clm.count_main_Y = set; }
                if (dat == clm.count_main_total_X) { clm.count_main_total_X = set; }
                if (dat == clm.pitch_bar_spacing_list) { clm.pitch_bar_spacing_list = set; }
                if (dat == clm.strength_web) { clm.strength_web = set; }
                if (dat == clm.strength_main) { clm.strength_main = set; }
                if (dat == clm.type) { clm.type = set; }
                if (dat == clm.typename) { clm.typename = set; }
                if (dat == clm.H) { clm.H = set; }
                if (dat == clm.B) { clm.B = set; }
                if (dat == clm.t1) { clm.t1 = set; }
                if (dat == clm.t2) { clm.t2 = set; }
                if (dat == clm.r) { clm.r = set; }
                if (dat == clm.direction_type) { clm.direction_type = set; }
                if (dat == clm.offset_X) { clm.offset_X = set; }
                if (dat == clm.offset_Y) { clm.offset_Y = set; }
                if (dat == clm.angle) { clm.angle = set; } //マッピングテーブルなし
                if (dat == clm.base_type) { clm.base_type = set; }
                if (dat == clm.concrete_reductionrate) { clm.concrete_reductionrate = set; }
                if (dat == clm.strength_reinforcement_main_Y) { clm.strength_reinforcement_main_Y = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.thickness_ex_start_X) { clm.thickness_ex_start_X = set; }
                if (dat == clm.thickness_ex_end_X) { clm.thickness_ex_end_X = set; }
                if (dat == clm.thickness_ex_start_Y) { clm.thickness_ex_start_Y = set; }
                if (dat == clm.thickness_ex_end_Y) { clm.thickness_ex_end_Y = set; }
                if (dat == clm.condition_bottom) { clm.condition_bottom = set; }
                if (dat == clm.condition_top) { clm.condition_top = set; }
                if (dat == clm.joint_top) { clm.joint_top = set; }
                if (dat == clm.joint_bottom) { clm.joint_bottom = set; }
                if (dat == clm.kind_joint_top) { clm.kind_joint_top = set; }
                if (dat == clm.kind_joint_bottom) { clm.kind_joint_bottom = set; }
            }
            catch
            { ret = false; }

            return ret;
        }
        //SRC柱＋形
        private static bool SetSRCClmCrossPara(FamilyStructure.SRC_Clm_Cross clm, string dat, string set)
        {
            bool ret = true;
            try
            {
                //タイプパラメータ                                
                if (dat == clm.strength_concrete) { clm.strength_concrete = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.DX) { clm.DX = set; }
                if (dat == clm.DY) { clm.DY = set; }
                if (dat == clm.name) { clm.name = set; }
                StringSet(clm.D_reinforcement_main, dat, set);
                StringSet(clm.D_reinforcement_2nd_main, dat, set);
                StringSet(clm.D_reinforcement_band, dat, set);
                if (dat == clm.D_bar_spacing) { clm.D_bar_spacing = set; }
                if (dat == clm.strength_reinforcement_main) { clm.strength_reinforcement_main = set; }
                if (dat == clm.strength_reinforcement_2nd_main) { clm.strength_reinforcement_2nd_main = set; }
                StringSet(clm.depth_cover_X, dat, set);
                StringSet(clm.depth_cover_Y, dat, set);
                StringSet(clm.count_main_X_1st, dat, set);
                StringSet(clm.count_2nd_main_X_1st, dat, set);
                StringSet(clm.count_main_X_2nd, dat, set);
                StringSet(clm.count_2nd_main_X_2nd, dat, set);
                StringSet(clm.count_main_Y_1st, dat, set);
                StringSet(clm.count_2nd_main_Y_1st, dat, set);
                StringSet(clm.count_main_Y_2nd, dat, set);
                StringSet(clm.count_2nd_main_Y_2nd, dat, set);
                StringSet(clm.count_band_dir_X, dat, set);
                StringSet(clm.count_band_dir_Y, dat, set);
                StringSet(clm.pitch_band, dat, set);
                StringSet(clm.count_bar_spacing_X, dat, set);
                StringSet(clm.count_bar_spacing_Y, dat, set);
                StringSet(clm.pitch_bar_spacing, dat, set);
                StringSet(clm.kind_reinforcement_corner, dat, set);
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.strength_reinforcement_band) { clm.strength_reinforcement_band = set; }
                if (dat == clm.strength_bar_spacing) { clm.strength_bar_spacing = set; }
                if (dat == clm.interval_reinforcement) { clm.interval_reinforcement = set; }
                if (dat == clm.count_main_total) { clm.count_main_total = set; }
                if (dat == clm.count_main_X) { clm.count_main_X = set; }
                if (dat == clm.count_main_Y) { clm.count_main_Y = set; }
                if (dat == clm.count_main_total_X) { clm.count_main_total_X = set; }
                if (dat == clm.pitch_bar_spacing_list) { clm.pitch_bar_spacing_list = set; }
                if (dat == clm.strength_main_X) { clm.strength_main_X = set; }
                if (dat == clm.strength_web_X) { clm.strength_web_X = set; }
                if (dat == clm.strength_main_Y) { clm.strength_main_Y = set; }
                if (dat == clm.strength_web_Y) { clm.strength_web_Y = set; }
                if (dat == clm.XH) { clm.XH = set; }
                if (dat == clm.XB) { clm.XB = set; }
                if (dat == clm.Xt1) { clm.Xt1 = set; }
                if (dat == clm.Xt2) { clm.Xt2 = set; }
                if (dat == clm.Xr) { clm.Xr = set; }
                if (dat == clm.YH) { clm.YH = set; }
                if (dat == clm.YB) { clm.YB = set; }
                if (dat == clm.Yt1) { clm.Yt1 = set; }
                if (dat == clm.Yt2) { clm.Yt2 = set; }
                if (dat == clm.Yr) { clm.Yr = set; }
                if (dat == clm.offset_XX) { clm.offset_XX = set; }
                if (dat == clm.offset_XY) { clm.offset_XY = set; }
                if (dat == clm.offset_YX) { clm.offset_YX = set; }
                if (dat == clm.offset_YY) { clm.offset_YY = set; }
                if (dat == clm.concrete_reductionrate) { clm.concrete_reductionrate = set; }
                if (dat == clm.strength_reinforcement_main_Y) { clm.strength_reinforcement_main_Y = set; }
                if (dat == clm.typename_X) { clm.typename_X = set; }
                if (dat == clm.typename_Y) { clm.typename_Y = set; }
                if (dat == clm.type_X) { clm.type_X = set; }
                if (dat == clm.type_Y) { clm.type_Y = set; }
                if (dat == clm.base_type) { clm.base_type = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.thickness_ex_start_X) { clm.thickness_ex_start_X = set; }
                if (dat == clm.thickness_ex_end_X) { clm.thickness_ex_end_X = set; }
                if (dat == clm.thickness_ex_start_Y) { clm.thickness_ex_start_Y = set; }
                if (dat == clm.thickness_ex_end_Y) { clm.thickness_ex_end_Y = set; }
                if (dat == clm.condition_bottom) { clm.condition_bottom = set; }
                if (dat == clm.condition_top) { clm.condition_top = set; }
                if (dat == clm.joint_top) { clm.joint_top = set; }
                if (dat == clm.joint_bottom) { clm.joint_bottom = set; }
                if (dat == clm.kind_joint_top) { clm.kind_joint_top = set; }
                if (dat == clm.kind_joint_bottom) { clm.kind_joint_bottom = set; }
            }
            catch
            { ret = false; }

            return ret;
        }
        //SRC柱T形
        private static bool SetSRCClmTPara(FamilyStructure.SRC_Clm_T clm, string dat, string set)
        {
            bool ret = true;
            try
            {
                //タイプパラメータ                                
                if (dat == clm.strength_concrete) { clm.strength_concrete = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.DX) { clm.DX = set; }
                if (dat == clm.DY) { clm.DY = set; }
                if (dat == clm.name) { clm.name = set; }
                StringSet(clm.D_reinforcement_main, dat, set);
                StringSet(clm.D_reinforcement_2nd_main, dat, set);
                StringSet(clm.D_reinforcement_band, dat, set);
                if (dat == clm.D_bar_spacing) { clm.D_bar_spacing = set; }
                if (dat == clm.strength_reinforcement_main) { clm.strength_reinforcement_main = set; }
                if (dat == clm.strength_reinforcement_2nd_main) { clm.strength_reinforcement_2nd_main = set; }
                StringSet(clm.depth_cover_X, dat, set);
                StringSet(clm.depth_cover_Y, dat, set);
                StringSet(clm.count_main_X_1st, dat, set);
                StringSet(clm.count_2nd_main_X_1st, dat, set);
                StringSet(clm.count_main_X_2nd, dat, set);
                StringSet(clm.count_2nd_main_X_2nd, dat, set);
                StringSet(clm.count_main_Y_1st, dat, set);
                StringSet(clm.count_2nd_main_Y_1st, dat, set);
                StringSet(clm.count_main_Y_2nd, dat, set);
                StringSet(clm.count_2nd_main_Y_2nd, dat, set);
                StringSet(clm.count_band_dir_X, dat, set);
                StringSet(clm.count_band_dir_Y, dat, set);
                StringSet(clm.pitch_band, dat, set);
                StringSet(clm.count_bar_spacing_X, dat, set);
                StringSet(clm.count_bar_spacing_Y, dat, set);
                StringSet(clm.pitch_bar_spacing, dat, set);
                StringSet(clm.kind_reinforcement_corner, dat, set);
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.strength_reinforcement_band) { clm.strength_reinforcement_band = set; }
                if (dat == clm.strength_bar_spacing) { clm.strength_bar_spacing = set; }
                if (dat == clm.interval_reinforcement) { clm.interval_reinforcement = set; }
                if (dat == clm.count_main_total) { clm.count_main_total = set; }
                if (dat == clm.count_main_X) { clm.count_main_X = set; }
                if (dat == clm.count_main_Y) { clm.count_main_Y = set; }
                if (dat == clm.count_main_total_X) { clm.count_main_total_X = set; }
                if (dat == clm.pitch_bar_spacing_list) { clm.pitch_bar_spacing_list = set; }
                if (dat == clm.strength_main_T) { clm.strength_main_T = set; }
                if (dat == clm.strength_web_T) { clm.strength_web_T = set; }
                if (dat == clm.strength_main_H) { clm.strength_main_H = set; }
                if (dat == clm.strength_web_H) { clm.strength_web_H = set; }
                if (dat == clm.direction_type) { clm.direction_type = set; }
                if (dat == clm.H) { clm.H = set; }
                if (dat == clm.B) { clm.B = set; }
                if (dat == clm.t1) { clm.t1 = set; }
                if (dat == clm.t2) { clm.t2 = set; }
                if (dat == clm.r) { clm.r = set; }
                if (dat == clm.CT_A) { clm.CT_A = set; }
                if (dat == clm.CT_B) { clm.CT_B = set; }
                if (dat == clm.CT_t1) { clm.CT_t1 = set; }
                if (dat == clm.CT_t2) { clm.CT_t2 = set; }
                if (dat == clm.CT_r) { clm.CT_r = set; }
                if (dat == clm.offset_HX) { clm.offset_HX = set; }
                if (dat == clm.offset_HY) { clm.offset_HY = set; }
                if (dat == clm.offset_T) { clm.offset_T = set; }
                if (dat == clm.angle) { clm.angle = set; } //マッピングテーブルなし
                if (dat == clm.type_H) { clm.type_H = set; }
                if (dat == clm.type_T) { clm.type_T = set; }
                if (dat == clm.typename_H) { clm.typename_H = set; }
                if (dat == clm.typename_T) { clm.typename_T = set; }
                if (dat == clm.base_type) { clm.base_type = set; }
                if (dat == clm.concrete_reductionrate) { clm.concrete_reductionrate = set; }
                if (dat == clm.strength_reinforcement_main_Y) { clm.strength_reinforcement_main_Y = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.thickness_ex_start_X) { clm.thickness_ex_start_X = set; }
                if (dat == clm.thickness_ex_end_X) { clm.thickness_ex_end_X = set; }
                if (dat == clm.thickness_ex_start_Y) { clm.thickness_ex_start_Y = set; }
                if (dat == clm.thickness_ex_end_Y) { clm.thickness_ex_end_Y = set; }
                if (dat == clm.condition_bottom) { clm.condition_bottom = set; }
                if (dat == clm.condition_top) { clm.condition_top = set; }
                if (dat == clm.joint_top) { clm.joint_top = set; }
                if (dat == clm.joint_bottom) { clm.joint_bottom = set; }
                if (dat == clm.kind_joint_top) { clm.kind_joint_top = set; }
                if (dat == clm.kind_joint_bottom) { clm.kind_joint_bottom = set; }
            }
            catch
            { ret = false; }

            return ret;
        }
        //SRC柱H形(円形)
        private static bool SetSRCClmH_RouPara(FamilyStructure.SRC_Clm_H_Rou clm, string dat, string set)
        {
            bool ret = true;
            try
            {
                //タイプパラメータ
                if (dat == clm.strength_concrete) { clm.strength_concrete = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.D) { clm.D = set; }
                if (dat == clm.name) { clm.name = set; }
                StringSet(clm.D_reinforcement_main, dat, set);
                StringSet(clm.count_main, dat, set);
                StringSet(clm.D_reinforcement_band, dat, set);
                StringSet(clm.count_band, dat, set);
                StringSet(clm.pitch_band, dat, set);
                if (dat == clm.D_bar_spacing) { clm.D_bar_spacing = set; }
                StringSet(clm.count_bar_spacing_X, dat, set);
                StringSet(clm.count_bar_spacing_Y, dat, set);
                StringSet(clm.pitch_bar_spacing, dat, set);
                if (dat == clm.depth_cover_X) { clm.depth_cover_X = set; }
                if (dat == clm.strength_reinforcement_main) { clm.strength_reinforcement_main = set; }
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.strength_reinforcement_band) { clm.strength_reinforcement_band = set; }
                if (dat == clm.strength_bar_spacing) { clm.strength_bar_spacing = set; }
                if (dat == clm.pitch_bar_spacing_list) { clm.pitch_bar_spacing_list = set; }
                if (dat == clm.strength_web) { clm.strength_web = set; }
                if (dat == clm.strength_main) { clm.strength_main = set; }
                if (dat == clm.direction_type) { clm.direction_type = set; }
                if (dat == clm.type) { clm.type = set; }
                if (dat == clm.typename) { clm.typename = set; }
                if (dat == clm.H) { clm.H = set; }
                if (dat == clm.B) { clm.B = set; }
                if (dat == clm.t1) { clm.t1 = set; }
                if (dat == clm.t2) { clm.t2 = set; }
                if (dat == clm.r) { clm.r = set; }
                if (dat == clm.offset_X) { clm.offset_X = set; }
                if (dat == clm.offset_Y) { clm.offset_Y = set; }
                if (dat == clm.angle) { clm.angle = set; } //マッピングテーブルなし
                if (dat == clm.base_type) { clm.base_type = set; }
                if (dat == clm.concrete_reductionrate) { clm.concrete_reductionrate = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.thickness_ex_start_X) { clm.thickness_ex_start_X = set; }
                if (dat == clm.condition_bottom) { clm.condition_bottom = set; }
                if (dat == clm.condition_top) { clm.condition_top = set; }
                if (dat == clm.joint_top) { clm.joint_top = set; }
                if (dat == clm.joint_bottom) { clm.joint_bottom = set; }
                if (dat == clm.kind_joint_top) { clm.kind_joint_top = set; }
                if (dat == clm.kind_joint_bottom) { clm.kind_joint_bottom = set; }
            }
            catch 
            { ret = false; }

            return ret;
        }
        //SRC柱＋形(円形)
        private static bool SetSRCClmCross_RouPara(FamilyStructure.SRC_Clm_Cross_Rou clm, string dat, string set)
        {
            bool ret = true;
            try
            {
                //タイプパラメータ
                if (dat == clm.strength_concrete) { clm.strength_concrete = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.D) { clm.D = set; }
                if (dat == clm.name) { clm.name = set; }
                StringSet(clm.D_reinforcement_main, dat, set);
                StringSet(clm.count_main, dat, set);
                StringSet(clm.D_reinforcement_band, dat, set);
                StringSet(clm.count_band, dat, set);
                StringSet(clm.pitch_band, dat, set);
                if (dat == clm.D_bar_spacing) { clm.D_bar_spacing = set; }
                StringSet(clm.count_bar_spacing_X, dat, set);
                StringSet(clm.count_bar_spacing_Y, dat, set);
                StringSet(clm.pitch_bar_spacing, dat, set);
                if (dat == clm.depth_cover_X) { clm.depth_cover_X = set; }
                if (dat == clm.strength_reinforcement_main) { clm.strength_reinforcement_main = set; }
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.strength_reinforcement_band) { clm.strength_reinforcement_band = set; }
                if (dat == clm.strength_bar_spacing) { clm.strength_bar_spacing = set; }
                if (dat == clm.pitch_bar_spacing_list) { clm.pitch_bar_spacing_list = set; }
                if (dat == clm.strength_main_X) { clm.strength_main_X = set; }
                if (dat == clm.strength_web_X) { clm.strength_web_X = set; }
                if (dat == clm.strength_main_Y) { clm.strength_main_Y = set; }
                if (dat == clm.strength_web_Y) { clm.strength_web_Y = set; }
                if (dat == clm.type_X) { clm.type_X = set; }
                if (dat == clm.type_Y) { clm.type_Y = set; }
                if (dat == clm.typename_X) { clm.typename_X = set; }
                if (dat == clm.typename_Y) { clm.typename_Y = set; }
                if (dat == clm.XH) { clm.XH = set; }
                if (dat == clm.XB) { clm.XB = set; }
                if (dat == clm.Xt1) { clm.Xt1 = set; }
                if (dat == clm.Xt2) { clm.Xt2 = set; }
                if (dat == clm.Xr) { clm.Xr = set; }
                if (dat == clm.YH) { clm.YH = set; }
                if (dat == clm.YB) { clm.YB = set; }
                if (dat == clm.Yt1) { clm.Yt1 = set; }
                if (dat == clm.Yt2) { clm.Yt2 = set; }
                if (dat == clm.Yr) { clm.Yr = set; }
                if (dat == clm.offset_XX) { clm.offset_XX = set; }
                if (dat == clm.offset_XY) { clm.offset_XY = set; }
                if (dat == clm.offset_YX) { clm.offset_YX = set; }
                if (dat == clm.offset_YY) { clm.offset_YY = set; }
                if (dat == clm.base_type) { clm.base_type = set; }
                if (dat == clm.concrete_reductionrate) { clm.concrete_reductionrate = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.thickness_ex_start_X) { clm.thickness_ex_start_X = set; }
                if (dat == clm.condition_bottom) { clm.condition_bottom = set; }
                if (dat == clm.condition_top) { clm.condition_top = set; }
                if (dat == clm.joint_top) { clm.joint_top = set; }
                if (dat == clm.joint_bottom) { clm.joint_bottom = set; }
                if (dat == clm.kind_joint_top) { clm.kind_joint_top = set; }
                if (dat == clm.kind_joint_bottom) { clm.kind_joint_bottom = set; }
            }
            catch
            { ret = false; }

            return ret;
        }
        //SRC柱T形(円形)
        private static bool SetSRCClmT_RouPara(FamilyStructure.SRC_Clm_T_Rou clm, string dat, string set)
        {
            bool ret = true;
            try
            {
                //タイプパラメータ
                if (dat == clm.strength_concrete) { clm.strength_concrete = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.D) { clm.D = set; }
                if (dat == clm.name) { clm.name = set; }
                StringSet(clm.D_reinforcement_main, dat, set);
                StringSet(clm.count_main, dat, set);
                StringSet(clm.D_reinforcement_band, dat, set);
                StringSet(clm.count_band, dat, set);
                StringSet(clm.pitch_band, dat, set);
                if (dat == clm.D_bar_spacing) { clm.D_bar_spacing = set; }
                StringSet(clm.count_bar_spacing_X, dat, set);
                StringSet(clm.count_bar_spacing_Y, dat, set);
                StringSet(clm.pitch_bar_spacing, dat, set);
                if (dat == clm.depth_cover_X) { clm.depth_cover_X = set; }
                if (dat == clm.strength_reinforcement_main) { clm.strength_reinforcement_main = set; }
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.strength_reinforcement_band) { clm.strength_reinforcement_band = set; }
                if (dat == clm.strength_bar_spacing) { clm.strength_bar_spacing = set; }
                if (dat == clm.pitch_bar_spacing_list) { clm.pitch_bar_spacing_list = set; }
                if (dat == clm.strength_main_T) { clm.strength_main_T = set; }
                if (dat == clm.strength_web_T) { clm.strength_web_T = set; }
                if (dat == clm.strength_main_H) { clm.strength_main_H = set; }
                if (dat == clm.strength_web_H) { clm.strength_web_H = set; }
                if (dat == clm.direction_type) { clm.direction_type = set; }
                if (dat == clm.type_H) { clm.type_H = set; }
                if (dat == clm.type_T) { clm.type_T = set; }
                if (dat == clm.typename_H) { clm.typename_H = set; }
                if (dat == clm.typename_T) { clm.typename_T = set; }
                if (dat == clm.H) { clm.H = set; }
                if (dat == clm.B) { clm.B = set; }
                if (dat == clm.t1) { clm.t1 = set; }
                if (dat == clm.t2) { clm.t2 = set; }
                if (dat == clm.r) { clm.r = set; }
                if (dat == clm.CT_A) { clm.CT_A = set; }
                if (dat == clm.CT_B) { clm.CT_B = set; }
                if (dat == clm.CT_t1) { clm.CT_t1 = set; }
                if (dat == clm.CT_t2) { clm.CT_t2 = set; }
                if (dat == clm.CT_r) { clm.CT_r = set; }
                if (dat == clm.offset_HX) { clm.offset_HX = set; }
                if (dat == clm.offset_HY) { clm.offset_HY = set; }
                if (dat == clm.offset_T) { clm.offset_T = set; }
                if (dat == clm.angle) { clm.angle = set; } //マッピングテーブルなし
                if (dat == clm.base_type) { clm.base_type = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.thickness_ex_start_X) { clm.thickness_ex_start_X = set; }
                if (dat == clm.condition_bottom) { clm.condition_bottom = set; }
                if (dat == clm.condition_top) { clm.condition_top = set; }
                if (dat == clm.joint_top) { clm.joint_top = set; }
                if (dat == clm.joint_bottom) { clm.joint_bottom = set; }
                if (dat == clm.kind_joint_top) { clm.kind_joint_top = set; }
                if (dat == clm.kind_joint_bottom) { clm.kind_joint_bottom = set; }
                if (dat == clm.concrete_reductionrate) { clm.concrete_reductionrate = set; }
            }
            catch
            { ret = false; }

            return ret;
        }
        //CFT柱角形
        private static bool SetCFTClmBoxPara(FamilyStructure.CFT_Clm_Box clm, string dat, string set)
        {
            bool ret = true;
            try
            {
                //タイプパラメータ                                
                if (dat == clm.strength_main) { clm.strength_main = set; }
                if (dat == clm.strength_concrete) { clm.strength_concrete = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.direction_type) { clm.direction_type = set; }
                if (dat == clm.type) { clm.type = set; }
                if (dat == clm.typename) { clm.typename = set; }
                if (dat == clm.B) { clm.B = set; }
                if (dat == clm.A) { clm.A = set; }
                if (dat == clm.t) { clm.t = set; }
                if (dat == clm.r1) { clm.r1 = set; }
                if (dat == clm.name) { clm.name = set; }
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.base_type) { clm.base_type = set; }
                if (dat == clm.enbedded_length) { clm.enbedded_length = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.condition_bottom) { clm.condition_bottom = set; }
                if (dat == clm.condition_top) { clm.condition_top = set; }
                if (dat == clm.joint_top) { clm.joint_top = set; }
                if (dat == clm.joint_bottom) { clm.joint_bottom = set; }
                if (dat == clm.kind_joint_top) { clm.kind_joint_top = set; }
                if (dat == clm.kind_joint_bottom) { clm.kind_joint_bottom = set; }
            }
            catch
            { ret = false; }

            return ret;
        }
        //CFT柱円形
        private static bool SetCFTClmPipePara(FamilyStructure.CFT_Clm_Pipe clm, string dat, string set)
        {
            bool ret = true;
            try
            {   //タイプパラメータ                                
                if (dat == clm.strength_main) { clm.strength_main = set; }
                if (dat == clm.strength_concrete) { clm.strength_concrete = set; }
                if (dat == clm.kind_column) { clm.kind_column = set; }
                if (dat == clm.kind_column2) { clm.kind_column2 = set; }
                if (dat == clm.typename) { clm.typename = set; }
                if (dat == clm.D) { clm.D = set; }
                if (dat == clm.t) { clm.t = set; }
                if (dat == clm.name) { clm.name = set; }
                if (dat == clm.SecId) { clm.SecId = set; }
                if (dat == clm.base_type) { clm.base_type = set; }
                if (dat == clm.enbedded_length) { clm.enbedded_length = set; }

                //インスタンスパラメータ
                if (dat == clm.MemId) { clm.MemId = set; }
                if (dat == clm.NameMembers) { clm.NameMembers = set; }
                if (dat == clm.condition_bottom) { clm.condition_bottom = set; }
                if (dat == clm.condition_top) { clm.condition_top = set; }
                if (dat == clm.joint_top) { clm.joint_top = set; }
                if (dat == clm.joint_bottom) { clm.joint_bottom = set; }
                if (dat == clm.kind_joint_top) { clm.kind_joint_top = set; }
                if (dat == clm.kind_joint_bottom) { clm.kind_joint_bottom = set; }
            }
            catch
            { ret = false; }

            return ret;
        }
        #endregion
        #region 大梁
        //RC大梁
        private static bool SetRCGir(FamilyStructure.RC_Gir gir, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == gir.strength_concrete) { gir.strength_concrete = set; }
            if (dat == gir.kind_beam) { gir.kind_beam = set; }
            if (dat == gir.kind_beam2) { gir.kind_beam2 = set; }
            if (dat == gir.width_start) { gir.width_start = set; }
            if (dat == gir.width_center) { gir.width_center = set; }
            if (dat == gir.width_end) { gir.width_end = set; }
            if (dat == gir.depth_start) { gir.depth_start = set; }
            if (dat == gir.depth_center) { gir.depth_center = set; }
            if (dat == gir.depth_end) { gir.depth_end = set; }
            StringSet(gir.BHaunch, dat, set);
            if (dat == gir.haunch_start) { gir.haunch_start = set; }
            if (dat == gir.haunch_end) { gir.haunch_end = set; }
            if (dat == gir.name) { gir.name = set; }
            StringSet(gir.D_reinforcement_main_top, dat, set);
            StringSet(gir.D_reinforcement_main_bottom, dat, set);
            StringSet(gir.D_reinforcement_2nd_main_top, dat, set);
            StringSet(gir.D_reinforcement_2nd_main_bottom, dat, set);
            StringSet(gir.count_main_top_1st, dat, set);
            StringSet(gir.count_main_top_2nd, dat, set);
            StringSet(gir.count_main_top_3rd, dat, set);
            StringSet(gir.count_main_bottom_1st, dat, set);
            StringSet(gir.count_main_bottom_2nd, dat, set);
            StringSet(gir.count_main_bottom_3rd, dat, set);
            StringSet(gir.count_2nd_main_top_1st, dat, set);
            StringSet(gir.count_2nd_main_top_2nd, dat, set);
            StringSet(gir.count_2nd_main_top_3rd, dat, set);
            StringSet(gir.count_2nd_main_bottom_1st, dat, set);
            StringSet(gir.count_2nd_main_bottom_2nd, dat, set);
            StringSet(gir.count_2nd_main_bottom_3rd, dat, set);
            StringSet(gir.D_stirrup, dat, set);
            StringSet(gir.count_stirrup, dat, set);
            StringSet(gir.pitch_stirrup, dat, set);
            StringSet(gir.D_reinforcement_web, dat, set);
            StringSet(gir.count_web, dat, set);
            StringSet(gir.D_bar_spacing, dat, set);
            StringSet(gir.count_bar_spacing, dat, set);
            StringSet(gir.pitch_bar_spacing, dat, set);
            if (dat == gir.strength_reinforcement_main) { gir.strength_reinforcement_main = set; }
            if (dat == gir.strength_reinforcement_2nd_main) { gir.strength_reinforcement_2nd_main = set; }
            if (dat == gir.strength_stirrup) { gir.strength_stirrup = set; }
            if (dat == gir.strength_reinforcement_web) { gir.strength_reinforcement_web = set; }
            if (dat == gir.strength_bar_spacing) { gir.strength_bar_spacing = set; }
            if (dat == gir.depth_cover_left) { gir.depth_cover_left = set; }
            if (dat == gir.depth_cover_right) { gir.depth_cover_right = set; }
            if (dat == gir.depth_cover_top) { gir.depth_cover_top = set; }
            if (dat == gir.depth_cover_bottom) { gir.depth_cover_bottom = set; }
            if (dat == gir.interval_reinforcement) { gir.interval_reinforcement = set; }
            if (dat == gir.count_X_main_top) { gir.count_X_main_top = set; }
            if (dat == gir.count_X_main_bottom) { gir.count_X_main_bottom = set; }
            if (dat == gir.center_reinforcement_top) { gir.center_reinforcement_top = set; }
            if (dat == gir.center_reinforcement_bottom) { gir.center_reinforcement_bottom = set; }
            if (dat == gir.bar_length_start) { gir.bar_length_start = set; }
            if (dat == gir.bar_length_end) { gir.bar_length_end = set; }
            if (dat == gir.isOutIn) { gir.isOutIn = set; }
            if (dat == gir.SecId) { gir.SecId = set; }

            //インスタンスパラメータ
            if (dat == gir.MemId) { gir.MemId = set; }
            if (dat == gir.NameMembers) { gir.NameMembers = set; }
            if (dat == gir.thickness_ex_top) { gir.thickness_ex_top = set; }
            if (dat == gir.thickness_ex_bottom) { gir.thickness_ex_bottom = set; }
            if (dat == gir.thickness_ex_right) { gir.thickness_ex_right = set; }
            if (dat == gir.thickness_ex_left) { gir.thickness_ex_left = set; }
            if (dat == gir.kind_haunch_start) { gir.kind_haunch_start = set; }
            if (dat == gir.kind_haunch_end) { gir.kind_haunch_end = set; }
            if (dat == gir.type_haunch_H) { gir.type_haunch_H = set; }
            if (dat == gir.type_haunch_V) { gir.type_haunch_V = set; }
            return ret;
        }
        //S大梁
        private static bool SetSGir(FamilyStructure.S_Gir_H gir, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == gir.name) { gir.name = set; }
            StringSet(gir.strength_web, dat, set);
            StringSet(gir.strength_main, dat, set);
            if (dat == gir.kind_beam) { gir.kind_beam = set; }
            if (dat == gir.kind_beam2) { gir.kind_beam2 = set; }
            if (dat == gir.isOutIn) { gir.isOutIn = set; }
            StringSet(gir.A, dat, set);
            StringSet(gir.B, dat, set);
            StringSet(gir.t1, dat, set);
            StringSet(gir.t2, dat, set);
            StringSet(gir.r, dat, set);
            if (dat == gir.haunch_start) { gir.haunch_start = set; }
            if (dat == gir.haunch_end) { gir.haunch_end = set; }
            if (dat == gir.SecId) { gir.SecId = set; }
            StringSet(gir.shape, dat, set);
            StringSet(gir.type, dat, set);
            if(dat == gir.kind_brace) { gir.kind_brace = set; }

            //インスタンスパラメータ
            if (dat == gir.MemId) { gir.MemId = set; }
            if (dat == gir.NameMembers) { gir.NameMembers = set; }
            if (dat == gir.condition_start) { gir.condition_start = set; }
            if (dat == gir.condition_end) { gir.condition_end = set; }
            if (dat == gir.kind_haunch_start) { gir.kind_haunch_start = set; }
            if (dat == gir.kind_haunch_end) { gir.kind_haunch_end = set; }
            if (dat == gir.type_haunch_H) { gir.type_haunch_H = set; }
            if (dat == gir.type_haunch_V) { gir.type_haunch_V = set; }
            if (dat == gir.joint_start) { gir.joint_start = set; }
            if (dat == gir.joint_end) { gir.joint_end = set; }
            if (dat == gir.kind_joint_start) { gir.kind_joint_start = set; }
            if (dat == gir.kind_joint_end) { gir.kind_joint_end = set; }
            if(dat == gir.future_brace) { gir.future_brace = set; }

            return ret;
        }
        //S梁組立H形鋼
        private static bool SetSGir_BH(FamilyStructure.S_Gir_BH gir, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == gir.name) { gir.name = set; }
            StringSet(gir.strength_web, dat, set);
            StringSet(gir.strength_main, dat, set);
            if (dat == gir.kind_beam) { gir.kind_beam = set; }
            if (dat == gir.kind_beam2) { gir.kind_beam2 = set; }
            if (dat == gir.isOutIn) { gir.isOutIn = set; }
            StringSet(gir.A, dat, set);
            StringSet(gir.B, dat, set);
            StringSet(gir.t1, dat, set);
            StringSet(gir.t2, dat, set);
            StringSet(gir.r, dat, set);
            if (dat == gir.haunch_start) { gir.haunch_start = set; }
            if (dat == gir.haunch_end) { gir.haunch_end = set; }
            if (dat == gir.SecId) { gir.SecId = set; }
            StringSet(gir.shape, dat, set);
            if (dat == gir.kind_brace) { gir.kind_brace = set; }

            //インスタンスパラメータ
            if (dat == gir.MemId) { gir.MemId = set; }
            if (dat == gir.NameMembers) { gir.NameMembers = set; }
            if (dat == gir.condition_start) { gir.condition_start = set; }
            if (dat == gir.condition_end) { gir.condition_end = set; }
            if (dat == gir.kind_haunch_start) { gir.kind_haunch_start = set; }
            if (dat == gir.kind_haunch_end) { gir.kind_haunch_end = set; }
            if (dat == gir.type_haunch_H) { gir.type_haunch_H = set; }
            if (dat == gir.type_haunch_V) { gir.type_haunch_V = set; }
            if (dat == gir.joint_start) { gir.joint_start = set; }
            if (dat == gir.joint_end) { gir.joint_end = set; }
            if (dat == gir.kind_joint_start) { gir.kind_joint_start = set; }
            if (dat == gir.kind_joint_end) { gir.kind_joint_end = set; }
            if (dat == gir.future_brace) { gir.future_brace = set; }

            return ret;
        }
        private static bool SetSGir_C(FamilyStructure.S_Gir_C gir, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == gir.name) { gir.name = set; }
            if (dat == gir.strength) { gir.strength = set; }
            if (dat == gir.kind_beam) { gir.kind_beam = set; }
            if (dat == gir.kind_beam2) { gir.kind_beam2 = set; }
            if (dat == gir.isOutIn) { gir.isOutIn = set; }
            StringSet(gir.H, dat, set);
            StringSet(gir.B, dat, set);
            StringSet(gir.t1, dat, set);
            StringSet(gir.t2, dat, set);
            StringSet(gir.r1, dat, set);
            StringSet(gir.r2, dat, set);
            if (dat == gir.SecId) { gir.SecId = set; }
            StringSet(gir.shape, dat, set);
            StringSet(gir.type, dat, set);
            StringSet(gir.side, dat, set);
            if (dat == gir.haunch_start) { gir.haunch_start = set; }
            if (dat == gir.haunch_end) { gir.haunch_end = set; }
            if (dat == gir.kind_brace) { gir.kind_brace = set; }

            //インスタンスパラメータ
            if (dat == gir.MemId) { gir.MemId = set; }
            if (dat == gir.NameMembers) { gir.NameMembers = set; }
            if (dat == gir.condition_start) { gir.condition_start = set; }
            if (dat == gir.condition_end) { gir.condition_end = set; }
            if (dat == gir.kind_haunch_start) { gir.kind_haunch_start = set; }
            if (dat == gir.kind_haunch_end) { gir.kind_haunch_end = set; }
            if (dat == gir.type_haunch_H) { gir.type_haunch_H = set; }
            if (dat == gir.type_haunch_V) { gir.type_haunch_V = set; }
            if (dat == gir.joint_start) { gir.joint_start = set; }
            if (dat == gir.joint_end) { gir.joint_end = set; }
            if (dat == gir.kind_joint_start) { gir.kind_joint_start = set; }
            if (dat == gir.kind_joint_end) { gir.kind_joint_end = set; }
            if (dat == gir.future_brace) { gir.future_brace = set; }

            return ret;
        }
        private static bool SetSGir_L(FamilyStructure.S_Gir_L gir, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == gir.name) { gir.name = set; }
            if (dat == gir.strength) { gir.strength = set; }
            if (dat == gir.kind_beam) { gir.kind_beam = set; }
            if (dat == gir.kind_beam2) { gir.kind_beam2 = set; }
            if (dat == gir.isOutIn) { gir.isOutIn = set; }
            StringSet(gir.A, dat, set);
            StringSet(gir.B, dat, set);
            StringSet(gir.t1, dat, set);
            StringSet(gir.t2, dat, set);
            StringSet(gir.r1, dat, set);
            StringSet(gir.r2, dat, set);
            if (dat == gir.SecId) { gir.SecId = set; }
            StringSet(gir.shape, dat, set);
            StringSet(gir.type, dat, set);
            StringSet(gir.side, dat, set);
            if (dat == gir.haunch_start) { gir.haunch_start = set; }
            if (dat == gir.haunch_end) { gir.haunch_end = set; }
            if (dat == gir.kind_brace) { gir.kind_brace = set; }

            //インスタンスパラメータ
            if (dat == gir.MemId) { gir.MemId = set; }
            if (dat == gir.NameMembers) { gir.NameMembers = set; }
            if (dat == gir.condition_start) { gir.condition_start = set; }
            if (dat == gir.condition_end) { gir.condition_end = set; }
            if (dat == gir.kind_haunch_start) { gir.kind_haunch_start = set; }
            if (dat == gir.kind_haunch_end) { gir.kind_haunch_end = set; }
            if (dat == gir.type_haunch_H) { gir.type_haunch_H = set; }
            if (dat == gir.type_haunch_V) { gir.type_haunch_V = set; }
            if (dat == gir.joint_start) { gir.joint_start = set; }
            if (dat == gir.joint_end) { gir.joint_end = set; }
            if (dat == gir.kind_joint_start) { gir.kind_joint_start = set; }
            if (dat == gir.kind_joint_end) { gir.kind_joint_end = set; }
            if (dat == gir.future_brace) { gir.future_brace = set; }

            return ret;
        }
        private static bool SetSGir_LipC(FamilyStructure.S_Gir_LipC gir, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == gir.name) { gir.name = set; }
            if (dat == gir.strength) { gir.strength = set; }
            if (dat == gir.kind_beam) { gir.kind_beam = set; }
            if (dat == gir.kind_beam2) { gir.kind_beam2 = set; }
            if (dat == gir.isOutIn) { gir.isOutIn = set; }
            StringSet(gir.H, dat, set);
            StringSet(gir.A, dat, set);
            StringSet(gir.C, dat, set);
            StringSet(gir.t, dat, set);
            if (dat == gir.SecId) { gir.SecId = set; }
            StringSet(gir.shape, dat, set);
            StringSet(gir.type, dat, set);
            StringSet(gir.side, dat, set);
            if (dat == gir.haunch_start) { gir.haunch_start = set; }
            if (dat == gir.haunch_end) { gir.haunch_end = set; }
            if (dat == gir.kind_brace) { gir.kind_brace = set; }

            //インスタンスパラメータ
            if (dat == gir.MemId) { gir.MemId = set; }
            if (dat == gir.NameMembers) { gir.NameMembers = set; }
            if (dat == gir.condition_start) { gir.condition_start = set; }
            if (dat == gir.condition_end) { gir.condition_end = set; }
            if (dat == gir.kind_haunch_start) { gir.kind_haunch_start = set; }
            if (dat == gir.kind_haunch_end) { gir.kind_haunch_end = set; }
            if (dat == gir.type_haunch_H) { gir.type_haunch_H = set; }
            if (dat == gir.type_haunch_V) { gir.type_haunch_V = set; }
            if (dat == gir.joint_start) { gir.joint_start = set; }
            if (dat == gir.joint_end) { gir.joint_end = set; }
            if (dat == gir.kind_joint_start) { gir.kind_joint_start = set; }
            if (dat == gir.kind_joint_end) { gir.kind_joint_end = set; }
            if (dat == gir.future_brace) { gir.future_brace = set; }

            return ret;
        }
        private static bool SetSRCGir(FamilyStructure.SRC_Gir gir, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == gir.strength_concrete) { gir.strength_concrete = set; }
            if (dat == gir.kind_beam) { gir.kind_beam = set; }
            if (dat == gir.kind_beam2) { gir.kind_beam2 = set; }
            if (dat == gir.isOutIn) { gir.isOutIn = set; }
            if (dat == gir.width_start) { gir.width_start = set; }
            if (dat == gir.width_center) { gir.width_center = set; }
            if (dat == gir.width_end) { gir.width_end = set; }
            if (dat == gir.depth_start) { gir.depth_start = set; }
            if (dat == gir.depth_center) { gir.depth_center = set; }
            if (dat == gir.depth_end) { gir.depth_end = set; }
            if (dat == gir.haunch_start) { gir.haunch_start = set; }
            if (dat == gir.haunch_end) { gir.haunch_end = set; }
            if (dat == gir.name) { gir.name = set; }
            StringSet(gir.BHaunch, dat, set);
            StringSet(gir.D_reinforcement_main_top, dat, set);
            StringSet(gir.D_reinforcement_main_bottom, dat, set);
            StringSet(gir.D_reinforcement_2nd_main_top, dat, set);
            StringSet(gir.D_reinforcement_2nd_main_bottom, dat, set);
            StringSet(gir.count_main_top_1st, dat, set);
            StringSet(gir.count_main_top_2nd, dat, set);
            StringSet(gir.count_main_top_3rd, dat, set);
            StringSet(gir.count_main_bottom_1st, dat, set);
            StringSet(gir.count_main_bottom_2nd, dat, set);
            StringSet(gir.count_main_bottom_3rd, dat, set);
            StringSet(gir.count_2nd_main_top_1st, dat, set);
            StringSet(gir.count_2nd_main_top_2nd, dat, set);
            StringSet(gir.count_2nd_main_top_3rd, dat, set);
            StringSet(gir.count_2nd_main_bottom_1st, dat, set);
            StringSet(gir.count_2nd_main_bottom_2nd, dat, set);
            StringSet(gir.count_2nd_main_bottom_3rd, dat, set);
            StringSet(gir.D_stirrup, dat, set);
            StringSet(gir.count_stirrup, dat, set);
            StringSet(gir.pitch_stirrup, dat, set);
            StringSet(gir.D_reinforcement_web, dat, set);
            StringSet(gir.count_web, dat, set);
            StringSet(gir.D_bar_spacing, dat, set);
            StringSet(gir.count_bar_spacing, dat, set);
            StringSet(gir.pitch_bar_spacing, dat, set);
            if (dat == gir.strength_reinforcement_main) { gir.strength_reinforcement_main = set; }
            if (dat == gir.strength_reinforcement_2nd_main) { gir.strength_reinforcement_2nd_main = set; }
            if (dat == gir.strength_stirrup) { gir.strength_stirrup = set; }
            if (dat == gir.strength_reinforcement_web) { gir.strength_reinforcement_web = set; }
            if (dat == gir.strength_bar_spacing) { gir.strength_bar_spacing = set; }
            if (dat == gir.depth_cover_left) { gir.depth_cover_left = set; }
            if (dat == gir.depth_cover_right) { gir.depth_cover_right = set; }
            if (dat == gir.depth_cover_top) { gir.depth_cover_top = set; }
            if (dat == gir.depth_cover_bottom) { gir.depth_cover_bottom = set; }
            if (dat == gir.interval_reinforcement) { gir.interval_reinforcement = set; }
            if (dat == gir.count_X_main_top) { gir.count_X_main_top = set; }
            if (dat == gir.count_X_main_bottom) { gir.count_X_main_bottom = set; }
            if (dat == gir.center_reinforcement_top) { gir.center_reinforcement_top = set; }
            if (dat == gir.center_reinforcement_bottom) { gir.center_reinforcement_bottom = set; }
            if (dat == gir.SecId) { gir.SecId = set; }
            StringSet(gir.strength_web, dat, set);
            StringSet(gir.strength_main, dat, set);
            StringSet(gir.A, dat, set);
            StringSet(gir.B, dat, set);
            StringSet(gir.t1, dat, set);
            StringSet(gir.t2, dat, set);
            StringSet(gir.r, dat, set);
            StringSet(gir.shape, dat, set);
            StringSet(gir.type, dat, set);
            if (dat == gir.offset) { gir.offset = set; }
            if (dat == gir.level) { gir.level = set; }

            //インスタンスパラメータ
            if (dat == gir.MemId) { gir.MemId = set; }
            if (dat == gir.NameMembers) { gir.NameMembers = set; }
            if (dat == gir.thickness_ex_top) { gir.thickness_ex_top = set; }
            if (dat == gir.thickness_ex_bottom) { gir.thickness_ex_bottom = set; }
            if (dat == gir.thickness_ex_right) { gir.thickness_ex_right = set; }
            if (dat == gir.thickness_ex_left) { gir.thickness_ex_left = set; }
            if (dat == gir.condition_start) { gir.condition_start = set; }
            if (dat == gir.condition_end) { gir.condition_end = set; }
            if (dat == gir.kind_haunch_start) { gir.kind_haunch_start = set; }
            if (dat == gir.kind_haunch_end) { gir.kind_haunch_end = set; }
            if (dat == gir.type_haunch_H) { gir.type_haunch_H = set; }
            if (dat == gir.type_haunch_V) { gir.type_haunch_V = set; }
            if (dat == gir.joint_start) { gir.joint_start = set; }
            if (dat == gir.joint_end) { gir.joint_end = set; }
            if (dat == gir.kind_joint_start) { gir.kind_joint_start = set; }
            if (dat == gir.kind_joint_end) { gir.kind_joint_end = set; }

            return ret;
        }
        private static bool SetSRCCGir(FamilyStructure.SRC_CGir gir, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == gir.strength_concrete) { gir.strength_concrete = set; }
            if (dat == gir.isOutIn) { gir.isOutIn = set; }
            if (dat == gir.kind_beam) { gir.kind_beam = set; }
            if (dat == gir.kind_beam2) { gir.kind_beam2 = set; }
            if (dat == gir.width_start) { gir.width_start = set; }
            if (dat == gir.width_center) { gir.width_center = set; }
            if (dat == gir.width_end) { gir.width_end = set; }
            if (dat == gir.depth_start) { gir.depth_start = set; }
            if (dat == gir.depth_center) { gir.depth_center = set; }
            if (dat == gir.depth_end) { gir.depth_end = set; }
            StringSet(gir.BHaunch, dat, set);
            if (dat == gir.haunch_start) { gir.haunch_start = set; }
            if (dat == gir.haunch_end) { gir.haunch_end = set; }
            if (dat == gir.name) { gir.name = set; }
            StringSet(gir.D_reinforcement_main_top, dat, set);
            StringSet(gir.D_reinforcement_main_bottom, dat, set);
            StringSet(gir.D_reinforcement_2nd_main_top, dat, set);
            StringSet(gir.D_reinforcement_2nd_main_bottom, dat, set);
            StringSet(gir.count_main_top_1st, dat, set);
            StringSet(gir.count_main_top_2nd, dat, set);
            StringSet(gir.count_main_top_3rd, dat, set);
            StringSet(gir.count_main_bottom_1st, dat, set);
            StringSet(gir.count_main_bottom_2nd, dat, set);
            StringSet(gir.count_main_bottom_3rd, dat, set);
            StringSet(gir.count_2nd_main_top_1st, dat, set);
            StringSet(gir.count_2nd_main_top_2nd, dat, set);
            StringSet(gir.count_2nd_main_top_3rd, dat, set);
            StringSet(gir.count_2nd_main_bottom_1st, dat, set);
            StringSet(gir.count_2nd_main_bottom_2nd, dat, set);
            StringSet(gir.count_2nd_main_bottom_3rd, dat, set);
            StringSet(gir.D_stirrup, dat, set);
            StringSet(gir.count_stirrup, dat, set);
            StringSet(gir.pitch_stirrup, dat, set);
            StringSet(gir.D_reinforcement_web, dat, set);
            StringSet(gir.count_web, dat, set);
            StringSet(gir.D_bar_spacing, dat, set);
            StringSet(gir.count_bar_spacing, dat, set);
            StringSet(gir.pitch_bar_spacing, dat, set);
            if (dat == gir.strength_reinforcement_main) { gir.strength_reinforcement_main = set; }
            if (dat == gir.SecId) { gir.SecId = set; }
            if (dat == gir.strength_reinforcement_2nd_main) { gir.strength_reinforcement_2nd_main = set; }
            if (dat == gir.strength_stirrup) { gir.strength_stirrup = set; }
            if (dat == gir.strength_reinforcement_web) { gir.strength_reinforcement_web = set; }
            if (dat == gir.strength_bar_spacing) { gir.strength_bar_spacing = set; }
            if (dat == gir.center_reinforcement_top) { gir.center_reinforcement_top = set; }
            if (dat == gir.center_reinforcement_bottom) { gir.center_reinforcement_bottom = set; }
            if (dat == gir.depth_cover_left) { gir.depth_cover_left = set; }
            if (dat == gir.depth_cover_right) { gir.depth_cover_right = set; }
            if (dat == gir.depth_cover_top) { gir.depth_cover_top = set; }
            if (dat == gir.depth_cover_bottom) { gir.depth_cover_bottom = set; }
            StringSet(gir.strength_web, dat, set);
            StringSet(gir.strength_main, dat, set);
            StringSet(gir.A, dat, set);
            StringSet(gir.B, dat, set);
            StringSet(gir.t1, dat, set);
            StringSet(gir.t2, dat, set);
            StringSet(gir.r, dat, set);
            StringSet(gir.type, dat, set);
            StringSet(gir.shape, dat, set);
            if (dat == gir.offset) { gir.offset = set; }
            if (dat == gir.level) { gir.level = set; }
            if (dat == gir.interval_reinforcement) { gir.interval_reinforcement = set; }
            if (dat == gir.count_X_main_top) { gir.count_X_main_top = set; }
            if (dat == gir.count_X_main_bottom) { gir.count_X_main_bottom = set; }

            //インスタンスパラメータ
            if (dat == gir.MemId) { gir.MemId = set; }
            if (dat == gir.NameMembers) { gir.NameMembers = set; }
            if (dat == gir.thickness_ex_top) { gir.thickness_ex_top = set; }
            if (dat == gir.thickness_ex_bottom) { gir.thickness_ex_bottom = set; }
            if (dat == gir.thickness_ex_right) { gir.thickness_ex_right = set; }
            if (dat == gir.thickness_ex_left) { gir.thickness_ex_left = set; }
            if (dat == gir.condition_start) { gir.condition_start = set; }
            if (dat == gir.condition_end) { gir.condition_end = set; }
            if (dat == gir.kind_haunch_start) { gir.kind_haunch_start = set; }
            if (dat == gir.kind_haunch_end) { gir.kind_haunch_end = set; }
            if (dat == gir.type_haunch_H) { gir.type_haunch_H = set; }
            if (dat == gir.type_haunch_V) { gir.type_haunch_V = set; }
            if (dat == gir.joint_start) { gir.joint_start = set; }
            if (dat == gir.joint_end) { gir.joint_end = set; }
            if (dat == gir.kind_joint_start) { gir.kind_joint_start = set; }
            if (dat == gir.kind_joint_end) { gir.kind_joint_end = set; }

            return ret;
        }
        #endregion
        #region 片持ち梁
        //RC片持ち梁
        private static bool SetRCCGir(FamilyStructure.RC_CGir gir, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == gir.strength_concrete) { gir.strength_concrete = set; }
            if (dat == gir.kind_beam) { gir.kind_beam = set; }
            if (dat == gir.kind_beam2) { gir.kind_beam2 = set; }
            if (dat == gir.isOutIn) { gir.isOutIn = set; }
            if (dat == gir.width_start) { gir.width_start = set; }
            if (dat == gir.width_end) { gir.width_end = set; }
            if (dat == gir.depth_start) { gir.depth_start = set; }
            if (dat == gir.depth_end) { gir.depth_end = set; }
            StringSet(gir.BHaunch, dat, set);
            if (dat == gir.haunch_start) { gir.haunch_start = set; } //マッピングテーブルなし
            if (dat == gir.haunch_end) { gir.haunch_end = set; }     //マッピングテーブルなし
            if (dat == gir.name) { gir.name = set; }
            StringSet(gir.D_reinforcement_main_top, dat, set);
            StringSet(gir.D_reinforcement_main_bottom, dat, set);
            StringSet(gir.D_reinforcement_2nd_main_top, dat, set);
            StringSet(gir.D_reinforcement_2nd_main_bottom, dat, set);
            StringSet(gir.count_main_top_1st, dat, set);
            StringSet(gir.count_main_top_2nd, dat, set);
            StringSet(gir.count_main_top_3rd, dat, set);
            StringSet(gir.count_main_bottom_1st, dat, set);
            StringSet(gir.count_main_bottom_2nd, dat, set);
            StringSet(gir.count_main_bottom_3rd, dat, set);
            StringSet(gir.count_2nd_main_top_1st, dat, set);
            StringSet(gir.count_2nd_main_top_2nd, dat, set);
            StringSet(gir.count_2nd_main_top_3rd, dat, set);
            StringSet(gir.count_2nd_main_bottom_1st, dat, set);
            StringSet(gir.count_2nd_main_bottom_2nd, dat, set);
            StringSet(gir.count_2nd_main_bottom_3rd, dat, set);
            StringSet(gir.D_stirrup, dat, set);
            StringSet(gir.count_stirrup, dat, set);
            StringSet(gir.pitch_stirrup, dat, set);
            StringSet(gir.D_reinforcement_web, dat, set);
            StringSet(gir.count_web, dat, set);
            StringSet(gir.D_bar_spacing, dat, set);
            StringSet(gir.count_bar_spacing, dat, set);
            StringSet(gir.pitch_bar_spacing, dat, set);
            if (dat == gir.strength_reinforcement_main) { gir.strength_reinforcement_main = set; }
            if (dat == gir.SecId) { gir.SecId = set; }
            if (dat == gir.strength_reinforcement_2nd_main) { gir.strength_reinforcement_2nd_main = set; }
            if (dat == gir.strength_stirrup) { gir.strength_stirrup = set; }
            if (dat == gir.strength_reinforcement_web) { gir.strength_reinforcement_web = set; }
            if (dat == gir.strength_bar_spacing) { gir.strength_bar_spacing = set; }
            if (dat == gir.depth_cover_left) { gir.depth_cover_left = set; }
            if (dat == gir.depth_cover_right) { gir.depth_cover_right = set; }
            if (dat == gir.depth_cover_top) { gir.depth_cover_top = set; }
            if (dat == gir.depth_cover_bottom) { gir.depth_cover_bottom = set; }
            if (dat == gir.interval_reinforcement) { gir.interval_reinforcement = set; }
            if (dat == gir.count_X_main_top) { gir.count_X_main_top = set; }
            if (dat == gir.count_X_main_bottom) { gir.count_X_main_bottom = set; }
            if (dat == gir.center_reinforcement_top) { gir.center_reinforcement_top = set; }
            if (dat == gir.center_reinforcement_bottom) { gir.center_reinforcement_bottom = set; }
            if (dat == gir.bar_length_start) { gir.bar_length_start = set; }
            if (dat == gir.bar_length_end) { gir.bar_length_end = set; }

            //インスタンスパラメータ
            if (dat == gir.MemId) { gir.MemId = set; }
            if (dat == gir.NameMembers) { gir.NameMembers = set; }
            if (dat == gir.thickness_ex_top) { gir.thickness_ex_top = set; }
            if (dat == gir.thickness_ex_bottom) { gir.thickness_ex_bottom = set; }
            if (dat == gir.thickness_ex_right) { gir.thickness_ex_right = set; }
            if (dat == gir.thickness_ex_left) { gir.thickness_ex_left = set; }
            if (dat == gir.kind_haunch_start) { gir.kind_haunch_start = set; }
            if (dat == gir.kind_haunch_end) { gir.kind_haunch_end = set; }
            if (dat == gir.type_haunch_H) { gir.type_haunch_H = set; }
            if (dat == gir.type_haunch_V) { gir.type_haunch_V = set; }

            return ret;
        }
        //S片持ち梁
        private static bool SetSCGir(FamilyStructure.S_CGir_H gir, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            StringSet(gir.strength_web, dat, set);
            StringSet(gir.strength_main, dat, set);
            if (dat == gir.name) { gir.name = set; }
            if (dat == gir.kind_beam) { gir.kind_beam = set; }
            if (dat == gir.kind_beam2) { gir.kind_beam2 = set; }
            StringSet(gir.A, dat, set);
            StringSet(gir.B, dat, set);
            StringSet(gir.t1, dat, set);
            StringSet(gir.t2, dat, set);
            StringSet(gir.r, dat, set);
            if (dat == gir.haunch_start) { gir.haunch_start = set; }
            if (dat == gir.haunch_end) { gir.haunch_end = set; }
            if (dat == gir.SecId) { gir.SecId = set; }
            StringSet(gir.shape, dat, set);
            StringSet(gir.type, dat, set);

            //インスタンスパラメータ
            if (dat == gir.MemId) { gir.MemId = set; }
            if (dat == gir.NameMembers) { gir.NameMembers = set; }
            if (dat == gir.condition_start) { gir.condition_start = set; }
            if (dat == gir.condition_end) { gir.condition_end = set; }
            if (dat == gir.kind_haunch_start) { gir.kind_haunch_start = set; }
            if (dat == gir.kind_haunch_end) { gir.kind_haunch_end = set; }
            if (dat == gir.type_haunch_H) { gir.type_haunch_H = set; }
            if (dat == gir.type_haunch_V) { gir.type_haunch_V = set; }
            if (dat == gir.joint_start) { gir.joint_start = set; }
            if (dat == gir.joint_end) { gir.joint_end = set; }
            if (dat == gir.kind_joint_start) { gir.kind_joint_start = set; }
            if (dat == gir.kind_joint_end) { gir.kind_joint_end = set; }
            return ret;
        }
        #endregion
        //スラブ
        private static bool SetSlab(FamilyStructure.Slab sla, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == sla.isEarthen) { sla.isEarthen = set; }
            if (dat == sla.isCanti) { sla.isCanti = set; } //マッピングテーブルなし
            if (dat == sla.FigureType) { sla.FigureType = set; }
            if (dat == sla.product_type) { sla.product_type = set; }
            if (dat == sla.product_company) { sla.product_company = set; }
            if (dat == sla.product_name) { sla.product_name = set; }
            if (dat == sla.product_code) { sla.product_code = set; }
            if (dat == sla.depth_center) { sla.depth_center = set; }
            if (dat == sla.depth_tip) { sla.depth_tip = set; }
            if (dat == sla.depth_base) { sla.depth_base = set; }
            if (dat == sla.product_depth) { sla.product_depth = set; }
            if (dat == sla.length_haunch) { sla.length_haunch = set; }
            if (dat == sla.name) { sla.name = set; }
            if (dat == sla.ArrengementType) { sla.ArrengementType = set; }
            StringSet(sla.D1, dat, set);
            StringSet(sla.D2, dat, set);
            StringSet(sla.pitch, dat, set);
            StringSet(sla.T_D1, dat, set);
            StringSet(sla.T_D2, dat, set);
            StringSet(sla.T_pitch, dat, set);
            if (dat == sla.addD) { sla.addD = set; }
            if (dat == sla.addpitch) { sla.addpitch = set; }
            StringSet(sla.D_op, dat, set);
            StringSet(sla.count_op, dat, set);
            StringSet(sla.length_op, dat, set);
            if (dat == sla.strength) { sla.strength = set; }
            if (dat == sla.depth_cover_top) { sla.depth_cover_top = set; }
            if (dat == sla.depth_cover_bottom) { sla.depth_cover_bottom = set; }
            if (dat == sla.SecId) { sla.SecId = set; }
            if (dat == sla.D_bar_spacing) { sla.D_bar_spacing = set; }
            if (dat == sla.pitch_bar_spacing) { sla.pitch_bar_spacing = set; }

            //インスタンスパラメータ
            if (dat == sla.MemId) { sla.MemId = set; }
            if (dat == sla.NameMembers) { sla.NameMembers = set; }
            if (dat == sla.thickness_ex_upper) { sla.thickness_ex_upper = set; }
            if (dat == sla.thickness_ex_bottom) { sla.thickness_ex_bottom = set; }
            if (dat == sla.dir_load) { sla.dir_load = set; }
            if (dat == sla.angle_load) { sla.angle_load = set; }
            if (dat == sla.isFoundation) { sla.isFoundation = set; }
            if (dat == sla.type_haunch) { sla.type_haunch = set; }
            if (dat == sla.kind_slab) { sla.kind_slab = set; }
            if (dat == sla.kind_structure) { sla.kind_structure = set; }
            return ret;
        }
        //壁
        private static bool SetWall(FamilyStructure.Wall wal, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == wal.name) { wal.name = set; }
            if (dat == wal.ArrengementType) { wal.ArrengementType = set; }
            if (dat == wal.strength) { wal.strength = set; }
            StringSet(wal.D, dat, set);
            StringSet(wal.D2, dat, set);
            StringSet(wal.pitch, dat, set);
            StringSet(wal.D_inout, dat, set);
            StringSet(wal.D2_inout, dat, set);
            StringSet(wal.pitch_inout, dat, set);
            StringSet(wal.D_Edge, dat, set);
            StringSet(wal.count_Edge, dat, set);
            StringSet(wal.D_op, dat, set);
            StringSet(wal.count_op, dat, set);
            StringSet(wal.length_op, dat, set);
            if (dat == wal.kind_form) { wal.kind_form = set; }
            if (dat == wal.isTip_line) { wal.isTip_line = set; }
            if (dat == wal.depth_T) { wal.depth_T = set; }
            if (dat == wal.depth_H) { wal.depth_H = set; }
            if (dat == wal.depth_T1) { wal.depth_T1 = set; }
            if (dat == wal.depth_H1) { wal.depth_H1 = set; }
            if (dat == wal.depth_H2) { wal.depth_H2 = set; }
            if (dat == wal.depth_H3) { wal.depth_H3 = set; }
            if (dat == wal.strength_Tip) { wal.strength_Tip = set; } //マッピングテーブルなし
            if (dat == wal.D_bar_spacing) { wal.D_bar_spacing = set; }
            if (dat == wal.pitch_bar_spacing) { wal.pitch_bar_spacing = set; }
            StringSet(wal.D_Tip, dat, set);
            StringSet(wal.pitch_Tip, dat, set);
            StringSet(wal.count_Tip, dat, set);
            StringSet(wal.D_Edge_Para, dat, set);
            StringSet(wal.count_Edge_Para, dat, set);
            if (dat == wal.depth_cover_outside) { wal.depth_cover_outside = set; }
            if (dat == wal.depth_cover_inside) { wal.depth_cover_inside = set; }
            if (dat == wal.SecId) { wal.SecId = set; }

            //インスタンスパラメータ
            if (dat == wal.kind_structure) { wal.kind_structure = set; }
            if (dat == wal.kind_layout) { wal.kind_layout = set; }
            if (dat == wal.kind_wall) { wal.kind_wall = set; }
            if (dat == wal.type_outside) { wal.type_outside = set; }
            if (dat == wal.isPress) { wal.isPress = set; }
            if (dat == wal.MemId) { wal.MemId = set; }
            if (dat == wal.NameMembers) { wal.NameMembers = set; }
            if (dat == wal.thickness_ex_right) { wal.thickness_ex_right = set; }
            if (dat == wal.thickness_ex_left) { wal.thickness_ex_left = set; }
            if (dat == wal.slit_upper) { wal.slit_upper = set; }
            if (dat == wal.slit_bottom) { wal.slit_bottom = set; }
            if (dat == wal.slit_left) { wal.slit_left = set; }
            if (dat == wal.slit_right) { wal.slit_right = set; }
            if (dat == wal.direction) { wal.direction = set; }
            return ret;
        }
        #region Sブレース
        //SブレースH形鋼
        private static bool SetSBra(FamilyStructure.S_Bra_H bra, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == bra.name) { bra.name = set; }
            StringSet(bra.strength_web, dat, set);
            StringSet(bra.strength_main, dat, set);
            StringSet(bra.A, dat, set);
            StringSet(bra.B, dat, set);
            StringSet(bra.t1, dat, set);
            StringSet(bra.t2, dat, set);
            StringSet(bra.r, dat, set);
            if (dat == bra.SecId) { bra.SecId = set; }
            StringSet(bra.shape, dat, set);
            StringSet(bra.type, dat, set);
            if (dat == bra.kind_brace) { bra.kind_brace = set; }

            //インスタンスパラメータ
            if (dat == bra.MemId) { bra.MemId = set; }
            if (dat == bra.NameMembers) { bra.NameMembers = set; }
            if (dat == bra.condition_start) { bra.condition_start = set; }
            if (dat == bra.condition_end) { bra.condition_end = set; }
            if (dat == bra.joint_start) { bra.joint_start = set; }
            if (dat == bra.joint_end) { bra.joint_end = set; }
            if (dat == bra.kind_joint_start) { bra.kind_joint_start = set; }
            if (dat == bra.kind_joint_end) { bra.kind_joint_end = set; }
            if (dat == bra.future_brace) { bra.future_brace = set; }

            return ret;
        }
        //Sブレース組立H形鋼
        private static bool SetSBra_BH(FamilyStructure.S_Bra_BH bra, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == bra.name) { bra.name = set; }
            StringSet(bra.strength_web, dat, set);
            StringSet(bra.strength_main, dat, set);
            StringSet(bra.A, dat, set);
            StringSet(bra.B, dat, set);
            StringSet(bra.t1, dat, set);
            StringSet(bra.t2, dat, set);
            StringSet(bra.r, dat, set);
            if (dat == bra.SecId) { bra.SecId = set; }
            StringSet(bra.shape, dat, set);
            if (dat == bra.kind_brace) { bra.kind_brace = set; }

            //インスタンスパラメータ
            if (dat == bra.MemId) { bra.MemId = set; }
            if (dat == bra.NameMembers) { bra.NameMembers = set; }
            if (dat == bra.condition_start) { bra.condition_start = set; }
            if (dat == bra.condition_end) { bra.condition_end = set; }
            if (dat == bra.joint_start) { bra.joint_start = set; }
            if (dat == bra.joint_end) { bra.joint_end = set; }
            if (dat == bra.kind_joint_start) { bra.kind_joint_start = set; }
            if (dat == bra.kind_joint_end) { bra.kind_joint_end = set; }
            if (dat == bra.future_brace) { bra.future_brace = set; }
            return ret;
        }
        private static bool SetSBra_Box(FamilyStructure.S_Bra_Box bra, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == bra.strength) { bra.strength = set; }
            if (dat == bra.kind_brace) { bra.kind_brace = set; }
            if (dat == bra.shape) { bra.shape = set; }
            if (dat == bra.type) { bra.type = set; }
            if (dat == bra.H) { bra.H = set; }
            if (dat == bra.B) { bra.B = set; }
            if (dat == bra.t1) { bra.t1 = set; }
            if (dat == bra.t2) { bra.t2 = set; }
            if (dat == bra.r) { bra.r = set; }
            if (dat == bra.name) { bra.name = set; }
            if (dat == bra.SecId) { bra.SecId = set; }

            //インスタンスパラメータ
            if (dat == bra.MemId) { bra.MemId = set; }
            if (dat == bra.NameMembers) { bra.NameMembers = set; }
            if (dat == bra.condition_start) { bra.condition_start = set; }
            if (dat == bra.condition_end) { bra.condition_end = set; }
            if (dat == bra.joint_start) { bra.joint_start = set; }
            if (dat == bra.joint_end) { bra.joint_end = set; }
            if (dat == bra.kind_joint_start) { bra.kind_joint_start = set; }
            if (dat == bra.kind_joint_end) { bra.kind_joint_end = set; }
            if (dat == bra.future_brace) { bra.future_brace = set; }
            return ret;
        }
        private static bool SetSBra_BBox(FamilyStructure.S_Bra_BBox bra, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == bra.strength) { bra.strength = set; }
            if (dat == bra.kind_brace) { bra.kind_brace = set; }
            if (dat == bra.shape) { bra.shape = set; }
            if (dat == bra.H) { bra.H = set; }
            if (dat == bra.B) { bra.B = set; }
            if (dat == bra.t1) { bra.t1 = set; }
            if (dat == bra.t2) { bra.t2 = set; }
            if (dat == bra.name) { bra.name = set; }
            if (dat == bra.SecId) { bra.SecId = set; }

            //インスタンスパラメータ
            if (dat == bra.MemId) { bra.MemId = set; }
            if (dat == bra.NameMembers) { bra.NameMembers = set; }
            if (dat == bra.condition_start) { bra.condition_start = set; }
            if (dat == bra.condition_end) { bra.condition_end = set; }
            if (dat == bra.joint_start) { bra.joint_start = set; }
            if (dat == bra.joint_end) { bra.joint_end = set; }
            if (dat == bra.kind_joint_start) { bra.kind_joint_start = set; }
            if (dat == bra.kind_joint_end) { bra.kind_joint_end = set; }
            if (dat == bra.future_brace) { bra.future_brace = set; }
            return ret;
        }
        private static bool SetSBra_Pipe(FamilyStructure.S_Bra_Pipe bra, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == bra.strength) { bra.strength = set; }
            if (dat == bra.kind_brace) { bra.kind_brace = set; }
            if (dat == bra.shape) { bra.shape = set; }
            if (dat == bra.D) { bra.D = set; }
            if (dat == bra.t) { bra.t = set; }
            if (dat == bra.name) { bra.name = set; }
            if (dat == bra.SecId) { bra.SecId = set; }

            //インスタンスパラメータ
            if (dat == bra.MemId) { bra.MemId = set; }
            if (dat == bra.NameMembers) { bra.NameMembers = set; }
            if (dat == bra.condition_start) { bra.condition_start = set; }
            if (dat == bra.condition_end) { bra.condition_end = set; }
            if (dat == bra.joint_start) { bra.joint_start = set; }
            if (dat == bra.joint_end) { bra.joint_end = set; }
            if (dat == bra.kind_joint_start) { bra.kind_joint_start = set; }
            if (dat == bra.kind_joint_end) { bra.kind_joint_end = set; }
            if (dat == bra.future_brace) { bra.future_brace = set; }
            return ret;
        }
        //Sブレース溝形鋼
        private static bool SetSBra_C(FamilyStructure.S_Bra_C bra, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == bra.name) { bra.name = set; }
            if (dat == bra.strength) { bra.strength = set; }
            StringSet(bra.H, dat, set);
            StringSet(bra.B, dat, set);
            StringSet(bra.t1, dat, set);
            StringSet(bra.t2, dat, set);
            StringSet(bra.r1, dat, set);
            StringSet(bra.r2, dat, set);
            if (dat == bra.SecId) { bra.SecId = set; }
            StringSet(bra.shape, dat, set);
            StringSet(bra.type, dat, set);
            StringSet(bra.side, dat, set);
            if (dat == bra.kind_brace) { bra.kind_brace = set; }

            //インスタンスパラメータ
            if (dat == bra.MemId) { bra.MemId = set; }
            if (dat == bra.NameMembers) { bra.NameMembers = set; }
            if (dat == bra.condition_start) { bra.condition_start = set; }
            if (dat == bra.condition_end) { bra.condition_end = set; }
            if (dat == bra.joint_start) { bra.joint_start = set; }
            if (dat == bra.joint_end) { bra.joint_end = set; }
            if (dat == bra.kind_joint_start) { bra.kind_joint_start = set; }
            if (dat == bra.kind_joint_end) { bra.kind_joint_end = set; }
            if (dat == bra.future_brace) { bra.future_brace = set; }
            return ret;
        }
        //Sブレース山形鋼
        private static bool SetSBra_L(FamilyStructure.S_Bra_L bra, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == bra.name) { bra.name = set; }
            if (dat == bra.strength) { bra.strength = set; }
            StringSet(bra.A, dat, set);
            StringSet(bra.B, dat, set);
            StringSet(bra.t1, dat, set);
            StringSet(bra.t2, dat, set);
            StringSet(bra.r1, dat, set);
            StringSet(bra.r2, dat, set);
            if (dat == bra.SecId) { bra.SecId = set; }
            StringSet(bra.shape, dat, set);
            StringSet(bra.type, dat, set);
            StringSet(bra.side, dat, set);
            if (dat == bra.kind_brace) { bra.kind_brace = set; }

            //インスタンスパラメータ
            if (dat == bra.MemId) { bra.MemId = set; }
            if (dat == bra.NameMembers) { bra.NameMembers = set; }
            if (dat == bra.condition_start) { bra.condition_start = set; }
            if (dat == bra.condition_end) { bra.condition_end = set; }
            if (dat == bra.joint_start) { bra.joint_start = set; }
            if (dat == bra.joint_end) { bra.joint_end = set; }
            if (dat == bra.kind_joint_start) { bra.kind_joint_start = set; }
            if (dat == bra.kind_joint_end) { bra.kind_joint_end = set; }
            if (dat == bra.future_brace) { bra.future_brace = set; }
            return ret;
        }
        //Sブレースリップ溝形鋼
        private static bool SetSBra_LipC(FamilyStructure.S_Bra_LipC bra, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == bra.name) { bra.name = set; }
            if (dat == bra.strength) { bra.strength = set; }
            StringSet(bra.H, dat, set);
            StringSet(bra.A, dat, set);
            StringSet(bra.C, dat, set);
            StringSet(bra.t, dat, set);
            if (dat == bra.SecId) { bra.SecId = set; }
            StringSet(bra.shape, dat, set);
            StringSet(bra.type, dat, set);
            StringSet(bra.side, dat, set);
            if (dat == bra.kind_brace) { bra.kind_brace = set; }

            //インスタンスパラメータ
            if (dat == bra.MemId) { bra.MemId = set; }
            if (dat == bra.NameMembers) { bra.NameMembers = set; }
            if (dat == bra.condition_start) { bra.condition_start = set; }
            if (dat == bra.condition_end) { bra.condition_end = set; }
            if (dat == bra.joint_start) { bra.joint_start = set; }
            if (dat == bra.joint_end) { bra.joint_end = set; }
            if (dat == bra.kind_joint_start) { bra.kind_joint_start = set; }
            if (dat == bra.kind_joint_end) { bra.kind_joint_end = set; }
            if (dat == bra.future_brace) { bra.future_brace = set; }
            return ret;
        }

        private static bool SetSBra_Bar(FamilyStructure.S_Bra_RollBar bra, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == bra.strength_main) { bra.strength_main = set; }
            if (dat == bra.kind_brace) { bra.kind_brace = set; }
            if (dat == bra.shape) { bra.shape = set; }
            if (dat == bra.D) { bra.D = set; }
            if (dat == bra.name) { bra.name = set; }
            if (dat == bra.SecId) { bra.SecId = set; }

            //インスタンスパラメータ
            if (dat == bra.MemId) { bra.MemId = set; }
            if (dat == bra.NameMembers) { bra.NameMembers = set; }
            if (dat == bra.condition_start) { bra.condition_start = set; }
            if (dat == bra.condition_end) { bra.condition_end = set; }
            if (dat == bra.joint_start) { bra.joint_start = set; }
            if (dat == bra.joint_end) { bra.joint_end = set; }
            if (dat == bra.kind_joint_start) { bra.kind_joint_start = set; }
            if (dat == bra.kind_joint_end) { bra.kind_joint_end = set; }
            if (dat == bra.future_brace) { bra.future_brace = set; }
            return ret;
        }
        private static bool SetSBra_FB(FamilyStructure.S_Bra_FB bra, string dat, string set)
        {
            bool ret = true;
            //タイプパラメータ
            if (dat == bra.strength_main) { bra.strength_main = set; }
            if (dat == bra.kind_brace) { bra.kind_brace = set; }
            if (dat == bra.shape) { bra.shape = set; }
            if (dat == bra.B) { bra.B = set; }
            if (dat == bra.t) { bra.t = set; }
            if (dat == bra.name) { bra.name = set; }
            if (dat == bra.SecId) { bra.SecId = set; }

            //インスタンスパラメータ
            if (dat == bra.MemId) { bra.MemId = set; }
            if (dat == bra.NameMembers) { bra.NameMembers = set; }
            if (dat == bra.condition_start) { bra.condition_start = set; }
            if (dat == bra.condition_end) { bra.condition_end = set; }
            if (dat == bra.joint_start) { bra.joint_start = set; }
            if (dat == bra.joint_end) { bra.joint_end = set; }
            if (dat == bra.kind_joint_start) { bra.kind_joint_start = set; }
            if (dat == bra.kind_joint_end) { bra.kind_joint_end = set; }
            if (dat == bra.future_brace) { bra.future_brace = set; }
            return ret;
        }
        #endregion
        #region 基礎
        //基礎矩形
        private static bool SetFRec(FamilyStructure.Foundation_Rect footing, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == footing.strength_concrete) { footing.strength_concrete = set; }
            if (dat == footing.SecId) { footing.SecId = set; }
            if (dat == footing.name) { footing.name = set; }
            if (dat == footing.depth_cover_top) { footing.depth_cover_top = set; }
            if (dat == footing.depth_cover_bottom) { footing.depth_cover_bottom = set; }
            if (dat == footing.depth_cover_side) { footing.depth_cover_side = set; }
            if (dat == footing.DX) { footing.DX = set; }
            if (dat == footing.DY) { footing.DY = set; }
            if (dat == footing.depth) { footing.depth = set; }
            if (dat == footing.strength) { footing.strength = set; }
            StringSet(footing.D, dat, set);
            StringSet(footing.count, dat, set);

            //インスタンスパラメータ
            if (dat == footing.MemId) { footing.MemId = set; }
            if (dat == footing.NameMembers) { footing.NameMembers = set; }
            if (dat == footing.thickness_ex_start_X) { footing.thickness_ex_start_X = set; }
            if (dat == footing.thickness_ex_end_X) { footing.thickness_ex_end_X = set; }
            if (dat == footing.thickness_ex_start_Y) { footing.thickness_ex_start_Y = set; }
            if (dat == footing.thickness_ex_end_Y) { footing.thickness_ex_end_Y = set; }
            if (dat == footing.thickness_ex_top) { footing.thickness_ex_top = set; }
            if (dat == footing.thickness_ex_bottom) { footing.thickness_ex_bottom = set; }

            return ret;
        }
        //基礎矩形テーパー
        private static bool SetFTRec(FamilyStructure.Foundation_Tapered_Rect footing, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == footing.SecId) { footing.SecId = set; }
            if (dat == footing.name) { footing.name = set; }
            if (dat == footing.strength_concrete) { footing.strength_concrete = set; }
            if (dat == footing.depth_cover_top) { footing.depth_cover_top = set; }
            if (dat == footing.depth_cover_bottom) { footing.depth_cover_bottom = set; }
            if (dat == footing.depth_cover_side) { footing.depth_cover_side = set; }
            if (dat == footing.DX) { footing.DX = set; }
            if (dat == footing.DY) { footing.DY = set; }
            if (dat == footing.t_DX) { footing.t_DX = set; }
            if (dat == footing.t_DY) { footing.t_DY = set; }
            if (dat == footing.t_offset_X) { footing.t_offset_X = set; }
            if (dat == footing.t_offset_Y) { footing.t_offset_Y = set; }
            if (dat == footing.depth_base) { footing.depth_base = set; }
            if (dat == footing.depth_tip) { footing.depth_tip = set; }
            if (dat == footing.strength) { footing.strength = set; }
            StringSet(footing.D, dat, set);
            StringSet(footing.count, dat, set);

            //インスタンスパラメータ
            if (dat == footing.MemId) { footing.MemId = set; }
            if (dat == footing.NameMembers) { footing.NameMembers = set; }
            if (dat == footing.thickness_ex_start_X) { footing.thickness_ex_start_X = set; }
            if (dat == footing.thickness_ex_end_X) { footing.thickness_ex_end_X = set; }
            if (dat == footing.thickness_ex_start_Y) { footing.thickness_ex_start_Y = set; }
            if (dat == footing.thickness_ex_end_Y) { footing.thickness_ex_end_Y = set; }
            if (dat == footing.thickness_ex_top) { footing.thickness_ex_top = set; }
            if (dat == footing.thickness_ex_bottom) { footing.thickness_ex_bottom = set; }

            return ret;
        }
        //基礎三角
        private static bool SetFTri(FamilyStructure.Foundation_Triangle footing, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == footing.strength_concrete) { footing.strength_concrete = set; }
            if (dat == footing.DX) { footing.DX = set; }
            if (dat == footing.DY) { footing.DY = set; }
            if (dat == footing.depth) { footing.depth = set; }
            if (dat == footing.name) { footing.name = set; }
            if (dat == footing.SecId) { footing.SecId = set; }
            if (dat == footing.strength) { footing.strength = set; }
            StringSet(footing.D, dat, set);
            StringSet(footing.count, dat, set);
            if (dat == footing.depth_cover_top) { footing.depth_cover_top = set; }
            if (dat == footing.depth_cover_bottom) { footing.depth_cover_bottom = set; }
            if (dat == footing.depth_cover_side) { footing.depth_cover_side = set; }

            //インスタンスパラメータ
            if (dat == footing.MemId) { footing.MemId = set; }
            if (dat == footing.NameMembers) { footing.NameMembers = set; }
            if (dat == footing.thickness_ex_start_X) { footing.thickness_ex_start_X = set; }
            if (dat == footing.thickness_ex_end_X) { footing.thickness_ex_end_X = set; }
            if (dat == footing.thickness_ex_start_Y) { footing.thickness_ex_start_Y = set; }
            if (dat == footing.thickness_ex_end_Y) { footing.thickness_ex_end_Y = set; }
            if (dat == footing.thickness_ex_top) { footing.thickness_ex_top = set; }
            if (dat == footing.thickness_ex_bottom) { footing.thickness_ex_bottom = set; }

            return ret;
        }
        private static bool SetFEqui_Tri(FamilyStructure.Foundation_Equi_Triangle footing, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == footing.strength_concrete) { footing.strength_concrete = set; }
            if (dat == footing.B) { footing.B = set; }
            if (dat == footing.C) { footing.C = set; }
            if (dat == footing.depth) { footing.depth = set; }
            if (dat == footing.name) { footing.name = set; }
            if (dat == footing.SecId) { footing.SecId = set; }
            if (dat == footing.strength) { footing.strength = set; }
            StringSet(footing.D, dat, set);
            StringSet(footing.count, dat, set);
            if (dat == footing.depth_cover_top) { footing.depth_cover_top = set; }
            if (dat == footing.depth_cover_bottom) { footing.depth_cover_bottom = set; }
            if (dat == footing.depth_cover_side) { footing.depth_cover_side = set; }

            //インスタンスパラメータ
            if (dat == footing.MemId) { footing.MemId = set; }
            if (dat == footing.NameMembers) { footing.NameMembers = set; }
            if (dat == footing.thickness_ex_start_X) { footing.thickness_ex_start_X = set; }
            if (dat == footing.thickness_ex_end_X) { footing.thickness_ex_end_X = set; }
            if (dat == footing.thickness_ex_start_Y) { footing.thickness_ex_start_Y = set; }
            if (dat == footing.thickness_ex_end_Y) { footing.thickness_ex_end_Y = set; }
            if (dat == footing.thickness_ex_top) { footing.thickness_ex_top = set; }
            if (dat == footing.thickness_ex_bottom) { footing.thickness_ex_bottom = set; }

            return ret;
        }
        private static bool SetFOcta(FamilyStructure.Foundation_Octagon footing, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == footing.strength_concrete) { footing.strength_concrete = set; }
            if (dat == footing.DX) { footing.DX = set; }
            if (dat == footing.DY) { footing.DY = set; }
            if (dat == footing.CX1) { footing.CX1 = set; }
            if (dat == footing.CY1) { footing.CY1 = set; }
            if (dat == footing.CX2) { footing.CX2 = set; }
            if (dat == footing.CY2) { footing.CY2 = set; }
            if (dat == footing.CX3) { footing.CX3 = set; }
            if (dat == footing.CY3) { footing.CY3 = set; }
            if (dat == footing.CX4) { footing.CX4 = set; }
            if (dat == footing.CY4) { footing.CY4 = set; }
            if (dat == footing.depth) { footing.depth = set; }
            if (dat == footing.name) { footing.name = set; }
            if (dat == footing.SecId) { footing.SecId = set; }
            if (dat == footing.strength) { footing.strength = set; }
            StringSet(footing.D, dat, set);
            StringSet(footing.count, dat, set);
            if (dat == footing.depth_cover_top) { footing.depth_cover_top = set; }
            if (dat == footing.depth_cover_bottom) { footing.depth_cover_bottom = set; }
            if (dat == footing.depth_cover_side) { footing.depth_cover_side = set; }

            //インスタンスパラメータ
            if (dat == footing.MemId) { footing.MemId = set; }
            if (dat == footing.NameMembers) { footing.NameMembers = set; }
            if (dat == footing.thickness_ex_start_X) { footing.thickness_ex_start_X = set; }
            if (dat == footing.thickness_ex_end_X) { footing.thickness_ex_end_X = set; }
            if (dat == footing.thickness_ex_start_Y) { footing.thickness_ex_start_Y = set; }
            if (dat == footing.thickness_ex_end_Y) { footing.thickness_ex_end_Y = set; }
            if (dat == footing.thickness_ex_top) { footing.thickness_ex_top = set; }
            if (dat == footing.thickness_ex_bottom) { footing.thickness_ex_bottom = set; }

            return ret;
        }
        //布基礎
        private static bool SetFConti(FamilyStructure.Foundation_Continuous footing, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == footing.SecId) { footing.SecId = set; }
            if (dat == footing.name) { footing.name = set; }
            if (dat == footing.strength_concrete) { footing.strength_concrete = set; }
            if (dat == footing.depth_cover_top) { footing.depth_cover_top = set; }
            if (dat == footing.depth_cover_bottom) { footing.depth_cover_bottom = set; }
            if (dat == footing.depth_cover_side) { footing.depth_cover_side = set; }
            if (dat == footing.B) { footing.B = set; }
            if (dat == footing.t_B) { footing.t_B = set; }
            if (dat == footing.depth_base) { footing.depth_base = set; }
            if (dat == footing.depth_tip) { footing.depth_tip = set; }
            if (dat == footing.type_right) { footing.type_right = set; } //マッピングテーブルなし
            if (dat == footing.type_left) { footing.type_left = set; }   //マッピングテーブルなし
            //StringSet(footing.strength, dat, set);
            if (dat == footing.strength) { footing.strength = set; }
            StringSet(footing.D, dat, set);
            StringSet(footing.count, dat, set);
            StringSet(footing.pitch, dat, set);
            if (dat == footing.type) { footing.type = set; }

            //インスタンスパラメータ
            if (dat == footing.MemId) { footing.MemId = set; }
            if (dat == footing.NameMembers) { footing.NameMembers = set; }
            if (dat == footing.length_ex_start) { footing.length_ex_start = set; }
            if (dat == footing.length_ex_end) { footing.length_ex_end = set; }

            return ret;
        }
        private static bool SetCastinpile(FamilyStructure.Pile pile, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == pile.strength_concrete) { pile.strength_concrete = set; }
            if (dat == pile.length_all) { pile.length_all = set; }
            if (dat == pile.length_head) { pile.length_head = set; }
            if (dat == pile.length_foot_Revit) { pile.length_foot_Revit = set; } //マッピングテーブルなし
            if (dat == pile.D) { pile.D = set; }
            if (dat == pile.D_extended_foot) { pile.D_extended_foot = set; }
            if (dat == pile.D_extended_top) { pile.D_extended_top = set; }
            if (dat == pile.name) { pile.name = set; }
            StringSet(pile.D_main_circumference_1st, dat, set);
            StringSet(pile.count_main_circumference_1st, dat, set);
            StringSet(pile.D_main_core, dat, set);
            StringSet(pile.count_main_core, dat, set);
            StringSet(pile.D_band, dat, set);
            StringSet(pile.pitch_band, dat, set);
            if (dat == pile.strength_main_circumference_1st) { pile.strength_main_circumference_1st = set; }
            if (dat == pile.strength_main_core) { pile.strength_main_core = set; }
            if (dat == pile.strength_band) { pile.strength_band = set; }
            if (dat == pile.depth_cover) { pile.depth_cover = set; }
            if (dat == pile.depth_cover_top) { pile.depth_cover_top = set; }
            if (dat == pile.SecId) { pile.SecId = set; }

            //インスタンスパラメータ
            if (dat == pile.kind_structure) { pile.kind_structure = set; } //マッピングテーブルなし
            if (dat == pile.MemId) { pile.MemId = set; }
            if (dat == pile.NameMembers) { pile.NameMembers = set; }


            return ret;
        }

        private static bool SetPrecastpile(FamilyStructure.Pile_2 pile, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == pile.strength_concrete) { pile.strength_concrete = set; }
            if (dat == pile.straight_D) { pile.straight_D = set; }
            if (dat == pile.straight_length) { pile.straight_length = set; }
            if (dat == pile.ef_D_axial) { pile.ef_D_axial = set; }
            if (dat == pile.ef_D_extended_foot) { pile.ef_D_extended_foot = set; }
            if (dat == pile.ef_length_axial) { pile.ef_length_axial = set; }
            if (dat == pile.ef_length_foot) { pile.ef_length_foot = set; }
            if (dat == pile.et_D_extended_top) { pile.et_D_extended_top = set; }
            if (dat == pile.et_D_axial) { pile.et_D_axial = set; }
            if (dat == pile.et_length_head) { pile.et_length_head = set; }
            if (dat == pile.et_length_axial) { pile.et_length_axial = set; }
            if (dat == pile.etf_D_extended_top) { pile.etf_D_extended_top = set; }
            if (dat == pile.etf_D_axial) { pile.etf_D_axial = set; }
            if (dat == pile.etf_D_extended_foot) { pile.etf_D_extended_foot = set; }
            if (dat == pile.etf_length_head) { pile.etf_length_head = set; }
            if (dat == pile.etf_length_axial) { pile.etf_length_axial = set; }
            if (dat == pile.etf_length_foot) { pile.etf_length_foot = set; }
            if (dat == pile.name) { pile.name = set; }
            StringSet(pile.D_main_circumference_1st, dat, set);
            StringSet(pile.count_main_circumference_1st, dat, set);
            StringSet(pile.D_main_core, dat, set);
            StringSet(pile.count_main_core, dat, set);
            StringSet(pile.D_band, dat, set);
            StringSet(pile.pitch_band, dat, set);
            if (dat == pile.strength_main_circumference_1st) { pile.strength_main_circumference_1st = set; }
            if (dat == pile.strength_main_core) { pile.strength_main_core = set; }
            if (dat == pile.strength_band) { pile.strength_band = set; }
            if (dat == pile.depth_cover) { pile.depth_cover = set; }
            if (dat == pile.depth_cover_top) { pile.depth_cover_top = set; }
            if (dat == pile.SecId) { pile.SecId = set; }

            //インスタンスパラメータ
            if (dat == pile.kind_structure) { pile.kind_structure = set; } //マッピングテーブルなし
            if (dat == pile.MemId) { pile.MemId = set; }
            if (dat == pile.NameMembers) { pile.NameMembers = set; }


            return ret;
        }

        private static bool SetPile(FamilyStructure.Pile_S pile, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == pile.name) { pile.name = set; }
            //else if (dat == pile.id_order) { pile.id_order = set; }
            else if (dat == pile.length_pile) { pile.length_pile = set; }
            else if (dat == pile.D) { pile.D = set; }
            else if (dat == pile.t) { pile.t = set; }
            else if (dat == pile.strength) { pile.strength = set; }
            else if (dat == pile.SecId) { pile.SecId = set; }

            //インスタンスパラメータ
            else if (dat == pile.MemId) { pile.MemId = set; }
            else if (dat == pile.NameMembers) { pile.NameMembers = set; }
            else if (dat == pile.length_all) { pile.length_all = set; }

            return ret;
        }

        private static bool SetPile(FamilyStructure.Pile_PHC pile, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == pile.name) { pile.name = set; }
            //else if (dat == pile.id_order) { pile.id_order = set; }
            else if (dat == pile.kind) { pile.kind = set; }
            else if (dat == pile.length_pile) { pile.length_pile = set; }
            else if (dat == pile.D) { pile.D = set; }
            else if (dat == pile.t) { pile.t = set; }
            else if (dat == pile.strength_concrete) { pile.strength_concrete = set; }
            else if (dat == pile.D_PC) { pile.D_PC = set; }
            else if (dat == pile.N_PC) { pile.N_PC = set; }
            else if (dat == pile.strength_PC) { pile.strength_PC = set; }
            else if (dat == pile.SecId) { pile.SecId = set; }

            //インスタンスパラメータ
            else if (dat == pile.MemId) { pile.MemId = set; }
            else if (dat == pile.NameMembers) { pile.NameMembers = set; }
            else if (dat == pile.length_all) { pile.length_all = set; }

            return ret;
        }

        private static bool SetPile(FamilyStructure.Pile_ST pile, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == pile.name) { pile.name = set; }
            //else if (dat == pile.id_order) { pile.id_order = set; }
            else if (dat == pile.kind) { pile.kind = set; }
            else if (dat == pile.length_pile) { pile.length_pile = set; }
            else if (dat == pile.D1) { pile.D1 = set; }
            else if (dat == pile.D2) { pile.D2 = set; }
            else if (dat == pile.t1) { pile.t1 = set; }
            else if (dat == pile.t2) { pile.t2 = set; }
            else if (dat == pile.strength_concrete) { pile.strength_concrete = set; }
            else if (dat == pile.D_PC) { pile.D_PC = set; }
            else if (dat == pile.N_PC) { pile.N_PC = set; }
            else if (dat == pile.strength_PC) { pile.strength_PC = set; }
            else if (dat == pile.SecId) { pile.SecId = set; }

            //インスタンスパラメータ
            else if (dat == pile.MemId) { pile.MemId = set; }
            else if (dat == pile.NameMembers) { pile.NameMembers = set; }
            else if (dat == pile.length_all) { pile.length_all = set; }

            return ret;
        }

        private static bool SetPile(FamilyStructure.Pile_SC pile, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == pile.name) { pile.name = set; }
            //else if (dat == pile.id_order) { pile.id_order = set; }
            else if (dat == pile.kind) { pile.kind = set; }
            else if (dat == pile.length_pile) { pile.length_pile = set; }
            else if (dat == pile.D) { pile.D = set; }
            else if (dat == pile.tc) { pile.tc = set; }
            else if (dat == pile.ts) { pile.ts = set; }
            else if (dat == pile.strength_concrete) { pile.strength_concrete = set; }
            else if (dat == pile.strength_pipe) { pile.strength_pipe = set; }
            else if (dat == pile.SecId) { pile.SecId = set; }

            //インスタンスパラメータ
            else if (dat == pile.MemId) { pile.MemId = set; }
            else if (dat == pile.NameMembers) { pile.NameMembers = set; }
            else if (dat == pile.length_all) { pile.length_all = set; }

            return ret;
        }

        private static bool SetPile(FamilyStructure.Pile_PRC pile, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == pile.name) { pile.name = set; }
            //else if (dat == pile.id_order) { pile.id_order = set; }
            else if (dat == pile.kind) { pile.kind = set; }
            else if (dat == pile.length_pile) { pile.length_pile = set; }
            else if (dat == pile.D) { pile.D = set; }
            else if (dat == pile.tc) { pile.tc = set; }
            else if (dat == pile.strength_concrete) { pile.strength_concrete = set; }
            else if (dat == pile.D_PC) { pile.D_PC = set; }
            else if (dat == pile.N_PC) { pile.N_PC = set; }
            else if (dat == pile.strength_PC) { pile.strength_PC = set; }
            else if (dat == pile.D_bar) { pile.D_bar = set; }
            else if (dat == pile.N_bar) { pile.N_bar = set; }
            else if (dat == pile.strength_bar) { pile.strength_bar = set; }
            else if (dat == pile.SecId) { pile.SecId = set; }

            //インスタンスパラメータ
            else if (dat == pile.MemId) { pile.MemId = set; }
            else if (dat == pile.NameMembers) { pile.NameMembers = set; }
            else if (dat == pile.length_all) { pile.length_all = set; }

            return ret;
        }

        private static bool SetPile(FamilyStructure.Pile_CPRC pile, string dat, string set)
        {
            bool ret = true;

            //タイプパラメータ
            if (dat == pile.name) { pile.name = set; }
            //else if (dat == pile.id_order) { pile.id_order = set; }
            else if (dat == pile.kind) { pile.kind = set; }
            else if (dat == pile.length_pile) { pile.length_pile = set; }
            else if (dat == pile.D) { pile.D = set; }
            else if (dat == pile.tc) { pile.tc = set; }
            else if (dat == pile.strength_concrete) { pile.strength_concrete = set; }
            else if (dat == pile.D_PC) { pile.D_PC = set; }
            else if (dat == pile.N_PC) { pile.N_PC = set; }
            else if (dat == pile.strength_PC) { pile.strength_PC = set; }
            else if (dat == pile.D_bar) { pile.D_bar = set; }
            else if (dat == pile.N_bar) { pile.N_bar = set; }
            else if (dat == pile.strength_bar) { pile.strength_bar = set; }
            else if (dat == pile.SecId) { pile.SecId = set; }

            //インスタンスパラメータ
            else if (dat == pile.MemId) { pile.MemId = set; }
            else if (dat == pile.NameMembers) { pile.NameMembers = set; }
            else if (dat == pile.length_all) { pile.length_all = set; }

            return ret;
        }

        #endregion
        private static bool StringSet(string[] st, string dat, string set)
        {
            bool ret = true;
            try
            {
                for(int i = 0; i < st.Length; i++)
                {
                    if (dat == st[i]) { st[i] = set; }
                }
            }
            catch(Exception)
            {
                ret = false;                
            }
            return ret;
        }
        #endregion

        #region SLMテーブル
        private static void CreateSLMTableFile()
        {
            const string Ver = RevitLNK.RevitVersion;
            const string SLM = "\\SLM" + Ver + "\\テーブル\\";
            const string TableFileName = "Setting for STBLink.stb";

            string mydoc = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string tblFolder = mydoc + SLM;
            if (Directory.Exists(tblFolder) == true)
            {
                //SLM用のテーブルフォルダがある場合にSS3Link対応テーブルを作る
                string data = "";
                string cate = "";

                #region 共通パラメータ
                cate = "共通パラメータ";

                data += SLMParameter(cate, "符号");
                //SLM側でレベルから自動取得するためブランク
                data += SLMParameter(cate, "階1");
                data += SLMParameter(cate, "階2");

                data += "\r\n";

                #endregion
                #region 梁パラメータ
                cate = "梁パラメータ";

                data += SLMParameter(cate, "左上端主筋径(太径)");
                data += SLMParameter(cate, "左上端主筋径(細径)");
                data += SLMParameter(cate, "左上端1段筋本数(太径)");
                data += SLMParameter(cate, "左上端2段筋本数(太径)");
                data += SLMParameter(cate, "左上端3段筋本数(太径)");
                data += SLMParameter(cate, "左上端1段筋本数(細径)");
                data += SLMParameter(cate, "左上端2段筋本数(細径)");
                data += SLMParameter(cate, "左上端3段筋本数(細径)");
                data += SLMParameter(cate, "左下端主筋径(太径)");
                data += SLMParameter(cate, "左下端主筋径(細径)");
                data += SLMParameter(cate, "左下端1段筋本数(太径)");
                data += SLMParameter(cate, "左下端2段筋本数(太径)");
                data += SLMParameter(cate, "左下端3段筋本数(太径)");
                data += SLMParameter(cate, "左下端1段筋本数(細径)");
                data += SLMParameter(cate, "左下端2段筋本数(細径)");
                data += SLMParameter(cate, "左下端3段筋本数(細径)");
                data += SLMParameter(cate, "左スターラップ径");
                data += SLMParameter(cate, "左スターラップ本数");
                data += SLMParameter(cate, "左スターラップ間隔");
                data += SLMParameter(cate, "左スターラップ記号");

                data += SLMParameter(cate, "中央上端主筋径(太径)");
                data += SLMParameter(cate, "中央上端主筋径(細径)");
                data += SLMParameter(cate, "中央上端1段筋本数(太径)");
                data += SLMParameter(cate, "中央上端2段筋本数(太径)");
                data += SLMParameter(cate, "中央上端3段筋本数(太径)");
                data += SLMParameter(cate, "中央上端1段筋本数(細径)");
                data += SLMParameter(cate, "中央上端2段筋本数(細径)");
                data += SLMParameter(cate, "中央上端3段筋本数(細径)");
                data += SLMParameter(cate, "中央下端主筋径(太径)");
                data += SLMParameter(cate, "中央下端主筋径(細径)");
                data += SLMParameter(cate, "中央下端1段筋本数(太径)");
                data += SLMParameter(cate, "中央下端2段筋本数(太径)");
                data += SLMParameter(cate, "中央下端3段筋本数(太径)");
                data += SLMParameter(cate, "中央下端1段筋本数(細径)");
                data += SLMParameter(cate, "中央下端2段筋本数(細径)");
                data += SLMParameter(cate, "中央下端3段筋本数(細径)");
                data += SLMParameter(cate, "中央スターラップ径");
                data += SLMParameter(cate, "中央スターラップ本数");
                data += SLMParameter(cate, "中央スターラップ間隔");
                data += SLMParameter(cate, "中央スターラップ記号");

                data += SLMParameter(cate, "右上端主筋径(太径)");
                data += SLMParameter(cate, "右上端主筋径(細径)");
                data += SLMParameter(cate, "右上端1段筋本数(太径)");
                data += SLMParameter(cate, "右上端2段筋本数(太径)");
                data += SLMParameter(cate, "右上端3段筋本数(太径)");
                data += SLMParameter(cate, "右上端1段筋本数(細径)");
                data += SLMParameter(cate, "右上端2段筋本数(細径)");
                data += SLMParameter(cate, "右上端3段筋本数(細径)");
                data += SLMParameter(cate, "右下端主筋径(太径)");
                data += SLMParameter(cate, "右下端主筋径(細径)");
                data += SLMParameter(cate, "右下端1段筋本数(太径)");
                data += SLMParameter(cate, "右下端2段筋本数(太径)");
                data += SLMParameter(cate, "右下端3段筋本数(太径)");
                data += SLMParameter(cate, "右下端1段筋本数(細径)");
                data += SLMParameter(cate, "右下端2段筋本数(細径)");
                data += SLMParameter(cate, "右下端3段筋本数(細径)");
                data += SLMParameter(cate, "右スターラップ径");
                data += SLMParameter(cate, "右スターラップ本数");
                data += SLMParameter(cate, "右スターラップ間隔");
                data += SLMParameter(cate, "右スターラップ記号");

                data += SLMParameter(cate, "全断面b");
                data += SLMParameter(cate, "全断面D");
                data += SLMParameter(cate, "左b");
                data += SLMParameter(cate, "左D");
                data += SLMParameter(cate, "中央b");
                data += SLMParameter(cate, "中央D");
                data += SLMParameter(cate, "右b");
                data += SLMParameter(cate, "右D");
                data += SLMParameter(cate, "全断面A");
                data += SLMParameter(cate, "全断面B");
                data += SLMParameter(cate, "全断面t1");
                data += SLMParameter(cate, "全断面t2");
                data += SLMParameter(cate, "全断面形状");
                data += SLMParameter(cate, "全断面鉄骨材種");
                data += SLMParameter(cate, "左A");
                data += SLMParameter(cate, "左B");
                data += SLMParameter(cate, "左t1");
                data += SLMParameter(cate, "左t2");
                data += SLMParameter(cate, "左形状");
                data += SLMParameter(cate, "左鉄骨材種");
                data += SLMParameter(cate, "中央A");
                data += SLMParameter(cate, "中央B");
                data += SLMParameter(cate, "中央t1");
                data += SLMParameter(cate, "中央t2");
                data += SLMParameter(cate, "中央形状");
                data += SLMParameter(cate, "中央鉄骨材種");
                data += SLMParameter(cate, "右A");
                data += SLMParameter(cate, "右B");
                data += SLMParameter(cate, "右t1");
                data += SLMParameter(cate, "右t2");
                data += SLMParameter(cate, "右形状");
                data += SLMParameter(cate, "右鉄骨材種");

                data += SLMParameter(cate, "R");
                data += SLMParameter(cate, "Kt");

                data += "\r\n";

                #endregion
                #region 柱パラメータ
                cate = "柱パラメータ";

                data += SLMParameter(cate, "配筋タイプy");
                data += SLMParameter(cate, "配筋タイプz");
                data += SLMParameter(cate, "柱頭Dy");
                data += SLMParameter(cate, "柱頭Dz");
                data += SLMParameter(cate, "柱頭主筋径(太径)");
                data += SLMParameter(cate, "柱頭主筋径(細径)");
                data += SLMParameter(cate, "柱頭主筋ny本数(一段筋太径)");
                data += SLMParameter(cate, "柱頭主筋ny本数(一段筋細径)");
                data += SLMParameter(cate, "柱頭主筋ny本数(二段筋太径)");
                data += SLMParameter(cate, "柱頭主筋ny本数(二段筋細径)");
                data += SLMParameter(cate, "柱頭主筋nz本数(一段筋太径)");
                data += SLMParameter(cate, "柱頭主筋nz本数(一段筋細径)");
                data += SLMParameter(cate, "柱頭主筋nz本数(二段筋太径)");
                data += SLMParameter(cate, "柱頭主筋nz本数(二段筋細径)");
                data += SLMParameter(cate, "柱頭帯筋径");
                data += SLMParameter(cate, "柱頭帯筋ny本数");
                data += SLMParameter(cate, "柱頭帯筋nz本数");
                data += SLMParameter(cate, "柱頭帯筋ピッチ");
                data += SLMParameter(cate, "柱頭帯筋記号");
                data += SLMParameter(cate, "柱頭鉄骨材種");
                data += SLMParameter(cate, "柱頭ZA");
                data += SLMParameter(cate, "柱頭ZB");
                data += SLMParameter(cate, "柱頭Zt1");
                data += SLMParameter(cate, "柱頭Zt2");
                data += SLMParameter(cate, "柱頭Z形状");
                data += SLMParameter(cate, "柱頭YA");
                data += SLMParameter(cate, "柱頭YB");
                data += SLMParameter(cate, "柱頭Yt1");
                data += SLMParameter(cate, "柱頭Yt2");
                data += SLMParameter(cate, "柱頭Y形状");

                data += SLMParameter(cate, "柱脚Dy");
                data += SLMParameter(cate, "柱脚Dz");
                data += SLMParameter(cate, "柱脚主筋径(太径)");
                data += SLMParameter(cate, "柱脚主筋径(細径)");
                data += SLMParameter(cate, "柱脚主筋ny本数(一段筋太径)");
                data += SLMParameter(cate, "柱脚主筋ny本数(一段筋細径)");
                data += SLMParameter(cate, "柱脚主筋ny本数(二段筋太径)");
                data += SLMParameter(cate, "柱脚主筋ny本数(二段筋細径)");
                data += SLMParameter(cate, "柱脚主筋nz本数(一段筋太径)");
                data += SLMParameter(cate, "柱脚主筋nz本数(一段筋細径)");
                data += SLMParameter(cate, "柱脚主筋nz本数(二段筋太径)");
                data += SLMParameter(cate, "柱脚主筋nz本数(二段筋細径)");
                data += SLMParameter(cate, "柱脚帯筋径");
                data += SLMParameter(cate, "柱脚帯筋ny本数");
                data += SLMParameter(cate, "柱脚帯筋nz本数");
                data += SLMParameter(cate, "柱脚帯筋ピッチ");
                data += SLMParameter(cate, "柱脚帯筋記号");
                data += SLMParameter(cate, "柱脚鉄骨材種");
                data += SLMParameter(cate, "柱脚ZA");
                data += SLMParameter(cate, "柱脚ZB");
                data += SLMParameter(cate, "柱脚Zt1");
                data += SLMParameter(cate, "柱脚Zt2");
                data += SLMParameter(cate, "柱脚Z形状");
                data += SLMParameter(cate, "柱脚YA");
                data += SLMParameter(cate, "柱脚YB");
                data += SLMParameter(cate, "柱脚Yt1");
                data += SLMParameter(cate, "柱脚Yt2");
                data += SLMParameter(cate, "柱脚Y形状");

                data += SLMParameter(cate, "Ky");
                data += SLMParameter(cate, "Ry");
                data += SLMParameter(cate, "Kz");
                data += SLMParameter(cate, "Rz");
                data += SLMParameter(cate, "中詰コンクリート材種");
                data += SLMParameter(cate, "補助筋本数nz");
                data += SLMParameter(cate, "補助筋本数ny");
                data += SLMParameter(cate, "補助筋径nz");
                data += SLMParameter(cate, "補助筋径ny");

                data += "\r\n";

                #endregion
                #region 床パラメータ
                cate = "床パラメータ";

                data += SLMParameter(cate, "厚さ");
                data += SLMParameter(cate, "デッキPLせい");
                data += SLMParameter(cate, "PL厚");
                data += SLMParameter(cate, "溝の幅");

                data += SLMParameter(cate, "上_短辺_A端部_D1");
                data += SLMParameter(cate, "上_短辺_A端部_D2");
                data += SLMParameter(cate, "上_短辺_A端部_PITCH");
                data += SLMParameter(cate, "上_短辺_A中央_D1");
                data += SLMParameter(cate, "上_短辺_A中央_D2");
                data += SLMParameter(cate, "上_短辺_A中央_PITCH");
                data += SLMParameter(cate, "上_短辺_B端中_D1");
                data += SLMParameter(cate, "上_短辺_B端中_D2");
                data += SLMParameter(cate, "上_短辺_B端中_PITCH");
                data += SLMParameter(cate, "上_長辺_A端部_D1");
                data += SLMParameter(cate, "上_長辺_A端部_D2");
                data += SLMParameter(cate, "上_長辺_A端部_PITCH");
                data += SLMParameter(cate, "上_長辺_A中央_D1");
                data += SLMParameter(cate, "上_長辺_A中央_D2");
                data += SLMParameter(cate, "上_長辺_A中央_PITCH");
                data += SLMParameter(cate, "上_長辺_B端中_D1");
                data += SLMParameter(cate, "上_長辺_B端中_D2");
                data += SLMParameter(cate, "上_長辺_B端中_PITCH");

                data += SLMParameter(cate, "下_短辺_A端部_D1");
                data += SLMParameter(cate, "下_短辺_A端部_D2");
                data += SLMParameter(cate, "下_短辺_A端部_PITCH");
                data += SLMParameter(cate, "下_短辺_A中央_D1");
                data += SLMParameter(cate, "下_短辺_A中央_D2");
                data += SLMParameter(cate, "下_短辺_A中央_PITCH");
                data += SLMParameter(cate, "下_短辺_B端中_D1");
                data += SLMParameter(cate, "下_短辺_B端中_D2");
                data += SLMParameter(cate, "下_短辺_B端中_PITCH");
                data += SLMParameter(cate, "下_長辺_A端部_D1");
                data += SLMParameter(cate, "下_長辺_A端部_D2");
                data += SLMParameter(cate, "下_長辺_A端部_PITCH");
                data += SLMParameter(cate, "下_長辺_A中央_D1");
                data += SLMParameter(cate, "下_長辺_A中央_D2");
                data += SLMParameter(cate, "下_長辺_A中央_PITCH");
                data += SLMParameter(cate, "下_長辺_B端中_D1");
                data += SLMParameter(cate, "下_長辺_B端中_D2");
                data += SLMParameter(cate, "下_長辺_B端中_PITCH");

                data += "\r\n";

                #endregion
                #region 壁パラメータ
                cate = "壁パラメータ";

                data += SLMParameter(cate, "コンクリート材種");
                data += SLMParameter(cate, "配筋タイプ");
                data += SLMParameter(cate, "厚さ");
                data += SLMParameter(cate, "地下外壁フラグ");

                data += SLMParameter(cate, "タテ筋D1");
                data += SLMParameter(cate, "タテ筋D2");
                data += SLMParameter(cate, "タテ筋Pitch");
                data += SLMParameter(cate, "ヨコ筋D1");
                data += SLMParameter(cate, "ヨコ筋D2");
                data += SLMParameter(cate, "ヨコ筋Pitch");

                data += SLMParameter(cate, "開口補強Aタテ本数");
                data += SLMParameter(cate, "開口補強AタテD");
                data += SLMParameter(cate, "開口補強Aヨコ本数");
                data += SLMParameter(cate, "開口補強AヨコD");
                data += SLMParameter(cate, "開口補強A斜メ本数");
                data += SLMParameter(cate, "開口補強A斜メD");
                data += SLMParameter(cate, "開口補強Bタテ本数");
                data += SLMParameter(cate, "開口補強BタテD");
                data += SLMParameter(cate, "開口補強Bヨコ本数");
                data += SLMParameter(cate, "開口補強BヨコD");
                data += SLMParameter(cate, "開口補強B斜メ本数");
                data += SLMParameter(cate, "開口補強B斜メD");

                data += SLMParameter(cate, "外タテ筋上D1");
                data += SLMParameter(cate, "外タテ筋上D2");
                data += SLMParameter(cate, "外タテ筋上Pitch");
                data += SLMParameter(cate, "外タテ筋中D1");
                data += SLMParameter(cate, "外タテ筋中D2");
                data += SLMParameter(cate, "外タテ筋中Pitch");
                data += SLMParameter(cate, "外タテ筋下D1");
                data += SLMParameter(cate, "外タテ筋下D2");
                data += SLMParameter(cate, "外タテ筋下Pitch");
                data += SLMParameter(cate, "外ヨコ筋端D1");
                data += SLMParameter(cate, "外ヨコ筋端D2");
                data += SLMParameter(cate, "外ヨコ筋端Pitch");
                data += SLMParameter(cate, "外ヨコ筋端D1_2");
                data += SLMParameter(cate, "外ヨコ筋端D2_2");
                data += SLMParameter(cate, "外ヨコ筋端Pitch_2");
                data += SLMParameter(cate, "外ヨコ筋中D1");
                data += SLMParameter(cate, "外ヨコ筋中D2");
                data += SLMParameter(cate, "外ヨコ筋中Pitch");

                data += SLMParameter(cate, "内タテ筋上D1");
                data += SLMParameter(cate, "内タテ筋上D2");
                data += SLMParameter(cate, "内タテ筋上Pitch");
                data += SLMParameter(cate, "内タテ筋中D1");
                data += SLMParameter(cate, "内タテ筋中D2");
                data += SLMParameter(cate, "内タテ筋中Pitch");
                data += SLMParameter(cate, "内タテ筋下D1");
                data += SLMParameter(cate, "内タテ筋下D2");
                data += SLMParameter(cate, "内タテ筋下Pitch");
                data += SLMParameter(cate, "内ヨコ筋端D1");
                data += SLMParameter(cate, "内ヨコ筋端D2");
                data += SLMParameter(cate, "内ヨコ筋端Pitch");
                data += SLMParameter(cate, "内ヨコ筋端D1_2");
                data += SLMParameter(cate, "内ヨコ筋端D2_2");
                data += SLMParameter(cate, "内ヨコ筋端Pitch_2");
                data += SLMParameter(cate, "内ヨコ筋中D1");
                data += SLMParameter(cate, "内ヨコ筋中D2");
                data += SLMParameter(cate, "内ヨコ筋中Pitch");

                data += "\r\n";

                #endregion


                data += "\r\n";


                #region ファミリ

                data += SLMFamily("RC梁");
                data += SLMFamily("RC梁_ハンチ");

                data += SLMFamily("S梁_H");
                data += SLMFamily("S梁_H_ハンチ");
                data += SLMFamily("S梁_BX");
                data += SLMFamily("S梁_P");
                data += SLMFamily("S梁_T");
                data += SLMFamily("S梁_L");
                data += SLMFamily("S梁_FB");
                data += SLMFamily("S梁_C");

                data += SLMFamily("SRC梁_H");

                data += SLMFamily("RC柱_角");
                data += SLMFamily("RC柱_丸");

                data += SLMFamily("SRC柱_角_Hy");
                data += SLMFamily("SRC柱_角_Hz");
                data += SLMFamily("SRC柱_角_BX");
                data += SLMFamily("SRC柱_角_P");
                data += SLMFamily("SRC柱_角_HH");
                data += SLMFamily("SRC柱_角_HT");
                data += SLMFamily("SRC柱_角_HT2");
                data += SLMFamily("SRC柱_角_THL");
                data += SLMFamily("SRC柱_角_THR");

                data += SLMFamily("SRC柱_丸_Hy");
                data += SLMFamily("SRC柱_丸_Hz");
                data += SLMFamily("SRC柱_丸_BX");
                data += SLMFamily("SRC柱_丸_P");
                data += SLMFamily("SRC柱_丸_HH");
                data += SLMFamily("SRC柱_丸_HT");
                data += SLMFamily("SRC柱_丸_HT2");
                data += SLMFamily("SRC柱_丸_THL");
                data += SLMFamily("SRC柱_丸_THR");

                data += SLMFamily("S柱_Hy");
                data += SLMFamily("S柱_Hz");
                data += SLMFamily("S柱_BX");
                data += SLMFamily("S柱_BX_z");
                data += SLMFamily("S柱_P");
                data += SLMFamily("S柱_HH");
                data += SLMFamily("S柱_HT");
                data += SLMFamily("S柱_HT2");
                data += SLMFamily("S柱_THL");
                data += SLMFamily("S柱_THR");

                data += "\r\n";

                #endregion


                try
                {
                    if (File.Exists(tblFolder + TableFileName) == true)
                    {
                        File.Delete(tblFolder + TableFileName);
                    }
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    File.WriteAllText(tblFolder + TableFileName, data, Encoding.GetEncoding("Shift_JIS"));
                }
                catch
                {
                }


            }

        }

        private static string SLMParameter(string cate, string name)
        {
            const string Kugiri = "："; //全角コロン
            const string Kugiri2 = ","; //半角カンマ

            string p = cate + Kugiri + name + Kugiri;

            switch (cate)
            {
                case "共通パラメータ":
                    #region 共通パラメータ
                    switch (name)
                    {
                        case "符号":
                            p += RCClmRe.name + Kugiri2;
                            p += RCClmRo.name + Kugiri2;
                            p += SRCClmH.name + Kugiri2;
                            p += SRCClmH_Rou.name + Kugiri2;
                            p += SRCClmCross.name + Kugiri2;
                            p += SRCClmCross_Rou.name + Kugiri2;
                            p += SRCClmT.name + Kugiri2;
                            p += SRCClmT_Rou.name + Kugiri2;
                            p += CFTClmBox.name + Kugiri2;
                            p += CFTClmPipe.name + Kugiri2;
                            p += RCGir.name + Kugiri2;
                            p += RCCGir.name + Kugiri2;
                            p += SGirBH.name + Kugiri2;
                            p += SGirH.name + Kugiri2;
                            p += SGirC.name + Kugiri2;
                            p += SGirL.name + Kugiri2;
                            p += SGirLipC.name + Kugiri2;
                            p += SCGirH.name + Kugiri2;
                            p += SRCGirH.name + Kugiri2;
                            p += SBraBBox.name + Kugiri2;
                            p += SBraBox.name + Kugiri2;
                            p += SBraPipe.name + Kugiri2;
                            p += SBraFB.name + Kugiri2;
                            p += SBraRollBar.name + Kugiri2;
                            p += FRect.name + Kugiri2;
                            p += FTRect.name + Kugiri2;
                            p += FTri.name + Kugiri2;
                            p += FETriangle.name + Kugiri2;
                            p += FOct.name + Kugiri2;
                            p += FConti.name + Kugiri2;
                            p += CastinPile.name + Kugiri2;
                            p += PrecastPile.name;
                            break;
                    }
                    break;
                #endregion

                case "梁パラメータ":
                    #region 梁パラメータ
                    switch (name)
                    {
                        #region 主筋

                        case "左上端主筋径(太径)":
                            p += RCGir.D_reinforcement_main_top[0] + Kugiri2;
                            p += RCCGir.D_reinforcement_main_top[0] + Kugiri2;
                            p += SRCGirH.D_reinforcement_main_top[0] + Kugiri2;
                            p += SRCCGirH.D_reinforcement_main_top[0] + Kugiri2;                         
                            break;
                        case "左上端主筋径(細径)":
                            p += RCGir.D_reinforcement_2nd_main_top[0] + Kugiri2;
                            p += RCCGir.D_reinforcement_2nd_main_top[0] + Kugiri2;
                            p += SRCGirH.D_reinforcement_2nd_main_top[0] + Kugiri2;
                            p += SRCCGirH.D_reinforcement_2nd_main_top[0] + Kugiri2;
                            break;
                        case "左上端1段筋本数(太径)":
                            p += RCGir.count_main_top_1st[0] + Kugiri2;
                            p += RCCGir.count_main_top_1st[0] + Kugiri2;
                            p += SRCGirH.count_main_top_1st[0] + Kugiri2;
                            p += SRCCGirH.count_main_top_1st[0] + Kugiri2;
                            break;
                        case "左上端2段筋本数(太径)":
                            p += RCGir.count_main_top_2nd[0] + Kugiri2;
                            p += RCCGir.count_main_top_2nd[0] + Kugiri2;
                            p += SRCGirH.count_main_top_2nd[0] + Kugiri2;
                            p += SRCCGirH.count_main_top_2nd[0] + Kugiri2;
                            break;
                        case "左上端3段筋本数(太径)":
                            p += RCGir.count_main_top_3rd[0] + Kugiri2;
                            p += RCCGir.count_main_top_3rd[0] + Kugiri2;
                            p += SRCGirH.count_main_top_3rd[0] + Kugiri2;
                            p += SRCCGirH.count_main_top_3rd[0] + Kugiri2;
                            break;
                        case "左上端1段筋本数(細径)":
                            p += RCGir.count_2nd_main_top_1st[0] + Kugiri2;
                            p += RCCGir.count_2nd_main_top_1st[0] + Kugiri2;
                            p += SRCGirH.count_2nd_main_top_1st[0] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_top_1st[0] + Kugiri2;
                            break;
                        case "左上端2段筋本数(細径)":
                            p += RCGir.count_2nd_main_top_2nd[0] + Kugiri2;
                            p += RCCGir.count_2nd_main_top_2nd[0] + Kugiri2;
                            p += SRCGirH.count_2nd_main_top_2nd[0] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_top_2nd[0] + Kugiri2;
                            break;
                        case "左上端3段筋本数(細径)":
                            p += RCGir.count_2nd_main_top_3rd[0] + Kugiri2;
                            p += RCCGir.count_2nd_main_top_3rd[0] + Kugiri2;
                            p += SRCGirH.count_2nd_main_top_3rd[0] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_top_3rd[0] + Kugiri2;
                            break;

                        case "左下端主筋径(太径)":
                            p += RCGir.D_reinforcement_main_bottom[0] + Kugiri2;
                            p += RCCGir.D_reinforcement_main_bottom[0] + Kugiri2;
                            p += SRCGirH.D_reinforcement_main_bottom[0] + Kugiri2;
                            p += SRCCGirH.D_reinforcement_main_bottom[0] + Kugiri2;
                            break;
                        case "左下端主筋径(細径)":
                            p += RCGir.D_reinforcement_2nd_main_bottom[0] + Kugiri2;
                            p += RCCGir.D_reinforcement_2nd_main_bottom[0] + Kugiri2;
                            p += SRCGirH.D_reinforcement_2nd_main_bottom[0] + Kugiri2;
                            p += SRCCGirH.D_reinforcement_2nd_main_bottom[0] + Kugiri2;
                            break;
                        case "左下端1段筋本数(太径)":
                            p += RCGir.count_main_bottom_1st[0] + Kugiri2;
                            p += RCCGir.count_main_bottom_1st[0] + Kugiri2;
                            p += SRCGirH.count_main_bottom_1st[0] + Kugiri2;
                            p += SRCCGirH.count_main_bottom_1st[0] + Kugiri2;
                            break;
                        case "左下端2段筋本数(太径)":
                            p += RCGir.count_main_bottom_2nd[0] + Kugiri2;
                            p += RCCGir.count_main_bottom_2nd[0] + Kugiri2;
                            p += SRCGirH.count_main_bottom_2nd[0] + Kugiri2;
                            p += SRCCGirH.count_main_bottom_2nd[0] + Kugiri2;
                            break;
                        case "左下端3段筋本数(太径)":
                            p += RCGir.count_main_bottom_3rd[0] + Kugiri2;
                            p += RCCGir.count_main_bottom_3rd[0] + Kugiri2;
                            p += SRCGirH.count_main_bottom_3rd[0] + Kugiri2;
                            p += SRCCGirH.count_main_bottom_3rd[0] + Kugiri2;
                            break;
                        case "左下端1段筋本数(細径)":
                            p += RCGir.count_2nd_main_bottom_1st[0] + Kugiri2;
                            p += RCCGir.count_2nd_main_bottom_1st[0] + Kugiri2;
                            p += SRCGirH.count_2nd_main_bottom_1st[0] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_bottom_1st[0] + Kugiri2;
                            break;
                        case "左下端2段筋本数(細径)":
                            p += RCGir.count_2nd_main_bottom_2nd[0] + Kugiri2;
                            p += RCCGir.count_2nd_main_bottom_2nd[0] + Kugiri2;
                            p += SRCGirH.count_2nd_main_bottom_2nd[0] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_bottom_2nd[0] + Kugiri2;
                            break;
                        case "左下端3段筋本数(細径)":
                            p += RCGir.count_2nd_main_bottom_2nd[0] + Kugiri2;
                            p += RCCGir.count_2nd_main_bottom_2nd[0] + Kugiri2;
                            p += SRCGirH.count_2nd_main_bottom_2nd[0] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_bottom_2nd[0] + Kugiri2;
                            break;


                        case "中央上端主筋径(太径)":
                            p += RCGir.D_reinforcement_main_top[1] + Kugiri2;                           
                            p += SRCGirH.D_reinforcement_main_top[1] + Kugiri2;
                            p += SRCCGirH.D_reinforcement_main_top[1] + Kugiri2;
                            break;
                        case "中央上端主筋径(細径)":
                            p += RCGir.D_reinforcement_2nd_main_top[1] + Kugiri2;
                            p += SRCGirH.D_reinforcement_2nd_main_top[1] + Kugiri2;
                            p += SRCCGirH.D_reinforcement_2nd_main_top[1] + Kugiri2;
                            break;
                        case "中央上端1段筋本数(太径)":
                            p += RCGir.count_main_top_1st[1] + Kugiri2;
                            p += SRCGirH.count_main_top_1st[1] + Kugiri2;
                            p += SRCCGirH.count_main_top_1st[1] + Kugiri2;
                            break;
                        case "中央上端2段筋本数(太径)":
                            p += RCGir.count_main_top_2nd[1] + Kugiri2;
                            p += SRCGirH.count_main_top_2nd[1] + Kugiri2;
                            p += SRCCGirH.count_main_top_2nd[1] + Kugiri2;
                            break;
                        case "中央上端3段筋本数(太径)":
                            p += RCGir.count_main_top_3rd[1] + Kugiri2;
                            p += SRCGirH.count_main_top_3rd[1] + Kugiri2;
                            p += SRCCGirH.count_main_top_3rd[1] + Kugiri2;
                            break;
                        case "中央上端1段筋本数(細径)":
                            p += RCGir.count_2nd_main_top_1st[1] + Kugiri2;
                            p += SRCGirH.count_2nd_main_top_1st[1] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_top_1st[1] + Kugiri2;
                            break;
                        case "中央上端2段筋本数(細径)":
                            p += RCGir.count_2nd_main_top_2nd[1] + Kugiri2;
                            p += SRCGirH.count_2nd_main_top_2nd[1] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_top_2nd[1] + Kugiri2;
                            break;
                        case "中央上端3段筋本数(細径)":
                            p += RCGir.count_2nd_main_top_3rd[1] + Kugiri2;
                            p += SRCGirH.count_2nd_main_top_3rd[1] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_top_3rd[1] + Kugiri2;
                            break;

                        case "中央下端主筋径(太径)":
                            p += RCGir.D_reinforcement_main_bottom[1] + Kugiri2;
                            p += SRCGirH.D_reinforcement_main_bottom[1] + Kugiri2;
                            p += SRCCGirH.D_reinforcement_main_bottom[1] + Kugiri2;
                            break;
                        case "中央下端主筋径(細径)":
                            p += RCGir.D_reinforcement_2nd_main_bottom[1] + Kugiri2;
                            p += SRCGirH.D_reinforcement_2nd_main_bottom[1] + Kugiri2;
                            p += SRCCGirH.D_reinforcement_2nd_main_bottom[1] + Kugiri2;
                            break;
                        case "中央下端1段筋本数(太径)":
                            p += RCGir.count_main_bottom_1st[1] + Kugiri2;
                            p += SRCGirH.count_main_bottom_1st[1] + Kugiri2;
                            p += SRCCGirH.count_main_bottom_1st[1] + Kugiri2;
                            break;
                        case "中央下端2段筋本数(太径)":
                            p += RCGir.count_main_bottom_2nd[1] + Kugiri2;
                            p += SRCGirH.count_main_bottom_2nd[1] + Kugiri2;
                            p += SRCCGirH.count_main_bottom_2nd[1] + Kugiri2;
                            break;
                        case "中央下端3段筋本数(太径)":
                            p += RCGir.count_main_bottom_3rd[1] + Kugiri2;
                            p += SRCGirH.count_main_bottom_3rd[1] + Kugiri2;
                            p += SRCCGirH.count_main_bottom_3rd[1] + Kugiri2;
                            break;
                        case "中央下端1段筋本数(細径)":
                            p += RCGir.count_2nd_main_bottom_1st[1] + Kugiri2;
                            p += SRCGirH.count_2nd_main_bottom_1st[1] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_bottom_1st[1] + Kugiri2;
                            break;
                        case "中央下端2段筋本数(細径)":
                            p += RCGir.count_2nd_main_bottom_2nd[1] + Kugiri2;
                            p += SRCGirH.count_2nd_main_bottom_2nd[1] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_bottom_2nd[1] + Kugiri2;
                            break;
                        case "中央下端3段筋本数(細径)":
                            p += RCGir.count_2nd_main_bottom_2nd[1] + Kugiri2;
                            p += SRCGirH.count_2nd_main_bottom_2nd[1] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_bottom_2nd[1] + Kugiri2;
                            break;

                        case "右上端主筋径(太径)":
                            p += RCGir.D_reinforcement_main_top[2] + Kugiri2;
                            p += RCCGir.D_reinforcement_main_top[1] + Kugiri2;
                            p += SRCGirH.D_reinforcement_main_top[2] + Kugiri2;
                            p += SRCCGirH.D_reinforcement_main_top[2] + Kugiri2;
                            break;
                        case "右上端主筋径(細径)":
                            p += RCGir.D_reinforcement_2nd_main_top[2] + Kugiri2;
                            p += RCCGir.D_reinforcement_2nd_main_top[1] + Kugiri2;
                            p += SRCGirH.D_reinforcement_2nd_main_top[2] + Kugiri2;
                            p += SRCCGirH.D_reinforcement_2nd_main_top[2] + Kugiri2;
                            break;
                        case "右上端1段筋本数(太径)":
                            p += RCGir.count_main_top_1st[2] + Kugiri2;
                            p += RCCGir.count_main_top_1st[1] + Kugiri2;
                            p += SRCGirH.count_main_top_1st[2] + Kugiri2;
                            p += SRCCGirH.count_main_top_1st[2] + Kugiri2;
                            break;
                        case "右上端2段筋本数(太径)":
                            p += RCGir.count_main_top_2nd[2] + Kugiri2;
                            p += RCCGir.count_main_top_2nd[1] + Kugiri2;
                            p += SRCGirH.count_main_top_2nd[2] + Kugiri2;
                            p += SRCCGirH.count_main_top_2nd[2] + Kugiri2;
                            break;
                        case "右上端3段筋本数(太径)":
                            p += RCGir.count_main_top_3rd[2] + Kugiri2;
                            p += RCCGir.count_main_top_3rd[1] + Kugiri2;
                            p += SRCGirH.count_main_top_3rd[2] + Kugiri2;
                            p += SRCCGirH.count_main_top_3rd[2] + Kugiri2;
                            break;
                        case "右上端1段筋本数(細径)":
                            p += RCGir.count_2nd_main_top_1st[2] + Kugiri2;
                            p += RCCGir.count_2nd_main_top_1st[1] + Kugiri2;
                            p += SRCGirH.count_2nd_main_top_1st[2] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_top_1st[2] + Kugiri2;
                            break;
                        case "右上端2段筋本数(細径)":
                            p += RCGir.count_2nd_main_top_2nd[2] + Kugiri2;
                            p += RCCGir.count_2nd_main_top_2nd[1] + Kugiri2;
                            p += SRCGirH.count_2nd_main_top_2nd[2] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_top_2nd[2] + Kugiri2;
                            break;
                        case "右上端3段筋本数(細径)":
                            p += RCGir.count_2nd_main_top_3rd[2] + Kugiri2;
                            p += RCCGir.count_2nd_main_top_3rd[1] + Kugiri2;
                            p += SRCGirH.count_2nd_main_top_3rd[2] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_top_3rd[2] + Kugiri2;
                            break;

                        case "右下端主筋径(太径)":
                            p += RCGir.D_reinforcement_main_bottom[2] + Kugiri2;
                            p += RCCGir.D_reinforcement_main_bottom[1] + Kugiri2;
                            p += SRCGirH.D_reinforcement_main_bottom[2] + Kugiri2;
                            p += SRCCGirH.D_reinforcement_main_bottom[2] + Kugiri2;
                            break;
                        case "右下端主筋径(細径)":
                            p += RCGir.D_reinforcement_2nd_main_bottom[2] + Kugiri2;
                            p += RCCGir.D_reinforcement_2nd_main_bottom[1] + Kugiri2;
                            p += SRCGirH.D_reinforcement_2nd_main_bottom[2] + Kugiri2;
                            p += SRCCGirH.D_reinforcement_2nd_main_bottom[2] + Kugiri2;
                            break;
                        case "右下端1段筋本数(太径)":
                            p += RCGir.count_main_bottom_1st[2] + Kugiri2;
                            p += RCCGir.count_main_bottom_1st[1] + Kugiri2;
                            p += SRCGirH.count_main_bottom_1st[2] + Kugiri2;
                            p += SRCCGirH.count_main_bottom_1st[2] + Kugiri2;
                            break;
                        case "右下端2段筋本数(太径)":
                            p += RCGir.count_main_bottom_2nd[2] + Kugiri2;
                            p += RCCGir.count_main_bottom_2nd[1] + Kugiri2;
                            p += SRCGirH.count_main_bottom_2nd[2] + Kugiri2;
                            p += SRCCGirH.count_main_bottom_2nd[2] + Kugiri2;
                            break;
                        case "右下端3段筋本数(太径)":
                            p += RCGir.count_main_bottom_3rd[2] + Kugiri2;
                            p += RCCGir.count_main_bottom_3rd[1] + Kugiri2;
                            p += SRCGirH.count_main_bottom_3rd[2] + Kugiri2;
                            p += SRCCGirH.count_main_bottom_3rd[2] + Kugiri2;
                            break;
                        case "右下端1段筋本数(細径)":
                            p += RCGir.count_2nd_main_bottom_1st[2] + Kugiri2;
                            p += RCCGir.count_2nd_main_bottom_1st[1] + Kugiri2;
                            p += SRCGirH.count_2nd_main_bottom_1st[2] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_bottom_1st[2] + Kugiri2;
                            break;
                        case "右下端2段筋本数(細径)":
                            p += RCGir.count_2nd_main_bottom_2nd[2] + Kugiri2;
                            p += RCCGir.count_2nd_main_bottom_2nd[1] + Kugiri2;
                            p += SRCGirH.count_2nd_main_bottom_2nd[2] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_bottom_2nd[2] + Kugiri2;
                            break;
                        case "右下端3段筋本数(細径)":
                            p += RCGir.count_2nd_main_bottom_2nd[2] + Kugiri2;
                            p += RCCGir.count_2nd_main_bottom_2nd[1] + Kugiri2;
                            p += SRCGirH.count_2nd_main_bottom_2nd[2] + Kugiri2;
                            p += SRCCGirH.count_2nd_main_bottom_2nd[2] + Kugiri2;
                            break;


                        #endregion
                        #region 肋筋

                        case "左スターラップ径":
                            p += RCGir.D_stirrup[0] + Kugiri2;
                            p += RCCGir.D_stirrup[0] + Kugiri2;
                            p += SRCGirH.D_stirrup[0] + Kugiri2;
                            p += SRCCGirH.D_stirrup[0] + Kugiri2;
                            break;
                        case "左スターラップ本数":
                            p += RCGir.count_stirrup[0] + Kugiri2;
                            p += RCCGir.count_stirrup[0] + Kugiri2;
                            p += SRCGirH.count_stirrup[0] + Kugiri2;
                            p += SRCCGirH.count_stirrup[0] + Kugiri2;
                            break;
                        case "左スターラップ間隔":
                            p += RCGir.pitch_stirrup[0] + Kugiri2;
                            p += RCCGir.pitch_stirrup[0] + Kugiri2;
                            p += SRCGirH.pitch_stirrup[0] + Kugiri2;
                            p += SRCCGirH.pitch_stirrup[0] + Kugiri2;
                            break;

                        case "中央スターラップ径":
                            p += RCGir.D_stirrup[1] + Kugiri2;
                            p += SRCGirH.D_stirrup[1] + Kugiri2;
                            p += SRCCGirH.D_stirrup[1] + Kugiri2;
                            break;
                        case "中央スターラップ本数":
                            p += RCGir.count_stirrup[1] + Kugiri2;
                            p += SRCGirH.count_stirrup[1] + Kugiri2;
                            p += SRCCGirH.count_stirrup[1] + Kugiri2;
                            break;
                        case "中央スターラップ間隔":
                            p += RCGir.pitch_stirrup[1] + Kugiri2;
                            p += SRCGirH.pitch_stirrup[1] + Kugiri2;
                            p += SRCCGirH.pitch_stirrup[1] + Kugiri2;
                            break;

                        case "右スターラップ径":
                            p += RCGir.D_stirrup[2] + Kugiri2;
                            p += RCCGir.D_stirrup[1] + Kugiri2;
                            p += SRCGirH.D_stirrup[2] + Kugiri2;
                            p += SRCCGirH.D_stirrup[2] + Kugiri2;
                            break;
                        case "右スターラップ本数":
                            p += RCGir.count_stirrup[2] + Kugiri2;
                            p += RCCGir.count_stirrup[1] + Kugiri2;
                            p += SRCGirH.count_stirrup[2] + Kugiri2;
                            p += SRCCGirH.count_stirrup[2] + Kugiri2;
                            break;
                        case "右スターラップ間隔":
                            p += RCGir.pitch_stirrup[2] + Kugiri2;
                            p += RCCGir.pitch_stirrup[1] + Kugiri2;
                            p += SRCGirH.pitch_stirrup[2] + Kugiri2;
                            p += SRCCGirH.pitch_stirrup[2] + Kugiri2;
                            break;

                        #endregion
                        #region 寸法_コンクリート
                       
                        case "左b":
                            p += RCGir.width_start + Kugiri2;
                            p += RCCGir.width_start + Kugiri2;
                            p += SRCGirH.width_start + Kugiri2;
                            p += SRCCGirH.width_start + Kugiri2;
                            break;
                        case "左D":
                            p += RCGir.depth_start + Kugiri2;
                            p += RCCGir.depth_start + Kugiri2;
                            p += SRCGirH.depth_start + Kugiri2;
                            p += SRCCGirH.depth_start + Kugiri2;
                            break;
                        case "全断面b":
                        case "中央b":
                            p += RCGir.width_center + Kugiri2;
                            p += SRCGirH.width_center + Kugiri2;
                            p += SRCCGirH.width_center + Kugiri2;
                            break;
                        case "全断面D":
                        case "中央D":
                            p += RCGir.depth_center + Kugiri2;
                            p += SRCGirH.depth_center + Kugiri2;
                            p += SRCCGirH.depth_center + Kugiri2;
                            break;
                        case "右b":
                            p += RCGir.width_end + Kugiri2;
                            p += RCCGir.width_end + Kugiri2;
                            p += SRCGirH.width_end + Kugiri2;
                            p += SRCCGirH.width_end + Kugiri2;
                            break;
                        case "右D":
                            p += RCGir.depth_end + Kugiri2;
                            p += RCCGir.depth_end + Kugiri2;
                            p += SRCGirH.depth_end + Kugiri2;
                            p += SRCCGirH.depth_end + Kugiri2;
                            break;
                        #endregion
                        #region 寸法_鉄骨

                        case "全断面A":
                        case "中央A":
                            p += SGirH.A[1] + Kugiri2;
                            p += SGirBH.A[1] + Kugiri2;
                            p += SGirC.H[1] + Kugiri2;
                            p += SGirL.A[1] + Kugiri2;
                            p += SGirLipC.H[1] + Kugiri2;
                            p += SRCGirH.A[1] + Kugiri2;
                            p += SCGirH.A[0] + Kugiri2;
                            p += SRCGirH.A[1] + Kugiri2;
                            break;
                        case "全断面B":
                        case "中央B":
                            p += SGirH.B[1] + Kugiri2;
                            p += SGirBH.B[1] + Kugiri2;
                            p += SGirC.B[1] + Kugiri2;
                            p += SGirL.B[1] + Kugiri2;
                            p += SGirLipC.A[1] + Kugiri2;
                            p += SRCGirH.B[1] + Kugiri2;
                            p += SCGirH.B[0] + Kugiri2;
                            p += SRCGirH.B[1] + Kugiri2;
                            break;
                        case "全断面t1":
                        case "中央t1":
                            p += SGirH.t1[1] + Kugiri2;
                            p += SGirBH.t1[1] + Kugiri2;
                            p += SGirC.t1[1] + Kugiri2;
                            p += SGirL.t1[1] + Kugiri2;
                            p += SGirLipC.t[1] + Kugiri2;
                            p += SRCGirH.t1[1] + Kugiri2;
                            p += SCGirH.t1[0] + Kugiri2;
                            p += SRCGirH.t1[1] + Kugiri2;
                            break;
                        case "全断面t2":
                        case "中央t2":
                            p += SGirH.t2[1] + Kugiri2;
                            p += SGirBH.t2[1] + Kugiri2;
                            p += SGirC.t2[1] + Kugiri2;
                            p += SGirL.t2[1] + Kugiri2;
                            p += SGirLipC.t[1] + Kugiri2;
                            p += SRCGirH.t2[1] + Kugiri2;
                            p += SCGirH.t2[0] + Kugiri2;
                            p += SRCGirH.t2[1] + Kugiri2;
                            break;
                        case "全断面形状":
                        case "中央形状":
                            break;
                        case "左A":
                            p += SGirH.A[0] + Kugiri2;
                            p += SGirBH.A[0] + Kugiri2;
                            p += SGirC.H[0] + Kugiri2;
                            p += SGirL.A[0] + Kugiri2;
                            p += SGirLipC.H[0] + Kugiri2;
                            p += SRCGirH.A[0] + Kugiri2;
                            p += SCGirH.A[0] + Kugiri2;
                            p += SRCGirH.A[0] + Kugiri2;
                            break;
                        case "左B":
                            p += SGirH.B[0] + Kugiri2;
                            p += SGirBH.B[0] + Kugiri2;
                            p += SGirC.B[0] + Kugiri2;
                            p += SGirL.B[0] + Kugiri2;
                            p += SGirLipC.A[0] + Kugiri2;
                            p += SRCGirH.B[0] + Kugiri2;
                            p += SCGirH.B[0] + Kugiri2;
                            p += SRCGirH.B[0] + Kugiri2;
                            break;
                        case "左t1":
                            p += SGirH.t1[0] + Kugiri2;
                            p += SGirBH.t1[0] + Kugiri2;
                            p += SGirC.t1[0] + Kugiri2;
                            p += SGirL.t1[0] + Kugiri2;
                            p += SGirLipC.t[0] + Kugiri2;
                            p += SRCGirH.t1[0] + Kugiri2;
                            p += SCGirH.t1[0] + Kugiri2;
                            p += SRCGirH.t1[0] + Kugiri2;
                            break;
                        case "左t2":
                            p += SGirH.t2[0] + Kugiri2;
                            p += SGirBH.t2[0] + Kugiri2;
                            p += SGirC.t2[0] + Kugiri2;
                            p += SGirL.t2[0] + Kugiri2;
                            p += SGirLipC.t[0] + Kugiri2;
                            p += SRCGirH.t2[0] + Kugiri2;
                            p += SCGirH.t2[0] + Kugiri2;
                            p += SRCGirH.t2[0] + Kugiri2;
                            break;
                        case "右A":
                            p += SGirH.A[2] + Kugiri2;
                            p += SGirBH.A[2] + Kugiri2;
                            p += SGirC.H[2] + Kugiri2;
                            p += SGirL.A[2] + Kugiri2;
                            p += SGirLipC.H[2] + Kugiri2;
                            p += SRCGirH.A[2] + Kugiri2;
                            p += SCGirH.A[1] + Kugiri2;
                            p += SRCGirH.A[2] + Kugiri2;
                            break;
                        case "右B":
                            p += SGirH.B[2] + Kugiri2;
                            p += SGirBH.B[2] + Kugiri2;
                            p += SGirC.B[2] + Kugiri2;
                            p += SGirL.B[2] + Kugiri2;
                            p += SGirLipC.A[2] + Kugiri2;
                            p += SRCGirH.B[2] + Kugiri2;
                            p += SCGirH.B[1] + Kugiri2;
                            p += SRCGirH.B[2] + Kugiri2;
                            break;
                        case "右t1":
                            p += SGirH.t1[2] + Kugiri2;
                            p += SGirBH.t1[2] + Kugiri2;
                            p += SGirC.t1[2] + Kugiri2;
                            p += SGirL.t1[2] + Kugiri2;
                            p += SGirLipC.t[2] + Kugiri2;
                            p += SRCGirH.t1[2] + Kugiri2;
                            p += SCGirH.t1[1] + Kugiri2;
                            p += SRCGirH.t1[2] + Kugiri2;
                            break;
                        case "右t2":
                            p += SGirH.t2[2] + Kugiri2;
                            p += SGirBH.t2[2] + Kugiri2;
                            p += SGirC.t2[2] + Kugiri2;
                            p += SGirL.t2[2] + Kugiri2;
                            p += SGirLipC.t[2] + Kugiri2;
                            p += SRCGirH.t2[2] + Kugiri2;
                            p += SCGirH.t2[1] + Kugiri2;
                            p += SRCGirH.t2[2] + Kugiri2;
                            break;

                            #endregion
                    }
                    break;
                #endregion
                #region
                //case "柱パラメータ":
                //    #region 柱パラメータ

                //    switch (name)
                //    {
                //        #region 主筋

                //        case "柱頭主筋径(太径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱頭主筋太径 + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_RC_Sylinder_RST_柱頭主筋径 + Kugiri2;
                //            break;
                //        case "柱頭主筋径(細径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱頭主筋細径 + Kugiri2;
                //            break;
                //        case "柱頭主筋ny本数(一段筋太径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱頭主筋X1段太径本数 + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_RC_Sylinder_RST_柱頭主筋本数 + Kugiri2;
                //            break;
                //        case "柱頭主筋ny本数(一段筋細径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱頭主筋X1段細径本数 + Kugiri2;
                //            break;
                //        case "柱頭主筋ny本数(二段筋太径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱頭主筋X2段太径本数 + Kugiri2;
                //            break;
                //        case "柱頭主筋ny本数(二段筋細径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱頭主筋X2段細径本数 + Kugiri2;
                //            break;
                //        case "柱頭主筋nz本数(一段筋太径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱頭主筋Y1段太径本数 + Kugiri2;
                //            break;
                //        case "柱頭主筋nz本数(一段筋細径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱頭主筋Y1段細径本数 + Kugiri2;
                //            break;
                //        case "柱頭主筋nz本数(二段筋太径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱頭主筋Y2段太径本数 + Kugiri2;
                //            break;
                //        case "柱頭主筋nz本数(二段筋細径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱頭主筋Y2段細径本数 + Kugiri2;
                //            break;

                //        case "柱脚主筋径(太径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱脚主筋太径 + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_RC_Sylinder_RST_柱脚主筋径 + Kugiri2;
                //            break;
                //        case "柱脚主筋径(細径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱脚主筋細径 + Kugiri2;
                //            break;
                //        case "柱脚主筋ny本数(一段筋太径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱脚主筋X1段太径本数 + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_RC_Sylinder_RST_柱脚主筋本数 + Kugiri2;
                //            break;
                //        case "柱脚主筋ny本数(一段筋細径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱脚主筋X1段細径本数 + Kugiri2;
                //            break;
                //        case "柱脚主筋ny本数(二段筋太径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱脚主筋X2段太径本数 + Kugiri2;
                //            break;
                //        case "柱脚主筋ny本数(二段筋細径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱脚主筋X2段細径本数 + Kugiri2;
                //            break;
                //        case "柱脚主筋nz本数(一段筋太径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱脚主筋Y1段太径本数 + Kugiri2;
                //            break;
                //        case "柱脚主筋nz本数(一段筋細径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱脚主筋Y1段細径本数 + Kugiri2;
                //            break;
                //        case "柱脚主筋nz本数(二段筋太径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱脚主筋Y2段太径本数 + Kugiri2;
                //            break;
                //        case "柱脚主筋nz本数(二段筋細径)":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱脚主筋Y2段細径本数 + Kugiri2;
                //            break;

                //        #endregion
                //        #region 帯筋

                //        case "柱頭帯筋径":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱頭帯筋径 + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_RC_Sylinder_RST_柱頭帯筋径 + Kugiri2;
                //            break;
                //        case "柱頭帯筋ny本数":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱頭帯筋Y本数 + Kugiri2;
                //            break;
                //        case "柱頭帯筋nz本数":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱頭帯筋X本数 + Kugiri2;
                //            break;
                //        case "柱頭帯筋ピッチ":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱頭帯筋ピッチ + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_RC_Sylinder_RST_柱頭帯筋ピッチ + Kugiri2;
                //            break;

                //        case "柱脚帯筋径":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱脚帯筋径 + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_RC_Sylinder_RST_柱脚帯筋径 + Kugiri2;
                //            break;
                //        case "柱脚帯筋ny本数":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱脚帯筋Y本数 + Kugiri2;
                //            break;
                //        case "柱脚帯筋nz本数":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱脚帯筋X本数 + Kugiri2;
                //            break;
                //        case "柱脚帯筋ピッチ":
                //            p += ThisMainExtension.Data.ParamName_C_RC_RST_柱脚帯筋ピッチ + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_RC_Sylinder_RST_柱脚帯筋ピッチ + Kugiri2;
                //            break;

                //        #endregion
                //        #region 寸法

                //        case "柱頭Dy":
                //        case "柱脚Dy":
                //            p += ThisMainExtension.Data.ParamName_C_RC_b + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_RC_Sylinder_直径 + Kugiri2;
                //            break;
                //        case "柱頭Dz":
                //        case "柱脚Dz":
                //            p += ThisMainExtension.Data.ParamName_C_RC_h + Kugiri2;
                //            break;
                //        case "柱頭YA":
                //        case "柱脚YA":
                //            p += ThisMainExtension.Data.ParamName_C_S_H_d + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_S_Box_h + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_S_Pipe_OD + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_CFT_Box_h + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_CFT_Pipe_OD + Kugiri2;
                //            break;
                //        case "柱頭YB":
                //        case "柱脚YB":
                //            p += ThisMainExtension.Data.ParamName_C_S_H_bf + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_S_Box_b + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_CFT_Box_b + Kugiri2;
                //            break;
                //        case "柱頭Yt1":
                //        case "柱脚Yt1":
                //            p += ThisMainExtension.Data.ParamName_C_S_H_tw + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_S_Box_t + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_S_Pipe_t + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_CFT_Box_t + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_CFT_Pipe_t + Kugiri2;
                //            break;
                //        case "柱頭Yt2":
                //        case "柱脚Yt2":
                //            p += ThisMainExtension.Data.ParamName_C_S_H_tf + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_S_Box_t + Kugiri2;
                //            p += ThisMainExtension.Data.ParamName_C_CFT_Box_t + Kugiri2;
                //            break;

                //        case "柱頭ZA":
                //        case "柱頭ZB":
                //        case "柱頭Zt1":
                //        case "柱頭Zt2":
                //        case "柱脚ZA":
                //        case "柱脚ZB":
                //        case "柱脚Zt1":
                //        case "柱脚Zt2":
                //            //I形
                //            break;

                //            #endregion
                //    }

                //    break;
                //#endregion

                //case "床パラメータ":
                //    #region 床パラメータ

                //    switch (name)
                //    {
                //        case "上_短辺_A端部_D1":
                //        case "上_短辺_A中央_D1":
                //            p += ThisMainExtension.Data.ParamName_S_RST_主中央上径1;
                //            break;
                //        case "上_短辺_A端部_D2":
                //        case "上_短辺_A中央_D2":
                //            p += ThisMainExtension.Data.ParamName_S_RST_主中央上径2;
                //            break;
                //        case "上_短辺_A端部_PITCH":
                //        case "上_短辺_A中央_PITCH":
                //            p += ThisMainExtension.Data.ParamName_S_RST_主中央上ピッチ;
                //            break;

                //        case "上_短辺_B端中_D1":
                //            p += ThisMainExtension.Data.ParamName_S_RST_主端部上径1;
                //            break;
                //        case "上_短辺_B端中_D2":
                //            p += ThisMainExtension.Data.ParamName_S_RST_主端部上径2;
                //            break;
                //        case "上_短辺_B端中_PITCH":
                //            p += ThisMainExtension.Data.ParamName_S_RST_主端部上ピッチ;
                //            break;

                //        case "上_長辺_A端部_D1":
                //        case "上_長辺_A中央_D1":
                //            p += ThisMainExtension.Data.ParamName_S_RST_配中央上径1;
                //            break;
                //        case "上_長辺_A端部_D2":
                //        case "上_長辺_A中央_D2":
                //            p += ThisMainExtension.Data.ParamName_S_RST_配中央上径2;
                //            break;
                //        case "上_長辺_A端部_PITCH":
                //        case "上_長辺_A中央_PITCH":
                //            p += ThisMainExtension.Data.ParamName_S_RST_配中央上ピッチ;
                //            break;

                //        case "上_長辺_B端中_D1":
                //            p += ThisMainExtension.Data.ParamName_S_RST_配端部上径1;
                //            break;
                //        case "上_長辺_B端中_D2":
                //            p += ThisMainExtension.Data.ParamName_S_RST_配端部上径2;
                //            break;
                //        case "上_長辺_B端中_PITCH":
                //            p += ThisMainExtension.Data.ParamName_S_RST_配端部上ピッチ;
                //            break;


                //        case "下_短辺_A端部_D1":
                //        case "下_短辺_A中央_D1":
                //            p += ThisMainExtension.Data.ParamName_S_RST_主中央下径1;
                //            break;
                //        case "下_短辺_A端部_D2":
                //        case "下_短辺_A中央_D2":
                //            p += ThisMainExtension.Data.ParamName_S_RST_主中央下径2;
                //            break;
                //        case "下_短辺_A端部_PITCH":
                //        case "下_短辺_A中央_PITCH":
                //            p += ThisMainExtension.Data.ParamName_S_RST_主中央下ピッチ;
                //            break;

                //        case "下_短辺_B端中_D1":
                //            p += ThisMainExtension.Data.ParamName_S_RST_主端部下径1;
                //            break;
                //        case "下_短辺_B端中_D2":
                //            p += ThisMainExtension.Data.ParamName_S_RST_主端部下径2;
                //            break;
                //        case "下_短辺_B端中_PITCH":
                //            p += ThisMainExtension.Data.ParamName_S_RST_主端部下ピッチ;
                //            break;

                //        case "下_長辺_A端部_D1":
                //        case "下_長辺_A中央_D1":
                //            p += ThisMainExtension.Data.ParamName_S_RST_配中央下径1;
                //            break;
                //        case "下_長辺_A端部_D2":
                //        case "下_長辺_A中央_D2":
                //            p += ThisMainExtension.Data.ParamName_S_RST_配中央下径2;
                //            break;
                //        case "下_長辺_A端部_PITCH":
                //        case "下_長辺_A中央_PITCH":
                //            p += ThisMainExtension.Data.ParamName_S_RST_配中央下ピッチ;
                //            break;

                //        case "下_長辺_B端中_D1":
                //            p += ThisMainExtension.Data.ParamName_S_RST_配端部下径1;
                //            break;
                //        case "下_長辺_B端中_D2":
                //            p += ThisMainExtension.Data.ParamName_S_RST_配端部下径2;
                //            break;
                //        case "下_長辺_B端中_PITCH":
                //            p += ThisMainExtension.Data.ParamName_S_RST_配端部下ピッチ;
                //            break;
                //    }
                //    break;

                //#endregion

                //case "壁パラメータ":
                //    #region 壁パラメータ
                //    switch (name)
                //    {
                //        case "配筋タイプ":
                //            p += ThisMainExtension.Data.ParamName_W_RST_配筋タイプ;
                //            break;

                //        case "タテ筋D1":
                //            p += ThisMainExtension.Data.ParamName_W_RST_縦筋径1;
                //            break;
                //        case "タテ筋D2":
                //            p += ThisMainExtension.Data.ParamName_W_RST_縦筋径2;
                //            break;
                //        case "タテ筋Pitch":
                //            p += ThisMainExtension.Data.ParamName_W_RST_縦筋ピッチ;
                //            break;
                //        case "ヨコ筋D1":
                //            p += ThisMainExtension.Data.ParamName_W_RST_横筋径1;
                //            break;
                //        case "ヨコ筋D2":
                //            p += ThisMainExtension.Data.ParamName_W_RST_横筋径2;
                //            break;
                //        case "ヨコ筋Pitch":
                //            p += ThisMainExtension.Data.ParamName_W_RST_横筋ピッチ;
                //            break;
                //    }
                //    break;
                //#endregion
                #endregion
                default:
                    break;
            }


            p += "\r\n";

            return p;
        }
        private static string SLMFamily(string name)
        {
            const string Kugiri = "："; //全角コロン
            //const string Kugiri2 = ","; //半角カンマ
            //const string RFA = ".rfa";

            string f = name + Kugiri + "ファミリ名称" + Kugiri;

            //switch (name)
            //{
            //    case "RC梁_ハンチ":
            //        f += ThisMainExtension.Data.FamilyName_FG + RFA + Kugiri2;
            //        f += ThisMainExtension.Data.FamilyName_FG_Hunch + RFA + Kugiri2;
            //        f += ThisMainExtension.Data.FamilyName_G_RC + RFA + Kugiri2;
            //        f += ThisMainExtension.Data.FamilyName_G_RC_Hunch + RFA + Kugiri2;
            //        f += ThisMainExtension.Data.FamilyName_B_RC + RFA + Kugiri2;
            //        break;

            //    case "S梁_H_ハンチ":
            //        f += ThisMainExtension.Data.FamilyName_G_S + RFA + Kugiri2;
            //        f += ThisMainExtension.Data.FamilyName_G_S_Hunch + RFA + Kugiri2;
            //        f += ThisMainExtension.Data.FamilyName_B_S + RFA + Kugiri2;
            //        break;

            //    case "RC柱_角":
            //        f += ThisMainExtension.Data.FamilyName_C_RC + RFA + Kugiri2;
            //        break;
            //    case "RC柱_丸":
            //        f += ThisMainExtension.Data.FamilyName_C_RC_Sylinder + RFA + Kugiri2;
            //        break;

            //    case "S柱_Hy":
            //        f += ThisMainExtension.Data.FamilyName_C_S_H + RFA + Kugiri2;
            //        break;
            //    case "S柱_BX":
            //        f += ThisMainExtension.Data.FamilyName_C_S_Box + RFA + Kugiri2;
            //        break;
            //    case "S柱_P":
            //        f += ThisMainExtension.Data.FamilyName_C_S_Pipe + RFA + Kugiri2;
            //        break;
            //}

            f += "\r\n";

            return f;
        }

        #endregion
    }
}
