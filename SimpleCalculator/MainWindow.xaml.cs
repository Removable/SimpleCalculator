using System;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Application = System.Windows.Application;
using Size = System.Drawing.Size;

namespace SimpleCalculator
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _historyStr = string.Empty;
        private StringBuilder _firstValue = new StringBuilder("0");
        private StringBuilder _result = new StringBuilder();
        private StringBuilder _secondValue = new StringBuilder();
        private ButtonType _sign = ButtonType.Null;
        private int _step;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     窗体载入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //为所有MyButton添加点击事件
            foreach (var control in MainGrid.Children)
            {
                var button = control as MyButton;
                if (button != null)
                    button.Click += ButtonClick;
            }
        }

        /// <summary>
        ///     按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as MyButton;
            if (button != null)
                switch (button.ButtonType)
                {
                    case ButtonType.Number: //数字键
                        NumberAndPointPress(button);
                        break;
                    case ButtonType.Point: //小数点
                        NumberAndPointPress(button);
                        break;
                    case ButtonType.Plus: //加号
                        OperatorPress(button);
                        break;
                    case ButtonType.Minus: //减号
                        OperatorPress(button);
                        break;
                    case ButtonType.Multiplication: //乘号
                        OperatorPress(button);
                        break;
                    case ButtonType.Division: //除号
                        OperatorPress(button);
                        break;
                    case ButtonType.Equal: //等号
                        EqualPress();
                        break;
                    case ButtonType.Percent: //百分号
                        DealWithSpecialCalc(button);
                        break;
                    case ButtonType.Root: //根号
                        DealWithSpecialCalc(button);
                        break;
                    case ButtonType.Square: //平方
                        DealWithSpecialCalc(button);
                        break;
                    case ButtonType.Semicolon: //x分之一
                        DealWithSpecialCalc(button);
                        break;
                    case ButtonType.Tive: //正负号
                        PositiveAndNegative();
                        break;
                    case ButtonType.ClearAll: //CE键
                        ClearAll();
                        break;
                    case ButtonType.ClearInput: //C键
                        ClearInput();
                        break;
                    case ButtonType.Backspace: //退格键
                        Backspace();
                        break;
                }

            #region 显示到“显示屏”上

            if (_step > 1)
            {
                var showText = new StringBuilder();
                if (_step <= 3)
                {
                    if (_step == 2)
                        RemoveLatestPoint(ref _firstValue);
                    showText.AppendFormat("{0} {1} {2}", _historyStr, _firstValue.ToString2(), DealWithOperator(_sign));
                }
                else if (_step == 4)
                {
                    showText.AppendFormat("{0} {1} {2} {3}", _historyStr,
                        RemoveLatestPoint(ref _firstValue).ToString2(), DealWithOperator(_sign),
                        RemoveLatestPoint(ref _secondValue).ToString2().Equals("0") ? "" : _secondValue.ToString2());
                }
                else if (_step == 5)
                {
                    showText.AppendFormat("{0} {1} {2} {3} {4}", _historyStr,
                        RemoveLatestPoint(ref _firstValue).ToString2(), DealWithOperator(_sign),
                        RemoveLatestPoint(ref _secondValue).ToString2(), "=");
                    TbResult.Text = RemoveLatestPoint(ref _result).ToString2();
                }
                var arr = showText.ToString().Split(' ');
                showText = new StringBuilder();
                foreach (var s in arr)
                    if (!string.IsNullOrEmpty(s))
                        showText.AppendFormat("{0} ", s);
                TbFormula.Text = showText.Remove(showText.Length - 1, 1).ToString();
            }
            if (_step < 3)
                TbResult.Text = _firstValue.ToString2();
            if (_step == 3)
                TbResult.Text = _secondValue.ToString2();

            #endregion
        }

        #region 运算相关

        /// <summary>
        ///     根号等运算处理
        /// </summary>
        private void DealWithSpecialCalc(MyButton button)
        {
            if (button.ButtonType != ButtonType.Percent)
            {
                if (_step <= 1)
                {
                    SpecialCalc(button.ButtonType, ref _firstValue);
                }
                else if (_step == 2)
                {
                    _secondValue = new StringBuilder(_firstValue.ToString2());
                    SpecialCalc(button.ButtonType, ref _secondValue);
                    _step = 3;
                }
                else
                {
                    SpecialCalc(button.ButtonType, ref _secondValue);
                }
            }
            else
                SpecialCalc(button.ButtonType, ref _firstValue);
        }

        /// <summary>
        ///     进行根号等运算
        /// </summary>
        /// <param name="buttonType">要进行的运算类型</param>
        /// <param name="num"></param>
        private void SpecialCalc(ButtonType buttonType, ref StringBuilder num)
        {
            switch (buttonType)
            {
                case ButtonType.Root: //根号
                    num = new StringBuilder(Math.Sqrt(num.ToDouble()).ToString2());
                    break;
                case ButtonType.Semicolon: //x分之一
                    num = new StringBuilder((1 / num.ToDouble()).ToString2());
                    break;
                case ButtonType.Square: //平方
                    num = new StringBuilder(Math.Pow(num.ToDouble(), 2).ToString2());
                    break;
                case ButtonType.Percent: //百分号
                    if (_step == 1)
                    {
                        _firstValue = new StringBuilder("0");
                        _step = 1;
                    }
                    else if (_step == 2)
                    {
                        _secondValue = new StringBuilder((_firstValue.ToDouble() * _firstValue.ToDouble() * 0.01)
                            .ToString2());
                        _step = 4;
                    }
                    else if (_step == 3 || _step == 4)
                    {
                        _secondValue = new StringBuilder((_secondValue.ToDouble() * _secondValue.ToDouble() * 0.01)
                            .ToString2());
                        _step = 4;
                    }
                    break;
            }
        }

        /// <summary>
        ///     正负号处理
        /// </summary>
        private void PositiveAndNegative()
        {
            if (_step == 1)
            {
                AddOrRemoveMinus(ref _firstValue);
            }
            else if (_step == 2)
            {
                _secondValue = new StringBuilder(_firstValue.ToString());
                AddOrRemoveMinus(ref _secondValue);
                _step = 3;
            }
        }

        /// <summary>
        ///     增加或删除负号
        /// </summary>
        /// <param name="num"></param>
        private void AddOrRemoveMinus(ref StringBuilder num)
        {
            if (num.ToString().Contains("-"))
                num.Remove(0, 1);
            else
                num.Insert(0, '-');
        }

        /// <summary>
        ///     等号计算
        /// </summary>
        private void EqualPress()
        {
            switch (_step)
            {
                case 0:
                case 1:
                    _step = 5;
                    _result = _firstValue;
                    break;
                case 2:
                    _step = 5;
                    _secondValue = _firstValue;
                    ArithmeticAndShow(_sign);
                    break;
                case 3:
                case 4:
                    _step = 5;
                    ArithmeticAndShow(_sign);
                    break;
            }
        }


        /// <summary>
        ///     计算结果
        /// </summary>
        /// <param name="operatorKey">四则运算符</param>
        /// <returns></returns>
        private double Arithmetic(ButtonType operatorKey)
        {
            var result = 0.0d;
            var first = _firstValue.ToDouble();
            var second = _secondValue.ToDouble();
            switch (operatorKey)
            {
                case ButtonType.Plus:
                    result = first + second;
                    break;
                case ButtonType.Minus:
                    result = first - second;
                    break;
                case ButtonType.Multiplication:
                    result = first * second;
                    break;
                case ButtonType.Division:
                    result = first / second;
                    break;
            }
            return result;
        }

        /// <summary>
        ///     计算结果并显示
        /// </summary>
        /// <param name="operatorKey">四则运算符</param>
        private void ArithmeticAndShow(ButtonType operatorKey)
        {
            var result = Arithmetic(operatorKey);
            if (double.IsInfinity(result) || double.IsNaN(result))
                _result = new StringBuilder("除数不能为零");
            else
                _result = new StringBuilder(result.ToString2());
        }

        #endregion

        #region 显示相关处理

        //暂未想好如何处理
        //private string OptimizedDisplay(string str, TextBlock textBlock, Font font)
        //{
        //    if (GetWidthOfString(str, font),Width > textBlock.ActualWidth)
        //}

        /// <summary>
        /// 获取字符串的显示尺寸
        /// </summary>
        /// <param name="str"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        private Size GetWidthOfString(string str, Font font)
        {
            var result = TextRenderer.MeasureText(str, font);
            return result;
        }

        /// <summary>
        ///     将数字字符最后的小数点去掉
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <returns></returns>
        private StringBuilder RemoveLatestPoint(ref StringBuilder stringBuilder)
        {
            var tempBuilder = new StringBuilder(stringBuilder.ToString());
            if (stringBuilder.Length > 0 && stringBuilder.ToString2().IndexOf('.') == stringBuilder.Length - 1)
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder;
        }

        /// <summary>
        ///     四则运算符点击处理
        /// </summary>
        /// <param name="button">按下的按钮</param>
        private void OperatorPress(MyButton button)
        {
            if (_step < 2)
            {
                _sign = button.ButtonType;
                _step = 2;
            }
            else if (_step == 2)
            {
                _sign = button.ButtonType;
            }
            else if (_step == 3)
            {
                //_historyStr = $"{_firstValue} {DealWithOperator(_sign)} {_secondValue}";
                _firstValue = new StringBuilder(Arithmetic(_sign).ToString2());
                _secondValue = new StringBuilder("0");
                _sign = button.ButtonType;
                _step = 2;
            }
            else if (_step == 5)
            {
                var temp = _result;
                ClearAll();
                _firstValue = temp;
                _sign = button.ButtonType;
                _step = 2;
            }
        }

        /// <summary>
        ///     数字与小数点点击处理
        /// </summary>
        /// <param name="button">按下的按钮</param>
        private void NumberAndPointPress(MyButton button)
        {
            if (_step == 5)
                ClearAll();
            if (_step < 2)
            {
                if (_firstValue.Length >= TbResult.ActualWidth)
                    return;
                DealWithStringForDisplay(ref _firstValue, button);
                _step = 1;
            }
            else
            {
                if (_secondValue.Length >= TbResult.ActualWidth)
                    return;
                DealWithStringForDisplay(ref _secondValue, button);
                _step = 3;
            }
        }

        /// <summary>
        ///     处理数字字符串以便显示
        /// </summary>
        /// <param name="value">要处理的值</param>
        /// <param name="button">按下的按钮</param>
        /// <returns></returns>
        private void DealWithStringForDisplay(ref StringBuilder value, MyButton button)
        {
            var str = button.Content.ToString2();
            if (button.ButtonType == ButtonType.Point)
            {
                if (!value.ToString2().Contains("."))
                    value.Append(str);
            }
            else if (button.ButtonType == ButtonType.Number)
            {
                if (!value.ToString2().Equals("0"))
                    value.Append(str);
                else
                    value = new StringBuilder(str);
            }
        }

        /// <summary>
        ///     四则运算符显示处理
        /// </summary>
        /// <param name="sign">要处理的符号</param>
        /// <returns></returns>
        private string DealWithOperator(ButtonType sign)
        {
            var result = string.Empty;
            switch (sign)
            {
                case ButtonType.Plus:
                    result = "+";
                    break;
                case ButtonType.Minus:
                    result = "-";
                    break;
                case ButtonType.Multiplication:
                    result = "×";
                    break;
                case ButtonType.Division:
                    result = "÷";
                    break;
                default:
                    result = string.Empty;
                    break;
            }
            return result;
        }

        #endregion

        #region 清除与退格

        /// <summary>
        ///     退格
        /// </summary>
        private void Backspace()
        {
            if (_step == 1)
                BackspaceForNumber(ref _firstValue);
            else if (_step == 3)
                BackspaceForNumber(ref _secondValue);
        }

        /// <summary>
        ///     退格操作
        /// </summary>
        /// <param name="num">要操作的数据</param>
        private void BackspaceForNumber(ref StringBuilder num)
        {
            var flag = 1;
            if (num.ToDouble() < 0)
                flag++;
            if (num.Length > flag)
                num.Remove(num.Length - 1, 1);
            else
                num = new StringBuilder("0");
        }

        /// <summary>
        ///     清空所有/重置
        /// </summary>
        private void ClearAll()
        {
            _firstValue = new StringBuilder("0");
            _sign = ButtonType.Null;
            _secondValue = new StringBuilder("");
            TbFormula.Text = string.Empty;
            TbResult.Text = "0";
            _step = 0;
            _historyStr = string.Empty;
        }

        /// <summary>
        ///     清空当前输入
        /// </summary>
        private void ClearInput()
        {
            switch (_step)
            {
                case 1:
                    _firstValue = new StringBuilder("0");
                    _step--;
                    break;
                case 2:
                    _step = 3;
                    break;
                case 3:
                    _secondValue = new StringBuilder("0");
                    break;
            }
        }

        #endregion

        #region 杂项

        //拖动窗体
        private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {
            }
        }

        private void ImgClose_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ImgMinimum_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion
    }
}