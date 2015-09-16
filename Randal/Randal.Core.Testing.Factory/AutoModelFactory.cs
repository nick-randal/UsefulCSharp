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
		public AutoModelFactory(IValueFactory haveValues = null)
		{
			_haveValues = haveValues ?? new IncrementByObjectValueFactory();
			_state = State.Constructed;
		}

		public void Prepare(PrepareOptions options = PrepareOptions.Default)
		{
			if(_state != State.Constructed)
				throw new InvalidOperationException("The factory has already been prepared for creating models.");

			var modelVariable = Expression.Variable(typeof (TModel), "model");
			var havingVariable = Expression.Variable(typeof (IValueFactory), "haveValues");

			var modelMembers = GetMemberInformation(options);
			
			var expressions = CreateExpressionList(modelVariable);

			AddSetterExpressions(modelMembers, modelVariable, havingVariable, expressions);

			CreateLambdaFunction(expressions, modelVariable, havingVariable);

			_state = State.Prepared;
		}

		public TModel Create()
		{
			if (_state != State.Prepared)
				throw new InvalidOperationException("The factory has not been prepared for creating models.  Please call Prepare(...) before this method.");

			return _createModel(_haveValues);
		}

		public IEnumerable<TModel> Create(int howMany)
		{
			if (_state != State.Prepared)
				throw new InvalidOperationException("The factory has not been prepared for creating models.  Please call Prepare(...) before this method.");

			var models = new List<TModel>();

			for (var n = 0; n < howMany; n++)
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

		private void CreateLambdaFunction(ICollection<Expression> expressions, ParameterExpression modelVariable,
			ParameterExpression havingVariable)
		{
			expressions.Add(Expression.Call(havingVariable, "Increment", new Type[0]));
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

				var typeInfo = GetTypeFor(propertyInfo, fieldInfo);
				MethodInfo getValueMethodInfo;

				if (TypeMethodInfoLookup.TryGetValue(typeInfo.BaseType, out getValueMethodInfo))
				{
					expressions.Add(
						CreateSetter(typeInfo, modelVariable, havingVariable, propertyInfo, fieldInfo, getValueMethodInfo)
					);
				}
			}
		}

		private static TypeInfo GetTypeFor(PropertyInfo propertyInfo, FieldInfo fieldInfo)
		{
			var ogType = propertyInfo != null ? propertyInfo.PropertyType : fieldInfo.FieldType;

			var typeInfo = new TypeInfo {BaseType = ogType, OriginalType = ogType, IsNullable = false};

			if (!ogType.IsGenericType)
				return typeInfo;

			if (ogType.GetGenericTypeDefinition() != typeof (Nullable<>)) 
				return typeInfo;

			typeInfo.BaseType = ogType.GenericTypeArguments[0];
			typeInfo.IsNullable = true;

			return typeInfo;
		}

		private static List<Expression> CreateExpressionList(Expression modelVariable)
		{
			var expressions = new List<Expression>();
			var constructorInfo = typeof (TModel).GetConstructor(new Type[0]);

			// ReSharper disable once AssignNullToNotNullAttribute
			// default constructor must exist because of generic where conditional
			expressions.Add(Expression.Assign(modelVariable, Expression.New(constructorInfo)));

			return expressions;
		}

		private static MethodInfo GetMethodInfo<TValue>(Expression<Func<IValueFactory, TValue>> getMethodExpression)
		{
			var me = (MethodCallExpression)getMethodExpression.Body;

			return me.Method;
		}

		private static Expression CreateSetter(TypeInfo typeInfo,
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
			var convertToType = Expression.Convert(callGetValue, typeInfo.OriginalType);

			return Expression.Assign(memberExpression, convertToType);
		}

		private static IEnumerable<MemberInfo> GetMemberInformation(PrepareOptions options)
		{
			var fieldBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField;
			var propertyBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField;

			if (options.HasFlag(PrepareOptions.IncludePrivateFields))
				fieldBindingFlags |= BindingFlags.NonPublic;

			if (options.HasFlag(PrepareOptions.IncludePrivateProperties))
				propertyBindingFlags |= BindingFlags.NonPublic;

			var modelType = typeof (TModel);
			return modelType.GetProperties(propertyBindingFlags).Select(p => (MemberInfo) p)
				.Union(modelType.GetFields(fieldBindingFlags))
				.OrderBy(p => p.Name)
				.ToList();
		}

		private readonly IValueFactory _haveValues;
		private State _state;
		private Func<IValueFactory, TModel> _createModel;

		private static readonly IDictionary<Type, MethodInfo> TypeMethodInfoLookup = new Dictionary<Type, MethodInfo>()
		{
			{ typeof(string), GetMethodInfo(h => h.GetString(null)) },
			{ typeof(bool), GetMethodInfo(h => h.GetBool(null)) },
			{ typeof(short), GetMethodInfo(h => h.GetInt16(null)) },
			{ typeof(int), GetMethodInfo(h => h.GetInt32(null)) },
			{ typeof(long), GetMethodInfo(h => h.GetInt64(null)) },
			{ typeof(char), GetMethodInfo(h => h.GetChar(null)) },
			{ typeof(byte), GetMethodInfo(h => h.GetByte(null)) },
			{ typeof(DateTime), GetMethodInfo(h => h.GetDateTime(null)) },
			{ typeof(decimal), GetMethodInfo(h => h.GetDecimal(null)) },
			{ typeof(float), GetMethodInfo(h => h.GetFloat(null)) },
			{ typeof(double), GetMethodInfo(h => h.GetDouble(null)) }
		}; 

		private enum State
		{
			Constructed = 0,
			Prepared = 1
		}
	}
}