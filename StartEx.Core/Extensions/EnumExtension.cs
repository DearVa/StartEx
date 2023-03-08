using System;
using System.Runtime.CompilerServices;

namespace StartEx.Core.Extensions; 

public static class EnumExtension {
	/// <summary>
	/// 比<see cref="Enum.HasFlag"/>更快的方法，去掉了装箱和类型检查，<b>只适用于int类型的enum</b>
	/// </summary>
	/// <typeparam name="TEnum"></typeparam>
	/// <param name="enum1"></param>
	/// <param name="enum2"></param>
	/// <returns></returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool HasFlagUnsafe<TEnum>(this TEnum enum1, TEnum enum2) where TEnum : struct, Enum {
		return (Unsafe.As<TEnum, int>(ref enum1) & Unsafe.As<TEnum, int>(ref enum2)) != 0;
	}
}