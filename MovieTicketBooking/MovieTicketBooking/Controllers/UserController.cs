using MovieTicketBooking.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace MovieTicketBooking.Controllers
{
    public class UserController : Controller
    {
        #region Database Context
        /// <summary>The database</summary>
        private MovieTicketContext db = new MovieTicketContext();
        #endregion

        #region View Movies List
        /// <summary>Movies the list.</summary>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult MovieList()
        {
            return View(db.Movies.ToList());
        }
        #endregion

        #region View Show List
        /// <summary>Shows the list.</summary>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult ShowList()
        {
            var movieShows = db.MovieShows.Include(m => m.Movie).Include(m => m.Show);
            return View(movieShows.ToList());
        }
        #endregion

        #region MyBookings
        /// <summary>Mies the booking.</summary>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult MyBooking()
        {
            BookingSeatModel mymodel = new BookingSeatModel();
            mymodel.Bookings = db.Bookings.ToList();
            mymodel.SeatBookings = db.SeatBookings.ToList();
            return View(mymodel);
        }
        #endregion

        #region Select Seats for movie
        /// <summary>Selects the seats.</summary>
        /// <param name="showId">The show identifier.</param>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult SelectSeats(int? showId, int? movieId)
        {
            Show show = db.Shows.Find(showId);
            Session["ShowId"] = showId;
            Session["MovieId"] = movieId;
            var seatBooking = db.SeatBookings.Where(m => m.Booking.ShowId == showId).Select(m => m.SeatId).ToList();
            ViewBag.SeatNo = db.Seats.Where(m => m.ScreenId == show.ScreenId && !seatBooking.Contains(m.SeatId)).ToList();
            return View();
        }

        /// <summary>Selects the seats.</summary>
        /// <param name="SeatNo">The seat no.</param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult SelectSeats(int[] SeatNo)
        {
            if (ModelState.IsValid)
            {
                Booking booking = new Booking();
                booking.UserId = Convert.ToInt32(Session["UserId"]);
                booking.ShowId = Convert.ToInt32(Session["ShowId"]);
                booking.MovieId = Convert.ToInt32(Session["MovieId"]);
                db.Bookings.Add(booking);
                db.SaveChanges();
                Booking book = db.Bookings.Where(m => m.UserId == booking.UserId && m.ShowId == booking.ShowId && m.MovieId == booking.MovieId).OrderByDescending(m => m.BookingId).FirstOrDefault();
                SeatBooking seatBooking = new SeatBooking();
                for (int i = 0; i < SeatNo.Length; i++)
                {
                    seatBooking.BookingId = book.BookingId;
                    seatBooking.SeatId = SeatNo[i];
                    db.SeatBookings.Add(seatBooking);
                    db.SaveChanges();
                }
                var message = new MailMessage();
                message.To.Add(new MailAddress(Convert.ToString(Session["UserEmail"])));  
                message.From = new MailAddress("aress.iphone5@gmail.com");  
                message.Subject = "Movie Tickets Booked";
                message.Body = "Your seats have been booked. Thank you for booking.";
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "aress.iphone5@gmail.com",
                        Password = "Aress123$"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Send(message);
                }
                return RedirectToAction("MyBooking");
            }
            return View();
        }
        #endregion

        #region Cancel Booking
        /// <summary>Cancels the booking.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [HttpGet]
        public ActionResult CancelBooking(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        /// <summary>Deletes the confirmed.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [HttpPost, ActionName("CancelBooking")]
        public ActionResult DeleteConfirmed(int id)
        {
            List<SeatBooking> seatBooking = db.SeatBookings.Where(m => m.BookingId == id).ToList();
            db.SeatBookings.RemoveRange(seatBooking);
            db.SaveChanges();
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("MovieList");
        }
        #endregion
    }
}
