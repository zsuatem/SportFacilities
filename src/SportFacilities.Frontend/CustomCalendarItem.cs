using Heron.MudCalendar;
using MudBlazor;

namespace SportFacilities.Frontend;

public class CustomCalendarItem : CalendarItem
{
    public string Title { get; set; } = string.Empty;
    public Color Color { get; set; } = Color.Primary;
}
