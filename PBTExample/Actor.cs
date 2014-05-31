using System;
using System.Drawing;

namespace PBTExample
{
    /// <summary>
    /// The list of possible impulses that we can handle in the pbt.
    /// </summary>
    public enum ActorImpulses
    {
        /// <summary>
        /// The collision impulse.
        /// Gets fired when the actor collides with another actor.
        /// The impulse data variable contains the other actor.
        /// </summary>
        Collision
    }

    /// <summary>
    /// Our actor class. Controlled by a pbt.
    /// </summary>
    public class Actor
    {
        /// <summary>
        /// The position of the actor.
        /// </summary>
        public float X, Y;

        /// <summary>
        /// The size of the actor.
        /// </summary>
        public float Size;

        /// <summary>
        /// The color brush of the actor.
        /// </summary>
        public Brush Brush;

        /// <summary>
        /// The pbt that controls the actor.
        /// </summary>
        public PBT.RootTask<Actor> AI;

        /// <summary>
        /// Initializes the actor and loads its AI pbt.
        /// </summary>
        /// <param name="position">The initial position.</param>
        public Actor(Point position)
        {
            X = position.X;
            Y = position.Y;
            Size = 20;
            Brush = new SolidBrush(Color.FromArgb(position.X % 255, (128 + position.X) % 255, Math.Max(0, 255 - position.Y)));
            AI = PBT.Parser.Load<Actor, ActorImpulses>(".", "AI", this, PBTConfig.Usings, PBTConfig.Logger);
        }

        /// <summary>
        /// Gets executed each frame.
        /// Updates AI pbt and draws itself.
        /// </summary>
        /// <param name="g">GDI+ Graphics context to draw to.</param>
        /// <param name="time">The current simulation/game time.</param>
        public void Update(Graphics g, double time)
        {
            AI.Update(time);
            g.FillEllipse(Brush, X - Size, Y - Size, 2 * Size, 2 * Size);
        }
    }
}
