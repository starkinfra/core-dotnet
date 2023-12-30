using System.Linq;
using System.Reflection;
using System.Collections.Generic;


namespace StarkCore.Utils
{
    public abstract class Resource : SubResource
    {
        public string ID { get; } 

        public Resource(string id)
        {
            ID = id;
        }
    }
}
