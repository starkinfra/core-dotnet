using StarkCore.Utils;

namespace StarkCore
{
    public abstract class User : Resource
    {
        public string Pem { get; }
        public string Environment { get; }

        public User(string environment, string id, string privateKey) : base(id)
        {
            Pem = Checks.CheckPrivateKey(privateKey);
            Environment = Checks.CheckEnvironment(environment);
        }

        public EllipticCurve.PrivateKey PrivateKey()
        {
            return EllipticCurve.PrivateKey.fromPem(Pem);
        }

        public abstract string AccessId();
    }
}
