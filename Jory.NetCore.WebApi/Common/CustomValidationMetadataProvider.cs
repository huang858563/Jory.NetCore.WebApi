using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Jory.NetCore.WebApi.Common
{
    //public class CustomValidationMetadataProvider : IValidationMetadataProvider
    //{
        //private readonly ResourceManager _resourceManager; 
        //private readonly Type _resourceType;
        //public CustomValidationMetadataProvider(string baseName, Type type)
        //{
        //    _resourceType = type;
        //    _resourceManager = new ResourceManager(baseName,
        //        type.GetTypeInfo().Assembly);
        //}
        //public void CreateValidationMetadata(
        //    ValidationMetadataProviderContext context)
        //{
        //    if (context.Key.ModelType.GetTypeInfo().IsValueType &&
        //        context.ValidationMetadata.ValidatorMetadata.All(m => m.GetType() != typeof(RequiredAttribute)))
        //        context.ValidationMetadata.ValidatorMetadata.
        //            Add(new RequiredAttribute());
        //    foreach (var attribute in context.ValidationMetadata.ValidatorMetadata)
        //    {
        //        if (attribute is ValidationAttribute tAttr && tAttr.ErrorMessage == null
        //                          && tAttr.ErrorMessageResourceName == null)
        //        {
        //            var name = tAttr.GetType().Name;
        //            if (_resourceManager.GetString(name) != null)
        //            {
        //                tAttr.ErrorMessageResourceType = _resourceType;
        //                tAttr.ErrorMessageResourceName = name;
        //                tAttr.ErrorMessage = null;
        //            }
        //        }
        //    }
        //}
    //}
}
