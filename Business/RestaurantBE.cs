using Data.DAO;
using Data.Search;
using Data.VO.Operations;
using Data.VO.Restaurant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business;

public class RestaurantBE
{
    private RestaurantDAO _dao = new();

    public async Task<IList<RestaurantVO>> GetRestaurantLsitBy(RestaurantSH search)
    {
        IList<RestaurantVO> restaurantList = new List<RestaurantVO>
        {
            new RestaurantVO
            {
                Id = Guid.Empty,
                Slug = "ocean-grill-raw-bar", Name = "Ocean Grill & Raw Bar", Cuisine = "Seafood", PriceLevel = 3,
                Rating = 4.9m, ReviewsCount = 420,
                Location = "V&A Waterfront, Cape Town", Address = "10 Breakwater Blvd, V&A Waterfront, Cape Town, 8001",
                CrossStreet = "Dock Road & Pierhead",
                ImageUrl = "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?auto=format&fit=crop&w=800&q=80",
                Badge = "Michelin Guide 2026",
                Description = "Panoramic oceanfront views serving fresh line-caught oysters, prime dry-aged seafood, and handcrafted coastal cocktails.",
                DressCode = "Smart Casual", Parking = "Complimentary Valet at Victoria Mall", Corkage = "R150 per bottle",
                TableList = GetTableList(25)
            },
            new RestaurantVO
            {
                Id = Guid.Empty,
                Slug = "le-petit-manoir", Name = "Le Petit Manoir", Cuisine = "French", PriceLevel = 4,
                Rating = 4.95m, ReviewsCount = 310,
                Location = "Franschhoek Valley", Address = "54 Huguenot Rd, Franschhoek, 7690",
                CrossStreet = "Main Road & Boucheron Street",
                ImageUrl = "https://images.unsplash.com/photo-1550966871-3ed3cdb5ed0c?auto=format&fit=crop&w=800&q=80",
                Badge = "World's 50 Best",
                Description = "Sophisticated modern French gastronomy using local artisanal produce paired with fine Cape Winelands vintages.",
                DressCode = "Formal / Elegant", Parking = "Private Estate Parking", Corkage = "R250 per bottle"
            },
            new RestaurantVO
            {
                Id = Guid.Empty,
                Slug = "kuro-omakase", Name = "Kuro Omakase", Cuisine = "Japanese", PriceLevel = 4,
                Rating = 4.88m, ReviewsCount = 195,
                Location = "Camps Bay, Cape Town", Address = "88 Victoria Rd, Camps Bay, Cape Town, 8005",
                CrossStreet = "Victoria Rd & Argyle",
                ImageUrl = "https://images.unsplash.com/photo-1579027989536-b7b1f875659b?auto=format&fit=crop&w=800&q=80",
                Badge = "Chef's Table",
                Description = "An intimate 14-seat counter Omakase experience featuring air-flown Tokyo market seafood and sake pairings.",
                DressCode = "Smart Casual", Parking = "Underground Bay Parking", Corkage = "No outside bottles"
            },
            new RestaurantVO
            {
                Id = Guid.Empty,
                Slug = "la-trattoria-del-sol", Name = "La Trattoria del Sol", Cuisine = "Italian", PriceLevel = 2,
                Rating = 4.76m, ReviewsCount = 512,
                Location = "Constantia Upper", Address = "12 Constantia Main Rd, Constantia, 7806",
                CrossStreet = "Spaanschemat River Rd",
                ImageUrl = "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?auto=format&fit=crop&w=800&q=80",
                Badge = "Top Terrace",
                Description = "Sunlit olive garden dining featuring hand-rolled woodfired pizzas, fresh pasta, and romantic courtyard tables.",
                DressCode = "Casual / Family", Parking = "On-site Lot", Corkage = "R100 per bottle"
            },
            new RestaurantVO
            {
                Id = Guid.Empty,
                Slug = "the-botanical-table", Name = "The Botanical Table", Cuisine = "Farm-to-Table", PriceLevel = 3,
                Rating = 4.82m, ReviewsCount = 240,
                Location = "Kirstenbosch Estate", Address = "Rhodes Dr, Newlands, Cape Town, 7735",
                CrossStreet = "Kirstenbosch Gate 2",
                ImageUrl = "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?auto=format&fit=crop&w=800&q=80",
                Badge = "Sustainable Winner",
                Description = "Organic greenhouse cuisine surrounded by flora, featuring hyper-seasonal zero-waste tasting menus.",
                DressCode = "Smart Casual", Parking = "Estate Gate Parking", Corkage = "R120 per bottle",
                TableList = GetTableList(15)
            }
        };

        return restaurantList;
    }

    private IList<TableVO> GetTableList(int v)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(v);

        string[] zones = { "Indoor" , "Terrace" , "Bar" , "Private Room" };

        IList<TableVO> tables = new List<TableVO>(v);

        for (int i = 0 ; i < v ; i++)
        {
            int seats = Random.Shared.Next(2 , 9);

            tables.Add(new TableVO
            {
                Name = $"T{i + 1}" ,
                Seats = seats ,
                Capacity = seats ,
                Zone = zones[Random.Shared.Next(zones.Length)] ,
                Status = "Available"
            });
        }

        return tables;
    }
    public async Task Persist(RestaurantVO restaurant)
    {
        Validation(restaurant);

        await _dao.Persist(restaurant);
    }

    private void Validation(RestaurantVO restaurant)
    {

    }
}
