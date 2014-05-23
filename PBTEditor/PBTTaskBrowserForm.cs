using GLGUI;
using System.Drawing;
using System;
using System.Linq;

namespace PBTEditor
{
    class PBTTaskBrowserForm : GLForm
    {
        private Action<Data.Task> callback;

        public PBTTaskBrowserForm(GLGui gui, PBTEditorControl editor, Action<Data.Task> callback, bool leafTasksSelectable, bool allowAborting = true)
            : base(gui)
        {
            this.callback = callback;

            Title = "Task Browser";
            SizeMin = Size = new Size(400, 300);

            var horizontalFlow = Add(new GLFlowLayout(gui)
            { 
                FlowDirection = GLFlowDirection.LeftToRight,
                AutoSize = false,
                Size = new Size(InnerWidth, InnerHeight - 23),
                Anchor = GLAnchorStyles.All
            });

            var categories = editor.TaskTypes.TaskTypeCategories.ToDictionary(c => c.Name);

            var parentTasksFlow = horizontalFlow.Add(new GLFlowLayout(gui)
            {
                FlowDirection = GLFlowDirection.TopDown,
                AutoSize = true
            });
            parentTasksFlow.Add(new GLLabel(gui) { Text = "ParentTasks", AutoSize = true });
            foreach (var taskType in categories["ParentTasks"].TaskTypes)
            {
                var ltt = taskType;
                parentTasksFlow.Add(new GLLinkLabel(gui) { Text = ltt.Name, AutoSize = true }).Click += (s, e) => callback(ltt.Create());
            }

            var decoratorsFlow = horizontalFlow.Add(new GLFlowLayout(gui)
            {
                FlowDirection = GLFlowDirection.TopDown,
                AutoSize = true
            });
            decoratorsFlow.Add(new GLLabel(gui) { Text = "Decorators", AutoSize = true });
            foreach (var taskType in categories["Decorators"].TaskTypes)
            {
                var ltt = taskType;
                decoratorsFlow.Add(new GLLinkLabel(gui) { Text = ltt.Name, AutoSize = true }).Click += (s, e) => callback(ltt.Create());
            }

            if (leafTasksSelectable)
            {
                var leafTasksFlow = horizontalFlow.Add(new GLFlowLayout(gui)
                {
                    FlowDirection = GLFlowDirection.TopDown,
                    AutoSize = true
                });
                leafTasksFlow.Add(new GLLabel(gui) { Text = "LeafTasks", AutoSize = true });
                foreach (var taskType in categories["LeafTasks"].TaskTypes)
                {
                    var ltt = taskType;
                    leafTasksFlow.Add(new GLLinkLabel(gui) { Text = ltt.Name, AutoSize = true }).Click += (s, e) => callback(ltt.Create());
                }
            }

            if (editor.Clipboard != null && (leafTasksSelectable || editor.Clipboard.TaskType.Category.Name != "LeafTasks"))
            {
                var paste = Add(new GLButton(gui)
                {
                    Text = "Paste",
                    Location = new Point(4, InnerHeight - 18),
                    Anchor = GLAnchorStyles.Bottom | GLAnchorStyles.Left
                });
                paste.Click += (s, e) => callback(editor.Clipboard.DeepCopy());
            }

            if (allowAborting)
            {
                var abort = Add(new GLButton(gui)
                {
                    Text = "Abort",
                    Location = new Point(InnerWidth - 79, InnerHeight - 19),
                    Anchor = GLAnchorStyles.Bottom | GLAnchorStyles.Right
                });
                abort.Click += (s, e) => Parent.Remove(this);
            }
        }
    }
}
