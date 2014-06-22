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
        /// A source for random numbers.
        /// </summary>
        public readonly Random Random = new Random();

        /// <summary>
        /// The size of the simulated world.
        /// </summary>
        public readonly Size WorldSize;

        /// <summary>
        /// The independant actors in our simulation.
        /// </summary>
        public Actor[] Actors = new Actor[256];

        private DateTime start;

        /// <summary>
        /// Instantiates our actors.
        /// </summary>
        /// <param name="control"></param>
        public Simulation(Control control)
        {
            WorldSize = control.ClientSize;

            for (int i = 0; i < Actors.Length; i++)
                Actors[i] = new Actor(this, new Point((i / 16) * (control.ClientSize.Width / 16), (i % 16) * (control.ClientSize.Height / 16)));

            start = DateTime.Now;
        }

        /// <summary>
        /// Resets the simulation.
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < Actors.Length; i++)
                Actors[i].Reset();
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

            for (int i = 0; i < Actors.Length; i++)
            {
                // send collision impulses to colliding actors
                for (int j = i + 1; j < Actors.Length; j++)
                {
                    float dx = Actors[i].X - Actors[j].X;
                    float dy = Actors[i].Y - Actors[j].Y;
                    float sij = Actors[i].Size + Actors[j].Size;
                    if(dx * dx + dy * dy <= sij * sij)
                    {
                        Actors[i].AI.Context.OnImpulse(ActorImpulses.Collision, this, Actors[j]);
                        Actors[j].AI.Context.OnImpulse(ActorImpulses.Collision, this, Actors[i]);
                    }
                }

                // let all actors do their thing
                Actors[i].Update(g, time);
            }
        }
    }
}
