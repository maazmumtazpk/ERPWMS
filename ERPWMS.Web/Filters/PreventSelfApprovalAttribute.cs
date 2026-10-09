using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Security.Claims;

namespace ERPWMS.Web.Filters
{
    /// <summary>
    /// Prevents a user from approving their own request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class PreventSelfApprovalAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Optional transaction type name (e.g., "SalesReturn", "PurchaseReturn").
        /// </summary>
        public string? TransactionType { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            foreach (var arg in context.ActionArguments)
            {
                var value = arg;
                if (value.Value == null) continue;

                var type = value.GetType();
                var createdByProp = type.GetProperty("CreatedBy");
                var approvedByProp = type.GetProperty("ApprovedBy");
                var userIdProp = type.GetProperty("UserId");

                var createdBy = createdByProp?.GetValue(value)?.ToString();
                var approvedBy = approvedByProp?.GetValue(value)?.ToString();
                var userId = userIdProp?.GetValue(value)?.ToString();

                if (!string.IsNullOrEmpty(currentUserId))
                {
                    if (createdBy == currentUserId || approvedBy == currentUserId || userId == currentUserId)
                    {
                        context.Result = new BadRequestObjectResult(new
                        {
                            success = false,
                            message = "You cannot approve your own request."
                        });
                        return;
                    }
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
