﻿using System;
using System.Reflection;
using UnityEditor;

namespace Teamon.Tools.Screenshooter.Utils
{
	public static class ReflectionEx {
		public static Type GetWindowType(string type) {
			return typeof(EditorWindow).Assembly.GetType("UnityEditor." + type);
		}

		public static object FetchField(this object obj, string field) {
			return obj.GetType().GetFieldRecursive(field, false).GetValue(obj);
		}

		public static object FetchProperty(this Type type, string property) {
			return type.GetPropertyRecursive(property, true).GetValue(null, null);
		}

		public static object FetchProperty(this object obj, string property) {
			return obj.GetType().GetPropertyRecursive(property, false).GetValue(obj, null);
		}

		public static object CallMethod(this object obj, string method, params object[] parameters) {
			return obj.GetType().GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(obj, parameters);
		}

		public static object CreateInstance(this Type type, params object[] parameters) {
			Type[] parameterTypes;
			if (parameters == null)
				parameterTypes = null;
			else {
				parameterTypes = new Type[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
					parameterTypes[i] = parameters[i].GetType();
			}

			return CreateInstance(type, parameterTypes, parameters);
		}

		public static object CreateInstance(this Type type, Type[] parameterTypes, object[] parameters) {
			return type.GetConstructor(parameterTypes).Invoke(parameters);
		}

		public static FieldInfo GetFieldRecursive(this Type type, string field, bool isStatic) {
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | (isStatic ? BindingFlags.Static : BindingFlags.Instance);
			do {
				FieldInfo fieldInfo = type.GetField(field, flags);
				if (fieldInfo != null)
					return fieldInfo;

				type = type.BaseType;
			} while (type != null);

			return null;
		}

		public static PropertyInfo GetPropertyRecursive(this Type type, string property, bool isStatic) {
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | (isStatic ? BindingFlags.Static : BindingFlags.Instance);
			do {
				PropertyInfo propertyInfo = type.GetProperty(property, flags);
				if (propertyInfo != null)
					return propertyInfo;

				type = type.BaseType;
			} while (type != null);

			return null;
		}
	}

}
