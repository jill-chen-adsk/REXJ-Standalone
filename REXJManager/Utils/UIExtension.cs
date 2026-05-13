using System ;
using System.Windows.Media.Imaging ;

namespace REXJManager
{
  public static class UiExtension
  {
    /// <summary>
    /// 特定パスの画像リソースをBitmapImageとして取り出す
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static BitmapImage ToResImageInPack( this string path )
    {
      
      var image = new BitmapImage( new Uri( $@"pack://application:,,,/REXJManager;component/{path}", UriKind.Absolute ) ) ;
      return image ;
    }
    
  }
  
  
}