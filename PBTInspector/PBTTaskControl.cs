using System.Drawing;
using GLGUI;
using System.Reflection;
using PBT;

namespace PBTInspector
{
    class PBTTaskControl<DataType> : GLFlowLayout
    {
        public readonly PBT.Task<DataType> Task;

        private GLLabel title;
        private GLSkin.GLFlowLayoutSkin skin;

        private /*static*/ GLFont monospaceFont;

        public PBTTaskControl(GLGui gui, PBT.Task<DataType> task, bool extended)
            : base(gui)
        {
            Task = task;

            Render += OnRender;

            AutoSize = true;
            FlowDirection = GLFlowDirection.TopDown;
            skin = Skin;
            skin.Border = new GLPadding(1);
            skin.BorderColor = Color.FromArgb(32, 32, 32);
            skin.BackgroundColor = Color.FromArgb(48, 48, 48);
            Skin = skin;

            if (monospaceFont == null)
            {
                monospaceFont = new GLFont(new Font("Lucida Console", 6.5f));
                monospaceFont.Options.Monospacing = GLFontMonospacing.Yes;
            }

            // title
            title = Add(new GLLabel(Gui) { AutoSize = true, Text = Task.GetType().Name.TrimEnd('1', '2', '`') });
            var titleSkin = title.SkinEnabled;
            titleSkin.TextAlign = GLFontAlignment.Centre;
            titleSkin.Color = Color.FromArgb(255, 255, 255);
            title.SkinEnabled = titleSkin;
            title.AutoSize = false;

            if (extended)
            {
                // content (quick and dirty)
                string contentString = "";
                var fields = task.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (var f in fields)
                {
                    var v = f.GetValue(task);
                    if (v == null)
                        continue;
                    var ft = f.FieldType.Name.TrimEnd('1', '2', '`') + " " + f.Name + ":\n ";
                    if (v is string && f.Name == "impulse")
                    {
                        contentString += ft + v.ToString() + "\n\n";
                    }
                    else if (v is Expression || v.GetType().Name.Contains("Expression")) // i'm lazy right now :/
                    {
                        var codeField = v.GetType().GetField("code", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (codeField == null)
                        { // must be ResultExpression
                            contentString += ft + v.ToString() + "\n\n";
                        }
                        else
                        {
                            var code = codeField.GetValue(v);
                            if (code != null)
                                contentString += ft + code.ToString().Replace("\n", "\n ") + "\n\n";
                            else
                                contentString += ft + "null\n\n";
                        }
                    }
                }
                contentString = contentString.TrimEnd('\n', '\t');
                if (contentString.Length > 0)
                {
                    var content = Add(new GLLabel(Gui) { AutoSize = true, Text = contentString, Multiline = true, WordWrap = false });
                    var contentSkin = content.SkinEnabled;
                    contentSkin.Font = monospaceFont;
                    content.SkinEnabled = contentSkin;
                }
            }
        }

        private void OnRender(object sender, double delta)
        {
            var c = Task.Started ?
                (Task.Finished ? Color.FromArgb(98, 98, 48) : Color.FromArgb(48, 96, 48)) :
                (Task.Finished ? Color.FromArgb(96, 48, 48) : Color.FromArgb(48, 48, 48));
            if (skin.BackgroundColor != c)
            {
                skin.BackgroundColor = c;
                Skin = skin;
            }
        }
    }
}
