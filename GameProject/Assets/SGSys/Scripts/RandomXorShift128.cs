/**
 * @file
 * @brief xorshift によるランダム
 */
using System;

namespace SGSys
{
	/// xorshift によるランダム
	public class XorShift128 : Random
	{
		int W, X, Y, Z;

		/// シード明示版
		public XorShift128(int x, int y, int z, int w)
        {
			x = X;
			y = Y;
			z = Z;
			w = W;
		}

		/// クロックチックをシードにする
		public XorShift128() : this((int)DateTime.Now.Ticks)
        {
		}

		public XorShift128(int seed)
        {
			W = 123456789 ^ seed;
			X = 362436069 ^ (seed << 16 + seed >> 16);
			Y = 521288629 ^ (W + X);
			Z = 88675123 ^ (X ^ Y);
		}

		/// 乱数取得 0 ～ 0x7fffffff
		public override int Next()
        {
			int t = X ^ (X << 11);
			X = Y;
			Y = Z;
			Z = W;
			W = (W ^ (W >> 19)) ^ (t ^ (t >> 8));
			return W & 0x7fffffff;
		}

		/// 範囲内の乱数を取得 [min,max)
		public override int Next(int min, int max)
        {
			return ((this.Next() >> 1) % (max - min)) + min;
		}

		/// 範囲内の乱数を取得 [0,max)
		public override int Next(int max)
        {
			return Next(0, max);
		}
	}
}
