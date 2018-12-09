using System.Reflection;

namespace Editor.Tools
{
	public interface ICheck
	{
		bool Check(string @namespace, object behaviour, FieldInfo field, object gameObject);
	}
}