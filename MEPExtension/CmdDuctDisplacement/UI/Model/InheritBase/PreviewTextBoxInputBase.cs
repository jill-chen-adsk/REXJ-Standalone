using System;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace CmdDuctDisplacement.UI.Model.InheritBase
{
    class PreviewTextBoxInputBase : ICommand
    {
        //メンバ変数
        #region Memeber Variables
        protected System.Windows.Controls.TextBox textbox;
        #endregion


        //コンストラクタ
        #region Constructor
        #endregion

        // メンバ関数
        #region Member Functions

#pragma warning disable 0067
        // 本クラスでは使用しない
        //コマンドの実行の可否が変化したときのイベント
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

        //現在の状態でこのコマンドを実行できるかどうかを判断するメソッドを定義します。
        public bool CanExecute(object parameter)
        { return true; }


        public virtual void Execute(object parameter)
        {
            ControlStatus controlstatus = ControlStatus.Instance;

            double condouble = 0;

            //数値として意味を持つかチェックする
            bool yes_parse = false;
            double xx;
            string tmp = textbox.Text.Insert(textbox.SelectionStart, controlstatus.TextCompositionEvent.Text);
            yes_parse = double.TryParse(tmp, out xx);

            //"－"判定
            if ((tmp.Length - tmp.Replace("-", "").Length > 0))
            {
                yes_parse &= false;
            }

            //","判定
            if ((tmp.Length - tmp.Replace(",", "").Length > 0))
            {
                yes_parse &= false;
            }

            //"."判定
            if ((tmp.Length - tmp.Replace(".", "").Length > 1))
            {
                yes_parse &= false;
            }

            //0の後は、"."しか受け付けない
            if ((textbox.Text == "0") &&
                (controlstatus.TextCompositionEvent.Text != "."))
            {
                yes_parse &= false;
            }

            //0.の後は、キャレットが3番目しか受け付けない
            if ((textbox.Text == "0.") &&
                (textbox.SelectionStart != 2))
            {
                yes_parse &= false;
            }

            //テキストの文字列を格納
            char[] cs = textbox.Text.ToCharArray();

            //0入力制御
            if (textbox.Text != string.Empty)
            {
                if ((cs[0] == '0') &&
                    (controlstatus.TextCompositionEvent.Text == "0"))
                {
                    if ((textbox.SelectionStart == 0) ||
                            (textbox.SelectionStart == 1))
                    {
                        yes_parse &= false;
                    }
                }

                //整数部6桁以上入力チェック
                string integer;

                if (0 >= tmp.IndexOf("."))
                {
                    integer = tmp.Substring(0, tmp.Length);
                }

                else
                {
                    integer = tmp.Substring(0, tmp.IndexOf("."));
                }

                if (Regex.IsMatch(integer, @"^[0-9]{7,}$"))
                {
                    yes_parse &= false;
                }

                //小数部桁数入力チェック
                string dec;

                if ((0 <= tmp.IndexOf(".")) &&
                    (((tmp.IndexOf(".") + 1)) < tmp.Length))
                {
                    double i;
                    int j = 2;
                    int indexnum = (tmp.IndexOf(".") + 1);
                    dec = tmp.Substring(indexnum,  tmp.Length - indexnum);

                    for (i = controlstatus.RevitProjectDecimalAccuracy; i < 0.1; i *= 10)
                    {
                        j++;
                    }

                    if (Regex.IsMatch(dec, @"^[0-9]{" + j.ToString() + ",}$"))
                    {
                        yes_parse &= false;
                    }
                }
            }

            if ((textbox.Text.Length > 0) &&
                    (textbox.SelectionStart == 0) &&
                    (cs[0] != '.') &&
                    (controlstatus.TextCompositionEvent.Text == "0"))
            {
                yes_parse &= false;
            }

            controlstatus.TextCompositionEvent.Handled = !yes_parse;

            if (!yes_parse == true)
            {
                return;
            }




            //指定の文字以外は、空白に置き換える
            var str = Regex.Replace(textbox.Text, @"[^0-9|.|-]", "");



            if (double.TryParse(str, out condouble))
            {
                //上記の整数対応と小数対応と同じ処理をしているが、念のため残しておく
                //IntegerPartControl(str, condouble, controlstatus);
                //DecimalPartControl(str, condouble, controlstatus);
            }
        }

        /// <summary>
        /// テキストボックス整数部分の入力制御
        /// </summary>
        /// <param name="str">テキストボックス入力文字列</param>
        /// <param name="textnum">テキストボックス入力文字列を数値のみ抽出したもの</param>
        /// <param name="controlstatus">ControlStatusインスタンス</param>
        public void IntegerPartControl(string str, double textnum, ControlStatus controlstatus)
        {
            //インクリメント
            int i = 0;
            //キャレットの位置情報
            int caret = textbox.SelectionStart;
            //strの正数部分を格納する
            string intstr;

            //テキストボックスの値をdouble型で0と判定したとき
            if (textnum == 0)
            {
                int indexnum = str.IndexOf(".");

                //整数部と小数部に切り分ける
                if (indexnum >= 1)
                {
                    intstr = str.Substring(0, str.IndexOf("."));
                }

                else
                {
                    intstr = str;
                }

            }

            else
            {
                intstr = ((int)textnum).ToString();
            }

            //符号がマイナスの時、マイナスを削除する
            int subbulance = str.IndexOf("-");
            if ((textnum < 0) ||
                (subbulance != -1))
            {
                intstr = intstr.Remove(0, 1);
                //キャレットの位置を扱いやすく整理する
                caret--;
                if (caret < 0)
                {
                    controlstatus.TextCompositionEvent.Handled = true;
                }
            }

            for (i = 0; i < 5; i++)
            {
                if (intstr != "")
                {
                    intstr = intstr.Remove(0, 1);
                }
            }

            if ((intstr != "") && (caret < 7) &&
                ((controlstatus.LastKey == Key.D0) ||
                (controlstatus.LastKey == Key.D1) ||
                (controlstatus.LastKey == Key.D2) ||
                (controlstatus.LastKey == Key.D3) ||
                (controlstatus.LastKey == Key.D4) ||
                (controlstatus.LastKey == Key.D5) ||
                (controlstatus.LastKey == Key.D6) ||
                (controlstatus.LastKey == Key.D7) ||
                (controlstatus.LastKey == Key.D8) ||
                (controlstatus.LastKey == Key.D9) ||
                (controlstatus.LastKey == Key.NumPad0) ||
                (controlstatus.LastKey == Key.NumPad1) ||
                (controlstatus.LastKey == Key.NumPad2) ||
                (controlstatus.LastKey == Key.NumPad3) ||
                (controlstatus.LastKey == Key.NumPad4) ||
                (controlstatus.LastKey == Key.NumPad5) ||
                (controlstatus.LastKey == Key.NumPad6) ||
                (controlstatus.LastKey == Key.NumPad7) ||
                (controlstatus.LastKey == Key.NumPad8) ||
                (controlstatus.LastKey == Key.NumPad9)))
            {
                controlstatus.TextCompositionEvent.Handled = true;
            }
        }
        /// <summary>
        /// テキストボックス整数部分の入力制御
        /// </summary>
        /// <param name="str">テキストボックス入力文字列</param>
        /// <param name="textnum">テキストボックス入力文字列を数値のみ抽出したもの</param>
        /// <param name="controlstatus">ControlStatusインスタンス</param>
        public void DecimalPartControl(string str, double textnum, ControlStatus controlstatus)
        {
            //文字列の小数部
            string decimalstr;
            //文字列の長さ
            int strlen;
            //キャレットの位置情報
            int caret = textbox.SelectionStart;

            int k = 0;
            double j = 0;

            //テキストボックスの値をdouble型で0と判定したとき
            if (textnum == 0)
            {
                int indexnum = str.IndexOf(".");

                //整数部と小数部に切り分ける
                if (indexnum >= 1)
                {
                    decimalstr = str.Substring(0, str.IndexOf("."));
                    decimalstr += ".";
                }

                else
                {
                    decimalstr = "-" + str;
                    decimalstr += ".";
                }

            }

            else
            {
                decimalstr = ((int)textnum).ToString();
                decimalstr += ".";
            }

            int subbulance = str.IndexOf("-");
            if ((textnum < 0) ||
                (subbulance != -1))
            {
                strlen = str.Length - 1;
                //キャレットの位置を扱いやすく整理する
                caret--;
                if (caret < 0)
                {
                    controlstatus.TextCompositionEvent.Handled = true;
                }
            }

            else
            {
                strlen = str.Length;
            }

            //Revitで設定された浮動小数点以下を受けつけない
            if (0 <= str.IndexOf(decimalstr))
            {
                str = str.Replace(decimalstr, "");
                str = str.Replace("-", "");
                for (j = controlstatus.RevitProjectDecimalAccuracy; j < 0.1; j *= 10)
                {
                    if (str != "")
                    {
                        str = str.Remove(0, 1);
                    }
                    //少数何位まで有効か計算する
                    k++;
                }

                //キャレットの位置から文字を受け付けるのか判定
                if ((str != "") && ((strlen - (k + 1)) <= caret) && (caret <= strlen) &&
                    ((controlstatus.LastKey == Key.D0) ||
                     (controlstatus.LastKey == Key.D1) ||
                     (controlstatus.LastKey == Key.D2) ||
                     (controlstatus.LastKey == Key.D3) ||
                     (controlstatus.LastKey == Key.D4) ||
                     (controlstatus.LastKey == Key.D5) ||
                     (controlstatus.LastKey == Key.D6) ||
                     (controlstatus.LastKey == Key.D7) ||
                     (controlstatus.LastKey == Key.D8) ||
                     (controlstatus.LastKey == Key.D9) ||
                     (controlstatus.LastKey == Key.NumPad0) ||
                     (controlstatus.LastKey == Key.NumPad1) ||
                     (controlstatus.LastKey == Key.NumPad2) ||
                     (controlstatus.LastKey == Key.NumPad3) ||
                     (controlstatus.LastKey == Key.NumPad4) ||
                     (controlstatus.LastKey == Key.NumPad5) ||
                     (controlstatus.LastKey == Key.NumPad6) ||
                     (controlstatus.LastKey == Key.NumPad7) ||
                     (controlstatus.LastKey == Key.NumPad8) ||
                     (controlstatus.LastKey == Key.NumPad9)))
                {
                    controlstatus.TextCompositionEvent.Handled = true;
                }
            }

        }
        #endregion
    }
}

