using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;

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
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);

            gl.Enable(OpenGL.GL_DEPTH_TEST);        //TODO 1: Ukljuceno testiranje dubine
            gl.Enable(OpenGL.GL_CULL_FACE);         //TODO 2: Sakrivanje nevidljivih povrsina

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
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            m_scene.Draw();
            m_scene_bullet.Draw();

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

            gl.Viewport(0, 0, m_width, m_height);      //TODO 4: Viewport preko celog prozora

            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(60f, (double)width / height, 1.0f, 20000f);  //TODO 3: fov = 60, near = 1
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
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

        public void loadGun(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(-1300f, 150f, 1100f);
            gl.Rotate(90, 0, 1, 0);
            gl.Scale(1800, 1800, 1800);
            m_scene.Draw();

            gl.PopMatrix();
        }

        public void loadBullet(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(-1130f, 365f, 1100f);
            gl.Scale(30f, 15f, 15f);
            gl.Rotate(-90, 0, 0, 1);
            m_scene_bullet.Draw();
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(-1600f, -250f, 1100f);
            gl.Scale(30f, 15f, 15f);
            gl.Rotate(-90, 0, 0, 1);
            m_scene_bullet.Draw();
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(-1600f, -250f, 1100f);
            gl.Scale(15f, 15f, 30f);
            gl.Rotate(-90, 0, 0, 1);
            gl.Rotate(90, 1, 0, 0);
            m_scene_bullet.Draw();
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(-1500f, -250f, 1100f);
            gl.Scale(15f, 30f, 15f);
            gl.Rotate(-90, 0, 1, 0);
            m_scene_bullet.Draw();
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(-1450f, -250f, 1100f);
            gl.Scale(15f, 30f, 15f);
            gl.Rotate(-90, 0, 1, 0);
            m_scene_bullet.Draw();
            gl.PopMatrix();
        }

        public void loadFloor(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(-100f, -300f, 550f);
            gl.Scale(80, 10, 90);


            gl.Begin(OpenGL.GL_QUADS); 

            gl.Color(0.980, 0.922, 0.843);

            gl.Normal(0f, 1f, 0f);
            gl.Vertex(30, 1, 15);
            gl.Vertex(30, 1, -15);
            gl.Vertex(-30, 1, -15);
            gl.Vertex(-30, 1, 15);

            gl.Normal(0f, 1f, 0f);
            gl.Vertex(30, 1, 15);
            gl.Vertex(-30, 1, 15);
            gl.Vertex(-30, -1, 15);
            gl.Vertex(30, -1, 15);

            gl.Normal(0f, 1f, 0f);
            gl.Vertex(30, -1, 15);
            gl.Vertex(30, -1, -15);
            gl.Vertex(30, 1, -15);
            gl.Vertex(30, 1, 15);

            gl.Normal(0f, 1f, 0f);
            gl.Vertex(-30, -1, 15);
            gl.Vertex(-30, -1, -15);
            gl.Vertex(30, -1, -15);
            gl.Vertex(30, -1, 15);

            gl.Normal(0f, 1f, 0f);
            gl.Vertex(-30, -1, -15);
            gl.Vertex(-30, -1, 15);
            gl.Vertex(-30, 1, 15);
            gl.Vertex(-30, 1, -15);

            gl.Normal(0f, 1f, 0f);
            gl.Vertex(-30, 1, -15);
            gl.Vertex(30, 1, -15);
            gl.Vertex(30, -1, -15);
            gl.Vertex(-30, -1, -15);

            gl.End();

            gl.PopMatrix();
        }

        public void loadTargetStand(OpenGL gl)
        {
            gl.PushMatrix();

            Cube targetStand = new Cube();
            gl.Translate(1600f, -50f, 600f);
            gl.Rotate(-90, 0, 1, 0);
            gl.Scale(700, 200, 40);

            gl.Color(0.627, 0.322, 0.176);
            targetStand.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();
        }

        public void loadTarget(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(1600f, 150, 600f);              
            gl.Scale(30f, 100f, 30f);
            gl.Rotate(-90f, 1, 0, 0);
            gl.Color(0.184f, 0.310f, 0.310); 
            Cylinder target = new Cylinder();
            target.NormalOrientation = Orientation.Outside;
            target.TopRadius = 1;
            target.CreateInContext(gl);
            target.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();


            gl.PushMatrix();
            gl.Translate(1600f, 150, 350f);
            gl.Scale(30f, 100f, 30f);
            gl.Rotate(-90f, 1, 0, 0);
            gl.Color(0.184f, 0.310f, 0.310);
            Cylinder target1 = new Cylinder();
            target1.NormalOrientation = Orientation.Outside;
            target1.TopRadius = 1;
            target1.CreateInContext(gl);
            target1.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(1600f, 150, 100f);
            gl.Scale(30f, 100f, 30f);
            gl.Rotate(-90f, 1, 0, 0);
            gl.Color(0.184f, 0.310f, 0.310);
            Cylinder target2 = new Cylinder();
            target2.NormalOrientation = Orientation.Outside;
            target2.TopRadius = 1;
            target2.CreateInContext(gl);
            target2.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(1600f, 150, 950f);
            gl.Scale(30f, 100f, 30f);
            gl.Rotate(-90f, 1, 0, 0);
            gl.Color(0.184f, 0.310f, 0.310);
            Cylinder target3 = new Cylinder();
            target3.NormalOrientation = Orientation.Outside;
            target3.TopRadius = 1;
            target3.CreateInContext(gl);
            target3.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();


            gl.PushMatrix();
            gl.Translate(1600f, 150, 1200f);
            gl.Scale(30f, 100f, 30f);
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
            gl.Viewport(m_width / 2, 0, m_width/2, m_height/2);
            gl.LoadIdentity();                      

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            gl.PushMatrix();
            gl.DrawText(200, 150, 1.0f, 1.0f, 1.0f, "Arial", 10, "Predmet: Racunarska grafika");
            gl.PopMatrix();

            gl.PushMatrix();
            gl.DrawText(200, 130, 1.0f, 1.0f, 1.0f, "Arial", 10, "Sk.god: 2020/21.");
            gl.PopMatrix();

            gl.PushMatrix();
            gl.DrawText(200, 110, 1.0f, 1.0f, 1.0f, "Arial", 10, "Ime: Maja ");
            gl.PopMatrix();

            gl.PushMatrix();
            gl.DrawText(200, 90, 1.0f, 1.0f, 1.0f, "Arial", 10, "Prezime: Stamenic");
            gl.PopMatrix();

            gl.PushMatrix();
            gl.DrawText(200, 70, 1.0f, 1.0f, 1.0f, "Arial", 10, "Sifra zad: 2.1. ");
            gl.PopMatrix();


            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.Viewport(0, 0, m_width, m_height);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

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
