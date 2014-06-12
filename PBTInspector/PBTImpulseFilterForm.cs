using GLGUI;
using System.Drawing;
using System;
using System.Linq;

namespace PBTInspector
{
    class PBTImpulseFilterForm<DataType, ImpulseType> : GLForm
    {
        public PBTImpulseFilterForm(GLGui gui, PBTImpulseLogger<DataType, ImpulseType> logger)
            : base(gui)
        {
            Title = "Impulse Filter";
            var impulseNames = Enum.GetNames(typeof(ImpulseType));
            var impulseValues = (ImpulseType[])Enum.GetValues(typeof(ImpulseType));
            SizeMin = Size = new Size(150, 40 + 15 * impulseNames.Length);

            var flow = Add(new GLFlowLayout(Gui)
            {
                Location = new Point(4, 4),
                Size = new Size(InnerWidth - 8, InnerHeight - 27),
                AutoSize = false,
                Anchor = GLAnchorStyles.All,
                FlowDirection = GLFlowDirection.TopDown
            });

            for (int j = 0; j < impulseNames.Length; j++)
            {
                int i = j;
                var cb = flow.Add(new GLCheckBox(Gui) { Text = impulseNames[j], AutoSize = true, Checked = logger.Enabled.Contains(impulseValues[j]) });
                cb.Changed += (s, ev) => { if (cb.Checked) logger.Enabled.Add(impulseValues[i]); else logger.Enabled.Remove(impulseValues[i]); };
            }

            var ok = Add(new GLButton(gui)
            { 
                Text = "OK", 
                Location = new Point(4, InnerHeight - 18), 
                Anchor = GLAnchorStyles.Bottom | GLAnchorStyles.Left 
            });
            ok.Click += (s, e) => { Parent.Remove(this); };
        }
    }
}
