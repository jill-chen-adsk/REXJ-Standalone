using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MepDuctPipeTool.Utils
{
  internal static class ErrorLogger
  {
    /// <summary>
    /// 本アドインの.dllと同じパスにlog出力する。
    /// </summary>
    internal static void LogException( Exception ex )
    {
      var logFilePath = CreateLogFilePath();
      var logMessage = CreateLogMessage( ex );
      Write( logFilePath, logMessage );
    }

    private static string CreateLogFilePath()
      => Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) + "\\REXJMEPDuctPipe.log";

    private static string CreateLogMessage( Exception ex )
    {
      var exceptionFrame = new StackTrace( ex, true ).GetFrame( 0 );
      var exceptionMethodName = exceptionFrame?.GetMethod()?.DeclaringType?.FullName ?? "";

      var callerFrame = new StackTrace( ex, true ).GetFrame( 1 );
      var callerMethodName = callerFrame?.GetMethod()?.DeclaringType?.FullName ?? "";

      // セキュリティの観点から、クラス名の大文字部分だけを出力する。
      var encryptedExMethodName = ExtractUppercaseAndDot( exceptionMethodName );
      var encryptedCallerMethodName = ExtractUppercaseAndDot( callerMethodName );

      var exMethodLineNum = exceptionFrame?.GetFileLineNumber();
      var callerMethodLineNum = callerFrame?.GetFileLineNumber();
      var time = DateTime.Now.ToString();

      return $"{time} Exception: {ex.Message} in {encryptedExMethodName} at ({exMethodLineNum}), called by {encryptedCallerMethodName} at ({callerMethodLineNum})";
    }

    private static string ExtractUppercaseAndDot( string str )
    {
      // 大文字のアルファベットに一致する正規表現パターン
      var regex = new Regex( @"[A-Z.]" );
      var matches = regex.Matches( str );


      return string.Concat( matches.Cast<Match>().Select( m => m.Value ) );
    }

    private static void Write( string logFilePath, string message )
    {
      using var writer = new StreamWriter( logFilePath, true );
      writer.WriteLine( message );
    }
  }
}