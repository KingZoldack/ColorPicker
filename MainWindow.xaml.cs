using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ColorPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread _thread;
        
        System.Drawing.Color selectedColor;
        
        bool _isPressed = false;
        volatile bool _stopThread = false;

        public MainWindow()
        {
            InitializeComponent();

            this.KeyDown += Window_KeyDown;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return && !_isPressed)
            {
                string hexValue = ColorTranslator.ToHtml(selectedColor);
                tbSelectedColor.Text = tbHexDisplay.Text;
                tbHexDisplay.Text = hexValue;
                _isPressed = true;
                _stopThread = true;
            }
        }

        System.Drawing.Color GetColorAt(System.Drawing.Point location)
        {
            using (Bitmap pixelContainer = new Bitmap(1, 1))
            {
                using (Graphics graphics = Graphics.FromImage(pixelContainer))
                {
                    graphics.CopyFromScreen(location, System.Drawing.Point.Empty, pixelContainer.Size);
                }

                return pixelContainer.GetPixel(0, 0);
            }
        }

        private void BackgroundThread()
        {
            while (!_stopThread)
            {
                System.Drawing.Point cursorPosition = System.Windows.Forms.Control.MousePosition;
                System.Drawing.Color selectedColor = GetColorAt(cursorPosition);

                Dispatcher.Invoke(() =>
                {
                    if (!_isPressed)
                    {
                        gridPanel.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B));
                        string hexValue = ColorTranslator.ToHtml(selectedColor);
                        tbHexDisplay.Text = hexValue;
                    }
                });

                Thread.Sleep(100);
            }
        }

        private void btSelectColorStart_Click(object sender, RoutedEventArgs e)
        {
            TextBlock[] textBlocks = { tbkPointOne, tbkPointTwo, tbkPointThree, tbkPointFour, tbkPointFive, tbkPointNote };

            _isPressed = false;
            _stopThread = false;

            tbHexDisplay.Focus();
            foreach (TextBlock textBlock in textBlocks)
            {
                textBlock.Foreground = new SolidColorBrush(Colors.Transparent);
            }

            _thread = new Thread(BackgroundThread) { IsBackground = true };
            _thread.Start();
        }
    }
}
