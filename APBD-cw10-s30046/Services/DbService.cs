using APBD_cw10_s30046.Data;
using APBD_cw10_s30046.DTO;
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

}

public interface IDbService
{
    Task<TripResponseDto> GetPagedTripsAsync(int page = 1, int pageSize = 10);
    Task<TripResponseDto> GetAllTripsAsync();
    
    
}