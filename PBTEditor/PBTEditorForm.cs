using System.Drawing;
using System.Windows.Forms;
using System;

namespace PBTEditor
{
    public class PBTEditorForm : Form
    {
        public PBTEditorControl Editor;

        public PBTEditorForm(Type dataType, Type impulseType, string pbtSearchPath)
        {
            Text = "PBT Editor";
            Size = new Size(1024, 600);

            Editor = new PBTEditorControl(dataType, impulseType, pbtSearchPath);
            Editor.Dock = DockStyle.Fill;
            Controls.Add(Editor);
        }
    }
}

