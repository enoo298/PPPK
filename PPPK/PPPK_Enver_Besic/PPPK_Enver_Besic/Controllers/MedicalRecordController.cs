using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PPPK_Enver_Besic.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PPPK_Enver_Besic.Controllers
{
    public class MedicalRecordController : Controller
    {
        private readonly AppDbContext _context;

        public MedicalRecordController(AppDbContext context)
        {
            _context = context;
        }

        // GET: MedicalRecord/Index
        public async Task<IActionResult> Index()
        {
            var records = await _context.MedicalRecords
                .Include(m => m.Patient)
                .ToListAsync();
            return View(records);
        }

        // GET: MedicalRecord/Create
        public async Task<IActionResult> Create()
        {
            var patients = await _context.Patients.ToListAsync();
            ViewBag.Patients = new SelectList(
                patients.Select(p => new { p.Id, Name = p.FirstName + " " + p.LastName }),
                "Id",
                "Name");
            return View();
        }

        // POST: MedicalRecord/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalRecord record)
        {
            if (ModelState.IsValid)
            {
                _context.MedicalRecords.Add(record);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var patients = await _context.Patients.ToListAsync();
            ViewBag.Patients = new SelectList(
                patients.Select(p => new { p.Id, Name = p.FirstName + " " + p.LastName }),
                "Id",
                "Name");
            return View(record);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var record = await _context.MedicalRecords.FindAsync(id);
            if (record == null)
                return NotFound();

            var patients = await _context.Patients.ToListAsync();
            ViewBag.Patients = new SelectList(
                patients.Select(p => new { p.Id, Name = p.FirstName + " " + p.LastName }),
                "Id",
                "Name");
            return View(record);
        }

        // POST: MedicalRecord/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicalRecord record)
        {
            if (id != record.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(record);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _context.MedicalRecords.FindAsync(record.Id) == null)
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
            return View(record);
        }

        // GET: MedicalRecord/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var record = await _context.MedicalRecords
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (record == null)
                return NotFound();
            return View(record);
        }

        // POST: MedicalRecord/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var record = await _context.MedicalRecords.FindAsync(id);
            if (record == null)
                return NotFound();
            _context.MedicalRecords.Remove(record);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
