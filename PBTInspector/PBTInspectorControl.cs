using System;
using System.Windows.Forms;
using GLGUI;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using GLGUI.Advanced;
using System.Drawing;

namespace PBTInspector
{
    /// <summary>
    /// The pbt inspector control.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
    /// <typeparam name="ImpulseType"></typeparam>
    public class PBTInspectorControl<DataType, ImpulseType> : OpenTK.GLControl
	{
        internal readonly PBT.RootTask<DataType> Root;
        internal GLScrollableControl TreeContainer { get; private set; }

        GLGui glGui;
        GLDataControl dataControl;
        GLLabel impulseLog;
        PBTImpulseLogger<DataType, ImpulseType> impulseLogger;

        private static GLFont monospaceFont;

        /// <summary>
        /// Constructs the pbt inspector.
        /// </summary>
        /// <param name="pbt">The root of the pbt to inspect.</param>
        public PBTInspectorControl(PBT.RootTask<DataType> pbt)
            : base(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 0, 4))
		{
            this.Root = pbt;
			this.Load += OnLoad;
		}

        private void OnLoad(object sender, EventArgs e)
		{
            MakeCurrent();
            MouseUp += (s, ev) => { try { MakeCurrent(); } catch (GraphicsContextException) { } }; // workaround for correct context switching (mouseclicks might change the gui directly)
			glGui = new GLGui(this);

            if (monospaceFont == null)
            {
                monospaceFont = new GLFont(new Font("Lucida Console", 6.5f));
                monospaceFont.Options.Monospacing = GLFontMonospacing.Yes;
            }

            var verticalSplitter = glGui.Add(new GLSplitLayout(glGui)
            {
                Size = ClientSize,
                SplitterPosition = 0.7f,
                Orientation = GLSplitterOrientation.Vertical,
                Anchor = GLAnchorStyles.All
            });

            TreeContainer = verticalSplitter.Add(new PBTTreeContainer(glGui));
            TreeContainer.Add(new PBTTaskTreeControl<DataType>(glGui, Root.Root, true));

            var sidebar = verticalSplitter.Add(new GLFlowLayout(glGui) { FlowDirection = GLFlowDirection.TopDown });
            var sidebarSkin = sidebar.Skin;
            sidebarSkin.BackgroundColor = System.Drawing.Color.FromArgb(48, 48, 48);
            sidebar.Skin = sidebarSkin;

            var extended = sidebar.Add(new GLCheckBox(glGui) { Text = "extended view", Checked = true, AutoSize = true });
            extended.Changed += (s, ev) =>
            {
                MakeCurrent();
                TreeContainer.Clear();
                TreeContainer.Add(new PBTTaskTreeControl<DataType>(glGui, Root.Root, extended.Checked));
            };

            var filter = sidebar.Add(new GLButton(glGui) { Text = "Impulse Filter", AutoSize = true });

            var horizontalSplitter = sidebar.Add(new GLSplitLayout(glGui)
            {
                Size = new Size(sidebar.InnerWidth, sidebar.InnerHeight - extended.Outer.Bottom),
                SplitterPosition = 0.5f,
                Orientation = GLSplitterOrientation.Horizontal,
                Anchor = GLAnchorStyles.All
            });

            dataControl = horizontalSplitter.Add(new GLDataControl(glGui));
            dataControl.SetData(Root.Context.Data);

            var impulseLogScroll = horizontalSplitter.Add(new GLScrollableControl(glGui));
            impulseLog = impulseLogScroll.Add(new GLLabel(glGui) { Multiline = true, AutoSize = true });
            var impulseLogSkin = impulseLog.SkinEnabled;
            impulseLogSkin.Font = monospaceFont;
            impulseLog.SkinEnabled = impulseLogSkin;

            impulseLogger = new PBTImpulseLogger<DataType, ImpulseType>(new LabelWriter(impulseLog), Root.Context);
            HandleDestroyed += (s, ev) => impulseLogger.Dispose();
            filter.Click += (s, ev) => glGui.Add(new PBTImpulseFilterForm<DataType, ImpulseType>(glGui, impulseLogger));

            Resize += (s, ev) => { MakeCurrent(); GL.Viewport(ClientSize); };
            Paint += OnRender;
            //Application.Idle += (s, ev) => Invalidate();
            Timer t = new Timer();
            t.Interval = 16;
            t.Tick += (s, ev) => Invalidate();
            t.Start();
		}

        private void OnRender(object sender, PaintEventArgs e)
		{
            try
            {
                MakeCurrent();
                glGui.Render();
                SwapBuffers();
                Context.MakeCurrent(null);
            }
            catch(GraphicsContextException)
            {
            }
		}

        /// <summary>
        /// Allow to handle some input keys that are needed by GLGUI.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override bool IsInputKey(Keys key)
        {
            switch (key)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Left:
                case Keys.Tab:
                    return true;
            }
            return base.IsInputKey(key);
        }
	}
}

