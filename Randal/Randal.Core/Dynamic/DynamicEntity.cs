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
using System.Dynamic;

namespace Randal.Core.Dynamic
{
	public sealed class DynamicEntity : DynamicObject
	{
		private readonly Dictionary<string, object> _dataDictionary;
		private readonly IDynamicEntityConverter _converter;
		private readonly MissingMemberBehavior _missingMemberBehavior;

		public DynamicEntity()
			: this(MissingMemberBehavior.ThrowException)
		{
			
		}

		public DynamicEntity(MissingMemberBehavior missingMemberBehavior,
			IDynamicEntityConverter converter = null, IEqualityComparer<string> comparer = null)
		{
			_missingMemberBehavior = missingMemberBehavior;
			_dataDictionary = new Dictionary<string, object>(comparer ?? StringComparer.InvariantCultureIgnoreCase);
			_converter = converter ?? new NullConverter();
		}

		public bool TestForMember(string name)
		{
			return _dataDictionary.ContainsKey(name);
		}

		public int Count()
		{
			return _dataDictionary.Count;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			var valueFound = _dataDictionary.TryGetValue(binder.Name, out result);

			if (valueFound)
				return true;

			switch (_missingMemberBehavior)
			{
				case MissingMemberBehavior.ReturnsNull:
					return true;
				default:
					return false;
			}
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			_dataDictionary[binder.Name] = value;
			return true;
		}

		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			if (_converter.HasConverters && _converter.TryConversion(binder.Type, _dataDictionary, out result))
				return true;

			return base.TryConvert(binder, out result);
		}

		public void Clear()
		{
			_dataDictionary.Clear();
		}
	}
}