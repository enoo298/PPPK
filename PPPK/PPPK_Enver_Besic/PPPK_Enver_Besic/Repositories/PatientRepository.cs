using Microsoft.EntityFrameworkCore;
using PPPK_Enver_Besic.Models;

namespace PPPK_Enver_Besic.Repositories
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Patient>> SearchByLastNameAsync(string lastName)
        {
            // Pretražuje pacijente čije prezime sadrži zadani string (može se prilagoditi, npr. na točno podudaranje)
            return await _context.Patients
                .Where(p => p.LastName.Contains(lastName))
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> SearchByOIBAsync(string oib)
        {
            // Pretražuje pacijente čiji OIB sadrži zadani string (također se može prilagoditi za točno podudaranje)
            return await _context.Patients
                .Where(p => p.OIB.Contains(oib))
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> SearchAsync(string searchTerm)
        {
            // Pretražuje pacijente gdje prezime ili OIB sadrži traženi termin (osjetljivo na mala i velika slova – prilagodite po potrebi)
            return await _context.Patients
                .Where(p => p.LastName.Contains(searchTerm) || p.OIB.Contains(searchTerm))
                .ToListAsync();
        }
    }
}
