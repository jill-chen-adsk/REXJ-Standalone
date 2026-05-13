using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
// ReSharper disable All

namespace MepManholeTool.Models
{
    public class RoutingParameter : INotifyPropertyChanged
    {
        private MasuItem? _masuItem;
        private double _gradientDenominator;
        private double _gradientDenominatorDifference;
        private MasuToolHeight _requiredBottomHeight;
        private MasuToolHeight _bottomHeight;
        private MasuToolHeight _checkBottomHeight;
        public event PropertyChangedEventHandler? HeightChanged;

        public MasuItem? MasuItem
        {
            get => _masuItem;
            set
            {
                _masuItem = value;
                OnPropertyChanged(nameof(MasuItem));
            }
        }

        /// <summary>
        /// 勾配分母
        /// </summary>
        public double GradientDenominator
        {
            get => _gradientDenominator;
            set
            {
                _gradientDenominator = value;
                OnPropertyChanged(nameof(GradientDenominator));
                
                HeightChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GradientDenominator)));
            }
        }

        public double GradientDenominatorDifference
        {
            get => _gradientDenominatorDifference;
            set
            {
                _gradientDenominatorDifference = value;
                OnPropertyChanged(nameof(GradientDenominatorDifference));
            }
        }
        
        /// <summary>
        /// 配管長
        /// </summary>
        public double PipeLength { get; set; }
        
        /// <summary>
        /// 桝深さ
        /// </summary>
        public double Depth { get; set; }

        /// <summary>
        /// 基準レベルからの管底高
        /// </summary>
        public double BaseBottomHeight { get; set; }

        /// <summary>
        /// 必要管底高
        /// </summary>
        public MasuToolHeight RequiredBottomHeight
        {
            get => _requiredBottomHeight;
            set
            {
                _requiredBottomHeight = value;
                OnPropertyChanged(nameof(RequiredBottomHeight));
            }
        }

        /// <summary>
        /// 設定管底高
        /// </summary>
        public MasuToolHeight BottomHeight
        {
            get => _bottomHeight;
            set
            {
                _bottomHeight = value;
                OnPropertyChanged(nameof(BottomHeight));
                
                HeightChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BottomHeight))) ;
            }
        }

        /// <summary>
        /// 管底高さ（入力値）
        /// </summary>
        public MasuToolHeight CheckBottomHeight
        {
            get => _checkBottomHeight;
            set
            {
                _checkBottomHeight = value ;
                OnPropertyChanged( nameof( CheckBottomHeight ) ) ;

                if ( _checkBottomHeight != null && _checkBottomHeight.Value != 0 ) {
                    BottomHeight.Value = _checkBottomHeight.Value ;
                }
            }
        }
        
        private bool _enableTopo = false;

        public bool EnableTopo
        {
            get => _enableTopo;
            set
            {
                _enableTopo = value;
                OnPropertyChanged(nameof(EnableTopo));
            }
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// 一括設定パラメータ
    /// </summary>
    public class BulkSetting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        public event PropertyChangedEventHandler? BulkSettingChanged;
        
        private double bulkGroundLevel;
        private double bulkOutputDiameter;
        private double bulkMudPuddle;
        private double bulkPipeStep;
        private double gradientDenominator;
        
        public double BulkGroundLevel
        {
            get => bulkGroundLevel;
            set
            {
                bulkGroundLevel = value;
                OnPropertyChanged();
                
                BulkSettingChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BulkGroundLevel)));
            }
        }

        public double BulkOutputDiameter
        {
            get => bulkOutputDiameter;
            set
            {
                bulkOutputDiameter = value;
                OnPropertyChanged();
                
                BulkSettingChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BulkOutputDiameter)));
            }
        }
        public double BulkMudPuddle
        {
            get => bulkMudPuddle;
            set
            {
                bulkMudPuddle = value;
                OnPropertyChanged();
                
                BulkSettingChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BulkMudPuddle)));
            }
        }
        
        public double BulkPipeStep
        {
            get => bulkPipeStep;
            set
            {
                bulkPipeStep = value;
                OnPropertyChanged();
                
                BulkSettingChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BulkPipeStep)));
            }
        }

        public double BulkGradientDenominator
        {
            get => gradientDenominator;
            set
            {
                gradientDenominator = value;
                OnPropertyChanged();
                
                BulkSettingChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BulkGradientDenominator)));
            }
        }
    }
    
    /// <summary>
    /// 桝ツール単位：5ｍｍ、10ｍｍなど
    /// </summary>
    public class MasuToolUnit : INotifyPropertyChanged
    {
        private string _symbol;
        
        private int _mmConversionRate;
        
        public string Symbol
        {
            get => _symbol;
            set
            {
                _symbol = value;
                OnPropertyChanged(nameof(Symbol));
            }
        }

        public int MmConversionRate
        {
            get => _mmConversionRate;
            set
            {
                _mmConversionRate = value;
                OnPropertyChanged(nameof(MmConversionRate));
            }
        }

        public MasuToolUnit(string symbol, int mmConversionRate)
        {
            _symbol = symbol;
            _mmConversionRate = mmConversionRate;
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class MasuToolHeight : INotifyPropertyChanged
    {
        private MasuToolUnit _masuToolUnit;
        private double _value;
        private int _roundingType;
        
        public event PropertyChangedEventHandler? PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler? HeightChanged;
        
        public MasuToolUnit MasuToolUnit
        {
            get => _masuToolUnit;
            set
            {
                _masuToolUnit = value;
            }
        }

        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
                
                HeightChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value))) ;
            }
        }

        /// <summary>
        /// 端数処理区分(0:Ceiling, 1:Round, 2:Floor)
        /// </summary>
        public int RoundingType
        {
            get => _roundingType;
            set
            {
                _roundingType = value;
            }
        }

        public double MmValue
        {
            get => Value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="masuToolUnit">単位：１０ｍｍ、５０ｍｍなど</param>
        /// <param name="roundingType">端数処理(0:Ceiling, 1:Round, 2:Floor)</param>
        /// <param name="value"></param>
        public MasuToolHeight(MasuToolUnit masuToolUnit, double value, int roundingType)
        {
            this.MasuToolUnit = masuToolUnit;
            this.RoundingType = roundingType;
            switch (RoundingType)
            {
                case 0:
                    if (value > 0)
                    {
                        this.Value = Math.Ceiling(value / masuToolUnit.MmConversionRate) * masuToolUnit.MmConversionRate;
                    }
                    else
                    {
                        this.Value = - Math.Ceiling(Math.Abs(value) / masuToolUnit.MmConversionRate) * masuToolUnit.MmConversionRate;
                    }
                    
                    break;
                case 1:
                    this.Value = Math.Round((value / masuToolUnit.MmConversionRate), MidpointRounding.AwayFromZero) * masuToolUnit.MmConversionRate;
                    break;
                case 2:
                    if ( value > 0 ) {
                        this.Value = Math.Ceiling(value / masuToolUnit.MmConversionRate) * masuToolUnit.MmConversionRate;
                    }
                    else {
                        this.Value = - Math.Abs( Math.Floor( value / masuToolUnit.MmConversionRate ) *
                                               masuToolUnit.MmConversionRate ) ;
                    }
                    break;
                case 3:
                    if ( value > 0 ) {
                        this.Value = Math.Ceiling(value / masuToolUnit.MmConversionRate) * masuToolUnit.MmConversionRate;
                    }
                    else {
                        this.Value = - Math.Abs( Math.Ceiling( value / masuToolUnit.MmConversionRate ) *
                                                masuToolUnit.MmConversionRate ) ;
                    }
                    break;
            }
        }
    }
}