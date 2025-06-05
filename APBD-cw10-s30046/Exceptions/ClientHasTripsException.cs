namespace APBD_cw10_s30046.Exceptions;

public class ClientHasTripsException : Exception
{
    public ClientHasTripsException() 
        : base("Client is assigned to at least one trip and cannot be deleted") {}
}
