using System;
using System.Collections.Generic;
using System.IO;

namespace PBT.LeafTasks
{
    public class Reference<DataType> : LeafTask<DataType>
	{
        public Task<DataType> RootTask { get { return rootTask; } }

        private string name;
        private Task<DataType> rootTask;
        private static Dictionary<string, List<Reference<DataType>>> active = new Dictionary<string, List<Reference<DataType>>>(StringComparer.Ordinal);

        public Reference(TaskContext<DataType> context, Expression<string> name)
            : base(context)
		{
            this.name = name;

            string path = Path.Combine(context.BaseDirectory, this.name + ".pbt");
            try
            {
                rootTask = Parser.Parse(path, context);
            }
            catch (Exception e)
            {
                context.Log.Error("Error while loading {0}:\n{1}", path, e);
                rootTask = null;
            }

            this.name = this.name.TrimStart('.', '/'); // remove "./" from the beginning if it exists
            if (!active.ContainsKey(this.name))
                active.Add(this.name, new List<Reference<DataType>>());
            active[this.name].Add(this);
		}

        ~Reference()
        {
            active[name].Remove(this);
        }

        public override bool CheckConditions()
        {
            return rootTask != null && rootTask.CheckConditions();
        }

        public override void Start()
        {
            if (rootTask == null)
                return;
            rootTask.SafeStart();
        }

        public override void End()
        {
            if (rootTask == null)
                return;
            rootTask.SafeEnd();
        }

        public override void DoAction()
        {
            if (rootTask == null)
                Finish();
            if (rootTask.Finished)
                Finish();
            else
                rootTask.DoAction(); 
        }

        public void Update(double timeSeconds)
        {
            if (rootTask == null)
                return;
            //try
            //{
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
            /*}
            catch (Exception ex)
            {
                Context.Log.Error("PBT Exception on {0}:\n{1}\n{2}\n", Context.Data, ex.Message,
                    ex.InnerException != null ? ex.InnerException.GetType().Name + ": " + ex.InnerException.Message : ex.StackTrace);
            }*/
        }

        private bool ReplacePBT(byte[] pbt)
        {
            try
            {
                SafeEnd();
                Context.ResetForUpdate();
                rootTask = Parser.Parse(pbt, this.name + ".pbt", Context);
                return true;
            }
            catch (Exception e)
            {
                Context.Log.Error("Error while loading {0}:\n{1}", this.name + ".pbt", e);
                rootTask = null;
                return false;
            }
        }

        public static bool ReplacePBT(string name, byte[] pbt)
        {
            if (active.ContainsKey(name))
                foreach (var reference in active[name])
                    if (!reference.ReplacePBT(pbt))
                        return false;
            return true;
        }
	}
}

