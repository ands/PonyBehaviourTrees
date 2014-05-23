using GLGUI;
using System.Drawing;
using System;

namespace PBTEditor
{
    class PBTDescriptionForm : GLForm
    {
        private Data.Task task;
        private GLLabel descriptionLabel;

        public PBTDescriptionForm(GLGui gui, GLLabel descriptionLabel, Data.Task task)
            : base(gui)
        {
            this.task = task;
            this.descriptionLabel = descriptionLabel;

            Title = task.TaskType.Name + " Description";
            SizeMin = Size = new Size(200, 100);

            var descriptionBackup = task.Description;

            var text = Add(new GLTextBox(gui)
            { 
                Text = task.Description ?? "", 
                Location = new Point(4, 4), 
                AutoSize = false,
                Multiline = true,
                WordWrap = true,
                Size = new Size(InnerWidth - 8, InnerHeight - 27),
                Anchor = GLAnchorStyles.All
            });
            text.Changed += (s, e) => descriptionLabel.Text = task.Description = text.Text;

            var ok = Add(new GLButton(gui)
            { 
                Text = "OK", 
                Location = new Point(4, InnerHeight - 18), 
                Anchor = GLAnchorStyles.Bottom | GLAnchorStyles.Left 
            });
            ok.Click += (s, e) => { if (text.Text == "") task.Description = null; Parent.Remove(this); };

            var abort = Add(new GLButton(gui)
            { 
                Text = "Abort", 
                Location = new Point(InnerWidth - 79, InnerHeight - 19), 
                Anchor = GLAnchorStyles.Bottom | GLAnchorStyles.Right 
            });
            abort.Click += (s, e) => { descriptionLabel.Text = (task.Description = descriptionBackup) ?? ""; Parent.Remove(this); };
        }
    }
}
