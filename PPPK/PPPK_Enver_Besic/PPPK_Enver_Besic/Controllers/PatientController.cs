using Microsoft.AspNetCore.Mvc;
using PPPK_Enver_Besic.Models;
using PPPK_Enver_Besic.Repositories;
using System.Text;

namespace PPPK_Enver_Besic.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPatientRepository _patientRepository;
        private readonly AppDbContext _context; 

        public PatientController(IPatientRepository patientRepository, AppDbContext context)
        {
            _patientRepository = patientRepository;
            _context = context;
        }

       
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            if (ModelState.IsValid)
            {
                await _patientRepository.AddAsync(patient);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            return View(patient);
        }

        public async Task<IActionResult> Details(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // GET: Patient/ExportDetails/5
        public async Task<IActionResult> ExportDetails(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                return NotFound();

            var csv = new StringBuilder();
            csv.AppendLine("Field,Value");
            csv.AppendLine($"Ime,{patient.FirstName}");
            csv.AppendLine($"Prezime,{patient.LastName}");
            csv.AppendLine($"OIB,{patient.OIB}");
            csv.AppendLine($"Datum rođenja,{patient.DateOfBirth.ToShortDateString()}");
            csv.AppendLine($"Spol,{patient.Gender}");
            csv.AppendLine();

            // Medicinska dokumentacija
            csv.AppendLine("Medicinska dokumentacija:");
            csv.AppendLine("Naziv bolesti,Datum početka,Datum završetka");
            if (patient.MedicalRecords != null && patient.MedicalRecords.Any())
            {
                foreach (var record in patient.MedicalRecords)
                {
                    csv.AppendLine($"{record.IllnessName},{record.StartDate.ToShortDateString()},{(record.EndDate.HasValue ? record.EndDate.Value.ToShortDateString() : "Aktivna")}");
                }
            }
            csv.AppendLine();

            // Pregledi
            csv.AppendLine("Pregledi:");
            csv.AppendLine("Datum i vrijeme,Tip pregleda");
            if (patient.Examinations != null && patient.Examinations.Any())
            {
                foreach (var exam in patient.Examinations)
                {
                    csv.AppendLine($"{exam.ExaminationDateTime},{exam.ExaminationType}");
                }
            }
            csv.AppendLine();

            // Recepti
            csv.AppendLine("Recepti:");
            csv.AppendLine("Naziv lijeka,Opis,Datum propisivanja");
            if (patient.Prescriptions != null && patient.Prescriptions.Any())
            {
                foreach (var pres in patient.Prescriptions)
                {
                    csv.AppendLine($"{pres.MedicationName},{pres.Description},{pres.DatePrescribed.ToShortDateString()}");
                }
            }

            byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());
            return File(buffer, "text/csv", $"pacijent_{patient.Id}_detalji.csv");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                return NotFound();

            return View(patient);
        }

        // POST: Patient/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient patient)
        {
            if (id != patient.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _patientRepository.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
                {
                    if (await _patientRepository.GetByIdAsync(patient.Id) == null)
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Index", "Home");
            }
            return View(patient);
        }

        // GET: Patient/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                return NotFound();
            return View(patient);
        }

        // POST: Patient/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                return NotFound();

            _patientRepository.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
