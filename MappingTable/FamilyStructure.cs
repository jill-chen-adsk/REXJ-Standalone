using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MappingTable
{
    public class FamilyStructure
    {

        internal class WallFamilyName
        {
            internal bool[][] flg = { new bool[] { false, false } };
            internal string[][] Name = { new string[] { "", "" } };
        }

        #region 柱
        internal class ClmFamilyName
        {
            /// <summary>プロジェクトにロードされている⇒true
            /// </summary>
            internal bool[][] flg = { new bool[] { false, false } ,
                                      new bool[] { false,false, false, false, false, false, false, false } ,
                                      new bool[] { false,false, false, false, false, false } ,
                                      new bool[] { false,false } };
            /// <summary>変換対象⇒true
            /// </summary>
            internal bool[][] convflg = { new bool[] { false, false } ,
                                      new bool[] { false,false, false, false, false, false, false, false } ,
                                      new bool[] { false,false, false, false, false, false } ,
                                      new bool[] { false,false } };

            internal string[][] FamilyName = { new string[] { "","" },
                                         new string[] { "","","","","","","","" },
                                         new string[] { "", "","", "","","" },
                                         new string[] { "","" } };
            internal string[][] TypeName = { new string[] { "","" },
                                         new string[] { "","","","","","","","" },
                                         new string[] { "", "","", "","","" },
                                         new string[] { "","" } };
        }
        internal class BClmFamilyName
        {
            /// <summary>プロジェクトにロードされている⇒true
            /// </summary>
            internal bool[][] flg = { new bool[] { false, false } };
            /// <summary>変換対象⇒true
            /// </summary>
            internal bool[][] convflg = { new bool[] { false, false } };
            internal string[][] FamilyName = { new string[] { "", "" } };
            internal string[][] TypeName = { new string[] { "", "" } };
        }



        /// <summary>RC柱
        /// </summary>
        internal class RC_Clm_Re
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*********************************************************
            internal string FamilyName = "RC柱";

            //*****タイプパラメータ***********************************************************************
            internal string strength_concrete = "構造マテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string DX = "柱幅";
            internal string DY = "柱せい";
            internal string name = "符号";
            internal string[] D_reinforcement_main = { "柱脚 主筋太径", "柱頭 主筋太径" };
            internal string[] D_reinforcement_2nd_main = { "柱頭 主筋細径", "柱脚 主筋細径" };
            internal string D_reinforcement_axial = "芯鉄筋径";
            internal string[] D_reinforcement_band = { "柱脚 帯筋径", "柱頭 帯筋径" };
            internal string D_bar_spacing = "幅止筋径";
            internal string strength_reinforcement_main = "主筋種別X";
            internal string strength_reinforcement_2nd_main = "副主筋強度";
            internal string strength_reinforcement_axial = "芯鉄筋種別";
            internal string[] depth_cover_X = { "かぶり厚さ（X始）", "かぶり厚さ（X終）" };
            internal string[] depth_cover_Y = { "かぶり厚さ（Y始）", "かぶり厚さ（Y終）" };
            internal string[] count_main_X_1st = { "柱脚 主筋X方向1段太筋本数", "柱頭 主筋X方向1段太筋本数" };
            internal string[] count_2nd_main_X_1st = { "柱脚 主筋X方向1段細筋本数", "柱頭 主筋X方向1段細筋本数" };
            internal string[] count_main_X_2nd = { "柱脚 主筋X方向2段太筋本数", "柱頭 主筋X方向2段太筋本数" };
            internal string[] count_2nd_main_X_2nd = { "柱脚 主筋X方向2段細筋本数", "柱頭 主筋X方向2段細筋本数" };
            internal string[] count_main_Y_1st = { "柱脚 主筋Y方向1段太筋本数", "柱頭 主筋Y方向1段太筋本数" };
            internal string[] count_2nd_main_Y_1st = { "柱脚 主筋Y方向1段細筋本数", "柱頭 主筋Y方向1段細筋本数" };
            internal string[] count_main_Y_2nd = { "柱脚 主筋Y方向2段太筋本数", "柱頭 主筋Y方向2段太筋本数" };
            internal string[] count_2nd_main_Y_2nd = { "柱脚 主筋Y方向2段細筋本数", "柱頭 主筋Y方向2段細筋本数" };
            internal string[] count_band_dir_X = { "柱脚 帯筋X方向本数", "柱頭 帯筋X方向本数" };
            internal string[] count_band_dir_Y = { "柱脚 帯筋Y方向本数", "柱頭 帯筋Y方向本数" };
            internal string[] pitch_band = { "柱脚 帯筋ピッチ", "柱頭 帯筋ピッチ" };
            internal string[] count_bar_spacing_X = { "柱脚 幅止筋X方向本数", "柱頭 幅止筋X方向本数" };
            internal string[] count_bar_spacing_Y = { "柱脚 幅止筋Y方向本数", "柱頭 幅止筋Y方向本数" };
            internal string[] pitch_bar_spacing = { "柱脚 幅止筋ピッチ", "柱頭 幅止筋ピッチ" };
            internal string[] count_axial = { "柱脚 芯鉄筋本数", "柱頭 芯鉄筋本数" };
            internal string center_reinforcement_start_X = "主筋重心位置（X始）";
            internal string center_reinforcement_end_X = "主筋重心位置（X終）";
            internal string center_reinforcement_start_Y = "主筋重心位置（Y始）";
            internal string center_reinforcement_end_Y = "主筋重心位置（Y終）";
            internal string[] kind_reinforcement_corner = { "柱脚 寄せ筋方向", "柱頭 寄せ筋方向" };
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string strength_reinforcement_band = "帯筋強度";
            internal string strength_bar_spacing = "幅止筋強度";
            internal string interval_reinforcement = "2段筋間隔";
            internal string count_main_total = "主筋総本数";
            internal string count_main_X = "X形主筋X方向本数";
            internal string count_main_Y = "X形主筋Y方向本数";
            internal string count_main_total_X = "X形主筋総本数";
            //断面リスト用
            /// <summary>断面リスト用幅止筋ピッチ
            /// </summary>
            internal string pitch_bar_spacing_list = "幅止筋ピッチ";
            /// <summary>断面リスト用芯鉄筋本数
            /// </summary>
            internal string count_axial_list = "芯鉄筋本数";
            internal string center_reinforcement_X = "躯体面から芯鉄筋X方向までの距離";
            internal string center_reinforcement_Y = "躯体面から芯鉄筋Y方向までの距離";
            internal string concrete_reductionrate = "コンクリート強度の低減率";
            internal string strength_reinforcement_main_Y = "主筋種別Y";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_start_X = "ふかし厚さ（X始）";
            internal string thickness_ex_end_X = "ふかし厚さ（X終）";
            internal string thickness_ex_start_Y = "ふかし厚さ（Y始）";
            internal string thickness_ex_end_Y = "ふかし厚さ（Y終）";
            internal string rotate = "断面回転";
        }

        //RC円柱
        internal class RC_Clm_Ro
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*********************************************************
            internal string FamilyName = "RC円柱";

            //*****タイプパラメータ***********************************************************************
            internal string strength_concrete = "構造マテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string D = "直径";
            internal string name = "符号";
            internal string[] D_reinforcement_main = { "柱脚 主筋径", "柱頭 主筋径" };
            internal string[] count_main = { "柱脚 主筋本数", "柱頭 主筋本数" };
            internal string[] D_reinforcement_band = { "柱脚 帯筋径", "柱頭 帯筋径" };
            internal string[] count_band = { "柱脚 帯筋本数", "柱頭 帯筋本数" };
            internal string[] pitch_band = { "柱脚 帯筋ピッチ", "柱頭 帯筋ピッチ" };
            internal string D_bar_spacing = "幅止筋径";
            internal string[] count_bar_spacing_X = { "柱脚 幅止筋X方向本数", "柱頭 幅止筋X方向本数" };
            internal string[] count_bar_spacing_Y = { "柱脚 幅止筋Y方向本数", "柱頭 幅止筋Y方向本数" };
            internal string[] pitch_bar_spacing = { "柱脚 幅止筋ピッチ", "柱頭 幅止筋ピッチ" };
            internal string depth_cover_X = "かぶり厚さ";
            internal string D_reinforcement_axial = "芯鉄筋径";
            internal string[] count_axial = { "柱脚 芯鉄筋本数", "柱頭 芯鉄筋本数" };
            internal string strength_reinforcement_main = "主筋種別";
            internal string strength_reinforcement_axial = "芯鉄筋種別";
            internal string center_reinforcement_start_X = "主筋重心位置";
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string strength_reinforcement_band = "帯筋強度";
            internal string strength_bar_spacing = "幅止筋強度";

            /// <summary>断面リスト用幅止筋ピッチ
            /// </summary>
            internal string pitch_bar_spacing_list = "幅止筋ピッチ";
            /// <summary>断面リスト用芯鉄筋本数
            /// </summary>
            internal string count_axial_list = "芯鉄筋本数";
            internal string center_reinforcement = "躯体面から芯鉄筋までの距離";
            internal string concrete_reductionrate = "コンクリート強度の低減率";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_start_X = "ふかし厚さ";
        }

        /// <summary>S柱H形鋼
        /// </summary>
        internal class S_Clm_H
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "S柱H形鋼";

            //*****タイプパラメータ***********************************************************************
            internal string strength_web = "ウェブ マテリアル";
            internal string strength_main = "フランジ マテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string B = "柱幅";
            internal string A = "柱せい";
            internal string t1 = "ウェブ厚";
            internal string t2 = "フランジ厚";
            internal string r = "フィレット";
            internal string name = "符号";
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string type_name = "形状名";
            internal string type = "形状タイプ";
            internal string base_type = "柱脚形式";

            //*****インスタンスパラメータ*****************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";           
            internal string condition_bottom = "始端条件";
            internal string condition_top = "終端条件";
            internal string joint_top = "ジョイント位置（上）";
            internal string joint_bottom = "ジョイント位置（下）";
            internal string kind_joint_top = "ジョイント種別（上）";
            internal string kind_joint_bottom = "ジョイント種別（下）";
        }

        /// <summary>S柱組立H形鋼
        /// </summary>
        internal class S_Clm_BH
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "S柱組立H形鋼";

            //*****タイプパラメータ***********************************************************************
            internal string strength_web = "ウェブ マテリアル";
            internal string strength_main = "フランジ マテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string B = "柱幅";
            internal string A = "柱せい";
            internal string t1 = "ウェブ厚";
            internal string t2 = "フランジ厚";
            internal string r = "フィレット";
            internal string name = "符号";
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string type_name = "形状名";
            internal string base_type = "柱脚形式";

            //*****インスタンスパラメータ*****************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";            
            internal string condition_bottom = "始端条件";
            internal string condition_top = "終端条件";
            internal string joint_top = "ジョイント位置（上）";
            internal string joint_bottom = "ジョイント位置（下）";
            internal string kind_joint_top = "ジョイント種別（上）";
            internal string kind_joint_bottom = "ジョイント種別（下）";
        }

        /// <summary>S柱角形鋼管
        /// </summary>
        internal class S_Clm_Box
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "S柱角形鋼管";

            //*****タイプパラメータ***********************************************************************
            internal string strength_main = "構造マテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string B = "柱幅";
            internal string A = "柱せい";
            internal string t1 = "板厚";
            internal string r = "フィレット";
            internal string name = "符号";
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string type_name = "形状名";
            internal string type = "形状タイプ";
            internal string base_type = "柱脚形式";

            //*****インスタンスパラメータ*****************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";          
            internal string condition_bottom = "始端条件";
            internal string condition_top = "終端条件";
            internal string joint_top = "ジョイント位置（上）";
            internal string joint_bottom = "ジョイント位置（下）";
            internal string kind_joint_top = "ジョイント種別（上）";
            internal string kind_joint_bottom = "ジョイント種別（下）";
        }

        /// <summary>S柱組立角形鋼管
        /// </summary>
        internal class S_Clm_BBox
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "S柱組立角形鋼管";

            //*****タイプパラメータ***********************************************************************
            internal string strength_main = "構造マテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string B = "柱幅";
            internal string A = "柱せい";
            internal string size_imput = "BBOX 板厚 別サイズ入力";
            internal string t1 = "せい方向板厚";
            internal string t2 = "幅方向板厚";
            internal string r = "フィレット";
            internal string name = "符号";
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string type_name = "形状名";
            internal string base_type = "柱脚形式";

            //*****インスタンスパラメータ*****************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";            
            internal string condition_bottom = "始端条件";
            internal string condition_top = "終端条件";
            internal string joint_top = "ジョイント位置（上）";
            internal string joint_bottom = "ジョイント位置（下）";
            internal string kind_joint_top = "ジョイント種別（上）";
            internal string kind_joint_bottom = "ジョイント種別（下）";
        }

        /// <summary>S柱鋼管
        /// </summary>
        internal class S_Clm_Pipe
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "S柱鋼管";

            //*****タイプパラメータ***********************************************************************
            internal string strength_main = "構造マテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string D = "直径";
            internal string t = "板厚";
            internal string name = "符号";
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string type_name = "形状名";
            internal string base_type = "柱脚形式";

            //*****インスタンスパラメータ*****************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";            
            internal string condition_bottom = "始端条件";
            internal string condition_top = "終端条件";
            internal string joint_top = "ジョイント位置（上）";
            internal string joint_bottom = "ジョイント位置（下）";
            internal string kind_joint_top = "ジョイント種別（上）";
            internal string kind_joint_bottom = "ジョイント種別（下）";
        }
        
        /// <summary>S柱T形鋼
        /// </summary>
        internal class S_Clm_T
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "S柱T形鋼";

            //*****タイプパラメータ***********************************************************************
            internal string strength_web = "ウェブ マテリアル";
            internal string strength_main = "フランジ マテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string B = "柱幅";
            internal string A = "柱せい";
            internal string t1 = "ウェブ厚";
            internal string t2 = "フランジ厚";
            internal string r = "フィレット";
            internal string name = "符号";
            internal string SecId = "断面ID";
            internal string type_name = "形状名";
            internal string type = "形状タイプ";
            internal string base_type = "柱脚形式";

            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";            
            internal string condition_bottom = "始端条件";
            internal string condition_top = "終端条件";
            internal string joint_top = "ジョイント位置（上）";
            internal string joint_bottom = "ジョイント位置（下）";
            internal string kind_joint_top = "ジョイント種別（上）";
            internal string kind_joint_bottom = "ジョイント種別（下）";
        }

        /// <summary>S柱溝形鋼
        /// </summary>
        internal class S_Clm_C
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "S柱溝形鋼";

            //*****タイプパラメータ***********************************************************************
            internal string strength_main = "構造マテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string B = "柱幅";
            internal string A = "柱せい";
            internal string t1 = "ウェブ厚";
            internal string t2 = "フランジ厚";
            internal string r1 = "フィレット";
            internal string r2 = "先端半径";
            internal string name = "符号";
            internal string SecId = "断面ID";
            internal string side = "背合わせ";
            internal string type_name = "形状名";
            internal string type = "形状タイプ";
            internal string base_type = "柱脚形式";

            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";         
            internal string condition_bottom = "始端条件";
            internal string condition_top = "終端条件";
            internal string joint_top = "ジョイント位置（上）";
            internal string joint_bottom = "ジョイント位置（下）";
            internal string kind_joint_top = "ジョイント種別（上）";
            internal string kind_joint_bottom = "ジョイント種別（下）";
        }

        /// <summary>S柱山形鋼
        /// </summary>
        internal class S_Clm_L
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "S柱山形鋼";

            //*****タイプパラメータ***********************************************************************
            internal string strength_main = "構造マテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string B = "柱幅";
            internal string A = "柱せい";
            internal string t1 = "せい方向板厚";
            internal string t2 = "幅方向板厚";
            internal string r1 = "フィレット";
            internal string r2 = "先端半径";
            internal string name = "符号";
            internal string SecId = "断面ID";
            internal string side = "背合わせ";
            internal string type_name = "形状名";
            internal string type = "形状タイプ";
            internal string base_type = "柱脚形式";

            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";            
            internal string condition_bottom = "始端条件";
            internal string condition_top = "終端条件";
            internal string joint_top = "ジョイント位置（上）";
            internal string joint_bottom = "ジョイント位置（下）";
            internal string kind_joint_top = "ジョイント種別（上）";
            internal string kind_joint_bottom = "ジョイント種別（下）";
        }

        /// <summary>SRC柱H形矩形
        /// </summary>
        internal class SRC_Clm_H
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "SRC柱H形(矩形)";

            //*****タイプパラメータ***********************************************************************
            //RC部
            internal string strength_concrete = "構造マテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string DX = "柱幅";
            internal string DY = "柱せい";
            internal string name = "符号";
            internal string[] D_reinforcement_main = { "柱脚 主筋太径", "柱頭 主筋太径" };
            internal string[] D_reinforcement_2nd_main = { "柱脚 主筋細径", "柱頭 主筋細径" };
            internal string[] D_reinforcement_band = { "柱脚 帯筋径", "柱頭 帯筋径" };
            internal string D_bar_spacing = "幅止筋径";
            internal string strength_reinforcement_main = "主筋種別X";
            internal string strength_reinforcement_2nd_main = "副主筋強度";
            internal string[] depth_cover_X = { "かぶり厚さ（X始）", "かぶり厚さ（X終）" };
            internal string[] depth_cover_Y = { "かぶり厚さ（Y始）", "かぶり厚さ（Y終）" };
            internal string[] count_main_X_1st = { "柱脚 主筋X方向1段太筋本数", "柱頭 主筋X方向1段太筋本数" };
            internal string[] count_2nd_main_X_1st = { "柱脚 主筋X方向1段細筋本数", "柱頭 主筋X方向1段細筋本数" };
            internal string[] count_main_X_2nd = { "柱脚 主筋X方向2段太筋本数", "柱頭 主筋X方向2段太筋本数" };
            internal string[] count_2nd_main_X_2nd = { "柱脚 主筋X方向2段細筋本数", "柱頭 主筋X方向2段細筋本数" };
            internal string[] count_main_Y_1st = { "柱脚 主筋Y方向1段太筋本数", "柱頭 主筋Y方向1段太筋本数" };
            internal string[] count_2nd_main_Y_1st = { "柱脚 主筋Y方向1段細筋本数", "柱頭 主筋Y方向1段細筋本数" };
            internal string[] count_main_Y_2nd = { "柱脚 主筋Y方向2段太筋本数", "柱頭 主筋Y方向2段太筋本数" };
            internal string[] count_2nd_main_Y_2nd = { "柱脚 主筋Y方向2段細筋本数", "柱頭 主筋Y方向2段細筋本数" };
            internal string[] count_band_dir_X = { "柱脚 帯筋X方向本数", "柱頭 帯筋X方向本数" };
            internal string[] count_band_dir_Y = { "柱脚 帯筋Y方向本数", "柱頭 帯筋Y方向本数" };
            internal string[] pitch_band = { "柱脚 帯筋ピッチ", "柱頭 帯筋ピッチ" };
            internal string[] count_bar_spacing_X = { "柱脚 幅止筋X方向本数", "柱頭 幅止筋X方向本数" };
            internal string[] count_bar_spacing_Y = { "柱脚 幅止筋Y方向本数", "柱頭 幅止筋Y方向本数" };
            internal string[] pitch_bar_spacing = { "柱脚 幅止筋ピッチ", "柱頭 幅止筋ピッチ" }; 
            internal string[] kind_reinforcement_corner = { "柱脚 寄せ筋方向", "柱頭 寄せ筋方向" };
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string strength_reinforcement_band = "帯筋強度";
            internal string strength_bar_spacing = "幅止筋強度";
            internal string interval_reinforcement = "2段筋間隔";
            internal string count_main_total = "主筋総本数";
            internal string count_main_X = "X形主筋X方向本数";
            internal string count_main_Y = "X形主筋Y方向本数";
            internal string count_main_total_X = "X形主筋総本数";
            /// <summary>断面リスト用幅止筋ピッチ
            /// </summary>
            internal string pitch_bar_spacing_list = "幅止筋ピッチ";
            internal string concrete_reductionrate = "コンクリート強度の低減率";
            internal string strength_reinforcement_main_Y = "主筋種別Y";

            //S部           
            internal string strength_web = "ウェブ マテリアル";
            internal string strength_main = "フランジ マテリアル";
            internal string type = "形状タイプ";
            internal string typename = "形状名";
            internal string H = "鉄骨せい";
            internal string B = "鉄骨幅";
            internal string t1 = "ウェブ厚";
            internal string t2 = "フランジ厚";
            internal string r = "フィレット";
            internal string direction_type = "鉄骨向き";
            internal string offset_X = "鉄骨偏心X";
            internal string offset_Y = "鉄骨偏心Y";
            internal string angle = "角度"; //鉄骨向きを角度に直したものをタイプパラメータ「角度」に入れるため
            internal string base_type = "柱脚形式";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_start_X = "ふかし厚さ（X始）";
            internal string thickness_ex_end_X = "ふかし厚さ（X終）";
            internal string thickness_ex_start_Y = "ふかし厚さ（Y始）";
            internal string thickness_ex_end_Y = "ふかし厚さ（Y終）";
            internal string condition_bottom = "始端条件";
            internal string condition_top = "終端条件";
            internal string joint_top = "ジョイント位置（上）";
            internal string joint_bottom = "ジョイント位置（下）";
            internal string kind_joint_top = "ジョイント種別（上）";
            internal string kind_joint_bottom = "ジョイント種別（下）";

        }

        /// <summary>SRC柱＋形矩形
        /// </summary>
        internal class SRC_Clm_Cross
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "SRC柱＋形(矩形)";

            //*****タイプパラメータ***********************************************************************
            //RC部
            internal string strength_concrete = "構造マテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string DX = "柱幅";
            internal string DY = "柱せい";
            internal string name = "符号";
            internal string[] D_reinforcement_main = { "柱脚 主筋太径", "柱頭 主筋太径" };
            internal string[] D_reinforcement_2nd_main = { "柱脚 主筋細径", "柱頭 主筋細径" };
            internal string[] D_reinforcement_band = { "柱脚 帯筋径", "柱頭 帯筋径" };
            internal string D_bar_spacing = "幅止筋径";
            internal string strength_reinforcement_main = "主筋種別X";
            internal string strength_reinforcement_2nd_main = "副主筋強度";
            internal string[] depth_cover_X = { "かぶり厚さ（X始）", "かぶり厚さ（X終）" };
            internal string[] depth_cover_Y = { "かぶり厚さ（Y始）", "かぶり厚さ（Y終）" };
            internal string[] count_main_X_1st = { "柱脚 主筋X方向1段太筋本数", "柱頭 主筋X方向1段太筋本数" };
            internal string[] count_2nd_main_X_1st = { "柱脚 主筋X方向1段細筋本数", "柱頭 主筋X方向1段細筋本数" };
            internal string[] count_main_X_2nd = { "柱脚 主筋X方向2段太筋本数", "柱頭 主筋X方向2段太筋本数" };
            internal string[] count_2nd_main_X_2nd = { "柱脚 主筋X方向2段細筋本数", "柱頭 主筋X方向2段細筋本数" };
            internal string[] count_main_Y_1st = { "柱脚 主筋Y方向1段太筋本数", "柱頭 主筋Y方向1段太筋本数" };
            internal string[] count_2nd_main_Y_1st = { "柱脚 主筋Y方向1段細筋本数", "柱頭 主筋Y方向1段細筋本数" };
            internal string[] count_main_Y_2nd = { "柱脚 主筋Y方向2段太筋本数", "柱頭 主筋Y方向2段太筋本数" };
            internal string[] count_2nd_main_Y_2nd = { "柱脚 主筋Y方向2段細筋本数", "柱頭 主筋Y方向2段細筋本数" };
            internal string[] count_band_dir_X = { "柱脚 帯筋X方向本数", "柱頭 帯筋X方向本数" };
            internal string[] count_band_dir_Y = { "柱脚 帯筋Y方向本数", "柱頭 帯筋Y方向本数" };
            internal string[] pitch_band = { "柱脚 帯筋ピッチ", "柱頭 帯筋ピッチ" };
            internal string[] count_bar_spacing_X = { "柱脚 幅止筋X方向本数", "柱頭 幅止筋X方向本数" };
            internal string[] count_bar_spacing_Y = { "柱脚 幅止筋Y方向本数", "柱頭 幅止筋Y方向本数" };
            internal string[] pitch_bar_spacing = { "柱脚 幅止筋ピッチ", "柱頭 幅止筋ピッチ" };          
            internal string[] kind_reinforcement_corner = { "柱脚 寄せ筋方向", "柱頭 寄せ筋方向" };
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string strength_reinforcement_band = "帯筋強度";
            internal string strength_bar_spacing = "幅止筋強度";
            internal string interval_reinforcement = "2段筋間隔";
            internal string count_main_total = "主筋総本数";
            internal string count_main_X = "X形主筋X方向本数";
            internal string count_main_Y = "X形主筋Y方向本数";
            internal string count_main_total_X = "X形主筋総本数";
            /// <summary>断面リスト用幅止筋ピッチ
            /// </summary>
            internal string pitch_bar_spacing_list = "幅止筋ピッチ";
            internal string concrete_reductionrate = "コンクリート強度の低減率";
            internal string strength_reinforcement_main_Y = "主筋種別Y";

            //S部
            internal string strength_main_X = "X方向鉄骨_フランジ マテリアル";
            internal string strength_web_X = "X方向鉄骨_ウェブ マテリアル";
            internal string strength_main_Y = "Y方向鉄骨_フランジ マテリアル";
            internal string strength_web_Y = "Y方向鉄骨_ウェブ マテリアル";
            internal string XH = "X方向鉄骨_柱せい";
            internal string XB = "X方向鉄骨_柱幅";
            internal string Xt1 = "X方向鉄骨_ウェブ厚";
            internal string Xt2 = "X方向鉄骨_フランジ厚";
            internal string Xr = "X方向鉄骨_フィレット";
            internal string YH = "Y方向鉄骨_柱せい";
            internal string YB = "Y方向鉄骨_柱幅";
            internal string Yt1 = "Y方向鉄骨_ウェブ厚";
            internal string Yt2 = "Y方向鉄骨_フランジ厚";
            internal string Yr = "Y方向鉄骨_フィレット";
            internal string offset_XX = "X方向鉄骨_鉄骨偏心X";
            internal string offset_XY = "X方向鉄骨_鉄骨偏心Y";
            internal string offset_YX = "Y方向鉄骨_鉄骨偏心X";
            internal string offset_YY = "Y方向鉄骨_鉄骨偏心Y";
            internal string type_X = "X方向鉄骨_形状タイプ";
            internal string type_Y = "Y方向鉄骨_形状タイプ";
            internal string typename_X = "X方向鉄骨_形状名";
            internal string typename_Y = "Y方向鉄骨_形状名";
            internal string base_type = "柱脚形式";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_start_X = "ふかし厚さ（X始）";
            internal string thickness_ex_end_X = "ふかし厚さ（X終）";
            internal string thickness_ex_start_Y = "ふかし厚さ（Y始）";
            internal string thickness_ex_end_Y = "ふかし厚さ（Y終）";
            internal string condition_bottom = "始端条件";
            internal string condition_top = "終端条件";
            internal string joint_top = "ジョイント位置（上）";
            internal string joint_bottom = "ジョイント位置（下）";
            internal string kind_joint_top = "ジョイント種別（上）";
            internal string kind_joint_bottom = "ジョイント種別（下）";
        }

        /// <summary>SRC柱T形矩形
        /// </summary>
        internal class SRC_Clm_T
        {
            //ロードされているか否か************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*******************************************************************
            internal string FamilyName = "SRC柱T形(矩形)";

            //*****タイプパラメータ***********************************************************************
            //RC部
            internal string strength_concrete = "構造マテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string DX = "柱幅";
            internal string DY = "柱せい";
            internal string name = "符号";
            internal string[] D_reinforcement_main = { "柱脚 主筋太径", "柱頭 主筋太径" };
            internal string[] D_reinforcement_2nd_main = { "柱脚 主筋細径", "柱頭 主筋細径" };
            internal string[] D_reinforcement_band = { "柱脚 帯筋径", "柱頭 帯筋径" };
            internal string D_bar_spacing = "幅止筋径";
            internal string strength_reinforcement_main = "主筋種別X";
            internal string strength_reinforcement_2nd_main = "副主筋強度";
            internal string[] depth_cover_X = { "かぶり厚さ（X始）", "かぶり厚さ（X終）" };
            internal string[] depth_cover_Y = { "かぶり厚さ（Y始）", "かぶり厚さ（Y終）" };
            internal string[] count_main_X_1st = { "柱脚 主筋X方向1段太筋本数", "柱頭 主筋X方向1段太筋本数" };
            internal string[] count_2nd_main_X_1st = { "柱脚 主筋X方向1段細筋本数", "柱頭 主筋X方向1段細筋本数" };
            internal string[] count_main_X_2nd = { "柱脚 主筋X方向2段太筋本数", "柱頭 主筋X方向2段太筋本数" };
            internal string[] count_2nd_main_X_2nd = { "柱脚 主筋X方向2段細筋本数", "柱頭 主筋X方向2段細筋本数" };
            internal string[] count_main_Y_1st = { "柱脚 主筋Y方向1段太筋本数", "柱頭 主筋Y方向1段太筋本数" };
            internal string[] count_2nd_main_Y_1st = { "柱脚 主筋Y方向1段細筋本数", "柱頭 主筋Y方向1段細筋本数" };
            internal string[] count_main_Y_2nd = { "柱脚 主筋Y方向2段太筋本数", "柱頭 主筋Y方向2段太筋本数" };
            internal string[] count_2nd_main_Y_2nd = { "柱脚 主筋Y方向2段細筋本数", "柱頭 主筋Y方向2段細筋本数" };
            internal string[] count_band_dir_X = { "柱脚 帯筋X方向本数", "柱頭 帯筋X方向本数" };
            internal string[] count_band_dir_Y = { "柱脚 帯筋Y方向本数", "柱頭 帯筋Y方向本数" };
            internal string[] pitch_band = { "柱脚 帯筋ピッチ", "柱頭 帯筋ピッチ" };
            internal string[] count_bar_spacing_X = { "柱脚 幅止筋X方向本数", "柱頭 幅止筋X方向本数" };
            internal string[] count_bar_spacing_Y = { "柱脚 幅止筋Y方向本数", "柱頭 幅止筋Y方向本数" };
            internal string[] pitch_bar_spacing = { "柱脚 幅止筋ピッチ", "柱頭 幅止筋ピッチ" };
            internal string[] kind_reinforcement_corner = { "柱脚 寄せ筋方向", "柱頭 寄せ筋方向" };
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string strength_reinforcement_band = "帯筋強度";
            internal string strength_bar_spacing = "幅止筋強度";
            internal string interval_reinforcement = "2段筋間隔";
            internal string count_main_total = "主筋総本数";
            internal string count_main_X = "X形主筋X方向本数";
            internal string count_main_Y = "X形主筋Y方向本数";
            internal string count_main_total_X = "X形主筋総本数";
            /// <summary>断面リスト用幅止筋ピッチ
            /// </summary>
            internal string pitch_bar_spacing_list = "幅止筋ピッチ";
            internal string concrete_reductionrate = "コンクリート強度の低減率";
            internal string strength_reinforcement_main_Y = "主筋種別Y";

            //S部
            internal string strength_main_T = "T形鋼_フランジ マテリアル";
            internal string strength_web_T = "T形鋼_ウェブ マテリアル";
            internal string strength_main_H = "H形鋼_フランジ マテリアル";
            internal string strength_web_H = "H形鋼_ウェブ マテリアル";
            internal string direction_type = "鉄骨向き";
            internal string H = "H形鋼_柱せい";
            internal string B = "H形鋼_柱幅";
            internal string t1 = "H形鋼_ウェブ厚";
            internal string t2 = "H形鋼_フランジ厚";
            internal string r = "H形鋼_フィレット";
            internal string CT_A = "T形鋼_柱せい";
            internal string CT_B = "T形鋼_柱幅";
            internal string CT_t1 = "T形鋼_ウェブ厚";
            internal string CT_t2 = "T形鋼_フランジ厚";
            internal string CT_r = "T形鋼_フィレット";
            internal string offset_HX = "H形鋼_鉄骨偏心X";
            internal string offset_HY = "H形鋼_鉄骨偏心Y";
            internal string offset_T = "T形鋼_鉄骨偏心";
            internal string angle = "角度";
            internal string type_H = "H形鋼_形状タイプ";
            internal string type_T = "T形鋼_形状タイプ";
            internal string typename_H = "H形鋼_形状名";
            internal string typename_T = "T形鋼_形状名";
            internal string base_type = "柱脚形式";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_start_X = "ふかし厚さ（X始）";
            internal string thickness_ex_end_X = "ふかし厚さ（X終）";
            internal string thickness_ex_start_Y = "ふかし厚さ（Y始）";
            internal string thickness_ex_end_Y = "ふかし厚さ（Y終）";
            internal string condition_bottom = "始端条件";
            internal string condition_top = "終端条件";
            internal string joint_top = "ジョイント位置（上）";
            internal string joint_bottom = "ジョイント位置（下）";
            internal string kind_joint_top = "ジョイント種別（上）";
            internal string kind_joint_bottom = "ジョイント種別（下）";
        }

        /// <summary>SRC柱H形円形
        /// </summary>
        internal class SRC_Clm_H_Rou
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "SRC柱H形(円形)";

            //*****タイプパラメータ***********************************************************************
            //RC部
            internal string strength_concrete = "構造マテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string D = "直径";
            internal string name = "符号";
            internal string[] D_reinforcement_main = { "柱脚 主筋径", "柱頭 主筋径" };
            internal string[] count_main = { "柱脚 主筋本数", "柱頭 主筋本数" };
            internal string[] D_reinforcement_band = { "柱脚 帯筋径", "柱頭 帯筋径" };
            internal string[] count_band = { "柱脚 帯筋本数", "柱頭 帯筋本数" };
            internal string[] pitch_band = { "柱脚 帯筋ピッチ", "柱頭 帯筋ピッチ" };
            internal string D_bar_spacing = "幅止筋径";
            internal string[] count_bar_spacing_X = { "柱脚 幅止筋X方向本数", "柱頭 幅止筋X方向本数" };
            internal string[] count_bar_spacing_Y = { "柱脚 幅止筋Y方向本数", "柱頭 幅止筋Y方向本数" };
            internal string[] pitch_bar_spacing = { "柱脚 幅止筋ピッチ", "柱頭 幅止筋ピッチ" };
            internal string depth_cover_X = "かぶり厚さ";
            internal string strength_reinforcement_main = "主筋種別";
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string strength_reinforcement_band = "帯筋強度";
            internal string strength_bar_spacing = "幅止筋強度";
            /// <summary>断面リスト用幅止筋ピッチ
            /// </summary>
            internal string pitch_bar_spacing_list = "幅止筋ピッチ";
            internal string concrete_reductionrate = "コンクリート強度の低減率";
            //S部           
            internal string strength_web = "ウェブ マテリアル";
            internal string strength_main = "フランジ マテリアル";
            internal string type = "形状タイプ";
            internal string typename = "形状名";
            internal string H = "鉄骨せい";
            internal string B = "鉄骨幅";
            internal string t1 = "ウェブ厚";
            internal string t2 = "フランジ厚";
            internal string r = "フィレット";
            internal string direction_type = "鉄骨向き";
            internal string offset_X = "鉄骨偏心X";
            internal string offset_Y = "鉄骨偏心Y";
            internal string angle = "角度"; //鉄骨向きを角度に直したものをタイプパラメータ「角度」に入れるため
            internal string base_type = "柱脚形式";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_start_X = "ふかし厚さ";
            internal string condition_bottom = "始端条件";
            internal string condition_top = "終端条件";
            internal string joint_top = "ジョイント位置（上）";
            internal string joint_bottom = "ジョイント位置（下）";
            internal string kind_joint_top = "ジョイント種別（上）";
            internal string kind_joint_bottom = "ジョイント種別（下）";

        }

        /// <summary>SRC柱＋形円形
        /// </summary>
        internal class SRC_Clm_Cross_Rou
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "SRC柱＋形(円形)";

            //*****タイプパラメータ***********************************************************************
            //RC部
            internal string strength_concrete = "構造マテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string D = "直径";
            internal string name = "符号";
            internal string[] D_reinforcement_main = { "柱脚 主筋径", "柱頭 主筋径" };
            internal string[] count_main = { "柱脚 主筋本数", "柱頭 主筋本数" };
            internal string[] D_reinforcement_band = { "柱脚 帯筋径", "柱頭 帯筋径" };
            internal string[] count_band = { "柱脚 帯筋本数", "柱頭 帯筋本数" };
            internal string[] pitch_band = { "柱脚 帯筋ピッチ", "柱頭 帯筋ピッチ" };
            internal string D_bar_spacing = "幅止筋径";
            internal string[] count_bar_spacing_X = { "柱脚 幅止筋X方向本数", "柱頭 幅止筋X方向本数" };
            internal string[] count_bar_spacing_Y = { "柱脚 幅止筋Y方向本数", "柱頭 幅止筋Y方向本数" };
            internal string[] pitch_bar_spacing = { "柱脚 幅止筋ピッチ", "柱頭 幅止筋ピッチ" };
            internal string depth_cover_X = "かぶり厚さ";
            internal string strength_reinforcement_main = "主筋種別";
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string strength_reinforcement_band = "帯筋強度";
            internal string strength_bar_spacing = "幅止筋強度";
            /// <summary>断面リスト用幅止筋ピッチ
            /// </summary>
            internal string pitch_bar_spacing_list = "幅止筋ピッチ";
            internal string concrete_reductionrate = "コンクリート強度の低減率";

            //S部
            internal string strength_main_X = "X方向鉄骨_フランジ マテリアル";
            internal string strength_web_X = "X方向鉄骨_ウェブ マテリアル";
            internal string strength_main_Y = "Y方向鉄骨_フランジ マテリアル";
            internal string strength_web_Y = "Y方向鉄骨_ウェブ マテリアル";
            internal string XH = "X方向鉄骨_柱せい";
            internal string XB = "X方向鉄骨_柱幅";
            internal string Xt1 = "X方向鉄骨_ウェブ厚";
            internal string Xt2 = "X方向鉄骨_フランジ厚";
            internal string Xr = "X方向鉄骨_フィレット";
            internal string YH = "Y方向鉄骨_柱せい";
            internal string YB = "Y方向鉄骨_柱幅";
            internal string Yt1 = "Y方向鉄骨_ウェブ厚";
            internal string Yt2 = "Y方向鉄骨_フランジ厚";
            internal string Yr = "Y方向鉄骨_フィレット";
            internal string offset_XX = "X方向鉄骨_鉄骨偏心X";
            internal string offset_XY = "X方向鉄骨_鉄骨偏心Y";
            internal string offset_YX = "Y方向鉄骨_鉄骨偏心X";
            internal string offset_YY = "Y方向鉄骨_鉄骨偏心Y";
            internal string type_X = "X方向鉄骨_形状タイプ";
            internal string type_Y = "Y方向鉄骨_形状タイプ";
            internal string typename_X = "X方向鉄骨_形状名";
            internal string typename_Y = "Y方向鉄骨_形状名";
            internal string base_type = "柱脚形式";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_start_X = "ふかし厚さ";
            internal string condition_bottom = "始端条件";
            internal string condition_top = "終端条件";
            internal string joint_top = "ジョイント位置（上）";
            internal string joint_bottom = "ジョイント位置（下）";
            internal string kind_joint_top = "ジョイント種別（上）";
            internal string kind_joint_bottom = "ジョイント種別（下）";
        }

        /// <summary>SRC柱T形円形
        /// </summary>
        internal class SRC_Clm_T_Rou
        {
            //ロードされているか否か************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*******************************************************************
            internal string FamilyName = "SRC柱T形(円形)";

            //*****タイプパラメータ***********************************************************************
            //RC部
            internal string strength_concrete = "構造マテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string D = "直径";
            internal string name = "符号";
            internal string[] D_reinforcement_main = { "柱脚 主筋径", "柱頭 主筋径" };
            internal string[] count_main = { "柱脚 主筋本数", "柱頭 主筋本数" };
            internal string[] D_reinforcement_band = { "柱脚 帯筋径", "柱頭 帯筋径" };
            internal string[] count_band = { "柱脚 帯筋本数", "柱頭 帯筋本数" };
            internal string[] pitch_band = { "柱脚 帯筋ピッチ", "柱頭 帯筋ピッチ" };
            internal string D_bar_spacing = "幅止筋径";
            internal string[] count_bar_spacing_X = { "柱脚 幅止筋X方向本数", "柱頭 幅止筋X方向本数" };
            internal string[] count_bar_spacing_Y = { "柱脚 幅止筋Y方向本数", "柱頭 幅止筋Y方向本数" };
            internal string[] pitch_bar_spacing = { "柱脚 幅止筋ピッチ", "柱頭 幅止筋ピッチ" };
            internal string depth_cover_X = "かぶり厚さ";
            internal string strength_reinforcement_main = "主筋種別";
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string strength_reinforcement_band = "帯筋強度";
            internal string strength_bar_spacing = "幅止筋強度";
            /// <summary>断面リスト用幅止筋ピッチ
            /// </summary>
            internal string pitch_bar_spacing_list = "幅止筋ピッチ";
            internal string concrete_reductionrate = "コンクリート強度の低減率";

            //S部
            internal string strength_main_T = "T形鋼_フランジ マテリアル";
            internal string strength_web_T = "T形鋼_ウェブ マテリアル";
            internal string strength_main_H = "H形鋼_フランジ マテリアル";
            internal string strength_web_H = "H形鋼_ウェブ マテリアル";
            internal string direction_type = "鉄骨向き";
            internal string H = "H形鋼_柱せい";
            internal string B = "H形鋼_柱幅";
            internal string t1 = "H形鋼_ウェブ厚";
            internal string t2 = "H形鋼_フランジ厚";
            internal string r = "H形鋼_フィレット";
            internal string CT_A = "T形鋼_柱せい";
            internal string CT_B = "T形鋼_柱幅";
            internal string CT_t1 = "T形鋼_ウェブ厚";
            internal string CT_t2 = "T形鋼_フランジ厚";
            internal string CT_r = "T形鋼_フィレット";
            internal string offset_HX = "H形鋼_鉄骨偏心X";
            internal string offset_HY = "H形鋼_鉄骨偏心Y";
            internal string offset_T = "T形鋼_鉄骨偏心";
            internal string angle = "角度";
            internal string type_H = "H形鋼_形状タイプ";
            internal string type_T = "T形鋼_形状タイプ";
            internal string typename_H = "H形鋼_形状名";
            internal string typename_T = "T形鋼_形状名";
            internal string base_type = "柱脚形式";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_start_X = "ふかし厚さ";
            internal string condition_bottom = "始端条件";
            internal string condition_top = "終端条件";
            internal string joint_top = "ジョイント位置（上）";
            internal string joint_bottom = "ジョイント位置（下）";
            internal string kind_joint_top = "ジョイント種別（上）";
            internal string kind_joint_bottom = "ジョイント種別（下）";
        }

        /// <summary>CFT柱角形鋼管
        /// </summary>
        internal class CFT_Clm_Box
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "CFT柱角形鋼管";

            //*****タイプパラメータ***********************************************************************
            internal string strength_main = "構造マテリアル";
            internal string strength_concrete = "コンクリートマテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string direction_type = "鉄骨向き";
            internal string type = "形状タイプ";
            internal string typename = "形状名";
            internal string B = "柱幅";
            internal string A = "柱せい";
            internal string t = "板厚";
            internal string r1 = "フィレット";
            internal string name = "符号";
            internal string SecId = "断面ID";
            internal string base_type = "柱脚形式";
            internal string enbedded_length = "柱脚埋め込み長さ";

            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";            
            internal string condition_bottom = "始端条件";
            internal string condition_top = "終端条件";
            internal string joint_top = "ジョイント位置（上）";
            internal string joint_bottom = "ジョイント位置（下）";
            internal string kind_joint_top = "ジョイント種別（上）";
            internal string kind_joint_bottom = "ジョイント種別（下）";
        }

        /// <summary>CFT柱円形鋼管
        /// </summary>
        internal class CFT_Clm_Pipe
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "CFT柱鋼管";

            //*****タイプパラメータ***********************************************************************
            internal string strength_main = "構造マテリアル";
            internal string strength_concrete = "コンクリートマテリアル";
            internal string kind_column = "柱種別";
            internal string kind_column2 = "柱の種別";
            internal string typename = "形状名";
            internal string D = "直径";
            internal string t = "板厚";
            internal string name = "符号";
            internal string SecId = "断面ID";
            internal string base_type = "柱脚形式";
            internal string enbedded_length = "柱脚埋め込み長さ";

            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_bottom = "始端条件";
            internal string condition_top = "終端条件";
            internal string joint_top = "ジョイント位置（上）";
            internal string joint_bottom = "ジョイント位置（下）";
            internal string kind_joint_top = "ジョイント種別（上）";
            internal string kind_joint_bottom = "ジョイント種別（下）";
        }
        #endregion

        #region 梁
        internal class GirFamilyName
        {
            /// <summary>プロジェクトにロードされている⇒true
            /// </summary>
            internal bool[][] flg = { new bool[] { false, false, false, false } ,
                                      new bool[] { false,false, false, false, false, false } ,
                                      new bool[] { false } };
            /// <summary>変換対象⇒true
            /// </summary>
            internal bool[][] convflg = { new bool[] { false, false, false, false } ,
                                      new bool[] { false,false, false, false, false, false } ,
                                      new bool[] { false } };

            internal string[][] TypeName = { new string[] { "", "","",""},
                                         new string[] { "","","","","","" },
                                         new string[] { ""} };
            internal string[][] FamilyName = { new string[] { "","","","" },
                                         new string[] { "","","","","" ,""},
                                         new string[] { "" } };
        }
        /// <summary>RC梁
        /// </summary>
        internal class RC_Gir
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "RC大梁";

            //*****タイプパラメータ***********************************************************************
            internal string strength_concrete = "構造マテリアル";
            internal string kind_beam = "梁種別";
            internal string kind_beam2 = "梁の種別";
            internal string width_start = "始端 梁幅";
            internal string width_center = "中央 梁幅";
            internal string width_end = "終端 梁幅";
            internal string depth_start = "始端 梁せい";
            internal string depth_center = "中央 梁せい";
            internal string depth_end = "終端 梁せい";
            internal string[] BHaunch = { "ボックスハンチ 始端", "ボックスハンチ 終端" };
            internal string haunch_start = "始端 ハンチ長さ";
            internal string haunch_end = "終端 ハンチ長さ";
            internal string name = "符号";
            internal string[] D_reinforcement_main_top = { "始端 上主筋 太径", "中央 上主筋 太径", "終端 上主筋 太径"};
            internal string[] D_reinforcement_main_bottom = { "始端 下主筋 太径", "中央 下主筋 太径", "終端 下主筋 太径" };
            internal string[] D_reinforcement_2nd_main_top = { "始端 上主筋 細径", "中央 上主筋 細径", "終端 上主筋 細径" };
            internal string[] D_reinforcement_2nd_main_bottom = { "始端 下主筋 細径", "中央 下主筋 細径", "終端 下主筋 細径" };
            internal string[] count_main_top_1st = { "始端 上主筋 1段筋太筋本数", "中央 上主筋 1段筋太筋本数", "終端 上主筋 1段筋太筋本数" };
            internal string[] count_main_top_2nd = { "始端 上主筋 2段筋太筋本数", "中央 上主筋 2段筋太筋本数", "終端 上主筋 2段筋太筋本数" };
            internal string[] count_main_top_3rd = { "始端 上主筋 3段筋太筋本数", "中央 上主筋 3段筋太筋本数", "終端 上主筋 3段筋太筋本数" };
            internal string[] count_main_bottom_1st = { "始端 下主筋 1段筋太筋本数", "中央 下主筋 1段筋太筋本数", "終端 下主筋 1段筋太筋本数" };
            internal string[] count_main_bottom_2nd = { "始端 下主筋 2段筋太筋本数", "中央 下主筋 2段筋太筋本数", "終端 下主筋 2段筋太筋本数" };
            internal string[] count_main_bottom_3rd = { "始端 下主筋 3段筋太筋本数", "中央 下主筋 3段筋太筋本数", "終端 下主筋 3段筋太筋本数" };
            internal string[] count_2nd_main_top_1st = { "始端 上主筋 1段筋細筋本数", "中央 上主筋 1段筋細筋本数", "終端 上主筋 1段筋細筋本数" };
            internal string[] count_2nd_main_top_2nd = { "始端 上主筋 2段筋細筋本数", "中央 上主筋 2段筋細筋本数", "終端 上主筋 2段筋細筋本数" };
            internal string[] count_2nd_main_top_3rd = { "始端 上主筋 3段筋細筋本数", "中央 上主筋 3段筋細筋本数", "終端 上主筋 3段筋細筋本数" };
            internal string[] count_2nd_main_bottom_1st = { "始端 下主筋 1段筋細筋本数", "中央 下主筋 1段筋細筋本数", "終端 下主筋 1段筋細筋本数" };
            internal string[] count_2nd_main_bottom_2nd = { "始端 下主筋 2段筋細筋本数", "中央 下主筋 2段筋細筋本数", "終端 下主筋 2段筋細筋本数" };
            internal string[] count_2nd_main_bottom_3rd = { "始端 下主筋 3段筋細筋本数", "中央 下主筋 3段筋細筋本数", "終端 下主筋 3段筋細筋本数" };
            internal string[] D_stirrup = { "始端 肋筋径", "中央 肋筋径", "終端 肋筋径" };
            internal string[] count_stirrup = { "始端 肋筋本数", "中央 肋筋本数", "終端 肋筋本数" };
            internal string[] pitch_stirrup = { "始端 肋筋ピッチ", "中央 肋筋ピッチ", "終端 肋筋ピッチ" };
            internal string[] D_reinforcement_web = { "始端 腹筋径", "中央 腹筋径", "終端 腹筋径" };
            internal string[] count_web = { "始端 腹筋本数", "中央 腹筋本数", "終端 腹筋本数" };
            internal string[] D_bar_spacing = { "始端 幅止筋径", "中央 幅止筋径", "終端 幅止筋径" };
            internal string[] count_bar_spacing = { "始端 幅止筋本数", "中央 幅止筋本数", "終端 幅止筋本数" };
            internal string[] pitch_bar_spacing = { "始端 幅止筋ピッチ", "中央 幅止筋ピッチ", "終端 幅止筋ピッチ" };
            internal string strength_reinforcement_main = "主筋種別";
            internal string strength_reinforcement_2nd_main = "副主筋強度";
            internal string strength_stirrup = "肋筋強度";
            internal string strength_reinforcement_web = "腹筋強度";
            internal string strength_bar_spacing = "幅止筋強度";
            internal string depth_cover_left = "かぶり厚さ（左）";
            internal string depth_cover_right = "かぶり厚さ（右）";
            internal string depth_cover_top = "かぶり厚さ（上）";
            internal string depth_cover_bottom = "かぶり厚さ（下）";
            internal string interval_reinforcement = "2段筋間隔";
            internal string count_X_main_top = "X形主筋本数（上）";
            internal string count_X_main_bottom = "X形主筋本数（下）";
            internal string center_reinforcement_top = "主筋重心位置（上）";
            internal string center_reinforcement_bottom = "主筋重心位置（下）";
            internal string bar_length_start = "始端側カットオフ筋長さ";
            internal string bar_length_end = "終端側カットオフ筋長さ";
            internal string isOutIn = "外端内端指定";
            internal string SecId = "断面ID";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_top = "ふかし厚さ（上）";
            internal string thickness_ex_bottom = "ふかし厚さ（下）";
            internal string thickness_ex_right = "ふかし厚さ（右）";
            internal string thickness_ex_left = "ふかし厚さ（左）";
            internal string kind_haunch_start = "ハンチ種類（始端）";
            internal string kind_haunch_end = "ハンチ種類（終端）";
            internal string type_haunch_H = "水平ハンチ形状";
            internal string type_haunch_V = "鉛直ハンチ形状";
        }

        internal class S_Gir_H
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "S大梁";

            //*****タイプパラメータ***********************************************************************
            internal string name = "符号";
            internal string[] strength_web = { "始端 ウェブ マテリアル", "中央 ウェブ マテリアル", "終端 ウェブ マテリアル" };
            internal string[] strength_main = { "始端 フランジ マテリアル", "中央 フランジ マテリアル", "終端 フランジ マテリアル" };
            internal string kind_beam = "梁種別";
            internal string kind_beam2 = "梁の種別";
            internal string isOutIn = "外端内端指定";
            internal string[] A = { "始端 梁せい", "中央 梁せい", "終端 梁せい" };
            internal string[] B = { "始端 梁幅", "中央 梁幅", "終端 梁幅" };
            internal string[] t1 = { "始端 ウェブ厚", "中央 ウェブ厚", "終端 ウェブ厚" };
            internal string[] t2 = { "始端 フランジ厚", "中央 フランジ厚", "終端 フランジ厚" };
            internal string[] r = { "始端 フィレット", "中央 フィレット", "終端 フィレット" };
            internal string haunch_start = "始端 ハンチ長さ";
            internal string haunch_end = "終端 ハンチ長さ";
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string[] shape = { "始端 形状名", "中央 形状名", "終端 形状名" };
            internal string[] type = { "始端 形状タイプ", "中央 形状タイプ", "終端 形状タイプ" };
            internal string kind_brace = "ブレースの種別";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string kind_haunch_start = "ハンチ種類（始端）";
            internal string kind_haunch_end = "ハンチ種類（終端）";
            internal string type_haunch_H = "水平ハンチ形状";
            internal string type_haunch_V = "鉛直ハンチ形状";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
            internal string future_brace = "ブレース特性";
        }

        /// <summary>
        /// S梁組立H形鋼
        /// </summary>
        internal class S_Gir_BH
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "S梁組立H形鋼";

            //*****タイプパラメータ***********************************************************************
            internal string name = "符号";
            internal string[] strength_web = { "始端 ウェブ マテリアル", "中央 ウェブ マテリアル", "終端 ウェブ マテリアル" };
            internal string[] strength_main = { "始端 フランジ マテリアル", "中央 フランジ マテリアル", "終端 フランジ マテリアル" };
            internal string kind_beam = "梁種別";
            internal string kind_beam2 = "梁の種別";
            internal string isOutIn = "外端内端指定";
            internal string[] A = { "始端 梁せい", "中央 梁せい", "終端 梁せい" };
            internal string[] B = { "始端 梁幅", "中央 梁幅", "終端 梁幅" };
            internal string[] t1 = { "始端 ウェブ厚", "中央 ウェブ厚", "終端 ウェブ厚" };
            internal string[] t2 = { "始端 フランジ厚", "中央 フランジ厚", "終端 フランジ厚" };
            internal string[] r = { "始端 フィレット", "中央 フィレット", "終端 フィレット" };
            internal string haunch_start = "始端 ハンチ長さ";
            internal string haunch_end = "終端 ハンチ長さ";
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string[] shape = { "始端 形状名", "中央 形状名", "終端 形状名" };
            internal string kind_brace = "ブレースの種別";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string kind_haunch_start = "ハンチ種類（始端）";
            internal string kind_haunch_end = "ハンチ種類（終端）";
            internal string type_haunch_H = "水平ハンチ形状";
            internal string type_haunch_V = "鉛直ハンチ形状";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
            internal string future_brace = "ブレース特性";


            /// <summary>
            /// cast (S_Gir_H)
            /// </summary>
            /// <param name="a"></param>
            public static implicit operator S_Gir_H(S_Gir_BH a)
            {
                S_Gir_H g = new S_Gir_H()
                {
                    Loadflg = a.Loadflg,
                    FamilyName = a.FamilyName,
                    name = a.name,
                    strength_web = a.strength_web,
                    strength_main = a.strength_main,
                    kind_beam = a.kind_beam,
                    kind_beam2 = a.kind_beam2,
                    isOutIn = a.isOutIn,
                    A = a.A,
                    B = a.B,
                    t1 = a.t1,
                    t2 = a.t2,
                    r = a.r,
                    haunch_start = a.haunch_start,
                    haunch_end = a.haunch_end,
                    SecId = a.SecId,
                    shape = a.shape,
                    kind_brace = a.kind_brace,
                    MemId = a.MemId,
                    NameMembers = a.NameMembers,
                    condition_start = a.condition_start,
                    condition_end = a.condition_end,
                    kind_haunch_start = a.kind_haunch_start,
                    kind_haunch_end = a.kind_haunch_end,
                    type_haunch_H = a.type_haunch_H,
                    type_haunch_V = a.type_haunch_V,
                    joint_start = a.joint_start,
                    joint_end = a.joint_end,
                    kind_joint_start = a.kind_joint_start,
                    kind_joint_end = a.kind_joint_end,
                    future_brace = a.future_brace,

                    type = null,
                };

                return g;
            }
        }
        //S梁溝形鋼
        internal class S_Gir_C
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "S梁溝形鋼";

            //*****タイプパラメータ***********************************************************************
            internal string name = "符号";
            internal string strength = "構造マテリアル";
            internal string kind_beam = "梁種別";
            internal string kind_beam2 = "梁の種別";
            internal string isOutIn = "外端内端指定";            
            internal string[] H = { "始端 梁せい", "中央 梁せい" , "終端 梁せい"};
            internal string[] B = { "始端 梁幅", "中央 梁幅", "終端 梁幅" };
            internal string[] t1 = { "始端 ウェブ厚", "中央 ウェブ厚", "終端 ウェブ厚" };
            internal string[] t2 = { "始端 フランジ厚", "中央 フランジ厚", "終端 フランジ厚" };
            internal string[] r1 = { "始端 フィレット", "中央 フィレット", "終端 フィレット" };
            internal string[] r2 = { "始端 先端半径", "中央 先端半径", "終端 先端半径" };
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string[] shape = { "始端 形状名", "中央 形状名", "終端 形状名" };
            internal string[] type = { "始端 形状タイプ", "中央 形状タイプ", "終端 形状タイプ" };
            internal string[] side = { "始端 背合わせ", "中央 背合わせ", "終端 背合わせ" };
            internal string haunch_start = "始端 ハンチ長さ";
            internal string haunch_end = "終端 ハンチ長さ";
            internal string kind_brace = "ブレースの種別";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string kind_haunch_start = "ハンチ種類（始端）";
            internal string kind_haunch_end = "ハンチ種類（終端）";
            internal string type_haunch_H = "水平ハンチ形状";
            internal string type_haunch_V = "鉛直ハンチ形状";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
            internal string future_brace = "ブレース特性";
        }
        //リップ溝形鋼
        internal class S_Gir_LipC
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "S梁リップ溝形鋼";

            //*****タイプパラメータ***********************************************************************
            internal string name = "符号";
            internal string strength = "構造マテリアル";
            internal string kind_beam = "梁種別";
            internal string kind_beam2 = "梁の種別";
            internal string isOutIn = "外端内端指定";
            internal string[] H = { "始端 梁せい", "中央 梁せい", "終端 梁せい" };
            internal string[] A = { "始端 梁幅", "中央 梁幅", "終端 梁幅" };
            internal string[] C = { "始端 リップ長", "中央 リップ長", "終端 リップ長" };
            internal string[] t = { "始端 板厚", "中央 板厚", "終端 板厚" };
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string[] shape = { "始端 形状名", "中央 形状名", "終端 形状名" };
            internal string[] type = { "始端 形状タイプ", "中央 形状タイプ", "終端 形状タイプ" };
            internal string[] side = { "始端 背合わせ", "中央 背合わせ", "終端 背合わせ" };
            internal string haunch_start = "始端 ハンチ長さ";
            internal string haunch_end = "終端 ハンチ長さ";
            internal string kind_brace = "ブレースの種別";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string kind_haunch_start = "ハンチ種類（始端）";
            internal string kind_haunch_end = "ハンチ種類（終端）";
            internal string type_haunch_H = "水平ハンチ形状";
            internal string type_haunch_V = "鉛直ハンチ形状";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
            internal string future_brace = "ブレース特性";
        }
        //S梁山形鋼
        internal class S_Gir_L
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "S梁山形鋼";

            //*****タイプパラメータ***********************************************************************
            internal string name = "符号";
            internal string strength = "構造マテリアル";
            internal string kind_beam = "梁種別";
            internal string kind_beam2 = "梁の種別";
            internal string isOutIn = "外端内端指定";
            internal string[] A = { "始端 梁せい", "中央 梁せい", "終端 梁せい" };
            internal string[] B = { "始端 梁幅", "中央 梁幅", "終端 梁幅" };
            internal string[] t1 = { "始端 せい方向板厚", "中央 せい方向板厚", "終端 せい方向板厚" };
            internal string[] t2 = { "始端 幅方向板厚", "中央 幅方向板厚", "終端 幅方向板厚" };
            internal string[] r1 = { "始端 フィレット", "中央 フィレット", "終端 フィレット" };
            internal string[] r2 = { "始端 先端半径", "中央 先端半径", "終端 先端半径" };
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string[] shape = { "始端 形状名", "中央 形状名", "終端 形状名" };
            internal string[] type = { "始端 形状タイプ", "中央 形状タイプ", "終端 形状タイプ" };
            internal string[] side = { "始端 背合わせ", "中央 背合わせ", "終端 背合わせ" };
            internal string haunch_start = "始端 ハンチ長さ";
            internal string haunch_end = "終端 ハンチ長さ";
            internal string kind_brace = "ブレースの種別";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string kind_haunch_start = "ハンチ種類（始端）";
            internal string kind_haunch_end = "ハンチ種類（終端）";
            internal string type_haunch_H = "水平ハンチ形状";
            internal string type_haunch_V = "鉛直ハンチ形状";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
            internal string future_brace = "ブレース特性";
        }
        internal class SRC_Gir
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "SRC大梁";

            //*****タイプパラメータ***********************************************************************
            //RC部
            internal string strength_concrete = "構造マテリアル";
            internal string kind_beam = "梁種別";
            internal string kind_beam2 = "梁の種別";
            internal string isOutIn = "外端内端指定";
            internal string width_start = "始端 梁幅";
            internal string width_center = "中央 梁幅";
            internal string width_end = "終端 梁幅";
            internal string depth_start = "始端 梁せい";
            internal string depth_center = "中央 梁せい";
            internal string depth_end = "終端 梁せい";
            internal string[] BHaunch = { "ボックスハンチ 始端", "ボックスハンチ 終端" };
            internal string haunch_start = "始端 ハンチ長さ";
            internal string haunch_end = "終端 ハンチ長さ";
            internal string name = "符号";
            internal string[] D_reinforcement_main_top = { "始端 上主筋 太径", "中央 上主筋 太径", "終端 上主筋 太径" };
            internal string[] D_reinforcement_main_bottom = { "始端 下主筋 太径", "中央 下主筋 太径", "終端 下主筋 太径" };
            internal string[] D_reinforcement_2nd_main_top = { "始端 上主筋 細径", "中央 上主筋 細径", "終端 上主筋 細径" };
            internal string[] D_reinforcement_2nd_main_bottom = { "始端 下主筋 細径", "中央 下主筋 細径", "終端 下主筋 細径" };
            internal string[] count_main_top_1st = { "始端 上主筋 1段筋太筋本数", "中央 上主筋 1段筋太筋本数", "終端 上主筋 1段筋太筋本数" };
            internal string[] count_main_top_2nd = { "始端 上主筋 2段筋太筋本数", "中央 上主筋 2段筋太筋本数", "終端 上主筋 2段筋太筋本数" };
            internal string[] count_main_top_3rd = { "始端 上主筋 3段筋太筋本数", "中央 上主筋 3段筋太筋本数", "終端 上主筋 3段筋太筋本数" };
            internal string[] count_main_bottom_1st = { "始端 下主筋 1段筋太筋本数", "中央 下主筋 1段筋太筋本数", "終端 下主筋 1段筋太筋本数" };
            internal string[] count_main_bottom_2nd = { "始端 下主筋 2段筋太筋本数", "中央 下主筋 2段筋太筋本数", "終端 下主筋 2段筋太筋本数" };
            internal string[] count_main_bottom_3rd = { "始端 下主筋 3段筋太筋本数", "中央 下主筋 3段筋太筋本数", "終端 下主筋 3段筋太筋本数" };
            internal string[] count_2nd_main_top_1st = { "始端 上主筋 1段筋細筋本数", "中央 上主筋 1段筋細筋本数", "終端 上主筋 1段筋細筋本数" };
            internal string[] count_2nd_main_top_2nd = { "始端 上主筋 2段筋細筋本数", "中央 上主筋 2段筋細筋本数", "終端 上主筋 2段筋細筋本数" };
            internal string[] count_2nd_main_top_3rd = { "始端 上主筋 3段筋細筋本数", "中央 上主筋 3段筋細筋本数", "終端 上主筋 3段筋細筋本数" };
            internal string[] count_2nd_main_bottom_1st = { "始端 下主筋 1段筋細筋本数", "中央 下主筋 1段筋細筋本数", "終端 下主筋 1段筋細筋本数" };
            internal string[] count_2nd_main_bottom_2nd = { "始端 下主筋 2段筋細筋本数", "中央 下主筋 2段筋細筋本数", "終端 下主筋 2段筋細筋本数" };
            internal string[] count_2nd_main_bottom_3rd = { "始端 下主筋 3段筋細筋本数", "中央 下主筋 3段筋細筋本数", "終端 下主筋 3段筋細筋本数" };
            internal string[] D_stirrup = { "始端 肋筋径", "中央 肋筋径", "終端 肋筋径" };
            internal string[] count_stirrup = { "始端 肋筋本数", "中央 肋筋本数", "終端 肋筋本数" };
            internal string[] pitch_stirrup = { "始端 肋筋ピッチ", "中央 肋筋ピッチ", "終端 肋筋ピッチ" };
            internal string[] D_reinforcement_web = { "始端 腹筋径", "中央 腹筋径", "終端 腹筋径" };
            internal string[] count_web = { "始端 腹筋本数", "中央 腹筋本数", "終端 腹筋本数" };
            internal string[] D_bar_spacing = { "始端 幅止筋径", "中央 幅止筋径", "終端 幅止筋径" };
            internal string[] count_bar_spacing = { "始端 幅止筋本数", "中央 幅止筋本数", "終端 幅止筋本数" };
            internal string[] pitch_bar_spacing = { "始端 幅止筋ピッチ", "中央 幅止筋ピッチ", "終端 幅止筋ピッチ" };
            internal string strength_reinforcement_main = "主筋種別";
            internal string strength_reinforcement_2nd_main = "副主筋強度";
            internal string strength_stirrup = "肋筋強度";
            internal string strength_reinforcement_web = "腹筋強度";
            internal string strength_bar_spacing = "幅止筋強度";
            internal string depth_cover_left = "かぶり厚さ（左）";
            internal string depth_cover_right = "かぶり厚さ（右）";
            internal string depth_cover_top = "かぶり厚さ（上）";
            internal string depth_cover_bottom = "かぶり厚さ（下）";
            internal string interval_reinforcement = "2段筋間隔";
            internal string count_X_main_top = "X形主筋本数（上）";
            internal string count_X_main_bottom = "X形主筋本数（下）";
            internal string center_reinforcement_top = "主筋重心位置（上）";
            internal string center_reinforcement_bottom = "主筋重心位置（下）";
            internal string SecId = "断面ID";
            //S部
            internal string[] strength_web = { "始端 ウェブ マテリアル", "中央 ウェブ マテリアル", "終端 ウェブ マテリアル" };
            internal string[] strength_main = { "始端 フランジ マテリアル", "中央 フランジ マテリアル", "終端 フランジ マテリアル" };
            internal string[] A = { "始端 鉄骨せい", "中央 鉄骨せい", "終端 鉄骨せい" };
            internal string[] B = { "始端 鉄骨幅", "中央 鉄骨幅", "終端 鉄骨幅" };
            internal string[] t1 = { "始端 ウェブ厚", "中央 ウェブ厚", "終端 ウェブ厚" };
            internal string[] t2 = { "始端 フランジ厚", "中央 フランジ厚", "終端 フランジ厚" };
            internal string[] r = { "始端 フィレット", "中央 フィレット", "終端 フィレット" };
            internal string[] shape = { "始端 形状名", "中央 形状名", "終端 形状名" };
            internal string[] type = { "始端 形状タイプ", "中央 形状タイプ", "終端 形状タイプ" };
            internal string offset = "RCとSの芯ズレ";
            internal string level = "RC天端からS天端までの距離";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_top = "ふかし厚さ（上）";
            internal string thickness_ex_bottom = "ふかし厚さ（下）";
            internal string thickness_ex_right = "ふかし厚さ（右）";
            internal string thickness_ex_left = "ふかし厚さ（左）";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string kind_haunch_start = "ハンチ種類（始端）";
            internal string kind_haunch_end = "ハンチ種類（終端）";
            internal string type_haunch_H = "水平ハンチ形状";
            internal string type_haunch_V = "鉛直ハンチ形状";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
        }
        #endregion

        #region 片持ち梁
        internal class CGirFamilyName
        {
            /// <summary>プロジェクトにロードされている⇒true
            /// </summary>
            internal bool[][] flg = { new bool[] { false ,false} ,
                                      new bool[] { false,false, false, false, false } ,
                                      new bool[] { false } };
            /// <summary>変換対象⇒true
            /// </summary>
            internal bool[][] convflg = { new bool[] { false ,false} ,
                                      new bool[] { false,false, false, false, false } ,
                                      new bool[] { false } };
            internal string[][] TypeName = { new string[] { "" , ""},
                                         new string[] { "","","","","" },
                                         new string[] { "" } };
            internal string[][] FamilyName = { new string[] { "" , ""},
                                         new string[] { "","","","","" },
                                         new string[] { "" } };
        }
        /// <summary>RC片持ち梁
        /// </summary>
        internal class RC_CGir
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "RC片持梁";

            //*****タイプパラメータ***********************************************************************
            internal string strength_concrete = "構造マテリアル";
            internal string kind_beam = "梁種別";
            internal string kind_beam2 = "梁の種別";
            internal string isOutIn = "外端内端指定";
            internal string width_start = "元端 梁幅";
            internal string width_end = "先端 梁幅";
            internal string depth_start = "元端 梁せい";
            internal string depth_end = "先端 梁せい";
            internal string[] BHaunch = { "ボックスハンチ 元端", "ボックスハンチ 先端" };
            internal string haunch_start = "元端 ハンチ長さ";
            internal string haunch_end = "先端 ハンチ長さ";
            internal string name = "符号";
            internal string[] D_reinforcement_main_top = { "元端 上主筋太径", "先端 上主筋太径" };
            internal string[] D_reinforcement_main_bottom = { "元端 下主筋太径", "先端 下主筋太径" };
            internal string[] D_reinforcement_2nd_main_top = { "元端 上主筋細径", "先端 上主筋細径" };
            internal string[] D_reinforcement_2nd_main_bottom = { "元端 下主筋細径", "先端 下主筋細径" };
            internal string[] count_main_top_1st = { "元端 上主筋1段太筋本数", "先端 上主筋1段太筋本数" };
            internal string[] count_main_top_2nd = { "元端 上主筋2段太筋本数", "先端 上主筋2段太筋本数" };
            internal string[] count_main_top_3rd = { "元端 上主筋3段太筋本数", "先端 上主筋3段太筋本数" };
            internal string[] count_main_bottom_1st = { "元端 下主筋1段太筋本数", "先端 下主筋1段太筋本数" };
            internal string[] count_main_bottom_2nd = { "元端 下主筋2段太筋本数", "先端 下主筋2段太筋本数" };
            internal string[] count_main_bottom_3rd = { "元端 下主筋3段太筋本数", "先端 下主筋3段太筋本数" };
            internal string[] count_2nd_main_top_1st = { "元端 上主筋1段細筋本数", "先端 上主筋1段細筋本数" };
            internal string[] count_2nd_main_top_2nd = { "元端 上主筋2段細筋本数", "先端 上主筋2段細筋本数" };
            internal string[] count_2nd_main_top_3rd = { "元端 上主筋3段細筋本数", "先端 上主筋3段細筋本数" };
            internal string[] count_2nd_main_bottom_1st = { "元端 下主筋1段細筋本数", "先端 下主筋1段細筋本数" };
            internal string[] count_2nd_main_bottom_2nd = { "元端 下主筋2段細筋本数", "先端 下主筋2段細筋本数" };
            internal string[] count_2nd_main_bottom_3rd = { "元端 下主筋3段細筋本数", "先端 下主筋3段細筋本数" };
            internal string[] D_stirrup = { "元端 肋筋径", "先端 肋筋径" };
            internal string[] count_stirrup = { "元端 肋筋本数", "先端 肋筋本数" };
            internal string[] pitch_stirrup = { "元端 肋筋ピッチ", "先端 肋筋ピッチ" };
            internal string[] D_reinforcement_web = { "元端 腹筋径", "先端 腹筋径" };
            internal string[] count_web = { "元端 腹筋本数", "先端 腹筋本数" };
            internal string[] D_bar_spacing = { "元端 幅止筋径", "先端 幅止筋径" };
            internal string[] count_bar_spacing = { "元端 幅止筋本数", "先端 幅止筋本数" };
            internal string[] pitch_bar_spacing = { "元端 幅止筋ピッチ", "先端 幅止筋ピッチ" };
            internal string strength_reinforcement_main = "主筋種別";
            internal string SecId = "断面ID";
            internal string strength_reinforcement_2nd_main = "副主筋強度";
            internal string strength_stirrup = "肋筋強度";
            internal string strength_reinforcement_web = "腹筋強度";
            internal string strength_bar_spacing = "幅止筋強度";
            internal string depth_cover_left = "かぶり厚さ（左）";
            internal string depth_cover_right = "かぶり厚さ（右）";
            internal string depth_cover_top = "かぶり厚さ（上）";
            internal string depth_cover_bottom = "かぶり厚さ（下）";
            internal string interval_reinforcement = "2段筋間隔";
            internal string count_X_main_top = "X形主筋本数（上）";
            internal string count_X_main_bottom = "X形主筋本数（下）";
            internal string center_reinforcement_top = "主筋重心位置（上）";
            internal string center_reinforcement_bottom = "主筋重心位置（下）";
            internal string bar_length_start = "始端側カットオフ筋長さ";
            internal string bar_length_end = "終端側カットオフ筋長さ";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_top = "ふかし厚さ（上）";
            internal string thickness_ex_bottom = "ふかし厚さ（下）";
            internal string thickness_ex_right = "ふかし厚さ（右）";
            internal string thickness_ex_left = "ふかし厚さ（左）";
            internal string kind_haunch_start = "ハンチ種類（始端）";
            internal string kind_haunch_end = "ハンチ種類（終端）";
            internal string type_haunch_H = "水平ハンチ形状";
            internal string type_haunch_V = "鉛直ハンチ形状";


            public static implicit operator RC_Gir(RC_CGir a)
            {
                RC_Gir g = new RC_Gir()
                {
                    Loadflg = a.Loadflg,
                    FamilyName = a.FamilyName,
                    strength_concrete = a.strength_concrete,
                    kind_beam = a.kind_beam,
                    kind_beam2 = a.kind_beam2,
                    width_start = a.width_start,
                    width_center = "",
                    width_end = a.width_end,
                    depth_start = a.depth_start,
                    depth_center = "",
                    depth_end = a.depth_end,
                    BHaunch = a.BHaunch,
                    haunch_start = a.haunch_start,
                    haunch_end = a.haunch_end,
                    name = a.name,
                    D_reinforcement_main_top = a.D_reinforcement_main_top,
                    D_reinforcement_main_bottom = a.D_reinforcement_main_bottom,
                    D_reinforcement_2nd_main_top = a.D_reinforcement_2nd_main_top,
                    D_reinforcement_2nd_main_bottom = a.D_reinforcement_2nd_main_bottom,
                    count_main_top_1st = a.count_main_top_1st,
                    count_main_top_2nd = a.count_main_top_2nd,
                    count_main_top_3rd = a.count_main_top_3rd,
                    count_main_bottom_1st = a.count_main_bottom_1st,
                    count_main_bottom_2nd = a.count_main_bottom_2nd,
                    count_main_bottom_3rd = a.count_main_bottom_3rd,
                    count_2nd_main_top_1st = a.count_2nd_main_top_1st,
                    count_2nd_main_top_2nd = a.count_2nd_main_top_2nd,
                    count_2nd_main_top_3rd = a.count_2nd_main_top_3rd,
                    count_2nd_main_bottom_1st = a.count_2nd_main_bottom_1st,
                    count_2nd_main_bottom_2nd = a.count_2nd_main_bottom_2nd,
                    count_2nd_main_bottom_3rd = a.count_2nd_main_bottom_3rd,
                    D_stirrup = a.D_stirrup,
                    count_stirrup = a.count_stirrup,
                    pitch_stirrup = a.pitch_stirrup,
                    D_reinforcement_web = a.D_reinforcement_web,
                    count_web = a.count_web,
                    D_bar_spacing = a.D_bar_spacing,
                    count_bar_spacing = a.count_bar_spacing,
                    pitch_bar_spacing = a.pitch_bar_spacing,
                    strength_reinforcement_main = a.strength_reinforcement_main,
                    strength_reinforcement_2nd_main = a.strength_reinforcement_2nd_main,
                    strength_stirrup = a.strength_stirrup,
                    strength_reinforcement_web = a.strength_reinforcement_web,
                    strength_bar_spacing = a.strength_bar_spacing,
                    depth_cover_left = a.depth_cover_left,
                    depth_cover_right = a.depth_cover_right,
                    depth_cover_top = a.depth_cover_top,
                    depth_cover_bottom = a.depth_cover_bottom,
                    interval_reinforcement = a.interval_reinforcement,
                    count_X_main_top = a.count_X_main_top,
                    count_X_main_bottom = a.count_X_main_bottom,
                    center_reinforcement_top = a.center_reinforcement_top,
                    center_reinforcement_bottom = a.center_reinforcement_bottom,
                    bar_length_start = a.bar_length_start,
                    bar_length_end = a.bar_length_end,
                    isOutIn = a.isOutIn,
                    SecId = a.SecId,
                    MemId = a.MemId,
                    NameMembers = a.NameMembers,
                    thickness_ex_top = a.thickness_ex_top,
                    thickness_ex_bottom = a.thickness_ex_bottom,
                    thickness_ex_right = a.thickness_ex_right,
                    thickness_ex_left = a.thickness_ex_left,
                    kind_haunch_start = a.kind_haunch_start,
                    kind_haunch_end = a.kind_haunch_end,
                    type_haunch_H = a.type_haunch_H,
                    type_haunch_V = a.type_haunch_V,
                };

                return g;
            }

        }
        internal class S_CGir_H
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "S片持梁";

            //*****タイプパラメータ***********************************************************************
            internal string[] strength_web = { "元端 ウェブ マテリアル", "先端 ウェブ マテリアル" };
            internal string[] strength_main = { "元端 フランジ マテリアル", "先端 フランジ マテリアル" };
            internal string name = "符号";
            internal string kind_beam = "梁種別";
            internal string kind_beam2 = "梁の種別";
            internal string[] A = { "元端 梁せい", "先端 梁せい" };
            internal string[] B = { "元端 梁幅", "先端 梁幅" };
            internal string[] t1 = { "元端 ウェブ厚", "先端 ウェブ厚" };
            internal string[] t2 = { "元端 フランジ厚", "先端 フランジ厚" };
            internal string[] r = { "元端 フィレット", "先端 フィレット" };
            internal string haunch_start = "元端 ハンチ長さ";
            internal string haunch_end = "先端 ハンチ長さ";
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string[] shape = { "元端 形状名", "先端 形状名" };
            internal string[] type = { "元端 形状タイプ", "先端 形状タイプ" };
            internal string isOutIn = "内端外端指定";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string kind_haunch_start = "ハンチ種類（始端）";
            internal string kind_haunch_end = "ハンチ種類（終端）";
            internal string type_haunch_H = "水平ハンチ形状";
            internal string type_haunch_V = "鉛直ハンチ形状";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";


            public static implicit operator S_Gir_H(S_CGir_H a)
            {
                S_Gir_H g = new S_Gir_H()
                {
                    Loadflg = a.Loadflg,
                    FamilyName = a.FamilyName,
                    name = a.name,
                    strength_web = a.strength_web,
                    strength_main = a.strength_main,
                    kind_beam = a.kind_beam,
                    kind_beam2 = a.kind_beam2,
                    isOutIn = a.isOutIn,
                    A = a.A,
                    B = a.B,
                    t1 = a.t1,
                    t2 = a.t2,
                    r = a.r,
                    haunch_start = a.haunch_start,
                    haunch_end = a.haunch_end,
                    SecId = a.SecId,
                    shape = a.shape,
                    type = a.type,
                    kind_brace = "",
                    MemId = a.MemId,
                    NameMembers = a.NameMembers,
                    condition_start = a.condition_start,
                    condition_end = a.condition_end,
                    kind_haunch_start = a.kind_haunch_start,
                    kind_haunch_end = a.kind_haunch_end,
                    type_haunch_H = a.type_haunch_H,
                    type_haunch_V = a.type_haunch_V,
                    joint_start = a.joint_start,
                    joint_end = a.joint_end,
                    kind_joint_start = a.kind_joint_start,
                    kind_joint_end = a.kind_joint_end,
                    future_brace = "",
                };

                return g;
            }
        }
        
        
        /// <summary>SRC片持梁
        /// </summary>
        internal class SRC_CGir
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "SRC片持梁";

            //*****タイプパラメータ***********************************************************************
            internal string strength_concrete = "構造マテリアル";
            internal string isOutIn = "外端内端指定";
            internal string kind_beam = "梁種別";
            internal string kind_beam2 = "梁の種別";
            internal string width_start = "元端 梁幅";
            internal string width_center = "中央 梁幅";
            internal string width_end = "先端 梁幅";
            internal string depth_start = "元端 梁せい";
            internal string depth_center = "中央 梁せい";
            internal string depth_end = "先端 梁せい";
            internal string[] BHaunch = { "ボックスハンチ 始端", "ボックスハンチ 終端" };
            internal string haunch_start = "始端 ハンチ長さ";
            internal string haunch_end = "終端 ハンチ長さ";
            internal string name = "符号";
            internal string[] D_reinforcement_main_top = { "元端 上主筋 太径", "中央 上主筋 太径", "先端 上主筋 太径" };
            internal string[] D_reinforcement_main_bottom = { "元端 下主筋 太径", "中央 下主筋 太径", "先端 下主筋 太径" };
            internal string[] D_reinforcement_2nd_main_top = { "元端 上主筋 細径", "中央 上主筋 細径", "先端 上主筋 細径" };
            internal string[] D_reinforcement_2nd_main_bottom = { "元端 下主筋 細径", "中央 下主筋 細径", "先端 下主筋 細径" };
            internal string[] count_main_top_1st = { "元端 上主筋 1段筋太筋本数", "中央 上主筋 1段筋太筋本数", "先端 上主筋 1段筋太筋本数" };
            internal string[] count_main_top_2nd = { "元端 上主筋 2段筋太筋本数", "中央 上主筋 2段筋太筋本数", "先端 上主筋 2段筋太筋本数" };
            internal string[] count_main_top_3rd = { "元端 上主筋 3段筋太筋本数", "中央 上主筋 3段筋太筋本数", "先端 上主筋 3段筋太筋本数" };
            internal string[] count_main_bottom_1st = { "元端 下主筋 1段筋太筋本数", "中央 下主筋 1段筋太筋本数", "先端 下主筋 1段筋太筋本数" };
            internal string[] count_main_bottom_2nd = { "元端 下主筋 2段筋太筋本数", "中央 下主筋 2段筋太筋本数", "先端 下主筋 2段筋太筋本数" };
            internal string[] count_main_bottom_3rd = { "元端 下主筋 3段筋太筋本数", "中央 下主筋 3段筋太筋本数", "先端 下主筋 3段筋太筋本数" };
            internal string[] count_2nd_main_top_1st = { "元端 上主筋 1段筋細筋本数", "中央 上主筋 1段筋細筋本数", "先端 上主筋 1段筋細筋本数" };
            internal string[] count_2nd_main_top_2nd = { "元端 上主筋 2段筋細筋本数", "中央 上主筋 2段筋細筋本数", "先端 上主筋 2段筋細筋本数" };
            internal string[] count_2nd_main_top_3rd = { "元端 上主筋 3段筋細筋本数", "中央 上主筋 3段筋細筋本数", "先端 上主筋 3段筋細筋本数" };
            internal string[] count_2nd_main_bottom_1st = { "元端 下主筋 1段筋細筋本数", "中央 下主筋 1段筋細筋本数", "先端 下主筋 1段筋細筋本数" };
            internal string[] count_2nd_main_bottom_2nd = { "元端 下主筋 2段筋細筋本数", "中央 下主筋 2段筋細筋本数", "先端 下主筋 2段筋細筋本数" };
            internal string[] count_2nd_main_bottom_3rd = { "元端 下主筋 3段筋細筋本数", "中央 下主筋 3段筋細筋本数", "先端 下主筋 3段筋細筋本数" };
            internal string[] D_stirrup = { "元端 肋筋径", "中央 肋筋径", "先端 肋筋径" };
            internal string[] count_stirrup = { "元端 肋筋本数", "中央 肋筋本数", "先端 肋筋本数" };
            internal string[] pitch_stirrup = { "元端 肋筋ピッチ", "中央 肋筋ピッチ", "先端 肋筋ピッチ" };
            internal string[] D_reinforcement_web = { "元端 腹筋径", "中央 腹筋径", "先端 腹筋径" };
            internal string[] count_web = { "元端 腹筋本数", "中央 腹筋本数", "先端 腹筋本数" };
            internal string[] D_bar_spacing = { "元端 幅止筋径", "中央 幅止筋径", "先端 幅止筋径" };
            internal string[] count_bar_spacing = { "元端 幅止筋本数", "中央 幅止筋本数", "先端 幅止筋本数" };
            internal string[] pitch_bar_spacing = { "元端 幅止筋ピッチ", "中央 幅止筋ピッチ", "先端 幅止筋ピッチ" };
            internal string strength_reinforcement_main = "主筋種別";
            internal string strength_reinforcement_2nd_main = "副主筋強度";
            internal string strength_stirrup = "肋筋強度";
            internal string strength_reinforcement_web = "腹筋強度";
            internal string strength_bar_spacing = "幅止筋強度";
            internal string depth_cover_left = "かぶり厚さ（左）";
            internal string depth_cover_right = "かぶり厚さ（右）";
            internal string depth_cover_top = "かぶり厚さ（上）";
            internal string depth_cover_bottom = "かぶり厚さ（下）";
            internal string interval_reinforcement = "2段筋間隔";
            internal string count_X_main_top = "X形主筋本数（上）";
            internal string count_X_main_bottom = "X形主筋本数（下）";
            internal string center_reinforcement_top = "主筋重心位置（上）";
            internal string center_reinforcement_bottom = "主筋重心位置（下）";
            internal string SecId = "断面ID";
            //S部
            internal string[] strength_web = { "元端 ウェブ マテリアル", "中央 ウェブ マテリアル", "先端 ウェブ マテリアル" };
            internal string[] strength_main = { "元端 フランジ マテリアル", "中央 フランジ マテリアル", "先端 フランジ マテリアル" };
            internal string[] A = { "元端 鉄骨せい", "中央 鉄骨せい", "先端 鉄骨せい" };
            internal string[] B = { "元端 鉄骨幅", "中央 鉄骨幅", "先端 鉄骨幅" };
            internal string[] t1 = { "元端 ウェブ厚", "中央 ウェブ厚", "先端 ウェブ厚" };
            internal string[] t2 = { "元端 フランジ厚", "中央 フランジ厚", "先端 フランジ厚" };
            internal string[] r = { "元端 フィレット", "中央 フィレット", "先端 フィレット" };
            internal string[] shape = { "元端 形状名", "中央 形状名", "先端 形状名" };
            internal string[] type = { "元端 形状タイプ", "中央 形状タイプ", "先端 形状タイプ" };
            internal string offset = "RCとSの芯ズレ";
            internal string level = "RC天端からS天端までの距離";


            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_top = "ふかし厚さ（上）";
            internal string thickness_ex_bottom = "ふかし厚さ（下）";
            internal string thickness_ex_right = "ふかし厚さ（右）";
            internal string thickness_ex_left = "ふかし厚さ（左）";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string kind_haunch_start = "ハンチ種類（始端）";
            internal string kind_haunch_end = "ハンチ種類（終端）";
            internal string type_haunch_H = "水平ハンチ形状";
            internal string type_haunch_V = "鉛直ハンチ形状";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
        }
        #endregion

        #region ブレース

        internal class BraFamilyName
        {
            /// <summary>プロジェクトにロードされている⇒true
            /// </summary>
            internal bool[][] flg = { new bool[] { false, false, false, false, false } ,
                                      new bool[] { false,false, false, false, false } };
            /// <summary>変換対象⇒true
            /// </summary>
            internal bool[][] convflg = { new bool[] { false, false, false, false, false } ,
                                      new bool[] { false,false, false, false, false } };
            internal string[][] FamilyName ={ new string[] { "","","","","" },
                                              new string[] { "","","","","" } };
            internal string[][] TypeName = { new string[] { "","","","","" },
                                             new string[] { "","","","","" } };
        }

        /// <summary>ブレースH形鋼
        /// </summary>
        internal class S_Bra_H
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "ブレースH形鋼";
            
            //*****タイプパラメータ***********************************************************************
            internal string name = "符号";
            internal string[] strength_web = { "始端 ウェブ マテリアル", "中央 ウェブ マテリアル", "終端 ウェブ マテリアル" };
            internal string[] strength_main = { "始端 フランジ マテリアル", "中央 フランジ マテリアル", "終端 フランジ マテリアル" };
            internal string[] A = { "始端 梁せい", "中央 梁せい", "終端 梁せい" };
            internal string[] B = { "始端 梁幅", "中央 梁幅", "終端 梁幅" };
            internal string[] t1 = { "始端 ウェブ厚", "中央 ウェブ厚", "終端 ウェブ厚" };
            internal string[] t2 = { "始端 フランジ厚", "中央 フランジ厚", "終端 フランジ厚" };
            internal string[] r = { "始端 フィレット", "中央 フィレット", "終端 フィレット" };           
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string[] shape = { "始端 形状名", "中央 形状名", "終端 形状名" };
            internal string[] type = { "始端 形状タイプ", "中央 形状タイプ", "終端 形状タイプ" };
            internal string kind_brace = "ブレースの種別";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";          
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
            internal string future_brace = "ブレース特性";
        }
        internal class S_Bra_BH
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "ブレース組立H形鋼";

            //*****タイプパラメータ***********************************************************************
            internal string name = "符号";
            internal string[] strength_web = { "始端 ウェブ マテリアル", "中央 ウェブ マテリアル", "終端 ウェブ マテリアル" };
            internal string[] strength_main = { "始端 フランジ マテリアル", "中央 フランジ マテリアル", "終端 フランジ マテリアル" };
            internal string[] A = { "始端 梁せい", "中央 梁せい", "終端 梁せい" };
            internal string[] B = { "始端 梁幅", "中央 梁幅", "終端 梁幅" };
            internal string[] t1 = { "始端 ウェブ厚", "中央 ウェブ厚", "終端 ウェブ厚" };
            internal string[] t2 = { "始端 フランジ厚", "中央 フランジ厚", "終端 フランジ厚" };
            internal string[] r = { "始端 フィレット", "中央 フィレット", "終端 フィレット" };
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string[] shape = { "始端 形状名", "中央 形状名", "終端 形状名" };
            internal string kind_brace = "ブレースの種別";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
            internal string future_brace = "ブレース特性";
        }
        /// <summary>ブレース角形鋼管
        /// </summary>
        internal class S_Bra_Box
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "ブレース組立角形鋼管";

            //*****タイプパラメータ***********************************************************************
            internal string strength = "構造マテリアル";
            internal string kind_brace = "ブレースの種別";
            internal string shape = "形状名";
            internal string type = "形状タイプ";
            internal string H = "鉄骨せい";
            internal string B = "鉄骨幅";
            internal string t1 = "せい方向の板厚";
            internal string t2 = "幅方向の板厚";
            internal string r = "フィレット";
            internal string name = "符号";
            internal string SecId = "断面ID";

            //*****インスタンスパラメータ******************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
            internal string future_brace = "ブレース特性";
        }
        internal class S_Bra_BBox
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "ブレース組立角形鋼管";

            //*****タイプパラメータ***********************************************************************
            internal string strength = "構造マテリアル";
            internal string kind_brace = "ブレースの種別";
            internal string shape = "形状名";
            internal string H = "鉄骨せい";
            internal string B = "鉄骨幅";
            internal string t1 = "せい方向の板厚";
            internal string t2 = "幅方向の板厚";
            internal string name = "符号";
            internal string SecId = "断面ID";

            //*****インスタンスパラメータ******************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
            internal string future_brace = "ブレース特性";
        }

        /// <summary>ブレース円形鋼管
        /// </summary>
        internal class S_Bra_Pipe
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "ブレース円形鋼管";

            //*****タイプパラメータ***********************************************************************
            internal string strength = "構造マテリアル";
            internal string kind_brace = "ブレースの種別";
            internal string shape = "形状名";
            internal string D = "直径";
            internal string t = "板厚";
            internal string name = "符号";
            internal string SecId = "断面ID";

            //*****インスタンスパラメータ******************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
            internal string future_brace = "ブレース特性";
        }

        /// <summary>ブレース溝形鋼
        /// </summary>
        internal class S_Bra_C
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "ブレース溝形鋼";

            //*****タイプパラメータ***********************************************************************
            internal string name = "符号";
            internal string strength = "構造マテリアル";
            internal string[] H = { "始端 梁せい", "中央 梁せい", "終端 梁せい" };
            internal string[] B = { "始端 梁幅", "中央 梁幅", "終端 梁幅" };
            internal string[] t1 = { "始端 ウェブ厚", "中央 ウェブ厚", "終端 ウェブ厚" };
            internal string[] t2 = { "始端 フランジ厚", "中央 フランジ厚", "終端 フランジ厚" };
            internal string[] r1 = { "始端 フィレット", "中央 フィレット", "終端 フィレット" };
            internal string[] r2 = { "始端 先端半径", "中央 先端半径", "終端 先端半径" };
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string[] shape = { "始端 形状名", "中央 形状名", "終端 形状名" };
            internal string[] type = { "始端 形状タイプ", "中央 形状タイプ", "終端 形状タイプ" };
            internal string[] side = { "始端 背合わせ", "中央 背合わせ", "終端 背合わせ" };
            internal string kind_brace = "ブレースの種別";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
            internal string future_brace = "ブレース特性";
        }
        /// <summary>ブレースリップ溝形鋼
        /// </summary>
        internal class S_Bra_LipC
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "ブレースリップ溝形鋼";

            //*****タイプパラメータ***********************************************************************
            internal string name = "符号";
            internal string strength = "構造マテリアル";
            internal string[] H = { "始端 梁せい", "中央 梁せい", "終端 梁せい" };
            internal string[] A = { "始端 梁幅", "中央 梁幅", "終端 梁幅" };
            internal string[] C = { "始端 リップ長", "中央 リップ長", "終端 リップ長" };
            internal string[] t = { "始端 板厚", "中央 板厚", "終端 板厚" };
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string[] shape = { "始端 形状名", "中央 形状名", "終端 形状名" };
            internal string[] type = { "始端 形状タイプ", "中央 形状タイプ", "終端 形状タイプ" };
            internal string[] side = { "始端 背合わせ", "中央 背合わせ", "終端 背合わせ" };
            internal string kind_brace = "ブレースの種別";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
            internal string future_brace = "ブレース特性";
        }
        /// <summary>ブレース山形鋼
        /// </summary>
        internal class S_Bra_L
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "ブレース山形鋼";

            //*****タイプパラメータ***********************************************************************
            internal string name = "符号";
            internal string strength = "構造マテリアル";
            internal string[] A = { "始端 梁せい", "中央 梁せい", "終端 梁せい" };
            internal string[] B = { "始端 梁幅", "中央 梁幅", "終端 梁幅" };
            internal string[] t1 = { "始端 せい方向板厚", "中央 せい方向板厚", "終端 せい方向板厚" };
            internal string[] t2 = { "始端 幅方向板厚", "中央 幅方向板厚", "終端 幅方向板厚" };
            internal string[] r1 = { "始端 フィレット", "中央 フィレット", "終端 フィレット" };
            internal string[] r2 = { "始端 先端半径", "中央 先端半径", "終端 先端半径" };
            //STBLinkで追加したもの
            internal string SecId = "断面ID";
            internal string[] shape = { "始端 形状名", "中央 形状名", "終端 形状名" };
            internal string[] type = { "始端 形状タイプ", "中央 形状タイプ", "終端 形状タイプ" };
            internal string[] side = { "始端 背合わせ", "中央 背合わせ", "終端 背合わせ" };
            internal string kind_brace = "ブレースの種別";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
            internal string future_brace = "ブレース特性";
        }

        /// <summary>ブレース丸鋼
        /// </summary>
        internal class S_Bra_RollBar
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "ブレース丸鋼";

            //*****タイプパラメータ***********************************************************************
            internal string strength_main = "構造マテリアル";
            internal string kind_brace = "ブレースの種別";
            internal string shape = "形状名";
            internal string D = "直径";
            internal string name = "符号";
            internal string SecId = "断面ID";

            //*****インスタンスパラメータ******************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
            internal string future_brace = "ブレース特性";
        }
    
        /// <summary>ブレースフラットバー
        /// </summary>
        internal class S_Bra_FB
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "ブレースフラットバー";

            //*****タイプパラメータ***********************************************************************
            internal string strength_main = "構造マテリアル";
            internal string kind_brace = "ブレースの種別";
            internal string shape = "形状名";
            internal string B = "幅";
            internal string t = "板厚";
            internal string name = "符号";
            internal string SecId = "断面ID";

            //*****インスタンスパラメータ******************************************************************
            //STBLinkで追加したもの
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string condition_start = "始端条件";
            internal string condition_end = "終端条件";
            internal string joint_start = "ジョイント位置（始端）";
            internal string joint_end = "ジョイント位置（終端）";
            internal string kind_joint_start = "ジョイント種別（始端）";
            internal string kind_joint_end = "ジョイント種別（終端）";
            internal string future_brace = "ブレース特性";
        }

       
        #endregion

        #region 床
        internal class Slab
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****タイプパラメータ***********************************************************************
            internal string isEarthen = "土間か否か";
            internal string isCanti = "床スラブ_種別";
            internal string FigureType = "断面名称"; 
            internal string product_type = "製品種別";
            internal string product_company = "メーカー";
            internal string product_name = "製品名";
            internal string product_code = "製品型番";
            internal string depth_center = "中央厚さ";
            internal string depth_tip = "先端厚さ";
            internal string depth_base = "根元厚さ";
            internal string product_depth = "製品高さ";
            internal string length_haunch = "ハンチ長さ";
            internal string name = "符号";
            internal string ArrengementType = "配筋タイプ";           
            internal string[] D1 = { "柱列帯 上主筋径1", "端部 上主筋径1", "中央 上主筋径1",
                                    "柱列帯 下主筋径1", "端部 下主筋径1","中央 下主筋径1",
                                    "柱列帯 上配力筋径1", "端部 上配力筋径1","中央 上配力筋径1",
                                    "柱列帯 下配力筋径1", "端部 下配力筋径1", "中央 下配力筋径1" };
            internal string[] D2 = { "柱列帯 上主筋径2", "端部 上主筋径2", "中央 上主筋径2",
                                    "柱列帯 下主筋径2", "端部 下主筋径2","中央 下主筋径2",
                                    "柱列帯 上配力筋径2", "端部 上配力筋径2","中央 上配力筋径2",
                                    "柱列帯 下配力筋径2", "端部 下配力筋径2", "中央 下配力筋径2" };
            internal string[] pitch ={ "柱列帯 上主筋ピッチ", "端部 上主筋ピッチ", "中央 上主筋ピッチ",
                                    "柱列帯 下主筋ピッチ", "端部 下主筋ピッチ","中央 下主筋ピッチ",
                                    "柱列帯 上配力筋ピッチ", "端部 上配力筋ピッチ","中央 上配力筋ピッチ",
                                    "柱列帯 下配力筋ピッチ", "端部 下配力筋ピッチ", "中央 下配力筋ピッチ" };
            internal string[] T_D1 = { "根元 上主筋径1", "根元 下主筋径1", "先端 上主筋径1",  "先端 下主筋径1" };
            internal string[] T_D2 = { "根元 上主筋径2", "根元 下主筋径2", "先端 上主筋径2", "先端 下主筋径2" };
            internal string[] T_pitch = { "根元 上主筋ピッチ",  "根元 下主筋ピッチ", "先端 上主筋ピッチ",  "先端 下主筋ピッチ"};
            internal string addD = "耐火補強筋 径";
            internal string addpitch = "耐火補強筋 ピッチ";
            internal string D_bar_spacing = "幅止筋径";
            internal string pitch_bar_spacing = "幅止筋ピッチ";

            //開口
            internal string[] D_op = { "スラブ開口 X方向上端筋径", "スラブ開口 X方向下端筋径", "スラブ開口 Y方向上端筋径", "スラブ開口 Y方向下端筋径", "スラブ開口 斜め方向上端筋径", "スラブ開口 斜め方向下端筋径" };
            internal string[] count_op = { "スラブ開口 X方向上端筋本数", "スラブ開口 X方向下端筋本数", "スラブ開口 Y方向上端筋本数", "スラブ開口 Y方向下端筋本数", "スラブ開口 斜め方向上端筋本数", "スラブ開口 斜め方向下端筋本数" };
            internal string[] length_op = { "スラブ開口 X方向上端筋長さ", "スラブ開口 X方向下端筋長さ", "スラブ開口 Y方向上端筋長さ", "スラブ開口 Y方向下端筋長さ", "スラブ開口 斜め方向上端筋長さ", "スラブ開口 斜め方向下端筋長さ" };
            internal string strength = "鉄筋種別";
            internal string depth_cover_top = "かぶり厚さ（上）";
            internal string depth_cover_bottom = "かぶり厚さ（下）";
            internal string SecId = "断面ID";

            //*****インスタンスパラメータ******************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_upper = "ふかし厚さ（上）";
            internal string thickness_ex_bottom = "ふかし厚さ（下）";
            internal string dir_load = "荷重伝達方向";
            internal string angle_load = "荷重伝達角度";
            internal string isFoundation = "基礎か否か";
            internal string type_haunch = "ハンチ形状";
            internal string kind_slab = "スラブ種類";
            internal string kind_structure = "構造種別";

        }
        #endregion

        #region 壁
        internal class Wall
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****タイプパラメータ***********************************************************************            
            internal string name = "符号";
            internal string ArrengementType = "配筋タイプ";
            internal string strength = "鉄筋種別";
            internal string[] D = { "縦筋径1", "横筋径1" };
            internal string[] D2 = { "縦筋径2", "横筋径2" };
            internal string[] pitch = { "縦筋ピッチ", "横筋ピッチ" };
            internal string[] D_inout = { "外側上端 縦筋径1", "外側中央 縦筋径1", "外側下端 縦筋径1" , "内側上端 縦筋径1", "内側中央 縦筋径1", "内側下端 縦筋径1" ,
                                          "外側始端 横筋径1", "外側中央 横筋径1", "外側終端 横筋径1" , "内側始端 横筋径1", "内側中央 横筋径1", "内側終端 横筋径1" };
            internal string[] D2_inout = { "外側上端 縦筋径2", "外側中央 縦筋径2", "外側下端 縦筋径2" , "内側上端 縦筋径2", "内側中央 縦筋径2", "内側下端 縦筋径2" ,
                                           "外側始端 横筋径2", "外側中央 横筋径2", "外側終端 横筋径2" , "内側始端 横筋径2", "内側中央 横筋径2", "内側終端 横筋径2" };
            internal string[] pitch_inout = {"外側上端 縦筋ピッチ", "外側中央 縦筋ピッチ", "外側下端 縦筋ピッチ" , "内側上端 縦筋ピッチ", "内側中央 縦筋ピッチ", "内側下端 縦筋ピッチ" ,
                                          　 "外側始端 横筋ピッチ", "外側中央 横筋ピッチ", "外側終端 横筋ピッチ" , "内側始端 横筋ピッチ", "内側中央 横筋ピッチ", "内側終端 横筋ピッチ" };            
            internal string[] D_Edge = { "袖壁始端 端部補強筋径", "袖壁終端 端部補強筋径", "垂壁下端 端部補強筋径", "腰壁上端 端部補強筋径" };
            internal string[] count_Edge = { "袖壁始端 端部補強筋本数", "袖壁終端 端部補強筋本数", "垂壁下端 端部補強筋本数", "腰壁上端 端部補強筋本数" };
            internal string D_bar_spacing = "幅止筋径";
            internal string pitch_bar_spacing = "幅止筋ピッチ";

            //開口
            internal string[] D_op = { "開口補強筋 縦筋径", "開口補強筋 横筋径", "開口補強筋 斜筋径" };
            internal string[] count_op = { "開口補強筋 縦筋本数", "開口補強筋 横筋本数", "開口補強筋 斜筋本数" };
            internal string[] length_op = { "開口補強筋 縦筋長さ", "開口補強筋 横筋長さ", "開口補強筋 斜筋長さ" };
            //パラペット
            internal string kind_form = "パラペット 断面形式";
            internal string isTip_line = "パラペット 垂下の鉄筋の有無";
            internal string depth_T = "パラペット 幅";
            internal string depth_H = "パラペット 高さ";
            internal string depth_T1 = "パラペット アゴ幅";
            internal string depth_H1 = "パラペット アゴ先端高さH1";
            internal string depth_H2 = "パラペット アゴ傾斜高さH2";
            internal string depth_H3 = "パラペット アゴ位置H3";
            internal string strength_Tip = "先端補強筋強度";
            internal string[] D_Tip = { "アゴ先端補強筋 短辺方向径", "アゴ先端補強筋 長辺方向径" };
            internal string[] pitch_Tip = { "アゴ先端補強筋 短辺方向ピッチ", "アゴ先端補強筋 長辺方向ピッチ" };
            internal string[] count_Tip = { "アゴ先端補強筋 短辺方向本数", "アゴ先端補強筋 長辺方向本数" };
            internal string[] D_Edge_Para = { "パラペット端部補強筋 始端径", "パラペット端部補強筋 終端径", "パラペット端部補強筋 上端径", "パラペット端部補強筋 下端径" };
            internal string[] count_Edge_Para = { "パラペット端部補強筋 始端本数", "パラペット端部補強筋 終端本数", "パラペット端部補強筋 上端本数", "パラペット端部補強筋 下端本数" };
            internal string depth_cover_outside = "かぶり厚さ（外側面）";
            internal string depth_cover_inside = "かぶり厚さ（内側面）";
            internal string SecId = "断面ID";

            //*****インスタンスパラメータ******************************************************************
            internal string kind_structure = "構造種別";
            internal string kind_layout = "壁種別";
            internal string kind_wall = "耐力区分";
            internal string type_outside = "外側タイプ";
            internal string isPress = "土圧壁か否か";
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_right = "ふかし厚さ（右）";
            internal string thickness_ex_left = "ふかし厚さ（左）";
            internal string slit_upper = "構造スリット（上）";
            internal string slit_bottom = "構造スリット（下）";
            internal string slit_left = "構造スリット（左）";
            internal string slit_right = "構造スリット（右）";

            //パラペット
            internal string direction = "パラペット アゴ方向";

        }
        #endregion

        #region 基礎
        internal class FoundationFamilyName
        {
            /// <summary>プロジェクトにロードされている⇒true
            /// </summary>
            internal bool[][] flg = { new bool[] { false, false, false, false, false } ,
                                      new bool[] { false },
                                      new bool[] { false, false, false, false, false, false, false, false } };
            /// <summary>変換対象⇒true
            /// </summary>
            internal bool[][] convflg = { new bool[] { false, false, false, false, false } ,
                                          new bool[] { false },
                                          new bool[] { false, false, false, false, false, false, false, false } };
            internal string[][] FamilyName = { new string[] { "", "", "", "", "" },
                                               new string[] { "" },
                                               new string[] { "", "", "", "", "", "", "", "" } };
            internal string[][] TypeName   = { new string[] { "", "", "", "", "" },
                                               new string[] { "" },
                                               new string[] { "", "", "", "", "", "", "", "" } };

            //杭memo
            //[2][0]:1.4 場所打ち杭 / 2.0 RC杭
            //[2][1]:1.4 既製杭     / 2.0 使わない
            //[2][2]:1.4 使わない   / 2.0 鋼管杭
            //[2][3]:1.4 使わない   / 2.0 既製杭 PHC
            //[2][4]:1.4 使わない   / 2.0 既製杭 ST
            //[2][5]:1.4 使わない   / 2.0 既製杭 SC
            //[2][6]:1.4 使わない   / 2.0 既製杭 PRC
            //[2][7]:1.4 使わない   / 2.0 既製杭 CPRC
        }
        internal class Foundation_Rect
        {
            //*****ロードされているか否か*****************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*********************************************************
            internal string FamilyName = "RC基礎矩形";

            //*****タイプパラメータ***********************************************************************
            internal string strength_concrete = "構造マテリアル";
            internal string SecId = "断面ID";
            internal string name = "符号";            
            internal string depth_cover_top = "かぶり厚さ（上）";
            internal string depth_cover_bottom = "かぶり厚さ（下）";
            internal string depth_cover_side = "かぶり厚さ（側面）";
            internal string DX = "X幅";
            internal string DY = "Y幅";
            internal string depth = "厚さ";
            internal string strength = "鉄筋種別";
            internal string[] D = { "X方向 上端筋径", "X方向 下端筋径", "Y方向 上端筋径", "Y方向 下端筋径", "横筋径" };
            internal string[] count = { "X方向 上端筋本数", "X方向 下端筋本数", "Y方向 上端筋本数", "Y方向 下端筋本数", "横筋本数" };

            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_start_X = "ふかし厚さ（X始）";
            internal string thickness_ex_end_X = "ふかし厚さ（X終）";
            internal string thickness_ex_start_Y = "ふかし厚さ（Y始）";
            internal string thickness_ex_end_Y = "ふかし厚さ（Y終）";
            internal string thickness_ex_top = "ふかし厚さ（上）";
            internal string thickness_ex_bottom = "ふかし厚さ（下）";
        }

        internal class Foundation_Tapered_Rect
        {
            //*****ロードされているか否か*****************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*********************************************************
            internal string FamilyName = "RC基礎矩形テーパー";

            //*****タイプパラメータ***********************************************************************
            internal string SecId = "断面ID";
            internal string name = "符号";
            internal string strength_concrete = "構造マテリアル";
            internal string depth_cover_top = "かぶり厚さ（上）";
            internal string depth_cover_bottom = "かぶり厚さ（下）";
            internal string depth_cover_side = "かぶり厚さ（側面）";
            internal string DX = "X幅";
            internal string DY = "Y幅";
            internal string t_DX = "テーパーX幅";
            internal string t_DY = "テーパーY幅";
            internal string t_offset_X = "テーパーX偏心";
            internal string t_offset_Y = "テーパーY偏心";
            internal string depth_base = "厚さ";
            internal string depth_tip = "先端厚さ";
            internal string strength = "鉄筋種別";
            internal string[] D = { "X方向 上端筋径", "X方向 下端筋径", "Y方向 上端筋径", "Y方向 下端筋径", "横筋径" };
            internal string[] count = { "X方向 上端筋本数", "X方向 下端筋本数", "Y方向 上端筋本数", "Y方向 下端筋本数", "横筋本数" };

            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_start_X = "ふかし厚さ（X始）";
            internal string thickness_ex_end_X = "ふかし厚さ（X終）";
            internal string thickness_ex_start_Y = "ふかし厚さ（Y始）";
            internal string thickness_ex_end_Y = "ふかし厚さ（Y終）";
            internal string thickness_ex_top = "ふかし厚さ（上）";
            internal string thickness_ex_bottom = "ふかし厚さ（下）";
        }

        //internal class Base_Tapered
        //{
        //    //ロードされているか否か**********************************************************************
        //    internal bool Loadflg = false;

        //    //*****マッピングテーブルのファミリ名*****************************************************************************
        //    internal string FamilyName = "RC基礎矩形テーパー";
        //}

        internal class Foundation_Triangle
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "RC基礎三角";

            //*****タイプパラメータ***********************************************************************
            internal string strength_concrete = "構造マテリアル";
            internal string DX = "X幅";
            internal string DY = "Y幅";
            internal string depth = "厚さ";
            internal string name = "符号";
            internal string SecId = "断面ID";
            internal string strength = "鉄筋種別";
            internal string[] D = { "主筋方向 上端筋径", "主筋方向 下端筋径", "配力筋方向 上端筋径", "配力筋方向 下端筋径", "横筋径" };
            internal string[] count = { "主筋方向 上端筋本数", "主筋方向 下端筋本数", "配力筋方向 上端筋本数", "配力筋方向 下端筋本数", "横筋本数" };
            internal string depth_cover_top = "かぶり厚さ（上）";
            internal string depth_cover_bottom = "かぶり厚さ（下）";
            internal string depth_cover_side = "かぶり厚さ（側面）";

            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_start_X = "ふかし厚さ（X始）";
            internal string thickness_ex_end_X = "ふかし厚さ（X終）";
            internal string thickness_ex_start_Y = "ふかし厚さ（Y始）";
            internal string thickness_ex_end_Y = "ふかし厚さ（Y終）";
            internal string thickness_ex_top = "ふかし厚さ（上）";
            internal string thickness_ex_bottom = "ふかし厚さ（下）";
        }

        internal class Foundation_Equi_Triangle
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "RC基礎正三角形";

            //*****タイプパラメータ***********************************************************************
            internal string strength_concrete = "構造マテリアル";
            internal string B = "底辺幅";
            internal string C = "面取り幅";
            internal string depth = "厚さ";
            internal string name = "符号";
            internal string SecId = "断面ID";
            internal string strength = "鉄筋種別";
            internal string[] D = { "主筋方向 上端筋径", "主筋方向 下端筋径", "外周上端 径", "外周下端 径", "横筋径" };
            internal string[] count = { "主筋方向 上端筋本数", "主筋方向 下端筋本数", "外周上端 本数", "外周下端 本数", "横筋本数" };
            internal string depth_cover_top = "かぶり厚さ（上）";
            internal string depth_cover_bottom = "かぶり厚さ（下）";
            internal string depth_cover_side = "かぶり厚さ（側面）";


            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_start_X = "ふかし厚さ（X始）";
            internal string thickness_ex_end_X = "ふかし厚さ（X終）";
            internal string thickness_ex_start_Y = "ふかし厚さ（Y始）";
            internal string thickness_ex_end_Y = "ふかし厚さ（Y終）";
            internal string thickness_ex_top = "ふかし厚さ（上）";
            internal string thickness_ex_bottom = "ふかし厚さ（下）";

        }

        internal class Foundation_Octagon
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "RC基礎八角形";

            //*****タイプパラメータ***********************************************************************
            internal string strength_concrete = "構造マテリアル";
            internal string DX = "X幅";
            internal string DY = "Y幅";
            internal string CX1 = "面取り幅X幅（1）";
            internal string CY1 = "面取り幅Y幅（1）";
            internal string CX2 = "面取り幅X幅（2）";
            internal string CY2 = "面取り幅Y幅（2）";
            internal string CX3 = "面取り幅X幅（3）";
            internal string CY3 = "面取り幅Y幅（3）";
            internal string CX4 = "面取り幅X幅（4）";
            internal string CY4 = "面取り幅Y幅（4）";
            internal string depth = "厚さ";
            internal string name = "符号";
            internal string SecId = "断面ID";
            internal string strength = "鉄筋種別";
            internal string[] D = { "X方向 上端筋径", "X方向 下端筋径", "Y方向 上端筋径", "Y方向 下端筋径", "横筋径" };
            internal string[] count = { "X方向 上端筋本数", "X方向 下端筋本数", "Y方向 上端筋本数", "Y方向 下端筋本数", "横筋本数" };
            internal string depth_cover_top = "かぶり厚さ（上）";
            internal string depth_cover_bottom = "かぶり厚さ（下）";
            internal string depth_cover_side = "かぶり厚さ（側面）";

            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string thickness_ex_start_X = "ふかし厚さ（X始）";
            internal string thickness_ex_end_X = "ふかし厚さ（X終）";
            internal string thickness_ex_start_Y = "ふかし厚さ（Y始）";
            internal string thickness_ex_end_Y = "ふかし厚さ（Y終）";
            internal string thickness_ex_top = "ふかし厚さ（上）";
            internal string thickness_ex_bottom = "ふかし厚さ（下）";
        }

        internal class Foundation_Continuous
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "布基礎";

            //*****タイプパラメータ***********************************************************************
            internal string SecId = "断面ID";
            internal string name = "符号";
            internal string strength_concrete = "構造マテリアル";
            internal string depth_cover_top = "かぶり厚さ（上）";
            internal string depth_cover_bottom = "かぶり厚さ（下）";
            internal string depth_cover_side = "かぶり厚さ（側面）";
            internal string B = "幅";
            internal string t_B = "テーパー幅";
            internal string depth_base = "根元厚さ";
            internal string depth_tip = "先端厚さ";
            internal string type_right = "右寄せ";
            internal string type_left = "左寄せ";
            //internal string[] strength = { "X方向上端筋強度", "X方向下端筋強度", "Y方向上端筋強度", "Y方向下端筋強度", "横筋強度" };
            internal string strength = "鉄筋種別";
            internal string[] D = { "主筋方向 上端筋径", "主筋方向 下端筋径", "配力筋方向 上端筋径", "配力筋方向 下端筋径", "横筋径" };
            internal string[] count = { "", "", "配力筋方向 上端筋本数", "配力筋方向 下端筋本数", "横筋本数" };
            internal string[] pitch = { "主筋方向 上端筋ピッチ", "主筋方向 下端筋ピッチ", "", "", "" };
            internal string type = "形状タイプ";

            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";           
            internal string length_ex_start = "始端側余長";
            internal string length_ex_end = "終端側余長";
        }
        internal class Pile
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "場所打ち杭";

            //*****タイプパラメータ***********************************************************************
            internal string strength_concrete = "構造マテリアル";
            internal string length_all = "杭全長";
            internal string length_head = "杭頭（拡頭）長さ";
            internal string length_foot = "杭脚長さ";
            internal string length_foot_Revit = "拡底部_長さ";
            internal string length_foot_taper = "拡底部_テーパー長さ";
            internal string length_head_taper = "拡頭部_テーパー長さ";
            internal string D = "杭径";
            internal string D_extended_foot = "拡底径";
            internal string D_extended_top = "拡頭径";
            internal string name = "符号";
            internal string[] D_main_circumference_1st = { "杭頭 主筋外周1段目径", "軸部 主筋外周1段目径", "杭脚 主筋外周1段目径" };
            internal string[] count_main_circumference_1st = { "杭頭 主筋外周1段目本数", "軸部 主筋外周1段目本数", "杭脚 主筋外周1段目本数" };
            internal string[] D_main_core = { "杭頭 主筋芯筋径", "軸部 主筋芯筋径", "杭脚 主筋芯筋径" };
            internal string[] count_main_core = { "杭頭 主筋芯筋本数", "軸部 主筋芯筋本数", "杭脚 主筋芯筋本数" };
            internal string[] D_band = { "杭頭 帯筋径", "軸部 帯筋径", "杭脚 帯筋径" };
            internal string[] pitch_band = { "杭頭 帯筋ピッチ", "軸部 帯筋ピッチ", "杭脚 帯筋ピッチ" };
            internal string strength_main_circumference_1st = "主筋種別";
            internal string strength_main_core = "芯筋種別";
            internal string strength_band = "帯筋強度";
            internal string depth_cover = "かぶり厚さ";
            internal string depth_cover_top = "拡頭部かぶり厚さ";
            internal string SecId = "断面ID";
            internal string zeroLength = "杭長0_Flag";

            //*****インスタンスパラメータ*****************************************************************
            internal string kind_structure = "構造種別";
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
        }
        internal class Pile_2
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "既製杭";

            //*****タイプパラメータ***********************************************************************
            internal string strength_concrete = "構造マテリアル";
            internal string straight_D = "ストレート 杭径";
            internal string straight_length = "ストレート 杭全長";
            internal string ef_D_axial = "脚部拡大 軸部径";
            internal string ef_D_extended_foot = "脚部拡大 拡底径";
            internal string ef_length_axial = "脚部拡大 軸部長さ";
            internal string ef_length_foot = "脚部拡大 杭脚長さ";
            internal string et_D_extended_top = "頂部拡大 拡頭径";
            internal string et_D_axial = "頂部拡大 軸部径";
            internal string et_length_head = "頂部拡大 杭頭長さ";
            internal string et_length_axial = "頂部拡大 軸部長さ";
            internal string etf_D_extended_top = "両端拡大 拡頭径";
            internal string etf_D_axial = "両端拡大 軸部径";
            internal string etf_D_extended_foot = "両端拡大 拡底径";
            internal string etf_length_head = "両端拡大 杭頭長さ";
            internal string etf_length_axial = "両端拡大 軸部長さ";
            internal string etf_length_foot = "両端拡大 杭脚長さ";
            internal string name = "符号";
            internal string[] D_main_circumference_1st = { "杭頭 主筋外周1段目径", "軸部 主筋外周1段目径", "杭脚 主筋外周1段目径" };
            internal string[] count_main_circumference_1st = { "杭頭 主筋外周1段目本数", "軸部 主筋外周1段目本数", "杭脚 主筋外周1段目本数" };
            internal string[] D_main_core = { "杭頭 主筋芯筋径", "軸部 主筋芯筋径", "杭脚 主筋芯筋径" };
            internal string[] count_main_core = { "杭頭 主筋芯筋本数", "軸部 主筋芯筋本数", "杭脚 主筋芯筋本数" };
            internal string[] D_band = { "杭頭 帯筋径", "軸部 帯筋径", "杭脚 帯筋径" };
            internal string[] pitch_band = { "杭頭 帯筋ピッチ", "軸部 帯筋ピッチ", "杭脚 帯筋ピッチ" };
            internal string strength_main_circumference_1st = "主筋種別";
            internal string strength_main_core = "芯筋種別";
            internal string strength_band = "帯筋強度";
            internal string depth_cover = "かぶり厚さ";
            internal string depth_cover_top = "拡頭部かぶり厚さ";
            internal string SecId = "断面ID";
            internal string zeroLength = "杭長0_Flag";

            //*****インスタンスパラメータ*****************************************************************
            internal string kind_structure = "構造種別";
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
        }

        //使用しない
        //internal class Pile_Straight
        //{
        //    //ロードされているか否か**********************************************************************
        //    internal bool Loadflg = false;

        //    //*****マッピングテーブルのファミリ名*****************************************************************************
        //    internal string FamilyName = "RC杭ストレート";

        //    //*****タイプパラメータ***********************************************************************
        //    internal string SecId = "断面ID";
        //    internal string name = "符号";
        //    internal string strength_concrete = "構造マテリアル";
        //    internal string depth_cover = "かぶり厚さ";
        //    internal string depth_cover_top = "拡頭部かぶり厚さ";
        //    internal string D = "杭径";
        //    internal string strength = "鉄筋強度";
        //    internal string[] D_main_circumference_1st = { "杭頭_主筋_径", "軸部_主筋_径", "脚部_主筋_径" };
        //    internal string[] count_main_circumference_1st = { "杭頭_主筋_本数", "軸部_主筋_本数", "脚部_主筋_本数" };
        //    internal string[] D_main_core = { "杭頭_芯鉄筋_径", "軸部_芯鉄筋_径", "脚部_芯鉄筋_径" };
        //    internal string[] count_main_core = { "杭頭_芯鉄筋_本数", "軸部_芯鉄筋_本数", "脚部_芯鉄筋_本数" };
        //    internal string[] D_band = { "杭頭_帯筋_径", "軸部_帯筋_径", "脚部_帯筋_径" };
        //    internal string[] count_band = { "杭頭_帯筋_本数", "軸部_帯筋_本数", "脚部_帯筋_本数" };
        //    internal string[] pitch_band = { "杭頭_帯筋_ピッチ", "軸部_帯筋_ピッチ", "脚部_帯筋_ピッチ" };
        //    internal string length_all = "杭全長";
        //    internal string pile_type = "杭種1<構造基礎>";

        //    //*****インスタンスパラメータ*****************************************************************
        //    internal string MemId = "配置ID";
        //    internal string NameMembers = "配置名";
        //}

        //internal class Pile_Extended_Foot
        //{
        //    //ロードされているか否か**********************************************************************
        //    internal bool Loadflg = false;

        //    //*****マッピングテーブルのファミリ名*****************************************************************************
        //    internal string FamilyName = "RC杭脚部拡大";

        //    //*****タイプパラメータ***********************************************************************
        //    internal string SecId = "断面ID";
        //    internal string name = "符号";
        //    internal string strength_concrete = "構造マテリアル";
        //    internal string depth_cover = "かぶり厚さ";
        //    internal string depth_cover_top = "拡頭部かぶり厚さ";
        //    internal string D_axial = "軸径";
        //    internal string D_extended_foot = "拡底径";
        //    internal string strength = "鉄筋強度";
        //    internal string[] D_main_circumference_1st = { "杭頭_主筋_径", "軸部_主筋_径", "脚部_主筋_径" };
        //    internal string[] count_main_circumference_1st = { "杭頭_主筋_本数", "軸部_主筋_本数", "脚部_主筋_本数" };
        //    internal string[] D_main_core = { "杭頭_芯鉄筋_径", "軸部_芯鉄筋_径", "脚部_芯鉄筋_径" };
        //    internal string[] count_main_core = { "杭頭_芯鉄筋_本数", "軸部_芯鉄筋_本数", "脚部_芯鉄筋_本数" };
        //    internal string[] D_band = { "杭頭_帯筋_径", "軸部_帯筋_径", "脚部_帯筋_径" };
        //    internal string[] count_band = { "杭頭_帯筋_本数", "軸部_帯筋_本数", "脚部_帯筋_本数" };
        //    internal string[] pitch_band = { "杭頭_帯筋_ピッチ", "軸部_帯筋_ピッチ", "脚部_帯筋_ピッチ" };

        //    //*****インスタンスパラメータ*****************************************************************
        //    internal string MemId = "配置ID";
        //    internal string NameMembers = "配置名";

        //}

        //internal class Pile_Extended_Top
        //{
        //    //ロードされているか否か**********************************************************************
        //    internal bool Loadflg = false;

        //    //*****マッピングテーブルのファミリ名*****************************************************************************
        //    internal string FamilyName = "RC杭頂部拡大";

        //    //*****タイプパラメータ***********************************************************************
        //    internal string SecId = "断面ID";
        //    internal string name = "符号";
        //    internal string strength_concrete = "構造マテリアル";
        //    internal string depth_cover = "かぶり厚さ";
        //    internal string depth_cover_top = "拡頭部かぶり厚さ";
        //    internal string D_axial = "軸径";
        //    internal string D_extended_top = "拡頭径";
        //    internal string strength = "鉄筋強度";
        //    internal string[] D_main_circumference_1st = { "杭頭_主筋_径", "軸部_主筋_径", "脚部_主筋_径" };
        //    internal string[] count_main_circumference_1st = { "杭頭_主筋_本数", "軸部_主筋_本数", "脚部_主筋_本数" };
        //    internal string[] D_main_core = { "杭頭_芯鉄筋_径", "軸部_芯鉄筋_径", "脚部_芯鉄筋_径" };
        //    internal string[] count_main_core = { "杭頭_芯鉄筋_本数", "軸部_芯鉄筋_本数", "脚部_芯鉄筋_本数" };
        //    internal string[] D_band = { "杭頭_帯筋_径", "軸部_帯筋_径", "脚部_帯筋_径" };
        //    internal string[] count_band = { "杭頭_帯筋_本数", "軸部_帯筋_本数", "脚部_帯筋_本数" };
        //    internal string[] pitch_band = { "杭頭_帯筋_ピッチ", "軸部_帯筋_ピッチ", "脚部_帯筋_ピッチ" };

        //    //*****インスタンスパラメータ*****************************************************************
        //    internal string MemId = "配置ID";
        //    internal string NameMembers = "配置名";
        //}

        //internal class Pile_Extended_Top_Foot
        //{
        //    //ロードされているか否か**********************************************************************
        //    internal bool Loadflg = false;

        //    //*****マッピングテーブルのファミリ名*****************************************************************************
        //    internal string FamilyName = "RC杭両端拡大";
        //    //*****タイプパラメータ***********************************************************************
        //    internal string SecId = "断面ID";
        //    internal string name = "符号";
        //    internal string strength_concrete = "構造マテリアル";
        //    internal string depth_cover = "かぶり厚さ";
        //    internal string depth_cover_top = "拡頭部かぶり厚さ";
        //    internal string D_axial = "軸径";
        //    internal string D_extended_top = "拡頭径";
        //    internal string D_extended_foot = "拡底径";
        //    internal string strength = "鉄筋強度";
        //    internal string[] D_main_circumference_1st = { "杭頭_主筋_径", "軸部_主筋_径", "脚部_主筋_径" };
        //    internal string[] count_main_circumference_1st = { "杭頭_主筋_本数", "軸部_主筋_本数", "脚部_主筋_本数" };
        //    internal string[] D_main_core = { "杭頭_芯鉄筋_径", "軸部_芯鉄筋_径", "脚部_芯鉄筋_径" };
        //    internal string[] count_main_core = { "杭頭_芯鉄筋_本数", "軸部_芯鉄筋_本数", "脚部_芯鉄筋_本数" };
        //    internal string[] D_band = { "杭頭_帯筋_径", "軸部_帯筋_径", "脚部_帯筋_径" };
        //    internal string[] count_band = { "杭頭_帯筋_本数", "軸部_帯筋_本数", "脚部_帯筋_本数" };
        //    internal string[] pitch_band = { "杭頭_帯筋_ピッチ", "軸部_帯筋_ピッチ", "脚部_帯筋_ピッチ" };

        //    //*****インスタンスパラメータ*****************************************************************
        //    internal string MemId = "配置ID";
        //    internal string NameMembers = "配置名";
        //}




        /// <summary>
        /// 鋼管杭
        /// </summary>
        internal class Pile_S
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "鋼管杭";

            //*****タイプパラメータ***********************************************************************
            internal string name = "断面名称";
            //internal string id_order = "継杭の位置";
            internal string length_pile = "杭の長さ";
            internal string D = "軸部径";
            internal string t = "鋼管の厚さ";
            internal string strength = "鋼管の鉄骨強度";
            internal string SecId = "断面ID";

            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string length_all = "杭全長";
        }



        /// <summary>
        /// 既製コンクリート杭 PHC杭
        /// </summary>
        internal class Pile_PHC
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "既製杭 PHC";

            //*****タイプパラメータ***********************************************************************
            internal string name = "断面名称";
            //internal string id_order = "継杭の位置";
            internal string kind = "種類";
            internal string length_pile = "杭の長さ";
            internal string D = "外径";
            internal string t = "厚さ";
            internal string strength_concrete = "構造マテリアル";
            internal string D_PC = "PC鋼棒径";
            internal string N_PC = "PC鋼棒本数";
            internal string strength_PC = "PC鋼棒強度";
            internal string SecId = "断面ID";

            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string length_all = "杭全長";
        }

        /// <summary>
        /// 既製コンクリート杭 ST杭
        /// </summary>
        internal class Pile_ST
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "既製杭 ST";

            //*****タイプパラメータ***********************************************************************
            internal string name = "断面名称";
            //internal string id_order = "継杭の位置";
            internal string kind = "種類";
            internal string length_pile = "杭の長さ";
            internal string D1 = "外径（本体部）";
            internal string D2 = "外径（拡径部）";
            internal string t1 = "厚さ（本体部）";
            internal string t2 = "厚さ（拡径部）";
            internal string strength_concrete = "構造マテリアル";
            internal string D_PC = "PC鋼棒径";
            internal string N_PC = "PC鋼棒本数";
            internal string strength_PC = "PC鋼棒強度";
            internal string SecId = "断面ID";

            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string length_all = "杭全長";
        }


        /// <summary>
        /// 既製コンクリート杭 SC杭
        /// </summary>
        internal class Pile_SC
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "既製杭 SC";

            //*****タイプパラメータ***********************************************************************
            internal string name = "断面名称";
            //internal string id_order = "継杭の位置";
            internal string length_pile = "杭の長さ";
            internal string kind = "種類";
            internal string D = "外径";
            internal string tc = "肉厚(含鋼管)";
            internal string ts = "鋼管の板厚";
            internal string strength_concrete = "構造マテリアル";
            internal string strength_pipe = "鋼管の鉄骨強度";
            internal string SecId = "断面ID";

            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string length_all = "杭全長";
        }


        /// <summary>
        /// 既製コンクリート杭 PRC杭
        /// </summary>
        internal class Pile_PRC
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "既製杭 PRC";

            //*****タイプパラメータ***********************************************************************
            internal string name = "断面名称";
            //internal string id_order = "継杭の位置";
            internal string kind = "種類";
            internal string length_pile = "杭の長さ";
            internal string D = "外径";
            internal string tc = "厚さ";
            internal string strength_concrete = "構造マテリアル";
            internal string D_PC = "PC鋼棒径";
            internal string N_PC = "PC鋼棒本数";
            internal string strength_PC = "PC鋼棒強度";
            internal string D_bar = "異形棒鋼径";
            internal string N_bar = "異形棒鋼本数";
            internal string strength_bar = "異形棒鋼強度";
            internal string SecId = "断面ID";

            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string length_all = "杭全長";
        }


        /// <summary>
        /// 既製コンクリート杭 CPRC杭
        /// </summary>
        internal class Pile_CPRC
        {
            //ロードされているか否か**********************************************************************
            internal bool Loadflg = false;

            //*****マッピングテーブルのファミリ名*****************************************************************************
            internal string FamilyName = "既製杭 CPRC";

            //*****タイプパラメータ***********************************************************************
            internal string name = "断面名称";
            //internal string id_order = "継杭の位置";
            internal string kind = "種類";
            internal string length_pile = "杭の長さ";
            internal string D = "外径";
            internal string tc = "厚さ";
            internal string strength_concrete = "構造マテリアル";
            internal string D_PC = "PC鋼棒径";
            internal string N_PC = "PC鋼棒本数";
            internal string strength_PC = "PC鋼棒強度";
            internal string D_bar = "異形棒鋼径";
            internal string N_bar = "異形棒鋼本数";
            internal string strength_bar = "異形棒鋼強度";
            internal string SecId = "断面ID";

            //*****インスタンスパラメータ*****************************************************************
            internal string MemId = "配置ID";
            internal string NameMembers = "配置名";
            internal string length_all = "杭全長";
        }

        #endregion
    }
}
