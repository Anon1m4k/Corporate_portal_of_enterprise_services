using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServiceHub.Tests.Unit.Model
{
    public static class ModelValidator
    {
        public static IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }
    }
}