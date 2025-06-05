using APBD_cw10_s30046.Data;
using APBD_cw10_s30046.DTO;
using APBD_cw10_s30046.Exceptions;
using APBD_cw10_s30046.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APBD_cw10_s30046.Services;

public class DbService(ClientAndTripsDbContext data) : IDbService
{
    public async Task<TripResponseDto> GetPagedTripsAsync(int pageNum = 1, int pageSize = 10)
    {
        var baseQuery = data.Trips
            .Include(t => t.ClientTrips).ThenInclude(ct => ct.IdClientNavigation)
            .Include(t => t.IdCountries)
            .OrderByDescending(t => t.DateFrom);
        
        var totalCount = await baseQuery.CountAsync();
        var allPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var trips = await baseQuery
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new TripResponseDto
        {
            PageNum = pageNum,
            PageSize = pageSize, 
            AllPages = allPages,
            Trips = trips.Select(t => new TripDto()
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries.Select(c => new CountryDto()
                {
                    Name = c.Name,
                }).ToList(),
                Clients = t.ClientTrips.Select(ct => new ClientDto()
                {
                    FirstName = ct.IdClientNavigation.FirstName,
                    LastName = ct.IdClientNavigation.LastName,
                    
                }).ToList()
            }).ToList()
        };
        return result;
    }
    public async Task<TripResponseDto> GetAllTripsAsync()
    {
        var trips = await data.Trips
            .Include(t => t.ClientTrips).ThenInclude(ct => ct.IdClientNavigation)
            .Include(t => t.IdCountries)
            .OrderByDescending(t => t.DateFrom)
            .ToListAsync();

        var result = new TripResponseDto
        {
            PageNum = 1,
            PageSize = trips.Count,
            AllPages = 1,
            Trips = trips.Select(t => new TripDto
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries.Select(c => new CountryDto
                {
                    Name = c.Name
                }).ToList(),
                Clients = t.ClientTrips.Select(ct => new ClientDto
                {
                    FirstName = ct.IdClientNavigation.FirstName,
                    LastName = ct.IdClientNavigation.LastName
                }).ToList()
            }).ToList()
        };

        return result;
    }
    public async Task DeleteClientAsync(int idClient)
    {
        var client = await data.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null)
            throw new NotFoundException("Client not found");

        if (client.ClientTrips.Any())
            throw new ClientHasTripsException();

        data.Clients.Remove(client);
        await data.SaveChangesAsync();
    }

    public async Task AddClientToTripAsync(int idTrip, ClientTripRequestDto dto)
    {
        var trip = await data.Trips.FindAsync(idTrip);
        if (trip == null)
            throw new NotFoundException("Trip not found");
        
        if (trip.DateFrom <= DateTime.Now)
            throw new InvalidOperationException("Trip has already started or ended");
        
        var existingClient = await data.Clients.FirstOrDefaultAsync(c => c.Pesel == dto.Pesel);
        
        if (existingClient != null)
        {
            var alreadyRegistered = await data.ClientTrips
                .AnyAsync(ct => ct.IdClient == existingClient.IdClient && ct.IdTrip == idTrip);

            if (alreadyRegistered)
                throw new InvalidOperationException("Client is already registered for this trip");
        }
        else
        {
            existingClient = new Client
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Telephone = dto.Telephone,
                Pesel = dto.Pesel
            };
            data.Clients.Add(existingClient);
            await data.SaveChangesAsync(); 
        }
        
        var clientTrip = new ClientTrip
        {
            IdClient = existingClient.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = dto.PaymentDate == default ? null : dto.PaymentDate
        };
        
        data.ClientTrips.Add(clientTrip);
        await data.SaveChangesAsync();
    }
    
}

public interface IDbService
{
    Task<TripResponseDto> GetPagedTripsAsync(int page = 1, int pageSize = 10);
    Task<TripResponseDto> GetAllTripsAsync();
    Task DeleteClientAsync(int idClient);
    Task AddClientToTripAsync(int idTrip, ClientTripRequestDto dto);

}