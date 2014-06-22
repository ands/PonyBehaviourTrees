<PBT.Decorators.Forever>
  <PBT.ParentTasks.Prioritize>
    <PBT.Decorators.OnImpulse impulse="Leave" sourceVariable="" dataVariable="size">
      <PBT.LeafTasks.Action description="push back towards the center" action="data.X = 0.99f*data.X + 0.01f*&#xA;	(vars[&quot;size&quot;].Width/2.0f);&#xA;data.Y = 0.99f*data.Y + 0.01f*&#xA;	(vars[&quot;size&quot;].Height/2.0f);" />
    </PBT.Decorators.OnImpulse>
    <PBT.Decorators.OnImpulse impulse="Collision" sourceVariable="" dataVariable="other">
      <PBT.ParentTasks.Sequence>
        <PBT.LeafTasks.Action description="resolve collision" action="var o = vars[&quot;other&quot;];&#xA;var dx = o.X - data.X;&#xA;var dy = o.Y - data.Y;&#xA;data.X -= dx * 0.01f;&#xA;data.Y -= dy * 0.01f;" />
        <PBT.LeafTasks.Action description="make friends" action="if(vars[&quot;friend&quot;] == null)&#xA;{&#xA;	vars[&quot;friend&quot;] =&#xA;		vars[&quot;other&quot;];&#xA;	return;&#xA;}&#xA;&#xA;var c = data.BrushColor;&#xA;var oc = vars[&quot;other&quot;]&#xA;	.BrushColor;&#xA;var fc = vars[&quot;friend&quot;]&#xA;	.BrushColor;&#xA;&#xA;var odiversity =&#xA;	(oc.R-c.R)*(oc.R-c.R) +&#xA;	(oc.G-c.G)*(oc.G-c.G) +&#xA;	(oc.B-c.B)*(oc.B-c.B);&#xA;var fdiversity =&#xA;	(fc.R-c.R)*(fc.R-c.R) +&#xA;	(fc.G-c.G)*(fc.G-c.G) +&#xA;	(fc.B-c.B)*(fc.B-c.B);&#xA;&#xA;if(odiversity &lt; fdiversity)&#xA;	vars[&quot;friend&quot;] =&#xA;		vars[&quot;other&quot;];" />
      </PBT.ParentTasks.Sequence>
    </PBT.Decorators.OnImpulse>
    <PBT.LeafTasks.Reference name="RandomWalk" />
  </PBT.ParentTasks.Prioritize>
</PBT.Decorators.Forever>