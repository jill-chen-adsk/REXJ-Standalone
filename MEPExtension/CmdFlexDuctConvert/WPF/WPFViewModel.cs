using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//INotifyPropertyChanged
using System.ComponentModel;
//ICommand
using System.Windows.Input;

namespace WPFView.WPF
{
    public class WPFViewModel:ViewModelBase
    {
        //テキストボックスの内部情報格納用変数
        private string _txt_001;

        //テキストボックスのプロパティ設定
        public string _txt_WPFViewModel001
        {
            get
            {
                return _txt_001;
            }
            set
            {
                _txt_001 = value;
                OnPropertyChanged("_txt_WPFViewModel001");
            }
        }

        //コマンドの設定
        public ICommand _btn_WPFViewModelOK { get; set; }
        public ICommand _btn_WPFViewModelCancel { get; set; }

        public Action CloseWindow { get; set; }

        Action<string> func_ok = (sttxt) =>
        {
            WPFModel._Is_FlexDuctChange = true;

            double d;
            WPFModel._Is_FlexDuctChange = double.TryParse(sttxt , out d);
            WPFModel._d_FlexDuctLength = d;
        };

        //コンストラクタ
        public WPFViewModel()
        {
            //_txt_001 = "";

            //ボタンの設定
            _btn_WPFViewModelOK = new WPFViewModelButton(this, () => func_ok(_txt_001),() => CloseWindow());
            _btn_WPFViewModelCancel = new WPFViewModelButton(this, () => WPFModel._Is_FlexDuctChange = false,()=> CloseWindow());
        }

    }

    //抽象クラスの設定
    //INotifyPropertyChangedインタフェースを継承
    abstract public class ViewModelBase : INotifyPropertyChanged
    {
        //イベント（デリゲート）の宣言
        public event PropertyChangedEventHandler PropertyChanged;
        //バインディングした要素のプロパティが変化したことを取得
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }


    //ICommandインターフェースの継承
    class WPFViewModelButton : ICommand
    {
        //通常はActionデリゲートでつなげるのかな
        Action execute0;
        Action execute1;
        //MainのViewモデルのパラメータを直接編集する場合は以下が良いかも
        WPFViewModel vmc;

        //コンストラクタ
        public WPFViewModelButton(WPFViewModel _vmc, Action _execute0, Action _execute1)
        {
            this.vmc = _vmc;
            this.execute0 = _execute0;
            this.execute1 = _execute1;
        }

        //ボタンの実行可能状態の変化を検知し挙動する（使ってない）
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        //ボタンが実行可能であるか否かを制御するメソッド
        public bool CanExecute(object parameter)
        {
            return true;
        }
        //ボタン実行メソッド
        public void Execute(object parameter)
        {


            execute0();
            execute1();
        }

    }


}
