using BlendoBot.Core.Command;
using BlendoBot.Core.Entities;
using BlendoBot.Core.Module;
using BlendoBot.Core.Utility;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlendoBot.Module.UserTimeZone;

internal class UserTimeZoneCommand : ICommand {
	public UserTimeZoneCommand(UserTimeZone module) {
		this.module = module;
	}

	private readonly UserTimeZone module;
	public IModule Module => module;

	public string Guid => "usertimezone.command";
	public string DesiredTerm => "tz";
	public string Description => "Saves a timezone so that commands can represent times in your timezone";
	public Dictionary<string, string> Usage => new() {
		{ string.Empty, "Prints your currently set timezone." },
		{ "[timezone]", "Sets your timezone." },
		{ "Notes", $"A timezone can be provided as either an IANA ID (a full list of these IDs can be viewed at https://nodatime.org/TimeZones) (e.g. {"UTC".Code()}, {"Australia/Sydney".Code()}, {"US/Eastern".Code()}, etc.), or a custom offset from UTC, in the format of {"+3:00".Code()}, {"10:00".Code()}, {"-4:30".Code()} etc. IANA timezones automatically support daylight savings." }
	};

	public async Task OnMessage(MessageCreateEventArgs e, string[] tokenizedMessage) {
		if (tokenizedMessage.Length == 0) {
			TimeZoneInfo timezone = module.GetUserTimeZone(e.Author);
			await module.DiscordInteractor.Send(this, new SendEventArgs {
				Message = $"Your timezone is currently set to {(timezone.IsDaylightSavingTime(DateTime.UtcNow) ? timezone.DaylightName : timezone.StandardName).Code()}",
				Channel = e.Channel,
				Tag = "UserTimezoneList"
			});
		} else if (tokenizedMessage.Length == 1) {
			try {
				TimeZoneInfo timezone = UserTimeZone.StringToCustomTimeZone(tokenizedMessage[0]);
				module.SetUserTimeZone(e.Author, timezone);
				await module.DiscordInteractor.Send(this, new SendEventArgs {
					Message = $"Your timezone is now {(timezone.IsDaylightSavingTime(DateTime.UtcNow) ? timezone.DaylightName : timezone.StandardName).Code()}",
					Channel = e.Channel,
					Tag = "UserTimezoneSet"
				});
			} catch (FormatException) {
				await module.DiscordInteractor.Send(this, new SendEventArgs {
					Message = $"Could not parse your timezone offset of {tokenizedMessage[0]}. Make sure it follows a valid format (i.e. {"+3:00".Code()}, {"10:00".Code()}, {"-4:30".Code()}, etc.).",
					Channel = e.Channel,
					Tag = "UserTimezoneSetError"
				});
			} catch (ArgumentOutOfRangeException) {
				await module.DiscordInteractor.Send(this, new SendEventArgs {
					Message = $"Your timezone does not exist, make sure it is between {"-14:00".Code()} and {"+14:00".Code()}.",
					Channel = e.Channel,
					Tag = "UserTimezoneSetError"
				});
			} catch (TimeZoneNotFoundException) {
				await module.DiscordInteractor.Send(this, new SendEventArgs {
					Message = $"Could not find a timezone that matched {tokenizedMessage[0].Code()}. Please use a valid time zone ID (case-sensitive) from this page: https://nodatime.org/TimeZones",
					Channel = e.Channel,
					Tag = "UserTimezoneSetErrorAmbiguous"
				});
			}
		} else {
			await module.DiscordInteractor.Send(this, new SendEventArgs {
				Message = $"Too many arguments! Please refer to {$"{module.ModuleManager.GetHelpTermForCommand(this)} usertimezone".Code()} for usage information.",
				Channel = e.Channel,
				Tag = "UserTimezoneTooManyArguments"
			});
		}
	}
}
