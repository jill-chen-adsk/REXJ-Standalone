using System.Text.RegularExpressions ;
using System.Windows ;
using System.Windows.Input ;
using Autodesk.Revit.UI ;
using MEPConnectTool.Utils ;

namespace MEPConnectTool.View ;

public partial class ConnectRectDuctWindow : Window
{
  private UIApplication _uiApplication ;

  public ConnectRectDuctWindow( UIApplication uiApp )
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
    LengthTextBox.Text = Regex.Replace( LengthTextBox.Text, @"[^0-9.]", "" ) ;

    var isFixedLength = RadioButtonLengthFixed.IsChecked == true ;
    var isHopperPreferred = HopperPreferredCheckbox.IsChecked == true ;
    var length = double.Parse( LengthTextBox.Text ) ;

    _uiApplication.BeginConnectParallelRectDuct( isFixedLength, isHopperPreferred, length) ;
  }

  private void OnKeyDownHandler( object sender, KeyEventArgs e )
  {
    if ( e.Key == Key.Escape ) Close() ;
    if ( e.Key == Key.Enter ) Place_OnClick( this, new RoutedEventArgs()) ;
  }
}