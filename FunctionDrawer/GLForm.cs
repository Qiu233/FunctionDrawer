using Evaluation;
using SharpGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FunctionDrawer
{
	public partial class GLForm : Form
	{
		private OpenGLControl oc;
		private Executer[] exe;
		private float[][] Colors;
		private int ScaleX, ScaleY;
		public GLForm(int scaleX, int scaleY, params string[] expr)
		{
			Random r = new Random();
			exe = new Executer[expr.Length];
			Colors = new float[expr.Length][];
			for (int i = 0; i < expr.Length; i++)
			{
				exe[i] = Executer.Create(expr[i]);
				//x^sin(x)
				exe[i].RegisterFunction("abs", typeof(Math).GetMethod("Abs", new Type[] { typeof(double) }));
				exe[i].RegisterFunction("sin", typeof(Math).GetMethod("Sin"));
				exe[i].RegisterFunction("cos", typeof(Math).GetMethod("Cos"));
				exe[i].RegisterFunction("log", typeof(Math).GetMethod("Log", new Type[] { typeof(double), typeof(double) }));

				Colors[i] = new float[3];
				Colors[i][0] = (float)(r.Next() % 100) / 100;
				Colors[i][1] = (float)(r.Next() % 100) / 100;
				Colors[i][2] = (float)(r.Next() % 100) / 100;
			}
			this.ScaleX = scaleX;
			this.ScaleY = scaleY;
			InitializeComponent();
			oc = new OpenGLControl();
			((ISupportInitialize)(oc)).BeginInit();

			oc.Dock = DockStyle.Fill;
			oc.DrawFPS = true;
			oc.FrameRate = 20;
			oc.RenderContextType = RenderContextType.FBO;

			oc.OpenGLInitialized += Oc_OpenGLInitialized;
			oc.OpenGLDraw += Oc_OpenGLDraw;
			oc.Resized += Oc_Resized;
			Controls.Add(oc);
			((ISupportInitialize)(oc)).EndInit();
		}

		private void Oc_OpenGLDraw(object sender, RenderEventArgs e)
		{
			OpenGL gl = oc.OpenGL;
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

			gl.LoadIdentity();
			gl.ClearColor(0.1f, 0.1f, 0.1f, 0.5f);

			gl.Color(1f, 1f, 1f);

			gl.Begin(OpenGL.GL_LINES);


			DrawAxis(gl);
			DrawScale(gl);

			DrawLabel(gl);

			bool continuous = false;
			double lX = 0, lY = 0;
			for (int t = 0; t < exe.Length; t++)
			{
				var ex = exe[t];
				continuous = false;
				gl.Color(Colors[t][0], Colors[t][1], Colors[t][2]);
				for (double i = -ScaleX; i <= ScaleX; i += 0.01)
				{
					ex.SetVariable("x", i);
					try
					{
						double v = (float)ex.Calculate();
						if (continuous)
						{
							gl.Vertex(lX, lY);
							gl.Vertex(i, v);
						}
						continuous = true;
						lX = i;
						lY = v;
					}
					catch (Exception err)
					{
						err.ToString();
						continuous = false;
						continue;
					}
				}
				gl.Color(1f, 1f, 1f);
			}

			DrawMouse(gl);

			gl.End();
			gl.Flush();
		}

		private void DrawMouse(OpenGL gl)
		{
			Point p = oc.PointToClient(MousePosition);
			double d1 = (double)oc.Width / ((double)ScaleX * 2);
			double d2 = (double)oc.Height / ((double)ScaleY * 2);
			double g1 = (double)p.X / d1;
			double g2 = (double)p.Y / d2;
			double h1 = g1 - ScaleX;
			double h2 = ScaleY - g2;
			gl.Color(0.5f, 0.5f, 0.5f);
			gl.Vertex(h1, -ScaleY);
			gl.Vertex(h1, ScaleY);
			gl.Vertex(-ScaleX, h2);
			gl.Vertex(ScaleX, h2);
			gl.End();
			DrawText(gl, (float)(h1 + (double)ScaleX / 30), (float)(h2 - (double)ScaleY / 25), 15, "(" + h1.ToString("f2") + "," + h2.ToString("f2") + ")");
			gl.Begin(OpenGL.GL_LINES);
			gl.Color(1f, 1f, 1f);
		}

		private void DrawLabel(OpenGL gl)
		{
			gl.End();
			double unit = (double)ScaleY / 20;
			for (int i = 0; i < exe.Length; i++)
			{
				gl.Color(Colors[i][0], Colors[i][1], Colors[i][2]);
				gl.Rect(-ScaleX * 0.95, ScaleY * 0.95 - i * unit, -ScaleX * 0.92, ScaleY * 0.92 - i * unit);
				DrawText(gl, (float)(-ScaleX * 0.95 + (float)ScaleX / 20), (float)(ScaleY * 0.95 - i * unit) - (float)ScaleY / 40, 12, exe[i].Expression);
			}
			gl.Color(1f, 1f, 1f);

			gl.Begin(OpenGL.GL_LINES);
		}

		private void DrawText(OpenGL gl, float x, float y, int size, string str)
		{
			double d1 = (double)oc.Width / ((double)ScaleX * 2);
			double d2 = (double)oc.Height / ((double)ScaleY * 2);
			double g1 = x * d1;
			double g2 = y * d2;
			gl.DrawText((int)(g1 + oc.Width / 2), (int)(g2 + oc.Height / 2), 1, 1, 1, "Arial", size, str);
		}



		private void DrawAxis(OpenGL gl)
		{
			//x
			gl.Vertex(-ScaleX, 0);
			gl.Vertex(ScaleX, 0);
			//arrow
			gl.Vertex(ScaleX - (double)ScaleX / 70, (double)ScaleY / 70);
			gl.Vertex(ScaleX, 0);
			gl.Vertex(ScaleX - (double)ScaleX / 70, -(double)ScaleY / 70);
			gl.Vertex(ScaleX, 0);

			gl.End();

			DrawText(gl, (float)(ScaleX - (double)ScaleX / 70 - (double)ScaleX / 20), (float)(-(double)ScaleY / 70 - (double)ScaleY / 25), 15, "x");

			gl.Begin(OpenGL.GL_LINES);

			//y
			gl.Vertex(0, -ScaleY);
			gl.Vertex(0, ScaleY);
			//arrow
			gl.Vertex((double)ScaleX / 70, ScaleY - (double)ScaleY / 70);
			gl.Vertex(0, ScaleY);
			gl.Vertex(-(double)ScaleX / 70, ScaleY - (double)ScaleY / 70);
			gl.Vertex(0, ScaleY);

			gl.End();

			DrawText(gl, (float)(-(double)ScaleX / 70 - (double)ScaleX / 25), (float)(ScaleY - (double)ScaleY / 70 - (double)ScaleY / 20), 15, "y");

			gl.Begin(OpenGL.GL_LINES);
		}

		private void DrawScale(OpenGL gl)
		{
			for (int i = -ScaleX + 1; i < ScaleX; i++)
			{
				if (i == 0) continue;
				gl.Vertex(i, (double)ScaleY / 150);
				gl.Vertex(i, -(double)ScaleY / 150);
			}
			for (int i = -ScaleY + 1; i < ScaleY; i++)
			{
				if (i == 0) continue;
				gl.Vertex((double)ScaleX / 150, i);
				gl.Vertex(-(double)ScaleX / 150, i);
			}
			gl.End();
			for (int i = -5; i <= 5; i++)
			{
				if (i == 0) continue;
				DrawText(gl, i, -0.5f, 10, i.ToString());
			}
			for (int i = -5; i <= 5; i++)
			{
				if (i == 0) continue;
				DrawText(gl, 0.5f, i, 10, i.ToString());
			}
			gl.Begin(OpenGL.GL_LINES);
		}



		private void Oc_Resized(object sender, EventArgs e)
		{
			OpenGL gl = oc.OpenGL;
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.LoadIdentity();
			gl.Perspective(90.0f, (double)oc.Width / (double)oc.Height, 1, 100);
			gl.LookAt(0, 0, -1, 0, 0, 0, 0, 1, 0);
			gl.Ortho(ScaleX, -ScaleX, -ScaleY, ScaleY, 10, -10);
			gl.MatrixMode(OpenGL.GL_MODELVIEW);

		}
		private void Oc_OpenGLInitialized(object sender, EventArgs e)
		{
		}

	}
}
