using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using GLGUI;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace PBTEditor
{
	public class PBTEditorControl : OpenTK.GLControl
	{
        public GLGui GLGui { get; private set; }
        public GLScrollableControl TreeContainer { get; private set; }
        public Data.TaskTypes TaskTypes;
        public Data.Task RootTask;
        public Data.Task Clipboard;

        public delegate bool PBTUpdateHandler(string pbtName, byte[] pbtData);
        public event PBTUpdateHandler PBTUpdate;

        Type dataType, impulseType;
        string pbtSearchPath;
        GLFlowLayout fileList;
        GLButton reload, save, create;

        byte[] typesXML;
        string currentPBTName;

		public PBTEditorControl(Type dataType, Type impulseType, string pbtSearchPath) : base(new GraphicsMode(new ColorFormat(8, 8, 8, 8), 24, 0, 4))
		{
			this.Load += OnLoad;

            this.dataType = dataType;
            this.impulseType = impulseType;
            this.pbtSearchPath = pbtSearchPath;

            LoadPBTTypes();
		}

        public void UpdatePBTFileList()
        {
            MakeCurrent();
            fileList.Clear();

            var pbtFiles = Directory.GetFiles(pbtSearchPath, "*.pbt", SearchOption.AllDirectories);
            foreach (var file in pbtFiles)
            {
                int l = pbtSearchPath.Length + 1;
                var name = file.Replace('\\', '/').TrimStart('/');
                name = name.Substring(l, name.Length - l - 4);
                var fileLink = fileList.Add(new GLLinkLabel(GLGui) { Text = name, AutoSize = true });
                fileLink.Click += (s, e) => ShowPBT(name);
            }
        }

        private void LoadPBTTypes()
        {
            // ENCODE
            var stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            try
            {
                PBT.TypeExporter.ExportAllTaskTypes(dataType, impulseType, writer);
            }
            catch (ReflectionTypeLoadException re)
            {
                var sb = new StringBuilder();
                foreach (var type in re.LoaderExceptions)
                    sb.AppendLine(type.ToString());
                MessageBox.Show(string.Format("PBTEditorControl<{0}, {1}> : ReflectionTypeLoadException. Could not load the following types: {2}",
                    dataType, impulseType, sb.ToString()), "Error during type loading", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            writer.Close();

            typesXML = stream.ToArray();
            //string debug = Encoding.UTF8.GetString(typesXML);

            // DECODE
            stream = new MemoryStream(typesXML);
            var reader = new XmlTextReader(stream);
            TaskTypes = Data.PBT.GetTaskTypes(reader);
            foreach (var category in TaskTypes.TaskTypeCategories)
                category.TaskTypes.Sort((x, y) => x.Name.CompareTo(y.Name));
            reader.Close();
            stream.Close();
        }

        internal void ShowPBT(string name)
        {
            string path = Path.Combine(pbtSearchPath, name + ".pbt");
            RootTask = Data.PBT.Deserialize(path, TaskTypes);

            UpdatePBT();

            currentPBTName = name;
            Parent.Text = "PBT Editor - " + currentPBTName;
            reload.Enabled = true;
            save.Enabled = true;
        }

        internal void UpdatePBT()
        {
            TreeContainer.Clear();
            if (RootTask != null)
                TreeContainer.Add(new PBTTaskTreeControl(GLGui, this, null, RootTask));
            else
                TreeContainer.Add(new PBTTaskBrowserForm(GLGui, this, task => { RootTask = task; UpdatePBT(); }, true, false));
        }

        private void SavePBT()
        {
            try
            {
                var stream = new MemoryStream();
                var writer = new XmlTextWriter(stream, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                Data.PBT.Serialize(writer, RootTask);
                writer.Flush();
                stream.Flush();
                stream.Close();
                byte[] pbt = stream.ToArray();

                // handle the pbt update:
                if (PBTUpdate != null)
                    if (!PBTUpdate(currentPBTName, pbt))
                        return;

                // save the new pbt
                string path = Path.Combine(pbtSearchPath, currentPBTName + ".pbt");
                File.WriteAllBytes(path, pbt);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error during PBT saving", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreatePBT()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = Directory.GetCurrentDirectory();
            sfd.Filter = "PBT files (*.pbt)|*.pbt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(sfd.FileName))
                {
                    Data.PBT.Serialize(sfd.FileName,
                        TaskTypes.TaskTypeCategories
                            .Find(c => c.Name == "LeafTasks").TaskTypes
                            .Find(t => t.Name == "TODO").Create(
                                "This is a new PBT file.\n\n" + 
                                "You can add and paste Tasks\nby pressing \"...\" buttons.\n\n" +
                                "To delete, copy or cut Tasks,\nor whole subtrees,\nright click on a Task.\n\n" +
                                "You can also add descriptions\nto a Task using that\nsame context menu."));
                    UpdatePBTFileList();
                }
                else
                {
                    MessageBox.Show("File does already exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnLoad(object sender, EventArgs e)
		{
            MakeCurrent();
            MouseUp += (s, ev) => MakeCurrent(); // workaround for correct context switching (mouseclicks might change the gui directly)
			GLGui = new GLGui(this);
            
            var verticalSplitter = GLGui.Add(new GLSplitLayout(GLGui)
            {
                Size = ClientSize,
                SplitterPosition = 0.85f,
                Orientation = GLSplitterOrientation.Vertical,
                Anchor = GLAnchorStyles.All
            });

            TreeContainer = verticalSplitter.Add(new GLScrollableControl(GLGui) { Anchor = GLAnchorStyles.All });
            var pbtTreeControlSkin = TreeContainer.Skin;
            pbtTreeControlSkin.BackgroundColor = System.Drawing.Color.FromArgb(96, 96, 96);//glgui.Skin.FormActive.BackgroundColor;
			pbtTreeControlSkin.BorderColor = GLGui.Skin.FormActive.BorderColor;
            TreeContainer.Skin = pbtTreeControlSkin;

            var sidebarFlow = verticalSplitter.Add(new GLFlowLayout(GLGui) { FlowDirection = GLFlowDirection.TopDown });
            var sidebarSkin = sidebarFlow.Skin;
            sidebarSkin.BackgroundColor = GLGui.Skin.FormActive.BackgroundColor;
            sidebarFlow.Skin = sidebarSkin;

            reload = sidebarFlow.Add(new GLButton(GLGui) { Text = "Reload", Enabled = false });
            reload.Click += (s, ev) => ShowPBT(currentPBTName);
            save = sidebarFlow.Add(new GLButton(GLGui) { Text = "Save", Enabled = false });
            save.Click += (s, ev) => SavePBT();
            create = sidebarFlow.Add(new GLButton(GLGui) { Text = "New" });
            create.Click += (s, ev) => CreatePBT();

            var fileListTitle = sidebarFlow.Add(new GLLabel(GLGui) { Text = "Load:", AutoSize = true });

            var fileListScrollable = sidebarFlow.Add(new GLScrollableControl(GLGui)
            {
                Size = new Size(sidebarFlow.InnerWidth, sidebarFlow.InnerHeight - fileListTitle.Outer.Bottom),
                Anchor = GLAnchorStyles.All
            });
            fileList = fileListScrollable.Add(new GLFlowLayout(GLGui) { FlowDirection = GLFlowDirection.TopDown, AutoSize = true });
            UpdatePBTFileList();

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
            MakeCurrent();
			GLGui.Render();
			SwapBuffers();
		}

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

