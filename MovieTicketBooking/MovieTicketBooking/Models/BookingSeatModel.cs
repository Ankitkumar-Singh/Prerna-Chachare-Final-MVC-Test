using System.Collections.Generic;

namespace MovieTicketBooking.Models
{
    public class BookingSeatModel
    {
        public IEnumerable<Booking> Bookings { get; set; }
        public IEnumerable<SeatBooking> SeatBookings { get; set; }
    }
}