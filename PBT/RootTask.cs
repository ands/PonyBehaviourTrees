using System;
using System.Collections.Generic;
using System.IO;

namespace PBT
{
    /// <summary>
    /// The root of a pbt.
    /// </summary>
    /// <typeparam name="DataType"></typeparam>
	public abstract class RootTask<DataType> : LeafTask<DataType>
	{
        /// <summary>
        /// The root of the containing pbt.
        /// </summary>
        public Task<DataType> Root { get { return rootTask; } }

        private string name;
        private Task<DataType> rootTask;
        private static Dictionary<string, List<RootTask<DataType>>> active = new Dictionary<string, List<RootTask<DataType>>>(StringComparer.Ordinal);

        /// <summary>
        /// The constructor executed by the parser.
        /// </summary>
        /// <param name="context">The task context.</param>
        /// <param name="name">The name of the pbt file (without extension) in the current base path.</param>
        public RootTask(TaskContext<DataType> context, Expression<string> name)
            : base(context)
		{
            this.name = name;

            string path = Path.Combine(context.BaseDirectory, this.name + ".pbt");
            rootTask = Parser.Parse(path, this.name, context);

            this.name = this.name.TrimStart('.', '/'); // remove "./" from the beginning if it exists
            if (!active.ContainsKey(this.name))
                active.Add(this.name, new List<RootTask<DataType>>());
            active[this.name].Add(this);
		}

        /// <summary>
        /// The destructor.
        /// </summary>
        ~RootTask()
        {
            active[name].Remove(this);
        }

        /// <summary>
        /// Will be executed frequently to check whether this task/subtree is ready to run.
        /// </summary>
        /// <returns>Returns true if the task/subtree is ready and can be started; otherwise, false.</returns>
        public override bool CheckConditions()
        {
            lock (rootTask)
            {
                return rootTask.CheckConditions();
            }
        }

        /// <summary>
        /// Will be executed when the task is started.
        /// </summary>
        public override void Start()
        {
            lock (rootTask)
            {
                rootTask.SafeStart();
            }
        }

        /// <summary>
        /// Will be executed when the task is stopped or has finished.
        /// </summary>
        public override void End()
        {
            lock (rootTask)
            {
                rootTask.SafeEnd();
            }
        }

        /// <summary>
        /// Will be executed each frame while the task is running.
        /// </summary>
        public override void DoAction()
        {
            lock (rootTask)
            {
                if (rootTask.Finished)
                    Finish();
                else
                    rootTask.DoAction();
            }
        }

        /// <summary>
        /// Has to be executed frequently. Executes the pbt.
        /// </summary>
        /// <param name="timeSeconds">The current game time in seconds.</param>
        public void Update(double timeSeconds)
        {
            lock (rootTask)
            {
                Context.Time = timeSeconds;
                if (!Started)
                {
                    if (CheckConditions())
                        SafeStart();
                }
                else if (Finished)
                    End();
                else
                    DoAction();
            }
        }

        private bool ReplacePBT(byte[] pbt)
        {
            try
            {
                lock (rootTask)
                {
                    if (Started && !Finished)
                    {
                        SafeEnd();
                        Finish();
                    }
                    rootTask = Parser.Parse(pbt, this.name + ".pbt", Context);
                }
                return true;
            }
            catch (Exception e)
            {
                Context.Log.Error("Error while loading {0}:\n{1}", this.name + ".pbt", e);
                return false;
            }
        }

        /// <summary>
        /// Replaces any loaded pbt that has the specified name with the specified new pbt.
        /// </summary>
        /// <param name="name">The pbt name.</param>
        /// <param name="pbt">The pbt source.</param>
        /// <returns>Returns true on success; otherwise, false.</returns>
        public static bool ReplacePBT(string name, byte[] pbt)
        {
            if (active.ContainsKey(name))
                foreach (var reference in active[name])
                    if (!reference.ReplacePBT(pbt))
                        return false;
            GC.Collect();
            return true;
        }
	}
}

