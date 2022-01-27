using BlendoBot.Core.Module;
using BlendoBot.Core.Services;
using BlendoBot.Module.UserTimeZone.Database;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlendoBot.Module.UserTimeZone;

[Module(Guid = "com.biendeo.blendobot.module.usertimezone", Name = "User TimeZone", Author = "Biendeo", Version = "2.0.0", Url = "https://github.com/BlendoBot/BlendoBot.Module.UserTimeZone")]
public class UserTimeZone : IModule {
	public UserTimeZone(IDiscordInteractor discordInteractor, IFilePathProvider filePathProvider, IModuleManager moduleManager) {
		DiscordInteractor = discordInteractor;
		FilePathProvider = filePathProvider;
		ModuleManager = moduleManager;

		UserTimeZoneCommand = new UserTimeZoneCommand(this);
	}

	internal ulong GuildId { get; private set; }

	internal readonly UserTimeZoneCommand UserTimeZoneCommand;
	internal readonly IDiscordInteractor DiscordInteractor;
	internal readonly IFilePathProvider FilePathProvider;
	internal readonly IModuleManager ModuleManager;

	public Task<bool> Startup(ulong guildId) {
		return Task.FromResult(ModuleManager.RegisterCommand(this, UserTimeZoneCommand, out _));
	}

	internal static TimeZoneInfo StringToCustomTimeZone(string s) {
		if (s.Contains(':')) {
			if (s.StartsWith('+')) {
				s = s[1..];
			}
			TimeSpan span = TimeSpan.Parse(s);
			string name = $"(UTC{TimeSpanOffsetToString(span)}) Custom TimeZone";
			return TimeZoneInfo.CreateCustomTimeZone(name, span, name, name);
		} else {
			return TimeZoneInfo.FindSystemTimeZoneById(s);
		}
	}

	public static string TimeZoneOffsetToString(TimeZoneInfo timezone) {
		return TimeSpanOffsetToString(timezone.BaseUtcOffset);
	}

	public static string TimeSpanOffsetToString(TimeSpan span) {
		return $"{(span.Hours >= 0 ? "+" : "")}{span.Hours.ToString().PadLeft(2, '0')}:{Math.Abs(span.Minutes).ToString().PadLeft(2, '0')}";
	}

	/// <summary>
	/// Returns the user time zone for a given user. It returns UTC if the user has not input a custom time zone.
	/// </summary>
	/// <param name="user"></param>
	/// <returns></returns>
	public TimeZoneInfo GetUserTimeZone(DiscordUser user) {
		using UserTimeZoneDbContext dbContext = UserTimeZoneDbContext.Get(this);
		UserSetting setting = dbContext.UserSettings.FirstOrDefault(s => s.UserId == user.Id);
		if (setting == null) {
			return TimeZoneInfo.Utc;
		} else {
			return setting.TimeZone;
		}
	}

	internal void SetUserTimeZone(DiscordUser user, TimeZoneInfo timeZone) {
		using UserTimeZoneDbContext dbContext = UserTimeZoneDbContext.Get(this);
		UserSetting setting = dbContext.UserSettings.FirstOrDefault(s => s.UserId == user.Id);
		if (setting == null) {
			dbContext.UserSettings.Add(new UserSetting {UserId = user.Id, TimeZone = timeZone});
		} else {
			setting.TimeZone = timeZone;
		}
		dbContext.SaveChanges();
	}

	public static string GetOffsetShortString(DateTime dateTime, TimeZoneInfo timeZone) {
		StringBuilder sb = new();
		TimeSpan offset = timeZone.GetUtcOffset(dateTime);
		if (offset.Hours >= 0) {
			sb.Append('+');
		} else {
			sb.Append('-');
		}

		sb.Append($"{Math.Abs(offset.Hours).ToString().PadLeft(2, '0')}:{Math.Abs(offset.Minutes).ToString().PadLeft(2, '0')}");

		return sb.ToString();
	}

	public string CommandTermWithPrefix => ModuleManager.GetCommandTermWithPrefix(UserTimeZoneCommand);
}
