using System;
using System.Drawing;
using System.Windows.Forms;

namespace PBTInspector
{
    /// <summary>
    /// A form that contains a pbt inspector.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
    /// <typeparam name="ImpulseType"></typeparam>
    public class PBTInspectorForm<DataType, ImpulseType> : Form
    {
        /// <summary>
        /// The inspector instance.
        /// </summary>
        public PBTInspectorControl<DataType, ImpulseType> Inspector;

        /// <summary>
        /// Constructs a form that contains a pbt inspector.
        /// </summary>
        /// <param name="pbt">The root of the pbt to inspect.</param>
        public PBTInspectorForm(PBT.RootTask<DataType> pbt)
        {
            Text = "PBT Inspector";
            Size = new Size(1024, 600);

            Inspector = new PBTInspectorControl<DataType, ImpulseType>(pbt);
            Inspector.Dock = DockStyle.Fill;
            Controls.Add(Inspector);
        }
    }
}

