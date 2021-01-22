using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using System.Drawing.Imaging;
using System.Drawing;
using Tao.OpenGl;
using System.Windows.Threading;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Ugao rotacije Meseca
        /// </summary>
        private float m_moonRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije Zemlje
        /// </summary>
        private float m_earthRotation = 0.0f;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        private AssimpScene m_scene_bullet;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;      

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 4000.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        private OpenGL gl;

        Glu.GLUquadric gluObject = Glu.gluNewQuadric();

        //Parametri za teksture
        private enum TextureObjects { DRVO = 0, METAL, BETON};
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;
        private uint[] m_textures = null;
        public string[] m_textureFiles = { "..//..//Texture//drvo2.jpg", "..//..//Texture//metal4.jpg", "..//..//Texture//concrete3.jpg" };


        //Za promenu ambijentalnog svetla
        public float ambLightR = 0.7f;
        public float ambLightG = 0.7f;
        public float ambLightB = 0.7f;

        public float skaliranjeMeta = 0.0f;
        public float duzinaMetaka = 30.0f;

        public DispatcherTimer timer1;

        //Animacija
        public bool animacijaUToku = false;
        public bool restartujAnimaciju = false;

        public bool pistoljPostavljen = false;
        public bool pistoljSpreman = false;
        public bool metakSeKrece = false;
        public bool metaPogodjena = false;
        public bool metaOborena = false;
        public bool metaPogodjena1 = false;
        public bool metaOborena1 = false;
        public bool metaPogodjena2 = false;
        public bool metaOborena2 = false;
        public bool pistoljTrzaj = false;
        public bool pistoljTrzaj1 = false;
        public bool pistoljTrzaj2 = false;

        public float gunY = -300f;
        public float gunRotate = 0;
        public float bulletX = -1200f;
        public float bulletY = -195f;
        public float targetX = 1300f;
        public float targetY = 150f;
        public float targetX1 = 1300f;
        public float targetY1 = 150f;
        public float targetX2 = 1300f;
        public float targetY2 = 150f;

        public MainWindow mainWindow;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName,int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_scene_bullet = new AssimpScene(scenePath, "lowpolybullet.3ds", gl);
            this.m_width = width;
            this.m_height = height;

            this.gl = gl;

            m_textures = new uint[m_textureCount];
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
//            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
//            gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);

            gl.Enable(OpenGL.GL_DEPTH_TEST);        
            gl.Enable(OpenGL.GL_CULL_FACE);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            LoadTextures(gl);

//            gl.ClearColor(0.545f, 0.271f, 0.075f, 1.0f);

            timer1 = new DispatcherTimer();             
            timer1.Interval = TimeSpan.FromMilliseconds(60);
            timer1.Tick += new EventHandler(animation);

            m_scene.LoadScene();
            m_scene.Initialize();

            m_scene_bullet.LoadScene();
            m_scene_bullet.Initialize();
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            setUpLighting(gl);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.PushMatrix();

            if (animacijaUToku)
            {
                timer1.Start();
            }

            gl.LookAt(500, 0, 0, 0, 0, -5000, 0, 1, 0);

            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            m_scene.Draw();
            m_scene_bullet.Draw();

            reflektor(gl);

            loadGun(gl);
            loadBullet(gl);
            loadFloor(gl);
            loadTargetStand(gl);
            loadTarget(gl);
            loadText(gl);

            gl.PopMatrix();
            // Oznaci kraj iscrtavanja
            gl.Flush();
        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;

            gl.Viewport(0, 0, m_width, m_height);      

            gl.MatrixMode(OpenGL.GL_PROJECTION);      
            gl.LoadIdentity();
            gl.Perspective(60f, (double)width / height, 1.0f, 20000f);  
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();               
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        private void setUpLighting(OpenGL gl)
        {
            float[] globalAmbiental = { 0.1f, 0.1f, 0.1f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, globalAmbiental);        
                                                                                  
            float[] light0pos = new float[] { -2000.0f, 0.0f, 350.0f, 1.0f };

            float[] light0ambient = new float[] { ambLightR, ambLightG, ambLightB, 1.0f };
            float[] light0diffuse = new float[] { 0.6f, 0.6f, 0.6f, 1.0f };
            float[] light0specular = new float[] { 0.0f, 0.0f, 0.0f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);      
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);      
                                                                           
            gl.Enable(OpenGL.GL_LIGHTING);                          
            gl.Enable(OpenGL.GL_LIGHT0);                            
            gl.Enable(OpenGL.GL_NORMALIZE);

        }

        private void reflektor(OpenGL gl)
        {
            float[] ambijentalnaKomponenta = { ambLightR, ambLightG, ambLightB, 1.0f };
            float[] difuznaKomponenta = { 1.0f, 0.0f, 0.0f, 1.0f };
            float[] pozicija = new float[] { 1300f, 1000f, 100f, 1.0f };
            float[] smer = new float[] { 0.0f, -1.0f, 0.0f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, pozicija);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, ambijentalnaKomponenta);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, difuznaKomponenta);

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 25.0f);       

            gl.Enable(OpenGL.GL_LIGHT1);
        }

        private void LoadTextures(OpenGL gl)
        {
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.GenTextures(m_textureCount, m_textures);


            for (int i = 0; i < m_textureCount; ++i)
            {

                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                Bitmap image = new Bitmap(m_textureFiles[i]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);

                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);

                image.UnlockBits(imageData);
                image.Dispose();

                gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
            }
        }

        public void loadGun(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(-1300f, gunY, 1100f);
            gl.Rotate(90 + gunRotate, 0, 1, 0);
            gl.Scale(1800, 1800, 1800);
            m_scene.Draw();

            gl.PopMatrix();
        }

        public void loadBullet(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(bulletX, bulletY, 1100f);
            gl.Scale((float)duzinaMetaka, 15f, 15f);
            gl.Rotate(-90, 0, 0, 1);
            m_scene_bullet.Draw();
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(-1600f, -250f, 1100f);
            gl.Scale((float)duzinaMetaka, 15f, 15f);
            gl.Rotate(-90, 0, 0, 1);
            m_scene_bullet.Draw();
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(-1600f, -250f, 1100f);
            gl.Scale((float)duzinaMetaka, 15f, 15f);
            gl.Rotate(-90, 0, 0, 1);
            gl.Rotate(90, 1, 0, 0);
            m_scene_bullet.Draw();
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(-1500f, -250f, 1100f);
            gl.Scale((float)duzinaMetaka, 15f, 15f);
            gl.Rotate(-90, 0, 1, 0);
            m_scene_bullet.Draw();
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(-1450f, -250f, 1100f);
            gl.Scale((float)duzinaMetaka, 15f, 15f);
            gl.Rotate(-90, 0, 1, 0);
            m_scene_bullet.Draw();
            gl.PopMatrix();
        }

        public void loadFloor(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(-100f, -300f, 550f);
            gl.Scale(80, 20, 90);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.BETON]);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            //gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PushMatrix();
            gl.Begin(OpenGL.GL_QUADS); 

            gl.Color(0f, 0f, 0f);

            gl.Normal(0f, 1f, 0f);
            gl.TexCoord(1, 1);
            gl.Vertex(30, 1, 15);
            gl.TexCoord(1, 0);
            gl.Vertex(30, 1, -15);
            gl.TexCoord(0, 0);
            gl.Vertex(-30, 1, -15);
            gl.TexCoord(0, 1);
            gl.Vertex(-30, 1, 15);

            gl.Color(0f, 0f, 0f);
            gl.Normal(0f, 1f, 0f);
            gl.TexCoord(1, 1);
            gl.Vertex(30, 1, 15);
            gl.TexCoord(1, 0);
            gl.Vertex(-30, 1, 15);
            gl.TexCoord(0, 0);
            gl.Vertex(-30, -1, 15);
            gl.TexCoord(0, 1);
            gl.Vertex(30, -1, 15);

            gl.Color(0f, 0f, 0f);
            gl.Normal(0f, 1f, 0f);
            gl.TexCoord(1, 1);
            gl.Vertex(30, -1, 15);
            gl.TexCoord(1, 0);
            gl.Vertex(30, -1, -15);
            gl.TexCoord(0, 0);
            gl.Vertex(30, 1, -15);
            gl.TexCoord(0, 1);
            gl.Vertex(30, 1, 15);

            gl.Color(0f, 0f, 0f);
            gl.Normal(0f, 1f, 0f);
            gl.TexCoord(1, 1);
            gl.Vertex(-30, -1, 15);
            gl.TexCoord(1, 0);
            gl.Vertex(-30, -1, -15);
            gl.TexCoord(0, 0);
            gl.Vertex(30, -1, -15);
            gl.TexCoord(0, 1);
            gl.Vertex(30, -1, 15);

            gl.Color(0f, 0f, 0f);
            gl.Normal(0f, 1f, 0f);
            gl.TexCoord(1, 1);
            gl.Vertex(-30, -1, -15);
            gl.TexCoord(1, 0);
            gl.Vertex(-30, -1, 15);
            gl.TexCoord(0, 0);
            gl.Vertex(-30, 1, 15);
            gl.TexCoord(0, 1);
            gl.Vertex(-30, 1, -15);

            gl.Color(0f, 0f, 0f);
            gl.Normal(0f, 1f, 0f);
            gl.TexCoord(1, 1);
            gl.Vertex(-30, 1, -15);
            gl.TexCoord(1, 0);
            gl.Vertex(30, 1, -15);
            gl.TexCoord(0, 0);
            gl.Vertex(30, -1, -15);
            gl.TexCoord(0, 1);
            gl.Vertex(-30, -1, -15);

            gl.End();

            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);

            gl.PopMatrix();
        }

        public void loadTargetStand(OpenGL gl)
        {
            gl.PushMatrix();

            Cube targetStand = new Cube();
            gl.Translate(1300f, -50f, 600f);
            gl.Rotate(-90, 0, 1, 0);
            gl.Scale(700, 200, 40);

            Glu.GLUquadric gluObject = Glu.gluNewQuadric();
            Glu.gluQuadricTexture(gluObject, 1);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.DRVO]);
 //           gl.MatrixMode(OpenGL.GL_TEXTURE);

            //TODO 2: def koordinate

            targetStand.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();
        }

        public void loadTarget(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(targetX, targetY, 600f);              
            gl.Scale(30f + skaliranjeMeta, 100f + skaliranjeMeta, 30f + skaliranjeMeta);
            gl.Rotate(-90f, 1, 0, 0);

            Glu.gluQuadricTexture(gluObject, 1); 
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.METAL]);

            gl.Color(0.184f, 0.310f, 0.310);
            Cylinder target = new Cylinder();
            target.NormalOrientation = Orientation.Outside;
            target.TopRadius = 1;
            target.CreateInContext(gl);
            target.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();


            gl.PushMatrix();
            gl.Translate(1300f, 150f, 350f);
            gl.Scale(30f + skaliranjeMeta, 100f + skaliranjeMeta, 30f + skaliranjeMeta);
            gl.Rotate(-90f, 1, 0, 0);
            gl.Color(0.184f, 0.310f, 0.310);
            Cylinder target1 = new Cylinder();
            target1.NormalOrientation = Orientation.Outside;
            target1.TopRadius = 1;
            target1.CreateInContext(gl);
            target1.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(1300f, 150f, 100f);
            gl.Scale(30f + skaliranjeMeta, 100f + skaliranjeMeta, 30f + skaliranjeMeta);
            gl.Rotate(-90f, 1, 0, 0);
            gl.Color(0.184f, 0.310f, 0.310);
            Cylinder target2 = new Cylinder();
            target2.NormalOrientation = Orientation.Outside;
            target2.TopRadius = 1;
            target2.CreateInContext(gl);
            target2.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(targetX1, targetY1, 950f);
            gl.Scale(30f + skaliranjeMeta, 100f + skaliranjeMeta, 30f + skaliranjeMeta);
            gl.Rotate(-90f, 1, 0, 0);
            gl.Color(0.184f, 0.310f, 0.310);
            Cylinder target3 = new Cylinder();
            target3.NormalOrientation = Orientation.Outside;
            target3.TopRadius = 1;
            target3.CreateInContext(gl);
            target3.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();


            gl.PushMatrix();
            gl.Translate(targetX2, targetY2, 1200f);
            gl.Scale(30f + skaliranjeMeta, 100f + skaliranjeMeta, 30f + skaliranjeMeta);
            gl.Rotate(-90f, 1, 0, 0);
            gl.Color(0.184f, 0.310f, 0.310);
            Cylinder target4 = new Cylinder();
            target4.NormalOrientation = Orientation.Outside;
            target4.TopRadius = 1;
            target4.CreateInContext(gl);
            target4.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

        }

        public void loadText(OpenGL gl)
        {
            gl.MatrixMode(OpenGL.GL_PROJECTION);  
            gl.PushMatrix();
            gl.Viewport(m_width / 2, 0, m_width / 2, m_height / 2);

           
            gl.DrawText(m_width - 350, 150, 1.0f, 1.0f, 1.0f, "Arial", 10, "Predmet: Racunarska grafika");
            gl.DrawText(m_width - 350, 130, 1.0f, 1.0f, 1.0f, "Arial", 10, "Sk.god: 2020/21.");
            gl.DrawText(m_width - 350, 110, 1.0f, 1.0f, 1.0f, "Arial", 10, "Ime: Maja ");
            gl.DrawText(m_width - 350, 90, 1.0f, 1.0f, 1.0f, "Arial", 10, "Prezime: Stamenic");
            gl.DrawText(m_width - 350, 70, 1.0f, 1.0f, 1.0f, "Arial", 10, "Sifra zad: 2.1. ");

            gl.Viewport(0, 0, m_width, m_height);
            gl.PopMatrix();

        }

        public void dodeliMainWindow(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        //Animacija
        public void animation(object sender, EventArgs e)
        {
            if (!restartujAnimaciju)
            {
                if (gunY < 50)
                {
                    gunY += 50;
                }
                else
                {
                    pistoljPostavljen = true;
                }
                if (pistoljPostavljen && bulletY < 265)
                {
                    bulletY = 265;
                    pistoljSpreman = true;
                }
                if (pistoljSpreman && !metaPogodjena && !metaPogodjena1)
                {
                    if (!pistoljTrzaj) 
                    {
                        if (gunRotate == 0)
                        {
                            gunRotate = 50;
                        }
                        else if (gunRotate == 50)
                        {
                            gunRotate = 0;
                            pistoljTrzaj = true;
                        }
                    }
                    if (bulletX < 1200)
                    {
                        bulletX += 100;
                    }
                    else
                    {
                        metaPogodjena = true;
                    }
                }
                if (metaPogodjena && !metaOborena && !metaOborena1)
                {
                    bulletX = -1200f;
                    if (targetX < 1500)
                    {
                        targetX += 50;
                    }
                    else if (targetY > -300)
                    {
                        targetY -= 50;
                    }
                    else
                    {
                        metaOborena = true;
                    }
                }
                if (metaOborena && !metaPogodjena1)
                {
                    if (!pistoljTrzaj1)
                    {
                        if (gunRotate == 0)
                        {
                            gunRotate = 50;
                        }
                        else if (gunRotate == 50)
                        {
                            gunRotate = 0;
                            pistoljTrzaj1 = true;
                        }
                    }
                    if (bulletX < 1200)
                    {
                        bulletX += 100;
                    }
                    else
                    {
                        metaPogodjena1 = true;
                    }
                }
                if (metaPogodjena1 && !metaOborena1)
                {
                    bulletX = -1200f;
                    if (targetX1 < 1500)
                    {
                        targetX1 += 100;
                    }
                    else if (targetY1 > -300)
                    {
                        targetY1 -= 50;
                    }
                    else
                    {
                        metaOborena1 = true;
                    }
                }
                if (metaOborena1)
                {
                    if (!pistoljTrzaj2)
                    {
                        if (gunRotate == 0)
                        {
                            gunRotate = 50;
                        }
                        else if (gunRotate == 50)
                        {
                            gunRotate = 0;
                            pistoljTrzaj2 = true;
                        }
                    }
                    if (bulletX < 1200)
                    {
                        bulletX += 100;
                    }
                    else
                    {
                        metaPogodjena2 = true;
                    }
                }
                if (metaPogodjena2)
                {
                    bulletX = -1200f;
                    if (targetX2 < 1500)
                    {
                        targetX2 += 100;
                    }
                    else if (targetY2 > -300)
                    {
                        targetY2 -= 50;
                    }
                    else
                    {
                        metaOborena2 = true;
                    }
                }
                if (metaOborena2)
                {
                    restartujAnimaciju = true;
                }
            }
            else
            {
                timer1.Stop();
                animacijaUToku = false;

                pistoljPostavljen = false;
                pistoljSpreman = false;
                metakSeKrece = false;
                metaPogodjena = false;
                metaOborena = false;
                metaPogodjena1 = false;
                metaOborena1 = false;
                metaPogodjena2 = false;
                metaOborena2 = false;
                pistoljTrzaj = false;
                pistoljTrzaj1 = false;
                pistoljTrzaj2 = false;

               gunY = -300f;
               bulletX = -1200f;
               bulletY = -195f;
               targetX = 1300f;
               targetY = 150f;
               targetX1 = 1300f;
               targetY1 = 150f;
               targetX2 = 1300f;
               targetY2 = 150f;

                restartujAnimaciju = false;
                mainWindow.omoguciInterakciju();
            }
        }
        

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
