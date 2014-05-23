<PBT.Decorators.Forever>
  <PBT.ParentTasks.Prioritize>
    <PBT.Decorators.OnImpulse impulse="Collision" sourceVariable="" eventVariable="other">
      <PBT.LeafTasks.Action action="float dx = vars[&quot;other&quot;].X - data.X;&#xA;float dy = vars[&quot;other&quot;].Y - data.Y;&#xA;data.X -= 0.01f * dx;&#xA;data.Y -= 0.01f * dy;&#xA;if(data.Size &gt; 14)&#xA;	data.Size -= 1.0f;" />
    </PBT.Decorators.OnImpulse>
    <PBT.LeafTasks.Action description="Execute movement" action="data.X += (float)(0.5 - random.NextDouble()*1.0);&#xA;data.Y += (float)(0.5 - random.NextDouble()*1.0);&#xA;data.X = Math.Max(data.Size, data.X);&#xA;data.X = Math.Min(640 - data.Size, data.X);&#xA;data.Y = Math.Max(data.Size, data.Y);&#xA;data.Y = Math.Min(480 - data.Size, data.Y);&#xA;&#xA;if(data.Size &lt; 64)&#xA;	data.Size += 0.1f;" />
  </PBT.ParentTasks.Prioritize>
</PBT.Decorators.Forever>