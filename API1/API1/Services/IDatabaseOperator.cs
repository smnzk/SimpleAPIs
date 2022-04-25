using APBD5.Models;

namespace APBD5.Services
{
    public interface IDatabaseOperator
    {
        Task<Information> Post(Entry entry);
    }
}
