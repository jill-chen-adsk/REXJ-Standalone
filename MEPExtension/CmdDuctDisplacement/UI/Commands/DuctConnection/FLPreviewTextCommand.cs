using CmdDuctDisplacement.UI.Model;
using CmdDuctDisplacement.UI.Model.InheritBase;
using CmdDuctDisplacement.UI.View;
using CmdDuctDisplacement.UI.ViewModel;
using RevitMEPAddin.Common;
using System.Text.RegularExpressions;

namespace CmdDuctDisplacement.UI.Commands.DuctConnection
{
    /// <summary>
    /// FMテキストボックスPreviewTextInputイベント関連クラス
    /// </summary>
    class FLPreviewTextCommand : PreviewTextBoxInputBase
    {
        //メンバ変数
        #region Memeber Variables
        private Logger log;
        private ConnectionSettingViewModel ductp;
        private ConnectionSettingView uiwindow;
        #endregion


        //コンストラクタ
        #region Constructor
        public FLPreviewTextCommand(ConnectionSettingViewModel ductPropaty, Logger _log, ConnectionSettingView _uiwindow)
        {
            ductp = ductPropaty;
            uiwindow = _uiwindow;
            log = _log;
            textbox = uiwindow.FLTextBox;
        }
        #endregion

        // メンバ関数
        #region Member Functions
        public override void Execute(object parameter)
        {
            ControlStatus controlstatus = ControlStatus.Instance;
            double condouble = 0;

            //数値として意味を持つかチェックする
            bool yes_parse = false;
            double xx;
            string tmp = textbox.Text.Insert(textbox.SelectionStart, controlstatus.TextCompositionEvent.Text);
            yes_parse = double.TryParse(tmp, out xx);

            //"－"判定
            if (controlstatus.TextCompositionEvent.Text == "-")
            {
                yes_parse |= true;
                if (textbox.SelectionStart != 0)
                {
                    yes_parse &= false;
                }
            }

            //"－"判定
            if ((tmp.Length - tmp.Replace("-", "").Length > 1))
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
            //ただし、先頭に-のみ受け付ける
            if (textbox.Text == "0")
            {
                if (controlstatus.TextCompositionEvent.Text == "-")
                {
                    if (textbox.SelectionStart != 0)
                    {
                        yes_parse &= false;
                    }
                }

                else if (controlstatus.TextCompositionEvent.Text != ".")
                {
                    yes_parse &= false;
                }
            }

            //-0の後は、"."しか受け付けない
            if ((textbox.Text == "-0") &&
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

            //-0.の後は、キャレットが4番目しか受け付けない
            if ((textbox.Text == "-0.") &&
                (textbox.SelectionStart != 3))
            {
                yes_parse &= false;
            }


            //テキストの文字列を格納
            char[] cs = textbox.Text.ToCharArray();

            //0入力制御
            if (textbox.Text != string.Empty)
            {
                if ((cs[0] == '-') &&
                    (textbox.Text.Length > 1))
                {
                    if ((cs[1] == '0') &&
                        (controlstatus.TextCompositionEvent.Text == "0"))
                    {
                        if ((textbox.SelectionStart == 1) ||
                                (textbox.SelectionStart == 2))
                        {
                            yes_parse &= false;
                        }
                    }
                }

                else
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
                }

                if ((textbox.Text.Length > 0) &&
                    (textbox.SelectionStart == 0) &&
                    (cs[0] != '.') &&
                    (controlstatus.TextCompositionEvent.Text == "0"))
                {
                    yes_parse &= false;
                }

                if ((textbox.Text.Length > 2) &&
                    (textbox.SelectionStart == 1) &&
                    (cs[0] == '-') &&
                    (cs[1] != '.') &&
                    (controlstatus.TextCompositionEvent.Text == "0"))
                {
                    yes_parse &= false;
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

                if (cs[0] == '-')
                {
                    integer = integer.Remove(0,1);
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
                    dec = tmp.Substring(indexnum, tmp.Length - indexnum);

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
        #endregion
    }
}

