//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MovieTicketBooking.Models
{
    using System.Collections.Generic;

    public partial class Seat
    {
        public Seat()
        {
            this.SeatBookings = new HashSet<SeatBooking>();
        }
    
        public int SeatId { get; set; }
        public int ScreenId { get; set; }
        public int SeatNo { get; set; }
    
        public virtual Screen Screen { get; set; }
        public virtual ICollection<SeatBooking> SeatBookings { get; set; }
    }
}