// Useful C#
// Copyright (C) 2014-2016 Nicholas Randal
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

namespace Randal.Core.Dynamic
{
	public interface IDynamicEntityConverter
	{
		int ConverterCount { get; }
		bool HasConverters { get; }
		bool TryConversion(Type type, Dictionary<string, object> data, out object result);
	}

	public class DynamicEntityConverter : IDynamicEntityConverter
	{
		private readonly Dictionary<Type, Func<Dictionary<string, object>, object>> _converters;

		public DynamicEntityConverter()
		{
			_converters = new Dictionary<Type, Func<Dictionary<string, object>, object>>();
		}

		public int ConverterCount
		{
			get { return _converters.Count; }
		}

		public bool HasConverters
		{
			get { return _converters.Count > 0; }
		}

		protected Dictionary<Type, Func<Dictionary<string, object>, object>> Converters
		{
			get { return _converters; }
		}

		public bool TryConversion(Type type, Dictionary<string, object> data, out object result)
		{
			Func<Dictionary<string, object>, object> converter;

			if (Converters.TryGetValue(type, out converter))
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

		public Func<Dictionary<string, object>, object> RemoveTypeConverter<TConvertTo>()
		{
			return RemoveTypeConverter(typeof (TConvertTo));
		}

		public Func<Dictionary<string, object>, object> RemoveTypeConverter(Type type)
		{
			Func<Dictionary<string, object>, object> converter;
			if (Converters.TryGetValue(type, out converter))
			{
				Converters.Remove(type);
				return converter;
			}

			return null;
		}
	}
}