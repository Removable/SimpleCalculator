using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SimpleCalculator
{
    public class MyButton : Button
    {
        public ButtonType ButtonType { set; get; }
        public string Sign { set; get; }
    }
}
