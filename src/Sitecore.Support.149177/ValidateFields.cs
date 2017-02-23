

namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.FieldsValidation
{
    using Sitecore.Data.Items;
    using Sitecore.Data.Validators;
    using Sitecore.Diagnostics;
    using Sitecore.ExperienceEditor.Speak.Server.Responses;
    using Sitecore.ExperienceEditor.Switchers;
    using System.Collections.Generic;
    using Sitecore.ExperienceEditor.Speak.Ribbon.Requests.FieldsValidation;
    using Sitecore.Data.Fields;
    using Sitecore.ExperienceEditor.Utils;

    public class ValidateFields : Sitecore.ExperienceEditor.Speak.Ribbon.Requests.FieldsValidation.ValidateFields
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            Item item = base.RequestContext.Item;
            Assert.IsNotNull(item, "Item is null");
            using (new ClientDatabaseSwitcher(item.Database))
            {
                ValidatorCollection validators = ValidatorManager.GetFieldsValidators(ValidatorsMode.ValidatorBar, base.RequestContext.GetControlsToValidate().Keys, item.Database);
                ValidatorManager.Validate(validators, new ValidatorOptions(true));
                List<FieldValidationError> list = new List<FieldValidationError>();
                foreach (BaseValidator validator in validators)
                {
                    if (!validator.IsValid && (!Sitecore.Data.ID.IsNullOrEmpty(validator.FieldID)))
                    {
                        FieldValidationError error = new FieldValidationError
                        {
                            Text = validator.Text,
                            Title = validator.Name,
                            FieldId = validator.FieldID.ToString(),
                            DataSourceId = validator.ItemUri.ItemID.ToString(),
                            Errors = validator.Errors,
                            Priority = (int)validator.Result
                        };
                        list.Add(error);
                    }
                }
                return new PipelineProcessorResponseValue { Value = list };
            }
        }
    }
}
