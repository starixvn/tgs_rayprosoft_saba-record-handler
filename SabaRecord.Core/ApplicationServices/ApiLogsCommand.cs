namespace SabaRecord.Core.ApplicationServices;

public record ApiLogsCommand(string Key, string Action, string Req, string Res);