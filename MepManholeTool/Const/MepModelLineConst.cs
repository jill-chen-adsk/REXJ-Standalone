namespace MepManholeTool.Const
{
    public static class MepModelLineConst
    {
        #region 機能表示名
        // リボンタブタイトル
        public static readonly string REVITリボン_タブ_タイトル = "REXJ Standalone";
        
        // 機能名。リボンに表示する名称
        public static readonly string REVITリボン_機能名_桝ツール = "Manhole Tool";
        #endregion

        #region プルダウン名
        // 桝ツール_プルダウン名
        public static readonly string REVITリボン_モデル線分作成_プルダウン名_線分 = "Model Line";
        public static readonly string REVITリボン_モデル線分作成_プルダウン名_パラメータ = "Parameter\nMapping";
        #endregion
        
        #region ツールチップ
        // リボンのツールチップ。(リボンにカーソルを当てると表示される。)
        public static readonly string REVITツールチップ_モデル線分 = "Creates model lines and verification view for selected manholes";
        public static readonly string REVITツールチップ_モデルパラメータマッピング = "Maps target parameters for each family";
        #endregion

        #region ヘルプ

        public static readonly string REVITツールヘルプドキュメント = "桝ツール アドイン.pdf" ;
        #endregion
        
        #region 桝の寸法

        public static readonly string 桝パラメータ_桝サイズ = "桝サイズ";
        public static readonly string 桝パラメータ_当該桝地盤レベル = "当該桝地盤レベル";
        public static readonly string 桝パラメータ_配管段差_丸 = "配管段差";
        public static readonly string 桝パラメータ_配管段差 = "桝(管底差)";
        public static readonly string 桝パラメータ_管底高_丸 = "基準レベルからの管底高";
        public static readonly string 桝パラメータ_管底高 = "流出管底";
        public static readonly string 桝パラメータ_泥だまり = "泥だまり";
        public static readonly string 桝パラメータ_桝深さ = "桝深さ";
        public static readonly string 桝パラメータ_出口径 = "出口径";
        public static readonly string 桝パラメータ_記号 = "記号";
        public static readonly string 桝パラメータ_備考 = "備考";
        #endregion

        #region 非機能定数
        // 作成するビュー名
        public static readonly string 桝確認ビュー名 = "桝確認ビュー_REXJ";
        
        // 仮桝記号の先頭文字
        public static readonly string 記号先頭文字 = "(tmp)";
        #endregion
    }
}