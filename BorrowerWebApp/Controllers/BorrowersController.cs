using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BorrowerWebApp.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BorrowerWebApp.Controllers
{
    public class BorrowersController : Controller
    {
        private readonly BorrowerContext _context;

        public BorrowersController(BorrowerContext context)
        {
            _context = context;
        }

        // GET: Borrowers
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? page, int? id)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["FirstNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "firstname_desc" : "";
            ViewData["LastNameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "lastname_desc" : "";
            ViewData["GenderSortParm"] = String.IsNullOrEmpty(sortOrder) ? "gender_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null) {
                page = 1;
            }

            var borrowers = from s in _context.Borrower
                            select s;
            switch (sortOrder) {
                case "firstname":
                    borrowers = borrowers.OrderBy(s => s.FirstName);
                    break;
                case "firstname_desc":
                    borrowers = borrowers.OrderByDescending(s => s.FirstName);
                    break;
                case "lastname":
                    borrowers = borrowers.OrderBy(s => s.LastName);
                    break;
                case "lastname_desc":
                    borrowers = borrowers.OrderByDescending(s => s.LastName);
                    break;
                case "gender":
                    borrowers = borrowers.OrderBy(s => s.Gender);
                    break;
                case "gender_desc":
                    borrowers = borrowers.OrderByDescending(s => s.Gender);
                    break;
                case "Date":
                    borrowers = borrowers.OrderBy(s => s.RegistrationDate);
                    break;
                case "date_desc":
                    borrowers = borrowers.OrderByDescending(s => s.RegistrationDate);
                    break;
                default:
                    borrowers = borrowers.OrderBy(s => s.LastName);
                    break;
            }
            int pageSize = 5;
            return View(await PaginatedList<Borrower>.CreateAsync(borrowers.AsNoTracking(), page ?? 1, pageSize));
        }

        // GET: Borrowers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) {
                return NotFound();
            }

            var borrower = await _context.Borrower
                .FirstOrDefaultAsync(m => m.BorrowerId == id);
            if (borrower == null) {
                return NotFound();
            }

            return View(borrower);
        }

        // GET: Borrowers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Borrowers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("BorrowerId,FirstName,LastName,Gender,RegistrationDate,Login,Password")] Borrower borrower)
        {
            if (ModelState.IsValid) {
                _context.Add(borrower);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(borrower);
        }

        // GET: Borrowers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) {
                return NotFound();
            }

            var borrower = await _context.Borrower.FindAsync(id);
            if (borrower == null) {
                return NotFound();
            }
            return View(borrower);
        }

        // POST: Borrowers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("BorrowerId,FirstName,LastName,Gender,RegistrationDate,Login,Password")] Borrower borrower)
        {
            if (id != borrower.BorrowerId) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(borrower);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!BorrowerExists(borrower.BorrowerId)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(borrower);
        }

        // GET: Borrowers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) {
                return NotFound();
            }

            var borrower = await _context.Borrower
                .FirstOrDefaultAsync(m => m.BorrowerId == id);
            if (borrower == null) {
                return NotFound();
            }

            return View(borrower);
        }

        // POST: Borrowers/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var borrower = await _context.Borrower.FindAsync(id);
            _context.Borrower.Remove(borrower);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BorrowerExists(int id)
        {
            return _context.Borrower.Any(e => e.BorrowerId == id);
        }


        public PartialViewResult CreatePartial()
        {
            this.ViewBag.genders = new List<SelectListItem>
{
            new SelectListItem { Value = "M", Text = "Male" },
            new SelectListItem { Value = "F", Text = "Female" }
        };
            return PartialView("_CreatePartial");
        }

        public async Task<PartialViewResult> EditPartial(int? id)
        {
            var borrower = await _context.Borrower.FirstOrDefaultAsync(m => m.BorrowerId == id);
            List <SelectListItem> items = new List<SelectListItem>
{
            new SelectListItem { Value = "M", Text = "Male" },
            new SelectListItem { Value = "F", Text = "Female" }
        };
            this.ViewBag.genders = new SelectList(items, "Value", "Text", borrower.Gender);
            return PartialView("_EditPartial", borrower);

        }


    }
}
