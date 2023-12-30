using System;
using StarkCore.Utils;

namespace StarkCore
{
    public abstract class PublicUser
	{
        public string Environment { get; }

        internal PublicUser(string environment)
        {
            Environment = Checks.CheckEnvironment(environment);
        }
    }
}
