using FluentValidation;
using PurchaseOrder.Models.Concrete;

namespace PurchaseOrder.Api.Validations
{
    /// <summary>
    /// Purchase order request validation.
    /// </summary>
    public class PurchaseOrderRequestValidator : AbstractValidator<PurchaseOrderRequest>
    {
        /// <summary>
        /// Contructor with rule set for <see cref="PurchaseOrderRequest"/>.
        /// </summary>
        /// <remarks>
        /// You'd assume that the request was valid anyway, but I'm just showing how we can check this
        /// using Fluent Validation.
        /// </remarks>
        public PurchaseOrderRequestValidator()
        {
            RuleFor(x => x.CustomerId).GreaterThan(0);
            RuleFor(x => x.Items.Count).GreaterThan(0);
        }
    }
}