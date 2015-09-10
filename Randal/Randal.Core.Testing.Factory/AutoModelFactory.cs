// Useful C#
// Copyright (C) 2014-2015 Nicholas Randal
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Randal.Core.Testing.Factory
{
	public sealed class AutoModelFactory<TModel> : IGenericAutoModelFactory<TModel>
		where TModel : class, new()
	{
		private readonly IValueFactory _haveValues;
		private State _state;
		private Func<IValueFactory, TModel> _createModel;

		public AutoModelFactory(IValueFactory haveValues = null)
		{
			_haveValues = haveValues ?? new GenericIncrementingValueFactory();
			_state = State.Constructed;
		}

		public void Prepare(
			IDictionary<Type, IUntypedAutoModelFactory> factoryLookup = null,
			bool includePrivateProperties = false, bool includePrivateFields = false
			
			)
		{
			if(_state != State.Constructed)
				throw new InvalidOperationException("The factory has already been prepared for creating models.");

			var modelVariable = Expression.Variable(typeof (TModel), "model");
			var havingVariable = Expression.Variable(typeof (IValueFactory), "haveValues");

			var props = GetMemberInformation(includePrivateProperties, includePrivateFields);
			
			var expressions = new List<Expression>();

			AddNewObjectExpression(expressions, modelVariable);

			AddSetterExpressions(props, modelVariable, havingVariable, expressions);

			CreateLambdaFunction(expressions, modelVariable, havingVariable);

			_state = State.Prepared;
		}

		private void CreateLambdaFunction(ICollection<Expression> expressions, ParameterExpression modelVariable,
			ParameterExpression havingVariable)
		{
			expressions.Add(modelVariable);

			var block = Expression.Block(new[] {modelVariable}, expressions);

			_createModel = Expression.Lambda<Func<IValueFactory, TModel>>(block, havingVariable).Compile();
		}

		private static void AddSetterExpressions(IEnumerable<MemberInfo> props, Expression modelVariable,
			Expression havingVariable, ICollection<Expression> expressions)
		{
			foreach (var memberInfo in props)
			{
				var propertyInfo = memberInfo as PropertyInfo;
				var fieldInfo = memberInfo as FieldInfo;

				Type valueType;
				MethodInfo getValueMethodInfo;

				if (propertyInfo != null)
				{
					valueType = propertyInfo.PropertyType;
				}
				else if (fieldInfo != null)
				{
					valueType = fieldInfo.FieldType;
				}
				else
				{
					continue;
				}

				if (valueType == typeof (string))
				{
					getValueMethodInfo = GetMethodInfo(h => h.GetString(null));
				}
				else if (valueType == typeof (int))
				{
					getValueMethodInfo = GetMethodInfo(h => h.GetInt32(null));
				}
				else if (valueType == typeof (long))
				{
					getValueMethodInfo = GetMethodInfo(h => h.GetInt64(null));
				}
				else if (valueType == typeof (bool))
				{
					getValueMethodInfo = GetMethodInfo(h => h.GetBool(null));
				}
				else if (valueType == typeof (char))
				{
					getValueMethodInfo = GetMethodInfo(h => h.GetChar(null));
				}
				else if (valueType == typeof (byte))
				{
					getValueMethodInfo = GetMethodInfo(h => h.GetByte(null));
				}
				else if (valueType == typeof(DateTime))
				{
					getValueMethodInfo = GetMethodInfo(h => h.GetDateTime(null));
				}
				else if (valueType == typeof(short))
				{
					getValueMethodInfo = GetMethodInfo(h => h.GetInt16(null));
				}
				else if (valueType == typeof(decimal))
				{
					getValueMethodInfo = GetMethodInfo(h => h.GetDecimal(null));
				}
				else
				{
					continue;
				}

				expressions.Add(
					CreateSetter(modelVariable, havingVariable, propertyInfo, fieldInfo, getValueMethodInfo)
				);
			}
		}

		private static void AddNewObjectExpression(ICollection<Expression> expressions, Expression modelVariable)
		{
			var constructorInfo = typeof (TModel).GetConstructor(new Type[0]);

			// ReSharper disable once AssignNullToNotNullAttribute
			// default constructor must exist because of generic where conditional
			expressions.Add(Expression.Assign(modelVariable, Expression.New(constructorInfo)));
		}

		private static MethodInfo GetMethodInfo<TValue>(Expression<Func<IValueFactory, TValue>> getMethodExpression)
		{
			var me = getMethodExpression.Body as MethodCallExpression;
			if (me == null)
				throw new ArgumentException("Expcted a method call signature from IHaveValues", "getMethodExpression");

			return me.Method;
		}

		private static Expression CreateSetter(
			Expression modelVariable, Expression valuesVariable, 
			PropertyInfo propertyInfo, FieldInfo fieldInfo, 
			MethodInfo getValueMethodInfo)
		{
			ConstantExpression memberName;
			MemberExpression memberExpression;

			if (propertyInfo != null)
			{
				memberName = Expression.Constant(propertyInfo.Name);
				memberExpression = Expression.Property(modelVariable, propertyInfo);
			}
			else if (fieldInfo != null)
			{
				memberName = Expression.Constant(fieldInfo.Name);
				memberExpression = Expression.Field(modelVariable, fieldInfo);
			}
			else
			{
				throw new ArgumentNullException("propertyInfo", "Neither propertyInfo or fieldInfo were passed with a valid instance.");
			}

			var callGetValue = Expression.Call(valuesVariable, getValueMethodInfo, memberName);

			return Expression.Assign(memberExpression, callGetValue);
		}
		
		private static IEnumerable<MemberInfo> GetMemberInformation(bool includePrivateFields, bool includePrivateProperties)
		{
			var fieldBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField;
			if (includePrivateFields)
				fieldBindingFlags |= BindingFlags.NonPublic;

			var propertyBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField;
			if (includePrivateProperties)
				propertyBindingFlags |= BindingFlags.NonPublic;

			var modelType = typeof (TModel);
			return modelType.GetProperties(propertyBindingFlags).Select(p => (MemberInfo) p)
				.Union(modelType.GetFields(fieldBindingFlags))
				.OrderBy(p => p.Name)
				.ToList();
		}

		public TModel Create()
		{
			if(_state != State.Prepared)
				throw new InvalidOperationException("The factory has not been prepared for creating models.  Please call Prepare(...) before this method.");
			
			return _createModel(_haveValues);
		}

		public IEnumerable<TModel> Create(int howMany)
		{
			if (_state != State.Prepared)
				throw new InvalidOperationException("The factory has not been prepared for creating models.  Please call Prepare(...) before this method.");

			var models = new List<TModel>();

			for(var n = 0; n < howMany; n++)
				models.Add(_createModel(_haveValues));

			return models;
		}

		public object CreateObject()
		{
			return Create();
		}

		public IEnumerable<object> CreateObject(int howMany)
		{
			return Create(howMany);
		}

		private enum State
		{
			Constructed = 0,
			Prepared = 1
		}
	}
}
