using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GLGUI;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace PBTInspector
{
    class PBTOverviewControl<DataType> : GLControl
    {
        public readonly PBTTreeContainer TreeContainer;

        private Color4 textColor = Color.FromArgb(255, 255, 255);
        private Color4 backgroundColor = Color.FromArgb(96, 96, 96);

        public PBTOverviewControl(GLGui gui, PBTTreeContainer treeContainer)
            : base(gui)
        {
            TreeContainer = treeContainer;
            Render += OnRender;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseMove += OnMouseMove;
        }

        private List<Tuple<Point, Point>> lines = new List<Tuple<Point, Point>>(64);
        private void OnRender(object sender, double delta)
        {
            GLDraw.Fill(ref backgroundColor);
            var root = (PBTTaskTreeControl<DataType>)TreeContainer.Controls.First().Controls.FirstOrDefault(c => c is PBTTaskTreeControl<DataType>);
            if (root != null)
            {
                lines.Clear();
                float scale = Math.Min(InnerWidth / (float)root.Width, InnerHeight / (float)root.Height);
                RenderSubtree(root, 0, 0, scale);

                GLDraw.PrepareCustomDrawing();
                var sr = GLDraw.CurrentScreenRect;
                GL.Color3(Color.White);
                GL.LineWidth(scale);
                GL.Enable(EnableCap.LineSmooth);
                GL.Begin(PrimitiveType.Lines);
                for (int i = 0; i < lines.Count; i++)
                {
                    GL.Vertex2(sr.X + lines[i].Item1.X, sr.Y + lines[i].Item1.Y);
                    GL.Vertex2(sr.X + lines[i].Item2.X, sr.Y + lines[i].Item2.Y);
                }
                GL.End();

                GL.LineWidth(2.0f);
                GL.Begin(PrimitiveType.LineLoop);
                GL.Vertex2(sr.X + TreeContainer.ScrollPosition.X * scale, sr.Y + TreeContainer.ScrollPosition.Y * scale);
                GL.Vertex2(sr.X + (TreeContainer.ScrollPosition.X + TreeContainer.InnerWidth) * scale, sr.Y + TreeContainer.ScrollPosition.Y * scale);
                GL.Vertex2(sr.X + (TreeContainer.ScrollPosition.X + TreeContainer.InnerWidth) * scale, sr.Y + (TreeContainer.ScrollPosition.Y + TreeContainer.InnerHeight) * scale);
                GL.Vertex2(sr.X + TreeContainer.ScrollPosition.X * scale, sr.Y + (TreeContainer.ScrollPosition.Y + TreeContainer.InnerHeight) * scale);
                GL.End();
            }
        }

        private Point RenderSubtree(PBTTaskTreeControl<DataType> ttc, int x, int y, float scale)
        {
            // add offset
            if (ttc.Parent is GLGroupLayout)
            {
                x += ttc.Parent.X; y += ttc.Parent.Y;
            }
            x += ttc.X; y += ttc.Y;

            // draw box
            var tc = ttc.TaskControl;
            Rectangle border = new Rectangle(
                (int)((x + tc.X) * scale),
                (int)((y + tc.Y) * scale),
                (int)(tc.Width * scale),
                (int)(tc.Height * scale));
            Rectangle content = new Rectangle(border.X + 1, border.Y + 1, border.Width - 2, border.Height - 2);
            var borderColor = tc.Skin.BorderColor;
            var contentColor = tc.Skin.BackgroundColor;
            GLDraw.FillRect(ref border, ref borderColor);
            GLDraw.FillRect(ref content, ref contentColor);
            //GLDraw.Text((GLFontText)typeof(GLLabel).GetField("textProcessed", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(tc.title), ref content, ref textColor);
            Point bottom = new Point(border.X, border.Y + border.Height);
            float dx = border.Width / (float)(ttc.Subtrees.Count + 1);

            // draw subtrees
            for (int i = 0; i < ttc.Subtrees.Count; i++)
            {
                bottom.X = border.X + (int)(dx + i * dx);
                lines.Add(new Tuple<Point, Point>(bottom, RenderSubtree(ttc.Subtrees[i], x, y, scale)));
            }
            return new Point((int)(border.X + border.Width / 2.0), border.Y);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                isDragged = true;
                ScrollTo(e.X, e.Y);
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
                isDragged = false;
        }

        private void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            if (isDragged)
                ScrollTo(e.X, e.Y);
        }

        private void ScrollTo(float x, float y)
        {
            var root = (PBTTaskTreeControl<DataType>)TreeContainer.Controls.First().Controls.FirstOrDefault(c => c is PBTTaskTreeControl<DataType>);
            if (root != null)
            {
                float scale = Math.Min(InnerWidth / (float)root.Width, InnerHeight / (float)root.Height);
                x /= scale; y /= scale;
                if (TreeContainer.Horizontal.Enabled)
                    TreeContainer.Horizontal.Value = (x - TreeContainer.InnerWidth * 0.5f) / TreeContainer.ScrollFreedom.Width;
                if (TreeContainer.Vertical.Enabled)
                    TreeContainer.Vertical.Value = (y - TreeContainer.InnerHeight * 0.5f) / TreeContainer.ScrollFreedom.Height;
            }
        }
    }
}
