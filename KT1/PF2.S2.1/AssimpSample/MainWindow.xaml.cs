using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;


namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    //"3D Models\\Gun"), "Handgun_3ds.3ds", 
                    //"3D Models\\Gun"), "Handgun_obj.obj",
                    "3D Models\\Gun"), "gun.obj",
                    //"3D Models\\Bullet"), "lowpolybullet.3ds",

                    (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
            velicinaMeteTextBox.Text = m_world.skaliranjeMeta.ToString();
            duzinaMetakaTextBox.Text = m_world.duzinaMetaka.ToString();
            m_world.dodeliMainWindow(this);
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F2: this.Close(); break;
                case Key.T:
                    {
                        if (!m_world.animacijaUToku)
                        {
                            if (m_world.RotationX > -5 && m_world.RotationX < 60)
                            {
                            m_world.RotationX -= 5.0f;
                            }
                        }
                        break;
                    }
                case Key.G:
                    {
                        if (!m_world.animacijaUToku)
                        {
                            if (m_world.RotationX < 55 && m_world.RotationX > -10)
                            {
                                m_world.RotationX += 5.0f;
                            }
                        }
                        break;
                    }
                case Key.F:
                    {
                        if (!m_world.animacijaUToku)
                            m_world.RotationY -= 5.0f;
                        break;
                    }
                case Key.H:
                    {
                        if (!m_world.animacijaUToku)
                            m_world.RotationY += 5.0f;
                        break;
                    }
                case Key.Add:
                    {
                        if (!m_world.animacijaUToku)
                        {
                            m_world.SceneDistance -= 700.0f;
                        }
                        break;
                    }
                case Key.Subtract:
                    {
                        if (!m_world.animacijaUToku)
                        {
                            m_world.SceneDistance += 700.0f;
                        }
                        break;
                    }
                case Key.X:
                    {
                        if (!m_world.animacijaUToku)
                        {
                            m_world.animacijaUToku = true;
                            onemoguciInterakciju();
                        }
                        break;
                    }
            }
        }

        private void onemoguciInterakciju() 
        {
            povecajMetuBtn.IsEnabled = false;
            smanjiMetuBtn.IsEnabled = false;
            povecajDuzinuBtn.IsEnabled = false;
            smanjiDuzinuBtn.IsEnabled = false;
            colorPicker.IsEnabled = false;
        }

        public void omoguciInterakciju()
        {
            povecajMetuBtn.IsEnabled = true;
            smanjiMetuBtn.IsEnabled = true;
            povecajDuzinuBtn.IsEnabled = true;
            smanjiDuzinuBtn.IsEnabled = true;
            colorPicker.IsEnabled = true;
        }

        private void povecajMetuBtn_Click(object sender, RoutedEventArgs e)
        {
            if (m_world.skaliranjeMeta < 80)
            {
                m_world.skaliranjeMeta += 10;
                velicinaMeteTextBox.Text = m_world.skaliranjeMeta.ToString();
            }
        }

        private void smanjiMetuBtn_Click(object sender, RoutedEventArgs e)
        {
            if (-20 < m_world.skaliranjeMeta)
            {
                m_world.skaliranjeMeta -= 10;
                velicinaMeteTextBox.Text = m_world.skaliranjeMeta.ToString();
            }
        }

        private void povecajDuzinuBtn_Click(object sender, RoutedEventArgs e)
        {
            if (m_world.duzinaMetaka < 60)
            {
                m_world.duzinaMetaka += 10;
                duzinaMetakaTextBox.Text = m_world.duzinaMetaka.ToString();
            }
        }

        private void smanjiDuzinuBtn_Click(object sender, RoutedEventArgs e)
        {
            if (m_world.duzinaMetaka > 10)
            {
                m_world.duzinaMetaka -= 10;
                duzinaMetakaTextBox.Text = m_world.duzinaMetaka.ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Color color = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                m_world.ambLightR = (color.R / 255.0f);
                m_world.ambLightG = (color.G / 255.0f);
                m_world.ambLightB = (color.B / 255.0f);

            }
        }
    }
}
