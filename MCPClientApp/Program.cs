using ModelContextProtocol.Client;

// Start the local MCP server using stdio transport
IMcpClient client = await McpClientFactory.CreateAsync(
    new StdioClientTransport(new()
    {
        Command = "dotnet",
        Arguments = ["run", "--project", "../MCPHostApp/MCPHostApp.csproj"],
        Name = "Local MCP Server",
    }));

Console.WriteLine("Available tools:");
var tools = await client.ListToolsAsync();
foreach (var tool in tools)
{
    Console.WriteLine(tool);
}

