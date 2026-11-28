using FluentNHibernate.Mapping;

namespace Data.VO.Customer;

/// <summary>
/// Guest who makes a booking.
/// Source: bookingState guest fields in index4.html.
/// </summary>
public class CustomerVO : EntityBase
{
    public virtual string Name { get; set; } = string.Empty;
    public virtual string Phone { get; set; } = string.Empty;
    public virtual string Email { get; set; } = string.Empty;
    public virtual string Notes { get; set; } = string.Empty;
}

public class CustomerMap : EntityWithIdMapping<CustomerVO>
{
    public CustomerMap()
    {
        Map(x => x.Name).Length(int.MaxValue).Not.Nullable();
        Map(x => x.Phone).Length(int.MaxValue);
        Map(x => x.Email).Length(int.MaxValue).Not.Nullable();
        Map(x => x.Notes).Length(int.MaxValue);
    }
}
