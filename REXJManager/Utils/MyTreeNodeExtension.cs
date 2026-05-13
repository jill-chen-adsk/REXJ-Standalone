using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using UIFramework ;
using AdWindows = Autodesk.Windows ;

namespace REXJManager
{
  /// <summary>
  /// ボタンなどをツリー表示するときに使っているMyTreeNodeに関連する拡張メソッド
  /// </summary>
  public static class MyTreeNodeExtension
  {
    //名前を持たないRowPanelやSeparatorなどの要素を区別するために入れる文字
    // '\u2060' : ワードジョイナーは表示されない文字だが、エディタによっては記号表示されるので都合がよい。
    private const char CounterStr = '\u2060' ;

    /// <summary>
    /// RibbonControlからMyTreeNodeもしくはパス文字列を作成する
    /// </summary>
    /// <param name="rbnCtrl"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Extract<T>( this AdWindows.RibbonControl rbnCtrl )
    {
      var type = typeof( T ) ;
      var isMyTreeNode = type == typeof( MyTreeNode ) ;
      var isString = type == typeof( string ) ;
      if ( ! isMyTreeNode && ! isString ) return (T)(object)null ;

      var root = new MyTreeNode { Name = "Tabs" } ;
      var sb = new StringBuilder() ;

      // タブ階層
      foreach ( var tab in rbnCtrl.Tabs ) {
        if ( string.IsNullOrEmpty( tab.Id ) ) continue ;
        if ( tab.Id != Resource.TAB_NAME && ( tab.Name == null || ! tab.Name.StartsWith( Resource.TAB_NAME, StringComparison.Ordinal ) ) ) continue ;

        var tabNode = new MyTreeNode { Name = tab.AutomationName.ToSingleLine(), IsChecked = tab.IsVisible, Obj = tab, Path = $"{tab.AutomationName}" } ;
        root.Children.Add( tabNode ) ;
        sb.Append( $"{tabNode.Path}\r\n" ) ;

        //パネル階層
        foreach ( var panel in tab.Panels ) {
          if ( panel.Source.AutomationName == Resource.TXT_CMD_SETTING ) continue ;

          var panelPath = $"{tabNode.Path}/{panel.Source.AutomationName.ToSingleLine()}" ;
          var panelNode = new MyTreeNode { Name = panel.Source.AutomationName.ToSingleLine(), IsChecked = panel.IsVisible, Obj = panel, Path = panelPath } ;

          if ( panelNode.Path.ShouldShow() ) {
            tabNode.Children.Add( panelNode ) ;
            sb.Append( $"{panelNode.Path}\r\n" ) ;
          }
          else {
            if ( isMyTreeNode ) panelNode.IsChecked = false ;
          }

          // 名前のない要素に振る番号
          var rowPanelNo = 0 ;
          var separatorNo = 0 ;

          //パネル要素階層
          foreach ( var item in panel.Source.Items ) {
            switch ( item ) {
              case AdWindows.RibbonRowBreak :
              case AdWindows.RibbonPanelBreak :
                continue ;
            }

            var itemName = item.AutomationName.ToSingleLine() ;
            switch ( item ) {
              case AdWindows.RibbonSeparator :
                itemName = $"<{Resource.PATH_RIBBON_SEPARATOR}>{HiddenSymbols( separatorNo++ )}" ;
                break ;
              case AdWindows.RibbonRowPanel :
                itemName = $"<{Resource.PATH_RIBBON_ROW_PANEL}>{HiddenSymbols( rowPanelNo++ )}" ;
                break ;
            }

            var itemPath = $"{tabNode.Path}/{panel.Source.AutomationName}/{itemName}" ;
            var itemNode = new MyTreeNode { Name = itemName, Obj = item, IsChecked = item.IsVisible, Path = itemPath } ;

            if ( item is AdWindows.RibbonRowPanel ribbonRowPanel ) {
              if ( panel.AutomationName == Resource.TXT_FUKASHI_PANEL_NAME ) continue ;
              foreach ( var rowItem in ribbonRowPanel.Items ) {
                switch ( rowItem ) {
                  case AdWindows.RibbonRowBreak :
                  case RvtRibbonCombo :
                  case AdWindows.RibbonTextBox :
                    continue ;
                }

                var rowItemName = rowItem.AutomationName.ToSingleLine() ;
                var rowItemNode = new MyTreeNode { Name = rowItemName, Obj = rowItem, IsChecked = rowItem.IsVisible } ;
                rowItemNode.Path = $"{itemNode.Path}/{panel.Source.AutomationName}/{rowItemNode.Name}" ;

                if ( rowItemNode.Path.ShouldShow() ) {
                  itemNode.Children.Add( rowItemNode ) ;
                  sb.Append( $"{rowItemNode.Path}\r\n" ) ;
                }
                else {
                  if ( isMyTreeNode ) rowItemNode.IsChecked = false ;
                }
              }
            }

            if ( itemNode.Path.ShouldShow() ) {
              panelNode.Children.Add( itemNode ) ;
              sb.Append( $"{itemNode.Path}\r\n" ) ;
            }
            else {
              if ( isMyTreeNode ) itemNode.IsChecked = false ;
            }

            if ( item is not AdWindows.RibbonSplitButton splitButton ) continue ;
            foreach ( var sbItem in splitButton.Items ) {
              var sbItemNode = new MyTreeNode { Name = sbItem.AutomationName.ToSingleLine(), Obj = sbItem, IsChecked = sbItem.IsVisible } ;
              if ( sbItem is AdWindows.RibbonSeparator ) sbItemNode.Name = $"<{Resource.PATH_RIBBON_SEPARATOR}>{HiddenSymbols( separatorNo++ )}" ;
              sbItemNode.Path = $"{itemNode.Path}/{panel.Source.AutomationName}/{sbItemNode.Name}" ;
              if ( ! sbItemNode.Path.ShouldShow() ) continue ;
              itemNode.Children.Add( sbItemNode ) ;
              sb.Append( $"{sbItemNode.Path}\r\n" ) ;
            }
          }
        }
      }

      if ( isMyTreeNode ) return (T)(object)root.Children.First() ;
      if ( isString ) return (T)(object)sb.ToString() ;

      return (T)(object)null ;
    }


    /// <summary>
    /// ノードにプリセットを読み込む。
    /// 拡張メソッドをやめたほうがよいかも。
    /// </summary>
    /// <param name="node"></param>
    /// <param name="name"></param>
    public static void Load( this MyTreeNode node, string name )
    {
      node.Children.Load( name ) ;
    }

    private static void Load( this IEnumerable<MyTreeNode> node, string name )
    {
      var lines = Preset.LoadPathListFromPresetFile( name ) ;
      if ( lines.Count == 0 ) return ;

      foreach ( var item in node ) {
        ApplyNode( item ) ;
      }

      return ;

      void ApplyNode( MyTreeNode n )
      {
        if ( n == null ) return ;
        n.IsChecked = lines.Contains( n.Path ) ;
        foreach ( var item in n.Children ) {
          item.IsChecked = lines.Contains( item.Path ) ;
          ApplyNode( item ) ;
        }
      }
    }

    /// <summary>
    /// MyTreeNodeのisEnabledをセット
    /// </summary>
    /// <param name="node"></param>
    /// <param name="isEnabled"></param>
    public static void SetIsEnabled( this MyTreeNode node, bool isEnabled )
    {
      Apply( node ) ;
      return ;

      void Apply( MyTreeNode n )
      {
        n.IsEnabled = isEnabled ;
        if ( n.Children.Count == 0 ) return ;
        foreach ( var item in n.Children ) {
          Apply( item ) ;
        }
      }
    }

    /// <summary>
    /// 文字列から改行コード"\r\n"、"\n"を削除
    /// ボタン名に改行コードが入っている場合に改行コードを削除するのに利用
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static string ToSingleLine( this string str )
    {
      return str.Replace( "\r\n", "" ).Replace( "\n", "" ) ;
    }

    private static string HiddenSymbols( int num ) => new( CounterStr, num ) ;
  }
}