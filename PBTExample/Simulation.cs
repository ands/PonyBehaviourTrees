using System;
using System.Drawing;
using System.Windows.Forms;

namespace PBTExample
{
    /// <summary>
    /// Our simulation class. It contains some actors that should all act independently.
    /// </summary>
    public class Simulation
    {
        /// <summary>
        /// Holds our independant actors.
        /// </summary>
        public Actor[] actors = new Actor[64];

        private DateTime start;

        /// <summary>
        /// Instantiates our actors.
        /// </summary>
        /// <param name="control"></param>
        public Simulation(Control control)
        {
            for (int i = 0; i < actors.Length; i++)
                actors[i] = new Actor(new Point((i / 8) * (control.ClientSize.Width / 8), (i % 8) * (control.ClientSize.Height / 8)));

            start = DateTime.Now;
        }

        /// <summary>
        /// Gets executed each frame.
        /// Sends collision events and updates actors.
        /// </summary>
        /// <param name="g">GDI+ Graphics context to draw to.</param>
        public void Update(Graphics g)
        {
            double time = (DateTime.Now - start).TotalSeconds;

            for (int i = 0; i < actors.Length; i++)
            {
                // send collision impulses to colliding actors
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

                // let all actors do their thing
                actors[i].Update(g, time);
            }
        }
    }
}
