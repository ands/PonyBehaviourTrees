using GLGUI;
using System.Drawing;
using System;
using System.Linq;

namespace PBTEditor
{
    class PBTEnumForm : GLForm
    {
        public PBTEnumForm(GLGui gui, PBTTaskControl taskControl, int parameterIndex, GLButton parameterControl)
            : base(gui)
        {
            var task = taskControl.Task;
            var parameterType = task.TaskType.Parameters[parameterIndex];
            var valueNames = parameterType.EnumType.ValueNames;
            Title = parameterType.ShortType + " " + parameterType.Name;
            SizeMin = Size = new Size(200, 100 + 15 * valueNames.Length);

            var valueBackup = task.ParameterValues[parameterIndex];

            var text = Add(new GLTextBox(gui)
            {
                Text = valueBackup,
                Location = new Point(4, 4),
                AutoSize = false,
                Multiline = true,
                WordWrap = true,
                Size = new Size(InnerWidth - 8, InnerHeight - 27 - valueNames.Length * 15),
                Anchor = GLAnchorStyles.All
            });
            text.Changed += (s, e) => task.ParameterValues[parameterIndex] = parameterControl.Text = text.Text;

            var parameterValue = Add(new GLOptions(Gui)
            {
                Location = new Point(4, text.Outer.Bottom + 4),
                Size = new Size(InnerWidth - 8, InnerHeight - 27 - text.Height),
                Anchor = GLAnchorStyles.Left | GLAnchorStyles.Bottom,
                AutoSize = false,
                FlowDirection = GLFlowDirection.TopDown
            });
            for (int j = 0; j < valueNames.Length; j++)
                parameterValue.Add(new GLCheckBox(Gui) { Text = valueNames[j], AutoSize = true });
            var selectionIndex = Array.IndexOf(valueNames, task.ParameterValues[parameterIndex]);
            if (selectionIndex != -1)
                parameterValue.Selection = (GLCheckBox)parameterValue.Controls.ElementAt(selectionIndex);
            parameterValue.Changed += (s, e) => task.ParameterValues[parameterIndex] = parameterControl.Text = parameterValue.Selection.Text;

            var ok = Add(new GLButton(gui)
            { 
                Text = "OK", 
                Location = new Point(4, InnerHeight - 18), 
                Anchor = GLAnchorStyles.Bottom | GLAnchorStyles.Left 
            });
            ok.Click += (s, e) => { Parent.Remove(this); };

            var abort = Add(new GLButton(gui)
            { 
                Text = "Abort", 
                Location = new Point(InnerWidth - 79, InnerHeight - 19), 
                Anchor = GLAnchorStyles.Bottom | GLAnchorStyles.Right 
            });
            abort.Click += (s, e) => { task.ParameterValues[parameterIndex] = parameterControl.Text = valueBackup; Parent.Remove(this); };
        }
    }
}
