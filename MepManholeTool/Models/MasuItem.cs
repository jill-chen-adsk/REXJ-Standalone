using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;

namespace MepManholeTool.Models
{
    public class MasuItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        public event PropertyChangedEventHandler? HeightChanged;
        public event PropertyChangedEventHandler? SymbolChanged;

        #region プロパティ

        private string _familyName;
        private string _masuSymbol;
        private string _biko;
        private double? _size;
        private double? _outputDiameter;
        private double? _mudPuddle;
        private string _groundLevel;
        private double _groundLevelHeight;
        private double? _pipeStep;
        private double _heightToTopo;
        private double _orgHeightTopo;
        private Element _masuElement;

        public string FamilyName
        {
            get => _familyName;
            set => _familyName = value;
        }

        /// <summary>
        /// 備考
        /// </summary>
        public string Biko
        {
            get => _biko;
            set
            {
                _biko = value;
                OnPropertyChanged(nameof(Biko));
            }
        }
        
        /// <summary>
        /// 記号
        /// </summary>
        public string MasuSymbol
        {
            get => _masuSymbol;
            set
            {
                _masuSymbol = value;
                OnPropertyChanged();

                SymbolChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PipeStep)));
            }
        }

        /// <summary>
        /// 桝サイズ
        /// </summary>
        public double? Size
        {
            get => _size;
            set
            {
                _size = value;
                OnPropertyChanged(nameof(Size));
            }
        }

        /// <summary>
        /// 出口径
        /// </summary>
        public double? OutputDiameter
        {
            get => _outputDiameter;
            set
            {
                _outputDiameter = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 泥だまり
        /// </summary>
        public double? MudPuddle
        {
            get => _mudPuddle;
            set
            {
                _mudPuddle = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 基準レベル
        /// </summary>
        public string GroundLevel
        {
            get => _groundLevel;
            set
            {
                _groundLevel = value;
                OnPropertyChanged(nameof(GroundLevel));
            }
        }
        
        /// <summary>
        /// 当該桝地盤レベル
        /// </summary>
        public double HeightFromGroundLevel
        {
            get => _groundLevelHeight;
            set
            {
                _groundLevelHeight = value;
                OnPropertyChanged(nameof(HeightFromGroundLevel));
            }
        }

        /// <summary>
        /// 配管段差
        /// </summary>
        public double? PipeStep
        {
            get => _pipeStep;
            set
            {
                _pipeStep = value;
                OnPropertyChanged(nameof(PipeStep));
                
                HeightChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PipeStep)));
            }
        }

        /// <summary>
        /// TopoSolid/TopoSurfaceの高さ
        /// </summary>
        public double HeightToTopo
        {
            get => _heightToTopo;
            set
            {
                _heightToTopo = value;
                OnPropertyChanged(nameof(HeightToTopo));
            }
        }

        public double OrgHeightToTopo
        {
            get => _orgHeightTopo;
            set
            {
                _orgHeightTopo = value;
            }
        }
        
        public Element MasuElement
        {
            get => _masuElement;
            set
            {
                _masuElement = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 流出管底
        /// </summary>
        public double BaseBottomHeight { get; set; }

        /// <summary>
        /// 基準レベルからの高さ
        /// </summary>
        public double HeightFromBase { get; set; }
        public double Depth { get; set; }
        public IList<Connector>? Connectors { get; set; }
        public IList<Connector>? InConnectors { get; set; }
        public IList<Connector>? OutConnectors { get; set; }
        #endregion

        #region 初期処理

        public MasuItem(Element masuElement, string familyName, string biko, string masuSymbol, double heightFromBaseLevel, double? size, double? output, 
                        double? mudPuddle, double? pipeStep, double baseBottomHeight, double depth, string groundLevel, 
                        double heightFromGroundLevel, IList<Connector>? connectors, IList<Connector>? inConnectors,
                        IList<Connector>? outConnectors)
        {
            this.FamilyName = familyName;
            this.MasuElement = masuElement;
            this.Biko = biko;
            this.MasuSymbol = masuSymbol;
            this.HeightFromBase = heightFromBaseLevel;
            this.Size = size;
            this.OutputDiameter = output;
            this.MudPuddle = mudPuddle;
            this.PipeStep = pipeStep;
            this.BaseBottomHeight = baseBottomHeight;
            this.Depth = depth;
            this.GroundLevel = groundLevel;
            this.HeightFromGroundLevel = heightFromGroundLevel;
            this.OrgHeightToTopo = heightFromGroundLevel;
            this.HeightToTopo = heightFromGroundLevel;
            this.Connectors = connectors;
            this.InConnectors = inConnectors;
            this.OutConnectors = outConnectors;
        }
        
        #endregion
        

    }
}