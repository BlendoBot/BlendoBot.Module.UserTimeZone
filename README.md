# BlendoBot.Module.UserTimeZone
## Description
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/BlendoBot/BlendoBot.Module.UserTimeZone/Tests)

The usertimezone command allows users to set their own timezone, which other modules can then use to display information in a local time without needing to ask every time!

## Discord Usage
- `?usertimezone`
  - Prints the user's timezone information that they've defined (defaults to UTC).
- `?usertimezone [timezone]`
  - Sets the timezone.

Timezones can be supplied in two formats:
- Users can type a UTC offset between the range of `-14:00` to `+14:00`. Examples are `+9:30`, `-8:00`, etc.
- Users can also type in an IANA ID (a list can be found at https://nodatime.org/TimeZones), which will automatically handle daylight savings when appropriate. Examples are `Australia/Sydney`, `America/Los_Angeles`, etc.