﻿using System.Runtime.InteropServices.JavaScript;

namespace APBD_cw10_s30046.DTO;

public class ClientTripRequestDto
{
    
    public String FirstName { get; set; }
    public String LastName { get; set; }
    public String Email { get; set; }
    public String Telephone { get; set; }
    public String Pesel { get; set; }
    public int IdTrip { get; set; }
    public String TripName { get; set; }
    public DateTime? PaymentDate { get; set; }
}