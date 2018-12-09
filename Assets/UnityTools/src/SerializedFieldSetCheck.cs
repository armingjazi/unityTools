using System.Reflection;
using UnityEngine;

namespace UnityTools.src
{
	public class SerializedFieldSetCheck : ICheck
	{
		public bool Check(string @namespace, object behaviour, FieldInfo field, object gameObject)
		{
			foreach (var attribute in field.GetCustomAttributes(true))
			{
				if (attribute.GetType() != typeof(SerializeField))
					continue;

				var value = field.GetValue(behaviour);

				if (value == null || !value.GetType().IsSubclassOf(typeof(Object))) continue;
				
				if (value.ToString() != "null")
					continue;

				var fullName = behaviour.GetType().FullName;
				if (fullName != null && !fullName.Contains(@namespace))
					continue;
			
				Debug.LogError("Field: " + field.Name + " " +
				               "From Script: " + behaviour.GetType().FullName  + " " +
				               "On: " + gameObject + " " +
				               "is missing a reference. Please assign it in the editor");
				return false;
			}

			return true;
		}
	}
}
