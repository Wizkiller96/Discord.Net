using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.Commands;

public class Program
{
	private CommandService _commands;
	private DiscordSocketClient _client;

	// Reroute our Main Method to run Start, and block until Start finishes.
	static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

	public async Task Start()
	{
		_client = new DiscordSocketClient();
		_commands = new CommandService();

		var token = "bot token here";

		await InstallCommands();

		await _client.LoginAsync(TokenType.Bot, token);
		await _client.ConnectAsync();

		// Block this task until the bot is exited.
		await Task.Delay(-1);
	}

	public async Task InstallCommands()
	{
		// Hook the MessageReceived Event into our Command Handler
		_client.MessageReceived += HandleCommand;
		// Discover all of the commands in this assembly and load them.
		await _commands.LoadAssembly(Assembly.GetEntryAssembly());
	}
	public async Task HandleCommand(IMessage msg)
	{
		// Internal integer, marks where the command begins
		int argPos = 0;
		// Get the current user (used for Mention parsing)
		var currentUser = await client.GetCurrentUserAsync();
		// Determine if the message is a command, based on if it starts with '!' or a mention prefix
		if (msg.HasCharPrefix('!', ref argPos) || msg.HasMentionPrefix(currentUser, ref argPos)) 
		{
			// Execute the command. (result does not indicate a return value, 
			// rather an object stating if the command executed succesfully)
			var result = await _commands.Execute(msg, argPos);
			if (!result.IsSuccess)
				await msg.Channel.SendMessageAsync(result.ErrorReason);
		}
	}
}