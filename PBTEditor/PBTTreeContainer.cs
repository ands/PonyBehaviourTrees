using GLGUI;
using System.Drawing;
using System;
using System.Linq;
using OpenTK.Input;

namespace PBTEditor
{
    class PBTTreeContainer : GLScrollableControl
    {
        public PBTTreeContainer(GLGui gui)
            : base(gui)
        {
            Anchor = GLAnchorStyles.All;
            var skin = Skin;
            skin.BackgroundColor = System.Drawing.Color.FromArgb(96, 96, 96);
            skin.BorderColor = gui.Skin.FormActive.BorderColor;
            Skin = skin;

            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseMove += OnMouseMove;
        }

        Point dragStart;
        Point scrollStart;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            isDragged = true;
            dragStart = e.Position;
            scrollStart = ScrollPosition;
            Gui.Cursor = GLCursor.SizeAll;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragged = false;
            Gui.Cursor = GLCursor.Default;
        }

        private void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            if (isDragged && Controls.Count() > 0)
            {
                int dx = e.X - dragStart.X;
                int dy = e.Y - dragStart.Y;
                int sx = scrollStart.X - dx;
                int sy = scrollStart.Y - dy;
                if (Horizontal.Enabled)
                    Horizontal.Value = Math.Max(0.0f, Math.Min(1.0f, sx / (float)ScrollFreedom.Width));
                if (Vertical.Enabled)
                    Vertical.Value = Math.Max(0.0f, Math.Min(1.0f, sy / (float)ScrollFreedom.Height));
                Invalidate();
            }
        }
    }
}
