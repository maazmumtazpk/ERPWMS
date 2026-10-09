using System.Collections.Generic;

namespace ERPWMS.Application.Common
{

    public interface IValidationService
    {
        IList<string> Validate(object obj);

        void ValidateAndThrow(object obj);

      
        bool IsValid(object obj);
    }
}