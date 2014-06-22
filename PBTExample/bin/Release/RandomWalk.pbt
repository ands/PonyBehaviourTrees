<PBT.ParentTasks.ParallelAnd description="turn and move in parallel">
  <PBT.ParentTasks.Sequence description="turn">
    <PBTExample.Turn description="random walk rotation" amount="return 6.28f * (float)(random&#xA;	.NextDouble() - 0.5);" />
    <PBT.Decorators.If description="biased rotation towards friend" condition="return vars[&quot;friend&quot;] != null">
      <PBT.Decorators.Repeat description="try multiple times to randomly rotate towards friend" iterations="3">
        <PBT.Decorators.If description="if we don't look towards our friend..." condition="var dx = vars[&quot;friend&quot;].X - data.X;&#xA;var dy = vars[&quot;friend&quot;].Y - data.Y;&#xA;var l = Math.Sqrt(dx*dx + dy*dy);&#xA;dx -= Math.Cos(data.Angle);&#xA;dy -= Math.Sin(data.Angle);&#xA;var l2 = Math.Sqrt(dx*dx + dy*dy);&#xA;&#xA;return l2 &gt; (l - 0.5f);">
          <PBTExample.Turn description="...rotate randomly and hope that we will be looking towards our friend" amount="return 6.28f * (float)(random&#xA;	.NextDouble() - 0.5);" />
        </PBT.Decorators.If>
      </PBT.Decorators.Repeat>
    </PBT.Decorators.If>
  </PBT.ParentTasks.Sequence>
  <PBTExample.Move amount="32" />
</PBT.ParentTasks.ParallelAnd>