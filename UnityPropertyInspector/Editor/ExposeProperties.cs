using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityPropertyInspector.Scripts;
using Object = UnityEngine.Object;


public static class ExposeProperties {
	public static void Expose(PropertyField[] properties) {

		GUILayoutOption[] emptyOptions = new GUILayoutOption[0];

		EditorGUILayout.BeginVertical(emptyOptions);

		foreach (PropertyField field in properties) {

			EditorGUILayout.BeginHorizontal(emptyOptions);
			switch (field.Type) {
				case SerializedPropertyType.Integer: {
						var oldValue = (int)field.GetValue();
						var newValue = EditorGUILayout.IntField(field.Name, oldValue, emptyOptions);
					if (oldValue != newValue) {
						field.SetValue(newValue);
#if UNITY_EDITOR
						EditorUtility.SetDirty((Object)field.m_Instance);
							if (Application.isEditor && !Application.isPlaying)
								EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
						}
				}
					break;
				case SerializedPropertyType.Float: {
						var oldValue = (float)field.GetValue();
						var newValue = EditorGUILayout.FloatField(field.Name, oldValue, emptyOptions);
					if (Math.Abs(oldValue - newValue) > Mathf.Epsilon) {
						field.SetValue(newValue);
						#if UNITY_EDITOR
						EditorUtility.SetDirty((Object) field.m_Instance);
							if (Application.isEditor && !Application.isPlaying)
								EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
						}
				}
					break;
				case SerializedPropertyType.Boolean: {
						var oldValue = (bool)field.GetValue();
						var newValue = EditorGUILayout.Toggle(field.Name, oldValue, emptyOptions);
					if (oldValue != newValue) {
						field.SetValue(newValue);
#if UNITY_EDITOR
						EditorUtility.SetDirty((Object)field.m_Instance);
							if (Application.isEditor && !Application.isPlaying)
								EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
						}
				}
					break;
				case SerializedPropertyType.String: {
						var oldValue = (string)field.GetValue();
						var newValue = EditorGUILayout.TextField(field.Name, oldValue, emptyOptions);
					if (oldValue != newValue) {
						field.SetValue(newValue);
#if UNITY_EDITOR
						EditorUtility.SetDirty((Object)field.m_Instance);
							if (Application.isEditor && !Application.isPlaying)
								EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
						}
				}
					break;
				case SerializedPropertyType.Vector2: {
						var oldValue = (Vector2)field.GetValue();
						var newValue = EditorGUILayout.Vector2Field(field.Name, oldValue, emptyOptions);
					if (oldValue != newValue) {
						field.SetValue(newValue);
#if UNITY_EDITOR
						EditorUtility.SetDirty((Object)field.m_Instance);
							if (Application.isEditor && !Application.isPlaying)
								EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
						}
				}
					break;
				case SerializedPropertyType.Vector3: {
						var oldValue = (Vector3)field.GetValue();
						var newValue = EditorGUILayout.Vector3Field(field.Name, oldValue, emptyOptions);
					if (oldValue != newValue) {
						field.SetValue(newValue);
#if UNITY_EDITOR
						EditorUtility.SetDirty((Object)field.m_Instance);
							if (Application.isEditor && !Application.isPlaying)
								EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
						}
				}
					break;
				case SerializedPropertyType.Enum: {
						var oldValue = (Enum)field.GetValue();
						var newValue = EditorGUILayout.EnumPopup(field.Name, oldValue, emptyOptions);
					if (oldValue != newValue) {
						field.SetValue(newValue);
#if UNITY_EDITOR
						EditorUtility.SetDirty((Object)field.m_Instance);
						if(Application.isEditor && !Application.isPlaying)
							EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
						}
				}
					break;
				case SerializedPropertyType.ObjectReference: {
					var oldValue = (Object)field.GetValue();
					var newValue = EditorGUILayout.ObjectField(field.Name, (Object) field.GetValue(),
						field.GetPropertyType(), true, emptyOptions);
						if (newValue != null && (oldValue == null || !oldValue.Equals(newValue))) {
							field.SetValue(newValue);
#if UNITY_EDITOR
							EditorUtility.SetDirty((Object)field.m_Instance);
							if (Application.isEditor && !Application.isPlaying)
								EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
						}
					break;
				}
			}
			EditorGUILayout.EndHorizontal();

		}

		EditorGUILayout.EndVertical();

	}

	public static PropertyField[] GetProperties(System.Object obj) {

		List<PropertyField> fields = new List<PropertyField>();

		PropertyInfo[] infos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

		foreach (PropertyInfo info in infos) {

			if (!(info.CanRead && info.CanWrite))
				continue;

			object[] attributes = info.GetCustomAttributes(true);

			bool isExposed = false;

			foreach (object o in attributes) {
				if (o.GetType() == typeof(ExposePropertyAttribute)) {
					isExposed = true;
					break;
				}
			}

			if (!isExposed)
				continue;

			SerializedPropertyType type = SerializedPropertyType.Integer;

			if (PropertyField.GetPropertyType(info, out type)) {
				PropertyField field = new PropertyField(obj, info, type);
				fields.Add(field);
			}

		}

		return fields.ToArray();

	}

}


public class PropertyField {
	public System.Object m_Instance;
	PropertyInfo m_Info;
	SerializedPropertyType m_Type;

	MethodInfo m_Getter;
	MethodInfo m_Setter;

	public SerializedPropertyType Type {
		get {
			return m_Type;
		}
	}

	public Type GetPropertyType() {
		return m_Info.PropertyType;
	}

	public String Name {
		get {
			return ObjectNames.NicifyVariableName(m_Info.Name);
		}
	}

	public PropertyField(System.Object instance, PropertyInfo info, SerializedPropertyType type) {

		m_Instance = instance;
		m_Info = info;
		m_Type = type;

		m_Getter = m_Info.GetGetMethod();
		m_Setter = m_Info.GetSetMethod();
	}

	public System.Object GetValue() {
		return m_Getter.Invoke(m_Instance, null);
	}

	public void SetValue(System.Object value) {
		m_Setter.Invoke(m_Instance, new System.Object[] { value });
	}

	public static bool GetPropertyType(PropertyInfo info, out SerializedPropertyType propertyType) {

		propertyType = SerializedPropertyType.Generic;

		Type type = info.PropertyType;

		if (type == typeof(int)) {
			propertyType = SerializedPropertyType.Integer;
			return true;
		}

		if (type == typeof(float)) {
			propertyType = SerializedPropertyType.Float;
			return true;
		}

		if (type == typeof(bool)) {
			propertyType = SerializedPropertyType.Boolean;
			return true;
		}

		if (type == typeof(string)) {
			propertyType = SerializedPropertyType.String;
			return true;
		}

		if (type == typeof(Vector2)) {
			propertyType = SerializedPropertyType.Vector2;
			return true;
		}

		if (type == typeof(Vector3)) {
			propertyType = SerializedPropertyType.Vector3;
			return true;
		}

		if (type.IsEnum) {
			propertyType = SerializedPropertyType.Enum;
			return true;
		}

		// COMMENT OUT to NOT expose custom objects/types
		propertyType = SerializedPropertyType.ObjectReference;
		return true;

		//return false;

	}

}