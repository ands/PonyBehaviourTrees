using System;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PBTExample
{
    public partial class MainForm : Form
    {
        Simulation simulation;

        public MainForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            simulation = new Simulation(this);

            Application.Idle += (s, e) => Invalidate();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            simulation.Draw(e.Graphics);
        }

        private void editPBTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var editorForm = new PBTEditor.PBTEditorForm(typeof(Actor), typeof(ActorImpulses), ".");
            editorForm.Editor.PBTUpdate += PBT.LeafTasks.Reference<Actor>.ReplacePBT;
            editorForm.Show();
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            simulation = new Simulation(this);
        }
    }
}