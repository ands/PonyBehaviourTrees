using GLGUI;
using System.Collections.Generic;
using System.Drawing;
using System;
using System.Linq;
using OpenTK.Graphics.OpenGL;

namespace PBTEditor
{
    class PBTTaskTreeControl : GLGroupLayout
    {
        public readonly PBTEditorControl Editor;
        public readonly PBTTaskTreeControl ParentTaskTreeControl;
        public readonly PBTTaskControl TaskControl;
        public readonly List<PBTTaskTreeControl> Subtrees = new List<PBTTaskTreeControl>();

        private const int VSpace = 16;
        private GLGroupLayout horizontalFlow;

        public PBTTaskTreeControl(GLGui gui, PBTEditorControl editor, PBTTaskTreeControl parentTaskTreeControl, Data.Task task)
            : base(gui)
        {
            Editor = editor;
            ParentTaskTreeControl = parentTaskTreeControl;
            Render += OnRender;
            HandleMouseEvents = false;
            AutoSize = true;
            TaskControl = Add(new PBTTaskControl(gui, editor, this, task));

            horizontalFlow = Add(new GLGroupLayout(gui)
            {
                AutoSize = true,
                HandleMouseEvents = false,
                Location = new Point(0, TaskControl.Height + VSpace)
            });

            foreach (var subtask in task.Subtasks)
                Subtrees.Add(horizontalFlow.Add(new PBTTaskTreeControl(gui, editor, this, subtask)));
        }

        protected override void UpdateLayout()
        {
            if (Subtrees.Count > 0)
            {
                // generate a tight tree
                for (int i = 1; i < Subtrees.Count; i++)
                {
                    int x = Subtrees.Take(i).Max(st => st.GetRightAbove(Subtrees[i].Height));
                    x += Subtrees[i].X;
                    x -= Subtrees.Take(i).Min(st => Subtrees[i].GetLeftAbove(st.Height)); // TODO: subtract widths of inbetween subtrees (min .. i-1) or something
                    if (x > 0)
                        Subtrees[i].Location = new Point(x, 0);
                }

                int r = Subtrees[Subtrees.Count - 1].X + Subtrees[Subtrees.Count - 1].Inner.X + Subtrees[Subtrees.Count - 1].TaskControl.Outer.Right;
                int l = Subtrees[0].X + Subtrees[0].Inner.X + Subtrees[0].TaskControl.X;
                int tx = l + (r - l - TaskControl.Width) / 2;
                TaskControl.Location = new Point(Math.Max(tx, 0), 0);
                horizontalFlow.Location = new Point(-Math.Min(tx, 0), TaskControl.Outer.Bottom + VSpace);
            }

            base.UpdateLayout();
        }

        private void OnRender(object sender, double delta)
        {
            if (Subtrees.Count == 0)
                return;

            // draw connection lines
            GLDraw.PrepareCustomDrawing();
            var sr = GLDraw.CurrentScreenRect;
            GL.Color3(Color.White);
            GL.LineWidth(1.0f);//3f);
            GL.Enable(EnableCap.LineSmooth);
            GL.Begin(PrimitiveType.Lines);
            for (int i = 0; i < Subtrees.Count; i++)
            {
                var src = new OpenTK.Vector2(
                    sr.X + TaskControl.X + TaskControl.Width * (i + 1) / (Subtrees.Count + 1),
                    sr.Y + TaskControl.Outer.Bottom + 1);
                var dst = new OpenTK.Vector2(
                    sr.X + Subtrees[i].Parent.X + Subtrees[i].Parent.Inner.X + Subtrees[i].X + Subtrees[i].Inner.X + Subtrees[i].TaskControl.X + Subtrees[i].TaskControl.Width / 2,
                    sr.Y + Subtrees[i].Parent.Y + Subtrees[i].Parent.Inner.Y + Subtrees[i].Y + Subtrees[i].Inner.Y + Subtrees[i].TaskControl.Y + 1);

                float srcX2 = src.X + (dst.X - src.X) * 0.02f;
                float dstX2 = dst.X - (dst.X - src.X) * 0.02f;

                GL.Vertex2(src);
                GL.Vertex2(srcX2, src.Y + 2);

                GL.Vertex2(srcX2, src.Y + 2);
                GL.Vertex2(dstX2, dst.Y - 2);

                GL.Vertex2(dstX2, dst.Y - 2);
                GL.Vertex2(dst);
            }
            GL.End();
        }

        protected int GetRightAbove(int maxY, int curX = 0, int curY = 0, int curR = 0)
        {
            curX += Location.X + Inner.X;
            curY += Location.Y + Inner.Y + TaskControl.Y + TaskControl.Height + VSpace;
            curR = Math.Max(curR, curX + TaskControl.Outer.Right);
            if (curY >= maxY || Subtrees.Count == 0)
                return curR;
            return Subtrees.Max(st => st.GetRightAbove(maxY, curX, curY, curR)) + 2;
        }

        protected int GetLeftAbove(int maxY, int curX = 0, int curY = 0, int curL = int.MaxValue)
        {
            curX += Location.X + Inner.X;
            curY += Location.Y + Inner.Y + TaskControl.Y + TaskControl.Height + VSpace;
            curL = Math.Min(curL, curX + TaskControl.X);
            if (curY >= maxY || Subtrees.Count == 0)
                return curL;
            return Subtrees.Min(st => st.GetLeftAbove(maxY, curX, curY, curL)) - 2;
        }
    }
}
