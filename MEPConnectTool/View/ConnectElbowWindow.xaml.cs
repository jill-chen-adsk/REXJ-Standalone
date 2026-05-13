using System ;
using System.Linq ;
using System.Text.RegularExpressions ;
using System.Windows ;
using System.Windows.Controls ;
using System.Windows.Input ;
using Autodesk.Revit.UI ;
using MEPConnectTool.Utils ;

namespace MEPConnectTool.View ;

public partial class ConnectElbowWindow : Window
{
  private UIApplication _uiApplication ;

  public ConnectElbowWindow( UIApplication uiApp )
  {
    _uiApplication = uiApp ;
    InitializeComponent() ;
    KeyDown += OnKeyDownHandler ;
  }

  private void Cancel_OnClick( object sender, RoutedEventArgs e )
  {
    Hide() ;
  }

  private void Place_OnClick( object sender, RoutedEventArgs e )
  {
    Hide() ;

    InnnerRadiusTextBox.Text = Regex.Replace( InnnerRadiusTextBox.Text, @"[^0-9.]", "" ) ;
    HopperLengthTextBox.Text = Regex.Replace( HopperLengthTextBox.Text, @"[^0-9.]", "" ) ;
    var familyName = FamilyPanel.Children.OfType<RadioButton>().FirstOrDefault( x => x.IsChecked.HasValue && x.IsChecked.Value ).Content as string ;
    var innnerRadius = InnerRPanel.Visibility == Visibility.Visible ? double.Parse( InnnerRadiusTextBox.Text ) : 0;
    var hopperLength = HopperLengthPanel.Visibility == Visibility.Visible ? double.Parse( HopperLengthTextBox.Text ) : 0;
    
    Console.WriteLine( $"{familyName} InnerR:{innnerRadius.ToF2()} HopperLen:{hopperLength.ToF2()}" ) ;
    _uiApplication.BeginConnectElbow( familyName ?? "", innnerRadius, hopperLength ) ;
  }
  
  private void OnKeyDownHandler( object sender, KeyEventArgs e )
  {
    if ( e.Key == Key.Escape ) Close() ;
    if ( e.Key == Key.Enter ) Place_OnClick( this, new RoutedEventArgs()) ;
  }

  private void OnFamilyChecked( object sender, RoutedEventArgs e )
  {
        
    if(InnerRPanel is null || HopperLengthPanel is null) return;
    if ( sender is not RadioButton radioButton) return ;
    var content = radioButton.Content.ToString() ;
    
    InnerRPanel!.Visibility = content.Contains( "内R" ) ? Visibility.Visible : Visibility.Collapsed ;
    HopperLengthPanel!.Visibility = (content == "011_角_エルボ_1R" || content == "011_角_エルボ_内R設定") ? Visibility.Visible : Visibility.Collapsed ;
  }
}