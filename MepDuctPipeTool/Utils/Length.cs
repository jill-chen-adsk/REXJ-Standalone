using System;
using System.Diagnostics;
using Autodesk.Revit.DB;

namespace MepDuctPipeTool.Utils
{
  [ DebuggerDisplay( "{" + nameof( Meters ) + "} m" ) ]
  public readonly struct Length : IComparable<Length>
  {
    public double Meters { get; }
    public double Centimeters => Meters * 100;
    public double Millimeters => Meters * 1000;

    // TODO 値が負かどうかチェック
    private Length( double meters ) => Meters = meters;

    public static Length FromMeters( double value ) => new Length( value );
    public static Length FromCentimeters( double value ) => FromMeters( value * 1e-2 );
    public static Length FromMillimeters( double value ) => FromMeters( value * 1e-3 );

    public static Length Zero { get; } = FromMeters( 0 );


    public static Length operator +( Length length1, Length length2 ) => FromMeters( length1.Meters + length2.Meters );
    public static Length operator -( Length length1, Length length2 ) => FromMeters( length1.Meters - length2.Meters );

    public static Length operator +( Length length ) => length;
    public static Length operator -( Length length ) => FromMeters( -length.Meters );
    public static Length operator *( double value, Length length ) => FromMeters( length.Meters * value );
    public static Length operator *( Length length, double value ) => FromMeters( length.Meters * value );
    public static Length operator /( Length length, double value ) => FromMeters( length.Meters / value );
    public static double operator /( Length length1, Length length2 ) => length1.Meters / length2.Meters;

    public int CompareTo( Length other ) => Meters.CompareTo( other.Meters );
    public static bool operator <( Length left, Length right ) => left.CompareTo( right ) < 0;
    public static bool operator >( Length left, Length right ) => left.CompareTo( right ) > 0;
    public static bool operator <=( Length left, Length right ) => left.CompareTo( right ) <= 0;
    public static bool operator >=( Length left, Length right ) => left.CompareTo( right ) >= 0;
  }


  public static class UnitConversionExtension
  {
    public static Length RevitUnitsToLength( this double value ) => Length.FromMeters( UnitUtils.ConvertFromInternalUnits( value, UnitTypeId.Meters ) );

    public static double LengthToRevitUnits( this Length length ) => UnitUtils.ConvertToInternalUnits( length.Meters, UnitTypeId.Meters );
  }
}