using System;

namespace UI.Helpers
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AutoBindAttribute : Attribute
    {
        public string Key { get; }
        
        public AutoBindAttribute(string key = null)
        {
            Key = key;
        }
    }
}
