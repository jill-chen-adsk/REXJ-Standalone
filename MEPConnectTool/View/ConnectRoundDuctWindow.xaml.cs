using System.Windows ;
using System.Text.RegularExpressions ;
using System.Windows.Input ;
using Autodesk.Revit.UI ;
using MEPConnectTool.Utils ;

namespace MEPConnectTool.View ;

public partial class ConnectRoundDuctWindow : Window
{
  private UIApplication _uiApplication ;
  

  public ConnectRoundDuctWindow(UIApplication uiApp )
  {
    _uiApplication = uiApp ;
    InitializeComponent() ;
    KeyDown += OnKeyDownHandler ;
  }
  
  private void Cancel_OnClick( object sender, RoutedEventArgs e )
  {
    Close() ;
  }

  private void Place_OnClick( object sender, RoutedEventArgs e )
  {
    Close() ;
    var isUse45degElbow = prefer45degElbowCheckbox.IsChecked == true ;
    _uiApplication.BeginConnectParallelRoundDuct( isUse45degElbow ) ;
  }

  private void OnKeyDownHandler( object sender, KeyEventArgs e )
  {
    if ( e.Key == Key.Escape ) Close() ;
    if ( e.Key == Key.Enter ) Place_OnClick( this, new RoutedEventArgs()) ;
  }
}