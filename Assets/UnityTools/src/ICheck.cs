using System.Reflection;

namespace UnityTools.src
{
	public interface ICheck
	{
		bool Check(string @namespace, object behaviour, FieldInfo field, object gameObject);
	}
}