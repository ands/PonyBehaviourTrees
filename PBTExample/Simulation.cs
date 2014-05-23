using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PBTExample
{
    public enum ActorImpulses
    {
        Collision
    }

    public class Actor
    {
        public float X, Y;
        public float Size;
        public Brush Brush;
        public PBT.LeafTasks.Reference<Actor> AI;

        public Actor(Point position)
        {
            X = position.X;
            Y = position.Y;
            Size = 20;
            Brush = new SolidBrush(Color.FromArgb(position.X % 255, (128 + position.X) % 255, Math.Max(0, 255 - position.Y)));
            AI = PBT.Parser.Load<Actor, ActorImpulses>(".", "AI", this, PBTConfig.Usings, PBTConfig.Logger);
        }

        public void Draw(Graphics g, double time)
        {
            AI.Update(time);
            g.FillEllipse(Brush, X - Size, Y - Size, 2 * Size, 2 * Size);
        }
    }

    public class Simulation
    {
        public Actor[] actors = new Actor[64];

        private DateTime start;

        public Simulation(Control control)
        {
            for (int i = 0; i < actors.Length; i++)
                actors[i] = new Actor(new Point((i / 8) * (control.ClientSize.Width / 8), (i % 8) * (control.ClientSize.Height / 8)));

            start = DateTime.Now;
        }

        public void Draw(Graphics g)
        {
            double time = (DateTime.Now - start).TotalSeconds;

            for (int i = 0; i < actors.Length; i++)
            {
                for (int j = i + 1; j < actors.Length; j++)
                {
                    float dx = actors[i].X - actors[j].X;
                    float dy = actors[i].Y - actors[j].Y;
                    float sij = actors[i].Size + actors[j].Size;
                    if(dx * dx + dy * dy <= sij * sij)
                    {
                        actors[i].AI.Context.OnImpulse(ActorImpulses.Collision, this, actors[j]);
                        actors[j].AI.Context.OnImpulse(ActorImpulses.Collision, this, actors[i]);
                    }
                }

                actors[i].Draw(g, time);
            }
        }
    }
}
