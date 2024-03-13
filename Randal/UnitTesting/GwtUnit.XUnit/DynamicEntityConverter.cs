// Useful C#
// Copyright (C) 2014-2022 Nicholas Randal
// 
// Useful C# is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

using System;
using System.Collections.Generic;

namespace GwtUnit.XUnit;

public class DynamicEntityConverter : IDynamicEntityConverter
{
	private readonly Dictionary<Type, Func<Dictionary<string, object>, object>> _converters;

	public DynamicEntityConverter()
	{
		_converters = new Dictionary<Type, Func<Dictionary<string, object>, object>>();
	}

	public int ConverterCount => _converters.Count;

	public bool HasConverters => _converters.Count > 0;

	protected Dictionary<Type, Func<Dictionary<string, object>, object>> Converters => _converters;

	public bool TryConversion(Type type, Dictionary<string, object> data, out object? result)
	{

		if (Converters.TryGetValue(type, out var converter))
		{
			result = converter(data);
			return true;
		}

		result = null;
		return false;
	}

	public void AddTypeConverter<TConvertTo>(Func<Dictionary<string, object>, object> converter)
	{
		Converters.Add(typeof (TConvertTo), converter);
	}

	public void AddTypeConverter(Type type, Func<Dictionary<string, object>, object> converter)
	{
		Converters.Add(type, converter);
	}

	public Func<Dictionary<string, object>, object>? RemoveTypeConverter<TConvertTo>()
	{
		return RemoveTypeConverter(typeof (TConvertTo));
	}

	public Func<Dictionary<string, object>, object>? RemoveTypeConverter(Type type)
	{
		if (Converters.TryGetValue(type, out var converter))
		{
			Converters.Remove(type);
			return converter;
		}

		return null;
	}
}