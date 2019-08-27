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
    using System.ComponentModel.DataAnnotations;

    public partial class Show
    {
        public Show()
        {
            this.Bookings = new HashSet<Booking>();
            this.MovieShows = new HashSet<MovieShow>();
        }
    
        public int ShowId { get; set; }

        [Display(Name = "Show Time")]
        public System.TimeSpan ShowTime { get; set; }
        public int ScreenId { get; set; }
    
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<MovieShow> MovieShows { get; set; }
        public virtual Screen Screen { get; set; }
    }
}