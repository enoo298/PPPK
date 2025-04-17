using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PPPK_Enver_Besic.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PPPK_Enver_Besic.Controllers
{
    public class ExaminationController : Controller
    {
        private readonly AppDbContext _context;

        public ExaminationController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Examination/Index
        public async Task<IActionResult> Index()
        {
            var examinations = await _context.Examinations
                .Include(e => e.Patient)
                .ToListAsync();
            return View(examinations);
        }

        public async Task<IActionResult> Create()
        {
            // Priprema liste pacijenata za dropdown
            var patients = await _context.Patients.ToListAsync();
            ViewBag.Patients = new SelectList(
                patients.Select(p => new { p.Id, Name = p.FirstName + " " + p.LastName }),
                "Id",
                "Name");

            // Definicija dozvoljenih tipova pregleda
            var examTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "GP", Text = "Opći tjelesni pregled" },
                new SelectListItem { Value = "KRV", Text = "Test krvi" },
                new SelectListItem { Value = "X-RAY", Text = "Rendgensko skeniranje" },
                new SelectListItem { Value = "CT", Text = "CT sken" },
                new SelectListItem { Value = "MR", Text = "MRI sken" },
                new SelectListItem { Value = "ULTRA", Text = "Ultrazvuk" },
                new SelectListItem { Value = "EKG", Text = "Elektrokardiogram" },
                new SelectListItem { Value = "ECHO", Text = "Ehokardiogram" },
                new SelectListItem { Value = "EYE", Text = "Pregled očiju" },
                new SelectListItem { Value = "DERM", Text = "Dermatološki pregled" },
                new SelectListItem { Value = "DENTA", Text = "Pregled zuba" },
                new SelectListItem { Value = "MAMMO", Text = "Mamografija" },
                new SelectListItem { Value = "NEURO", Text = "Neurološki pregled" }
            };
            ViewBag.ExamTypes = examTypes;

            return View();
        }

        // POST: Examination/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Examination examination)
        {
            if (ModelState.IsValid)
            {
                _context.Examinations.Add(examination);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // Ako je došlo do greške, ponovno pripremite liste za view
            var patients = await _context.Patients.ToListAsync();
            ViewBag.Patients = new SelectList(
                patients.Select(p => new { p.Id, Name = p.FirstName + " " + p.LastName }),
                "Id",
                "Name");

            var examTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "GP", Text = "Opći tjelesni pregled" },
                new SelectListItem { Value = "KRV", Text = "Test krvi" },
                new SelectListItem { Value = "X-RAY", Text = "Rendgensko skeniranje" },
                new SelectListItem { Value = "CT", Text = "CT sken" },
                new SelectListItem { Value = "MR", Text = "MRI sken" },
                new SelectListItem { Value = "ULTRA", Text = "Ultrazvuk" },
                new SelectListItem { Value = "EKG", Text = "Elektrokardiogram" },
                new SelectListItem { Value = "ECHO", Text = "Ehokardiogram" },
                new SelectListItem { Value = "EYE", Text = "Pregled očiju" },
                new SelectListItem { Value = "DERM", Text = "Dermatološki pregled" },
                new SelectListItem { Value = "DENTA", Text = "Pregled zuba" },
                new SelectListItem { Value = "MAMMO", Text = "Mamografija" },
                new SelectListItem { Value = "NEURO", Text = "Neurološki pregled" }
            };
            ViewBag.ExamTypes = examTypes;

            return View(examination);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var exam = await _context.Examinations.FindAsync(id);
            if (exam == null)
                return NotFound();

            // Priprema dropdowna za pacijente i tipove pregleda
            var patients = await _context.Patients.ToListAsync();
            ViewBag.Patients = new SelectList(
                patients.Select(p => new { p.Id, Name = p.FirstName + " " + p.LastName }),
                "Id",
                "Name");
            ViewBag.ExamTypes = GetExamTypes();

            return View(exam);
        }

        // POST: Examination/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Examination exam)
        {
            if (id != exam.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _context.Examinations.FindAsync(exam.Id) == null)
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Patients = new SelectList(
                (await _context.Patients.ToListAsync())
                .Select(p => new { p.Id, Name = p.FirstName + " " + p.LastName }),
                "Id",
                "Name");
            ViewBag.ExamTypes = GetExamTypes();
            return View(exam);
        }

        // GET: Examination/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var exam = await _context.Examinations
                .Include(e => e.Patient)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (exam == null)
                return NotFound();
            return View(exam);
        }

        // POST: Examination/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exam = await _context.Examinations.FindAsync(id);
            if (exam == null)
                return NotFound();
            _context.Examinations.Remove(exam);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private List<SelectListItem> GetExamTypes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "GP", Text = "Opći tjelesni pregled" },
                new SelectListItem { Value = "KRV", Text = "Test krvi" },
                new SelectListItem { Value = "X-RAY", Text = "Rendgensko skeniranje" },
                new SelectListItem { Value = "CT", Text = "CT sken" },
                new SelectListItem { Value = "MR", Text = "MRI sken" },
                new SelectListItem { Value = "ULTRA", Text = "Ultrazvuk" },
                new SelectListItem { Value = "EKG", Text = "Elektrokardiogram" },
                new SelectListItem { Value = "ECHO", Text = "Ehokardiogram" },
                new SelectListItem { Value = "EYE", Text = "Pregled očiju" },
                new SelectListItem { Value = "DERM", Text = "Dermatološki pregled" },
                new SelectListItem { Value = "DENTA", Text = "Pregled zuba" },
                new SelectListItem { Value = "MAMMO", Text = "Mamografija" },
                new SelectListItem { Value = "NEURO", Text = "Neurološki pregled" }
            };
        }
    }
}
