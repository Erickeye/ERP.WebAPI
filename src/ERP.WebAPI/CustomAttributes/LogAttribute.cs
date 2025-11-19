using ERP.Library.Enums;

namespace ERP.WebAPI.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class LogAttribute : Attribute
    {
        public OperationActionType ActionCode { get; }
        public string? Description { get; }

        public LogAttribute(OperationActionType actionCode, string? description)
        {
            Description = description;
            ActionCode = actionCode;
        }
    }

}
