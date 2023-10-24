using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxChinook.Data
{
    public class DataResult : IDataResult
    {
        public DataResult()
        {

        }
        public DataResult(DataMode mode, string propertyName, Exception err)
        {
            Mode = mode;
            Success = (err == null);
            if (!Success)
            {
                Exception = (err as ValidationException)!;
                if (Exception == null)
                    Exception = new ValidationException(new[] {
                        new ValidationFailure(propertyName, err!.InnerException == null ? err.Message : err.InnerException.Message)
                    });
            }
        }
        public bool Success { get; set; }
        public DataMode Mode { get; set; }
        public ValidationException Exception { get; set; } = default!;
    }
}
