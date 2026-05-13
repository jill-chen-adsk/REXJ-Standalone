using System.ComponentModel;

namespace CmdDuctDisplacement.UI.Model.InheritBase
{
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// プロパティの変更通知イベントを発生させる
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged(string propertyName)
        {
            var d = PropertyChanged;
            if (d != null)
                d(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 一括で反映したい場合の通知を行う
        /// </summary>
        public void NotifyChangedAll()
        {
            RaisePropertyChanged("AllProperties");
        }
    }
}
