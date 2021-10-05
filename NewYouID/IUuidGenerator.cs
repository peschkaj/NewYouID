using System.Numerics;

namespace NewYouID
{
    public interface IUuidGenerator
    {
        public BigInteger NextId();
    }
}