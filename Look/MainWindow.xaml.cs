using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Look
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string RegistryDirectory = @"*\shell\Look!";
            using (RegistryKey ClassesRootKey = Registry.ClassesRoot.OpenSubKey(RegistryDirectory))
            {
                if (ClassesRootKey != null)
                {
                    //Delete context menu
                    MessageBoxResult result = MessageBox.Show("Delete context menu?", "Look!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            Registry.ClassesRoot.DeleteSubKeyTree(RegistryDirectory);
                            MessageBox.Show("Сontext menu was removed", "Look!", MessageBoxButton.OK, MessageBoxImage.Information);
                            Closer();
                            break;
                        case MessageBoxResult.No:
                            Closer();
                            break;
                    }
                    //Delete context menu
                }
                else
                {
                    //Create context menu
                    MessageBoxResult result = MessageBox.Show("Create context menu?", "Look!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            string Directory = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.SetupInformation.ApplicationName;
                            Registry.ClassesRoot.CreateSubKey(@"*\shell\Look!\command").SetValue("", $@"""{Directory}"" ""%1""");
                            MessageBox.Show("Сontext menu was created", "Look!", MessageBoxButton.OK, MessageBoxImage.Information);
                            Closer();
                            break;
                        case MessageBoxResult.No:
                            Closer();
                            break;
                    }
                    //Create context menu
                }
            }
        }

        void Closer()
        {
            Close();
        }

        public MainWindow(string fileName)
        {
            InitializeComponent();
            if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
            {
                try
                {
                    var image_info = new BitmapImage(new Uri(fileName));
                    Image_box.Source = image_info;
                    Size_Control(image_info);

                    MainWind.MinHeight = Image_box.Height + 200;
                    MainWind.MinWidth = Image_box.Width + 50;

                    Image_box.RenderTransformOrigin = new Point(0.5, 0.5);
                    Canvas_Draw_Panel.RenderTransformOrigin = new Point(0.5, 0.5);

                    Border_Height = Tool_border.Height;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Closer();
                }
            }
        }

        private void Size_Control(BitmapImage image_info)
        {
            Image_box.Width = image_info.Width;
            Image_box.Height = image_info.Height;
            Image_box.MaxWidth = SystemParameters.PrimaryScreenWidth - 200;
            Image_box.MaxHeight = SystemParameters.PrimaryScreenHeight - 200;
            if (image_info.Height < 800 && image_info.Width < 800)
            {
                MainWind.Height = 900;
                MainWind.Width = 900;
            }
            else
            {
                MainWind.Height = image_info.Height + 100;
                MainWind.Width = image_info.Width + 100;
            }
        }

        private void Control_Panel_Animation_func(Border control_figure, double to, double duration)
        {
            var Animation = new DoubleAnimation
            {
                From = control_figure.Height,
                To = to,
                Duration = TimeSpan.FromMilliseconds(duration)
            };
            control_figure.BeginAnimation(HeightProperty, Animation);
        }

        private void Control_Panel_Button_Checked(object sender, RoutedEventArgs e)
        {
            Control_Panel_Animation_func(Tool_border, 0, 200);
        }

        private double Border_Height;

        private void Control_Panel_Button_Unchecked(object sender, RoutedEventArgs e)
        {
            Control_Panel_Animation_func(Tool_border, Border_Height, 500);
        }

        private void Rotation_Animation_func(Control control)
        {
            control.IsEnabled = false;
            double Rotate_Value;
            if (control.Name.ToString().Contains("Left"))
            {
                Rotate_Value = (Image_Rotate_transf.Angle - 90);
            }
            else
            {
                Rotate_Value = (Image_Rotate_transf.Angle + 90);
            }

            var Anim_Rotate_Canvas = new DoubleAnimation
            {
                From = Canvas_Rotate_Transf.Angle,
                To = Rotate_Value,
                Duration = TimeSpan.FromMilliseconds(200)
            };
            var Anim_Rotate_Image = new DoubleAnimation
            {
                From = Image_Rotate_transf.Angle,
                To = Rotate_Value,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            Anim_Rotate_Image.Completed += (s, e) =>
            {
                control.IsEnabled = true;
            };
            Image_Rotate_transf.BeginAnimation(RotateTransform.AngleProperty, Anim_Rotate_Image);
            Canvas_Rotate_Transf.BeginAnimation(RotateTransform.AngleProperty, Anim_Rotate_Canvas);
        }

        private void Rotation_Left_90_Click(object sender, RoutedEventArgs e)
        {
            Rotation_Animation_func(Rotation_Left_90);
        }

        private void Rotation_Right_90_Click(object sender, RoutedEventArgs e)
        {
            Rotation_Animation_func(Rotation_Right_90);
        }

        private void Zoom_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var Amin_Zoom_X_Canvas = new DoubleAnimation
            {
                From = Canvas_Scale_Transf.ScaleX,
                To = e.NewValue,
                Duration = TimeSpan.FromMilliseconds(400)
            };
            var Amin_Zoom_Y_Canvas = new DoubleAnimation
            {
                From = Canvas_Scale_Transf.ScaleY,
                To = e.NewValue,
                Duration = TimeSpan.FromMilliseconds(400)
            };

            var Amin_Zoom_X = new DoubleAnimation
            {
                From = Image_Scale_transf.ScaleX,
                To = e.NewValue,
                Duration = TimeSpan.FromMilliseconds(400)
            };
            var Amin_Zoom_Y = new DoubleAnimation
            {
                From = Image_Scale_transf.ScaleY,
                To = e.NewValue,
                Duration = TimeSpan.FromMilliseconds(400)
            };
            Image_Scale_transf.BeginAnimation(ScaleTransform.ScaleXProperty, Amin_Zoom_X);
            Image_Scale_transf.BeginAnimation(ScaleTransform.ScaleYProperty, Amin_Zoom_Y);
            Canvas_Scale_Transf.BeginAnimation(ScaleTransform.ScaleXProperty, Amin_Zoom_X_Canvas);
            Canvas_Scale_Transf.BeginAnimation(ScaleTransform.ScaleYProperty, Amin_Zoom_Y_Canvas);
        }

        private void Full_Screen_Checked(object sender, RoutedEventArgs e)
        {
            MainWind.ResizeMode = ResizeMode.NoResize;
            MainWind.WindowStyle = WindowStyle.None;
            if (MainWind.WindowState != WindowState.Maximized)
            {
                MainWind.WindowState = WindowState.Maximized;
            }
            else
            {
                MainWind.WindowState = WindowState.Normal;
                MainWind.WindowState = WindowState.Maximized;
            }
        }

        private void Full_Screen_Unchecked(object sender, RoutedEventArgs e)
        {
            MainWind.ResizeMode = ResizeMode.CanResize;
            MainWind.WindowState = WindowState.Normal;
            MainWind.WindowStyle = WindowStyle.SingleBorderWindow;
        }

        private void MainWind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && Full_Screen.IsChecked == true)
            {
                Full_Screen.IsChecked = false;
            }
        }


        private void MainWind_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                double Scroll_Change = Zoom_Slider.LargeChange / 4;
                if (e.Delta >0) //Zoom +
                {
                    Zoom_Slider.Value += Scroll_Change;
                }
                else //Zoom -
                {
                    Zoom_Slider.Value -= Scroll_Change;
                }
            }
        }

        private void Draw_Button_Checked(object sender, RoutedEventArgs e)
        {
            Canvas_Cleaner.IsChecked = false;
            Canvas_Draw_Panel.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void Draw_Button_Unchecked(object sender, RoutedEventArgs e)
        {
            Canvas_Draw_Panel.EditingMode = InkCanvasEditingMode.None;
        }

        private void Canvas_Cleaner_Unchecked(object sender, RoutedEventArgs e)
        {
            Canvas_Draw_Panel.EditingMode = InkCanvasEditingMode.None;
        }

        private void Canvas_Cleaner_Checked(object sender, RoutedEventArgs e)
        {
            Draw_Button.IsChecked = false;
            Canvas_Draw_Panel.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }
    }
}
