using Microsoft.AspNetCore.Mvc;
using PPPK_Enver_Besic.Repositories;
using System.Threading.Tasks;

namespace PPPK_Enver_Besic.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPatientRepository _patientRepository;

        public HomeController(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        // Po?etna stranica – prikazuje sve pacijente
        public async Task<IActionResult> Index()
        {
            var patients = await _patientRepository.GetAllAsync();
            return View(patients);
        }

        // Akcija za pretragu (preuzima query string "searchTerm")
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // Ako nije unesen termin, preusmjeri natrag na po?etnu stranicu
                return RedirectToAction("Index");
            }

            var patients = await _patientRepository.SearchAsync(searchTerm);
            return View("Index", patients);
        }
    }
}
