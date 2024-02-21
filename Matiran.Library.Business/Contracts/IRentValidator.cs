using Matiran.Library.Model;

namespace Matiran.Library.Data.Contracts
{
    public interface IRentValidator
    {
        Task ValidateRent(Rent rent);
        Task ValidateReturnRent(ReturnBookRequest returnBookRequest);
    }
}
