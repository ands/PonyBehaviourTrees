using System.Collections.Generic;
using System.IO;
using System.Text;
using GLGUI;
using OpenTK.Graphics;

namespace PBTInspector
{
    class LabelWriter : TextWriter
    {
        private GLLabel label;
        private readonly List<string> data = new List<string>(1024);
        private StringBuilder currentLine = new StringBuilder(256);

        public LabelWriter(GLLabel label)
        {
            this.label = label;
        }

        public override void Write(char value)
        {
            if (value == '\n')
                Flush();
            else
                currentLine.Append(value);
        }

        public override void Flush()
        {
            if (data.Count == 1024)
                data.RemoveAt(0);
            data.Add(currentLine.ToString());
            currentLine.Clear();

            try
            {
                label.Gui.ParentControl.MakeCurrent();
                label.Text = string.Join("\n", data);
                label.Gui.ParentControl.Context.MakeCurrent(null);
            }
            catch(GraphicsContextException)
            {
            }
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }

        public void Clear()
        {
            data.Clear();
            currentLine.Clear();

            try
            {
                label.Gui.ParentControl.MakeCurrent();
                label.Text = "";
                label.Gui.ParentControl.Context.MakeCurrent(null);
            }
            catch (GraphicsContextException)
            {
            }
        }
    }
}
