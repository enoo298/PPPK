using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PPPK_Enver_Besic.Models;

namespace PPPK_Enver_Besic.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly AppDbContext _context;

        public PrescriptionController(AppDbContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> Index()
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.Patient)
                .ToListAsync();
            return View(prescriptions);
        }

        
        public async Task<IActionResult> Create()
        {
            var patients = await _context.Patients.ToListAsync();
            ViewBag.Patients = new SelectList(
                patients.Select(p => new { p.Id, Name = p.FirstName + " " + p.LastName }),
                "Id",
                "Name");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Prescription prescription)
        {
            if (ModelState.IsValid)
            {
                // Ako je DatePrescribed Kind Unspecified, postavi ga kao UTC
                prescription.DatePrescribed = DateTime.SpecifyKind(prescription.DatePrescribed, DateTimeKind.Utc);

                _context.Prescriptions.Add(prescription);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var patients = await _context.Patients.ToListAsync();
            ViewBag.Patients = new SelectList(
                patients.Select(p => new { p.Id, Name = p.FirstName + " " + p.LastName }),
                "Id",
                "Name");
            return View(prescription);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
                return NotFound();

            var patients = await _context.Patients.ToListAsync();
            ViewBag.Patients = new SelectList(
                patients.Select(p => new { p.Id, Name = p.FirstName + " " + p.LastName }),
                "Id",
                "Name");
            return View(prescription);
        }

        // POST: Prescription/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Prescription prescription)
        {
            if (id != prescription.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Osigurajte da DatePrescribed bude u UTC
                    prescription.DatePrescribed = DateTime.SpecifyKind(prescription.DatePrescribed, DateTimeKind.Utc);
                    _context.Update(prescription);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _context.Prescriptions.FindAsync(prescription.Id) == null)
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            var patients = await _context.Patients.ToListAsync();
            ViewBag.Patients = new SelectList(
                patients.Select(p => new { p.Id, Name = p.FirstName + " " + p.LastName }),
                "Id",
                "Name");
            return View(prescription);
        }

        // GET: Prescription/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (prescription == null)
                return NotFound();
            return View(prescription);
        }

        // POST: Prescription/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
                return NotFound();
            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
