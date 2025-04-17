using PPPK_Enver_Besic.Models;

namespace PPPK_Enver_Besic.Repositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<IEnumerable<Patient>> SearchByLastNameAsync(string lastName);
        Task<IEnumerable<Patient>> SearchByOIBAsync(string oib);

        Task<IEnumerable<Patient>> SearchAsync(string searchTerm);
    }
}
