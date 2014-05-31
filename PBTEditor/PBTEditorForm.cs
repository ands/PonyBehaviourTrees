using System;
using System.Drawing;
using System.Windows.Forms;

namespace PBTEditor
{
    /// <summary>
    /// A form that contains a pbt editor.
    /// </summary>
    public class PBTEditorForm : Form
    {
        /// <summary>
        /// The editor instance.
        /// </summary>
        public PBTEditorControl Editor;

        /// <summary>
        /// Constructs a form that contains a pbt editor.
        /// </summary>
        /// <param name="dataType">The type of the pbt-controlled entity.</param>
        /// <param name="impulseType">The type of the impulse enum to use.</param>
        /// <param name="pbtSearchPath">The pbt base path.</param>
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

