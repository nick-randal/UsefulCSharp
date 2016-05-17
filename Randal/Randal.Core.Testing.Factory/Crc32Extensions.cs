using System.Text;
using DamienG.Security.Cryptography;

namespace Randal.Core.Testing.Factory
{
	public static class Crc32Extensions
	{
		public static uint ToCrc32(this string value)
		{
			return Crc32.Compute(Encoding.UTF8.GetBytes(value));
		}
	}
}