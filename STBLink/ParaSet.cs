using System ;
using System.Linq ;
using Autodesk.Revit.DB ;
using Revit = Autodesk.Revit ;

namespace STBLink
{
  class ParaSet
  {
    /// <summary>構造マテリアル
    /// </summary>
    // private const BuiltInParameterGroup Mate = BuiltInParameterGroup.PG_MATERIALS ;

    /// <summary>寸法
    /// </summary>
    // private const BuiltInParameterGroup Sunpou = BuiltInParameterGroup.PG_GEOMETRY ;

    /// <summary>文字
    /// </summary>
    // private const BuiltInParameterGroup Moji = BuiltInParameterGroup.PG_TEXT ;

    /// <summary>構造
    /// </summary>
    // private const BuiltInParameterGroup Kouzou = BuiltInParameterGroup.PG_STRUCTURAL ;

    /// <summary>鉄筋セット
    /// </summary>
    // private const BuiltInParameterGroup Tekkin = BuiltInParameterGroup.PG_REBAR_ARRAY ;

    /// <summary>識別情報
    /// </summary>
    // private const BuiltInParameterGroup Sikibetu = BuiltInParameterGroup.PG_IDENTITY_DATA ;

    #region 柱

    /// <summary>RC柱矩形
    /// </summary>
    /// <param name="fmg"></param>
    /// <param name="clm"></param>
    internal static void SetPara_RCClmRe( FamilyManager fmg, FamilyStructure.RC_Clm_Re clm )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( clm.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.DX, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.DY, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( clm.D_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_reinforcement_2nd_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_reinforcement_axial, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_reinforcement_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_2nd_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_axial, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.depth_cover_X, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.depth_cover_Y, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_X_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_2nd_main_X_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_X_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_2nd_main_X_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_Y_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_2nd_main_Y_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_Y_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_2nd_main_Y_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_band_dir_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_band_dir_Y, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_band, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_bar_spacing_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_bar_spacing_Y, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_bar_spacing, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_axial, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.center_reinforcement_start_X, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.center_reinforcement_end_X, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.center_reinforcement_start_Y, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.center_reinforcement_end_Y, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.kind_reinforcement_corner, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.strength_reinforcement_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.interval_reinforcement, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_total, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_Y, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_total_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_bar_spacing_list, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_axial_list, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.center_reinforcement_X, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.center_reinforcement_Y, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.concrete_reductionrate, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_main_Y, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;

        //インスタンスパラメータ*********************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_start_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_end_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_start_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_end_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_RCClmRo( FamilyManager fmg, FamilyStructure.RC_Clm_Ro clm )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( clm.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.D, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( clm.D_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_reinforcement_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_band, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_band, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_bar_spacing_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_bar_spacing_Y, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_bar_spacing, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.depth_cover_X, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_reinforcement_axial, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_axial, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_axial, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.center_reinforcement_start_X, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_bar_spacing_list, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_axial_list, fmg, SpecTypeId.Int.Integer, GroupTypeId.Materials ) ;
        SetPara( clm.center_reinforcement, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.concrete_reductionrate, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;

        //インスタンスパラメータ*********************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_start_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SClmH( FamilyManager fmg, FamilyStructure.S_Clm_H clm )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( clm.strength_web, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( clm.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type_name, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.base_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SClmBH( FamilyManager fmg, FamilyStructure.S_Clm_BH clm )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( clm.strength_web, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( clm.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type_name, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.base_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SClmBox( FamilyManager fmg, FamilyStructure.S_Clm_Box clm )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( clm.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( clm.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type_name, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.base_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( clm.MemId, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SClmBBox( FamilyManager fmg, FamilyStructure.S_Clm_BBox clm )
    {
      try {
        //タイプパラメータ************************************************                
        SetPara( clm.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( clm.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type_name, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.base_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SClmPipe( FamilyManager fmg, FamilyStructure.S_Clm_Pipe clm )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( clm.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.D, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( clm.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type_name, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.base_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SClmT( FamilyManager fmg, FamilyStructure.S_Clm_T clm )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( clm.strength_web, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( clm.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type_name, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.base_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SClmC( FamilyManager fmg, FamilyStructure.S_Clm_C clm )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( clm.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.B, fmg, SpecTypeId.String.Text, GroupTypeId.Geometry ) ; ;
        SetPara( clm.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.r1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.r2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.Length, GroupTypeId.Text ) ;
        SetPara( clm.SecId, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.side, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type_name, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.base_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SClmL( FamilyManager fmg, FamilyStructure.S_Clm_L clm )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( clm.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.B, fmg, SpecTypeId.String.Text, GroupTypeId.Geometry ) ; ;
        SetPara( clm.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.r1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.r2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.Length, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.SecId, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.side, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type_name, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.base_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SRCClmH( FamilyManager fmg, FamilyStructure.SRC_Clm_H clm )
    {
      try {
        //タイプパラメータ************************************************
        //RC部
        SetPara( clm.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.DX, fmg, SpecTypeId.String.Text, GroupTypeId.Geometry ) ; ;
        SetPara( clm.DY, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.D_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_reinforcement_2nd_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_reinforcement_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_2nd_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.depth_cover_X, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.depth_cover_Y, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_X_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_2nd_main_X_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_X_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_2nd_main_X_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_Y_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_2nd_main_Y_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_Y_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_2nd_main_Y_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_band_dir_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_band_dir_Y, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_band, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_bar_spacing_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_bar_spacing_Y, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_bar_spacing, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.kind_reinforcement_corner, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.strength_reinforcement_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.interval_reinforcement, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_total, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_Y, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_total_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_bar_spacing_list, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.concrete_reductionrate, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_main_Y, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;

        //S部
        SetPara( clm.strength_web, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.typename, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.H, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.direction_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.offset_X, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_Y, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.angle, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.base_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;


        //インスタンスパラメータ*********************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_start_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_end_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_start_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_end_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SRCClmCross( FamilyManager fmg, FamilyStructure.SRC_Clm_Cross clm )
    {
      try {
        //タイプパラメータ************************************************
        //RC部
        SetPara( clm.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.DX, fmg, SpecTypeId.String.Text, GroupTypeId.Geometry ) ; ;
        SetPara( clm.DY, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.D_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_reinforcement_2nd_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_reinforcement_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_2nd_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.depth_cover_X, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.depth_cover_Y, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_X_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_2nd_main_X_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_X_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_2nd_main_X_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_Y_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_2nd_main_Y_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_Y_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_2nd_main_Y_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_band_dir_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_band_dir_Y, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_band, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_bar_spacing_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_bar_spacing_Y, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_bar_spacing, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.kind_reinforcement_corner, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.strength_reinforcement_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.interval_reinforcement, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_total, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_Y, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_total_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_bar_spacing_list, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.concrete_reductionrate, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_main_Y, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;

        //S部
        SetPara( clm.strength_main_X, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_web_X, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_main_Y, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_web_Y, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.XH, fmg, SpecTypeId.Reference.Material, GroupTypeId.Geometry ) ; ;
        SetPara( clm.XB, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.Xt1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.Xt2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.Xr, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.YH, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.YB, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.Yt1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.Yt2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.Yr, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_XX, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_XY, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_YX, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_YY, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.type_X, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type_Y, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.typename_X, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.typename_Y, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.base_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ******************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_start_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_end_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_start_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_end_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SRCClmT( FamilyManager fmg, FamilyStructure.SRC_Clm_T clm )
    {
      try {
        //タイプパラメータ************************************************
        //RC部
        SetPara( clm.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.DX, fmg, SpecTypeId.String.Text, GroupTypeId.Geometry ) ; ;
        SetPara( clm.DY, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.D_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_reinforcement_2nd_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_reinforcement_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_2nd_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.depth_cover_X, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.depth_cover_Y, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_X_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_2nd_main_X_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_X_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_2nd_main_X_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_Y_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_2nd_main_Y_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_Y_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_2nd_main_Y_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_band_dir_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_band_dir_Y, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_band, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_bar_spacing_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_bar_spacing_Y, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_bar_spacing, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.kind_reinforcement_corner, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.strength_reinforcement_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.interval_reinforcement, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_total, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_Y, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main_total_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_bar_spacing_list, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.concrete_reductionrate, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_main_Y, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;

        //S部
        SetPara( clm.strength_main_T, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_web_T, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_main_H, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_web_H, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.direction_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.H, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.CT_A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.CT_B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.CT_t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.CT_t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.CT_r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_HX, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_HY, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_T, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.angle, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.type_H, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type_T, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.typename_H, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.typename_T, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.base_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ******************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_start_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_end_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_start_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_end_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SRCClmH_Rou( FamilyManager fmg, FamilyStructure.SRC_Clm_H_Rou clm )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( clm.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.D, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( clm.D_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_reinforcement_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_band, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_band, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_bar_spacing_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_bar_spacing_Y, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_bar_spacing, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.depth_cover_X, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.strength_reinforcement_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_bar_spacing_list, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.concrete_reductionrate, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;

        //S部
        SetPara( clm.strength_web, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.direction_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.typename, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.H, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_X, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_Y, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.angle, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.base_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_start_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SRCClmCross_Rou( FamilyManager fmg, FamilyStructure.SRC_Clm_Cross_Rou clm )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( clm.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.D, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( clm.D_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_reinforcement_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_band, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_band, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_bar_spacing_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_bar_spacing_Y, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_bar_spacing, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.depth_cover_X, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.strength_reinforcement_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_bar_spacing_list, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.concrete_reductionrate, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;

        //S部
        SetPara( clm.strength_main_X, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_web_X, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_main_Y, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_web_Y, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.XH, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.XB, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.Xt1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.Xt2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.Xr, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.YH, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.YB, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.Yt1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.Yt2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.Yr, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_XX, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_XY, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_YX, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_YY, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.typename_X, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type_X, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.typename_Y, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type_Y, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.base_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_start_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SRCClmT_Rou( FamilyManager fmg, FamilyStructure.SRC_Clm_T_Rou clm )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( clm.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.D, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( clm.D_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_main, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_reinforcement_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_band, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_band, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.D_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_bar_spacing_X, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.count_bar_spacing_Y, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_bar_spacing, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.depth_cover_X, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.strength_reinforcement_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.strength_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( clm.pitch_bar_spacing_list, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( clm.concrete_reductionrate, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;

        //S部
        SetPara( clm.strength_main_T, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_web_T, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_main_H, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_web_H, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.direction_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type_H, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type_T, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.typename_H, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.typename_T, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.H, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.CT_A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.CT_B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.CT_t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.CT_t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.CT_r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_HX, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_HY, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.offset_T, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.angle, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.base_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.thickness_ex_start_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_CFTClmBox( FamilyManager fmg, FamilyStructure.CFT_Clm_Box clm )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( clm.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.direction_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.typename, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.r1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( clm.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.base_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.enbedded_length, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_CFTClmPipe( FamilyManager fmg, FamilyStructure.CFT_Clm_Pipe clm )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( clm.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( clm.kind_column, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.kind_column2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.typename, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.D, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.t, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( clm.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( clm.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.base_type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( clm.enbedded_length, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( clm.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.condition_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.joint_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_top, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( clm.kind_joint_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, clm.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    #endregion

    #region 梁

    /// <summary> RC梁
    /// </summary>
    /// <param name="fmg"></param>
    /// <param name="gir"></param>
    internal static void SetPara_RCGir( FamilyManager fmg, FamilyStructure.RC_Gir gir )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( gir.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.kind_beam, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.kind_beam2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.width_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.width_center, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.width_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.depth_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.depth_center, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.depth_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.BHaunch, fmg, SpecTypeId.Boolean.YesNo, GroupTypeId.Graphics ) ;
        SetPara( gir.haunch_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.haunch_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( gir.D_reinforcement_main_top, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_reinforcement_main_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_reinforcement_2nd_main_top, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_reinforcement_2nd_main_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_top_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_top_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_top_3rd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_bottom_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_bottom_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_bottom_3rd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_top_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_top_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_top_3rd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_bottom_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_bottom_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_bottom_3rd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_stirrup, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_stirrup, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.pitch_stirrup, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_reinforcement_web, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_web, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_bar_spacing, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.pitch_bar_spacing, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.strength_reinforcement_2nd_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_stirrup, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_reinforcement_web, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_left, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_right, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_top, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_bottom, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.interval_reinforcement, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_X_main_top, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_X_main_bottom, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.center_reinforcement_top, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.center_reinforcement_bottom, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.bar_length_start, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.bar_length_end, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.isOutIn, fmg, SpecTypeId.Boolean.YesNo, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( gir.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.thickness_ex_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.thickness_ex_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.thickness_ex_right, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.thickness_ex_left, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_H, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_V, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, gir.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SGirH( FamilyManager fmg, FamilyStructure.S_Gir_H gir )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( gir.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( gir.strength_web, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.kind_beam, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.kind_beam2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.isOutIn, fmg, SpecTypeId.Boolean.YesNo, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.haunch_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.haunch_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.kind_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( gir.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_H, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_V, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.joint_start, fmg, SpecTypeId.Number, GroupTypeId.Geometry, true ) ;
        SetPara( gir.joint_end, fmg, SpecTypeId.Number, GroupTypeId.Geometry, true ) ;
        SetPara( gir.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.future_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, gir.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SGirBH( FamilyManager fmg, FamilyStructure.S_Gir_BH gir )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( gir.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( gir.strength_web, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.kind_beam, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.kind_beam2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.isOutIn, fmg, SpecTypeId.Boolean.YesNo, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.haunch_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.haunch_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.kind_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        //インスタンスパラメータ*********************************************
        SetPara( gir.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_H, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_V, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.joint_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.joint_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.future_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, gir.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SGirC( FamilyManager fmg, FamilyStructure.S_Gir_C gir )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( gir.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( gir.strength, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.kind_beam, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.kind_beam2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.isOutIn, fmg, SpecTypeId.Boolean.YesNo, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.H, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.r1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.r2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.side, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.haunch_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.haunch_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.kind_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( gir.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_H, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_V, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.joint_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.joint_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.future_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, gir.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SGirL( FamilyManager fmg, FamilyStructure.S_Gir_L gir )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( gir.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( gir.strength, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.kind_beam, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.kind_beam2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.isOutIn, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.r1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.r2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.side, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.haunch_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.haunch_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.kind_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( gir.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_H, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_V, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.joint_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.joint_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.future_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, gir.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SGirLipC( FamilyManager fmg, FamilyStructure.S_Gir_LipC gir )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( gir.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( gir.strength, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.kind_beam, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.kind_beam2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.isOutIn, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.H, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.C, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.t, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.side, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.haunch_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.haunch_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.kind_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( gir.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_H, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_V, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.joint_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.joint_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.future_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, gir.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_RCCGir( FamilyManager fmg, FamilyStructure.RC_CGir gir )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( gir.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.kind_beam, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.kind_beam2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.isOutIn, fmg, SpecTypeId.Boolean.YesNo, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.width_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.width_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.depth_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.depth_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.haunch_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.haunch_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( gir.D_reinforcement_main_top, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_reinforcement_main_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_reinforcement_2nd_main_top, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_reinforcement_2nd_main_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_top_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_top_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_top_3rd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_bottom_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_bottom_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_bottom_3rd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_top_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_top_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_top_3rd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_bottom_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_bottom_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_bottom_3rd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_stirrup, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_stirrup, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.pitch_stirrup, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_reinforcement_web, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_web, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_bar_spacing, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.pitch_bar_spacing, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.strength_reinforcement_2nd_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_stirrup, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_reinforcement_web, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_left, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_right, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_top, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_bottom, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.interval_reinforcement, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_X_main_top, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_X_main_bottom, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.center_reinforcement_top, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.center_reinforcement_bottom, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.bar_length_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.bar_length_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( gir.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.thickness_ex_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.thickness_ex_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.thickness_ex_right, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.thickness_ex_left, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_H, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_V, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, gir.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SCGirH( FamilyManager fmg, FamilyStructure.S_CGir_H gir )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( gir.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( gir.kind_beam, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.kind_beam2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.strength_web, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.haunch_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.haunch_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( gir.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_H, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_V, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.joint_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.joint_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, gir.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SRCGirH( FamilyManager fmg, FamilyStructure.SRC_Gir gir )
    {
      try {
        //タイプパラメータ************************************************
        //RC部
        SetPara( gir.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.kind_beam, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.kind_beam2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.isOutIn, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.width_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.width_center, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.width_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.depth_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.depth_center, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.depth_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.BHaunch, fmg, SpecTypeId.Boolean.YesNo, GroupTypeId.Graphics ) ;
        SetPara( gir.haunch_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.haunch_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( gir.D_reinforcement_main_top, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_reinforcement_main_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_reinforcement_2nd_main_top, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_reinforcement_2nd_main_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_top_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_top_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_top_3rd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_bottom_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_bottom_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_bottom_3rd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_top_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_top_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_top_3rd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_bottom_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_bottom_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_bottom_3rd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_stirrup, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_stirrup, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.pitch_stirrup, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_reinforcement_web, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_web, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_bar_spacing, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.pitch_bar_spacing, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_reinforcement_2nd_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_stirrup, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_reinforcement_web, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_left, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_left, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_right, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_top, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_bottom, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.interval_reinforcement, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_X_main_top, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_X_main_bottom, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.center_reinforcement_top, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.center_reinforcement_bottom, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        //S部
        SetPara( gir.strength_web, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.offset, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.level, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( gir.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.thickness_ex_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.thickness_ex_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.thickness_ex_right, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.thickness_ex_left, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_H, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_V, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.joint_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.joint_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, gir.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SRCCGirH( FamilyManager fmg, FamilyStructure.SRC_CGir gir )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( gir.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.isOutIn, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.kind_beam, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.kind_beam2, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.width_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.width_center, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.width_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.depth_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.depth_center, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.depth_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.BHaunch, fmg, SpecTypeId.Boolean.YesNo, GroupTypeId.Graphics ) ;
        SetPara( gir.haunch_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.haunch_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( gir.D_reinforcement_main_top, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_reinforcement_main_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_reinforcement_2nd_main_top, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_reinforcement_2nd_main_bottom, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_top_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_top_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_top_3rd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_bottom_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_bottom_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_main_bottom_3rd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_top_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_top_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_top_3rd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_bottom_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_bottom_2nd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_2nd_main_bottom_3rd, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_stirrup, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_stirrup, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.pitch_stirrup, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_reinforcement_web, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_web, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.D_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_bar_spacing, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.pitch_bar_spacing, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_reinforcement_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_reinforcement_2nd_main, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_stirrup, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_reinforcement_web, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.strength_bar_spacing, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_left, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_right, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_top, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.depth_cover_bottom, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.interval_reinforcement, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_X_main_top, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.count_X_main_bottom, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( gir.center_reinforcement_top, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.center_reinforcement_bottom, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( gir.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        //S部
        SetPara( gir.strength_web, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( gir.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( gir.offset, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( gir.level, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( gir.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.thickness_ex_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.thickness_ex_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.thickness_ex_right, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.thickness_ex_left, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_haunch_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_H, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.type_haunch_V, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.joint_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.joint_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( gir.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, gir.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    #endregion

    #region ブレース

    internal static void SetPara_SBraH( FamilyManager fmg, FamilyStructure.S_Bra_H bra )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( bra.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( bra.strength_web, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( bra.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( bra.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.kind_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( bra.MemId, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.future_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, bra.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SBraBH( FamilyManager fmg, FamilyStructure.S_Bra_BH bra )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( bra.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( bra.strength_web, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( bra.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( bra.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.kind_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( bra.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.future_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, bra.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SBraBox( FamilyManager fmg, FamilyStructure.S_Bra_Box bra )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( bra.strength, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( bra.kind_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.H, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.r, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( bra.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( bra.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.future_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, bra.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SBraBBox( FamilyManager fmg, FamilyStructure.S_Bra_BBox bra )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( bra.strength, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( bra.kind_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.H, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( bra.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;


        //インスタンスパラメータ*********************************************
        SetPara( bra.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.future_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, bra.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SBraPipe( FamilyManager fmg, FamilyStructure.S_Bra_Pipe bra )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( bra.strength, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( bra.kind_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.D, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.t, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( bra.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( bra.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.future_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, bra.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SBraC( FamilyManager fmg, FamilyStructure.S_Bra_C bra )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( bra.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( bra.strength, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( bra.H, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.r1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.r2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.side, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.kind_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( bra.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.future_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, bra.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SBraL( FamilyManager fmg, FamilyStructure.S_Bra_L bra )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( bra.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( bra.strength, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( bra.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.r1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.r2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.side, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.kind_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( bra.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.future_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, bra.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SBraLipC( FamilyManager fmg, FamilyStructure.S_Bra_LipC bra )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( bra.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( bra.strength, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( bra.H, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.A, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.C, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.t, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.side, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.kind_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( bra.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.future_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, bra.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SBraFB( FamilyManager fmg, FamilyStructure.S_Bra_FB bra )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( bra.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( bra.kind_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.t, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( bra.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( bra.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.future_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, bra.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_SBraRollBar( FamilyManager fmg, FamilyStructure.S_Bra_RollBar bra )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( bra.strength_main, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( bra.kind_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.shape, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( bra.D, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( bra.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( bra.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( bra.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.condition_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_start, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.joint_end, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_start, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.kind_joint_end, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( bra.future_brace, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, bra.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    #endregion

    /// <summary>構造床
    /// </summary>
    /// <param name="Cate"></param>
    /// <param name="el"></param>
    /// <param name="sla"></param>
    internal static void SetPara_Slab( string Cate, Element el, FamilyStructure.Slab sla )
    {
      Transaction tran = new Transaction( Commons.doc, "構造床パラメータの追加" ) ;
      try {
        tran.Start() ;

        //タイプパラメータ************************************************
        SetPara_el( sla.isEarthen, el, Cate, SpecTypeId.Boolean.YesNo, GroupTypeId.IdentityData ) ; ;
        SetPara_el( sla.isCanti, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara_el( sla.FigureType, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara_el( sla.product_type, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara_el( sla.product_company, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara_el( sla.product_name, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara_el( sla.product_code, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara_el( sla.depth_center, el, Cate, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara_el( sla.depth_tip, el, Cate, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara_el( sla.depth_base, el, Cate, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara_el( sla.product_depth, el, Cate, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara_el( sla.length_haunch, el, Cate, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara_el( sla.name, el, Cate, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara_el( sla.ArrengementType, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( sla.D1, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( sla.D2, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( sla.pitch, el, Cate, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara_el( sla.T_D1, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( sla.T_D2, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( sla.T_pitch, el, Cate, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara_el( sla.addD, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( sla.addpitch, el, Cate, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara_el( sla.D_op, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( sla.count_op, el, Cate, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara_el( sla.length_op, el, Cate, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara_el( sla.strength, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( sla.depth_cover_top, el, Cate, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara_el( sla.depth_cover_bottom, el, Cate, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara_el( sla.SecId, el, Cate, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara_el( sla.D_bar_spacing, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( sla.pitch_bar_spacing, el, Cate, SpecTypeId.Number, GroupTypeId.RebarArray ) ;

        //インスタンスパラメータ*********************************************
        SetPara_el( sla.MemId, el, Cate, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara_el( sla.NameMembers, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara_el( sla.thickness_ex_upper, el, Cate, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara_el( sla.thickness_ex_bottom, el, Cate, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara_el( sla.dir_load, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara_el( sla.angle_load, el, Cate, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara_el( sla.isFoundation, el, Cate, SpecTypeId.Boolean.YesNo, GroupTypeId.IdentityData, true ) ;
        SetPara_el( sla.type_haunch, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara_el( sla.kind_slab, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara_el( sla.kind_structure, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;

        tran.Commit() ;
      }
      catch ( Exception ) {
        tran.RollBack() ;
        LogData.AddLog( LogData.LogKind.Error, 3000, "構造床ファミリへのパラメータ追加に失敗しました。" ) ;
      }
    }

    /// <summary>構造壁
    /// </summary>
    /// <param name="Cate"></param>
    /// <param name="el"></param>
    /// <param name="wal"></param>
    internal static void SetPara_Wall( string Cate, Element el, FamilyStructure.Wall wal )
    {
      Transaction tran = new Transaction( Commons.doc, "構造壁パラメータの追加" ) ;
      try {
        tran.Start() ;

        //タイプパラメータ************************************************                
        SetPara_el( wal.name, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara_el( wal.ArrengementType, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.strength, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.D, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.D2, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.pitch, el, Cate, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.D_inout, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.D2_inout, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.pitch_inout, el, Cate, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.D_Edge, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.count_Edge, el, Cate, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.D_op, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.count_op, el, Cate, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.length_op, el, Cate, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.kind_form, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara_el( wal.isTip_line, el, Cate, SpecTypeId.Boolean.YesNo, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.depth_T, el, Cate, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara_el( wal.depth_H, el, Cate, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara_el( wal.depth_T1, el, Cate, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara_el( wal.depth_H1, el, Cate, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara_el( wal.depth_H2, el, Cate, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara_el( wal.depth_H3, el, Cate, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara_el( wal.strength_Tip, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.D_Tip, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.pitch_Tip, el, Cate, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.count_Tip, el, Cate, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.D_Edge_Para, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.count_Edge_Para, el, Cate, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.depth_cover_outside, el, Cate, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.depth_cover_inside, el, Cate, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.SecId, el, Cate, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara_el( wal.D_bar_spacing, el, Cate, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara_el( wal.pitch_bar_spacing, el, Cate, SpecTypeId.Number, GroupTypeId.RebarArray ) ;

        //インスタンスパラメータ*********************************************
        SetPara_el( wal.kind_structure, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara_el( wal.kind_layout, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara_el( wal.kind_wall, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara_el( wal.type_outside, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara_el( wal.isPress, el, Cate, SpecTypeId.Boolean.YesNo, GroupTypeId.IdentityData, true ) ;
        SetPara_el( wal.MemId, el, Cate, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara_el( wal.NameMembers, el, Cate, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara_el( wal.thickness_ex_right, el, Cate, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara_el( wal.thickness_ex_left, el, Cate, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara_el( wal.slit_upper, el, Cate, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara_el( wal.slit_bottom, el, Cate, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara_el( wal.slit_left, el, Cate, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara_el( wal.slit_right, el, Cate, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara_el( wal.direction, el, Cate, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;

        tran.Commit() ;
      }
      catch ( Exception ) {
        tran.RollBack() ;
        LogData.AddLog( LogData.LogKind.Error, 3000, "構造壁ファミリへのパラメータ追加に失敗しました。" ) ;
      }
    }

    #region 基礎

    /// <summary>基礎矩形
    /// </summary>
    /// <param name="fmg"></param>
    /// <param name="footing"></param>
    internal static void SetPara_Foundation_Rect( FamilyManager fmg, FamilyStructure.Foundation_Rect footing )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( footing.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( footing.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( footing.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( footing.depth_cover_top, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( footing.depth_cover_bottom, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( footing.depth_cover_side, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( footing.DX, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.DY, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.depth, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.strength, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( footing.D, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( footing.count, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;

        //インスタンスパラメータ*********************************************
        SetPara( footing.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_start_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_end_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_start_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_end_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, footing.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    /// <summary>基礎矩形テーパー
    /// </summary>
    /// <param name="fmg"></param>
    /// <param name="footing"></param>
    internal static void SetPara_Foundation_Tapered_Rect( FamilyManager fmg, FamilyStructure.Foundation_Tapered_Rect footing )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( footing.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( footing.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( footing.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( footing.depth_cover_top, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( footing.depth_cover_bottom, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( footing.depth_cover_side, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( footing.DX, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.DY, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.t_DX, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.t_DY, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.t_offset_X, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.t_offset_Y, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.depth_base, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.depth_tip, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.strength, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( footing.D, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( footing.count, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;

        //インスタンスパラメータ*********************************************
        SetPara( footing.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_start_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_end_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_start_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_end_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, footing.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    /// <summary>基礎三角
    /// </summary>
    /// <param name="fmg"></param>
    /// <param name="footing"></param>
    internal static void SetPara_Foundation_Triangle( FamilyManager fmg, FamilyStructure.Foundation_Triangle footing )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( footing.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( footing.DX, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.DY, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.depth, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( footing.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( footing.strength, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( footing.D, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( footing.count, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( footing.depth_cover_top, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( footing.depth_cover_bottom, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( footing.depth_cover_side, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;

        //インスタンスパラメータ*********************************************
        SetPara( footing.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_start_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_end_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_start_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_end_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, footing.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_Foundation_ETriangle( FamilyManager fmg, FamilyStructure.Foundation_Equi_Triangle footing )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( footing.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( footing.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.C, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.depth, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( footing.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( footing.strength, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( footing.D, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( footing.count, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( footing.depth_cover_top, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( footing.depth_cover_bottom, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( footing.depth_cover_side, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;


        //インスタンスパラメータ*********************************************
        SetPara( footing.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_start_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_end_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_start_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_end_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, footing.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_Foundation_Octagon( FamilyManager fmg, FamilyStructure.Foundation_Octagon footing )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( footing.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( footing.DX, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.DY, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.CX1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.CY1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.CX2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.CY2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.CX3, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.CY3, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.CX4, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.CY4, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.depth, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( footing.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( footing.strength, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( footing.D, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( footing.count, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( footing.depth_cover_top, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( footing.depth_cover_bottom, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( footing.depth_cover_side, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;

        //インスタンスパラメータ*********************************************
        SetPara( footing.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_start_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_end_X, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_start_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_end_Y, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_top, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.thickness_ex_bottom, fmg, SpecTypeId.Number, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, footing.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_Foundation_Continuous( FamilyManager fmg, FamilyStructure.Foundation_Continuous footing )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( footing.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( footing.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( footing.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( footing.depth_cover_top, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( footing.depth_cover_bottom, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( footing.depth_cover_side, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( footing.B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.t_B, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.depth_base, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.depth_tip, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( footing.strength, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( footing.D, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( footing.count, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( footing.pitch, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( footing.type, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( footing.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( footing.length_ex_start, fmg, SpecTypeId.Length, GroupTypeId.Geometry, true ) ;
        SetPara( footing.length_ex_end, fmg, SpecTypeId.Length, GroupTypeId.Geometry, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, footing.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_Castinpile( FamilyManager fmg, FamilyStructure.Pile pile )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( pile.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( pile.length_all, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.length_head, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.length_foot, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.length_foot_Revit, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.D, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.D_extended_foot, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.D_extended_top, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( pile.D_main_circumference_1st, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( pile.count_main_circumference_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( pile.D_main_core, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( pile.count_main_core, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( pile.D_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( pile.pitch_band, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( pile.strength_main_circumference_1st, fmg, SpecTypeId.String.Text, GroupTypeId.Materials ) ;
        SetPara( pile.strength_main_core, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( pile.strength_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( pile.depth_cover, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( pile.depth_cover_top, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( pile.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( pile.zeroLength, fmg, SpecTypeId.Boolean.YesNo, GroupTypeId.IdentityData, false, true, 0 ) ;

        //インスタンスパラメータ*********************************************
        SetPara( pile.kind_structure, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( pile.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( pile.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, pile.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    internal static void SetPara_Precastpile( FamilyManager fmg, FamilyStructure.Pile_2 pile )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( pile.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( pile.straight_D, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.straight_length, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.ef_D_axial, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.ef_D_extended_foot, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.ef_length_axial, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.ef_length_foot, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.et_D_extended_top, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.et_D_axial, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.et_length_head, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.et_length_axial, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.etf_D_extended_top, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.etf_D_axial, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.etf_D_extended_foot, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.etf_length_head, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.etf_length_axial, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.etf_length_foot, fmg, SpecTypeId.Length, GroupTypeId.Structural ) ;
        SetPara( pile.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        SetPara( pile.D_main_circumference_1st, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( pile.count_main_circumference_1st, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( pile.D_main_core, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( pile.count_main_core, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( pile.D_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( pile.pitch_band, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( pile.strength_main_circumference_1st, fmg, SpecTypeId.String.Text, GroupTypeId.Materials ) ;
        SetPara( pile.strength_main_core, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( pile.strength_band, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( pile.depth_cover, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( pile.depth_cover_top, fmg, SpecTypeId.Number, GroupTypeId.RebarArray ) ;
        SetPara( pile.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( pile.zeroLength, fmg, SpecTypeId.Boolean.YesNo, GroupTypeId.IdentityData, false, true, 0 ) ;

        //インスタンスパラメータ*********************************************
        SetPara( pile.kind_structure, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( pile.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( pile.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, pile.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }


    /// <summary>
    /// パラメータ追加 鋼管杭
    /// </summary>
    /// <param name="fmg"></param>
    /// <param name="pile"></param>
    internal static void SetPara_Pile( FamilyManager fmg, FamilyStructure.Pile_S pile )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( pile.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        //SetPara(pile.id_order, fmg, SpecTypeId.Int.Integer, Sikibetu);
        SetPara( pile.length_pile, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.D, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.t, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.strength, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( pile.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( pile.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( pile.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( pile.length_all, fmg, SpecTypeId.Length, GroupTypeId.Geometry, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, pile.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    /// <summary>
    /// パラメータ追加 既製コンクリート杭 PHC杭
    /// </summary>
    /// <param name="fmg"></param>
    /// <param name="pile"></param>
    internal static void SetPara_Pile( FamilyManager fmg, FamilyStructure.Pile_PHC pile )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( pile.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        //SetPara(pile.id_order, fmg, SpecTypeId.Int.Integer, Sikibetu);
        SetPara( pile.kind, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( pile.length_pile, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.D, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.t, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( pile.D_PC, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.N_PC, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( pile.strength_PC, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( pile.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( pile.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( pile.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( pile.length_all, fmg, SpecTypeId.Length, GroupTypeId.Geometry, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, pile.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    /// <summary>
    /// パラメータ追加 既製コンクリート杭 ST杭
    /// </summary>
    /// <param name="fmg"></param>
    /// <param name="pile"></param>
    internal static void SetPara_Pile( FamilyManager fmg, FamilyStructure.Pile_ST pile )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( pile.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        //SetPara(pile.id_order, fmg, SpecTypeId.Int.Integer, Sikibetu);
        SetPara( pile.kind, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( pile.length_pile, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.D1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.D2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.t1, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.t2, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( pile.D_PC, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.N_PC, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( pile.strength_PC, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( pile.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( pile.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( pile.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( pile.length_all, fmg, SpecTypeId.Length, GroupTypeId.Geometry, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, pile.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    /// <summary>
    /// パラメータ追加 既製コンクリート杭 SC杭
    /// </summary>
    /// <param name="fmg"></param>
    /// <param name="pile"></param>
    internal static void SetPara_Pile( FamilyManager fmg, FamilyStructure.Pile_SC pile )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( pile.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        //SetPara(pile.id_order, fmg, SpecTypeId.Int.Integer, Sikibetu);
        SetPara( pile.kind, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( pile.length_pile, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.D, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.tc, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.ts, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( pile.strength_pipe, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( pile.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( pile.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( pile.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( pile.length_all, fmg, SpecTypeId.Length, GroupTypeId.Geometry, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, pile.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    /// <summary>
    /// パラメータ追加 既製コンクリート杭 PRC杭
    /// </summary>
    /// <param name="fmg"></param>
    /// <param name="pile"></param>
    internal static void SetPara_Pile( FamilyManager fmg, FamilyStructure.Pile_PRC pile )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( pile.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        //SetPara(pile.id_order, fmg, SpecTypeId.Int.Integer, Sikibetu);
        SetPara( pile.kind, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( pile.length_pile, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.D, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.tc, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( pile.D_PC, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.N_PC, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( pile.strength_PC, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( pile.D_bar, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( pile.N_bar, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( pile.strength_bar, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( pile.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( pile.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( pile.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( pile.length_all, fmg, SpecTypeId.Length, GroupTypeId.Geometry, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, pile.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    /// <summary>
    /// パラメータ追加 既製コンクリート杭 CPRC杭
    /// </summary>
    /// <param name="fmg"></param>
    /// <param name="pile"></param>
    internal static void SetPara_Pile( FamilyManager fmg, FamilyStructure.Pile_CPRC pile )
    {
      try {
        //タイプパラメータ************************************************
        SetPara( pile.name, fmg, SpecTypeId.String.Text, GroupTypeId.Text ) ;
        //SetPara(pile.id_order, fmg, SpecTypeId.Int.Integer, Sikibetu);
        SetPara( pile.kind, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData ) ; ;
        SetPara( pile.length_pile, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.D, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.tc, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.strength_concrete, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( pile.D_PC, fmg, SpecTypeId.Length, GroupTypeId.Geometry ) ; ;
        SetPara( pile.N_PC, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;
        SetPara( pile.strength_PC, fmg, SpecTypeId.Reference.Material, GroupTypeId.Materials ) ;
        SetPara( pile.D_bar, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( pile.N_bar, fmg, SpecTypeId.Int.Integer, GroupTypeId.RebarArray ) ;
        SetPara( pile.strength_bar, fmg, SpecTypeId.String.Text, GroupTypeId.RebarArray ) ;
        SetPara( pile.SecId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData ) ; ;

        //インスタンスパラメータ*********************************************
        SetPara( pile.MemId, fmg, SpecTypeId.Int.Integer, GroupTypeId.IdentityData, true ) ;
        SetPara( pile.NameMembers, fmg, SpecTypeId.String.Text, GroupTypeId.IdentityData, true ) ;
        SetPara( pile.length_all, fmg, SpecTypeId.Length, GroupTypeId.Geometry, true ) ;
      }
      catch ( Exception ) {
        LogData.AddLog( LogData.LogKind.Error, 3000, pile.FamilyName + " のパラメータ追加に失敗しました。" ) ;
      }
    }

    #endregion


    internal static bool SetPara( string st, FamilyManager fmg, ForgeTypeId type, ForgeTypeId group, bool flg = false, bool setdef = false, object defval = null )
    {
      if ( group is null ) group = new Revit.DB.ForgeTypeId( string.Empty ) ;
      
      bool ret = false ;
      if ( st == "" ) {
        return ret ;
      }

      if ( fmg.get_Parameter( st ) != null ) {
        return ret ;
      }

      DefinitionFile defFile = Commons.doc.Application.OpenSharedParameterFile() ;
      DefinitionGroups defGroups = defFile.Groups ;
      DefinitionGroup g = defGroups.get_Item( RevitLNK.groupName ) ;
      Definition d = g.Definitions.get_Item( st ) ;

      if ( d == null ) {
        ExternalDefinitionCreationOptions opt = new ExternalDefinitionCreationOptions( st, type ) ;
        g.Definitions.Create( opt ) ;
        d = g.Definitions.get_Item( st ) ;
        ret = true ;
      }

      if ( fmg.get_Parameter( st ) == null ) {
        ExternalDefinition ed = g.Definitions.get_Item( st ) as ExternalDefinition ;
        fmg.AddParameter( ed, group, flg ) ;
        ret = true ;

        if ( setdef && defval != null ) {
          FamilyParameter p = fmg.get_Parameter( st ) ;
          if ( p != null ) {
            //必要になったら随時追加
            if ( p.Definition.GetDataType() == SpecTypeId.Boolean.YesNo ) {
              fmg.Set( p, (int)defval ) ;
            }
          }
        }
      }

      return ret ;
    }

    internal static bool SetPara( string[] st, FamilyManager fmg, ForgeTypeId type, ForgeTypeId group , bool flg = false )
    {
      if ( group is null ) group = new Revit.DB.ForgeTypeId( string.Empty ) ;
      bool ret = true ;
      for ( int i = 0 ; i < st.Count() ; i++ ) {
        ret = SetPara( st[ i ], fmg, type, group, flg ) ;
      }

      return ret ;
    }

    internal static bool SetPara_el( string st, Element element, string cateName, ForgeTypeId type, ForgeTypeId group , bool flg = false )
    {
      if ( group is null ) group = new Revit.DB.ForgeTypeId( string.Empty ) ;
      bool ret = false ;
      if ( st == "" ) {
        return ret ;
      }

      Parameter p = element.LookupParameter( st ) ;
      if ( p != null ) {
        return ret ;
      }

      DefinitionFile defFile = Commons.doc.Application.OpenSharedParameterFile() ;
      DefinitionGroups defGroups = defFile.Groups ;
      DefinitionGroup g = defGroups.get_Item( RevitLNK.groupName ) ;
      Definition d = g.Definitions.get_Item( st ) ;

      Category Cate = Commons.doc.Settings.Categories.get_Item( cateName ) ;
      CategorySet CSet = Commons.doc.Application.Create.NewCategorySet() ;
      CSet.Insert( Cate ) ;


      TypeBinding TBind = Commons.doc.Application.Create.NewTypeBinding( CSet ) ;
      InstanceBinding IBind = Commons.doc.Application.Create.NewInstanceBinding( CSet ) ;


      if ( d == null ) {
        ExternalDefinitionCreationOptions opt = new ExternalDefinitionCreationOptions( st, type ) ;
        g.Definitions.Create( opt ) ;
        d = g.Definitions.get_Item( st ) ;
        ret = true ;
      }

      if ( Commons.doc.ParameterBindings.Contains( d ) ) {
        if ( flg ) {
          IBind = (InstanceBinding)Commons.doc.ParameterBindings.get_Item( d ) ;
          IBind.Categories.Insert( Cate ) ;
        }
        else {
          TBind = (TypeBinding)Commons.doc.ParameterBindings.get_Item( d ) ;
          TBind.Categories.Insert( Cate ) ;
        }

        Commons.doc.ParameterBindings.Remove( d ) ;
        ret = true ;
      }

      if ( flg ) {
        Commons.doc.ParameterBindings.Insert( d, IBind, group ) ;
      }
      else {
        Commons.doc.ParameterBindings.Insert( d, TBind, group ) ;
      }

      return ret ;
    }

    internal static bool SetPara_el( string[] st, Element element, string cateName, ForgeTypeId type, ForgeTypeId group, bool flg = false )
    {
      if ( group is null ) group = new Revit.DB.ForgeTypeId( string.Empty ) ;
      bool ret = true ;
      for ( int i = 0 ; i < st.Count() ; i++ ) {
        ret = SetPara_el( st[ i ], element, cateName, type, group, flg ) ;
      }

      return ret ;
    }
  }
}