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
        Collision,
        /// <summary>
        /// The leave impulse.
        /// Gets fired when the actor leaves the visible area of the simulation.
        /// The impulse data variable contains the size of the area.
        /// </summary>
        Leave
    }

    /// <summary>
    /// Our actor class. Controlled by a pbt.
    /// </summary>
    public class Actor
    {
        /// <summary>
        /// The simulation that this actor is part of.
        /// </summary>
        public Simulation Simulation;

        /// <summary>
        /// The position of the actor.
        /// </summary>
        public float X, Y;

        /// <summary>
        /// The view angle of the actor
        /// </summary>
        public float Angle;

        /// <summary>
        /// The size of the actor.
        /// </summary>
        public float Size;

        /// <summary>
        /// The color of the brush of the actor.
        /// </summary>
        public readonly Color BrushColor;

        /// <summary>
        /// The color brush of the actor.
        /// </summary>
        public Brush Brush;

        /// <summary>
        /// The view line pen of the actor.
        /// </summary>
        public Pen Pen;

        /// <summary>
        /// The pbt that controls the actor.
        /// </summary>
        public PBT.RootTask<Actor> AI;

        private Point initialPosition;

        /// <summary>
        /// Initializes the actor and loads its AI pbt.
        /// </summary>
        /// <param name="simulation">The simulation that the constructed actor is part of.</param>
        /// <param name="position">The initial position.</param>
        public Actor(Simulation simulation, Point position)
        {
            initialPosition = position;

            Simulation = simulation;
            X = position.X;
            Y = position.Y;
            Angle = (float)(Simulation.Random.NextDouble() * 2.0 * Math.PI);
            Size = 4;
            BrushColor = Color.FromArgb(position.X % 255, (128 + position.X) % 255, Math.Max(0, 255 - position.Y));
            Brush = new SolidBrush(BrushColor);
            Pen = Pens.LightYellow;
            AI = PBT.Parser.Load<Actor, ActorImpulses>(".", "AI", this, PBTConfig.Usings, PBTConfig.Logger);
        }

        /// <summary>
        /// Resets the actor.
        /// </summary>
        public void Reset()
        {
            X = initialPosition.X;
            Y = initialPosition.Y;
            Angle = (float)(Simulation.Random.NextDouble() * 2.0 * Math.PI);
            if (!AI.Finished)
                AI.SafeEnd();
            AI.Context.Variables.Clear();
            if(AI.CheckConditions())
                AI.SafeStart();
        }

        /// <summary>
        /// Gets executed each frame.
        /// Updates AI pbt and draws itself.
        /// </summary>
        /// <param name="g">GDI+ Graphics context to draw to.</param>
        /// <param name="time">The current simulation/game time.</param>
        public void Update(Graphics g, double time)
        {
            // execute ai ten times faster:
            //for(int i = 0; i < 10; i++)
                AI.Update(time);

            g.FillEllipse(Brush, X - Size, Y - Size, 2 * Size, 2 * Size);
            g.DrawLine(Pen, X, Y,
                X + (float)Math.Cos(Angle) * Size * 1.5f,
                Y + (float)Math.Sin(Angle) * Size * 1.5f);

            // draw connection to a possible friend:
            /*Actor friend = AI.Context.Variables["friend"];
            if (friend != null)
                g.DrawLine(Pens.DarkGray, X, Y, friend.X, friend.Y);*/
        }
    }
}
