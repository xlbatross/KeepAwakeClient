using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCVForm
{
    internal class Notify : INotifyPropertyChanged
    {
        public string text = "text2";

        public string Text
        {
            get { return text; }
            set 
            { 
                if (text == value)
                {
                    return;
                }
                text = value;
                var propArg = new PropertyChangedEventArgs(nameof(Text));
                PropertyChanged?.Invoke(this, propArg);
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
