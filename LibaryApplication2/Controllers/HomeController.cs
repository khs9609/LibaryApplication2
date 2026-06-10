using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer.Symbols;
using LibaryApplication2.Context;
using LibaryApplication2.Models;

namespace LibaryApplication2.Controllers
{
    public class HomeController : Controller
    {
        private LibraryDb db = new LibraryDb();

        // GET: Home
        public ActionResult Index()
        {
            int maxListCount = 3;
            int pageNum = 1;
            string keyword = Request.QueryString["keyword"] ?? string.Empty;
            string searchKind = Request.QueryString["searchKind"] ?? string.Empty;
            int totalCount = 0;
            

            if (Request.QueryString["page"] != null)
                pageNum = Convert.ToInt32(Request.QueryString["page"]);

            var books = new List<Book>();

            if (string.IsNullOrEmpty(keyword))
            {
                books = db.Books.OrderBy(x => x.Book_U)
                        .Skip((pageNum - 1) * maxListCount)
                        .Take(maxListCount).ToList();
                totalCount = db.Books.Count();
            }
            else
            {
                /* db.Books 는 여러번 사용되서 상단에 따로 정의하고 싶지만 
                 검색 결과가 많아지는 경우, 속도 저하의 영향을 끼칠 수 있어 
                 여기선 코드가 길어지더라도 아래와 같이 where 먼저 걸어준다.
                 */
                switch (searchKind) {
                    case "Title":
                        books = db.Books.Where(x => x.Title.Contains(keyword)).ToList();
                        totalCount = db.Books.Where(x => x.Title.Contains(keyword)).Count();
                        break;
                    case "Writer":
                        books = db.Books.Where(x => x.Writer.Contains(keyword)).ToList();
                        totalCount = db.Books.Where(x => x.Writer.Contains(keyword)).Count();
                        break;
                    case "Publisher":
                        books = db.Books.Where(x => x.Publisher.Contains(keyword)).ToList();
                        totalCount = db.Books.Where(x => x.Publisher.Contains(keyword)).Count();
                        break;
                }

                books = books.OrderBy(x => x.Book_U)
                    .Skip((pageNum - 1) * maxListCount)
                    .Take(maxListCount).ToList();
            }

            ViewBag.Page = pageNum;
            ViewBag.TotalCount = totalCount;
            ViewBag.MaxListCount = maxListCount;
            ViewBag.SearchKind = searchKind;
            ViewBag.Keyword = keyword;

            return View(books);
        }

        // GET: Home/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Find 라는 메서드는 매개변수 해당 되는 고유한 ID 하나만 가져오는 기능이다.
            Book book = db.Books.Find(id);
            if (book == null)
                return HttpNotFound();

            return View(book);
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하세요. 
        // 자세한 내용은 https://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하세요.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Book_U,Title,Writer,Summary,Publisher,Published_Date")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Books.Add(book);
                db.SaveChanges(); // 이 페이지에서 어떤 action이 일어난다고 했을 때 (create. update, delete 등) => 저장을 해줘야 한다.
                return RedirectToAction("Index");
            }

            return View(book);
        }

        // GET: Home/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Book book = db.Books.Find(id);
            if (book == null)
                return HttpNotFound();

            return View(book);
        }

        // POST: Home/Edit/5
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하세요. 
        // 자세한 내용은 https://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하세요.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Book_U,Title,Writer,Summary,Publisher,Published_Date")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Home/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
