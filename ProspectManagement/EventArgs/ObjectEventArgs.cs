using System;
namespace ProspectManagement.Core
{
    public class ObjectEventArgs : EventArgs
    {
        private readonly object _businessObject;

        public ObjectEventArgs(object businessObject)
        {
            _businessObject = businessObject;
        }

        public object BusinessObject
        {
            get { return _businessObject; }
        }
    }
}