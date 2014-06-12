using GLGUI;
using System.Drawing;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PBTEditor
{
    class PBTTaskControl : GLFlowLayout
    {
        public readonly PBTEditorControl Editor;
        public readonly PBTTaskTreeControl TaskTreeControl;
        public readonly Data.Task Task;

        private GLLabel title;
        private GLLabel description;
        private GLLabel[] parameterTitles;
        private GLControl[] parameterValues;
        private int minInnerWidth;
        private GLButton addAbove;
        private List<GLButton> addBelow = new List<GLButton>();
        private GLFlowLayout addBelowFlow;

        private static GLFont monospaceFont, addButtonFont;
        private static GLSkin.GLTextBoxSkin codeBoxActive, codeBoxDisabled, codeBoxEnabled, codeBoxHover;
        private static GLSkin.GLButtonSkin addButtonPressed, addButtonDisabled, addButtonEnabled, addButtonHover;

        public PBTTaskControl(GLGui gui, PBTEditorControl editor, PBTTaskTreeControl taskTreeControl, Data.Task task)
            : base(gui)
        {
            Editor = editor;
            TaskTreeControl = taskTreeControl;
            Task = task;

            AutoSize = true;
            FlowDirection = GLFlowDirection.TopDown;
            var skin = Skin;
            skin.Border = new GLPadding(1);
            skin.BorderColor = Color.FromArgb(32, 32, 32);
            skin.BackgroundColor = Color.FromArgb(48, 48, 48);
            Skin = skin;

            LoadCommon();

            AddContextMenu();
            AddContent();
        }

        private void AddContextMenu()
        {
            ContextMenu = new GLContextMenu(Gui);
            ContextMenu.Add(new GLContextMenuEntry(Gui) { Text = "Edit Description" }).Click += (s, e) =>
            {
                var c = ContextMenu.Location;
                var p = Editor.TreeContainer.ScrollPosition;
                Editor.TreeContainer.Add(new PBTDescriptionForm(Gui, description, Task)
                {
                    Location = new Point(p.X + c.X, p.Y + c.Y)
                });
            };
            ContextMenu.Add(new GLContextMenuEntry(Gui) { Text = "Copy" })
                .Click += (s, e) => Editor.Clipboard = new Data.Task(Task.TaskType, Task.Description, Task.ParameterValues.ToArray());
            ContextMenu.Add(new GLContextMenuEntry(Gui) { Text = "Cut" })
                .Click += (s, e) => { Editor.Clipboard = new Data.Task(Task.TaskType, Task.Description, Task.ParameterValues.ToArray()); Delete(false); };
            ContextMenu.Add(new GLContextMenuEntry(Gui) { Text = "Delete" })
                .Click += (s, e) => Delete(false);
            ContextMenu.Add(new GLContextMenuEntry(Gui) { Text = "Copy Subtree" })
                .Click += (s, e) => Editor.Clipboard = Task.DeepCopy();
            ContextMenu.Add(new GLContextMenuEntry(Gui) { Text = "Cut Subtree" })
                .Click += (s, e) => { Editor.Clipboard = Task.DeepCopy(); Delete(true); };
            ContextMenu.Add(new GLContextMenuEntry(Gui) { Text = "Delete Subtree" })
                .Click += (s, e) => Delete(true);
        }

        private void Delete(bool deep)
        {
            var parentTaskTreeControl = TaskTreeControl.ParentTaskTreeControl;
            if(parentTaskTreeControl != null)
            {
                int parentPosition = parentTaskTreeControl.Subtrees.IndexOf(TaskTreeControl);
                parentTaskTreeControl.TaskControl.Task.Subtasks.Remove(Task);
                if (!deep)
                {
                    if (parentTaskTreeControl.TaskControl.Task.TaskType.Category.Name == "Decorators")
                    {
                        if (TaskTreeControl.Subtrees.Count == 1)
                            parentTaskTreeControl.TaskControl.Task.Subtasks.Insert(0, TaskTreeControl.Subtrees[0].TaskControl.Task);
                    }
                    else
                        parentTaskTreeControl.TaskControl.Task.Subtasks.InsertRange(parentPosition, TaskTreeControl.TaskControl.Task.Subtasks);
                }
            }
            else
            {
                Editor.RootTask = null;
                if (!deep && TaskTreeControl.Subtrees.Count == 1)
                        Editor.RootTask = TaskTreeControl.Subtrees[0].TaskControl.Task;
            }

            Editor.UpdatePBT();
        }

        private void InsertAbove(Data.Task task)
        {
            var parentTaskTreeControl = TaskTreeControl.ParentTaskTreeControl;
            if (parentTaskTreeControl != null)
            {
                int position = parentTaskTreeControl.Subtrees.IndexOf(TaskTreeControl);
                parentTaskTreeControl.TaskControl.Task.Subtasks[position] = task;
            }
            else
            {
                Editor.RootTask = task;
            }
            task.Subtasks.Add(Task);
            Editor.UpdatePBT();
        }

        private void InsertBelow(int position, Data.Task task)
        {
            Task.Subtasks.Insert(position, task);
            Editor.UpdatePBT();
        }

        private GLButton AddTaskAddButton(GLControl parent, Action<Data.Task> callback, bool leafTasksSelectable = true)
        {
            var button = parent.Add(new GLButton(Gui) { Text = "###" });
            button.SkinDisabled = addButtonDisabled;
            button.SkinEnabled = addButtonEnabled;
            button.SkinHover = addButtonHover;
            button.SkinPressed = addButtonPressed;
            button.Click += (s, e) =>
            {
                var t = Editor.TreeContainer.InnerSize;
                var p = Editor.TreeContainer.ScrollPosition;
                Editor.TreeContainer.Add(new PBTTaskBrowserForm(Gui, Editor, callback, leafTasksSelectable)
                {
                    Location = new Point(p.X + t.Width / 2 - 200, p.Y + t.Height / 2 - 150)
                });
            };
            return button;
        }

        private void AddContent()
        {
            // ###above###
            addAbove = AddTaskAddButton(this, task => InsertAbove(task), false);

            // title
            title = Add(new GLLabel(Gui) { AutoSize = true, Text = Task.TaskType.Name });
            minInnerWidth = title.Width;
            var titleSkin = title.SkinEnabled;
            titleSkin.TextAlign = GLFontAlignment.Centre;
            titleSkin.Color = Color.FromArgb(255, 255, 255);
            title.SkinEnabled = titleSkin;
            title.AutoSize = false;

            // description
            description = Add(new GLLabel(Gui)
            {
                Text = Task.Description ?? "", 
                AutoSize = true,
                Multiline = true, 
                WordWrap = true,
                SizeMin = new Size(0, 0)
            });
            var descriptionSkin = description.SkinEnabled;
            descriptionSkin.Color = Color.FromArgb(255, 192, 96);
            description.SkinEnabled = descriptionSkin;

            // ###parameters###
            parameterTitles = new GLLabel[Task.TaskType.Parameters.Count];
            parameterValues = new GLControl[Task.TaskType.Parameters.Count];
            for (int i = 0; i < Task.TaskType.Parameters.Count; i++)
            {
                int localIndex = i;
                var parameter = Task.TaskType.Parameters[i];
                parameterTitles[i] = Add(new GLLabel(Gui) { Text = parameter.ShortType + " " + parameter.Name + ":", AutoSize = true });
                minInnerWidth = Math.Max(minInnerWidth, parameterTitles[i].Width);
                if (parameter.IsEnum)
                {
                    var parameterValue = Add(new GLButton(Gui)
                    {
                        AutoSize = true,
                        Text = Task.ParameterValues[i] ?? "",
                    });
                    parameterValue.Click += (s, e) =>
                    {
                        var t = Editor.TreeContainer.InnerSize;
                        var p = Editor.TreeContainer.ScrollPosition;
                        Editor.TreeContainer.Add(new PBTEnumForm(Gui, this, localIndex, parameterValue)
                        {
                            Location = new Point(p.X + t.Width / 2 - 200, p.Y + t.Height / 2 - 150)
                        });
                    };

                    /*var parameterValue = Add(new GLOptions(Gui)
                    {
                        AutoSize = true,
                        FlowDirection = GLFlowDirection.TopDown
                    });
                    for (int j = 0; j < parameter.EnumType.ValueNames.Length; j++)
                        parameterValue.Add(new GLCheckBox(Gui) { Text = parameter.EnumType.ValueNames[j], AutoSize = true });
                    parameterValue.Selection = (GLCheckBox)parameterValue.Controls.ElementAt(Array.IndexOf(parameter.EnumType.ValueNames, Task.ParameterValues[i]));
                    parameterValue.Changed += (s, e) => Task.ParameterValues[localIndex] = parameterValue.Selection.Text;*/
                    parameterValues[i] = parameterValue;
                }
                else
                {
                    var parameterValue = Add(new GLTextBox(Gui)
                    {
                        AutoSize = true,
                        Text = Task.ParameterValues[i] ?? "",
                        Multiline = true,
                        WordWrap = false,
                    });
                    parameterValue.SkinActive = codeBoxActive;
                    parameterValue.SkinDisabled = codeBoxDisabled;
                    parameterValue.SkinEnabled = codeBoxEnabled;
                    parameterValue.SkinHover = codeBoxHover;
                    parameterValue.Changed += (s, e) => Task.ParameterValues[localIndex] = parameterValue.Text;
                    parameterValues[i] = parameterValue;
                }
            }
            for (int i = 0; i < Task.TaskType.Parameters.Count; i++)
            {
                parameterValues[i].SizeMin = new Size(minInnerWidth, 14);
            }

            if (Task.TaskType.Name == "Reference")
            {
                var loadTask = Add(new GLButton(Gui) { Text = "Load" });
                var name = Task.ParameterValues[0];
                loadTask.Click += (s, e) => Editor.ShowPBT(name);
            }

            // ###below###
            addBelowFlow = Add(new GLFlowLayout(Gui) { FlowDirection = GLFlowDirection.LeftToRight, AutoSize = true });
            var addButtonsFlowSkin = addBelowFlow.Skin;
            addButtonsFlowSkin.Padding = new GLPadding(0);
            addBelowFlow.Skin = addButtonsFlowSkin;

            switch (Task.TaskType.Category.Name)
            {
                case "LeafTasks":
                    break;
                case "Decorators":
                    if (Task.Subtasks.Count == 0)
                        addBelow.Add(AddTaskAddButton(addBelowFlow, task => InsertBelow(0, task)));
                    break;
                case "ParentTasks":
                    for (int i = 0; i <= Task.Subtasks.Count; i++)
                    {
                        int li = i;
                        addBelow.Add(AddTaskAddButton(addBelowFlow, task => InsertBelow(li, task)));
                    }
                    break;
                default:
                    MessageBox.Show("Unknown Task Category: " + Task.TaskType.Category.Name, "Error during Task loading", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        protected override void UpdateLayout()
        {
            int width = (parameterValues != null && parameterValues.Length > 0 && parameterValues.All(pv => pv != null)) ?
                Math.Max(parameterValues.Max(pv => pv.Width), minInnerWidth) : minInnerWidth;
            if (addAbove != null)
                addAbove.Size = new Size(width, addAbove.SizeMin.Height);
            if (title != null)
                title.Size = new Size(width, title.Height);
            if (description != null)
                description.SizeMax = new Size(width, int.MaxValue);
            if (addBelow != null && addBelow.Count > 0)
            {
                int bw = (width - addBelowFlow.Skin.Space * Task.Subtasks.Count) / (Task.Subtasks.Count + 1);
                for (int i = 0; i < addBelow.Count - 1; i++)
                {
                    if (addBelow[i] != null)
                        addBelow[i].Size = new Size(bw, addBelow[i].SizeMin.Height);
                }
                bw = width - Task.Subtasks.Count * (bw + addBelowFlow.Skin.Space);
                if (addBelow[addBelow.Count - 1] != null)
                    addBelow[addBelow.Count - 1].Size = new Size(bw, addBelow[addBelow.Count - 1].SizeMin.Height);
            }
            base.UpdateLayout();
        }

        private void LoadCommon()
        {
            if (monospaceFont == null)
            {
                monospaceFont = new GLFont(new Font("Lucida Console", 7.0f));
                monospaceFont.Options.Monospacing = GLFontMonospacing.Yes;

                codeBoxActive = Gui.Skin.TextBoxActive;
                codeBoxDisabled = Gui.Skin.TextBoxDisabled;
                codeBoxEnabled = Gui.Skin.TextBoxEnabled;
                codeBoxHover = Gui.Skin.TextBoxHover;

                codeBoxActive.Font = monospaceFont;
                codeBoxDisabled.Font = monospaceFont;
                codeBoxEnabled.Font = monospaceFont;
                codeBoxHover.Font = monospaceFont;

                addButtonFont = new GLFont(new Font("Arial", 3.0f));

                addButtonPressed = Gui.Skin.ButtonPressed;
                addButtonDisabled = Gui.Skin.ButtonDisabled;
                addButtonEnabled = Gui.Skin.ButtonEnabled;
                addButtonHover = Gui.Skin.ButtonHover;

                addButtonPressed.Font = addButtonFont;
                addButtonDisabled.Font = addButtonFont;
                addButtonEnabled.Font = addButtonFont;
                addButtonHover.Font = addButtonFont;
            }
        }
    }
}
