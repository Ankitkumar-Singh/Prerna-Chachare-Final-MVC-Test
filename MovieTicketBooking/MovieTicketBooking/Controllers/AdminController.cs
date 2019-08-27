using MovieTicketBooking.Models;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MovieTicketBooking.Controllers
{
    public class AdminController : Controller
    {
        #region Database Context
        /// <summary>The database</summary>
        private MovieTicketContext db = new MovieTicketContext();
        #endregion

        #region Add Movie
        /// <summary>Adds the movie.</summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddMovie()
        {
            return View();
        }

        /// <summary>Adds the movie.</summary>
        /// <param name="movie">The movie.</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddMovie(Movie movie)
        {
            string fileName = Path.GetFileName(movie.ImageFile.FileName);
            movie.MovieImage = "~/MovieImages/" + fileName;
            fileName = Path.Combine(Server.MapPath("~/MovieImages/"), fileName);
            movie.ImageFile.SaveAs(fileName);
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("MovieList");
            }
            return View(movie);
        }
        #endregion

        #region Display Movie List
        /// <summary>Movies the list.</summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult MovieList()
        {
            return View(db.Movies.ToList());
        }
        #endregion

        #region View Movie Details
        /// <summary>Movies the details.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult MovieDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }
        #endregion

        #region Edit Movie Details
        /// <summary>Edits the movie.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditMovie(int id)
        {
            Movie movie = db.Movies.Single(mv => mv.MovieId == id);
            return View(movie);
        }

        /// <summary>Edits the movie.</summary>
        /// <param name="movie">The movie.</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditMovie(Movie movie)
        {
            Movie mv = db.Movies.Single(m => m.MovieId == movie.MovieId);
            if (movie.ImageFile != null)
            {
                string fileName = Path.GetFileName(movie.ImageFile.FileName);
                movie.MovieImage = "~/MovieImages/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/MovieImages/"), fileName);
                movie.ImageFile.SaveAs(fileName);
            }
            else
            {
                mv.MovieImage = movie.MovieImage;
            }
            mv.MovieId = movie.MovieId;
            mv.MovieName = movie.MovieName;
            mv.Category = movie.Category;
            mv.Description = movie.Description;
            mv.TicketPrice = movie.TicketPrice;
            if (ModelState.IsValid)
            {
                db.Entry(mv).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("MovieList");
            }
            return View(movie);
        }
        #endregion

        #region Delete Movie
        /// <summary>Deletes the movie.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteMovie(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        /// <summary>Deletes the confirmed.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("DeleteMovie")]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("MovieList");
        }
        #endregion

        #region View User Bookings
        /// <summary>Users the booking.</summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult UserBooking()
        {
            BookingSeatModel mymodel = new BookingSeatModel();
            mymodel.Bookings = db.Bookings.ToList();
            mymodel.SeatBookings = db.SeatBookings.ToList();
            return View(mymodel);
        }
        #endregion

        #region Add Movie Shows
        /// <summary>Movies the shows.</summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult MovieShows()
        {
            ViewBag.ScrenId = new SelectList(db.Screens, "ScreenId", "ScreenNo");
            ViewBag.ShowId = new SelectList(db.Shows, "ShowId", "ShowTime");
            ViewBag.MovieId = new SelectList(db.Movies, "MovieId", "MovieName");
            return View();
        }

        /// <summary>Movies the shows.</summary>
        /// <param name="movieShow">The movie show.</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult MovieShows(MovieShow movieShow)
        {
            if (ModelState.IsValid)
            {
                db.MovieShows.Add(movieShow);
                db.SaveChanges();
                return RedirectToAction("MovieList");
            }
            ModelState.Clear();
            return View(movieShow);
        }
        #endregion
    }
}