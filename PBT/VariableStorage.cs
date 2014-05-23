using System.Collections.Generic;

namespace PBT
{
    public class VariableStorage : Dictionary<string, dynamic>
    {
        public new dynamic this[string key]
        {
            get
            {
                dynamic ret;
                if (TryGetValue(key, out ret))
                    return ret;
                return null;
            }
            set { base[key] = value; }
        }
    }
}

