using System.Collections.Generic ;
using System.ComponentModel ;
using AdWindows = Autodesk.Windows ;

namespace REXJManager
{
  public class MyTreeNode : INotifyPropertyChanged
  {
    private bool _isChecked = true ;
    
    public event PropertyChangedEventHandler PropertyChanged ;

    public string DebugName => $"({Obj.GetType()}) {Path}" ;
    public string Name { get ; set ; }

    public List<MyTreeNode> Children { get ; set ; } = new() ;

    public bool IsChecked
    {
      get => _isChecked ;
      set
      {
        if ( _isChecked == value ) return ;
        
        _isChecked = value ;
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( nameof( IsChecked ) ) ) ;

        switch ( Obj ) {
          case AdWindows.RibbonTab item :
            item.IsVisible = _isChecked ;
            break ;
          case AdWindows.RibbonPanel item :
            item.IsVisible = _isChecked ;
            break ;
          case AdWindows.RibbonSplitButton item :
            item.IsVisible = _isChecked ;
            break ;
          case AdWindows.RibbonSeparator item :
            item.IsVisible = _isChecked ;
            break ;
          case AdWindows.RibbonButton item :
            item.IsVisible = _isChecked ;
            break ;
          case AdWindows.RibbonRowPanel item :
            item.IsVisible = _isChecked ;
            break ;
          case AdWindows.RibbonRowBreak item :
            item.IsVisible = _isChecked ;
            break ;
        }
      }
    }

    public object Obj ;

    public string Path { get ; set ; }

    public bool IsEnabled { get ; set ; } = true ;
  }
}