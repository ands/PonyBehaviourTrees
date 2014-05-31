using System.Collections.Generic;

namespace PBT
{
    /// <summary>
    /// A storage for pbt scripting variables.
    /// </summary>
    public class VariableStorage : Dictionary<string, dynamic>
    {
        /// <summary>
        /// Variable read/write access.
        /// </summary>
        /// <param name="key">Variable name.</param>
        /// <returns>Returns the variable value.</returns>
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

