using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlendoBot.Module.UserTimeZone.Database;

internal class UserSetting {
	public ulong UserId { get; set; }

	private string timeZone;
	[NotMapped]
	public TimeZoneInfo TimeZone {
		get => TimeZoneInfo.FromSerializedString(timeZone);
		set => timeZone = value.ToSerializedString();
	}
}
