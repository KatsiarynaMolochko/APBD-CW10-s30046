namespace APBD_cw10_s30046.DTO;

public class TripResponseDto
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int AllPages { get; set; }
    public List<TripDto> Trips { get; set; }
    
}