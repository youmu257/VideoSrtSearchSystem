using MySqlConnector;
using System.Numerics;

namespace VideoSrtSearchSystem.Tool.MySQL
{
    public abstract class BaseModel
    {
        protected int GetInt(MySqlDataReader dr, int index)
        {
            return dr.IsDBNull(index) ? 0 : dr.GetInt32(index);
        }

        protected ulong GetUnsignedLong(MySqlDataReader dr, int index)
        {
            return dr.IsDBNull(index) ? 0 : dr.GetUInt64(index);
        }

        protected uint GetUnsignedInt(MySqlDataReader dr, int index)
        {
            return dr.IsDBNull(index) ? 0 : dr.GetUInt32(index);
        }

        protected short GetShort(MySqlDataReader dr, int index)
        {
            return dr.IsDBNull(index) ? (short)0 : dr.GetInt16(index);
        }

        protected BigInteger GetBigInteger(MySqlDataReader dr, int index)
        {
            return dr.IsDBNull(index) ? 0 : (BigInteger)dr.GetInt64(index);
        }

        protected string GetString(MySqlDataReader dr, int index)
        {
            return dr.IsDBNull(index) ? string.Empty : dr.GetString(index);
        }

        protected byte[]? GetBytes(MySqlDataReader dr, int index)
        {
            // 先確認長度
            int length = (int)dr.GetBytes(index, 0, null, 0, 0);
            if (length == 0)
            {
                return null;
            }
            var byteArray = new byte[length];
            dr.GetBytes(index, 0, byteArray, 0, length);
            return byteArray;
        }

        public abstract void Set(MySqlDataReader dr);
    }
}
